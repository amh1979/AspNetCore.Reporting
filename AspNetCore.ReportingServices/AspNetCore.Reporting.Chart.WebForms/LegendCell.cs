using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCell_LegendCell")]
	internal class LegendCell : IMapAreaAttributes
	{
		private Legend legend;

		private LegendItem legendItem;

		private string name = string.Empty;

		private LegendCellType cellType;

		private string text = string.Empty;

		private Color textColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Font font;

		private string image = string.Empty;

		private Color imageTranspColor = Color.Empty;

		private Size imageSize = Size.Empty;

		private Size seriesSymbolSize = new Size(200, 70);

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private int cellSpan = 1;

		private string toolTip = string.Empty;

		private Margins margins = new Margins(0, 0, 15, 15);

		private string href = string.Empty;

		private int rowIndex = -1;

		private int columnIndex = -1;

		private string mapAreaAttribute = string.Empty;

		internal Rectangle cellPosition = Rectangle.Empty;

		internal Rectangle cellPositionWithMargins = Rectangle.Empty;

		private Size cachedCellSize = Size.Empty;

		private int cachedCellSizeFontReducedBy;

		private object mapAreaTag;

		[SRDescription("DescriptionAttributeLegendCell_Name")]
		[SRCategory("CategoryAttributeMisc")]
		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (!(this.name != value))
				{
					return;
				}
				if (this.legendItem != null)
				{
					foreach (LegendCell cell in this.legendItem.Cells)
					{
						if (cell.Name == value)
						{
							throw new ArgumentException(SR.ExceptionLegendCellNameAlreadyExistsInCollection(value));
						}
					}
				}
				if (value != null && value.Length != 0)
				{
					this.name = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionLegendCellNameIsEmpty);
			}
		}

		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeLegendCell_CellType")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(LegendCellType.Text)]
		public virtual LegendCellType CellType
		{
			get
			{
				return this.cellType;
			}
			set
			{
				this.cellType = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendCell_Text")]
		[DefaultValue("")]
		public virtual string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendCell_TextColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		public virtual Color TextColor
		{
			get
			{
				return this.textColor;
			}
			set
			{
				this.textColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendCell_BackColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		public virtual Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeLegendCell_Font")]
		public virtual Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.Invalidate();
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendCell_Image")]
		public virtual string Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendCell_ImageTransparentColor")]
		public virtual Color ImageTransparentColor
		{
			get
			{
				return this.imageTranspColor;
			}
			set
			{
				this.imageTranspColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendCell_ImageSize")]
		[SRCategory("CategoryAttributeLayout")]
		[DefaultValue(typeof(Size), "0, 0")]
		[TypeConverter(typeof(SizeEmptyValueConverter))]
		public virtual Size ImageSize
		{
			get
			{
				return this.imageSize;
			}
			set
			{
				if (value.Width >= 0 && value.Height >= 0)
				{
					this.imageSize = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.ExceptionLegendCellImageSizeIsNegative, "value");
			}
		}

		[SRDescription("DescriptionAttributeLegendCell_SeriesSymbolSize")]
		[SRCategory("CategoryAttributeLayout")]
		[DefaultValue(typeof(Size), "200, 70")]
		public virtual Size SeriesSymbolSize
		{
			get
			{
				return this.seriesSymbolSize;
			}
			set
			{
				if (value.Width >= 0 && value.Height >= 0)
				{
					this.seriesSymbolSize = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.ExceptionLegendCellSeriesSymbolSizeIsNegative, "value");
			}
		}

		[SRCategory("CategoryAttributeLayout")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[SRDescription("DescriptionAttributeLegendCell_Alignment")]
		public virtual ContentAlignment Alignment
		{
			get
			{
				return this.alignment;
			}
			set
			{
				this.alignment = value;
				this.Invalidate();
			}
		}

		[DefaultValue(1)]
		[SRCategory("CategoryAttributeLayout")]
		[SRDescription("DescriptionAttributeLegendCell_CellSpan")]
		public virtual int CellSpan
		{
			get
			{
				return this.cellSpan;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException(SR.ExceptionLegendCellSpanIsLessThenOne, "value");
				}
				this.cellSpan = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Margins), "0,0,15,15")]
		[SRCategory("CategoryAttributeLayout")]
		[SRDescription("DescriptionAttributeLegendCell_Margins")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[NotifyParentProperty(true)]
		public virtual Margins Margins
		{
			get
			{
				return this.margins;
			}
			set
			{
				this.margins = value;
				this.Invalidate();
				if (this.GetLegend() != null)
				{
					this.margins.Common = this.GetLegend().Common;
				}
			}
		}

		[SRDescription("DescriptionAttributeLegendCell_ToolTip")]
		[SRCategory("CategoryAttributeMapArea")]
		[DefaultValue("")]
		public virtual string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[SRDescription("DescriptionAttributeLegendCell_Href")]
		[DefaultValue("")]
		public virtual string Href
		{
			get
			{
				return this.href;
			}
			set
			{
				this.href = value;
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttributeMapArea")]
		[SRDescription("DescriptionAttributeLegendCell_MapAreaAttributes")]
		public virtual string MapAreaAttributes
		{
			get
			{
				return this.mapAreaAttribute;
			}
			set
			{
				this.mapAreaAttribute = value;
			}
		}

		object IMapAreaAttributes.Tag
		{
			get
			{
				return this.mapAreaTag;
			}
			set
			{
				this.mapAreaTag = value;
			}
		}

		string IMapAreaAttributes.ToolTip
		{
			get
			{
				return this.ToolTip;
			}
			set
			{
				this.ToolTip = value;
			}
		}

		string IMapAreaAttributes.Href
		{
			get
			{
				return this.Href;
			}
			set
			{
				this.Href = value;
			}
		}

		string IMapAreaAttributes.MapAreaAttributes
		{
			get
			{
				return this.MapAreaAttributes;
			}
			set
			{
				this.MapAreaAttributes = value;
			}
		}

		public LegendCell()
		{
			this.Intitialize(LegendCellType.Text, string.Empty, ContentAlignment.MiddleCenter);
		}

		public LegendCell(string text)
		{
			this.Intitialize(LegendCellType.Text, text, ContentAlignment.MiddleCenter);
		}

		public LegendCell(LegendCellType cellType, string text)
		{
			this.Intitialize(cellType, text, ContentAlignment.MiddleCenter);
		}

		public LegendCell(LegendCellType cellType, string text, ContentAlignment alignment)
		{
			this.Intitialize(cellType, text, alignment);
		}

		private void Intitialize(LegendCellType cellType, string text, ContentAlignment alignment)
		{
			this.cellType = cellType;
			if (this.cellType == LegendCellType.Image)
			{
				this.image = text;
			}
			else
			{
				this.text = text;
			}
			this.alignment = alignment;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeMargins()
		{
			if (this.margins.Top == 0 && this.margins.Bottom == 0 && this.margins.Left == 15 && this.margins.Right == 15)
			{
				return false;
			}
			return true;
		}

		internal void ResetCache()
		{
			this.cachedCellSize = Size.Empty;
			this.cachedCellSizeFontReducedBy = 0;
		}

		internal void SetCellPosition(ChartGraphics graph, int columnIndex, int rowIndex, Rectangle position, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			this.cellPosition = position;
			this.cellPositionWithMargins = position;
			this.rowIndex = rowIndex;
			this.columnIndex = columnIndex;
			this.cellPosition.X += (int)((float)(this.Margins.Left * singleWCharacterSize.Width) / 100.0);
			this.cellPosition.Y += (int)((float)(this.Margins.Top * singleWCharacterSize.Height) / 100.0);
			this.cellPosition.Width -= (int)((float)(this.Margins.Left * singleWCharacterSize.Width) / 100.0) + (int)((float)(this.Margins.Right * singleWCharacterSize.Width) / 100.0);
			this.cellPosition.Height -= (int)((float)(this.Margins.Top * singleWCharacterSize.Height) / 100.0) + (int)((float)(this.Margins.Bottom * singleWCharacterSize.Height) / 100.0);
			if (this.GetLegend() != null && this.legendItem != null && this.legendItem.Separator != 0)
			{
				this.cellPosition.Height -= this.GetLegend().GetSeparatorSize(graph, this.legendItem.Separator).Height;
			}
		}

		internal Size MeasureCell(ChartGraphics graph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			if (this.cachedCellSizeFontReducedBy == fontSizeReducedBy && !this.cachedCellSize.IsEmpty)
			{
				return this.cachedCellSize;
			}
			Size result = Size.Empty;
			bool flag = false;
			Font cellFont = this.GetCellFont(legendAutoFont, fontSizeReducedBy, out flag);
			if (this.CellType == LegendCellType.SeriesSymbol)
			{
				result.Width = (int)((float)(Math.Abs(this.SeriesSymbolSize.Width) * singleWCharacterSize.Width) / 100.0);
				result.Height = (int)((float)(Math.Abs(this.SeriesSymbolSize.Height) * singleWCharacterSize.Height) / 100.0);
				goto IL_01ab;
			}
			if (this.CellType == LegendCellType.Image)
			{
				if (this.ImageSize.IsEmpty && this.Image.Length > 0)
				{
					SizeF sizeF = default(SizeF);
					if (this.GetLegend().Common.ImageLoader.GetAdjustedImageSize(this.Image, graph.Graphics, ref sizeF))
					{
						result.Width = (int)sizeF.Width;
						result.Height = (int)sizeF.Height;
					}
				}
				else
				{
					result.Width = (int)((float)(Math.Abs(this.ImageSize.Width) * singleWCharacterSize.Width) / 100.0);
					result.Height = (int)((float)(Math.Abs(this.ImageSize.Height) * singleWCharacterSize.Height) / 100.0);
				}
				goto IL_01ab;
			}
			if (this.CellType == LegendCellType.Text)
			{
				string cellText = this.GetCellText();
				result = graph.MeasureStringAbs(cellText + "I", cellFont);
				goto IL_01ab;
			}
			throw new InvalidOperationException(SR.ExceptionLegendCellTypeUnknown(this.CellType.ToString()));
			IL_01ab:
			result.Width += (int)((float)((this.Margins.Left + this.Margins.Right) * singleWCharacterSize.Width) / 100.0);
			result.Height += (int)((float)((this.Margins.Top + this.Margins.Bottom) * singleWCharacterSize.Height) / 100.0);
			if (this.GetLegend() != null && this.legendItem != null && this.legendItem.Separator != 0)
			{
				result.Height += this.GetLegend().GetSeparatorSize(graph, this.legendItem.Separator).Height;
			}
			if (flag)
			{
				cellFont.Dispose();
				cellFont = null;
			}
			this.cachedCellSize = result;
			this.cachedCellSizeFontReducedBy = fontSizeReducedBy;
			return result;
		}

		private Color GetCellBackColor()
		{
			Color result = this.BackColor;
			if (this.BackColor.IsEmpty && this.GetLegend() != null)
			{
				if (this.legendItem != null)
				{
					int num = this.legendItem.Cells.IndexOf(this);
					if (num >= 0 && num < this.GetLegend().CellColumns.Count && !this.GetLegend().CellColumns[num].BackColor.IsEmpty)
					{
						result = this.GetLegend().CellColumns[num].BackColor;
					}
				}
				if (result.IsEmpty && this.GetLegend().InterlacedRows && this.rowIndex % 2 != 0)
				{
					result = ((!this.GetLegend().InterlacedRowsColor.IsEmpty) ? this.GetLegend().InterlacedRowsColor : ((!(this.GetLegend().BackColor == Color.Empty)) ? ((!(this.GetLegend().BackColor == Color.Transparent)) ? ChartGraphics.GetGradientColor(this.GetLegend().BackColor, Color.Black, 0.2) : ((!(this.GetLegend().Common.Chart.BackColor != Color.Transparent) || !(this.GetLegend().Common.Chart.BackColor != Color.Black)) ? Color.LightGray : ChartGraphics.GetGradientColor(this.GetLegend().Common.Chart.BackColor, Color.Black, 0.2))) : Color.LightGray));
				}
			}
			return result;
		}

		private Font GetCellFont(Font legendAutoFont, int fontSizeReducedBy, out bool disposeFont)
		{
			Font font = this.Font;
			disposeFont = false;
			if (font == null && this.GetLegend() != null)
			{
				if (this.legendItem != null)
				{
					int num = this.legendItem.Cells.IndexOf(this);
					if (num >= 0 && num < this.GetLegend().CellColumns.Count && this.GetLegend().CellColumns[num].Font != null)
					{
						font = this.GetLegend().CellColumns[num].Font;
					}
				}
				if (font == null)
				{
					return legendAutoFont;
				}
			}
			if (font != null && fontSizeReducedBy != 0)
			{
				disposeFont = true;
				int num2 = (int)Math.Round((double)(font.Size - (float)fontSizeReducedBy));
				if (num2 < 1)
				{
					num2 = 1;
				}
				font = new Font(font.FontFamily, (float)num2, font.Style, font.Unit);
			}
			return font;
		}

		private string GetCellToolTip()
		{
			if (this.ToolTip.Length > 0)
			{
				return this.ToolTip;
			}
			if (this.legendItem != null)
			{
				return this.legendItem.ToolTip;
			}
			return string.Empty;
		}

		private string GetCellHref()
		{
			if (this.href.Length > 0)
			{
				return this.href;
			}
			if (this.legendItem != null)
			{
				return this.legendItem.Href;
			}
			return string.Empty;
		}

		private string GetCellMapAreaAttributes()
		{
			if (this.mapAreaAttribute.Length > 0)
			{
				return this.mapAreaAttribute;
			}
			if (this.legendItem != null)
			{
				return this.legendItem.MapAreaAttributes;
			}
			return string.Empty;
		}

		private string GetCellText()
		{
			string text = this.Text.Replace("\\n", "\n");
			text = ((this.legendItem == null) ? text.Replace("#LEGENDTEXT", "") : text.Replace("#LEGENDTEXT", this.legendItem.Name));
			if (this.GetLegend() != null)
			{
				int textWrapThreshold = this.GetLegend().TextWrapThreshold;
				if (textWrapThreshold > 0 && text.Length > textWrapThreshold)
				{
					int num = 0;
					for (int i = 0; i < text.Length; i++)
					{
						if (text[i] == '\n')
						{
							num = 0;
						}
						else
						{
							num++;
							if (char.IsWhiteSpace(text, i) && num >= textWrapThreshold)
							{
								num = 0;
								text = text.Substring(0, i) + "\n" + text.Substring(i + 1).TrimStart();
							}
						}
					}
				}
			}
			return text;
		}

		private Color GetCellTextColor()
		{
			if (!this.TextColor.IsEmpty)
			{
				return this.TextColor;
			}
			if (this.GetLegend() != null)
			{
				if (this.legendItem != null)
				{
					int num = this.legendItem.Cells.IndexOf(this);
					if (num >= 0 && num < this.GetLegend().CellColumns.Count && !this.GetLegend().CellColumns[num].TextColor.IsEmpty)
					{
						return this.GetLegend().CellColumns[num].TextColor;
					}
				}
				return this.GetLegend().FontColor;
			}
			return Color.Black;
		}

		protected void Invalidate()
		{
			if (this.legend != null)
			{
				this.legend.Invalidate(false);
			}
		}

		internal void SetContainingLegend(Legend legend, LegendItem legendItem)
		{
			this.legend = legend;
			this.legendItem = legendItem;
			if (this.legend != null)
			{
				this.margins.Common = this.legend.Common;
			}
		}

		public virtual Legend GetLegend()
		{
			if (this.legend != null)
			{
				return this.legend;
			}
			if (this.legendItem != null)
			{
				return this.legendItem.Legend;
			}
			return null;
		}

		public virtual LegendItem GetLegendItem()
		{
			return this.legendItem;
		}

		internal void Paint(ChartGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize, PointF animationLocationAdjustment)
		{
			if (this.cellPosition.Width > 0 && this.cellPosition.Height > 0)
			{
				if (this.GetLegend().Common.ProcessModePaint)
				{
					Color cellBackColor = this.GetCellBackColor();
					RectangleF relativeRectangle = chartGraph.GetRelativeRectangle(this.cellPositionWithMargins);
					if (!cellBackColor.IsEmpty)
					{
						chartGraph.FillRectangleRel(relativeRectangle, cellBackColor, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, ChartDashStyle.NotSet, Color.Empty, 0, PenAlignment.Inset);
					}
					this.GetLegend().Common.EventsManager.OnBackPaint(this, new ChartPaintEventArgs(chartGraph, this.GetLegend().Common, new ElementPosition(relativeRectangle.X, relativeRectangle.Y, relativeRectangle.Width, relativeRectangle.Height)));
					switch (this.CellType)
					{
					case LegendCellType.Text:
						this.PaintCellText(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize, animationLocationAdjustment);
						break;
					case LegendCellType.Image:
						this.PaintCellImage(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize, animationLocationAdjustment);
						break;
					case LegendCellType.SeriesSymbol:
						this.PaintCellSeriesSymbol(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize, animationLocationAdjustment);
						break;
					default:
						throw new InvalidOperationException(SR.ExceptionLegendCellTypeUnknown(this.CellType.ToString()));
					}
					this.GetLegend().Common.EventsManager.OnPaint(this, new ChartPaintEventArgs(chartGraph, this.GetLegend().Common, new ElementPosition(relativeRectangle.X, relativeRectangle.Y, relativeRectangle.Width, relativeRectangle.Height)));
				}
				if (this.GetLegend().Common.ProcessModeRegions)
				{
					this.GetLegend().Common.HotRegionsList.AddHotRegion(chartGraph, chartGraph.GetRelativeRectangle(this.cellPositionWithMargins), this.GetCellToolTip(), this.GetCellHref(), this.GetCellMapAreaAttributes(), this.legendItem, this, ChartElementType.LegendItem, this.legendItem.SeriesName);
				}
			}
		}

		private void PaintCellText(ChartGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize, PointF animationLocationAdjustment)
		{
			bool flag = false;
			Font cellFont = this.GetCellFont(legendAutoFont, fontSizeReducedBy, out flag);
			chartGraph.StartHotRegion(this.GetCellHref(), this.GetCellToolTip());
			chartGraph.StartAnimation();
			using (SolidBrush brush = new SolidBrush(this.GetCellTextColor()))
			{
				StringFormat stringFormat = new StringFormat(StringFormat.GenericDefault);
				stringFormat.FormatFlags = StringFormatFlags.LineLimit;
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				stringFormat.Alignment = StringAlignment.Center;
				if (this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.TopLeft)
				{
					stringFormat.Alignment = StringAlignment.Near;
				}
				else if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.MiddleRight || this.Alignment == ContentAlignment.TopRight)
				{
					stringFormat.Alignment = StringAlignment.Far;
				}
				stringFormat.LineAlignment = StringAlignment.Center;
				if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.BottomRight)
				{
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				else if (this.Alignment == ContentAlignment.TopCenter || this.Alignment == ContentAlignment.TopLeft || this.Alignment == ContentAlignment.TopRight)
				{
					stringFormat.LineAlignment = StringAlignment.Near;
				}
				SizeF sizeF = chartGraph.MeasureStringAbs(this.GetCellText(), cellFont, new SizeF(10000f, 10000f), stringFormat);
				if (sizeF.Height > (float)this.cellPosition.Height && (stringFormat.FormatFlags & StringFormatFlags.LineLimit) != 0)
				{
					stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
				}
				else if (sizeF.Height < (float)this.cellPosition.Height && (stringFormat.FormatFlags & StringFormatFlags.LineLimit) == (StringFormatFlags)0)
				{
					stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
				}
				chartGraph.DrawStringRel(this.GetCellText(), cellFont, brush, chartGraph.GetRelativeRectangle(this.cellPosition), stringFormat);
			}
			chartGraph.StopAnimation();
			chartGraph.EndHotRegion();
			if (flag)
			{
				cellFont.Dispose();
				cellFont = null;
			}
		}

		private void PaintCellImage(ChartGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize, PointF animationLocationAdjustment)
		{
			if (this.Image.Length > 0)
			{
				Rectangle empty = Rectangle.Empty;
				Image image = this.GetLegend().Common.ImageLoader.LoadImage(this.Image);
				SizeF sizeF = default(SizeF);
				ImageLoader.GetAdjustedImageSize(image, chartGraph.Graphics, ref sizeF);
				empty.Width = (int)sizeF.Width;
				empty.Height = (int)sizeF.Height;
				Rectangle rectangle = this.cellPosition;
				rectangle.Width = empty.Width;
				rectangle.Height = empty.Height;
				if (!this.ImageSize.IsEmpty)
				{
					if (this.ImageSize.Width > 0)
					{
						int num = (int)((float)(this.ImageSize.Width * singleWCharacterSize.Width) / 100.0);
						if (num > this.cellPosition.Width)
						{
							num = this.cellPosition.Width;
						}
						rectangle.Width = num;
					}
					if (this.ImageSize.Height > 0)
					{
						int num2 = (int)((float)(this.ImageSize.Height * singleWCharacterSize.Height) / 100.0);
						if (num2 > this.cellPosition.Height)
						{
							num2 = this.cellPosition.Height;
						}
						rectangle.Height = num2;
					}
				}
				float num3 = 1f;
				if (empty.Height > rectangle.Height)
				{
					num3 = (float)empty.Height / (float)rectangle.Height;
				}
				if (empty.Width > rectangle.Width)
				{
					num3 = Math.Max(num3, (float)empty.Width / (float)rectangle.Width);
				}
				empty.Height = (int)((float)empty.Height / num3);
				empty.Width = (int)((float)empty.Width / num3);
				empty.X = (int)((float)this.cellPosition.X + (float)this.cellPosition.Width / 2.0 - (float)empty.Width / 2.0);
				empty.Y = (int)((float)this.cellPosition.Y + (float)this.cellPosition.Height / 2.0 - (float)empty.Height / 2.0);
				if (this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.TopLeft)
				{
					empty.X = this.cellPosition.X;
				}
				else if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.MiddleRight || this.Alignment == ContentAlignment.TopRight)
				{
					empty.X = this.cellPosition.Right - empty.Width;
				}
				if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.BottomRight)
				{
					empty.Y = this.cellPosition.Bottom - empty.Height;
				}
				else if (this.Alignment == ContentAlignment.TopCenter || this.Alignment == ContentAlignment.TopLeft || this.Alignment == ContentAlignment.TopRight)
				{
					empty.Y = this.cellPosition.Y;
				}
				ImageAttributes imageAttributes = new ImageAttributes();
				if (this.ImageTransparentColor != Color.Empty)
				{
					imageAttributes.SetColorKey(this.ImageTransparentColor, this.ImageTransparentColor, ColorAdjustType.Default);
				}
				SmoothingMode smoothingMode = chartGraph.SmoothingMode;
				CompositingQuality compositingQuality = chartGraph.Graphics.CompositingQuality;
				InterpolationMode interpolationMode = chartGraph.Graphics.InterpolationMode;
				chartGraph.SmoothingMode = SmoothingMode.AntiAlias;
				chartGraph.Graphics.CompositingQuality = CompositingQuality.HighQuality;
				chartGraph.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				chartGraph.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				chartGraph.SmoothingMode = smoothingMode;
				chartGraph.Graphics.CompositingQuality = compositingQuality;
				chartGraph.Graphics.InterpolationMode = interpolationMode;
			}
		}

		private void PaintCellSeriesSymbol(ChartGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, SizeF singleWCharacterSize, PointF animationLocationAdjustment)
		{
			Rectangle r = this.cellPosition;
			if (this.SeriesSymbolSize.Width >= 0)
			{
				int num = (int)((float)this.SeriesSymbolSize.Width * singleWCharacterSize.Width / 100.0);
				if (num > this.cellPosition.Width)
				{
					num = this.cellPosition.Width;
				}
				r.Width = num;
			}
			if (this.SeriesSymbolSize.Height >= 0)
			{
				int num2 = (int)((float)this.SeriesSymbolSize.Height * singleWCharacterSize.Height / 100.0);
				if (num2 > this.cellPosition.Height)
				{
					num2 = this.cellPosition.Height;
				}
				r.Height = num2;
			}
			if (r.Height > 0 && r.Width > 0)
			{
				r.X = (int)((float)this.cellPosition.X + (float)this.cellPosition.Width / 2.0 - (float)r.Width / 2.0);
				r.Y = (int)((float)this.cellPosition.Y + (float)this.cellPosition.Height / 2.0 - (float)r.Height / 2.0);
				if (this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.TopLeft)
				{
					r.X = this.cellPosition.X;
				}
				else if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.MiddleRight || this.Alignment == ContentAlignment.TopRight)
				{
					r.X = this.cellPosition.Right - r.Width;
				}
				if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.BottomRight)
				{
					r.Y = this.cellPosition.Bottom - r.Height;
				}
				else if (this.Alignment == ContentAlignment.TopCenter || this.Alignment == ContentAlignment.TopLeft || this.Alignment == ContentAlignment.TopRight)
				{
					r.Y = this.cellPosition.Y;
				}
				chartGraph.StartHotRegion(this.GetCellHref(), this.GetCellToolTip());
				chartGraph.StartAnimation();
				if (this.legendItem.Image.Length > 0)
				{
					Rectangle empty = Rectangle.Empty;
					Image image = this.GetLegend().Common.ImageLoader.LoadImage(this.legendItem.Image);
					if (image != null)
					{
						SizeF sizeF = default(SizeF);
						ImageLoader.GetAdjustedImageSize(image, chartGraph.Graphics, ref sizeF);
						empty.Width = (int)sizeF.Width;
						empty.Height = (int)sizeF.Height;
						float num3 = 1f;
						if (empty.Height > r.Height)
						{
							num3 = (float)empty.Height / (float)r.Height;
						}
						if (empty.Width > r.Width)
						{
							num3 = Math.Max(num3, (float)empty.Width / (float)r.Width);
						}
						empty.Height = (int)((float)empty.Height / num3);
						empty.Width = (int)((float)empty.Width / num3);
						empty.X = (int)((float)r.X + (float)r.Width / 2.0 - (float)empty.Width / 2.0);
						empty.Y = (int)((float)r.Y + (float)r.Height / 2.0 - (float)empty.Height / 2.0);
						ImageAttributes imageAttributes = new ImageAttributes();
						if (this.legendItem.BackImageTransparentColor != Color.Empty)
						{
							imageAttributes.SetColorKey(this.legendItem.BackImageTransparentColor, this.legendItem.BackImageTransparentColor, ColorAdjustType.Default);
						}
						chartGraph.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					}
				}
				else
				{
					int num4 = (int)Math.Round(3.0 * chartGraph.Graphics.DpiX / 96.0);
					int num5 = (int)Math.Round(3.0 * chartGraph.Graphics.DpiX / 96.0);
					if (this.legendItem.Style == LegendImageStyle.Rectangle)
					{
						int num6 = (int)Math.Round(2.0 * chartGraph.Graphics.DpiX / 96.0);
						chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(r), this.legendItem.Color, this.legendItem.BackHatchStyle, this.legendItem.Image, this.legendItem.backImageMode, this.legendItem.BackImageTransparentColor, this.legendItem.backImageAlign, this.legendItem.backGradientType, this.legendItem.backGradientEndColor, this.legendItem.borderColor, (this.legendItem.BorderWidth > num6) ? num6 : this.legendItem.BorderWidth, this.legendItem.BorderStyle, this.legendItem.ShadowColor, (this.legendItem.ShadowOffset > num4) ? num4 : this.legendItem.ShadowOffset, PenAlignment.Inset);
					}
					if (this.legendItem.Style == LegendImageStyle.Line)
					{
						Point p = default(Point);
						p.X = r.X;
						p.Y = (int)((float)r.Y + (float)r.Height / 2.0);
						Point p2 = default(Point);
						p2.Y = p.Y;
						p2.X = r.Right;
						SmoothingMode smoothingMode = chartGraph.SmoothingMode;
						chartGraph.SmoothingMode = SmoothingMode.None;
						chartGraph.DrawLineRel(this.legendItem.Color, (this.legendItem.borderWidth > num5) ? num5 : this.legendItem.borderWidth, this.legendItem.borderStyle, chartGraph.GetRelativePoint(p), chartGraph.GetRelativePoint(p2), this.legendItem.shadowColor, (this.legendItem.shadowOffset > num4) ? num4 : this.legendItem.shadowOffset);
						chartGraph.SmoothingMode = smoothingMode;
					}
					if (this.legendItem.Style == LegendImageStyle.Marker || this.legendItem.Style == LegendImageStyle.Line)
					{
						MarkerStyle markerStyle = this.legendItem.markerStyle;
						if (this.legendItem.style == LegendImageStyle.Marker)
						{
							markerStyle = ((this.legendItem.markerStyle == MarkerStyle.None) ? MarkerStyle.Circle : this.legendItem.markerStyle);
						}
						if (markerStyle != 0 || this.legendItem.markerImage.Length > 0)
						{
							int num7 = Math.Min(r.Width, r.Height);
							num7 = (int)Math.Min((float)this.legendItem.markerSize, (float)((this.legendItem.style == LegendImageStyle.Line) ? (2.0 * ((float)num7 / 3.0)) : ((float)num7)));
							int num8 = (this.legendItem.MarkerBorderWidth > num5) ? num5 : this.legendItem.MarkerBorderWidth;
							if (num8 > 0)
							{
								num7 -= num8;
								if (num7 < 1)
								{
									num7 = 1;
								}
							}
							Point point = default(Point);
							point.X = (int)((float)r.X + (float)r.Width / 2.0);
							point.Y = (int)((float)r.Y + (float)r.Height / 2.0);
							Rectangle empty2 = Rectangle.Empty;
							if (this.legendItem.markerImage.Length > 0)
							{
								Image image2 = this.GetLegend().Common.ImageLoader.LoadImage(this.legendItem.markerImage);
								SizeF sizeF2 = default(SizeF);
								ImageLoader.GetAdjustedImageSize(image2, chartGraph.Graphics, ref sizeF2);
								empty2.Width = (int)sizeF2.Width;
								empty2.Height = (int)sizeF2.Height;
								float num9 = 1f;
								if (empty2.Height > r.Height)
								{
									num9 = (float)empty2.Height / (float)r.Height;
								}
								if (empty2.Width > r.Width)
								{
									num9 = Math.Max(num9, (float)empty2.Width / (float)r.Width);
								}
								empty2.Height = (int)((float)empty2.Height / num9);
								empty2.Width = (int)((float)empty2.Width / num9);
							}
							PointF absolute = new PointF((float)point.X, (float)point.Y);
							if ((double)(num7 % 2) != 0.0)
							{
								absolute.X -= 0.5f;
								absolute.Y -= 0.5f;
							}
							chartGraph.DrawMarkerRel(chartGraph.GetRelativePoint(absolute), markerStyle, num7, (this.legendItem.markerColor == Color.Empty) ? this.legendItem.Color : this.legendItem.markerColor, (this.legendItem.markerBorderColor == Color.Empty) ? this.legendItem.borderColor : this.legendItem.markerBorderColor, num8, this.legendItem.markerImage, this.legendItem.markerImageTranspColor, (this.legendItem.shadowOffset > num4) ? num4 : this.legendItem.shadowOffset, this.legendItem.shadowColor, empty2);
						}
					}
				}
				chartGraph.StopAnimation();
				chartGraph.EndHotRegion();
			}
		}
	}
}
