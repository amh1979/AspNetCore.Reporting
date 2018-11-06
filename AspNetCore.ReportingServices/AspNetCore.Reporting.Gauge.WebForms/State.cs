using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(StateConverter))]
	internal class State : Range
	{
		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.Red;

		private GradientType fillGradientType = GradientType.Center;

		private Color fillGradientEndColor = Color.DarkRed;

		private GaugeHatchStyle fillHatchStyle;

		private string text = "Text";

		private StateIndicatorStyle style = StateIndicatorStyle.CircularLed;

		private float scaleFactor = 1f;

		private string image = "";

		private Color imageTransColor = Color.Empty;

		private Color imageHueColor = Color.Empty;

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeState_Name")]
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

		[SRCategory("CategoryLayout")]
		[Browsable(false)]
		[DefaultValue(70.0)]
		[SRDescription("DescriptionAttributeState_EndValue")]
		public override double StartValue
		{
			get
			{
				return base.StartValue;
			}
			set
			{
				base.StartValue = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[DefaultValue(100.0)]
		[SRDescription("DescriptionAttributeState_EndValue")]
		public override double EndValue
		{
			get
			{
				return base.EndValue;
			}
			set
			{
				base.EndValue = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeState_TriggerDelay")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(0.0)]
		internal double TriggerDelay
		{
			get
			{
				return base.InRangeTimeout;
			}
			set
			{
				base.InRangeTimeout = value;
			}
		}

		[SRDescription("DescriptionAttributeState_TriggerDelayType")]
		[DefaultValue(PeriodType.Seconds)]
		[SRCategory("CategoryBehavior")]
		internal PeriodType TriggerDelayType
		{
			get
			{
				return base.InRangeTimeoutType;
			}
			set
			{
				base.InRangeTimeoutType = value;
			}
		}

		[SRDescription("DescriptionAttributeState_Font")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
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

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_BorderColor")]
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
		[DefaultValue(GaugeDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeState_BorderStyle")]
		[NotifyParentProperty(true)]
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

		[SRDescription("DescriptionAttributeState_BorderWidth")]
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
		[DefaultValue(typeof(Color), "Red")]
		[SRDescription("DescriptionAttributeState_FillColor")]
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue(GradientType.Center)]
		[SRDescription("DescriptionAttributeState_FillGradientType")]
		[NotifyParentProperty(true)]
		public GradientType FillGradientType
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

		[DefaultValue(typeof(Color), "DarkRed")]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientEndColor5")]
		[NotifyParentProperty(true)]
		public Color FillGradientEndColor
		{
			get
			{
				return this.fillGradientEndColor;
			}
			set
			{
				this.fillGradientEndColor = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeState_FillHatchStyle")]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle FillHatchStyle
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

		[Localizable(true)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeState_Text")]
		[DefaultValue("Text")]
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

		[SRDescription("DescriptionAttributeStateIndicator_IndicatorStyle")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(StateIndicatorStyle.CircularLed)]
		[ParenthesizePropertyName(true)]
		public StateIndicatorStyle IndicatorStyle
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

		[SRCategory("CategoryPosition")]
		[ValidateBound(0.0, 1.0)]
		[SRDescription("DescriptionAttributeStateIndicator_ScaleFactor")]
		[DefaultValue(1f)]
		public float ScaleFactor
		{
			get
			{
				return this.scaleFactor;
			}
			set
			{
				if (!(value > 1.0) && !(value < 0.0))
				{
					this.scaleFactor = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 1));
			}
		}

		[SRDescription("DescriptionAttributeState_Image")]
		[SRCategory("CategoryImage")]
		[DefaultValue("")]
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
		[SRDescription("DescriptionAttributeImageTransColor6")]
		[SRCategory("CategoryImage")]
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

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeImageHueColor4")]
		[DefaultValue(typeof(Color), "")]
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

		public override string ToString()
		{
			return this.Name;
		}

		internal override void OnAdded()
		{
			StateIndicator stateIndicator = (StateIndicator)this.ParentElement;
			((IValueConsumer)stateIndicator.Data).Refresh();
			base.OnAdded();
		}

		internal override void OnValueRangeTimeOut(object sender, ValueRangeEventArgs e)
		{
			base.OnValueRangeTimeOut(sender, e);
			StateIndicator stateIndicator = (StateIndicator)this.ParentElement;
			stateIndicator.Refresh();
		}
	}
}
