using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeTitle5")]
	internal class Title : IMapAreaAttributes
	{
		private Chart chart;

		internal int titleBorderSpacing = 4;

		private object mapAreaTag;

		private string name = "Chart Title";

		private string text = string.Empty;

		private TextStyle style;

		private ElementPosition position = new ElementPosition();

		private bool visible = true;

		private Color backColor = Color.Empty;

		private ChartHatchStyle backHatchStyle;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private int shadowOffset;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private Color borderColor = Color.Empty;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle = ChartDashStyle.Solid;

		private Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private Color color = Color.Black;

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private Docking docking;

		private string dockToChartArea = "NotSet";

		private bool dockInsideChartArea = true;

		private int dockOffset;

		private string toolTip = string.Empty;

		private string href = string.Empty;

		private string mapAreaAttributes = string.Empty;

		private TextOrientation textOrientation;

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttribute_TextOrientation")]
		[NotifyParentProperty(true)]
		[DefaultValue(TextOrientation.Auto)]
		public TextOrientation TextOrientation
		{
			get
			{
				return this.textOrientation;
			}
			set
			{
				this.textOrientation = value;
				this.Invalidate(true);
			}
		}

		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTitle_Visible")]
		public virtual bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				this.Invalidate(false);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeTitle_Name")]
		[DefaultValue("Chart Title")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name != value && !(this.name == "Default Title") && !(value == "Chart Title"))
				{
					if (value == null || value.Length == 0)
					{
						throw new ArgumentException(SR.ExceptionTitleNameIsEmpty);
					}
					if (this.Chart != null && this.Chart.Titles.IndexOf(value) != -1)
					{
						throw new ArgumentException(SR.ExceptionTitleNameIsNotUnique(value));
					}
					this.name = value;
				}
			}
		}

		[TypeConverter(typeof(LegendAreaNameConverter))]
		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeTitle_DockToChartArea")]
		[SRCategory("CategoryAttributeDocking")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		public string DockToChartArea
		{
			get
			{
				return this.dockToChartArea;
			}
			set
			{
				if (value != this.dockToChartArea)
				{
					if (value.Length == 0)
					{
						this.dockToChartArea = "NotSet";
					}
					else
					{
						this.dockToChartArea = value;
					}
					this.Invalidate(false);
				}
			}
		}

		[SRDescription("DescriptionAttributeTitle_DockInsideChartArea")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeDocking")]
		[NotifyParentProperty(true)]
		[DefaultValue(true)]
		public bool DockInsideChartArea
		{
			get
			{
				return this.dockInsideChartArea;
			}
			set
			{
				if (value != this.dockInsideChartArea)
				{
					this.dockInsideChartArea = value;
					this.Invalidate(false);
				}
			}
		}

		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeTitle_DockOffset")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeDocking")]
		public int DockOffset
		{
			get
			{
				return this.dockOffset;
			}
			set
			{
				if (value != this.dockOffset)
				{
					this.dockOffset = value;
					this.Invalidate(false);
				}
			}
		}

		[SRDescription("DescriptionAttributeTitle_Position")]
		[Bindable(true)]
		[DefaultValue(typeof(ElementPosition), "Auto")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ElementPositionConverter))]
		[SerializationVisibility(SerializationVisibility.Element)]
		[SRCategory("CategoryAttributeAppearance")]
		public ElementPosition Position
		{
			get
			{
				if (this.chart != null && this.chart.serializationStatus == SerializationStatus.Saving)
				{
					if (this.position.Auto)
					{
						return new ElementPosition();
					}
					ElementPosition elementPosition = new ElementPosition();
					elementPosition.Auto = true;
					elementPosition.SetPositionNoAuto(this.position.X, this.position.Y, this.position.Width, this.position.Height);
					return elementPosition;
				}
				return this.position;
			}
			set
			{
				this.position = value;
				this.Invalidate(false);
			}
		}

		[SRDescription("DescriptionAttributeTitle_Text")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		[DefaultValue("")]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = ((value == null) ? string.Empty : value);
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(TextStyle.Default)]
		[SRDescription("DescriptionAttributeTitle_Style")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		public TextStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTitle_BackColor")]
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				this.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTitle_BorderColor")]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeTitle_BorderStyle")]
		[NotifyParentProperty(true)]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeTitle_BorderWidth")]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionTitleBorderWidthIsNegative);
				}
				this.borderWidth = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage17")]
		public string BackImage
		{
			get
			{
				return this.backImage;
			}
			set
			{
				this.backImage = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTitle_BackImageMode")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return this.backImageMode;
			}
			set
			{
				this.backImageMode = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTitle_BackImageTransparentColor")]
		public Color BackImageTransparentColor
		{
			get
			{
				return this.backImageTranspColor;
			}
			set
			{
				this.backImageTranspColor = value;
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeBackImageAlign")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[NotifyParentProperty(true)]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return this.backImageAlign;
			}
			set
			{
				this.backImageAlign = value;
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeTitle_BackGradientType")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		public GradientType BackGradientType
		{
			get
			{
				return this.backGradientType;
			}
			set
			{
				this.backGradientType = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTitle_BackGradientEndColor")]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		public Color BackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				this.backGradientEndColor = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTitle_BackHatchStyle")]
		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return this.backHatchStyle;
			}
			set
			{
				this.backHatchStyle = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTitle_Font")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.Invalidate(false);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTitle_Color")]
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeDocking")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[SRDescription("DescriptionAttributeTitle_Alignment")]
		[NotifyParentProperty(true)]
		public ContentAlignment Alignment
		{
			get
			{
				return this.alignment;
			}
			set
			{
				this.alignment = value;
				this.Invalidate(false);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeDocking")]
		[NotifyParentProperty(true)]
		[DefaultValue(Docking.Top)]
		[SRDescription("DescriptionAttributeTitle_Docking")]
		public Docking Docking
		{
			get
			{
				return this.docking;
			}
			set
			{
				this.docking = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeTitle_ShadowOffset")]
		public int ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				this.shadowOffset = value;
				this.Invalidate(false);
			}
		}

		[SRDescription("DescriptionAttributeTitle_ShadowColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128, 0, 0, 0")]
		[NotifyParentProperty(true)]
		public Color ShadowColor
		{
			get
			{
				return this.shadowColor;
			}
			set
			{
				this.shadowColor = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeTitle_ToolTip")]
		[DefaultValue("")]
		public string ToolTip
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

		[Browsable(false)]
		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeTitle_Href")]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeTitle_MapAreaAttributes")]
		[DefaultValue("")]
		public string MapAreaAttributes
		{
			get
			{
				return this.mapAreaAttributes;
			}
			set
			{
				this.mapAreaAttributes = value;
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

		internal bool BackGroundIsVisible
		{
			get
			{
				if (this.BackColor.IsEmpty && this.BackImage.Length <= 0 && (this.BorderColor.IsEmpty || this.BorderStyle == ChartDashStyle.NotSet))
				{
					return false;
				}
				return true;
			}
		}

		internal Chart Chart
		{
			get
			{
				return this.chart;
			}
			set
			{
				this.chart = value;
				this.position.chart = value;
			}
		}

		private bool IsTextVertical
		{
			get
			{
				TextOrientation textOrientation = this.GetTextOrientation();
				if (textOrientation != TextOrientation.Rotated90)
				{
					return textOrientation == TextOrientation.Rotated270;
				}
				return true;
			}
		}

		public Title()
		{
			this.Initialize(string.Empty, Docking.Top, null, Color.Black);
		}

		public Title(string text)
		{
			this.Initialize(text, Docking.Top, null, Color.Black);
		}

		public Title(string text, Docking docking)
		{
			this.Initialize(text, docking, null, Color.Black);
		}

		public Title(string text, Docking docking, Font font, Color color)
		{
			this.Initialize(text, docking, font, color);
		}

		private void Initialize(string text, Docking docking, Font font, Color color)
		{
			this.text = text;
			this.docking = docking;
			this.color = color;
			if (font != null)
			{
				this.font = font;
			}
		}

		private TextOrientation GetTextOrientation()
		{
			if (this.TextOrientation == TextOrientation.Auto)
			{
				if (this.Position.Auto)
				{
					if (this.Docking == Docking.Left)
					{
						return TextOrientation.Rotated270;
					}
					if (this.Docking == Docking.Right)
					{
						return TextOrientation.Rotated90;
					}
				}
				return TextOrientation.Horizontal;
			}
			return this.TextOrientation;
		}

		internal bool IsVisible()
		{
			if (this.Visible)
			{
				if (this.DockToChartArea.Length > 0 && this.Chart != null && this.Chart.ChartAreas.GetIndex(this.DockToChartArea) >= 0)
				{
					ChartArea chartArea = this.Chart.ChartAreas[this.DockToChartArea];
					if (!chartArea.Visible)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal void Invalidate(bool invalidateTitleOnly)
		{
		}

		internal void Paint(ChartGraphics chartGraph)
		{
			if (this.IsVisible())
			{
				CommonElements common = this.chart.chartPicture.common;
				string text = this.Text;
				if (this.chart != null && this.chart.LocalizeTextHandler != null)
				{
					text = this.chart.LocalizeTextHandler(this, text, 0, ChartElementType.Title);
				}
				RectangleF rectangleF = this.Position.ToRectangleF();
				if (!this.Position.Auto && this.chart != null && this.chart.chartPicture != null && (rectangleF.Width == 0.0 || rectangleF.Height == 0.0))
				{
					SizeF sizeF = new SizeF((rectangleF.Width == 0.0) ? ((float)this.chart.chartPicture.Width) : rectangleF.Width, (rectangleF.Height == 0.0) ? ((float)this.chart.chartPicture.Height) : rectangleF.Height);
					if (this.IsTextVertical)
					{
						float width = sizeF.Width;
						sizeF.Width = sizeF.Height;
						sizeF.Height = width;
					}
					sizeF = chartGraph.GetAbsoluteSize(sizeF);
					SizeF size = chartGraph.MeasureString(text.Replace("\\n", "\n"), this.Font, sizeF, new StringFormat(), this.GetTextOrientation());
					if (this.BackGroundIsVisible)
					{
						size.Width += (float)this.titleBorderSpacing;
						size.Height += (float)this.titleBorderSpacing;
					}
					if (this.IsTextVertical)
					{
						float width2 = size.Width;
						size.Width = size.Height;
						size.Height = width2;
					}
					size = chartGraph.GetRelativeSize(size);
					if (rectangleF.Width == 0.0)
					{
						rectangleF.Width = size.Width;
						if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.MiddleRight || this.Alignment == ContentAlignment.TopRight)
						{
							rectangleF.X -= rectangleF.Width;
						}
						else if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.TopCenter)
						{
							rectangleF.X -= (float)(rectangleF.Width / 2.0);
						}
					}
					if (rectangleF.Height == 0.0)
					{
						rectangleF.Height = size.Height;
						if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.BottomLeft)
						{
							rectangleF.Y -= rectangleF.Height;
						}
						else if (this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.MiddleRight)
						{
							rectangleF.Y -= (float)(rectangleF.Height / 2.0);
						}
					}
				}
				RectangleF rectangleF2 = new RectangleF(rectangleF.Location, rectangleF.Size);
				rectangleF2 = chartGraph.GetAbsoluteRectangle(rectangleF2);
				if (this.BackGroundIsVisible && common.ProcessModePaint)
				{
					chartGraph.StartHotRegion(this.href, this.toolTip);
					chartGraph.StartAnimation();
					chartGraph.FillRectangleRel(rectangleF, this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.BorderWidth, this.BorderStyle, this.ShadowColor, this.ShadowOffset, PenAlignment.Inset);
					chartGraph.StopAnimation();
					chartGraph.EndHotRegion();
				}
				else
				{
					chartGraph.StartHotRegion(this.href, this.toolTip);
					SizeF absoluteSize = chartGraph.GetAbsoluteSize(rectangleF.Size);
					SizeF size2 = chartGraph.MeasureString(text.Replace("\\n", "\n"), this.Font, absoluteSize, new StringFormat(), this.GetTextOrientation());
					size2 = chartGraph.GetRelativeSize(size2);
					RectangleF rectF = new RectangleF(rectangleF.X, rectangleF.Y, size2.Width, size2.Height);
					if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.BottomRight)
					{
						rectF.Y = rectangleF.Bottom - rectF.Height;
					}
					else if (this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.MiddleRight)
					{
						rectF.Y = (float)(rectangleF.Y + rectangleF.Height / 2.0 - rectF.Height / 2.0);
					}
					if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.MiddleRight || this.Alignment == ContentAlignment.TopRight)
					{
						rectF.X = rectangleF.Right - rectF.Width;
					}
					else if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.MiddleCenter || this.Alignment == ContentAlignment.TopCenter)
					{
						rectF.X = (float)(rectangleF.X + rectangleF.Width / 2.0 - rectF.Width / 2.0);
					}
					if (true)
					{
						chartGraph.FillRectangleRel(rectF, Color.FromArgb(0, Color.White), ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Tile, this.BackImageTransparentColor, this.BackImageAlign, GradientType.None, this.BackGradientEndColor, Color.Transparent, 0, this.BorderStyle, Color.Transparent, 0, PenAlignment.Inset);
					}
					chartGraph.EndHotRegion();
				}
				if (this.BackGroundIsVisible)
				{
					rectangleF2.Width -= (float)this.titleBorderSpacing;
					rectangleF2.Height -= (float)this.titleBorderSpacing;
					rectangleF2.X += (float)((float)this.titleBorderSpacing / 2.0);
					rectangleF2.Y += (float)((float)this.titleBorderSpacing / 2.0);
				}
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				if (this.Alignment == ContentAlignment.BottomCenter || this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.BottomRight)
				{
					stringFormat.LineAlignment = StringAlignment.Far;
				}
				else if (this.Alignment == ContentAlignment.TopCenter || this.Alignment == ContentAlignment.TopLeft || this.Alignment == ContentAlignment.TopRight)
				{
					stringFormat.LineAlignment = StringAlignment.Near;
				}
				if (this.Alignment == ContentAlignment.BottomLeft || this.Alignment == ContentAlignment.MiddleLeft || this.Alignment == ContentAlignment.TopLeft)
				{
					stringFormat.Alignment = StringAlignment.Near;
				}
				else if (this.Alignment == ContentAlignment.BottomRight || this.Alignment == ContentAlignment.MiddleRight || this.Alignment == ContentAlignment.TopRight)
				{
					stringFormat.Alignment = StringAlignment.Far;
				}
				Color gradientColor = ChartGraphics.GetGradientColor(this.Color, Color.Black, 0.8);
				int num = 1;
				TextStyle textStyle = this.Style;
				if ((textStyle == TextStyle.Default || textStyle == TextStyle.Shadow) && !this.BackGroundIsVisible && this.ShadowOffset != 0)
				{
					textStyle = TextStyle.Shadow;
					gradientColor = this.ShadowColor;
					num = this.ShadowOffset;
				}
				text = text.Replace("\\n", "\n");
				Matrix matrix = null;
				if (this.IsTextVertical)
				{
					if (this.GetTextOrientation() == TextOrientation.Rotated270)
					{
						stringFormat.FormatFlags |= (StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical);
						matrix = chartGraph.Transform.Clone();
						PointF empty = PointF.Empty;
						empty.X = (float)(rectangleF2.X + rectangleF2.Width / 2.0);
						empty.Y = (float)(rectangleF2.Y + rectangleF2.Height / 2.0);
						Matrix matrix2 = chartGraph.Transform.Clone();
						matrix2.RotateAt(180f, empty);
						chartGraph.Transform = matrix2;
					}
					else if (this.GetTextOrientation() == TextOrientation.Rotated90)
					{
						stringFormat.FormatFlags |= (StringFormatFlags.DirectionRightToLeft | StringFormatFlags.DirectionVertical);
					}
				}
				if (text.Length > 0)
				{
					chartGraph.StartAnimation();
					switch (textStyle)
					{
					case TextStyle.Default:
						chartGraph.StartHotRegion(this.href, this.toolTip);
						chartGraph.DrawString(text, this.Font, new SolidBrush(this.Color), rectangleF2, stringFormat, this.GetTextOrientation());
						chartGraph.EndHotRegion();
						break;
					case TextStyle.Frame:
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.AddString(ChartGraphics.GetStackedText(text), this.Font.FontFamily, (int)this.Font.Style, (float)(this.Font.Size * 1.2999999523162842), rectangleF2, stringFormat);
						graphicsPath.CloseAllFigures();
						chartGraph.StartHotRegion(this.href, this.toolTip);
						chartGraph.DrawPath(new Pen(this.Color, 1f), graphicsPath);
						chartGraph.EndHotRegion();
						break;
					}
					case TextStyle.Embed:
					{
						RectangleF rect3 = new RectangleF(rectangleF2.Location, rectangleF2.Size);
						rect3.X -= 1f;
						rect3.Y -= 1f;
						chartGraph.DrawString(text, this.Font, new SolidBrush(gradientColor), rect3, stringFormat, this.GetTextOrientation());
						rect3.X += 2f;
						rect3.Y += 2f;
						Color gradientColor3 = ChartGraphics.GetGradientColor(Color.White, this.Color, 0.3);
						chartGraph.DrawString(text, this.Font, new SolidBrush(gradientColor3), rect3, stringFormat, this.GetTextOrientation());
						chartGraph.StartHotRegion(this.href, this.toolTip);
						chartGraph.DrawString(text, this.Font, new SolidBrush(this.Color), rectangleF2, stringFormat, this.GetTextOrientation());
						chartGraph.EndHotRegion();
						break;
					}
					case TextStyle.Emboss:
					{
						RectangleF rect2 = new RectangleF(rectangleF2.Location, rectangleF2.Size);
						rect2.X += 1f;
						rect2.Y += 1f;
						chartGraph.DrawString(text, this.Font, new SolidBrush(gradientColor), rect2, stringFormat, this.GetTextOrientation());
						rect2.X -= 2f;
						rect2.Y -= 2f;
						Color gradientColor2 = ChartGraphics.GetGradientColor(Color.White, this.Color, 0.3);
						chartGraph.DrawString(text, this.Font, new SolidBrush(gradientColor2), rect2, stringFormat, this.GetTextOrientation());
						chartGraph.StartHotRegion(this.href, this.toolTip);
						chartGraph.DrawString(text, this.Font, new SolidBrush(this.Color), rectangleF2, stringFormat, this.GetTextOrientation());
						chartGraph.EndHotRegion();
						break;
					}
					case TextStyle.Shadow:
					{
						RectangleF rect = new RectangleF(rectangleF2.Location, rectangleF2.Size);
						rect.X += (float)num;
						rect.Y += (float)num;
						chartGraph.DrawString(text, this.Font, new SolidBrush(gradientColor), rect, stringFormat, this.GetTextOrientation());
						chartGraph.StartHotRegion(this.href, this.toolTip);
						chartGraph.DrawString(text, this.Font, new SolidBrush(this.Color), rectangleF2, stringFormat, this.GetTextOrientation());
						chartGraph.EndHotRegion();
						break;
					}
					default:
						throw new InvalidOperationException(SR.ExceptionTitleTextDrawingStyleUnknown);
					}
					chartGraph.StopAnimation();
				}
				if (matrix != null)
				{
					chartGraph.Transform = matrix;
				}
				if (common.ProcessModeRegions)
				{
					common.HotRegionsList.AddHotRegion(chartGraph, rectangleF, this.ToolTip, this.Href, this.MapAreaAttributes, this, ChartElementType.Title, string.Empty);
				}
			}
		}

		internal void CalcTitlePosition(ChartGraphics chartGraph, ref RectangleF chartAreasRectangle, ref RectangleF frameTitlePosition, float elementSpacing)
		{
			if (!frameTitlePosition.IsEmpty && this.Position.Auto && this.Docking == Docking.Top && this.DockToChartArea == "NotSet")
			{
				this.Position.SetPositionNoAuto(frameTitlePosition.X + elementSpacing, frameTitlePosition.Y, (float)(frameTitlePosition.Width - 2.0 * elementSpacing), frameTitlePosition.Height);
				frameTitlePosition = RectangleF.Empty;
			}
			else
			{
				RectangleF rectangleF = default(RectangleF);
				StringFormat stringFormat = new StringFormat();
				SizeF sizeF = new SizeF(chartAreasRectangle.Width, chartAreasRectangle.Height);
				if (this.IsTextVertical)
				{
					float width = sizeF.Width;
					sizeF.Width = sizeF.Height;
					sizeF.Height = width;
				}
				sizeF.Width -= (float)(2.0 * elementSpacing);
				sizeF.Height -= (float)(2.0 * elementSpacing);
				sizeF = chartGraph.GetAbsoluteSize(sizeF);
				SizeF size = chartGraph.MeasureString(this.Text.Replace("\\n", "\n"), this.Font, sizeF, stringFormat, this.GetTextOrientation());
				if (this.BackGroundIsVisible)
				{
					size.Width += (float)this.titleBorderSpacing;
					size.Height += (float)this.titleBorderSpacing;
				}
				if (this.IsTextVertical)
				{
					float width2 = size.Width;
					size.Width = size.Height;
					size.Height = width2;
				}
				size = chartGraph.GetRelativeSize(size);
				rectangleF.Height = size.Height;
				rectangleF.Width = size.Width;
				if (!float.IsNaN(size.Height) && !float.IsNaN(size.Width))
				{
					if (this.Docking == Docking.Top)
					{
						rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
						rectangleF.X = chartAreasRectangle.X + elementSpacing;
						rectangleF.Width = chartAreasRectangle.Right - rectangleF.X - elementSpacing;
						if (rectangleF.Width < 0.0)
						{
							rectangleF.Width = 0f;
						}
						chartAreasRectangle.Height -= rectangleF.Height + elementSpacing;
						chartAreasRectangle.Y = rectangleF.Bottom;
					}
					else if (this.Docking == Docking.Bottom)
					{
						rectangleF.Y = chartAreasRectangle.Bottom - size.Height - elementSpacing;
						rectangleF.X = chartAreasRectangle.X + elementSpacing;
						rectangleF.Width = chartAreasRectangle.Right - rectangleF.X - elementSpacing;
						if (rectangleF.Width < 0.0)
						{
							rectangleF.Width = 0f;
						}
						chartAreasRectangle.Height -= rectangleF.Height + elementSpacing;
					}
					if (this.Docking == Docking.Left)
					{
						rectangleF.X = chartAreasRectangle.X + elementSpacing;
						rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
						rectangleF.Height = chartAreasRectangle.Bottom - rectangleF.Y - elementSpacing;
						if (rectangleF.Height < 0.0)
						{
							rectangleF.Height = 0f;
						}
						chartAreasRectangle.Width -= rectangleF.Width + elementSpacing;
						chartAreasRectangle.X = rectangleF.Right;
					}
					if (this.Docking == Docking.Right)
					{
						rectangleF.X = chartAreasRectangle.Right - size.Width - elementSpacing;
						rectangleF.Y = chartAreasRectangle.Y + elementSpacing;
						rectangleF.Height = chartAreasRectangle.Bottom - rectangleF.Y - elementSpacing;
						if (rectangleF.Height < 0.0)
						{
							rectangleF.Height = 0f;
						}
						chartAreasRectangle.Width -= rectangleF.Width + elementSpacing;
					}
					if (this.DockOffset != 0)
					{
						if (this.Docking == Docking.Top || this.Docking == Docking.Bottom)
						{
							rectangleF.Y += (float)this.DockOffset;
						}
						else
						{
							rectangleF.X += (float)this.DockOffset;
						}
					}
					this.Position.SetPositionNoAuto(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
				}
			}
		}
	}
}
