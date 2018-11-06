using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.Utilities;
using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models;
using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal sealed class WordOpenXmlWriter : IWordWriter, IDisposable
	{
		public delegate Stream CreateXmlStream(string name);

		private CreateAndRegisterStream _createAndRegisterStream;

		private AutoFit _autofit;

		private OpenXmlDocumentModel _document;

		private OpenXmlParagraphModel _currentParagraph;

		private OpenXmlRunPropertiesModel _currentTextStyle;

		private ScalabilityCache _scalabilityCache;

		private int _numberedListId;

		private int _bookmarkId;

		private uint _nextPictureId;

		private bool _forceEmptyLayoutCell;

		public bool CanBand
		{
			get
			{
				return this._document.TableContext.Depth > 1;
			}
		}

		public AutoFit AutoFit
		{
			get
			{
				return this._autofit;
			}
			set
			{
				this._autofit = value;
			}
		}

		public bool HasTitlePage
		{
			set
			{
				this._document.SectionHasTitlePage = value;
			}
		}

		public int SectionCount
		{
			get
			{
				return 1;
			}
		}

		private uint NextPictureId()
		{
			return this._nextPictureId++;
		}

		private OpenXmlParagraphModel GetCurrentParagraph()
		{
			if (this._currentParagraph == null)
			{
				this._currentParagraph = new OpenXmlParagraphModel();
			}
			return this._currentParagraph;
		}

		private void EndAndWriteCurrentParagraph(bool forceEmptyParagraph)
		{
			if (this._currentParagraph != null)
			{
				if (this._document.TableContext.Location == TableContext.State.InCell)
				{
					this._document.TableContext.CurrentCell.AddContent(this.GetCurrentParagraph());
				}
				else
				{
					this._document.WriteParagraph(this.GetCurrentParagraph());
				}
				this._currentParagraph = null;
			}
			else if (forceEmptyParagraph)
			{
				if (this._document.TableContext.Location == TableContext.State.InCell)
				{
					this._document.TableContext.CurrentCell.AddContent(new OpenXmlParagraphModel.EmptyParagraph());
				}
				else
				{
					this._document.WriteEmptyParagraph();
				}
			}
		}

		private OpenXmlRunPropertiesModel GetCurrentTextStyle()
		{
			if (this._currentTextStyle == null)
			{
				this._currentTextStyle = new OpenXmlRunPropertiesModel();
			}
			return this._currentTextStyle;
		}

		private void SetTextDirection(RPLFormat.Directions direction)
		{
			this.GetCurrentTextStyle().RightToLeft = (direction == RPLFormat.Directions.RTL);
		}

		private void SetTextDecoration(RPLFormat.TextDecorations decoration)
		{
			switch (decoration)
			{
			case RPLFormat.TextDecorations.Overline:
				break;
			case RPLFormat.TextDecorations.LineThrough:
				this.GetCurrentTextStyle().Strikethrough = true;
				break;
			case RPLFormat.TextDecorations.Underline:
				this.GetCurrentTextStyle().Underline = true;
				break;
			}
		}

		private void SetTextColor(string color)
		{
			this.SetTextColor(new RPLReportColor(color).ToColor());
		}

		private void SetTextColor(Color color)
		{
			this.GetCurrentTextStyle().Color = color;
		}

		private void SetLineHeight(string height)
		{
			double lineHeight = new RPLReportSize(height).ToPoints();
			this.GetCurrentParagraph().Properties.LineHeight = lineHeight;
		}

		private void SetUnicodeBiDi(RPLFormat.UnicodeBiDiTypes biDiType)
		{
		}

		private void SetLanguage(string language)
		{
			this.GetCurrentTextStyle().Language = language;
		}

		private void SetParagraphDirection(RPLFormat.Directions direction)
		{
			this.GetCurrentParagraph().Properties.RightToLeft = (direction == RPLFormat.Directions.RTL);
		}

		private void SetBorderColor(IHaveABorderAndShading borderHolder, string color)
		{
			this.SetBorderColor(borderHolder, color, TableData.Positions.Bottom);
			this.SetBorderColor(borderHolder, color, TableData.Positions.Left);
			this.SetBorderColor(borderHolder, color, TableData.Positions.Right);
			this.SetBorderColor(borderHolder, color, TableData.Positions.Top);
		}

		private void SetBorderColor(IHaveABorderAndShading borderHolder, string color, TableData.Positions side)
		{
			switch (side)
			{
			case TableData.Positions.Top:
				borderHolder.BorderTop.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
				break;
			case TableData.Positions.Bottom:
				borderHolder.BorderBottom.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
				break;
			case TableData.Positions.Left:
				borderHolder.BorderLeft.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
				break;
			case TableData.Positions.Right:
				borderHolder.BorderRight.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
				break;
			}
		}

		private void SetBorderStyle(IHaveABorderAndShading borderHolder, RPLFormat.BorderStyles style)
		{
			this.SetBorderStyle(borderHolder, style, TableData.Positions.Top);
			this.SetBorderStyle(borderHolder, style, TableData.Positions.Bottom);
			this.SetBorderStyle(borderHolder, style, TableData.Positions.Left);
			this.SetBorderStyle(borderHolder, style, TableData.Positions.Right);
		}

		private OpenXmlBorderPropertiesModel.BorderStyle RPLFormatToBorderStyle(RPLFormat.BorderStyles style)
		{
			OpenXmlBorderPropertiesModel.BorderStyle result = OpenXmlBorderPropertiesModel.BorderStyle.None;
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				result = OpenXmlBorderPropertiesModel.BorderStyle.Dashed;
				break;
			case RPLFormat.BorderStyles.Dotted:
				result = OpenXmlBorderPropertiesModel.BorderStyle.Dotted;
				break;
			case RPLFormat.BorderStyles.Double:
				result = OpenXmlBorderPropertiesModel.BorderStyle.Double;
				break;
			case RPLFormat.BorderStyles.None:
				result = OpenXmlBorderPropertiesModel.BorderStyle.None;
				break;
			case RPLFormat.BorderStyles.Solid:
				result = OpenXmlBorderPropertiesModel.BorderStyle.Solid;
				break;
			}
			return result;
		}

		private void SetBorderStyle(IHaveABorderAndShading borderHolder, RPLFormat.BorderStyles style, TableData.Positions side)
		{
			OpenXmlBorderPropertiesModel.BorderStyle style2 = this.RPLFormatToBorderStyle(style);
			switch (side)
			{
			case TableData.Positions.Top:
				borderHolder.BorderTop.Style = style2;
				break;
			case TableData.Positions.Bottom:
				borderHolder.BorderBottom.Style = style2;
				break;
			case TableData.Positions.Left:
				borderHolder.BorderLeft.Style = style2;
				break;
			case TableData.Positions.Right:
				borderHolder.BorderRight.Style = style2;
				break;
			}
		}

		private void SetBorderWidth(IHaveABorderAndShading borderHolder, string width)
		{
			this.SetBorderWidth(borderHolder, width, TableData.Positions.Top);
			this.SetBorderWidth(borderHolder, width, TableData.Positions.Bottom);
			this.SetBorderWidth(borderHolder, width, TableData.Positions.Left);
			this.SetBorderWidth(borderHolder, width, TableData.Positions.Right);
		}

		private void SetBorderWidth(IHaveABorderAndShading borderHolder, string width, TableData.Positions side)
		{
			int widthInEighthPoints = (int)Math.Floor(new RPLReportSize(width).ToPoints() * 8.0);
			switch (side)
			{
			case TableData.Positions.Top:
				borderHolder.BorderTop.WidthInEighthPoints = widthInEighthPoints;
				break;
			case TableData.Positions.Bottom:
				borderHolder.BorderBottom.WidthInEighthPoints = widthInEighthPoints;
				break;
			case TableData.Positions.Left:
				borderHolder.BorderLeft.WidthInEighthPoints = widthInEighthPoints;
				break;
			case TableData.Positions.Right:
				borderHolder.BorderRight.WidthInEighthPoints = widthInEighthPoints;
				break;
			}
		}

		private void SetVerticalAlign(RPLFormat.VerticalAlignments alignment)
		{
			switch (alignment)
			{
			case RPLFormat.VerticalAlignments.Bottom:
				this._document.TableContext.CurrentCell.CellProperties.VerticalAlignment = OpenXmlTableCellPropertiesModel.VerticalAlign.Bottom;
				break;
			case RPLFormat.VerticalAlignments.Middle:
				this._document.TableContext.CurrentCell.CellProperties.VerticalAlignment = OpenXmlTableCellPropertiesModel.VerticalAlign.Middle;
				break;
			case RPLFormat.VerticalAlignments.Top:
				this._document.TableContext.CurrentCell.CellProperties.VerticalAlignment = OpenXmlTableCellPropertiesModel.VerticalAlign.Top;
				break;
			}
		}

		private void SetWritingMode(RPLFormat.WritingModes mode)
		{
			switch (mode)
			{
			case RPLFormat.WritingModes.Rotate270:
				this._document.TableContext.CurrentCell.CellProperties.TextOrientation = OpenXmlTableCellPropertiesModel.TextOrientationEnum.Rotate270;
				break;
			case RPLFormat.WritingModes.Vertical:
				this._document.TableContext.CurrentCell.CellProperties.TextOrientation = OpenXmlTableCellPropertiesModel.TextOrientationEnum.Rotate90;
				break;
			}
		}

		private void SetShading(IHaveABorderAndShading shadingHolder, string shading)
		{
			if (!shading.Equals("Transparent"))
			{
				string text2 = shadingHolder.BackgroundColor = WordOpenXmlUtils.RgbColor(new RPLReportColor(shading).ToColor());
			}
		}

		private Stream CreateXmlStreamImplementation(string name)
		{
			return this._createAndRegisterStream(name, "xml", null, "application/xml", false, StreamOper.CreateOnly);
		}

		public void Init(CreateAndRegisterStream createAndRegisterStream, AutoFit autoFit, string reportName)
		{
			this._createAndRegisterStream = createAndRegisterStream;
			this._autofit = autoFit;
			this.InitCache(this._createAndRegisterStream);
			this._document = new OpenXmlDocumentModel(createAndRegisterStream(reportName, "docx", null, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", true, StreamOper.CreateAndRegister), this.CreateXmlStreamImplementation, this._scalabilityCache);
			this._nextPictureId = 0u;
			this._forceEmptyLayoutCell = false;
		}

		public void SetPageDimensions(float pageHeight, float pageWidth, float leftMargin, float rightMargin, float topMargin, float bottomMargin)
		{
			this._document.SectionProperties.Height = pageHeight;
			this._document.SectionProperties.Width = pageWidth;
			this._document.SectionProperties.IsLandscape = (pageWidth > pageHeight);
			this._document.SectionProperties.LeftMargin = leftMargin;
			this._document.SectionProperties.RightMargin = rightMargin;
			this._document.SectionProperties.TopMargin = topMargin;
			this._document.SectionProperties.BottomMargin = bottomMargin;
		}

		public void AddImage(byte[] imgBuf, float height, float width, RPLFormat.Sizings sizing)
		{
			bool flag = imgBuf == null || imgBuf.Length == 0;
			Size image = default(Size);
			string extension = null;
			if (!flag)
			{
				try
				{
					using (Image image2 = Image.FromStream(new MemoryStream(imgBuf)))
					{
						image.Height = WordOpenXmlUtils.PixelsToEmus(image2.Height, (double)image2.VerticalResolution, 0, 20116800);
						image.Width = WordOpenXmlUtils.PixelsToEmus(image2.Width, (double)image2.HorizontalResolution, 0, 20116800);
						extension = ((!(image2.RawFormat.Guid == ImageFormat.Png.Guid)) ? ((!(image2.RawFormat.Guid == ImageFormat.Jpeg.Guid)) ? ((!(image2.RawFormat.Guid == ImageFormat.Gif.Guid)) ? "bmp" : "gif") : "jpg") : "png");
					}
				}
				catch (ArgumentException)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.AddImage(PictureDescriptor.INVALIDIMAGEDATA, height, width, RPLFormat.Sizings.Clip);
			}
			else
			{
				Size size = default(Size);
				size.Height = WordOpenXmlUtils.ToEmus(height, 0, 20116800);
				size.Width = WordOpenXmlUtils.ToEmus(width, 0, 20116800);
				Size size2 = size;
				ImageHash hash = new SizingIndependentImageHash(new OfficeImageHasher(imgBuf).Hash);
				Relationship relationship = this._document.WriteImageData(imgBuf, hash, extension);
				Size.Strategy strategy = Size.Strategy.AutoSize;
				switch (sizing)
				{
				case RPLFormat.Sizings.AutoSize:
					strategy = Size.Strategy.AutoSize;
					break;
				case RPLFormat.Sizings.Fit:
					strategy = Size.Strategy.Fit;
					break;
				case RPLFormat.Sizings.FitProportional:
					strategy = Size.Strategy.FitProportional;
					break;
				case RPLFormat.Sizings.Clip:
					strategy = Size.Strategy.Clip;
					break;
				}
				Size size3 = WordOpenXmlUtils.SizeImage(image, size2, strategy);
				Size desiredSize = (strategy == Size.Strategy.FitProportional || strategy == Size.Strategy.AutoSize) ? size3 : size2;
				this.GetCurrentParagraph().AddImage(new OpenXmlPictureModel(size3, desiredSize, sizing == RPLFormat.Sizings.Clip, this.NextPictureId(), this.NextPictureId(), relationship.RelationshipId, Path.GetFileName(relationship.RelatedPart)));
			}
		}

		public void WriteText(string text)
		{
			this.GetCurrentParagraph().AddText(text, this.GetCurrentTextStyle());
			this._currentTextStyle = null;
		}

		public void WriteHyperlinkBegin(string target, bool bookmarkLink)
		{
			this.GetCurrentParagraph().StartHyperlink(target, bookmarkLink, this.GetCurrentTextStyle());
		}

		public void WriteHyperlinkEnd()
		{
			this.GetCurrentParagraph().EndHyperlink(this.GetCurrentTextStyle());
		}

		public void AddTableStyleProp(byte code, object value)
		{
			if (value != null)
			{
				IHaveABorderAndShading tableProperties = this._document.TableContext.CurrentTable.TableProperties;
				switch (code)
				{
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 35:
				case 36:
				case 37:
					break;
				case 0:
					this.SetBorderColor(tableProperties, (string)value);
					break;
				case 1:
					this.SetBorderColor(tableProperties, (string)value, TableData.Positions.Left);
					break;
				case 2:
					this.SetBorderColor(tableProperties, (string)value, TableData.Positions.Right);
					break;
				case 3:
					this.SetBorderColor(tableProperties, (string)value, TableData.Positions.Top);
					break;
				case 4:
					this.SetBorderColor(tableProperties, (string)value, TableData.Positions.Bottom);
					break;
				case 5:
					this.SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value);
					break;
				case 6:
					this.SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Left);
					break;
				case 7:
					this.SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Right);
					break;
				case 8:
					this.SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Top);
					break;
				case 9:
					this.SetBorderStyle(tableProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Bottom);
					break;
				case 10:
					this.SetBorderWidth(tableProperties, (string)value);
					break;
				case 11:
					this.SetBorderWidth(tableProperties, (string)value, TableData.Positions.Left);
					break;
				case 12:
					this.SetBorderWidth(tableProperties, (string)value, TableData.Positions.Right);
					break;
				case 13:
					this.SetBorderWidth(tableProperties, (string)value, TableData.Positions.Top);
					break;
				case 14:
					this.SetBorderWidth(tableProperties, (string)value, TableData.Positions.Bottom);
					break;
				case 34:
					this.SetShading(tableProperties, (string)value);
					break;
				}
			}
		}

		public void SetTableContext(BorderContext borderContext)
		{
			if (borderContext.Top)
			{
				this.SetBorderStyle(this._document.TableContext.CurrentTable.TableProperties, RPLFormat.BorderStyles.None, TableData.Positions.Top);
			}
			if (borderContext.Bottom)
			{
				this.SetBorderStyle(this._document.TableContext.CurrentTable.TableProperties, RPLFormat.BorderStyles.None, TableData.Positions.Bottom);
			}
			if (borderContext.Left)
			{
				this.SetBorderStyle(this._document.TableContext.CurrentTable.TableProperties, RPLFormat.BorderStyles.None, TableData.Positions.Left);
			}
			if (borderContext.Right)
			{
				this.SetBorderStyle(this._document.TableContext.CurrentTable.TableProperties, RPLFormat.BorderStyles.None, TableData.Positions.Right);
			}
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
				IHaveABorderAndShading cellProperties = this._document.TableContext.CurrentCell.CellProperties;
				switch (code)
				{
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 27:
				case 28:
				case 29:
				case 31:
				case 32:
				case 33:
				case 35:
					break;
				case 0:
					this.SetBorderColor(cellProperties, (string)value);
					break;
				case 1:
					this.SetBorderColor(cellProperties, (string)value, TableData.Positions.Left);
					break;
				case 2:
					this.SetBorderColor(cellProperties, (string)value, TableData.Positions.Right);
					break;
				case 3:
					this.SetBorderColor(cellProperties, (string)value, TableData.Positions.Top);
					break;
				case 4:
					this.SetBorderColor(cellProperties, (string)value, TableData.Positions.Bottom);
					break;
				case 5:
					this.SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value);
					break;
				case 6:
					this.SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Left);
					break;
				case 7:
					this.SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Right);
					break;
				case 8:
					this.SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Top);
					break;
				case 9:
					this.SetBorderStyle(cellProperties, (RPLFormat.BorderStyles)value, TableData.Positions.Bottom);
					break;
				case 10:
					this.SetBorderWidth(cellProperties, (string)value);
					break;
				case 11:
					this.SetBorderWidth(cellProperties, (string)value, TableData.Positions.Left);
					break;
				case 12:
					this.SetBorderWidth(cellProperties, (string)value, TableData.Positions.Right);
					break;
				case 13:
					this.SetBorderWidth(cellProperties, (string)value, TableData.Positions.Top);
					break;
				case 14:
					this.SetBorderWidth(cellProperties, (string)value, TableData.Positions.Bottom);
					break;
				case 26:
					this.SetVerticalAlign((RPLFormat.VerticalAlignments)value);
					break;
				case 30:
					this.SetWritingMode((RPLFormat.WritingModes)value);
					break;
				case 34:
					this.SetShading(cellProperties, (string)value);
					break;
				}
			}
		}

		public void AddPadding(int cellIndex, byte code, object value, int defaultValue)
		{
			double num = new RPLReportSize((string)value).ToPoints();
			switch (code)
			{
			case 15:
				this._document.TableContext.CurrentCell.CellProperties.PaddingLeft = num;
				break;
			case 16:
				this._document.TableContext.CurrentCell.CellProperties.PaddingRight = num;
				break;
			case 17:
				this._document.TableContext.CurrentCell.CellProperties.PaddingTop = num;
				this._document.TableContext.CurrentRow.RowProperties.SetCellPaddingTop(num);
				break;
			case 18:
				this._document.TableContext.CurrentCell.CellProperties.PaddingBottom = num;
				this._document.TableContext.CurrentRow.RowProperties.SetCellPaddingBottom(num);
				break;
			}
		}

		public void ApplyCellBorderContext(BorderContext borderContext)
		{
			OpenXmlTableCellModel currentCell = this._document.TableContext.CurrentCell;
			if (borderContext.Top)
			{
				currentCell.UseTopTableBorder();
			}
			if (borderContext.Bottom)
			{
				currentCell.UseBottomTableBorder();
			}
			if (borderContext.Left)
			{
				currentCell.UseLeftTableBorder();
			}
			if (borderContext.Right)
			{
				currentCell.UseRightTableBorder();
			}
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
					this.SetTextDecoration((RPLFormat.TextDecorations)value);
					break;
				case 27:
					if (value is string)
					{
						this.SetTextColor((string)value);
					}
					else if (value is Color)
					{
						this.SetTextColor((Color)value);
					}
					break;
				case 28:
					this.SetLineHeight(value as string);
					break;
				case 29:
					this.SetParagraphDirection((RPLFormat.Directions)value);
					break;
				case 31:
					this.SetUnicodeBiDi((RPLFormat.UnicodeBiDiTypes)value);
					break;
				case 32:
					this.SetLanguage(value as string);
					break;
				}
			}
		}

		public void AddFirstLineIndent(float indent)
		{
			if (indent >= 0.0)
			{
				this.GetCurrentParagraph().Properties.Indentation.First = (double)indent;
			}
			else
			{
				this.GetCurrentParagraph().Properties.Indentation.Hanging = 0.0 - indent;
			}
		}

		public void AddLeftIndent(float margin)
		{
			this.GetCurrentParagraph().Properties.Indentation.Left = (double)margin;
		}

		public void AddRightIndent(float margin)
		{
			this.GetCurrentParagraph().Properties.Indentation.Right = (double)margin;
		}

		public void AddSpaceBefore(float space)
		{
			this._currentParagraph.Properties.PointsBefore = (double)space;
		}

		public void AddSpaceAfter(float space)
		{
			this._currentParagraph.Properties.PointsAfter = (double)space;
		}

		public void RenderTextRunDirection(RPLFormat.Directions direction)
		{
			this.SetTextDirection(direction);
		}

		public void RenderTextAlign(TypeCode type, RPLFormat.TextAlignments textAlignments, RPLFormat.Directions direction)
		{
			OpenXmlParagraphPropertiesModel.HorizontalAlignment horizontalAlign = OpenXmlParagraphPropertiesModel.HorizontalAlignment.Left;
			if (textAlignments == RPLFormat.TextAlignments.General)
			{
				textAlignments = (RPLFormat.TextAlignments)((!WordOpenXmlUtils.GetTextAlignForType(type)) ? 1 : 3);
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
				horizontalAlign = OpenXmlParagraphPropertiesModel.HorizontalAlignment.Left;
				break;
			case RPLFormat.TextAlignments.Center:
				horizontalAlign = OpenXmlParagraphPropertiesModel.HorizontalAlignment.Center;
				break;
			case RPLFormat.TextAlignments.Right:
				horizontalAlign = OpenXmlParagraphPropertiesModel.HorizontalAlignment.Right;
				break;
			}
			this.GetCurrentParagraph().Properties.HorizontalAlign = horizontalAlign;
		}

		public void RenderFontWeight(RPLFormat.FontWeights fontWeights, RPLFormat.Directions dir)
		{
			this.GetCurrentTextStyle().SetBold((int)fontWeights >= 5, dir == RPLFormat.Directions.RTL);
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
			this.GetCurrentTextStyle().SetSize(new RPLReportSize(size).ToPoints(), dir == RPLFormat.Directions.RTL);
		}

		public void RenderFontFamily(string font, RPLFormat.Directions dir)
		{
			this.GetCurrentTextStyle().SetFont(font, dir == RPLFormat.Directions.RTL);
		}

		public void RenderFontStyle(RPLFormat.FontStyles value, RPLFormat.Directions dir)
		{
			this.GetCurrentTextStyle().SetItalic(value == RPLFormat.FontStyles.Italic, dir == RPLFormat.Directions.RTL);
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
			this.EndAndWriteCurrentParagraph(true);
		}

		public void WriteListEnd(int level, RPLFormat.ListStyles listStyle, bool endParagraph)
		{
			if (listStyle != 0)
			{
				OpenXmlParagraphModel currentParagraph = this.GetCurrentParagraph();
				currentParagraph.Properties.ListLevel = level - 1;
				currentParagraph.Properties.ListStyleId = ((listStyle == RPLFormat.ListStyles.Bulleted) ? 1 : this._numberedListId);
			}
			if (endParagraph)
			{
				this.WriteParagraphEnd();
			}
		}

		public void InitListLevels()
		{
		}

		public void ResetListlevels()
		{
			this._numberedListId = this._document.ListManager.RegisterNewNumberedList();
		}

		public void WriteTableCellEnd(int cellIndex, BorderContext borderContext, bool emptyLayoutCell)
		{
			this.EndAndWriteCurrentParagraph(false);
			this._document.TableContext.WriteTableCellEnd(cellIndex, borderContext, emptyLayoutCell || this._forceEmptyLayoutCell);
			this._forceEmptyLayoutCell = false;
		}

		public void WriteEmptyStyle()
		{
			this._forceEmptyLayoutCell = true;
		}

		public void WriteTableBegin(float left, bool layoutTable)
		{
			this._document.TableContext.WriteTableBegin(left, layoutTable, this.AutoFit);
		}

		public void WriteTableRowBegin(float left, float height, float[] columnWidths)
		{
			this._document.TableContext.WriteTableRowBegin(left, height, columnWidths);
		}

		public void IgnoreRowHeight(bool canGrow)
		{
			this._document.TableContext.CurrentRow.RowProperties.IgnoreRowHeight = canGrow;
		}

		public void SetWriteExactRowHeight(bool writeExactRowHeight)
		{
			this._document.TableContext.CurrentRow.RowProperties.ExactRowHeight = writeExactRowHeight;
		}

		public void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge)
		{
			this._document.TableContext.WriteTableCellBegin(cellIndex, numColumns, firstVertMerge, firstHorzMerge, vertMerge, horzMerge);
		}

		public void WriteTableRowEnd()
		{
			this._document.TableContext.WriteTableRowEnd();
		}

		public void WriteTableEnd()
		{
			this._document.TableContext.WriteTableEnd();
		}

		public void Finish(string title, string author, string comments)
		{
			this._document.WriteDocumentProperties(title, author, comments);
			this._document.Save();
		}

		public int WriteFont(string fontName)
		{
			return 0;
		}

		public void RenderBookmark(string name)
		{
			this.GetCurrentParagraph().AddBookmark(name, this._bookmarkId++);
		}

		public void RenderLabel(string label, int level)
		{
			this.GetCurrentParagraph().AddLabel(label, level, this.GetCurrentTextStyle());
		}

		public void WritePageNumberField()
		{
			this.GetCurrentParagraph().AddPageNumberField(this.GetCurrentTextStyle());
		}

		public void WriteTotalPagesField()
		{
			this.GetCurrentParagraph().AddPageCountField(this.GetCurrentTextStyle());
		}

		public void AddListStyle(int level, bool bulleted)
		{
		}

		public void WriteCellDiagonal(int cellIndex, RPLFormat.BorderStyles style, string width, string color, bool slantUp)
		{
			OpenXmlTableCellModel currentCell = this._document.TableContext.CurrentCell;
			OpenXmlBorderPropertiesModel openXmlBorderPropertiesModel = slantUp ? currentCell.CellProperties.BorderDiagonalUp : currentCell.CellProperties.BorderDiagonalDown;
			openXmlBorderPropertiesModel.Color = WordOpenXmlUtils.RgbColor(new RPLReportColor(color).ToColor());
			openXmlBorderPropertiesModel.Style = this.RPLFormatToBorderStyle(style);
			openXmlBorderPropertiesModel.WidthInEighthPoints = (int)Math.Floor(new RPLReportSize(width).ToPoints() * 8.0);
		}

		public void WritePageBreak()
		{
			this.EndAndWriteCurrentParagraph(false);
			if (this._document.TableContext.Location == TableContext.State.InCell)
			{
				this._document.TableContext.CurrentCell.AddContent(new OpenXmlParagraphModel.PageBreakParagraph());
			}
			else
			{
				this._document.WritePageBreak();
			}
		}

		public void WriteEndSection()
		{
			this._document.WriteSectionBreak();
		}

		public void ClearCellBorder(TableData.Positions position)
		{
			this.SetBorderStyle(this._document.TableContext.CurrentCell.CellProperties, RPLFormat.BorderStyles.None, position);
			this._document.TableContext.CurrentCell.BlockBorderAt(position);
		}

		public void StartHeader()
		{
			this._document.StartHeader();
		}

		public void StartHeader(bool firstPage)
		{
			if (firstPage)
			{
				this._document.StartFirstPageHeader();
			}
			else
			{
				this._document.StartHeader();
			}
		}

		public void FinishHeader()
		{
			this._document.FinishHeader();
		}

		public void StartFooter()
		{
			this._document.StartFooter();
		}

		public void StartFooter(bool firstPage)
		{
			if (firstPage)
			{
				this._document.StartFirstPageFooter();
			}
			else
			{
				this._document.StartFooter();
			}
		}

		public void FinishFooter()
		{
			this._document.FinishFooter();
		}

		private void InitCache(CreateAndRegisterStream streamDelegate)
		{
			if (this._scalabilityCache == null)
			{
				this._scalabilityCache = (ScalabilityCache)ScalabilityUtils.CreateCacheForTransientAllocations(streamDelegate, "Word", AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.StorageObjectCreator.Instance, WordRendererReferenceCreator.Instance, ComponentType.Rendering, 1);
			}
		}

		public void Dispose()
		{
			if (this._scalabilityCache != null)
			{
				this._scalabilityCache.Dispose();
			}
		}

		public void InitHeaderFooter()
		{
		}

		public void FinishHeader(int section)
		{
		}

		public void FinishFooter(int section)
		{
		}

		public void FinishHeader(int section, Word97Writer.HeaderFooterLocation location)
		{
		}

		public void FinishFooter(int section, Word97Writer.HeaderFooterLocation location)
		{
		}

		public void FinishHeaderFooterRegion(int section, int index)
		{
		}

		public void FinishHeadersFooters(bool hasTitlePage)
		{
		}
	}
}
