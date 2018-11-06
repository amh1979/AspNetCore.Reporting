using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class Word97Writer : IWordWriter, IDisposable
	{
		internal enum HeaderFooterLocation
		{
			First,
			Odd,
			Even
		}

		private class ListLevelInfo
		{
			internal RPLFormat.ListStyles? Style = null;

			internal int ListIndex = -1;

			public void Reset()
			{
				this.Style = null;
				this.ListIndex = -1;
			}
		}

		private const int EmptyStyleIndex = 16;

		private int m_fcStart = 1024;

		private Stream m_fontTable;

		private Stream m_tableStream;

		private Stream m_mainStream;

		private Stream m_dataStream;

		private Stream m_listStream;

		private Stream m_listLevelStream;

		private Stream m_outStream;

		private Dictionary<ImageHash, int> m_images;

		private Dictionary<string, int> m_fontNameSet;

		private int m_currentFontIndex;

		private TableData m_currentRow;

		private int[] m_headerFooterOffsets;

		private int m_ccpText;

		private int m_ccpHdd;

		private int m_nestingLevel;

		private Stack<TableData> m_tapStack;

		private FieldsTable m_fldsCurrent;

		private FieldsTable m_fldsMain;

		private FieldsTable m_fldsHdr;

		private Bookmarks m_bookmarks;

		private CharacterFormat m_charFormat;

		private ParagraphFormat m_parFormat;

		private SectionFormat m_secFormat;

		private WordText m_wordText;

		private int m_imgIndex;

		private int m_listIndex;

		private ListData m_currentList;

		private AutoFit m_autoFit = AutoFit.Default;

		private ListLevelInfo[] m_levelData = new ListLevelInfo[9];

		private int m_currentMaxListLevel;

		public bool CanBand
		{
			get
			{
				return this.m_nestingLevel > 1;
			}
		}

		public AutoFit AutoFit
		{
			get
			{
				return this.m_autoFit;
			}
			set
			{
				this.m_autoFit = value;
			}
		}

		public int SectionCount
		{
			get
			{
				return this.m_secFormat.SectionCount;
			}
		}

		public bool HasTitlePage
		{
			set
			{
			}
		}

		public Word97Writer()
		{
			this.InitListLevels();
		}

		public void Init(CreateAndRegisterStream createAndRegisterStream, AutoFit autoFit, string reportName)
		{
			this.m_outStream = createAndRegisterStream(reportName, "doc", null, "application/msword", false, StreamOper.CreateAndRegister);
			this.m_tableStream = createAndRegisterStream("TableStream", null, null, null, true, StreamOper.CreateOnly);
			this.m_mainStream = createAndRegisterStream("WordDocument", null, null, null, true, StreamOper.CreateOnly);
			this.m_fontTable = createAndRegisterStream("FontTable", null, null, null, true, StreamOper.CreateOnly);
			this.m_dataStream = createAndRegisterStream("Data", null, null, null, true, StreamOper.CreateOnly);
			this.m_listStream = createAndRegisterStream("List", null, null, null, true, StreamOper.CreateOnly);
			this.m_listLevelStream = createAndRegisterStream("ListLevel", null, null, null, true, StreamOper.CreateOnly);
			Stream textPiece = createAndRegisterStream("TextPiece", null, null, null, true, StreamOper.CreateOnly);
			Stream chpTable = createAndRegisterStream("ChpTable", null, null, null, true, StreamOper.CreateOnly);
			Stream papTable = createAndRegisterStream("PapTable", null, null, null, true, StreamOper.CreateOnly);
			this.m_charFormat = new CharacterFormat(chpTable, this.m_fcStart);
			this.m_parFormat = new ParagraphFormat(papTable, this.m_fcStart);
			this.m_wordText = new WordText(textPiece);
			this.m_secFormat = new SectionFormat();
			this.m_currentRow = new TableData(1, true);
			this.m_tapStack = new Stack<TableData>();
			this.m_fontNameSet = new Dictionary<string, int>();
			this.WriteFont("Times New Roman");
			this.WriteFont("Symbol");
			this.WriteFont("Arial");
			this.m_imgIndex = 0;
			this.m_fldsMain = new FieldsTable();
			this.m_fldsHdr = new FieldsTable();
			this.m_fldsCurrent = this.m_fldsMain;
			this.m_bookmarks = new Bookmarks();
			this.m_images = new Dictionary<ImageHash, int>();
			this.m_autoFit = autoFit;
		}

		public void SetPageDimensions(float pageHeight, float pageWidth, float leftMargin, float rightMargin, float topMargin, float bottomMargin)
		{
			if (pageWidth > 558.79998779296875)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The maximum page width exceeded:{0}", pageWidth);
				pageWidth = 558.8f;
			}
			if (Word97Writer.FixMargins(pageHeight, ref topMargin, ref bottomMargin))
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The top or bottom margin is either <0 or the sum exceeds the page height.");
			}
			if (pageHeight > 558.79998779296875)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "The maximum page height exceeded:{0}", pageHeight);
				pageHeight = 558.8f;
			}
			this.m_secFormat.AddSprm(45088, Word97Writer.ToTwips(pageHeight), null);
			this.m_secFormat.AddSprm(45087, Word97Writer.ToTwips(pageWidth), null);
			this.m_secFormat.AddSprm(45089, Word97Writer.ToTwips(leftMargin), null);
			this.m_secFormat.AddSprm(45090, Word97Writer.ToTwips(rightMargin), null);
			this.m_secFormat.AddSprm(36899, Word97Writer.ToTwips(topMargin), null);
			this.m_secFormat.AddSprm(36900, Word97Writer.ToTwips(bottomMargin), null);
			if (pageWidth > pageHeight)
			{
				this.m_secFormat.AddSprm(12317, 2, null);
			}
			this.m_secFormat.AddSprm(45079, 0, null);
			this.m_secFormat.AddSprm(45080, 0, null);
		}

		public void InitHeaderFooter()
		{
			this.m_ccpText = this.m_wordText.CurrentCp;
			this.m_headerFooterOffsets = new int[14 + 6 * this.m_secFormat.SectionCount];
			this.m_fldsCurrent = this.m_fldsHdr;
		}

		public void FinishHeader(int section)
		{
			this.FinishHeader(section, HeaderFooterLocation.Odd);
		}

		public void FinishFooter(int section)
		{
			this.FinishFooter(section, HeaderFooterLocation.Odd);
		}

		public void FinishHeader(int section, HeaderFooterLocation location)
		{
			int index = 0;
			switch (location)
			{
			case HeaderFooterLocation.Even:
				index = 7;
				break;
			case HeaderFooterLocation.Odd:
				index = 8;
				break;
			case HeaderFooterLocation.First:
				index = 11;
				break;
			}
			this.FinishHeaderFooterRegion(section, index);
		}

		public void FinishFooter(int section, HeaderFooterLocation location)
		{
			int index = 0;
			switch (location)
			{
			case HeaderFooterLocation.Even:
				index = 9;
				break;
			case HeaderFooterLocation.Odd:
				index = 10;
				break;
			case HeaderFooterLocation.First:
				index = 12;
				break;
			}
			this.FinishHeaderFooterRegion(section, index);
		}

		private void FinishHeaderFooterRegion(int section, int index)
		{
			this.WriteParagraphEnd();
			int num = this.m_wordText.CurrentCp - this.m_ccpText;
			index += section * 6;
			for (int i = index; i < this.m_headerFooterOffsets.Length - 1; i++)
			{
				this.m_headerFooterOffsets[i] = num;
			}
			this.m_headerFooterOffsets[this.m_headerFooterOffsets.Length - 1] = num + 3;
		}

		public void FinishHeadersFooters(bool hasTitlePage)
		{
			this.WriteParagraphEnd();
			this.m_ccpHdd = this.m_wordText.CurrentCp - this.m_ccpText;
			this.WriteParagraphEnd();
			if (hasTitlePage)
			{
				this.m_secFormat.UseTitlePage = true;
			}
		}

		public void AddImage(byte[] imgBuf, float height, float width, RPLFormat.Sizings sizing)
		{
			if (imgBuf == null || imgBuf.Length == 0)
			{
				sizing = RPLFormat.Sizings.Clip;
				imgBuf = PictureDescriptor.INVALIDIMAGEDATA;
			}
			OfficeImageHasher officeImageHasher = new OfficeImageHasher(imgBuf);
			byte[] hash = officeImageHasher.Hash;
			int num = Word97Writer.ToTwips(height);
			int num2 = Word97Writer.ToTwips(width);
			int num3 = (int)this.m_dataStream.Position;
			ImageHash key = new ImageHash(hash, sizing, num2, num);
			if (this.m_images.ContainsKey(key))
			{
				num3 = this.m_images[key];
			}
			else
			{
				PictureDescriptor pictureDescriptor = new PictureDescriptor(imgBuf, hash, num2, num, sizing, this.m_imgIndex);
				pictureDescriptor.Serialize(this.m_dataStream);
				this.m_imgIndex++;
				this.m_images.Add(key, num3);
			}
			this.m_charFormat.SetIsInlineImage(num3);
			this.WriteSpecialText("\u0001");
		}

		private void WriteSpecialText(string text)
		{
			int currentCp = this.m_wordText.CurrentCp;
			this.m_wordText.WriteSpecialText(text);
			this.m_charFormat.CommitLastCharacterRun(currentCp, this.m_wordText.CurrentCp);
		}

		public void WriteText(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				int currentCp = this.m_wordText.CurrentCp;
				this.m_wordText.WriteText(text);
				this.m_charFormat.CommitLastCharacterRun(currentCp, this.m_wordText.CurrentCp);
			}
		}

		public void WriteHyperlinkBegin(string target, bool bookmarkLink)
		{
			HyperlinkWriter.LinkType type = HyperlinkWriter.LinkType.Bookmark;
			if (!bookmarkLink)
			{
				Uri uri = new Uri(target);
				if (uri.IsFile)
				{
					type = HyperlinkWriter.LinkType.File;
					target = uri.LocalPath;
				}
				else
				{
					type = HyperlinkWriter.LinkType.Hyperlink;
				}
			}
			this.m_charFormat.Push(15);
			this.m_fldsCurrent.Add(new HyperlinkFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.Start));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText('\u0013'.ToString());
			string text = " HYPERLINK " + (bookmarkLink ? "\\l" : "") + "\"" + target + "\" ";
			this.WriteSpecialText(text);
			string text2 = "\u0001";
			this.m_charFormat.AddSprm(2133, 1, null);
			this.m_charFormat.AddSprm(2050, 1, null);
			this.m_charFormat.AddSprm(2054, 1, null);
			this.m_charFormat.AddSprm(27139, (int)this.m_dataStream.Length, null);
			this.WriteSpecialText(text2);
			HyperlinkWriter.WriteHyperlink(this.m_dataStream, target, type);
			this.m_fldsCurrent.Add(new HyperlinkFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.Middle));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText('\u0014'.ToString());
			this.m_charFormat.Pop();
		}

		public void WriteHyperlinkEnd()
		{
			this.m_fldsCurrent.Add(new HyperlinkFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.End));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText('\u0015'.ToString());
		}

		public void AddTableStyleProp(byte code, object value)
		{
			if (value != null)
			{
				this.m_currentRow.AddTableStyleProp(code, value);
			}
		}

		public void SetTableContext(BorderContext borderContext)
		{
			this.m_currentRow.SetTableContext(borderContext);
		}

		public void AddBodyStyleProp(byte code, object value)
		{
			switch (code)
			{
			}
		}

		public void AddCellStyleProp(int cellIndex, byte code, object value)
		{
			if (value != null)
			{
				this.m_currentRow.AddCellStyleProp(cellIndex, code, value);
			}
		}

		public void AddPadding(int cellIndex, byte code, object value, int defaultValue)
		{
			this.m_currentRow.AddPadding(cellIndex, code, value, defaultValue);
		}

		public void ApplyCellBorderContext(BorderContext borderContext)
		{
		}

		public void AddTextStyleProp(byte code, object value)
		{
			if (value != null)
			{
				switch (code)
				{
				case 23:
				case 25:
				case 26:
				case 30:
				case 33:
				case 34:
				case 35:
				case 36:
				case 37:
					break;
				case 24:
					this.RenderTextDecoration((RPLFormat.TextDecorations)value);
					break;
				case 27:
					if (value is string)
					{
						this.RenderTextColor((string)value);
					}
					else if (value is Color)
					{
						this.RenderTextColor((Color)value);
					}
					break;
				case 28:
					this.RenderLineHeight(value as string);
					break;
				case 29:
					this.RenderDirection((RPLFormat.Directions)value);
					break;
				case 31:
					this.RenderUnicodeBiDi((RPLFormat.UnicodeBiDiTypes)value);
					break;
				case 32:
					this.RenderLanguage(value as string);
					break;
				}
			}
		}

		public void AddFirstLineIndent(float indent)
		{
			int param = this.PointsToTwips(indent);
			this.m_parFormat.AddSprm(ParagraphSprms.SPRM_DXALEFT12k3, param, null);
		}

		public void AddLeftIndent(float margin)
		{
			int param = this.PointsToTwips(margin);
			this.m_parFormat.AddSprm(ParagraphSprms.SPRM_DXALEFT2k3, param, null);
		}

		public void AddRightIndent(float margin)
		{
			int param = this.PointsToTwips(margin);
			this.m_parFormat.AddSprm(ParagraphSprms.SPRM_DXARIGHT2k3, param, null);
		}

		public void AddSpaceBefore(float space)
		{
			int param = this.PointsToTwips(space);
			this.m_parFormat.AddSprm(ParagraphSprms.SPRM_DYABEFORE, param, null);
		}

		public void AddSpaceAfter(float space)
		{
			int param = this.PointsToTwips(space);
			this.m_parFormat.AddSprm(ParagraphSprms.SPRM_DYAAFTER, param, null);
		}

		public static bool FixMargins(float totalSize, ref float left, ref float right)
		{
			if (left < 0.0)
			{
				left = 0f;
			}
			if (right < 0.0)
			{
				right = 0f;
			}
			if (left + right >= totalSize)
			{
				left = 0f;
				right = 0f;
				return true;
			}
			return false;
		}

		private int PointsToTwips(float indent)
		{
			return (int)(indent * 20.0);
		}

		private void RenderTextDecoration(RPLFormat.TextDecorations textDecorations)
		{
			switch (textDecorations)
			{
			case RPLFormat.TextDecorations.Overline:
				break;
			case RPLFormat.TextDecorations.LineThrough:
				this.m_charFormat.AddSprm(2103, 1, null);
				break;
			case RPLFormat.TextDecorations.Underline:
				this.m_charFormat.AddSprm(10814, 1, null);
				break;
			}
		}

		private void RenderLanguage(string p)
		{
		}

		private void RenderUnicodeBiDi(RPLFormat.UnicodeBiDiTypes unicodeBiDiTypes)
		{
		}

		private void RenderDirection(RPLFormat.Directions directions)
		{
			int param = (directions == RPLFormat.Directions.RTL) ? 1 : 0;
			this.m_parFormat.AddSprm(9281, param, null);
		}

		public void RenderTextRunDirection(RPLFormat.Directions direction)
		{
			int param = (direction == RPLFormat.Directions.RTL) ? 1 : 0;
			this.m_charFormat.AddSprm(2138, param, null);
		}

		private void RenderLineHeight(string size)
		{
			RPLReportSize rPLReportSize = new RPLReportSize(size);
			int param = Word97Writer.ToTwips((float)rPLReportSize.ToMillimeters());
			this.m_parFormat.AddSprm(25618, param, null);
		}

		private void RenderTextColor(string strColor)
		{
			RPLReportColor rPLReportColor = new RPLReportColor(strColor);
			Color color = rPLReportColor.ToColor();
			this.RenderTextColor(color);
		}

		private void RenderTextColor(Color color)
		{
			int param = color.B << 16 | color.G << 8 | color.R;
			this.m_charFormat.AddSprm(26736, param, null);
		}

		private bool GetTextAlignForType(TypeCode typeCode)
		{
			bool flag = false;
			switch (typeCode)
			{
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				return true;
			default:
				return false;
			}
		}

		public void RenderTextAlign(TypeCode type, RPLFormat.TextAlignments textAlignments, RPLFormat.Directions direction)
		{
			int param = 0;
			if (textAlignments == RPLFormat.TextAlignments.General)
			{
				textAlignments = (RPLFormat.TextAlignments)((!this.GetTextAlignForType(type)) ? 1 : 3);
			}
			else if (direction == RPLFormat.Directions.RTL)
			{
				switch (textAlignments)
				{
				case RPLFormat.TextAlignments.Left:
					textAlignments = RPLFormat.TextAlignments.Right;
					break;
				case RPLFormat.TextAlignments.Right:
					textAlignments = RPLFormat.TextAlignments.Left;
					break;
				}
			}
			switch (textAlignments)
			{
			case RPLFormat.TextAlignments.Left:
				param = 0;
				break;
			case RPLFormat.TextAlignments.Center:
				param = 1;
				break;
			case RPLFormat.TextAlignments.Right:
				param = 2;
				break;
			}
			this.m_parFormat.AddSprm(9313, param, null);
		}

		public void RenderFontWeight(RPLFormat.FontWeights fontWeights, RPLFormat.Directions dir)
		{
			if ((int)fontWeights >= 5)
			{
				this.m_charFormat.AddSprm(2101, 1, null);
				if (dir == RPLFormat.Directions.RTL)
				{
					this.m_charFormat.AddSprm(2140, 1, null);
				}
			}
			else
			{
				this.m_charFormat.AddSprm(2101, 0, null);
				if (dir == RPLFormat.Directions.RTL)
				{
					this.m_charFormat.AddSprm(2140, 0, null);
				}
			}
		}

		public void RenderFontWeight(RPLFormat.FontWeights? fontWeights, RPLFormat.Directions dir)
		{
			if (fontWeights.HasValue)
			{
				this.RenderFontWeight(fontWeights.Value, dir);
			}
		}

		public void RenderFontSize(string size, RPLFormat.Directions dir)
		{
			RPLReportSize rPLReportSize = new RPLReportSize(size);
			double num = rPLReportSize.ToPoints();
			int param = (int)Math.Round(num * 2.0);
			this.m_charFormat.AddSprm(19011, param, null);
			if (dir == RPLFormat.Directions.RTL)
			{
				this.m_charFormat.AddSprm(19041, param, null);
			}
		}

		public void RenderFontFamily(string font, RPLFormat.Directions dir)
		{
			int param = this.WriteFont(font);
			this.m_charFormat.AddSprm(19023, param, null);
			this.m_charFormat.AddSprm(19024, param, null);
			this.m_charFormat.AddSprm(19025, param, null);
			if (dir == RPLFormat.Directions.RTL)
			{
				this.m_charFormat.AddSprm(19038, param, null);
			}
		}

		public void RenderFontStyle(RPLFormat.FontStyles value, RPLFormat.Directions dir)
		{
			if (value == RPLFormat.FontStyles.Italic)
			{
				this.m_charFormat.AddSprm(2102, 1, null);
				if (dir == RPLFormat.Directions.RTL)
				{
					this.m_charFormat.AddSprm(2141, 1, null);
				}
			}
			else
			{
				this.m_charFormat.AddSprm(2102, 0, null);
				if (dir == RPLFormat.Directions.RTL)
				{
					this.m_charFormat.AddSprm(2141, 0, null);
				}
			}
		}

		public void RenderFontStyle(RPLFormat.FontStyles? value, RPLFormat.Directions dir)
		{
			if (value.HasValue)
			{
				this.RenderFontStyle(value.Value, dir);
			}
		}

		public void WriteParagraphEnd()
		{
			this.WriteCurrentListData();
			this.WriteParagraphEnd("\r", false);
		}

		public void WriteListEnd(int level, RPLFormat.ListStyles listStyle, bool endParagraph)
		{
			int listIndex = this.m_listIndex;
			for (int i = level; i < this.m_currentMaxListLevel; i++)
			{
				this.m_levelData[i].Reset();
			}
			if (this.m_levelData[level - 1].Style == listStyle)
			{
				listIndex = this.m_levelData[level - 1].ListIndex;
			}
			else
			{
				if (this.m_currentList != null)
				{
					this.WriteCurrentListData();
				}
				this.m_currentList = new ListData(++this.m_listIndex);
				listIndex = this.m_listIndex;
				this.m_currentList.SetLevel(level - 1, new ListLevelOnFile(level - 1, listStyle, this));
				this.m_levelData[level - 1].Style = listStyle;
				this.m_levelData[level - 1].ListIndex = listIndex;
				this.m_currentMaxListLevel = level;
			}
			this.m_parFormat.AddSprm(17931, listIndex, null);
			this.m_parFormat.AddSprm(9738, level - 1, null);
			if (endParagraph)
			{
				this.WriteParagraphEnd("\r", false);
			}
		}

		public void InitListLevels()
		{
			int num = this.m_levelData.Length;
			for (int i = 0; i < num; i++)
			{
				this.m_levelData[i] = new ListLevelInfo();
			}
		}

		public void ResetListlevels()
		{
			for (int i = 0; i < this.m_currentMaxListLevel; i++)
			{
				this.m_levelData[i].Reset();
			}
			this.m_currentMaxListLevel = 0;
		}

		public void WriteTableCellEnd(int cellIndex, BorderContext borderContext, bool emptyLayoutCell)
		{
			this.m_currentRow.WriteTableCellEnd(cellIndex, borderContext);
			string parEnd = "\a";
			if (this.m_nestingLevel > 1)
			{
				this.m_parFormat.AddSprm(9291, 1, null);
				parEnd = "\r";
			}
			if (emptyLayoutCell)
			{
				this.m_parFormat.StyleIndex = 16;
			}
			this.WriteCurrentListData();
			this.WriteParagraphEnd(parEnd, false);
		}

		public void WriteEmptyStyle()
		{
			this.m_parFormat.StyleIndex = 16;
		}

		public void WriteTableBegin(float left, bool layoutTable)
		{
			if (this.m_nestingLevel > 0)
			{
				this.m_tapStack.Push(this.m_currentRow);
			}
			this.m_currentRow = new TableData(this.m_nestingLevel + 1, layoutTable);
			this.m_nestingLevel++;
		}

		public void WriteTableRowBegin(float left, float height, float[] columnWidths)
		{
			this.m_currentRow.InitTableRow(left, height, columnWidths, this.m_autoFit);
		}

		public void IgnoreRowHeight(bool canGrow)
		{
			this.m_currentRow.WriteRowHeight = !canGrow;
		}

		public void SetWriteExactRowHeight(bool writeExactRowHeight)
		{
			this.m_currentRow.WriteExactRowHeight = writeExactRowHeight;
		}

		public void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge)
		{
			this.m_currentRow.WriteTableCellBegin(cellIndex, numColumns, firstVertMerge, firstHorzMerge, vertMerge, horzMerge);
		}

		public void WriteTableRowEnd()
		{
			if (this.m_nestingLevel > 1)
			{
				this.WriteParagraphEnd("\r", true);
			}
			else
			{
				this.WriteParagraphEnd("\a", true);
			}
		}

		public void WriteTableEnd()
		{
			if (this.m_nestingLevel > 1)
			{
				this.m_currentRow = this.m_tapStack.Pop();
			}
			this.m_nestingLevel--;
		}

		public void Finish(string title, string author, string comments)
		{
			this.m_charFormat.Finish(this.m_wordText.CurrentCp);
			this.m_parFormat.Finish(this.m_wordText.CurrentCp);
			byte[] array = new byte[FileInformationBlock.FieldsSize];
			BinaryWriter binaryWriter = new BinaryWriter(this.m_tableStream);
			BinaryWriter binaryWriter2 = new BinaryWriter(this.m_mainStream);
			binaryWriter2.Write(FileInformationBlock.StartBuffer, 0, FileInformationBlock.StartBuffer.Length);
			binaryWriter2.Write(new byte[2], 0, 2);
			binaryWriter2.Write(array);
			binaryWriter2.Write(FileInformationBlock.EndBuffer, 0, FileInformationBlock.EndBuffer.Length);
			binaryWriter.Flush();
			int num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, this.GetFcField(33), num);
			this.m_wordText.WriteClxTo(binaryWriter, this.m_fcStart);
			LittleEndian.PutInt(array, this.GetLcbField(33), (int)binaryWriter.BaseStream.Position - num);
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, this.GetFcField(1), num);
			binaryWriter.Write(StyleSheet.Buffer, 0, StyleSheet.Buffer.Length);
			binaryWriter.Flush();
			LittleEndian.PutInt(array, this.GetLcbField(1), (int)binaryWriter.BaseStream.Position - num);
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, this.GetFcField(31), num);
			binaryWriter.Write(DocumentProperties.Buffer, 0, DocumentProperties.Buffer.Length);
			binaryWriter.Flush();
			LittleEndian.PutInt(array, this.GetLcbField(31), (int)binaryWriter.BaseStream.Position - num);
			binaryWriter2.Flush();
			this.TransferData(this.m_wordText.Stream, binaryWriter2.BaseStream, 4096);
			int num2 = (int)binaryWriter2.BaseStream.Position;
			int num3 = 512 - num2 % 512;
			binaryWriter2.Write(new byte[num3], 0, num3);
			binaryWriter2.Flush();
			int num4 = (int)binaryWriter2.BaseStream.Position / 512;
			this.TransferData(this.m_charFormat.Stream, binaryWriter2.BaseStream, 4096);
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, this.GetFcField(12), num);
			this.m_charFormat.WriteBinTableTo(binaryWriter, ref num4);
			binaryWriter.Flush();
			LittleEndian.PutInt(array, this.GetLcbField(12), (int)binaryWriter.BaseStream.Position - num);
			this.TransferData(this.m_parFormat.Stream, binaryWriter2.BaseStream, 4096);
			binaryWriter.Flush();
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, this.GetFcField(13), num);
			this.m_parFormat.WriteBinTableTo(binaryWriter, ref num4);
			binaryWriter.Flush();
			LittleEndian.PutInt(array, this.GetLcbField(13), (int)binaryWriter.BaseStream.Position - num);
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, this.GetFcField(6), num);
			this.m_secFormat.WriteTo(binaryWriter, binaryWriter2, this.m_wordText.CurrentCp);
			LittleEndian.PutInt(array, this.GetLcbField(6), (int)binaryWriter.BaseStream.Position - num);
			if (this.m_fldsMain.Size > 0)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(16), num);
				LittleEndian.PutInt(array, this.GetLcbField(16), this.m_fldsMain.Size);
				this.m_fldsMain.WriteTo(binaryWriter, 0, this.m_wordText.CurrentCp);
				binaryWriter.Flush();
			}
			if (this.m_fldsHdr.Size > 0)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(17), num);
				LittleEndian.PutInt(array, this.GetLcbField(17), this.m_fldsHdr.Size);
				this.m_fldsHdr.WriteTo(binaryWriter, this.m_ccpText, this.m_wordText.CurrentCp);
				binaryWriter.Flush();
			}
			if (this.m_headerFooterOffsets != null)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(11), num);
				LittleEndian.PutInt(array, this.GetLcbField(11), this.m_headerFooterOffsets.Length * 4);
				for (int i = 0; i < this.m_headerFooterOffsets.Length; i++)
				{
					binaryWriter.Write(this.m_headerFooterOffsets[i]);
				}
				binaryWriter.Flush();
			}
			if (this.m_bookmarks.Count > 0)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(22), num);
				this.m_bookmarks.SerializeStarts(binaryWriter, this.m_wordText.CurrentCp);
				LittleEndian.PutInt(array, this.GetLcbField(22), (int)binaryWriter.BaseStream.Position - num);
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(23), num);
				this.m_bookmarks.SerializeEnds(binaryWriter, this.m_wordText.CurrentCp);
				LittleEndian.PutInt(array, this.GetLcbField(23), (int)binaryWriter.BaseStream.Position - num);
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(21), num);
				this.m_bookmarks.SerializeNames(binaryWriter);
				LittleEndian.PutInt(array, this.GetLcbField(21), (int)binaryWriter.BaseStream.Position - num);
			}
			if (this.m_listIndex > 0)
			{
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(73), num);
				binaryWriter.Write((short)this.m_listIndex);
				binaryWriter.Flush();
				this.TransferData(this.m_listStream, binaryWriter.BaseStream, (int)this.m_listStream.Length);
				this.TransferData(this.m_listLevelStream, binaryWriter.BaseStream, (int)this.m_listLevelStream.Length);
				LittleEndian.PutInt(array, this.GetLcbField(73), (int)binaryWriter.BaseStream.Position - num);
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(74), num);
				binaryWriter.Write(this.m_listIndex);
				binaryWriter.Flush();
				this.WriteListFormatOverrides(binaryWriter);
				LittleEndian.PutInt(array, this.GetLcbField(74), (int)binaryWriter.BaseStream.Position - num);
				num = (int)binaryWriter.BaseStream.Position;
				LittleEndian.PutInt(array, this.GetFcField(91), num);
				this.WriteListNameTable(binaryWriter);
				LittleEndian.PutInt(array, this.GetLcbField(91), (int)binaryWriter.BaseStream.Position - num);
			}
			num = (int)binaryWriter.BaseStream.Position;
			LittleEndian.PutInt(array, this.GetFcField(15), num);
			binaryWriter.Write((short)this.m_currentFontIndex);
			binaryWriter.Write((short)0);
			binaryWriter.Flush();
			this.TransferData(this.m_fontTable, binaryWriter.BaseStream, (int)this.m_fontTable.Length);
			LittleEndian.PutInt(array, this.GetLcbField(15), (int)binaryWriter.BaseStream.Position - num);
			binaryWriter2.Flush();
			binaryWriter2.BaseStream.Seek(24L, SeekOrigin.Begin);
			binaryWriter2.Write(this.m_fcStart);
			binaryWriter2.Write(num2);
			binaryWriter2.Flush();
			binaryWriter2.BaseStream.Seek(64L, SeekOrigin.Begin);
			binaryWriter2.Write((int)binaryWriter2.BaseStream.Length);
			binaryWriter2.Flush();
			binaryWriter2.BaseStream.Seek(76L, SeekOrigin.Begin);
			binaryWriter2.Write((this.m_ccpText > 0) ? this.m_ccpText : this.m_wordText.CurrentCp);
			binaryWriter2.Flush();
			if (this.m_ccpText > 0)
			{
				binaryWriter2.BaseStream.Seek(84L, SeekOrigin.Begin);
				binaryWriter2.Write(this.m_ccpHdd);
				binaryWriter2.Flush();
			}
			int val = 0;
			for (int j = 0; j < array.Length; j += 8)
			{
				int @int = LittleEndian.getInt(array, j);
				int int2 = LittleEndian.getInt(array, j + 4);
				if (int2 == 0)
				{
					LittleEndian.PutInt(array, j, val);
				}
				else
				{
					val = @int + int2;
				}
			}
			binaryWriter2.BaseStream.Seek(FileInformationBlock.StartBuffer.Length, SeekOrigin.Begin);
			binaryWriter2.Write((short)(array.Length / 8));
			binaryWriter2.Write(array, 0, array.Length);
			binaryWriter2.Flush();
			StructuredStorage.CreateMultiStreamFile(new Stream[3]
			{
				this.m_mainStream,
				this.m_tableStream,
				this.m_dataStream
			}, new string[3]
			{
				"WordDocument",
				"1Table",
				"Data"
			}, "00020906-0000-0000-c000-000000000046", author, title, comments, this.m_outStream, false);
			this.m_listStream.Close();
			this.m_mainStream.Close();
			this.m_tableStream.Close();
			this.m_dataStream.Close();
			this.m_fontTable.Close();
			this.m_listLevelStream.Close();
			this.m_charFormat.Stream.Close();
			this.m_parFormat.Stream.Close();
			this.m_wordText.Stream.Close();
		}

		public int WriteFont(string fontName)
		{
			if (string.IsNullOrEmpty(fontName))
			{
				fontName = "Times New Roman";
			}
			if (this.m_fontNameSet.ContainsKey(fontName))
			{
				return this.m_fontNameSet[fontName];
			}
			Ffn font = BuiltInFonts.GetFont(fontName);
			byte[] array = font.toByteArray();
			this.m_fontTable.Write(array, 0, array.Length);
			this.m_fontNameSet.Add(fontName, this.m_currentFontIndex);
			return this.m_currentFontIndex++;
		}

		private void WriteParagraphEnd(string parEnd, bool useTapx)
		{
			this.WriteSpecialText(parEnd);
			if (!useTapx && this.m_nestingLevel > 0)
			{
				this.m_parFormat.SetIsInTable(this.m_nestingLevel);
			}
			this.m_parFormat.CommitParagraph(this.m_wordText.CurrentCp, useTapx ? this.m_currentRow : null, this.m_dataStream);
		}

		public static int AddSprm(byte[] grpprl, int offset, ushort instruction, int param, byte[] varParam)
		{
			int num = (instruction & 0xE000) >> 13;
			byte[] array = null;
			switch (num)
			{
			case 0:
			case 1:
				array = new byte[3]
				{
					0,
					0,
					(byte)param
				};
				break;
			case 2:
				array = new byte[4];
				LittleEndian.PutUShort(array, 2, (ushort)param);
				break;
			case 3:
				array = new byte[6];
				LittleEndian.PutInt(array, 2, param);
				break;
			case 4:
			case 5:
				array = new byte[4];
				LittleEndian.PutUShort(array, 2, (ushort)param);
				break;
			case 6:
				array = new byte[3 + varParam.Length];
				array[2] = (byte)varParam.Length;
				Array.Copy(varParam, 0, array, 3, varParam.Length);
				break;
			case 7:
			{
				array = new byte[5];
				byte[] array2 = new byte[4];
				LittleEndian.PutInt(array2, 0, param);
				Array.Copy(array2, 0, array, 2, 3);
				break;
			}
			}
			LittleEndian.PutUShort(array, 0, instruction);
			Array.Copy(array, 0, grpprl, offset, array.Length);
			return array.Length;
		}

		public static double ToPoints(string size)
		{
			RPLReportSize rPLReportSize = new RPLReportSize(size);
			return rPLReportSize.ToPoints();
		}

		public static int ToIco24(string color)
		{
			RPLReportColor rPLReportColor = new RPLReportColor(color);
			return WordColor.GetIco24(rPLReportColor.ToColor());
		}

		public static ushort ToTwips(string size)
		{
			RPLReportSize rPLReportSize = new RPLReportSize(size);
			return (ushort)(rPLReportSize.ToPoints() * 20.0);
		}

		public static ushort ToTwips(object size)
		{
			if (size == null)
			{
				return 0;
			}
			return Word97Writer.ToTwips(size as string);
		}

		public static ushort ToTwips(float mm)
		{
			float num = (float)(mm / 25.399999618530273);
			float num2 = (float)(num * 1440.0);
			return (ushort)num2;
		}

		public static float TwipsToMM(int twips)
		{
			double num = (double)twips / 1440.0;
			return (float)(num * 25.399999618530273);
		}

		private int GetFcField(int fieldNum)
		{
			return 4 * (fieldNum * 2);
		}

		private int GetLcbField(int fieldNum)
		{
			return 4 * (fieldNum * 2 + 1);
		}

		private void TransferData(Stream inStream, Stream outStream, int bufSize)
		{
			inStream.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[bufSize];
			for (int num = inStream.Read(array, 0, array.Length); num != 0; num = inStream.Read(array, 0, array.Length))
			{
				outStream.Write(array, 0, num);
			}
			inStream.Close();
			inStream.Dispose();
		}

		public void RenderBookmark(string name)
		{
			this.m_bookmarks.AddBookmark(name, this.m_wordText.CurrentCp);
		}

		public void RenderLabel(string label, int level)
		{
			this.m_charFormat.AddSprm(2133, 1, null);
			this.m_charFormat.AddSprm(2050, 1, null);
			this.WriteSpecialText("\u0013");
			this.m_charFormat.AddSprm(2050, 1, null);
			this.WriteSpecialText(" TC \"" + label + "\" \\f C \\l \"" + level + "\" ");
			this.m_charFormat.AddSprm(2133, 1, null);
			this.m_charFormat.AddSprm(2050, 1, null);
			this.WriteSpecialText("\u0015");
		}

		public void WritePageNumberField()
		{
			this.m_charFormat.CopyAndPush();
			this.m_fldsCurrent.Add(new PageNumberFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.Start));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText("\u0013");
			this.m_charFormat.Pop();
			this.m_charFormat.CopyAndPush();
			this.WriteSpecialText(" PAGE ");
			this.m_charFormat.Pop();
			this.m_charFormat.CopyAndPush();
			this.m_fldsCurrent.Add(new PageNumberFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.Middle));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText("\u0014");
			this.m_charFormat.Pop();
			this.m_charFormat.CopyAndPush();
			this.WriteSpecialText("1");
			this.m_charFormat.Pop();
			this.m_fldsCurrent.Add(new PageNumberFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.End));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText("\u0015");
		}

		public void WriteTotalPagesField()
		{
			this.m_charFormat.CopyAndPush();
			this.m_fldsCurrent.Add(new TotalPagesFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.Start));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText("\u0013");
			this.m_charFormat.Pop();
			this.m_charFormat.CopyAndPush();
			this.WriteSpecialText(" NUMPAGES ");
			this.m_charFormat.Pop();
			this.m_charFormat.CopyAndPush();
			this.m_fldsCurrent.Add(new TotalPagesFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.Middle));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText("\u0014");
			this.m_charFormat.Pop();
			this.m_charFormat.CopyAndPush();
			this.WriteSpecialText("1");
			this.m_charFormat.Pop();
			this.m_fldsCurrent.Add(new TotalPagesFieldInfo(this.m_wordText.CurrentCp, FieldInfo.Location.End));
			this.m_charFormat.AddSprm(2133, 1, null);
			this.WriteSpecialText("\u0015");
		}

		public void AddListStyle(int level, bool bulleted)
		{
		}

		private void WriteCurrentListData()
		{
			if (this.m_currentList != null)
			{
				BinaryWriter binaryWriter = new BinaryWriter(this.m_listStream);
				BinaryWriter binaryWriter2 = new BinaryWriter(this.m_listLevelStream);
				this.m_currentList.Write(binaryWriter, binaryWriter2, this);
				binaryWriter.Flush();
				binaryWriter2.Flush();
				this.m_currentList = null;
			}
		}

		private void WriteListFormatOverrides(BinaryWriter writer)
		{
			for (int i = 0; i < this.m_listIndex; i++)
			{
				int value = i + 1;
				writer.Write(value);
				writer.Write(0);
				writer.Write(0);
				writer.Write(0);
			}
			for (int j = 0; j < this.m_listIndex; j++)
			{
				writer.Write(-1);
			}
			writer.Flush();
		}

		private void WriteListNameTable(BinaryWriter writer)
		{
			writer.Write((ushort)65535);
			writer.Write((short)this.m_listIndex);
			writer.Write((short)0);
			for (int i = 0; i < this.m_listIndex; i++)
			{
				writer.Write((short)0);
			}
			writer.Flush();
		}

		public void WriteCellDiagonal(int cellIndex, RPLFormat.BorderStyles style, string width, string color, bool slantUp)
		{
			this.m_currentRow.AddCellDiagonal(cellIndex, style, width, color, slantUp);
		}

		public void WritePageBreak()
		{
			this.WriteParagraphEnd("\f", false);
			this.m_parFormat.StyleIndex = 16;
			this.WriteParagraphEnd();
		}

		public void WriteEndSection()
		{
			this.m_parFormat.StyleIndex = 16;
			this.WriteParagraphEnd("\f", false);
			this.m_secFormat.EndSection(this.m_wordText.CurrentCp);
		}

		public void ClearCellBorder(TableData.Positions position)
		{
			this.m_currentRow.ClearCellBorder(position);
		}

		public void Dispose()
		{
		}

		void IWordWriter.FinishHeaderFooterRegion(int section, int index)
		{
		}

		public void StartHeader()
		{
		}

		public void StartHeader(bool firstPage)
		{
		}

		public void FinishHeader()
		{
		}

		public void StartFooter()
		{
		}

		public void StartFooter(bool firstPage)
		{
		}

		public void FinishFooter()
		{
		}
	}
}
