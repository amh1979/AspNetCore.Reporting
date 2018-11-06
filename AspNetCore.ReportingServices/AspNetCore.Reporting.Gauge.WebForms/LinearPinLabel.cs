using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LinearPinLabel : GaugeObject
	{
		private string text = "";

		private Font font = new Font("Microsoft Sans Serif", 12f);

		private FontUnit fontUnit;

		private Color textColor = Color.Black;

		private Placement placement;

		private float fontAngle;

		private float scaleOffset = 2f;

		[DefaultValue("")]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeText5")]
		[Localizable(true)]
		[NotifyParentProperty(true)]
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

		[SRDescription("DescriptionAttributeFont4")]
		[SRCategory("CategoryAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 12pt")]
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

		[SRDescription("DescriptionAttributeTextColor")]
		[SRCategory("CategoryAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[NotifyParentProperty(true)]
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

		[DefaultValue(Placement.Inside)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributePlacement3")]
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

		[DefaultValue(0f)]
		[ValidateBound(0.0, 360.0)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFontAngle")]
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

		[ValidateBound(-30.0, 30.0)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeDistanceFromScale6")]
		[DefaultValue(2f)]
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

		public LinearPinLabel()
			: this(null)
		{
		}

		public LinearPinLabel(object parent)
			: base(parent)
		{
		}
	}
}
