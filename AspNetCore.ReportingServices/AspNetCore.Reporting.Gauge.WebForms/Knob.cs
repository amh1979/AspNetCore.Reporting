using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(KnobConverter))]
	internal class Knob : PointerBase, ISelectable
	{
		private bool capVisible = true;

		private bool capReflection = true;

		private float capWidth = 60f;

		private string capImage = "";

		private Color capImageTransColor = Color.Empty;

		private Color capImageHueColor = Color.Empty;

		private Point capImageOrigin = Point.Empty;

		private KnobStyle style;

		private Color capFillColor = Color.Gainsboro;

		private GradientType capFillGradientType = GradientType.DiagonalLeft;

		private Color capFillGradientEndColor = Color.Gray;

		private GaugeHatchStyle capFillHatchStyle;

		private float markerLength = 15f;

		private float markerWidth = 10f;

		private float markerPosition = 36f;

		private Color markerFillColor = Color.DarkGray;

		private GradientType markerFillGradientType = GradientType.VerticalCenter;

		private Color markerFillGradientEndColor = Color.White;

		private GaugeHatchStyle markerFillHatchStyle;

		private float capShadowOffset = 2f;

		private bool rotateGradient;

		private bool capRotateGradient;

		private bool markerRotateGradient = true;

		private bool selected;

		private GraphicsPath[] hotRegions = new GraphicsPath[2];

		[SRDescription("DescriptionAttributeKnob_Name")]
		[SRCategory("CategoryMisc")]
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

		[SRCategory("CategoryMisc")]
		[TypeConverter(typeof(ScaleSourceConverter))]
		[SRDescription("DescriptionAttributeKnob_ScaleName")]
		[DefaultValue("Default")]
		public override string ScaleName
		{
			get
			{
				return base.ScaleName;
			}
			set
			{
				base.ScaleName = value;
			}
		}

		[SRCategory("CategoryImage")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeKnob_Image")]
		public override string Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_ImageTransColor")]
		public override Color ImageTransColor
		{
			get
			{
				return base.ImageTransColor;
			}
			set
			{
				base.ImageTransColor = value;
			}
		}

		[SRCategory("CategoryImage")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeKnob_ImageHueColor")]
		public override Color ImageHueColor
		{
			get
			{
				return base.ImageHueColor;
			}
			set
			{
				base.ImageHueColor = value;
			}
		}

		[TypeConverter(typeof(EmptyPointConverter))]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_ImageOrigin")]
		[DefaultValue(typeof(Point), "0, 0")]
		public override Point ImageOrigin
		{
			get
			{
				return base.ImageOrigin;
			}
			set
			{
				base.ImageOrigin = value;
			}
		}

		[SRCategory("CategoryData")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeKnob_Value")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
		public override double Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

		[SRCategory("CategoryData")]
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeKnob_ValueSource")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[TypeConverter(typeof(ValueSourceConverter))]
		public override string ValueSource
		{
			get
			{
				return base.ValueSource;
			}
			set
			{
				base.ValueSource = value;
			}
		}

		[DefaultValue(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeKnob_SnappingEnabled")]
		public override bool SnappingEnabled
		{
			get
			{
				return base.SnappingEnabled;
			}
			set
			{
				base.SnappingEnabled = value;
			}
		}

		[DefaultValue(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeKnob_DampeningEnabled")]
		public override bool DampeningEnabled
		{
			get
			{
				return base.DampeningEnabled;
			}
			set
			{
				base.DampeningEnabled = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeKnob_ToolTip")]
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

		[DefaultValue(true)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeKnob_Interactive")]
		public override bool Interactive
		{
			get
			{
				return base.Interactive;
			}
			set
			{
				base.Interactive = value;
			}
		}

		[SRDescription("DescriptionAttributeKnob_Visible")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(true)]
		[ParenthesizePropertyName(true)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}

		[DefaultValue(typeof(Color), "Gainsboro")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_FillColor")]
		public override Color FillColor
		{
			get
			{
				return base.FillColor;
			}
			set
			{
				base.FillColor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "Gray")]
		[SRDescription("DescriptionAttributeKnob_FillGradientEndColor")]
		public override Color FillGradientEndColor
		{
			get
			{
				return base.FillGradientEndColor;
			}
			set
			{
				base.FillGradientEndColor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[DefaultValue(GaugeHatchStyle.None)]
		[SRDescription("DescriptionAttributeKnob_FillHatchStyle")]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public override GaugeHatchStyle FillHatchStyle
		{
			get
			{
				return base.FillHatchStyle;
			}
			set
			{
				base.FillHatchStyle = value;
			}
		}

		[DefaultValue(GradientType.DiagonalLeft)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_FillGradientType")]
		public override GradientType FillGradientType
		{
			get
			{
				return base.FillGradientType;
			}
			set
			{
				base.FillGradientType = value;
			}
		}

		[DefaultValue(true)]
		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapVisible")]
		[ParenthesizePropertyName(true)]
		public bool CapVisible
		{
			get
			{
				return this.capVisible;
			}
			set
			{
				this.capVisible = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeKnob_CapReflection")]
		public bool CapReflection
		{
			get
			{
				return this.capReflection;
			}
			set
			{
				this.capReflection = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[DefaultValue(60f)]
		[SRDescription("DescriptionAttributeKnob_CapWidth")]
		[ValidateBound(0.0, 100.0)]
		public float CapWidth
		{
			get
			{
				return this.capWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.capWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_CapImage")]
		public string CapImage
		{
			get
			{
				return this.capImage;
			}
			set
			{
				this.capImage = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeKnob_CapImageTransColor")]
		[SRCategory("CategoryImage")]
		[DefaultValue(typeof(Color), "")]
		public Color CapImageTransColor
		{
			get
			{
				return this.capImageTransColor;
			}
			set
			{
				this.capImageTransColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCapImageHueColor")]
		[DefaultValue(typeof(Color), "")]
		public Color CapImageHueColor
		{
			get
			{
				return this.capImageHueColor;
			}
			set
			{
				this.capImageHueColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Point), "0, 0")]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_CapImageOrigin")]
		[TypeConverter(typeof(EmptyPointConverter))]
		public Point CapImageOrigin
		{
			get
			{
				return this.capImageOrigin;
			}
			set
			{
				this.capImageOrigin = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeKnob_Width")]
		[ValidateBound(0.0, 200.0)]
		[DefaultValue(80f)]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					base.Width = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[SRCategory("CategoryMarker")]
		[DefaultValue(MarkerStyle.Wedge)]
		[SRDescription("DescriptionAttributeKnob_Style")]
		public override MarkerStyle MarkerStyle
		{
			get
			{
				return base.MarkerStyle;
			}
			set
			{
				base.MarkerStyle = value;
				this.Invalidate();
			}
		}

		[Obsolete("This property is obsolete in Dundas Gauge 2.0. KnobStyle is supposed to be used instead.")]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeKnob_Style")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(KnobStyle.Style1)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public KnobStyle Style
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

		[SRCategory("CategoryLayout")]
		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeKnob_Style")]
		[DefaultValue(KnobStyle.Style1)]
		public KnobStyle KnobStyle
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

		[DefaultValue(typeof(Color), "Gainsboro")]
		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapFillColor")]
		public Color CapFillColor
		{
			get
			{
				return this.capFillColor;
			}
			set
			{
				this.capFillColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(GradientType.DiagonalLeft)]
		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillGradientType")]
		public GradientType CapFillGradientType
		{
			get
			{
				return this.capFillGradientType;
			}
			set
			{
				this.capFillGradientType = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapFillGradientEndColor")]
		[DefaultValue(typeof(Color), "Gray")]
		public Color CapFillGradientEndColor
		{
			get
			{
				return this.capFillGradientEndColor;
			}
			set
			{
				this.capFillGradientEndColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[DefaultValue(GaugeHatchStyle.None)]
		[SRDescription("DescriptionAttributeKnob_CapFillHatchStyle")]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public GaugeHatchStyle CapFillHatchStyle
		{
			get
			{
				return this.capFillHatchStyle;
			}
			set
			{
				this.capFillHatchStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(15f)]
		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerLength")]
		[ValidateBound(0.0, 50.0)]
		public override float MarkerLength
		{
			get
			{
				return this.markerLength;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.markerLength = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[DefaultValue(10f)]
		[ValidateBound(0.0, 50.0)]
		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerWidth")]
		public float MarkerWidth
		{
			get
			{
				return this.markerWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.markerWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[ValidateBound(0.0, 100.0)]
		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerPosition")]
		[DefaultValue(36f)]
		public float MarkerPosition
		{
			get
			{
				return this.markerPosition;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.markerPosition = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRCategory("CategoryMarker")]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillColor")]
		public Color MarkerFillColor
		{
			get
			{
				return this.markerFillColor;
			}
			set
			{
				this.markerFillColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(GradientType.VerticalCenter)]
		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillGradientType")]
		public GradientType MarkerFillGradientType
		{
			get
			{
				return this.markerFillGradientType;
			}
			set
			{
				this.markerFillGradientType = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillGradientEndColor")]
		public Color MarkerFillGradientEndColor
		{
			get
			{
				return this.markerFillGradientEndColor;
			}
			set
			{
				this.markerFillGradientEndColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[DefaultValue(GaugeHatchStyle.None)]
		[SRDescription("DescriptionAttributeKnob_MarkerFillHatchStyle")]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public GaugeHatchStyle MarkerFillHatchStyle
		{
			get
			{
				return this.markerFillHatchStyle;
			}
			set
			{
				this.markerFillHatchStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[DefaultValue(2f)]
		[ValidateBound(-5.0, 5.0)]
		[SRDescription("DescriptionAttributeKnob_CapShadowOffset")]
		public float CapShadowOffset
		{
			get
			{
				return this.capShadowOffset;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.capShadowOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_RotateGradient")]
		[DefaultValue(false)]
		public bool RotateGradient
		{
			get
			{
				return this.rotateGradient;
			}
			set
			{
				this.rotateGradient = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapRotateGradient")]
		[DefaultValue(false)]
		public bool CapRotateGradient
		{
			get
			{
				return this.capRotateGradient;
			}
			set
			{
				this.capRotateGradient = value;
				this.Invalidate();
			}
		}

		[DefaultValue(true)]
		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerRotateGradient")]
		public bool MarkerRotateGradient
		{
			get
			{
				return this.markerRotateGradient;
			}
			set
			{
				this.markerRotateGradient = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeKnob_Cursor")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(GaugeCursor.Default)]
		public override GaugeCursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				base.Cursor = value;
			}
		}

		[SRDescription("DescriptionAttributeKnob_Selected")]
		[DefaultValue(false)]
		[SRCategory("CategoryAppearance")]
		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				this.selected = value;
				this.Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override float DistanceFromScale
		{
			get
			{
				return base.DistanceFromScale;
			}
			set
			{
				base.DistanceFromScale = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override BarStart BarStart
		{
			get
			{
				return base.BarStart;
			}
			set
			{
				base.BarStart = value;
			}
		}

		public Knob()
			: base(MarkerStyle.Wedge, 15f, 80f, GradientType.DiagonalLeft, Color.Gainsboro, Color.Gray, true)
		{
		}

		protected bool ShouldSerializeStyle()
		{
			return false;
		}

		internal KnobStyleAttrib GetKnobStyleAttrib(GaugeGraphics g, PointF pointOrigin, float angle)
		{
			KnobStyleAttrib knobStyleAttrib = new KnobStyleAttrib();
			if (this.Image != "" && this.CapImage != "")
			{
				return knobStyleAttrib;
			}
			float absoluteDimension = g.GetAbsoluteDimension(this.Width);
			float num = (float)(this.CapWidth / 100.0 * absoluteDimension);
			knobStyleAttrib.paths = new GraphicsPath[6];
			knobStyleAttrib.brushes = new Brush[6];
			if (this.Image == "")
			{
				knobStyleAttrib.paths[0] = this.GetKnobPath(g, absoluteDimension, (float)(absoluteDimension * 0.5));
			}
			else
			{
				knobStyleAttrib.paths[0] = null;
			}
			if (this.CapVisible && this.CapImage == "")
			{
				if (this.CapShadowOffset != 0.0)
				{
					knobStyleAttrib.paths[1] = new GraphicsPath();
					knobStyleAttrib.paths[1].AddEllipse((float)(0.0 - num + this.CapShadowOffset), (float)(0.0 - num + this.CapShadowOffset), (float)(num * 2.0), (float)(num * 2.0));
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
						knobStyleAttrib.paths[1].Transform(matrix);
					}
				}
				knobStyleAttrib.paths[2] = new GraphicsPath();
				knobStyleAttrib.paths[2].AddEllipse((float)(0.0 - num), (float)(0.0 - num), (float)(num * 2.0), (float)(num * 2.0));
			}
			else
			{
				if (this.CapShadowOffset == 0.0)
				{
					knobStyleAttrib.paths[1] = null;
				}
				knobStyleAttrib.paths[2] = null;
			}
			float y = (float)(this.MarkerPosition / 100.0 * absoluteDimension * 2.0);
			float num2 = (float)(this.MarkerWidth / 100.0 * absoluteDimension * 2.0);
			float markerHeight = (float)(this.MarkerLength / 100.0 * absoluteDimension * 2.0);
			PointF point = new PointF(0f, y);
			knobStyleAttrib.paths[3] = g.CreateMarker(point, num2, markerHeight, this.MarkerStyle);
			using (Matrix matrix2 = new Matrix())
			{
				matrix2.RotateAt(180f, point, MatrixOrder.Append);
				if (!this.MarkerRotateGradient)
				{
					matrix2.Rotate(angle, MatrixOrder.Append);
					matrix2.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
				knobStyleAttrib.paths[3].Transform(matrix2);
			}
			if (this.Image == "" && knobStyleAttrib.paths[0] != null)
			{
				float angle2 = (float)(this.RotateGradient ? angle : 0.0);
				knobStyleAttrib.brushes[0] = this.GetFillBrush(g, knobStyleAttrib.paths[0], pointOrigin, angle2, this.FillColor, this.FillGradientType, this.FillGradientEndColor, this.FillHatchStyle);
			}
			else
			{
				knobStyleAttrib.brushes[0] = null;
			}
			if (this.CapVisible && this.CapImage == "")
			{
				if (this.CapShadowOffset != 0.0)
				{
					knobStyleAttrib.brushes[1] = g.GetShadowBrush();
				}
				float angle3 = (float)(this.CapRotateGradient ? angle : 0.0);
				knobStyleAttrib.brushes[2] = this.GetFillBrush(g, knobStyleAttrib.paths[2], pointOrigin, angle3, this.CapFillColor, this.CapFillGradientType, this.CapFillGradientEndColor, this.CapFillHatchStyle);
			}
			else
			{
				if (this.CapShadowOffset == 0.0)
				{
					knobStyleAttrib.brushes[1] = null;
				}
				knobStyleAttrib.brushes[2] = null;
			}
			float angle4 = (float)(this.MarkerRotateGradient ? angle : 0.0);
			PointF pointOrigin2 = pointOrigin;
			if (!this.MarkerRotateGradient)
			{
				pointOrigin2 = new PointF(0f, 0f);
			}
			knobStyleAttrib.brushes[3] = g.GetMarkerBrush(knobStyleAttrib.paths[3], this.MarkerStyle, pointOrigin2, angle4, this.MarkerFillColor, this.MarkerFillGradientType, this.MarkerFillGradientEndColor, this.MarkerFillHatchStyle);
			if (this.CapVisible && this.CapReflection && this.CapImage == "")
			{
				g.GetCircularEdgeReflection(knobStyleAttrib.paths[2].GetBounds(), 135f, 200, pointOrigin, out knobStyleAttrib.paths[4], out knobStyleAttrib.brushes[4]);
				g.GetCircularEdgeReflection(knobStyleAttrib.paths[2].GetBounds(), 315f, 128, pointOrigin, out knobStyleAttrib.paths[5], out knobStyleAttrib.brushes[5]);
			}
			else
			{
				knobStyleAttrib.paths[4] = null;
				knobStyleAttrib.paths[5] = null;
				knobStyleAttrib.brushes[4] = null;
				knobStyleAttrib.brushes[5] = null;
			}
			using (Matrix matrix3 = new Matrix())
			{
				matrix3.Rotate(angle, MatrixOrder.Append);
				matrix3.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				if (knobStyleAttrib.paths[0] != null)
				{
					knobStyleAttrib.paths[0].Transform(matrix3);
				}
				if (knobStyleAttrib.paths[2] != null)
				{
					knobStyleAttrib.paths[2].Transform(matrix3);
				}
				if (knobStyleAttrib.paths[3] != null)
				{
					if (this.MarkerRotateGradient)
					{
						knobStyleAttrib.paths[3].Transform(matrix3);
						return knobStyleAttrib;
					}
					return knobStyleAttrib;
				}
				return knobStyleAttrib;
			}
		}

		private GraphicsPath GetKnobPath(GaugeGraphics g, float knobRadius, float capRadius)
		{
			if ((double)knobRadius < 0.0001)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			if (this.KnobStyle == KnobStyle.Style1)
			{
				float num = (float)(knobRadius * 0.949999988079071);
				bool flag = false;
				for (int i = 15; i < 360; i += 30)
				{
					if (flag)
					{
						graphicsPath.AddArc((float)(0.0 - knobRadius), (float)(0.0 - knobRadius), (float)(knobRadius * 2.0), (float)(knobRadius * 2.0), (float)i, 30f);
					}
					else
					{
						graphicsPath.AddArc((float)(0.0 - num), (float)(0.0 - num), (float)(num * 2.0), (float)(num * 2.0), (float)i, 30f);
					}
					flag = !flag;
				}
			}
			else if (this.KnobStyle == KnobStyle.Style2)
			{
				for (int j = 15; j < 360; j += 30)
				{
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						float num2 = knobRadius - capRadius;
						float num3 = capRadius + num2;
						graphicsPath2.AddArc((float)((0.0 - num2) / 6.0), (float)(num3 - num2 / 6.0), (float)(num2 / 3.0), (float)(num2 / 3.0), 0f, -180f);
						using (Matrix matrix = new Matrix())
						{
							matrix.Rotate((float)j);
							graphicsPath2.Transform(matrix);
						}
						graphicsPath.AddPath(graphicsPath2, true);
					}
				}
			}
			else if (this.KnobStyle == KnobStyle.Style3)
			{
				graphicsPath.AddEllipse((float)(0.0 - knobRadius), (float)(0.0 - knobRadius), (float)(knobRadius * 2.0), (float)(knobRadius * 2.0));
			}
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		private PathGradientBrush GetSpecialCapBrush(GaugeGraphics g, GraphicsPath path, PointF pointOrigin, float angle, Color fillColor, GradientType fillGradientType, Color fillGradientEndColor, GaugeHatchStyle fillHatchStyle)
		{
			using (GraphicsPath graphicsPath = (GraphicsPath)path.Clone())
			{
				graphicsPath.Flatten(null, 0.3f);
				graphicsPath.Reset();
				RectangleF bounds = path.GetBounds();
				bounds.Inflate(-20f, -20f);
				PointF[] points = new PointF[4]
				{
					new PointF(bounds.Left, bounds.Top),
					new PointF(bounds.Right, bounds.Top),
					new PointF(bounds.Right, bounds.Bottom),
					new PointF(bounds.Left, bounds.Bottom)
				};
				graphicsPath.AddLines(points);
				PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
				pathGradientBrush.SurroundColors = new Color[4]
				{
					Color.Red,
					Color.Green,
					Color.Blue,
					Color.Green
				};
				pathGradientBrush.CenterColor = Color.Transparent;
				pathGradientBrush.CenterPoint = new PointF(bounds.Left, bounds.Top);
				pathGradientBrush.RotateTransform(angle, MatrixOrder.Append);
				pathGradientBrush.TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				return pathGradientBrush;
			}
		}

		private Brush GetFillBrush(GaugeGraphics g, GraphicsPath path, PointF pointOrigin, float angle, Color fillColor, GradientType fillGradientType, Color fillGradientEndColor, GaugeHatchStyle fillHatchStyle)
		{
			Brush brush = null;
			if (fillHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(fillHatchStyle, fillColor, fillGradientEndColor);
			}
			else if (fillGradientType != 0)
			{
				RectangleF bounds = path.GetBounds();
				switch (fillGradientType)
				{
				case GradientType.DiagonalLeft:
					brush = g.GetGradientBrush(bounds, fillColor, fillGradientEndColor, GradientType.LeftRight);
					using (Matrix matrix2 = new Matrix())
					{
						matrix2.RotateAt(45f, new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0)));
						((LinearGradientBrush)brush).Transform = matrix2;
					}
					break;
				case GradientType.DiagonalRight:
					brush = g.GetGradientBrush(bounds, fillColor, fillGradientEndColor, GradientType.TopBottom);
					using (Matrix matrix = new Matrix())
					{
						matrix.RotateAt(135f, new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0)));
						((LinearGradientBrush)brush).Transform = matrix;
					}
					break;
				case GradientType.Center:
					bounds.Inflate(1f, 1f);
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddArc(bounds, 0f, 360f);
						PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
						pathGradientBrush.CenterColor = fillColor;
						pathGradientBrush.CenterPoint = new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0));
						pathGradientBrush.SurroundColors = new Color[1]
						{
							fillGradientEndColor
						};
						brush = pathGradientBrush;
					}
					break;
				default:
					brush = g.GetGradientBrush(path.GetBounds(), fillColor, fillGradientEndColor, fillGradientType);
					break;
				}
				if (brush is LinearGradientBrush)
				{
					((LinearGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
					((LinearGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
				else if (brush is PathGradientBrush)
				{
					((PathGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
					((PathGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
			}
			else
			{
				brush = new SolidBrush(fillColor);
			}
			return brush;
		}

		internal override void Render(GaugeGraphics g)
		{
			if (this.Common != null && this.Visible && this.GetScale() != null)
			{
				g.StartHotRegion(this);
				this.GetScale().SetDrawRegion(g);
				if (this.Image != "" && this.CapImage != "")
				{
					this.DrawImage(g, true, false);
					this.DrawImage(g, false, false);
					this.SetAllHotRegions(g);
					g.RestoreDrawRegion();
					g.EndHotRegion();
				}
				else
				{
					if (this.Image != "")
					{
						this.DrawImage(g, true, false);
					}
					float positionFromValue = this.GetScale().GetPositionFromValue(base.Position);
					PointF absolutePoint = g.GetAbsolutePoint(this.GetScale().GetPivotPoint());
					Pen pen = new Pen(base.BorderColor, (float)base.BorderWidth);
					pen.DashStyle = g.GetPenStyle(base.BorderStyle);
					if (pen.DashStyle != 0)
					{
						pen.Alignment = PenAlignment.Center;
					}
					KnobStyleAttrib knobStyleAttrib = this.GetKnobStyleAttrib(g, absolutePoint, positionFromValue);
					try
					{
						if (knobStyleAttrib.paths != null)
						{
							for (int i = 0; i < knobStyleAttrib.paths.Length; i++)
							{
								if (knobStyleAttrib.brushes[i] != null && knobStyleAttrib.paths[i] != null)
								{
									g.FillPath(knobStyleAttrib.brushes[i], knobStyleAttrib.paths[i]);
								}
							}
							if (base.BorderWidth > 0 && knobStyleAttrib.paths[0] != null)
							{
								g.DrawPath(pen, knobStyleAttrib.paths[0]);
							}
							if (knobStyleAttrib.paths[0] != null)
							{
								this.AddHotRegion((GraphicsPath)knobStyleAttrib.paths[0].Clone(), true);
							}
						}
					}
					finally
					{
						knobStyleAttrib.Dispose();
					}
					if (this.CapImage != "")
					{
						this.DrawImage(g, false, false);
					}
					this.SetAllHotRegions(g);
					g.RestoreDrawRegion();
					g.EndHotRegion();
				}
			}
		}

		internal void DrawImage(GaugeGraphics g, bool primary, bool drawShadow)
		{
			if (this.Visible)
			{
				if (drawShadow && base.ShadowOffset == 0.0)
				{
					return;
				}
				float width = this.Width;
				width = g.GetAbsoluteDimension(width);
				Image image = null;
				image = ((!primary) ? this.Common.ImageLoader.LoadImage(this.CapImage) : this.Common.ImageLoader.LoadImage(this.Image));
				if (image.Width != 0 && image.Height != 0)
				{
					Point point = Point.Empty;
					point = ((!primary) ? this.CapImageOrigin : this.ImageOrigin);
					if (point.IsEmpty)
					{
						point.X = image.Width / 2;
						point.Y = image.Height / 2;
					}
					int num = (image.Height <= image.Width) ? image.Width : image.Height;
					if (num != 0)
					{
						float num2 = (!primary) ? (g.GetAbsoluteDimension((float)(this.CapWidth * 2.0)) / (float)num) : (g.GetAbsoluteDimension((float)(this.Width * 2.0)) / (float)num);
						Rectangle rectangle = new Rectangle(0, 0, (int)((float)image.Width * num2), (int)((float)image.Height * num2));
						ImageAttributes imageAttributes = new ImageAttributes();
						if (primary && this.ImageTransColor != Color.Empty)
						{
							imageAttributes.SetColorKey(this.ImageTransColor, this.ImageTransColor, ColorAdjustType.Default);
						}
						if (!primary && this.CapImageTransColor != Color.Empty)
						{
							imageAttributes.SetColorKey(this.CapImageTransColor, this.CapImageTransColor, ColorAdjustType.Default);
						}
						Matrix transform = g.Transform;
						Matrix matrix = g.Transform.Clone();
						float positionFromValue = this.GetScale().GetPositionFromValue(base.Position);
						PointF absolutePoint = g.GetAbsolutePoint(this.GetScale().GetPivotPoint());
						PointF pointF = new PointF((float)point.X * num2, (float)point.Y * num2);
						float offsetX = matrix.OffsetX;
						float offsetY = matrix.OffsetY;
						matrix.Translate(absolutePoint.X - pointF.X, absolutePoint.Y - pointF.Y, MatrixOrder.Append);
						absolutePoint.X += offsetX;
						absolutePoint.Y += offsetY;
						matrix.RotateAt(positionFromValue, absolutePoint, MatrixOrder.Append);
						if (drawShadow)
						{
							ColorMatrix colorMatrix = new ColorMatrix();
							colorMatrix.Matrix00 = 0f;
							colorMatrix.Matrix11 = 0f;
							colorMatrix.Matrix22 = 0f;
							colorMatrix.Matrix33 = (float)(this.Common.GaugeCore.ShadowIntensity / 100.0);
							imageAttributes.SetColorMatrix(colorMatrix);
							matrix.Translate(base.ShadowOffset, base.ShadowOffset, MatrixOrder.Append);
						}
						else if (primary && !this.ImageHueColor.IsEmpty)
						{
							Color color = g.TransformHueColor(this.ImageHueColor);
							ColorMatrix colorMatrix2 = new ColorMatrix();
							colorMatrix2.Matrix00 = (float)((float)(int)color.R / 255.0);
							colorMatrix2.Matrix11 = (float)((float)(int)color.G / 255.0);
							colorMatrix2.Matrix22 = (float)((float)(int)color.B / 255.0);
							imageAttributes.SetColorMatrix(colorMatrix2);
						}
						else if (!primary && !this.CapImageHueColor.IsEmpty)
						{
							Color color2 = g.TransformHueColor(this.CapImageHueColor);
							ColorMatrix colorMatrix3 = new ColorMatrix();
							colorMatrix3.Matrix00 = (float)((float)(int)color2.R / 255.0);
							colorMatrix3.Matrix11 = (float)((float)(int)color2.G / 255.0);
							colorMatrix3.Matrix22 = (float)((float)(int)color2.B / 255.0);
							imageAttributes.SetColorMatrix(colorMatrix3);
						}
						g.Transform = matrix;
						ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
						imageSmoothingState.Set();
						g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
						imageSmoothingState.Restore();
						g.Transform = transform;
						if (!drawShadow)
						{
							matrix.Translate((float)(0.0 - offsetX), (float)(0.0 - offsetY), MatrixOrder.Append);
							GraphicsPath graphicsPath = new GraphicsPath();
							graphicsPath.AddRectangle(rectangle);
							graphicsPath.Transform(matrix);
							this.AddHotRegion(graphicsPath, primary);
						}
					}
				}
			}
		}

		internal GraphicsPath GetPointerPath(GaugeGraphics g)
		{
			if (!this.Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			float positionFromValue = this.GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(this.GetScale().GetPivotPoint());
			KnobStyleAttrib knobStyleAttrib = this.GetKnobStyleAttrib(g, absolutePoint, positionFromValue);
			if (knobStyleAttrib.paths != null && knobStyleAttrib.paths[0] != null)
			{
				graphicsPath.AddPath(knobStyleAttrib.paths[0], false);
			}
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			if (base.ShadowOffset != 0.0 && this.GetScale() != null)
			{
				this.GetScale().SetDrawRegion(g);
				if (this.Image != "")
				{
					this.DrawImage(g, true, true);
				}
				if (this.CapImage != "")
				{
					this.DrawImage(g, false, true);
				}
				GraphicsPath pointerPath = this.GetPointerPath(g);
				if (pointerPath != null && pointerPath.PointCount != 0)
				{
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(base.ShadowOffset, base.ShadowOffset);
						pointerPath.Transform(matrix);
					}
					PointF pointF = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
					g.RestoreDrawRegion();
					PointF pointF2 = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
					using (Matrix matrix2 = new Matrix())
					{
						matrix2.Translate(pointF.X - pointF2.X, pointF.Y - pointF2.Y);
						pointerPath.Transform(matrix2);
						return pointerPath;
					}
				}
				g.RestoreDrawRegion();
				return null;
			}
			return null;
		}

		internal void AddHotRegion(GraphicsPath path, bool primary)
		{
			if (primary)
			{
				this.hotRegions[0] = path;
			}
			else
			{
				this.hotRegions[1] = path;
			}
		}

		internal void SetAllHotRegions(GaugeGraphics g)
		{
			this.Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(this.GetScale().GetPivotPoint()), this.hotRegions[0], this.hotRegions[1]);
			this.hotRegions[0] = null;
			this.hotRegions[1] = null;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public CircularGauge GetGauge()
		{
			if (this.Collection == null)
			{
				return null;
			}
			return (CircularGauge)this.Collection.parent;
		}

		public CircularScale GetScale()
		{
			if (this.GetGauge() == null)
			{
				return null;
			}
			if (this.ScaleName == string.Empty)
			{
				return null;
			}
			if (this.ScaleName == "Default" && this.GetGauge().Scales.Count == 0)
			{
				return null;
			}
			return this.GetGauge().Scales[this.ScaleName];
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			if (this.GetGauge() != null && this.GetScale() != null)
			{
				Stack stack = new Stack();
				for (NamedElement namedElement = this.GetGauge().ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
				{
					stack.Push(namedElement);
				}
				foreach (IRenderable item in stack)
				{
					g.CreateDrawRegion(item.GetBoundRect(g));
				}
				g.CreateDrawRegion(((IRenderable)this.GetGauge()).GetBoundRect(g));
				this.GetScale().SetDrawRegion(g);
				using (GraphicsPath graphicsPath = this.GetPointerPath(g))
				{
					if (graphicsPath != null)
					{
						RectangleF bounds = graphicsPath.GetBounds();
						g.DrawSelection(bounds, designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
					}
				}
				g.RestoreDrawRegion();
				g.RestoreDrawRegion();
				foreach (IRenderable item2 in stack)
				{
					IRenderable renderable = item2;
					g.RestoreDrawRegion();
				}
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			Knob knob = new Knob();
			binaryFormatSerializer.Deserialize(knob, stream);
			return knob;
		}
	}
}
