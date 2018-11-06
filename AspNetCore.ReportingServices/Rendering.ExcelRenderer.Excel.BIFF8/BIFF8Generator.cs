using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class BIFF8Generator : IExcelGenerator
	{
		private ExcelGeneratorConstants.CreateTempStream m_createTempStream;

		private List<BIFF8Color> m_colors = new List<BIFF8Color>();

		private Dictionary<string, BIFF8Color> m_colorLookup = new Dictionary<string, BIFF8Color>();

		private Escher.DrawingGroupContainer m_drawingGroupContainer;

		private SSTHandler m_stringTable = new SSTHandler();

		private WorksheetInfo m_worksheet;

		private BinaryWriter m_worksheetOut;

		private int m_numRowsThisBlock;

		private RowHandler m_rowHandler;

		private int m_column = -1;

		private object m_cellValue;

		private TypeCode m_cellValueType = TypeCode.String;

		private ExcelErrorCode m_cellErrorCode = ExcelErrorCode.None;

		private StyleContainer m_styleContainer;

		private bool m_checkForRotatedEastAsianChars;

		private int m_rowIndexStartOfBlock;

		private long m_startOfBlock = -1L;

		private long m_startOfFirstCellData;

		private long m_lastDataCheckPoint = -1L;

		private List<WorksheetInfo> m_worksheets = new List<WorksheetInfo>();

		private ExternSheetInfo m_externSheet;

		private Dictionary<string, string> m_bookmarks = new Dictionary<string, string>();

		private Dictionary<string, int> m_worksheetNames = new Dictionary<string, int>();

		public int MaxRows
		{
			get
			{
				return 65536;
			}
		}

		public int MaxColumns
		{
			get
			{
				return 256;
			}
		}

		public int RowBlockSize
		{
			get
			{
				return 32;
			}
		}

		internal BIFF8Generator(ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			this.m_createTempStream = createTempStream;
			this.m_styleContainer = new StyleContainer();
			this.m_worksheetNames.Add(ExcelRenderRes.SheetName.ToUpperInvariant(), 1);
			this.AddNewWorksheet();
		}

		private void RenderCell()
		{
			if (this.m_column != -1)
			{
				ExcelDataType excelDataType = ExcelDataType.Blank;
				RichTextInfo richTextInfo = null;
				if (this.m_cellValue != null)
				{
					if (this.m_cellValueType == TypeCode.Object)
					{
						richTextInfo = (this.m_cellValue as RichTextInfo);
						if (richTextInfo != null)
						{
							excelDataType = ExcelDataType.RichString;
						}
                        //else if (this.m_cellValue is SqlGeometry || this.m_cellValue is SqlGeography)
                        //{
                        //    excelDataType = ExcelDataType.String;
                        //}
                    }
					else
					{
						excelDataType = FormatHandler.GetDataType(this.m_cellValueType);
					}
				}
				bool flag = false;
				switch (excelDataType)
				{
				case ExcelDataType.String:
				{
					string text = this.m_cellValue.ToString();
					if (text.Length > 0)
					{
						StringBuilder stringBuilder = new StringBuilder(text.Length);
						ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(text, stringBuilder, this.m_checkForRotatedEastAsianChars, out flag);
						this.m_cellValue = stringBuilder.ToString();
					}
					break;
				}
				case ExcelDataType.RichString:
					flag = richTextInfo.FoundRotatedFarEastChar;
					break;
				}
				if (flag)
				{
					this.m_styleContainer.Orientation = Orientation.Vertical;
				}
				this.m_styleContainer.Finish();
				if (this.m_cellValue != null || this.m_styleContainer.CellIxfe != 15)
				{
					this.m_rowHandler.Add(this.m_cellValue, richTextInfo, this.m_cellValueType, excelDataType, this.m_cellErrorCode, (short)this.m_column, (ushort)this.m_styleContainer.CellIxfe);
				}
				this.m_cellValue = null;
				this.m_cellValueType = TypeCode.String;
				this.m_cellErrorCode = ExcelErrorCode.None;
				this.m_styleContainer.Reset();
				this.m_checkForRotatedEastAsianChars = false;
			}
		}

		private void FinishRow()
		{
			this.RenderCell();
			if (this.m_rowHandler != null)
			{
				this.m_rowHandler.FlushRow();
			}
			this.m_column = -1;
			long num = this.m_worksheetOut.BaseStream.Position - this.m_lastDataCheckPoint;
			this.m_worksheet.SizeOfCellData.Add((ushort)num);
			this.m_lastDataCheckPoint = this.m_worksheetOut.BaseStream.Position;
		}

		private void CalcAndWriteDBCells()
		{
			long num = this.m_worksheetOut.BaseStream.Position - this.m_startOfBlock;
			this.m_worksheet.DBCellOffsets.Add((uint)this.m_worksheetOut.BaseStream.Position);
			RecordFactory.DBCELL(this.m_worksheetOut, (uint)num, this.m_worksheet.SizeOfCellData);
			this.m_numRowsThisBlock = 0;
			this.m_worksheet.SizeOfCellData.Clear();
			this.m_startOfBlock = -1L;
		}

		private void AddNewWorksheet()
		{
			this.m_worksheet = new WorksheetInfo(this.m_createTempStream("Page" + this.m_worksheets.Count + 1), ExcelRenderRes.SheetName + (this.m_worksheets.Count + 1));
			this.m_worksheet.SheetIndex = this.m_worksheets.Count;
			this.m_worksheets.Add(this.m_worksheet);
			this.m_worksheetOut = new BinaryWriter(this.m_worksheet.CellData, Encoding.Unicode);
			this.m_numRowsThisBlock = 0;
			this.m_rowHandler = null;
			this.m_column = -1;
			this.m_cellValue = null;
			this.m_cellValueType = TypeCode.String;
			this.m_checkForRotatedEastAsianChars = false;
			this.m_startOfBlock = -1L;
			this.m_rowIndexStartOfBlock = 0;
			this.m_startOfFirstCellData = 0L;
			this.m_lastDataCheckPoint = -1L;
			this.m_styleContainer.Reset();
		}

		private int AddExternSheet(int supBookIndex, int firstTab, int lastTab)
		{
			if (this.m_externSheet == null)
			{
				this.m_externSheet = new ExternSheetInfo();
			}
			return this.m_externSheet.AddXTI((ushort)supBookIndex, (ushort)firstTab, (ushort)lastTab);
		}

		private void CompleteCurrentWorksheet()
		{
			this.FinishRow();
			this.CalcAndWriteDBCells();
			this.m_worksheetOut.Flush();
		}

		private List<long> WriteGlobalStream(BinaryWriter writer)
		{
			writer.BaseStream.Write(Constants.GLOBAL1, 0, Constants.GLOBAL1.Length);
			this.m_styleContainer.Write(writer);
			writer.BaseStream.Write(Constants.GLOBAL3, 0, Constants.GLOBAL3.Length);
			RecordFactory.PALETTE(writer, this.m_colors);
			List<long> list = new List<long>();
			foreach (WorksheetInfo worksheet in this.m_worksheets)
			{
				list.Add(writer.BaseStream.Position);
				RecordFactory.BOUNDSHEET(writer, 0u, worksheet.SheetName);
			}
			if (this.m_externSheet != null)
			{
				RecordFactory.SUPBOOK(writer, (ushort)this.m_worksheets.Count);
				RecordFactory.EXTERNSHEET(writer, this.m_externSheet);
				foreach (WorksheetInfo worksheet2 in this.m_worksheets)
				{
					if (worksheet2.PrintTitle != null)
					{
						RecordFactory.NAME_PRINTTITLE(writer, worksheet2.PrintTitle);
					}
				}
			}
			if (this.m_drawingGroupContainer != null)
			{
				MsoDrawingGroup.WriteToStream(writer, this.m_drawingGroupContainer);
			}
			this.m_stringTable.Write(writer.BaseStream);
			writer.BaseStream.Write(Constants.GLOBAL4, 0, Constants.GLOBAL4.Length);
			return list;
		}

		private void AdjustWorksheetName(string reportName, WorksheetInfo workSheet)
		{
			if (!string.IsNullOrEmpty(reportName))
			{
				if (reportName.Length <= 31)
				{
					workSheet.SheetName = reportName;
				}
				else
				{
					workSheet.SheetName = reportName.Substring(0, 31);
				}
			}
		}

		private bool CheckForSheetNameConflict(string pageName)
		{
			if (pageName.Length >= ExcelRenderRes.SheetName.Length && pageName.StartsWith(ExcelRenderRes.SheetName, StringComparison.OrdinalIgnoreCase))
			{
				int num = 0;
				return int.TryParse(pageName.Substring(ExcelRenderRes.SheetName.Length - 1), out num);
			}
			return false;
		}

		public void NextWorksheet()
		{
			this.CompleteCurrentWorksheet();
			this.AddNewWorksheet();
		}

		public void SetCurrentSheetName(string name)
		{
			this.m_worksheet.SheetName = name;
		}

		public void AdjustFirstWorksheetName(string reportName, bool addedDocMap)
		{
			if (addedDocMap)
			{
				if (this.m_worksheets.Count == 2)
				{
					this.AdjustWorksheetName(reportName, this.m_worksheets[1]);
				}
			}
			else if (this.m_worksheets.Count == 1)
			{
				this.AdjustWorksheetName(reportName, this.m_worksheets[0]);
			}
		}

		public void GenerateWorksheetName(string pageName)
		{
			if (!string.IsNullOrEmpty(pageName))
			{
				StringBuilder stringBuilder = ExcelGeneratorStringUtil.SanitizeSheetName(pageName);
				if (stringBuilder.Length != 0)
				{
					pageName = stringBuilder.ToString();
					int num = 0;
					while (true)
					{
						if (!this.m_worksheetNames.TryGetValue(pageName.ToUpperInvariant(), out num) && !this.CheckForSheetNameConflict(pageName))
						{
							break;
						}
						num++;
						this.m_worksheetNames[pageName.ToUpperInvariant()] = num;
						string text = string.Format(CultureInfo.InvariantCulture, ExcelRenderRes.SheetNameCounterSuffix, num);
						if (pageName.Length + text.Length > 31)
						{
							pageName = pageName.Remove(31 - text.Length);
						}
						pageName += text;
					}
					this.m_worksheet.SheetName = pageName;
					this.m_worksheetNames.Add(pageName.ToUpperInvariant(), 1);
				}
			}
		}

		public void SetSummaryRowAfter(bool after)
		{
			this.m_worksheet.SummaryRowAfter = after;
		}

		public void SetSummaryColumnToRight(bool after)
		{
			this.m_worksheet.SummaryColumnToRight = after;
		}

		public void SetColumnProperties(int columnIndex, double widthInPoints, byte outlineLevel, bool collapsed)
		{
			WorksheetInfo.ColumnInfo columnInfo = new WorksheetInfo.ColumnInfo(widthInPoints);
			columnInfo.OutlineLevel = outlineLevel;
			this.m_worksheet.MaxColumnOutline = Math.Max(this.m_worksheet.MaxColumnOutline, outlineLevel);
			columnInfo.Collapsed = collapsed;
			this.m_worksheet.Columns[columnIndex] = columnInfo;
		}

		public void SetColumnExtents(int min, int max)
		{
			this.m_worksheet.ColFirst = (ushort)min;
			this.m_worksheet.ColLast = (ushort)max;
		}

		public void AddRow(int rowIndex)
		{
			if (this.m_startOfBlock == -1)
			{
				this.m_startOfBlock = this.m_worksheetOut.BaseStream.Position;
				this.m_startOfFirstCellData = -20L;
				this.m_rowIndexStartOfBlock = rowIndex;
			}
			if (this.m_worksheet.RowFirst > rowIndex)
			{
				this.m_worksheet.RowFirst = (ushort)rowIndex;
			}
			if (this.m_worksheet.RowLast < rowIndex)
			{
				this.m_worksheet.RowLast = (ushort)rowIndex;
			}
			this.m_startOfFirstCellData += RecordFactory.ROW(this.m_worksheetOut, (ushort)rowIndex, this.m_worksheet.ColFirst, this.m_worksheet.ColLast, 0, 0, false, false);
		}

		public void SetRowProperties(int rowIndex, int heightIn20thPoints, byte rowOutlineLevel, bool collapsed, bool autoSize)
		{
			this.m_numRowsThisBlock++;
			int num = rowIndex - this.m_rowIndexStartOfBlock;
			long position = this.m_startOfBlock + num * 20;
			this.m_worksheetOut.BaseStream.Position = position;
			RecordFactory.ROW(this.m_worksheetOut, (ushort)rowIndex, this.m_worksheet.ColFirst, this.m_worksheet.ColLast, (ushort)heightIn20thPoints, rowOutlineLevel, collapsed, autoSize);
			this.m_worksheetOut.BaseStream.Seek(0L, SeekOrigin.End);
			this.m_worksheet.MaxRowOutline = Math.Max(this.m_worksheet.MaxRowOutline, rowOutlineLevel);
			this.FinishRow();
			if (this.m_numRowsThisBlock == 32)
			{
				this.CalcAndWriteDBCells();
			}
		}

		public void SetRowContext(int row)
		{
			if (this.m_rowHandler == null)
			{
				this.m_rowHandler = new RowHandler(this.m_worksheetOut, row, this.m_stringTable);
			}
			this.m_rowHandler.Row = row;
		}

		public void SetColumnContext(int column)
		{
			this.RenderCell();
			this.m_column = column;
		}

		public void SetCellValue(object value, TypeCode type)
		{
			if (this.m_lastDataCheckPoint == -1)
			{
				this.m_lastDataCheckPoint = this.m_worksheetOut.BaseStream.Position;
				this.m_worksheet.SizeOfCellData.Add((ushort)this.m_startOfFirstCellData);
				this.m_startOfFirstCellData = 0L;
			}
			this.m_cellValue = value;
			this.m_cellValueType = type;
			this.m_cellErrorCode = ExcelErrorCode.None;
		}

		public void SetModifiedRotationForEastAsianChars(bool value)
		{
			this.m_checkForRotatedEastAsianChars = value;
		}

		public IRichTextInfo GetCellRichTextInfo()
		{
			this.m_cellValueType = TypeCode.Object;
			this.m_cellValue = new RichTextInfo(this.m_styleContainer);
			return (IRichTextInfo)this.m_cellValue;
		}

		public void SetCellError(ExcelErrorCode errorCode)
		{
			this.SetCellValue(null, TypeCode.Empty);
			this.m_cellErrorCode = errorCode;
		}

		public void AddMergeCell(int rowStart, int columnStart, int rowStop, int columnStop)
		{
			this.m_worksheet.MergeCellAreas.Add(new AreaInfo(rowStart, rowStop, columnStart, columnStop));
		}

		public IColor AddColor(string color)
		{
			if (this.m_colorLookup.ContainsKey(color))
			{
				return this.m_colorLookup[color];
			}
			Color color2 = LayoutConvert.ToColor(color);
			BIFF8Color bIFF8Color = null;
			for (int i = 0; i < this.m_colors.Count; i++)
			{
				if (this.m_colors[i].Equals(color2))
				{
					bIFF8Color = this.m_colors[i];
				}
			}
			if (bIFF8Color == null)
			{
				if (this.m_colors.Count >= 56)
				{
					return this.m_colors[0];
				}
				bIFF8Color = new BIFF8Color(color2, this.m_colors.Count + 8);
				this.m_colors.Add(bIFF8Color);
			}
			this.m_colorLookup[color] = bIFF8Color;
			return bIFF8Color;
		}

		public IStyle GetCellStyle()
		{
			return this.m_styleContainer;
		}

		public TypeCode GetCellValueType()
		{
			return this.m_cellValueType;
		}

		public void AddImage(string imageName, Stream imageData, ImageFormat format, int rowStart, double rowStartPercentage, int columnStart, double columnStartPercentage, int rowEnd, double rowEndPercentage, int columnEnd, double columnEndPercentage, string hyperlinkURL, bool isBookmarkLink)
		{
			if (imageData != null && 0 != imageData.Length)
			{
				if (this.m_drawingGroupContainer == null)
				{
					this.m_drawingGroupContainer = new Escher.DrawingGroupContainer();
				}
				uint starterShapeID = 0u;
				ushort drawingID = 0;
				uint referenceIndex = this.m_drawingGroupContainer.AddImage(imageData, format, imageName, this.m_worksheets.Count - 1, out starterShapeID, out drawingID);
				Escher.ClientAnchor.SPRC clientAnchor = new Escher.ClientAnchor.SPRC((ushort)columnStart, (short)(columnStartPercentage * 1024.0), (ushort)rowStart, (short)(rowStartPercentage * 256.0), (ushort)columnEnd, (short)(columnEndPercentage * 1024.0), (ushort)rowEnd, (short)(rowEndPercentage * 256.0));
				this.m_worksheet.AddImage(drawingID, starterShapeID, imageName, clientAnchor, referenceIndex, hyperlinkURL, isBookmarkLink);
			}
		}

		public void AddBackgroundImage(byte[] data, string imageName, ref Stream backgroundImage, ref ushort backgroundImageWidth, ref ushort backgroundImageHeight)
		{
			Image image = null;
			Bitmap bitmap = null;
			ushort num = 0;
			ushort num2 = 0;
			if (backgroundImage == null && imageName != null && !string.IsNullOrEmpty(imageName))
			{
				backgroundImage = this.m_createTempStream("BACKGROUNDIMAGE");
				backgroundImage.Write(data, 0, data.Length);
				backgroundImage.Position = 0L;
				try
				{
					image = Image.FromStream(backgroundImage);
				}
				catch
				{
				}
				if (image != null)
				{
					bitmap = new Bitmap(image);
					Color white = Color.White;
					num = (ushort)image.Width;
					num2 = (ushort)image.Height;
					int num3 = num * 3;
					int num4 = num3 % 4;
					if (num4 > 0)
					{
						num4 = 4 - num4;
					}
					byte[] array = new byte[num3 + num4];
					backgroundImage.SetLength(0L);
					backgroundImage.Position = 0L;
					for (int num5 = num2 - 1; num5 >= 0; num5--)
					{
						for (int i = 0; i < num; i++)
						{
							Color pixel = bitmap.GetPixel(i, num5);
							if (pixel.A == 0)
							{
								array[i * 3] = white.B;
								array[i * 3 + 1] = white.G;
								array[i * 3 + 2] = white.R;
							}
							else
							{
								array[i * 3] = pixel.B;
								array[i * 3 + 1] = pixel.G;
								array[i * 3 + 2] = pixel.R;
							}
						}
						backgroundImage.Write(array, 0, array.Length);
						Array.Clear(array, 0, array.Length);
					}
					backgroundImageWidth = num;
					backgroundImageHeight = num2;
					if (bitmap != null)
					{
						bitmap.Dispose();
						bitmap = null;
					}
					if (image != null)
					{
						image.Dispose();
						image = null;
					}
				}
				else if (backgroundImage != null)
				{
					backgroundImage.Close();
					backgroundImage = null;
				}
			}
		}

		public void AddHeader(string left, string center, string right)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(left, stringBuilder.Append("&L"));
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(center, stringBuilder.Append("&C"));
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(right, stringBuilder.Append("&R"));
			this.m_worksheet.HeaderString = stringBuilder.ToString();
		}

		public void AddFooter(string left, string center, string right)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(left, stringBuilder.Append("&L"));
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(center, stringBuilder.Append("&C"));
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(right, stringBuilder.Append("&R"));
			this.m_worksheet.FooterString = stringBuilder.ToString();
		}

		public void AddPrintTitle(int rowStart, int rowEnd)
		{
			int externSheetIndex = this.AddExternSheet(0, this.m_worksheet.SheetIndex, this.m_worksheet.SheetIndex);
			this.m_worksheet.AddPrintTitle(externSheetIndex, rowStart, rowEnd);
		}

		public void AddFreezePane(int row, int column)
		{
			this.m_worksheet.AddFreezePane(row, column);
		}

		public void AddHyperlink(string label, string url)
		{
			this.m_worksheet.AddHyperlink(this.m_rowHandler.Row, this.m_column, url, label);
		}

		public void AddBookmarkLink(string label, string bookmark)
		{
			this.m_worksheet.AddBookmark(this.m_rowHandler.Row, this.m_column, bookmark, label);
		}

		public void AddBookmarkTarget(string targetName)
		{
			this.m_bookmarks[targetName] = CellReference.CreateExcelReference(this.m_worksheet.SheetName, this.m_rowHandler.Row, this.m_column);
		}

		public void SetPageContraints(int paperSize, bool isPortrait, double headerMargin, double footerMargin)
		{
			this.m_worksheet.SetPageContraints(paperSize, isPortrait, headerMargin, footerMargin);
		}

		public void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin)
		{
			this.m_worksheet.SetMargins(topMargin, bottomMargin, leftMargin, rightMargin);
		}

		public void SaveSpreadsheet(Stream outputStream, Stream backgroundImage, ushort backgroundImageWidth, ushort backgroundImageHeight)
		{
			this.CompleteCurrentWorksheet();
			this.m_worksheet = null;
			this.m_worksheetOut = null;
			Stream stream = this.m_createTempStream("Workbook");
			BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.Unicode);
			List<long> list = this.WriteGlobalStream(binaryWriter);
			bool isFirstPage = true;
			for (int i = 0; i < this.m_worksheets.Count; i++)
			{
				WorksheetInfo worksheetInfo = this.m_worksheets[i];
				worksheetInfo.ResolveCellReferences(this.m_bookmarks);
				worksheetInfo.Write(binaryWriter, isFirstPage, this.m_createTempStream, backgroundImage, backgroundImageWidth, backgroundImageHeight);
				isFirstPage = false;
			}
			for (int j = 0; j < list.Count; j++)
			{
				stream.Seek(list[j] + 4, SeekOrigin.Begin);
				binaryWriter.Write((uint)this.m_worksheets[j].BOFStartOffset);
			}
			this.m_worksheets = null;
			stream.Flush();
			stream.Seek(0L, SeekOrigin.Begin);
			StructuredStorage.CreateSingleStreamFile(stream, "Workbook", "00020820-0000-0000-c000-000000000046", outputStream, false);
			stream.Close();
			stream = null;
			outputStream.Flush();
		}

		public bool UseCachedStyle(string id)
		{
			return this.m_styleContainer.UseSharedStyle(id);
		}

		public void DefineCachedStyle(string id)
		{
			this.m_styleContainer.DefineSharedStyle(id);
		}

		public void EndCachedStyle()
		{
			this.m_styleContainer.Finish();
		}

		public Stream CreateStream(string name)
		{
			return this.m_createTempStream(name);
		}

		public void BuildHeaderFooterString(StringBuilder str, RPLTextBoxProps textBox, ref string lastFont, ref double lastFontSize)
		{
			RPLElementStyle style = textBox.Style;
			string text = (string)style[20];
			if (!string.IsNullOrEmpty(text) && !text.Equals(lastFont))
			{
				str.Append("&").Append("\"");
				FormulaHandler.EncodeHeaderFooterString(str, text);
				str.Append("\"");
				lastFont = text;
			}
			string text2 = (string)style[21];
			if (!string.IsNullOrEmpty(text2))
			{
				double num = LayoutConvert.ToPoints(text2);
				if (num != 0.0 && num != lastFontSize)
				{
					str.Append("&").Append((int)num);
				}
				lastFontSize = num;
			}
			StringBuilder stringBuilder = new StringBuilder();
			object obj = style[19];
			if (obj != null && (RPLFormat.FontStyles)obj == RPLFormat.FontStyles.Italic)
			{
				str.Append("&I");
				stringBuilder.Append("&I");
			}
			object obj2 = textBox.Style[22];
			if (obj2 != null && LayoutConvert.ToFontWeight((RPLFormat.FontWeights)obj2) >= 600)
			{
				str.Append("&B");
				stringBuilder.Append("&B");
			}
			object obj3 = textBox.Style[24];
			if (obj3 != null)
			{
				RPLFormat.TextDecorations textDecorations = (RPLFormat.TextDecorations)obj3;
				if (textDecorations == RPLFormat.TextDecorations.Underline)
				{
					str.Append("&u");
					stringBuilder.Append("&u");
				}
				if (textDecorations == RPLFormat.TextDecorations.LineThrough)
				{
					str.Append("&s");
					stringBuilder.Append("&s");
				}
			}
			string text3 = string.Empty;
			string text4 = ((RPLTextBoxPropsDef)textBox.Definition).Formula;
			if (text4 != null && text4.Length != 0)
			{
				if (text4.StartsWith("=", StringComparison.Ordinal))
				{
					text4 = text4.Remove(0, 1);
				}
				text3 = FormulaHandler.ProcessHeaderFooterFormula(text4);
			}
			if (text3 != null && text3.Length != 0)
			{
				str.Append(text3);
			}
			else
			{
				string text5 = textBox.Value;
				if (text5 == null)
				{
					text5 = ((RPLTextBoxPropsDef)textBox.Definition).Value;
				}
				if (text5 == null && textBox.OriginalValue != null)
				{
					text5 = textBox.OriginalValue.ToString();
				}
				if (text5 != null)
				{
					FormulaHandler.EncodeHeaderFooterString(str, text5.Trim());
				}
			}
			if (stringBuilder.Length > 0)
			{
				str.Append(stringBuilder);
			}
			str.Append(" ");
		}
	}
}
