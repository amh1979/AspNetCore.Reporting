using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class CustomTickMark : MapObject
	{
		private bool visible = true;

		private Placement placement = Placement.Cross;

		private Color borderColor = Color.DarkGray;

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

		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeCustomTickMark_Visible")]
		[NotifyParentProperty(true)]
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

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeCustomTickMark_Placement")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(Placement.Cross)]
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

		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeCustomTickMark_BorderColor")]
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
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeCustomTickMark_BorderWidth")]
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
				throw new ArgumentException(SR.must_in_range(0.0, 25.0));
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "WhiteSmoke")]
		[SRDescription("DescriptionAttributeCustomTickMark_FillColor")]
		[NotifyParentProperty(true)]
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

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeCustomTickMark_EnableGradient")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(true)]
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

		[DefaultValue(30f)]
		[SRDescription("DescriptionAttributeCustomTickMark_GradientDensity")]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
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
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[SRDescription("DescriptionAttributeCustomTickMark_DistanceFromScale")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(0f)]
		[NotifyParentProperty(true)]
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
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_Shape")]
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeCustomTickMark_Length")]
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
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[SRDescription("DescriptionAttributeCustomTickMark_Width")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(3f)]
		[NotifyParentProperty(true)]
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
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeCustomTickMark_Image")]
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

		[SRDescription("DescriptionAttributeCustomTickMark_ImageTransColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttribute_Appearance")]
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
			this.Shape = shape;
			this.length = length;
			this.width = width;
		}
	}
}
