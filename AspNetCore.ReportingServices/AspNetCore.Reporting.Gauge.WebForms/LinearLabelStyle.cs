using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LinearLabelStyle : GaugeObject
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
		[SRDescription("DescriptionAttributeVisible6")]
		[SRCategory("CategoryAppearance")]
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

		[DefaultValue(Placement.Inside)]
		[SRDescription("DescriptionAttributePlacement5")]
		[SRCategory("CategoryAppearance")]
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
		[SRDescription("DescriptionAttributeFont3")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 14pt")]
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
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontUnit")]
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

		[ValidateBound(0.0, 360.0)]
		[DefaultValue(0f)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontAngle3")]
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTextColor5")]
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
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeInterval")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionIntervalNegative"));
				}
				if (value == 0.0)
				{
					value = double.NaN;
				}
				this.interval = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeIntervalOffset3")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		[SRCategory("CategoryBehavior")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionIntervalOffsetNegative"));
				}
				this.intervalOffset = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeShowEndLabels")]
		[DefaultValue(true)]
		[SRCategory("CategoryAppearance")]
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

		[ValidateBound(-30.0, 30.0)]
		[DefaultValue(2f)]
		[SRDescription("DescriptionAttributeDistanceFromScale5")]
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

		[DefaultValue("")]
		[Localizable(true)]
		[SRCategory("CategoryAppearance")]
		[Editor(typeof(LabelFormatEditor), typeof(UITypeEditor))]
		[SRDescription("DescriptionAttributeFormatString3")]
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

		protected LinearLabelStyle(object parent, Font font)
			: base(parent)
		{
			this.font = font;
		}

		internal string GetFormatStr()
		{
			if (this.formatStr == string.Empty)
			{
				if (this.formatString.Length > 0)
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
