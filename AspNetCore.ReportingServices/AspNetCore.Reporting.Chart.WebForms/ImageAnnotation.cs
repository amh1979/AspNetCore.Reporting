using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeImageAnnotation_ImageAnnotation")]
	internal class ImageAnnotation : Annotation
	{
		private string imageName = string.Empty;

		private ChartImageWrapMode imageMode = ChartImageWrapMode.Scaled;

		private Color imageTransparentColor = Color.Empty;

		[SRCategory("CategoryAttributeImage")]
		[SRDescription("DescriptionAttributeImageAnnotation_Image")]
		[Bindable(true)]
		[DefaultValue("")]
		public virtual string Image
		{
			get
			{
				return this.imageName;
			}
			set
			{
				this.imageName = value;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeImageAnnotation_ImageMode")]
		[SRCategory("CategoryAttributeImage")]
		[Bindable(true)]
		[DefaultValue(ChartImageWrapMode.Scaled)]
		public ChartImageWrapMode ImageMode
		{
			get
			{
				return this.imageMode;
			}
			set
			{
				this.imageMode = value;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeImageAnnotation_ImageTransparentColor")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeImage")]
		[DefaultValue(typeof(Color), "")]
		public Color ImageTransparentColor
		{
			get
			{
				return this.imageTransparentColor;
			}
			set
			{
				this.imageTransparentColor = value;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeImageAnnotation_Alignment")]
		[SRCategory("CategoryAttributeImage")]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		public override ContentAlignment Alignment
		{
			get
			{
				return base.Alignment;
			}
			set
			{
				base.Alignment = value;
				base.Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType
		{
			get
			{
				return "Image";
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal override SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.Rectangle;
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		public override Color TextColor
		{
			get
			{
				return base.TextColor;
			}
			set
			{
				base.TextColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		[Browsable(false)]
		[DefaultValue(typeof(TextStyle), "Default")]
		public override TextStyle TextStyle
		{
			get
			{
				return base.TextStyle;
			}
			set
			{
				base.TextStyle = value;
			}
		}

		[Browsable(false)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[NotifyParentProperty(true)]
		public override ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		[Browsable(false)]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		[Browsable(false)]
		public override Color LineColor
		{
			get
			{
				return base.LineColor;
			}
			set
			{
				base.LineColor = value;
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeImageAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(1)]
		public override int LineWidth
		{
			get
			{
				return base.LineWidth;
			}
			set
			{
				base.LineWidth = value;
			}
		}

		[SRDescription("DescriptionAttributeLineStyle6")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(ChartDashStyle.Solid)]
		public override ChartDashStyle LineStyle
		{
			get
			{
				return base.LineStyle;
			}
			set
			{
				base.LineStyle = value;
			}
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			this.Chart = chart;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			SizeF empty3 = SizeF.Empty;
			((Annotation)this).GetRelativePosition(out empty, out empty3, out empty2);
			PointF pointF = new PointF(empty.X + empty3.Width, empty.Y + empty3.Height);
			RectangleF rect = new RectangleF(empty, new SizeF(pointF.X - empty.X, pointF.Y - empty.Y));
			RectangleF rectangleF = new RectangleF(rect.Location, rect.Size);
			if (rectangleF.Width < 0.0)
			{
				rectangleF.X = rectangleF.Right;
				rectangleF.Width = (float)(0.0 - rectangleF.Width);
			}
			if (rectangleF.Height < 0.0)
			{
				rectangleF.Y = rectangleF.Bottom;
				rectangleF.Height = (float)(0.0 - rectangleF.Height);
			}
			if (!float.IsNaN(rectangleF.X) && !float.IsNaN(rectangleF.Y) && !float.IsNaN(rectangleF.Right) && !float.IsNaN(rectangleF.Bottom))
			{
				if (this.Chart.chartPicture.common.ProcessModePaint)
				{
					if (this.imageName.Length == 0 && this.Chart.IsDesignMode())
					{
						graphics.FillRectangleRel(rectangleF, this.BackColor, this.BackHatchStyle, this.imageName, this.imageMode, this.imageTransparentColor, this.GetImageAlignment(this.Alignment), this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, this.ShadowColor, this.ShadowOffset, PenAlignment.Center);
						using (Brush brush = new SolidBrush(this.TextColor))
						{
							StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
							stringFormat.Alignment = StringAlignment.Center;
							stringFormat.LineAlignment = StringAlignment.Center;
							stringFormat.FormatFlags = StringFormatFlags.LineLimit;
							stringFormat.Trimming = StringTrimming.EllipsisCharacter;
							graphics.DrawStringRel("(no image)", this.TextFont, brush, rectangleF, stringFormat);
						}
					}
					else
					{
						graphics.FillRectangleRel(rectangleF, Color.Transparent, this.BackHatchStyle, this.imageName, this.imageMode, this.imageTransparentColor, this.GetImageAlignment(this.Alignment), this.BackGradientType, Color.Transparent, Color.Transparent, 0, this.LineStyle, this.ShadowColor, this.ShadowOffset, PenAlignment.Center);
					}
				}
				if (this.Chart.chartPicture.common.ProcessModeRegions)
				{
					this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, rectangleF, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation, string.Empty);
				}
				this.PaintSelectionHandles(graphics, rect, null);
			}
		}

		private ChartImageAlign GetImageAlignment(ContentAlignment alignment)
		{
			switch (alignment)
			{
			case ContentAlignment.TopLeft:
				return ChartImageAlign.TopLeft;
			case ContentAlignment.TopCenter:
				return ChartImageAlign.Top;
			case ContentAlignment.TopRight:
				return ChartImageAlign.TopRight;
			case ContentAlignment.MiddleRight:
				return ChartImageAlign.Right;
			case ContentAlignment.BottomRight:
				return ChartImageAlign.BottomRight;
			case ContentAlignment.BottomCenter:
				return ChartImageAlign.Bottom;
			case ContentAlignment.BottomLeft:
				return ChartImageAlign.BottomLeft;
			case ContentAlignment.MiddleLeft:
				return ChartImageAlign.Left;
			default:
				return ChartImageAlign.Center;
			}
		}

		internal override RectangleF GetContentPosition()
		{
			if (this.Image.Length > 0)
			{
				try
				{
					if (this.Chart != null)
					{
						ImageLoader imageLoader = (ImageLoader)this.Chart.serviceContainer.GetService(typeof(ImageLoader));
						if (imageLoader != null)
						{
							ChartGraphics graphics = base.GetGraphics();
							if (graphics != null)
							{
								SizeF size = default(SizeF);
								if (imageLoader.GetAdjustedImageSize(this.Image, graphics.Graphics, ref size))
								{
									SizeF relativeSize = graphics.GetRelativeSize(size);
									return new RectangleF(float.NaN, float.NaN, relativeSize.Width, relativeSize.Height);
								}
							}
						}
					}
				}
				catch
				{
				}
			}
			return new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);
		}
	}
}
