using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CustomTickMark : GaugeObject
	{
		private bool visible = true;

		private Placement placement = Placement.Cross;

		private Color borderColor = Color.DimGray;

		private GaugeDashStyle borderStyle = GaugeDashStyle.Solid;

		private int borderWidth = 1;

		private Color fillColor = Color.WhiteSmoke;

		private bool enableGradient = true;

		private float gradientDensity = 30f;

		private float offset;

		private MarkerStyle shape = MarkerStyle.Trapezoid;

		private float length = 5f;

		private float width = 3f;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Color imageHueColor = Color.Empty;

		[SRDescription("DescriptionAttributeVisible12")]
		[DefaultValue(true)]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public bool Visible
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

		[DefaultValue(Placement.Cross)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributePlacement6")]
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

		[SRDescription("DescriptionAttributeBorderColor3")]
		[DefaultValue(typeof(Color), "DimGray")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderStyle3")]
		[DefaultValue(GaugeDashStyle.Solid)]
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

		[DefaultValue(1)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderWidth")]
		[NotifyParentProperty(true)]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value >= 0 && value <= 25)
				{
					this.borderWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 25));
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "WhiteSmoke")]
		[SRDescription("DescriptionAttributeFillColor4")]
		[SRCategory("CategoryAppearance")]
		public Color FillColor
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeEnableGradient")]
		[NotifyParentProperty(true)]
		public bool EnableGradient
		{
			get
			{
				return this.enableGradient;
			}
			set
			{
				this.enableGradient = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeGradientDensity")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(30f)]
		[NotifyParentProperty(true)]
		[ValidateBound(0.0, 100.0)]
		public float GradientDensity
		{
			get
			{
				return this.gradientDensity;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.gradientDensity = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[NotifyParentProperty(true)]
		[ValidateBound(-30.0, 30.0)]
		[DefaultValue(0f)]
		[SRDescription("DescriptionAttributeDistanceFromScale4")]
		[SRCategory("CategoryAppearance")]
		public float DistanceFromScale
		{
			get
			{
				return this.offset;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.offset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeShape3")]
		[SRCategory("CategoryAppearance")]
		public virtual MarkerStyle Shape
		{
			get
			{
				return this.shape;
			}
			set
			{
				this.shape = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLength")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		public virtual float Length
		{
			get
			{
				return this.length;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.length = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[DefaultValue(3f)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeWidth7")]
		public virtual float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.width = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRDescription("DescriptionAttributeImage5")]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[SRCategory("CategoryAppearance")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeImageTransColor4")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
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

		[SRDescription("DescriptionAttributeImageHueColor6")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAppearance")]
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

		public CustomTickMark()
			: this(null)
		{
		}

		public CustomTickMark(object parent)
			: base(parent)
		{
		}

		public CustomTickMark(object parent, MarkerStyle shape, float length, float width)
			: this(parent)
		{
			this.shape = shape;
			this.length = length;
			this.width = width;
		}
	}
}
