using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Description("Represents a cell of a legend item.")]
	internal class LegendCell : NamedElement, IToolTipProvider, IImageMapProvider
	{
		private Legend legend;

		private LegendItem legendItem;

		private LegendCellType cellType;

		private string text = string.Empty;

		private Color textColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Font font;

		private string image = string.Empty;

		private Color imageTranspColor = Color.Empty;

		private Size imageSize = Size.Empty;

		private Size symbolSize = new Size(200, 70);

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private int cellSpan = 1;

		private string toolTip = string.Empty;

		private Margins margins = new Margins(0, 0, 15, 15);

		private int rowIndex = -1;

		private int columnIndex = -1;

		internal Rectangle cellPosition = Rectangle.Empty;

		internal Rectangle cellPositionWithMargins = Rectangle.Empty;

		private Size cachedCellSize = Size.Empty;

		private int cachedCellSizeFontReducedBy;

		private string href = string.Empty;

		private string cellAttributes = string.Empty;

		private object mapAreaTag;

		[SRCategory("CategoryAttribute_Data")]
		[Browsable(true)]
		[SRDescription("DescriptionAttributeLegendCell_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[DefaultValue(LegendCellType.Text)]
		[SRCategory("CategoryAttribute_Appearance")]
		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeLegendCell_CellType")]
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

		[SRDescription("DescriptionAttributeLegendCell_Text")]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLegendCell_TextColor")]
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
		[SRCategory("CategoryAttribute_Appearance")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLegendCell_Font")]
		[DefaultValue(null)]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLegendCell_Image")]
		[DefaultValue("")]
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
		[SRDescription("DescriptionAttributeLegendCell_ImageTranspColor")]
		[SRCategory("CategoryAttribute_Appearance")]
		public virtual Color ImageTranspColor
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

		[DefaultValue(typeof(Size), "0, 0")]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLegendCell_ImageSize")]
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
				throw new ArgumentException("Cell image width and height cannot be negative numbers.", "ImageSize");
			}
		}

		[SRDescription("DescriptionAttributeLegendCell_SymbolSize")]
		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(typeof(Size), "200, 70")]
		public virtual Size SymbolSize
		{
			get
			{
				return this.symbolSize;
			}
			set
			{
				if (value.Width >= 0 && value.Height >= 0)
				{
					this.symbolSize = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException("Cell symbol width and height cannot be negative numbers.", "SeriesSymbolSize");
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLegendCell_Alignment")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
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
		[SRDescription("DescriptionAttributeLegendCell_CellSpan")]
		[SRCategory("CategoryAttribute_Layout")]
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
					throw new ArgumentException("Cell span must be equal or more than one.", "CellSpan");
				}
				this.cellSpan = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(typeof(Margins), "0,0,15,15")]
		[SRDescription("DescriptionAttributeLegendCell_Margins")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

		[DefaultValue("")]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegendCell_ToolTip")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LegendItem LegendItem
		{
			get
			{
				return this.legendItem;
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegendCell_Href")]
		[NotifyParentProperty(true)]
		public string Href
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegendCell_CellAttributes")]
		[DefaultValue("")]
		public virtual string CellAttributes
		{
			get
			{
				return this.cellAttributes;
			}
			set
			{
				this.cellAttributes = value;
			}
		}

		object IImageMapProvider.Tag
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

		internal override string DefaultName
		{
			get
			{
				return "Cell";
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (this.margins != null)
				{
					this.margins.Common = value;
				}
			}
		}

		public LegendCell()
			: base(null)
		{
			this.Intitialize(LegendCellType.Text, string.Empty, ContentAlignment.MiddleCenter);
		}

		public LegendCell(string text)
			: base(null)
		{
			this.Intitialize(LegendCellType.Text, text, ContentAlignment.MiddleCenter);
		}

		public LegendCell(LegendCellType cellType, string text)
			: base(null)
		{
			this.Intitialize(cellType, text, ContentAlignment.MiddleCenter);
		}

		public LegendCell(LegendCellType cellType, string text, ContentAlignment alignment)
			: base(null)
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
		protected bool ShouldSerializeMargins()
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

		internal void SetCellPosition(MapGraphics g, int columnIndex, int rowIndex, Rectangle position, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
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
				this.cellPosition.Height -= this.GetLegend().GetSeparatorSize(g, this.legendItem.Separator).Height;
			}
		}

		internal Size MeasureCell(MapGraphics g, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			if (this.cachedCellSizeFontReducedBy == fontSizeReducedBy && !this.cachedCellSize.IsEmpty)
			{
				return this.cachedCellSize;
			}
			Size result = Size.Empty;
			bool flag = false;
			Font cellFont = this.GetCellFont(legendAutoFont, fontSizeReducedBy, out flag);
			if (this.CellType == LegendCellType.Symbol)
			{
				result.Width = (int)((float)(Math.Abs(this.SymbolSize.Width) * singleWCharacterSize.Width) / 100.0);
				result.Height = (int)((float)(Math.Abs(this.SymbolSize.Height) * singleWCharacterSize.Height) / 100.0);
				result.Width = (int)Math.Round((double)result.Width * 1.1);
				result.Height = (int)Math.Round((double)result.Height * 1.25);
				goto IL_01dd;
			}
			if (this.CellType == LegendCellType.Image)
			{
				if (this.ImageSize.IsEmpty && !string.IsNullOrEmpty(this.Image))
				{
					Image image = this.GetLegend().Common.ImageLoader.LoadImage(this.Image);
					result.Width = image.Width;
					result.Height = image.Height;
				}
				else
				{
					result.Width = (int)((float)(Math.Abs(this.ImageSize.Width) * singleWCharacterSize.Width) / 100.0);
					result.Height = (int)((float)(Math.Abs(this.ImageSize.Height) * singleWCharacterSize.Height) / 100.0);
				}
				goto IL_01dd;
			}
			if (this.CellType == LegendCellType.Text)
			{
				string cellText = this.GetCellText();
				result = g.MeasureStringAbs(cellText + "I", cellFont);
				goto IL_01dd;
			}
			throw new InvalidOperationException("Unknown Legend Cell Type: " + ((Enum)(object)this.CellType).ToString((IFormatProvider)CultureInfo.CurrentCulture));
			IL_01dd:
			result.Width += (int)((float)((this.Margins.Left + this.Margins.Right) * singleWCharacterSize.Width) / 100.0);
			result.Height += (int)((float)((this.Margins.Top + this.Margins.Bottom) * singleWCharacterSize.Height) / 100.0);
			if (this.GetLegend() != null && this.legendItem != null && this.legendItem.Separator != 0)
			{
				result.Height += this.GetLegend().GetSeparatorSize(g, this.legendItem.Separator).Height;
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
					result = ((!this.GetLegend().InterlacedRowsColor.IsEmpty) ? this.GetLegend().InterlacedRowsColor : ((!(this.GetLegend().BackColor == Color.Empty)) ? ((!(this.GetLegend().BackColor == Color.Transparent)) ? MapGraphics.GetGradientColor(this.GetLegend().BackColor, Color.Black, 0.2) : ((!(this.GetLegend().Common.MapCore.BackColor != Color.Transparent) || !(this.GetLegend().Common.MapCore.BackColor != Color.Black)) ? Color.LightGray : MapGraphics.GetGradientColor(this.GetLegend().Common.MapCore.BackColor, Color.Black, 0.2))) : Color.LightGray));
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

		private string GetCellText()
		{
			string text = this.Text.Replace("\\n", "\n");
			text = ((this.legendItem == null) ? text.Replace("#LEGENDTEXT", "") : text.Replace("#LEGENDTEXT", this.legendItem.Text));
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
				return this.GetLegend().TextColor;
			}
			return Color.Black;
		}

		internal override void Invalidate()
		{
			if (this.legend != null)
			{
				this.legend.Invalidate();
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

		internal void Paint(MapGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			if (this.cellPosition.Width > 0 && this.cellPosition.Height > 0 && this.GetLegend().Common.ProcessModePaint)
			{
				Color cellBackColor = this.GetCellBackColor();
				if (!cellBackColor.IsEmpty)
				{
					chartGraph.FillRectangleRel(chartGraph.GetRelativeRectangle(this.cellPositionWithMargins), cellBackColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, MapDashStyle.None, Color.Empty, 0, PenAlignment.Inset);
				}
				if (this.GetLegend().Common.ProcessModePaint)
				{
					switch (this.CellType)
					{
					case LegendCellType.Text:
						this.PaintCellText(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize);
						break;
					case LegendCellType.Image:
						this.PaintCellImage(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize);
						break;
					case LegendCellType.Symbol:
						this.PaintCellSeriesSymbol(chartGraph, fontSizeReducedBy, legendAutoFont, singleWCharacterSize);
						break;
					default:
						throw new InvalidOperationException("Unknown legend cell type: '" + ((Enum)(object)this.CellType).ToString((IFormatProvider)CultureInfo.CurrentCulture) + "'.");
					}
				}
			}
		}

		private void PaintCellText(MapGraphics g, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			bool flag = false;
			Font cellFont = this.GetCellFont(legendAutoFont, fontSizeReducedBy, out flag);
			g.StartHotRegion(this);
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
				SizeF sizeF = g.MeasureStringAbs(this.GetCellText(), cellFont, new SizeF(10000f, 10000f), stringFormat);
				if (sizeF.Height > (float)this.cellPosition.Height && (stringFormat.FormatFlags & StringFormatFlags.LineLimit) != 0)
				{
					stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
				}
				else if (sizeF.Height < (float)this.cellPosition.Height && (stringFormat.FormatFlags & StringFormatFlags.LineLimit) == (StringFormatFlags)0)
				{
					stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
				}
				g.DrawStringRel(this.GetCellText(), cellFont, brush, g.GetRelativeRectangle(this.cellPosition), stringFormat);
			}
			g.EndHotRegion();
			if (flag)
			{
				cellFont.Dispose();
				cellFont = null;
			}
		}

		private void PaintCellImage(MapGraphics chartGraph, int fontSizeReducedBy, Font legendAutoFont, Size singleWCharacterSize)
		{
			if (!string.IsNullOrEmpty(this.Image))
			{
				Rectangle empty = Rectangle.Empty;
				Image image = this.GetLegend().Common.ImageLoader.LoadImage(this.Image);
				empty.Width = image.Size.Width;
				empty.Height = image.Size.Height;
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
				if (this.ImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(this.ImageTranspColor, this.ImageTranspColor, ColorAdjustType.Default);
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

		private void PaintCellSeriesSymbol(MapGraphics g, int fontSizeReducedBy, Font legendAutoFont, SizeF singleWCharacterSize)
		{
			Rectangle r = this.cellPosition;
			if (this.SymbolSize.Width >= 0)
			{
				int num = (int)((float)this.SymbolSize.Width * singleWCharacterSize.Width / 100.0);
				if (num > this.cellPosition.Width)
				{
					num = this.cellPosition.Width;
				}
				r.Width = num;
			}
			if (this.SymbolSize.Height >= 0)
			{
				int num2 = (int)((float)this.SymbolSize.Height * singleWCharacterSize.Height / 100.0);
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
				g.StartHotRegion(this);
				if (!string.IsNullOrEmpty(this.legendItem.Image))
				{
					Rectangle empty = Rectangle.Empty;
					Image image = this.GetLegend().Common.ImageLoader.LoadImage(this.legendItem.Image);
					empty.Width = image.Size.Width;
					empty.Height = image.Size.Height;
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
					if (this.legendItem.ImageTranspColor != Color.Empty)
					{
						imageAttributes.SetColorKey(this.legendItem.ImageTranspColor, this.legendItem.ImageTranspColor, ColorAdjustType.Default);
					}
					g.DrawImage(image, empty, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				}
				else if (this.legendItem.ItemStyle == LegendItemStyle.Shape)
				{
					g.FillRectangleRel(g.GetRelativeRectangle(r), this.legendItem.Color, this.legendItem.HatchStyle, this.legendItem.Image, this.legendItem.ImageWrapMode, this.legendItem.ImageTranspColor, this.legendItem.ImageAlign, this.legendItem.GradientType, this.legendItem.SecondaryColor, this.legendItem.borderColor, (this.legendItem.BorderWidth > 2) ? 2 : this.legendItem.BorderWidth, this.legendItem.BorderStyle, this.legendItem.ShadowColor, (this.legendItem.ShadowOffset > 3) ? 3 : this.legendItem.ShadowOffset, PenAlignment.Center);
				}
				else if (this.legendItem.ItemStyle == LegendItemStyle.Path)
				{
					Point location = r.Location;
					location.Y = (int)Math.Round((float)r.Y + (float)r.Height / 2.0);
					Point pt = new Point(r.Right, location.Y);
					int num4 = (int)Math.Round((float)this.legendItem.PathWidth / 2.0);
					location.X += num4;
					pt.X -= num4;
					SmoothingMode smoothingMode = g.SmoothingMode;
					if (this.legendItem.PathWidth < 2 && this.legendItem.BorderWidth < 2)
					{
						g.SmoothingMode = SmoothingMode.None;
					}
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddLine(location, pt);
						int num5 = (this.legendItem.shadowOffset > 3) ? 3 : this.legendItem.shadowOffset;
						if (num5 > 0)
						{
							using (Pen pen = Path.GetColorPen(g.GetShadowColor(), (float)this.legendItem.PathWidth, (float)this.legendItem.BorderWidth))
							{
								if (pen != null)
								{
									Matrix matrix = new Matrix();
									matrix.Translate((float)num5, (float)num5, MatrixOrder.Append);
									graphicsPath.Transform(matrix);
									g.DrawPath(pen, graphicsPath);
									matrix.Reset();
									matrix.Translate((float)(-num5), (float)(-num5), MatrixOrder.Append);
									graphicsPath.Transform(matrix);
								}
							}
						}
						if (this.legendItem.BorderWidth > 0)
						{
							using (Pen pen2 = Path.GetColorPen(this.legendItem.BorderColor, (float)this.legendItem.PathWidth, (float)this.legendItem.BorderWidth))
							{
								if (pen2 != null)
								{
									g.DrawPath(pen2, graphicsPath);
								}
							}
						}
						RectangleF bounds = graphicsPath.GetBounds();
						bounds.Inflate((float)((float)this.legendItem.PathWidth / 2.0), (float)((float)this.legendItem.PathWidth / 2.0));
						using (Pen pen3 = Path.GetFillPen(g, graphicsPath, bounds, (float)this.legendItem.PathWidth, this.legendItem.PathLineStyle, this.legendItem.Color, this.legendItem.SecondaryColor, this.legendItem.GradientType, this.legendItem.HatchStyle))
						{
							if (pen3 != null)
							{
								g.DrawPath(pen3, graphicsPath);
								if (pen3.Brush != null)
								{
									pen3.Brush.Dispose();
								}
							}
						}
					}
					g.SmoothingMode = smoothingMode;
				}
				else if (this.legendItem.ItemStyle == LegendItemStyle.Symbol)
				{
					MarkerStyle markerStyle = this.legendItem.markerStyle;
					if (markerStyle != 0 || !string.IsNullOrEmpty(this.legendItem.markerImage))
					{
						int num6 = Math.Min(r.Width, r.Height);
						int num7 = (this.legendItem.MarkerBorderWidth > 3) ? 3 : this.legendItem.MarkerBorderWidth;
						if (num7 > 0)
						{
							num6 -= num7;
							if (num6 < 1)
							{
								num6 = 1;
							}
						}
						Point point = default(Point);
						point.X = (int)((float)r.X + (float)r.Width / 2.0);
						point.Y = (int)((float)r.Y + (float)r.Height / 2.0);
						Rectangle empty2 = Rectangle.Empty;
						if (!string.IsNullOrEmpty(this.legendItem.markerImage))
						{
							Image image2 = this.GetLegend().Common.ImageLoader.LoadImage(this.legendItem.markerImage);
							empty2.Width = image2.Size.Width;
							empty2.Height = image2.Size.Height;
							float num8 = 1f;
							if (empty2.Height > r.Height)
							{
								num8 = (float)empty2.Height / (float)r.Height;
							}
							if (empty2.Width > r.Width)
							{
								num8 = Math.Max(num8, (float)empty2.Width / (float)r.Width);
							}
							empty2.Height = (int)((float)empty2.Height / num8);
							empty2.Width = (int)((float)empty2.Width / num8);
						}
						Color color = (this.legendItem.markerColor == Color.Empty) ? this.legendItem.Color : this.legendItem.markerColor;
						if (Symbol.IsXamlMarker(markerStyle))
						{
							RectangleF rect = r;
							if (rect.Width > rect.Height)
							{
								rect.X += (float)((rect.Width - rect.Height) / 2.0);
								rect.Width = rect.Height;
							}
							else if (rect.Height > rect.Width)
							{
								rect.Y += (float)((rect.Height - rect.Width) / 2.0);
								rect.Height = rect.Width;
							}
							using (XamlRenderer xamlRenderer = Symbol.CreateXamlRenderer(markerStyle, color, rect))
							{
								XamlLayer[] layers = xamlRenderer.Layers;
								foreach (XamlLayer xamlLayer in layers)
								{
									xamlLayer.Render(g);
								}
							}
						}
						else
						{
							PointF absolute = new PointF((float)point.X, (float)point.Y);
							if ((double)(num6 % 2) != 0.0)
							{
								absolute.X -= 0.5f;
								absolute.Y -= 0.5f;
							}
							g.DrawMarkerRel(g.GetRelativePoint(absolute), markerStyle, num6, color, this.legendItem.MarkerGradientType, this.legendItem.MarkerHatchStyle, this.legendItem.MarkerSecondaryColor, this.legendItem.MarkerBorderStyle, (this.legendItem.markerBorderColor == Color.Empty) ? this.legendItem.borderColor : this.legendItem.markerBorderColor, num7, this.legendItem.markerImage, this.legendItem.markerImageTranspColor, (this.legendItem.shadowOffset > 3) ? 3 : this.legendItem.shadowOffset, this.legendItem.shadowColor, empty2);
						}
					}
				}
				g.EndHotRegion();
			}
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip();
		}

		string IImageMapProvider.GetHref()
		{
			if (!string.IsNullOrEmpty(this.href))
			{
				return this.Href;
			}
			if (this.legendItem != null)
			{
				return this.legendItem.Href;
			}
			return string.Empty;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (this.CellAttributes.Length > 0)
			{
				return this.CellAttributes;
			}
			if (this.legendItem != null)
			{
				return this.legendItem.MapAreaAttributes;
			}
			return string.Empty;
		}

		string IToolTipProvider.GetToolTip()
		{
			if (!string.IsNullOrEmpty(this.ToolTip))
			{
				return this.ToolTip;
			}
			if (this.legendItem != null)
			{
				return this.legendItem.ToolTip;
			}
			return string.Empty;
		}
	}
}
