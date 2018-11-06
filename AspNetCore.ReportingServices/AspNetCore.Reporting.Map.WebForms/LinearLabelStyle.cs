using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LinearLabelStyle : MapObject
	{
		private string formatStr = string.Empty;

		private bool visible = true;

		private Placement placement;

		private Font font = new Font("Microsoft Sans Serif", 14f);

		private FontUnit fontUnit;

		private float fontAngle;

		private Color textColor = Color.Black;

		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		private bool showEndLabels = true;

		private float scaleOffset = 2f;

		private string formatString = string.Empty;

		[DefaultValue(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_Visible")]
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

		[SRDescription("DescriptionAttributeLinearLabelStyle_Placement")]
		[SRCategory("CategoryAttribute_Appearance")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 14pt")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_Font")]
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

		[DefaultValue(FontUnit.Percent)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_FontUnit")]
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

		[SRDescription("DescriptionAttributeLinearLabelStyle_FontAngle")]
		[DefaultValue(0f)]
		[SRCategory("CategoryAttribute_Appearance")]
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
				throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 360.0));
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_TextColor")]
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

		[DefaultValue(double.NaN)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_Interval")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.interval_negative);
				}
				if (value == 0.0)
				{
					value = double.NaN;
				}
				this.interval = value;
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_IntervalOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		public double IntervalOffset
		{
			get
			{
				return this.intervalOffset;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.interval_offset_negative);
				}
				this.intervalOffset = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLinearLabelStyle_ShowEndLabels")]
		public bool ShowEndLabels
		{
			get
			{
				return this.showEndLabels;
			}
			set
			{
				this.showEndLabels = value;
				this.Invalidate();
			}
		}

		[DefaultValue(2f)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_DistanceFromScale")]
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
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		[Localizable(true)]
		//[Editor(typeof(LabelFormatEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearLabelStyle_FormatString")]
		[DefaultValue("")]
		public string FormatString
		{
			get
			{
				return this.formatString;
			}
			set
			{
				this.formatString = value;
				this.formatStr = string.Empty;
				this.Invalidate();
			}
		}

		public LinearLabelStyle()
			: this(null)
		{
		}

		public LinearLabelStyle(object parent)
			: base(parent)
		{
		}

		internal string GetFormatStr()
		{
			if (string.IsNullOrEmpty(this.formatStr))
			{
				if (!string.IsNullOrEmpty(this.formatString))
				{
					this.formatStr = "{0:" + this.formatString + "}";
				}
				else
				{
					this.formatStr = "{0}";
				}
			}
			return this.formatStr;
		}
	}
}
