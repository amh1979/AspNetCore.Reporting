using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(MapImageConverter))]
	internal class MapImage : DockablePanel, IToolTipProvider, IImageMapProvider
	{
		private ResizeMode resizeMode = ResizeMode.AutoFit;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private float shadowOffset = 1f;

		private float angle;

		private float transparency;

		private object mapAreaTag;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapImage_ToolTip")]
		[Localizable(true)]
		public override string ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[Localizable(true)]
		[SRDescription("DescriptionAttributeMapImage_Href")]
		[DefaultValue("")]
		public sealed override string Href
		{
			get
			{
				return base.Href;
			}
			set
			{
				base.Href = value;
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeMapImage_MapAreaAttributes")]
		public override string MapAreaAttributes
		{
			get
			{
				return base.MapAreaAttributes;
			}
			set
			{
				base.MapAreaAttributes = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always)]
		[SRDescription("DescriptionAttributeMapImage_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[SRCategory("CategoryAttribute_Data")]
		[Browsable(true)]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMapImage_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
				this.Invalidate();
			}
		}

		[DefaultValue(ResizeMode.AutoFit)]
		[SRDescription("DescriptionAttributeMapImage_ResizeMode")]
		[SRCategory("CategoryAttribute_Image")]
		public ResizeMode ResizeMode
		{
			get
			{
				return this.resizeMode;
			}
			set
			{
				this.resizeMode = value;
				this.Invalidate();
			}
		}

		[DefaultValue("")]
		[SRDescription("DescriptionAttributeMapImage_Image")]
		[SRCategory("CategoryAttribute_Image")]
		public string Image
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
		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeMapImage_ImageTransColor")]
		public Color ImageTransColor
		{
			get
			{
				return this.imageTransColor;
			}
			set
			{
				this.imageTransColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMapImage_ShadowOffset")]
		[DefaultValue(1f)]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		public float ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.shadowOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[SRCategory("CategoryAttribute_Position")]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeMapImage_Angle")]
		public float Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				if (!(value > 360.0) && !(value < 0.0))
				{
					this.angle = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 360.0));
			}
		}

		[SRDescription("DescriptionAttributeMapImage_Transparency")]
		[DefaultValue(0f)]
		[SRCategory("CategoryAttribute_Image")]
		public float Transparency
		{
			get
			{
				return this.transparency;
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					this.transparency = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 100.0));
			}
		}

		[DefaultValue(MapDashStyle.Solid)]
		[SRDescription("DescriptionAttributeMapImage_BorderStyle")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		public override MapDashStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackSecondaryColor
		{
			get
			{
				return base.BackSecondaryColor;
			}
			set
			{
				base.BackSecondaryColor = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override MapHatchStyle BackHatchStyle
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

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int BackShadowOffset
		{
			get
			{
				return base.BackShadowOffset;
			}
			set
			{
				base.BackShadowOffset = value;
			}
		}

		internal Position Position
		{
			get
			{
				return new Position(this.Location, this.Size, ContentAlignment.TopLeft);
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

		public MapImage()
			: this(null)
		{
		}

		internal MapImage(CommonElements common)
			: base(common)
		{
			this.Location = new MapLocation(this, 20f, 20f);
			this.Location.DefaultValues = true;
			this.Size.DefaultValues = true;
			this.BorderStyle = MapDashStyle.Solid;
			this.Visible = true;
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal override bool ShouldRenderBackground()
		{
			return false;
		}

		internal override void Render(MapGraphics g)
		{
			g.StartHotRegion(this);
			MapDashStyle mapDashStyle = this.BorderStyle;
			if (!string.IsNullOrEmpty(this.Image))
			{
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				this.DrawImage(g, this.image, true);
				this.DrawImage(g, this.image, false);
				imageSmoothingState.Restore();
			}
			else
			{
				string text = "No image.";
				Font font = new Font("Microsoft Sans Serif", 8.25f);
				SizeF sizeF = g.MeasureString(text, font);
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				PointF absolutePoint = g.GetAbsolutePoint(new PointF(50f, 50f));
				new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f).Inflate((float)(sizeF.Width / 2.0), (float)(sizeF.Height / 2.0));
				using (Brush brush = new SolidBrush(Color.Gray))
				{
					g.DrawString(text, font, brush, absoluteRectangle, stringFormat);
				}
				mapDashStyle = MapDashStyle.Solid;
			}
			if (mapDashStyle != 0 && this.BorderColor != Color.Transparent && this.BorderWidth != 0)
			{
				using (GraphicsPath path = this.GetPath(g))
				{
					using (Pen pen = this.GetPen())
					{
						AntiAliasing antiAliasing = g.AntiAliasing;
						if (this.Angle % 90.0 == 0.0)
						{
							g.AntiAliasing = AntiAliasing.None;
						}
						g.DrawPath(pen, path);
						g.AntiAliasing = antiAliasing;
					}
				}
			}
			g.EndHotRegion();
		}

		internal void DrawImage(MapGraphics g, string imageName, bool drawShadow)
		{
			if (drawShadow && this.ShadowOffset == 0.0)
			{
				return;
			}
			Image image = this.Common.ImageLoader.LoadImage(imageName);
			if (image.Width != 0 && image.Height != 0)
			{
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				Rectangle rectangle = Rectangle.Empty;
				if (this.ResizeMode == ResizeMode.AutoFit)
				{
					rectangle = new Rectangle((int)absoluteRectangle.X, (int)absoluteRectangle.Y, (int)absoluteRectangle.Width, (int)absoluteRectangle.Height);
				}
				else
				{
					rectangle = new Rectangle(0, 0, image.Width, image.Height);
					PointF absolutePoint = g.GetAbsolutePoint(new PointF(50f, 50f));
					rectangle.X = (int)(absolutePoint.X - (float)(rectangle.Size.Width / 2));
					rectangle.Y = (int)(absolutePoint.Y - (float)(rectangle.Size.Height / 2));
				}
				ImageAttributes imageAttributes = new ImageAttributes();
				if (this.ImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(this.ImageTransColor, this.ImageTransColor, ColorAdjustType.Default);
				}
				float num = (float)((100.0 - this.Transparency) / 100.0);
				float num2 = (float)(this.Common.MapCore.ShadowIntensity / 100.0);
				if (drawShadow)
				{
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0f;
					colorMatrix.Matrix11 = 0f;
					colorMatrix.Matrix22 = 0f;
					colorMatrix.Matrix33 = num2 * num;
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				else if (this.Transparency > 0.0)
				{
					ColorMatrix colorMatrix2 = new ColorMatrix();
					colorMatrix2.Matrix33 = num;
					imageAttributes.SetColorMatrix(colorMatrix2);
				}
				if (this.Angle != 0.0)
				{
					PointF point = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
					Matrix transform = g.Transform;
					Matrix matrix = g.Transform.Clone();
					float offsetX = matrix.OffsetX;
					float offsetY = matrix.OffsetY;
					point.X += offsetX;
					point.Y += offsetY;
					matrix.RotateAt(this.Angle, point, MatrixOrder.Append);
					if (drawShadow)
					{
						matrix.Translate(this.ShadowOffset, this.ShadowOffset, MatrixOrder.Append);
					}
					g.Transform = matrix;
					g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					g.Transform = transform;
				}
				else
				{
					if (drawShadow)
					{
						rectangle.X += (int)this.ShadowOffset;
						rectangle.Y += (int)this.ShadowOffset;
					}
					g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				}
				if (!drawShadow)
				{
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddRectangle(rectangle);
						if (this.Angle != 0.0)
						{
							PointF point2 = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
							using (Matrix matrix2 = new Matrix())
							{
								matrix2.RotateAt(this.Angle, point2, MatrixOrder.Append);
								graphicsPath.Transform(matrix2);
							}
						}
						this.Common.MapCore.HotRegionList.SetHotRegion(g, this, graphicsPath);
					}
				}
			}
		}

		internal GraphicsPath GetPath(MapGraphics g)
		{
			if (!this.IsVisible())
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			graphicsPath.AddRectangle(absoluteRectangle);
			if (this.Angle != 0.0)
			{
				PointF point = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
				using (Matrix matrix = new Matrix())
				{
					matrix.RotateAt(this.Angle, point);
					graphicsPath.Transform(matrix);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal Pen GetPen()
		{
			if (this.BorderWidth <= 0)
			{
				return null;
			}
			Color borderColor = this.BorderColor;
			int borderWidth = this.BorderWidth;
			MapDashStyle borderStyle = this.BorderStyle;
			Pen pen = new Pen(this.BorderColor, (float)this.BorderWidth);
			pen.DashStyle = MapGraphics.GetPenStyle(this.BorderStyle);
			pen.Alignment = PenAlignment.Center;
			return pen;
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Size":
				return new MapSize(null, 100f, 100f);
			case "Dock":
				return PanelDockStyle.None;
			case "DockAlignment":
				return DockAlignment.Center;
			case "BackColor":
				return Color.Empty;
			case "BackGradientType":
				return GradientType.None;
			case "BorderWidth":
				return 0;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		string IToolTipProvider.GetToolTip()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.ToolTip, this);
			}
			return this.ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip();
		}

		string IImageMapProvider.GetHref()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.Href, this);
			}
			return this.Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				return this.Common.MapCore.ResolveAllKeywords(this.MapAreaAttributes, this);
			}
			return this.MapAreaAttributes;
		}
	}
}
