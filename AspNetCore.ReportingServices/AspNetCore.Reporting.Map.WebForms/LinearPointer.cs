using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LinearPointer : MapObject, IToolTipProvider
	{
		private double position;

		internal bool dragging;

		private LinearPointerType type;

		private Placement placement = Placement.Outside;

		private float width = 20f;

		private MarkerStyle markerStyle = MarkerStyle.Triangle;

		private float markerLength = 20f;

		private MapCursor cursor = MapCursor.Default;

		private float distanceFromScale;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Point imageOrigin = Point.Empty;

		private double val;

		private bool snappingEnabled;

		private double snappingInterval;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		private bool interactive = true;

		private bool visible = true;

		private float shadowOffset = 2f;

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.White;

		private Color fillSecondaryColor = Color.Red;

		private MapHatchStyle fillHatchStyle;

		private GradientType fillGradientType = GradientType.DiagonalLeft;

		[SRCategory("CategoryAttribute_TypeSpecific")]
		[DefaultValue(LinearPointerType.Marker)]
		[SRDescription("DescriptionAttributeLinearPointer_Type")]
		[ParenthesizePropertyName(true)]
		public LinearPointerType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
				this.Invalidate();
			}
		}

		[DefaultValue(Placement.Outside)]
		[SRDescription("DescriptionAttributeLinearPointer_Placement")]
		[SRCategory("CategoryAttribute_Layout")]
		public Placement Placement
		{
			get
			{
				return this.placement;
			}
			set
			{
				this.placement = value;
				this.Invalidate();
			}
		}

		[DefaultValue(20f)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearPointer_Width")]
		public float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 200.0))
				{
					this.width = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 200.0));
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_MarkerStyle")]
		[DefaultValue(MarkerStyle.Triangle)]
		[SRCategory("CategoryAttribute_TypeSpecific")]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return this.markerStyle;
			}
			set
			{
				this.markerStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_MarkerLength")]
		[DefaultValue(20f)]
		[SRCategory("CategoryAttribute_TypeSpecific")]
		public float MarkerLength
		{
			get
			{
				return this.markerLength;
			}
			set
			{
				if (!(value < 0.0) && !(value > 200.0))
				{
					this.markerLength = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 200.0));
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_Cursor")]
		[DefaultValue(MapCursor.Default)]
		public MapCursor Cursor
		{
			get
			{
				return this.cursor;
			}
			set
			{
				this.cursor = value;
			}
		}

		[DefaultValue(0f)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearPointer_DistanceFromScale")]
		public virtual float DistanceFromScale
		{
			get
			{
				return this.distanceFromScale;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.distanceFromScale = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Image")]
		[SRDescription("DescriptionAttributeLinearPointer_Image")]
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

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLinearPointer_ImageTransColor")]
		public virtual Color ImageTransColor
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

		[SRCategory("CategoryAttribute_Image")]
		[DefaultValue(typeof(Point), "0, 0")]
		[SRDescription("DescriptionAttributeLinearPointer_ImageOrigin")]
		[TypeConverter(typeof(EmptyPointConverter))]
		public virtual Point ImageOrigin
		{
			get
			{
				return this.imageOrigin;
			}
			set
			{
				this.imageOrigin = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeLinearPointer_Value")]
		public virtual double Value
		{
			get
			{
				return this.val;
			}
			set
			{
				LinearScale scale = this.GetScale();
				this.val = Math.Max(scale.Minimum, Math.Min(value, scale.Maximum));
				this.Position = this.val;
				if (!this.dragging)
				{
					this.GetGauge().InternalZoomLevelChanged();
				}
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_SnappingEnabled")]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(false)]
		public virtual bool SnappingEnabled
		{
			get
			{
				return this.snappingEnabled;
			}
			set
			{
				this.snappingEnabled = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_SnappingInterval")]
		[DefaultValue(0.0)]
		[SRCategory("CategoryAttribute_Behavior")]
		public double SnappingInterval
		{
			get
			{
				return this.snappingInterval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.ExceptionValueCannotBeNegative);
				}
				this.snappingInterval = value;
				this.Invalidate();
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_ToolTip")]
		[Localizable(true)]
		public virtual string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_Href")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[Localizable(true)]
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

		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLinearPointer_MapAreaAttributes")]
		[SRCategory("CategoryAttribute_Behavior")]
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

		[DefaultValue(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearPointer_Interactive")]
		public virtual bool Interactive
		{
			get
			{
				return this.interactive;
			}
			set
			{
				this.interactive = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_Visible")]
		[DefaultValue(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[ParenthesizePropertyName(true)]
		public virtual bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(2f)]
		[SRDescription("DescriptionAttributeLinearPointer_ShadowOffset")]
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

		[SRDescription("DescriptionAttributeLinearPointer_BorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_BorderStyle")]
		[DefaultValue(MapDashStyle.None)]
		public MapDashStyle BorderStyle
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

		[DefaultValue(1)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearPointer_BorderWidth")]
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
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_FillColor")]
		[DefaultValue(typeof(Color), "White")]
		[SRCategory("CategoryAttribute_Appearance")]
		public virtual Color FillColor
		{
			get
			{
				return this.fillColor;
			}
			set
			{
				this.fillColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Red")]
		[SRDescription("DescriptionAttributeLinearPointer_FillSecondaryColor")]
		[SRCategory("CategoryAttribute_Appearance")]
		public virtual Color FillSecondaryColor
		{
			get
			{
				return this.fillSecondaryColor;
			}
			set
			{
				this.fillSecondaryColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_FillHatchStyle")]
		[SRCategory("CategoryAttribute_Appearance")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[DefaultValue(MapHatchStyle.None)]
		public virtual MapHatchStyle FillHatchStyle
		{
			get
			{
				return this.fillHatchStyle;
			}
			set
			{
				this.fillHatchStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(GradientType.DiagonalLeft)]
		[SRDescription("DescriptionAttributeLinearPointer_FillGradientType")]
		[SRCategory("CategoryAttribute_Appearance")]
		public virtual GradientType FillGradientType
		{
			get
			{
				return this.fillGradientType;
			}
			set
			{
				this.fillGradientType = value;
				this.Invalidate();
			}
		}

		internal double Position
		{
			get
			{
				if (double.IsNaN(this.position))
				{
					return this.GetScale().GetValueLimit(this.position);
				}
				return this.position;
			}
			set
			{
				this.position = value;
				this.Invalidate();
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
			}
		}

		public LinearPointer()
			: this(null)
		{
		}

		public LinearPointer(object parent)
			: base(parent)
		{
			this.markerStyle = MarkerStyle.Triangle;
			this.markerLength = 20f;
			this.width = 20f;
			this.fillGradientType = GradientType.DiagonalLeft;
		}

		internal void Render(MapGraphics g)
		{
			if (this.Common != null && this.Visible && this.GetScale() != null)
			{
				g.StartHotRegion(this);
				if (!string.IsNullOrEmpty(this.Image))
				{
					this.DrawImage(g, false);
					g.EndHotRegion();
				}
				else
				{
					Pen pen = new Pen(this.BorderColor, (float)this.BorderWidth);
					pen.DashStyle = MapGraphics.GetPenStyle(this.BorderStyle);
					if (pen.DashStyle != 0)
					{
						pen.Alignment = PenAlignment.Center;
					}
					MarkerStyleAttrib markerStyleAttrib = this.GetMarkerStyleAttrib(g);
					try
					{
						if (markerStyleAttrib.path != null)
						{
							bool circularFill = (byte)((this.MarkerStyle == MarkerStyle.Circle) ? 1 : 0) != 0;
							g.FillPath(markerStyleAttrib.brush, markerStyleAttrib.path, 0f, true, circularFill);
						}
						if (this.BorderWidth > 0 && markerStyleAttrib.path != null)
						{
							g.DrawPath(pen, markerStyleAttrib.path);
						}
					}
					catch (Exception)
					{
						markerStyleAttrib.Dispose();
					}
					if (markerStyleAttrib.path != null)
					{
						this.Common.MapCore.HotRegionList.SetHotRegion(g, this, markerStyleAttrib.path);
					}
					g.EndHotRegion();
				}
			}
		}

		internal void DrawImage(MapGraphics g, bool drawShadow)
		{
			if (this.Visible)
			{
				if (drawShadow && this.ShadowOffset == 0.0)
				{
					return;
				}
				Image image = this.Common.ImageLoader.LoadImage(this.Image);
				if (image.Width != 0 && image.Height != 0)
				{
					Point point = new Point(this.ImageOrigin.X, this.ImageOrigin.Y);
					if (point.X == 0 && point.Y == 0)
					{
						point.X = image.Width / 2;
						point.Y = image.Height / 2;
					}
					float absoluteDimension = g.GetAbsoluteDimension(this.Width);
					float absoluteDimension2 = g.GetAbsoluteDimension(this.MarkerLength);
					float num = absoluteDimension / (float)image.Width;
					float num2 = absoluteDimension2 / (float)image.Height;
					float num3 = this.CalculateMarkerDistance();
					float positionFromValue = this.GetScale().GetPositionFromValue(this.Position);
					PointF pointF = Point.Empty;
					pointF = ((this.GetGauge().GetOrientation() != 0) ? g.GetAbsolutePoint(new PointF(num3, positionFromValue)) : g.GetAbsolutePoint(new PointF(positionFromValue, num3)));
					Rectangle rectangle = new Rectangle((int)(pointF.X - (float)point.X * num) + 1, (int)(pointF.Y - (float)point.Y * num2) + 1, (int)((float)image.Width * num) + 1, (int)((float)image.Height * num2) + 1);
					ImageAttributes imageAttributes = new ImageAttributes();
					if (this.ImageTransColor != Color.Empty)
					{
						imageAttributes.SetColorKey(this.ImageTransColor, this.ImageTransColor, ColorAdjustType.Default);
					}
					if (drawShadow)
					{
						ColorMatrix colorMatrix = new ColorMatrix();
						colorMatrix.Matrix00 = 0f;
						colorMatrix.Matrix11 = 0f;
						colorMatrix.Matrix22 = 0f;
						colorMatrix.Matrix33 = (float)(this.Common.MapCore.ShadowIntensity / 100.0);
						imageAttributes.SetColorMatrix(colorMatrix);
					}
					g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					if (!drawShadow)
					{
						using (GraphicsPath graphicsPath = new GraphicsPath())
						{
							graphicsPath.AddRectangle(rectangle);
							this.Common.MapCore.HotRegionList.SetHotRegion(g, this, graphicsPath);
						}
					}
				}
			}
		}

		internal MarkerStyleAttrib GetMarkerStyleAttrib(MapGraphics g)
		{
			MarkerStyleAttrib markerStyleAttrib = new MarkerStyleAttrib();
			if (!string.IsNullOrEmpty(this.Image))
			{
				return markerStyleAttrib;
			}
			float absoluteDimension = g.GetAbsoluteDimension(this.MarkerLength);
			float absoluteDimension2 = g.GetAbsoluteDimension(this.Width);
			markerStyleAttrib.path = g.CreateMarker(new PointF(0f, 0f), absoluteDimension2, absoluteDimension, this.MarkerStyle);
			float num = 0f;
			if (this.Placement == Placement.Cross || this.Placement == Placement.Inside)
			{
				num = (float)(num + 180.0);
			}
			if (this.GetGauge().GetOrientation() == Orientation.Vertical)
			{
				num = (float)(num + 270.0);
			}
			if (num > 0.0)
			{
				using (Matrix matrix = new Matrix())
				{
					matrix.Rotate(num);
					markerStyleAttrib.path.Transform(matrix);
				}
			}
			float num2 = this.CalculateMarkerDistance();
			LinearScale scale = this.GetScale();
			float positionFromValue = scale.GetPositionFromValue(scale.GetValueLimit(this.Position));
			PointF pointOrigin = Point.Empty;
			pointOrigin = ((this.GetGauge().GetOrientation() != 0) ? g.GetAbsolutePoint(new PointF(num2, positionFromValue)) : g.GetAbsolutePoint(new PointF(positionFromValue, num2)));
			markerStyleAttrib.brush = g.GetMarkerBrush(markerStyleAttrib.path, this.MarkerStyle, pointOrigin, 0f, this.FillColor, this.FillGradientType, this.FillSecondaryColor, this.FillHatchStyle);
			using (Matrix matrix2 = new Matrix())
			{
				matrix2.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				markerStyleAttrib.path.Transform(matrix2);
				return markerStyleAttrib;
			}
		}

		internal float CalculateMarkerDistance()
		{
			if (this.Placement == Placement.Cross)
			{
				return this.GetScale().Position - this.DistanceFromScale;
			}
			if (this.Placement == Placement.Inside)
			{
				return (float)(this.GetScale().Position - this.GetScale().Width / 2.0 - this.DistanceFromScale - this.MarkerLength / 2.0);
			}
			return (float)(this.GetScale().Position + this.GetScale().Width / 2.0 + this.DistanceFromScale + this.MarkerLength / 2.0);
		}

		internal GraphicsPath GetPointerPath(MapGraphics g)
		{
			if (!this.Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			LinearScale scale = this.GetScale();
			scale.GetPositionFromValue(scale.GetValueLimit(this.Position));
			if (this.Type == LinearPointerType.Marker)
			{
				MarkerStyleAttrib markerStyleAttrib = this.GetMarkerStyleAttrib(g);
				if (markerStyleAttrib.path != null)
				{
					graphicsPath.AddPath(markerStyleAttrib.path, false);
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(MapGraphics g)
		{
			if (this.ShadowOffset != 0.0 && this.GetScale() != null)
			{
				if (!string.IsNullOrEmpty(this.Image))
				{
					this.DrawImage(g, true);
				}
				GraphicsPath pointerPath = this.GetPointerPath(g);
				if (pointerPath != null && pointerPath.PointCount != 0)
				{
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(this.ShadowOffset, this.ShadowOffset);
						pointerPath.Transform(matrix);
						return pointerPath;
					}
				}
				return null;
			}
			return null;
		}

		public ZoomPanel GetGauge()
		{
			return (ZoomPanel)this.Parent;
		}

		internal LinearScale GetScale()
		{
			return this.GetGauge().Scale;
		}

		internal virtual void DragTo(int x, int y, PointF refPoint, bool dragging)
		{
			LinearScale scale = this.GetScale();
			this.dragging = dragging;
			double value = scale.GetValue(refPoint, new PointF((float)x, (float)y));
			value = scale.GetValueLimit(value, this.SnappingEnabled, this.SnappingInterval);
			if (this.Common != null)
			{
				this.Value = value;
			}
		}

		internal virtual void RenderShadow(MapGraphics g)
		{
		}

		internal override void BeginInit()
		{
			base.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
		}

		string IToolTipProvider.GetToolTip()
		{
			return this.ToolTip;
		}
	}
}
