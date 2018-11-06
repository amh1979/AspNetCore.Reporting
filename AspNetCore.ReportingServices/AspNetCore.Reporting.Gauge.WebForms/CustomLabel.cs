using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CustomLabelConverter))]
	internal class CustomLabel : NamedElement
	{
		private double labelValue;

		private CustomTickMark tickMarkStyle;

		private string text = "";

		private Font font = new Font("Microsoft Sans Serif", 14f);

		private FontUnit fontUnit;

		private Color textColor = Color.Black;

		private bool visible = true;

		private Placement placement;

		private bool rotateLabels;

		private bool allowUpsideDown;

		private float fontAngle;

		private float scaleOffset;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeCustomLabel_Name")]
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

		[DefaultValue(0.0)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeCustomLabel_Value")]
		public double Value
		{
			get
			{
				return this.labelValue;
			}
			set
			{
				this.labelValue = value;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(typeof(CustomTickMark), "MarkerStyle.Trapezoid, 10F, 6F")]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeCustomLabel_TickMarkStyle")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public CustomTickMark TickMarkStyle
		{
			get
			{
				if (this.tickMarkStyle == null)
				{
					this.tickMarkStyle = new CustomTickMark(this, MarkerStyle.Trapezoid, 10f, 6f);
				}
				return this.tickMarkStyle;
			}
			set
			{
				this.tickMarkStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeText5")]
		[Localizable(true)]
		public string Text
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

		[SRDescription("DescriptionAttributeCustomLabel_Font")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 14pt")]
		public Font Font
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

		[SRDescription("DescriptionAttributeFontUnit")]
		[DefaultValue(FontUnit.Percent)]
		[SRCategory("CategoryAppearance")]
		public FontUnit FontUnit
		{
			get
			{
				return this.fontUnit;
			}
			set
			{
				this.fontUnit = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_TextColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color TextColor
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
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

		[SRDescription("DescriptionAttributeCustomLabel_Placement")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(Placement.Inside)]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_RotateLabel")]
		[DefaultValue(false)]
		public bool RotateLabel
		{
			get
			{
				return this.rotateLabels;
			}
			set
			{
				this.rotateLabels = value;
				this.Invalidate();
			}
		}

		[DefaultValue(false)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_AllowUpsideDown")]
		public bool AllowUpsideDown
		{
			get
			{
				return this.allowUpsideDown;
			}
			set
			{
				this.allowUpsideDown = value;
				this.Invalidate();
			}
		}

		[ValidateBound(0.0, 360.0)]
		[DefaultValue(0f)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_FontAngle")]
		public float FontAngle
		{
			get
			{
				return this.fontAngle;
			}
			set
			{
				if (!(value > 360.0) && !(value < 0.0))
				{
					this.fontAngle = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
			}
		}

		[SRDescription("DescriptionAttributeCustomLabel_DistanceFromScale")]
		[DefaultValue(0f)]
		[ValidateBound(-30.0, 30.0)]
		[SRCategory("CategoryAppearance")]
		public float DistanceFromScale
		{
			get
			{
				return this.scaleOffset;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.scaleOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		public override string ToString()
		{
			return this.Name;
		}
	}
}
