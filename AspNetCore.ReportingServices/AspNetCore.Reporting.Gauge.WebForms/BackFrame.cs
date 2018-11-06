using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(BackFrameConverter))]
	internal class BackFrame : GaugeObject
	{
		private XamlRenderer xamlRenderer;

		private BackFrameStyle style;

		private BackFrameShape shape;

		private float frameWidth = 8f;

		private Color frameColor = Color.Gainsboro;

		private GradientType frameGradientType = GradientType.DiagonalLeft;

		private Color frameGradientEndColor = Color.Gray;

		private GaugeHatchStyle frameHatchStyle;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Color imageHueColor = Color.Empty;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color backColor = Color.Silver;

		private GradientType backGradientType = GradientType.DiagonalLeft;

		private Color backGradientEndColor = Color.Gray;

		private GaugeHatchStyle backHatchStyle;

		private float shadowOffset;

		private GlassEffect glassEffect;

		private bool clipImage;

		[SRCategory("CategoryAppearance")]
		[Obsolete("This property is obsolete in Dundas Gauge 2.0. FrameStyle is supposed to be used instead.")]
		[SRDescription("DescriptionAttributeBackFrame_FrameStyle")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public BackFrameStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
				this.Invalidate();
			}
		}

		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameStyle")]
		[NotifyParentProperty(true)]
		public BackFrameStyle FrameStyle
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[Obsolete("This property is obsolete in Dundas Gauge 2.0. FrameShape is supposed to be used instead.")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameShape")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[RefreshProperties(RefreshProperties.All)]
		[ParenthesizePropertyName(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public BackFrameShape Shape
		{
			get
			{
				return this.shape;
			}
			set
			{
				this.shape = value;
				this.ResetCachedXamlRenderer();
				this.Invalidate();
			}
		}

		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameShape")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		public BackFrameShape FrameShape
		{
			get
			{
				return this.shape;
			}
			set
			{
				this.shape = value;
				this.ResetCachedXamlRenderer();
				this.Invalidate();
			}
		}

		[ValidateBound(0.0, 50.0)]
		[SRDescription("DescriptionAttributeBackFrame_FrameWidth")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(8f)]
		public float FrameWidth
		{
			get
			{
				return this.frameWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 50.0))
				{
					this.frameWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 50));
			}
		}

		[SRDescription("DescriptionAttributeBackFrame_FrameColor")]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Gainsboro")]
		public Color FrameColor
		{
			get
			{
				return this.frameColor;
			}
			set
			{
				this.frameColor = value;
				this.ResetCachedXamlRenderer();
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_FrameGradientType")]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.DiagonalLeft)]
		public GradientType FrameGradientType
		{
			get
			{
				return this.frameGradientType;
			}
			set
			{
				this.frameGradientType = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeBackFrame_FrameGradientEndColor")]
		[DefaultValue(typeof(Color), "Gray")]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		public Color FrameGradientEndColor
		{
			get
			{
				return this.frameGradientEndColor;
			}
			set
			{
				this.frameGradientEndColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeBackFrame_FrameHatchStyle")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(GaugeHatchStyle.None)]
		[NotifyParentProperty(true)]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public GaugeHatchStyle FrameHatchStyle
		{
			get
			{
				return this.frameHatchStyle;
			}
			set
			{
				this.frameHatchStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_Image")]
		[NotifyParentProperty(true)]
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

		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_ImageTransColor")]
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

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_ImageHueColor")]
		[NotifyParentProperty(true)]
		public Color ImageHueColor
		{
			get
			{
				return this.imageHueColor;
			}
			set
			{
				this.imageHueColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeBackFrame_BorderColor")]
		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeBackFrame_BorderStyle")]
		[DefaultValue(GaugeDashStyle.NotSet)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		public GaugeDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeBackFrame_BorderWidth")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(1)]
		[NotifyParentProperty(true)]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.borderWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "Silver")]
		[SRDescription("DescriptionAttributeBackFrame_BackColor")]
		[NotifyParentProperty(true)]
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				this.ResetCachedXamlRenderer();
				this.Invalidate();
			}
		}

		[DefaultValue(GradientType.DiagonalLeft)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_BackGradientType")]
		public GradientType BackGradientType
		{
			get
			{
				return this.backGradientType;
			}
			set
			{
				this.backGradientType = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackFrame_BackGradientEndColor")]
		[DefaultValue(typeof(Color), "Gray")]
		[SRCategory("CategoryAppearance")]
		public Color BackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				this.backGradientEndColor = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackFrame_BackHatchStyle")]
		[DefaultValue(GaugeHatchStyle.None)]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryAppearance")]
		public GaugeHatchStyle BackHatchStyle
		{
			get
			{
				return this.backHatchStyle;
			}
			set
			{
				this.backHatchStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(0f)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		[ValidateBound(-5.0, 5.0)]
		[SRDescription("DescriptionAttributeShadowOffset4")]
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[SRDescription("DescriptionAttributeBackFrame_GlassEffect")]
		[DefaultValue(GlassEffect.None)]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		public GlassEffect GlassEffect
		{
			get
			{
				return this.glassEffect;
			}
			set
			{
				this.glassEffect = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBackFrame_ClipImage")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool ClipImage
		{
			get
			{
				return this.clipImage;
			}
			set
			{
				this.clipImage = value;
				this.Invalidate();
			}
		}

		public BackFrame()
			: this(null)
		{
		}

		public BackFrame(object parent)
			: base(parent)
		{
		}

		public BackFrame(object parent, BackFrameStyle style, BackFrameShape shape)
			: this(parent)
		{
			this.style = style;
			this.shape = shape;
		}

		protected bool ShouldSerializeStyle()
		{
			return false;
		}

		protected bool ShouldSerializeShape()
		{
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetFrameStyle()
		{
			if (this.Parent is GaugeCore)
			{
				this.FrameStyle = BackFrameStyle.None;
			}
			else if (this.Parent is GaugeBase)
			{
				this.FrameStyle = BackFrameStyle.Edged;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetFrameShape()
		{
			if (this.Parent is LinearGauge || this.Parent is GaugeCore)
			{
				this.FrameShape = BackFrameShape.Rectangular;
			}
			else if (this.Parent is CircularGauge)
			{
				this.FrameShape = BackFrameShape.Circular;
			}
		}

		internal XamlRenderer GetCachedXamlRenderer(GaugeGraphics g)
		{
			if (this.xamlRenderer != null)
			{
				return this.xamlRenderer;
			}
			BackFrameStyle backFrameStyle = this.FrameStyle;
			if (backFrameStyle == BackFrameStyle.None)
			{
				backFrameStyle = BackFrameStyle.Edged;
			}
			this.xamlRenderer = new XamlRenderer(this.FrameShape.ToString() + "." + backFrameStyle.ToString() + ".xaml");
			this.xamlRenderer.AllowPathGradientTransform = false;
			RectangleF frameRectangle = this.GetFrameRectangle(g);
			Color[] layerHues = new Color[2]
			{
				this.FrameColor,
				this.BackColor
			};
			this.xamlRenderer.ParseXaml(frameRectangle, layerHues, null);
			return this.xamlRenderer;
		}

		internal void ResetCachedXamlRenderer()
		{
			if (this.xamlRenderer != null)
			{
				this.xamlRenderer.Dispose();
				this.xamlRenderer = null;
			}
		}

		internal GraphicsPath GetFramePath(GaugeGraphics g, float shrinkBy)
		{
			RectangleF frameRectangle = this.GetFrameRectangle(g);
			float absoluteDimension = g.GetAbsoluteDimension(shrinkBy);
			frameRectangle.Inflate((float)(0.0 - absoluteDimension), (float)(0.0 - absoluteDimension));
			if (shrinkBy > 0.0)
			{
				frameRectangle.Inflate(1f, 1f);
			}
			if (this.FrameShape == BackFrameShape.Circular)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddEllipse(frameRectangle);
				return graphicsPath;
			}
			if (this.FrameShape == BackFrameShape.AutoShape)
			{
				GraphicsPath graphicsPath2 = new GraphicsPath();
				if (this.Parent is CircularGauge)
				{
					CircularGauge circularGauge = (CircularGauge)this.Parent;
					if (circularGauge.Scales.Count == 0)
					{
						graphicsPath2.AddEllipse(frameRectangle);
					}
					else
					{
						this.BuildCircularGaugeAutoFrame(g, graphicsPath2, circularGauge, shrinkBy);
					}
				}
				else
				{
					graphicsPath2.AddRectangle(frameRectangle);
				}
				return graphicsPath2;
			}
			if (this.FrameShape == BackFrameShape.RoundedRectangular)
			{
				float num = (!(frameRectangle.Width > frameRectangle.Height)) ? frameRectangle.Width : frameRectangle.Height;
				float num2 = (float)(num / 8.0);
				float[] array = new float[10];
				for (int i = 0; i < 10; i++)
				{
					array[i] = num2;
				}
				return g.CreateRoundedRectPath(frameRectangle, array);
			}
			if (this.FrameShape == BackFrameShape.Rectangular)
			{
				GraphicsPath graphicsPath3 = new GraphicsPath();
				graphicsPath3.AddRectangle(frameRectangle);
				return graphicsPath3;
			}
			GraphicsPath graphicsPath4 = new GraphicsPath();
			graphicsPath4.FillMode = FillMode.Winding;
			XamlRenderer cachedXamlRenderer = this.GetCachedXamlRenderer(g);
			graphicsPath4.AddPath(cachedXamlRenderer.Layers[0].Paths[0], false);
			return graphicsPath4;
		}

		internal GraphicsPath GetBackPath(GaugeGraphics g)
		{
			if (this.FrameShape == BackFrameShape.AutoShape)
			{
				return this.GetFramePath(g, 0f);
			}
			if (this.IsCustomXamlFrame())
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.FillMode = FillMode.Winding;
				XamlRenderer cachedXamlRenderer = this.GetCachedXamlRenderer(g);
				graphicsPath.AddPath(cachedXamlRenderer.Layers[2].Paths[0], false);
				return graphicsPath;
			}
			return this.GetFramePath(g, this.FrameWidth);
		}

		private void BuildCircularGaugeAutoFrame(GaugeGraphics g, GraphicsPath path, CircularGauge gauge, float shrinkBy)
		{
			float startAngle = gauge.Scales[0].StartAngle;
			float num = gauge.Scales[0].StartAngle + gauge.Scales[0].SweepAngle;
			float radius = gauge.Scales[0].Radius;
			float num2 = gauge.Scales[0].GetLargestRadius(g);
			if (gauge.Scales.Count > 1)
			{
				for (int i = 1; i < gauge.Scales.Count; i++)
				{
					if (startAngle > gauge.Scales[i].StartAngle)
					{
						startAngle = gauge.Scales[i].StartAngle;
					}
					if (num < gauge.Scales[i].StartAngle + gauge.Scales[i].SweepAngle)
					{
						num = gauge.Scales[i].StartAngle + gauge.Scales[i].SweepAngle;
					}
					if (radius < gauge.Scales[i].Radius)
					{
						radius = gauge.Scales[i].Radius;
					}
					float largestRadius = gauge.Scales[i].GetLargestRadius(g);
					if (num2 < largestRadius)
					{
						num2 = largestRadius;
					}
				}
			}
			float num3 = 0f;
			foreach (CircularPointer pointer in gauge.Pointers)
			{
				if (pointer.Visible && pointer.Type == CircularPointerType.Needle)
				{
					float num4 = (float)(pointer.CapWidth / 2.0 * pointer.GetScale().Radius / 100.0);
					if (pointer.CapVisible && num4 > num3)
					{
						num3 = num4;
					}
					float num5 = (float)(pointer.GetNeedleTailLength() * pointer.GetScale().Radius / 100.0);
					if (num5 > num3)
					{
						num3 = num5;
					}
				}
			}
			foreach (Knob knob in gauge.Knobs)
			{
				if (knob.Visible)
				{
					float num6 = (float)(knob.Width * knob.GetScale().Radius / 100.0);
					if (num6 > num3)
					{
						num3 = num6;
					}
				}
			}
			num3 = g.GetAbsoluteDimension(num3);
			float absoluteDimension = g.GetAbsoluteDimension((float)(radius / 5.0));
			float absoluteDimension2 = g.GetAbsoluteDimension((float)(this.FrameWidth * radius / 100.0));
			float absoluteDimension3 = g.GetAbsoluteDimension((float)(shrinkBy * radius / 100.0));
			absoluteDimension += absoluteDimension2;
			absoluteDimension -= absoluteDimension3;
			float num7 = num - startAngle;
			PointF absolutePoint = g.GetAbsolutePoint(gauge.PivotPoint);
			float absoluteDimension4 = g.GetAbsoluteDimension(num2);
			float num8 = (float)(startAngle * 3.1415927410125732 / 180.0);
			float num9 = (float)((360.0 - startAngle - num7) * 3.1415927410125732 / 180.0);
			PointF pointF = default(PointF);
			pointF.X = absolutePoint.X - absoluteDimension4 * (float)Math.Sin((double)num8);
			pointF.Y = absolutePoint.Y + absoluteDimension4 * (float)Math.Cos((double)num8);
			PointF pointF2 = default(PointF);
			pointF2.X = absolutePoint.X + absoluteDimension4 * (float)Math.Sin((double)num9);
			pointF2.Y = absolutePoint.Y + absoluteDimension4 * (float)Math.Cos((double)num9);
			RectangleF rect = new RectangleF(pointF.X, pointF.Y, 0f, 0f);
			rect.Inflate(absoluteDimension, absoluteDimension);
			RectangleF rect2 = new RectangleF(pointF2.X, pointF2.Y, 0f, 0f);
			rect2.Inflate(absoluteDimension, absoluteDimension);
			RectangleF rect3 = new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f);
			rect3.Inflate(absoluteDimension + num3, absoluteDimension + num3);
			RectangleF rect4 = new RectangleF(absolutePoint.X, absolutePoint.Y, 0f, 0f);
			rect4.Inflate(absoluteDimension4 + absoluteDimension, absoluteDimension4 + absoluteDimension);
			if (num7 < 270.0)
			{
				path.AddArc(rect, (float)(startAngle + 270.0 + 90.0), 90f);
				path.AddArc(rect4, (float)(startAngle + 90.0), num7);
				path.AddArc(rect2, (float)(startAngle + num7 + 90.0), 90f);
				path.AddArc(rect3, (float)(startAngle + num7 + 90.0 + 45.0), (float)(360.0 - num7 - 90.0));
			}
			else if (num7 >= 320.0)
			{
				path.AddEllipse(rect4);
			}
			else
			{
				float num10 = (float)(90.0 - (360.0 - num7) / 2.0);
				path.AddArc(rect, (float)(startAngle + 270.0 + 90.0 + num10), (float)(90.0 - num10));
				path.AddArc(rect4, (float)(startAngle + 90.0), num7);
				path.AddArc(rect2, (float)(startAngle + num7 + 90.0), (float)(90.0 - num10));
			}
			path.CloseFigure();
		}

		internal static float GetXamlFrameAspectRatio(BackFrameShape shape)
		{
			float result = 1f;
			if (shape >= (BackFrameShape)2000 && shape <= (BackFrameShape)2199)
			{
				result = 1.48275864f;
			}
			else if (shape >= (BackFrameShape)2200 && shape <= (BackFrameShape)2399)
			{
				result = 0.6744186f;
			}
			return result;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			GraphicsPath framePath = this.GetFramePath(g, 0f);
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(this.ShadowOffset, this.ShadowOffset);
				framePath.Transform(matrix);
				return framePath;
			}
		}

		internal void DrawFrameImage(GaugeGraphics g)
		{
			GraphicsPath graphicsPath = null;
			Pen pen = null;
			Region region = null;
			try
			{
				graphicsPath = this.GetFramePath(g, 0f);
				RectangleF frameRectangle = this.GetFrameRectangle(g);
				Region clip = null;
				if (this.ClipImage)
				{
					this.RenderShadow(g);
					region = new Region(graphicsPath);
					clip = g.Clip;
					g.Clip = region;
				}
				else if (this.ShadowOffset != 0.0)
				{
					using (Brush brush = g.GetShadowBrush())
					{
						RectangleF rect = frameRectangle;
						rect.Offset(this.ShadowOffset, this.ShadowOffset);
						g.FillRectangle(brush, rect);
					}
				}
				ImageAttributes imageAttributes = new ImageAttributes();
				if (this.ImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(this.ImageTransColor, this.ImageTransColor, ColorAdjustType.Default);
				}
				Image image = this.Common.ImageLoader.LoadImage(this.Image);
				Rectangle destRect = new Rectangle((int)Math.Round((double)frameRectangle.X), (int)Math.Round((double)frameRectangle.Y), (int)Math.Round((double)frameRectangle.Width), (int)Math.Round((double)frameRectangle.Height));
				if (!this.ImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(this.ImageHueColor);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = (float)((float)(int)color.R / 255.0);
					colorMatrix.Matrix11 = (float)((float)(int)color.G / 255.0);
					colorMatrix.Matrix22 = (float)((float)(int)color.B / 255.0);
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
				if (this.ClipImage)
				{
					g.Clip = clip;
				}
				if (this.BorderWidth > 0 && this.BorderStyle != 0)
				{
					pen = new Pen(this.BorderColor, (float)this.BorderWidth);
					pen.DashStyle = g.GetPenStyle(this.BorderStyle);
					pen.Alignment = PenAlignment.Center;
					if (this.ClipImage)
					{
						g.DrawPath(pen, graphicsPath);
					}
					else
					{
						g.DrawRectangle(pen, frameRectangle.X, frameRectangle.Y, frameRectangle.Width, frameRectangle.Height);
					}
				}
			}
			finally
			{
				if (graphicsPath != null)
				{
					graphicsPath.Dispose();
				}
				if (pen != null)
				{
					pen.Dispose();
				}
				if (region != null)
				{
					region.Dispose();
				}
			}
		}

		internal bool IsCustomXamlFrame()
		{
			if (this.FrameShape != 0 && this.FrameShape != BackFrameShape.Rectangular && this.FrameShape != BackFrameShape.RoundedRectangular && this.FrameShape != BackFrameShape.AutoShape)
			{
				return true;
			}
			return false;
		}

		internal void RenderFrame(GaugeGraphics g)
		{
			if (this.Image.Length != 0)
			{
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRenderingImageFrame);
				this.DrawFrameImage(g);
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceImageFrameRenderingComplete);
			}
			else if (this.FrameStyle != 0)
			{
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRenderingFrame);
				this.ResetCachedXamlRenderer();
				if (this.IsCustomXamlFrame())
				{
					this.RenderShadow(g);
					XamlRenderer cachedXamlRenderer = this.GetCachedXamlRenderer(g);
					cachedXamlRenderer.Layers[0].Render(g);
					cachedXamlRenderer.Layers[1].Render(g);
				}
				else
				{
					this.RenderShadow(g);
					Brush brush = null;
					GraphicsPath graphicsPath = null;
					GraphicsPath graphicsPath2 = null;
					GraphicsPath graphicsPath3 = null;
					Brush brush2 = null;
					Brush brush3 = null;
					Pen pen = null;
					Pen pen2 = null;
					try
					{
						graphicsPath = this.GetFramePath(g, 0f);
						graphicsPath3 = this.GetFramePath(g, this.FrameWidth);
						brush = this.GetBrush(g, graphicsPath3.GetBounds(), this.backHatchStyle, this.BackGradientType, this.BackColor, this.BackGradientEndColor, false, 0f);
						g.FillPath(brush, graphicsPath3, 0f, false, this.FrameShape == BackFrameShape.Circular);
						if (this.FrameStyle == BackFrameStyle.Simple)
						{
							using (GraphicsPath graphicsPath4 = new GraphicsPath())
							{
								graphicsPath4.AddPath(graphicsPath, false);
								graphicsPath4.AddPath(graphicsPath3, false);
								graphicsPath4.CloseFigure();
								brush2 = this.GetBrush(g, graphicsPath4.GetBounds(), this.FrameHatchStyle, this.FrameGradientType, this.FrameColor, this.FrameGradientEndColor, true, this.frameWidth);
								pen = new Pen(brush, 2f);
								g.DrawPath(pen, graphicsPath3);
								g.FillPath(brush2, graphicsPath4, 0f, false, this.FrameShape == BackFrameShape.Circular);
							}
						}
						else if (this.FrameStyle == BackFrameStyle.Edged)
						{
							float num = (float)(this.FrameWidth * 0.699999988079071);
							using (GraphicsPath addingPath = this.GetFramePath(g, num))
							{
								using (GraphicsPath graphicsPath5 = new GraphicsPath())
								{
									using (GraphicsPath graphicsPath6 = new GraphicsPath())
									{
										graphicsPath5.AddPath(graphicsPath, false);
										graphicsPath5.AddPath(addingPath, false);
										graphicsPath6.AddPath(addingPath, false);
										graphicsPath6.AddPath(graphicsPath3, false);
										brush2 = this.GetBrush(g, graphicsPath5.GetBounds(), this.FrameHatchStyle, this.FrameGradientType, this.FrameColor, this.FrameGradientEndColor, true, num);
										g.FillPath(brush2, graphicsPath5, 0f, false, this.FrameShape == BackFrameShape.Circular);
										brush3 = ((this.FrameGradientType != 0 || this.FrameHatchStyle != 0) ? this.GetBrush(g, graphicsPath6.GetBounds(), this.FrameHatchStyle, this.FrameGradientType, this.FrameGradientEndColor, this.FrameColor, true, this.frameWidth - num) : this.GetBrush(g, graphicsPath6.GetBounds(), this.FrameHatchStyle, this.FrameGradientType, this.FrameColor, this.FrameColor, true, this.frameWidth - num));
										if (this.FrameWidth > 0.0)
										{
											pen = new Pen(brush3, 2f);
											g.DrawPath(pen, graphicsPath6);
										}
										g.FillPath(brush3, graphicsPath6, 0f, false, this.FrameShape == BackFrameShape.Circular);
									}
								}
							}
						}
						if (this.BorderWidth > 0 && this.BorderStyle != 0)
						{
							pen2 = new Pen(this.BorderColor, (float)this.BorderWidth);
							pen2.DashStyle = g.GetPenStyle(this.BorderStyle);
							pen2.Alignment = PenAlignment.Center;
							g.DrawPath(pen2, graphicsPath);
						}
					}
					finally
					{
						if (brush != null)
						{
							brush.Dispose();
						}
						if (graphicsPath != null)
						{
							graphicsPath.Dispose();
						}
						if (graphicsPath2 != null)
						{
							graphicsPath2.Dispose();
						}
						if (brush2 != null)
						{
							brush2.Dispose();
						}
						if (brush3 != null)
						{
							brush3.Dispose();
						}
						if (pen != null)
						{
							pen.Dispose();
						}
						if (pen2 != null)
						{
							pen2.Dispose();
						}
					}
				}
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceFrameRenderingComplete);
			}
		}

		internal void RenderShadow(GaugeGraphics g)
		{
			if (this.ShadowOffset != 0.0)
			{
				using (GraphicsPath path = this.GetShadowPath(g))
				{
					using (Brush brush = g.GetShadowBrush())
					{
						g.FillPath(brush, path);
					}
				}
			}
		}

		internal Brush GetBrush(GaugeGraphics g, RectangleF rect, GaugeHatchStyle hatchStyle, GradientType gradientType, Color fillColor, Color gradientEndColor, bool frame, float frameWidth)
		{
			Brush brush = null;
			if (hatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(hatchStyle, fillColor, gradientEndColor);
			}
			else if (gradientType != 0)
			{
				if (this.FrameShape == BackFrameShape.Circular && gradientType == GradientType.DiagonalLeft)
				{
					brush = g.GetGradientBrush(rect, fillColor, gradientEndColor, GradientType.LeftRight);
					Matrix matrix = new Matrix();
					matrix.RotateAt(45f, new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0)));
					((LinearGradientBrush)brush).Transform = matrix;
				}
				else if (this.FrameShape == BackFrameShape.Circular && gradientType == GradientType.DiagonalRight)
				{
					brush = g.GetGradientBrush(rect, fillColor, gradientEndColor, GradientType.TopBottom);
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(135f, new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0)));
					((LinearGradientBrush)brush).Transform = matrix2;
				}
				else if (gradientType == GradientType.Center)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					if (this.FrameShape == BackFrameShape.Circular)
					{
						graphicsPath.AddArc(rect.X, rect.Y, rect.Width, rect.Height, 0f, 360f);
					}
					else
					{
						graphicsPath.AddRectangle(rect);
					}
					PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
					pathGradientBrush.CenterColor = fillColor;
					pathGradientBrush.CenterPoint = new PointF((float)(rect.X + rect.Width / 2.0), (float)(rect.Y + rect.Height / 2.0));
					pathGradientBrush.SurroundColors = new Color[1]
					{
						gradientEndColor
					};
					if (frame)
					{
						pathGradientBrush.FocusScales = new PointF((float)((rect.Width - frameWidth * 2.0) / rect.Width), (float)((rect.Height - frameWidth * 2.0) / rect.Height));
					}
					brush = pathGradientBrush;
				}
				else
				{
					brush = g.GetGradientBrush(rect, fillColor, gradientEndColor, gradientType);
				}
			}
			else
			{
				brush = new SolidBrush(fillColor);
			}
			return brush;
		}

		internal RectangleF GetFrameRectangle(GaugeGraphics g)
		{
			RectangleF result;
			if (this.Parent is GaugeCore)
			{
				result = new RectangleF(0f, 0f, (float)(((GaugeCore)this.Parent).GetWidth() - 1), (float)(((GaugeCore)this.Parent).GetHeight() - 1));
			}
			else
			{
				if ((this.FrameShape == BackFrameShape.Rectangular || this.FrameShape == BackFrameShape.RoundedRectangular) && this.Parent is CircularGauge)
				{
					CircularGauge circularGauge = (CircularGauge)this.Parent;
					if (circularGauge.ParentObject != null)
					{
						result = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
						if (circularGauge.Position.Rectangle.Width > 0.0 && circularGauge.Position.Rectangle.Height > 0.0)
						{
							if (!double.IsNaN((double)circularGauge.AspectRatio))
							{
								if (circularGauge.AspectRatio >= 1.0)
								{
									float width = result.Width;
									result.Width = result.Height * circularGauge.AspectRatio;
									result.X += (float)((width - result.Width) / 2.0);
								}
								else
								{
									float height = result.Height;
									result.Height = result.Width / circularGauge.AspectRatio;
									result.Y += (float)((height - result.Height) / 2.0);
								}
							}
							else
							{
								float num = circularGauge.Position.Rectangle.Width / circularGauge.Position.Rectangle.Height;
								if (circularGauge.Position.Rectangle.Width > circularGauge.Position.Rectangle.Height)
								{
									float num2 = result.Height * num;
									result.X += (float)((result.Width - num2) / 2.0);
									result.Width = num2;
								}
								else
								{
									float num3 = result.Width / num;
									result.Y += (float)((result.Height - num3) / 2.0);
									result.Height = num3;
								}
							}
						}
					}
					else
					{
						result = circularGauge.absoluteRect;
						if (!double.IsNaN((double)circularGauge.AspectRatio))
						{
							if (result.Width > result.Height * circularGauge.AspectRatio)
							{
								float width2 = result.Width;
								result.Width = result.Height * circularGauge.AspectRatio;
								result.X += (float)((width2 - result.Width) / 2.0);
							}
							else
							{
								float height2 = result.Height;
								result.Height = result.Width / circularGauge.AspectRatio;
								result.Y += (float)((height2 - result.Height) / 2.0);
							}
						}
						PointF empty = PointF.Empty;
						empty.X = (float)(0.0 - g.Graphics.Transform.OffsetX + g.InitialOffset.X);
						empty.Y = (float)(0.0 - g.Graphics.Transform.OffsetY + g.InitialOffset.Y);
						result.Offset(empty.X, empty.Y);
					}
					goto IL_0371;
				}
				result = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			}
			goto IL_0371;
			IL_0371:
			if (this.FrameShape == BackFrameShape.Circular || this.IsCustomXamlFrame())
			{
				if (result.Width > result.Height)
				{
					result.X += (float)((result.Width - result.Height) / 2.0);
					result.Width = result.Height;
				}
				else if (result.Height > result.Width)
				{
					result.Y += (float)((result.Height - result.Width) / 2.0);
					result.Height = result.Width;
				}
			}
			float xamlFrameAspectRatio = BackFrame.GetXamlFrameAspectRatio(this.Shape);
			if (xamlFrameAspectRatio > 1.0)
			{
				float num4 = result.Height * xamlFrameAspectRatio;
				result.X += (float)((result.Width - num4) / 2.0);
				result.Width = num4;
			}
			else if (xamlFrameAspectRatio < 1.0)
			{
				float num5 = result.Width / xamlFrameAspectRatio;
				result.Y += (float)((result.Height - num5) / 2.0);
				result.Height = num5;
			}
			if (this.Parent is GaugeCore)
			{
				if (this.ShadowOffset < 0.0)
				{
					result.X -= this.ShadowOffset;
					result.Y -= this.ShadowOffset;
					result.Width += this.ShadowOffset;
					result.Height += this.ShadowOffset;
				}
				else if (this.ShadowOffset > 0.0)
				{
					result.Width -= this.ShadowOffset;
					result.Height -= this.ShadowOffset;
				}
			}
			return result;
		}

		internal void RenderGlassEffect(GaugeGraphics g)
		{
			if (this.GlassEffect != 0 && this.FrameStyle != 0)
			{
				if (this.IsCustomXamlFrame())
				{
					XamlRenderer cachedXamlRenderer = this.GetCachedXamlRenderer(g);
					cachedXamlRenderer.Layers[2].Render(g);
				}
				else
				{
					RectangleF bounds;
					using (GraphicsPath graphicsPath = this.GetFramePath(g, (float)(this.FrameWidth - 1.0)))
					{
						bounds = graphicsPath.GetBounds();
						using (Brush brush = new LinearGradientBrush(bounds, Color.FromArgb(15, Color.Black), Color.FromArgb(128, Color.Black), LinearGradientMode.ForwardDiagonal))
						{
							if (bounds.Height > 0.0 && bounds.Width > 0.0)
							{
								g.FillPath(brush, graphicsPath);
							}
						}
					}
					if (this.FrameShape == BackFrameShape.Rectangular || this.FrameShape == BackFrameShape.RoundedRectangular)
					{
						float width = g.GetAbsoluteSize(new SizeF(8f, 0f)).Width;
						GraphicsPath graphicsPath2 = new GraphicsPath();
						float absoluteDimension = g.GetAbsoluteDimension(30f);
						float absoluteDimension2 = g.GetAbsoluteDimension(10f);
						float absoluteDimension3 = g.GetAbsoluteDimension(50f);
						float absoluteDimension4 = g.GetAbsoluteDimension(5f);
						g.GetAbsoluteDimension(30f);
						g.GetAbsoluteDimension(5f);
						PointF[] points = new PointF[4]
						{
							new PointF(bounds.X, bounds.Y + absoluteDimension),
							new PointF(bounds.X + absoluteDimension, bounds.Y),
							new PointF(bounds.X + absoluteDimension + absoluteDimension2, bounds.Y),
							new PointF(bounds.X, bounds.Y + absoluteDimension + absoluteDimension2)
						};
						PointF[] points2 = new PointF[4]
						{
							new PointF(bounds.X, bounds.Y + absoluteDimension3),
							new PointF(bounds.X + absoluteDimension3, bounds.Y),
							new PointF(bounds.X + absoluteDimension3 + absoluteDimension4, bounds.Y),
							new PointF(bounds.X, bounds.Y + absoluteDimension3 + absoluteDimension4)
						};
						graphicsPath2.AddPolygon(points);
						graphicsPath2.AddPolygon(points2);
						Brush brush2 = new SolidBrush(Color.FromArgb(148, Color.White));
						g.FillPath(brush2, graphicsPath2);
					}
					else
					{
						if (this.FrameShape != 0 && this.FrameShape != BackFrameShape.AutoShape)
						{
							return;
						}
						if (this.GlassEffect == GlassEffect.Simple)
						{
							float absoluteDimension5 = g.GetAbsoluteDimension(15f);
							bounds.X += absoluteDimension5;
							bounds.Y += absoluteDimension5;
							bounds.Width -= (float)(absoluteDimension5 * 2.0);
							bounds.Height -= (float)(absoluteDimension5 * 2.0);
							GraphicsPath circularRangePath = g.GetCircularRangePath(g.GetRelativeRectangle(bounds), 226f, 30f, 6f, 6f, Placement.Inside);
							GraphicsPath circularRangePath2 = g.GetCircularRangePath(g.GetRelativeRectangle(bounds), 224f, -30f, 6f, 6f, Placement.Inside);
							Brush brush3 = new SolidBrush(Color.FromArgb(200, Color.White));
							g.FillPath(brush3, circularRangePath);
							g.FillPath(brush3, circularRangePath2);
						}
					}
				}
			}
		}
	}
}
