using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueLinearConverter))]
	internal class CalculatedValueLinear : CalculatedValue
	{
		private double multiplier = 1.0;

		private double addend;

		[SRDescription("DescriptionAttributeCalculatedValueLinear_Multiplier")]
		[DefaultValue(1.0)]
		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		public double Multiplier
		{
			get
			{
				return this.multiplier;
			}
			set
			{
				this.multiplier = value;
			}
		}

		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeCalculatedValueLinear_Adder")]
		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		public double AddConstant
		{
			get
			{
				return this.addend;
			}
			set
			{
				this.addend = value;
			}
		}

		[Browsable(false)]
		public override long Period
		{
			get
			{
				return base.Period;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public override DurationType PeriodType
		{
			get
			{
				return base.PeriodType;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public override double RefreshRate
		{
			get
			{
				return base.RefreshRate;
			}
			set
			{
			}
		}

		[Browsable(false)]
		public override PeriodType RefreshRateType
		{
			get
			{
				return base.RefreshRateType;
			}
			set
			{
			}
		}

		internal override void CalculateValue(double value, DateTime timestamp)
		{
			base.noMoreData = true;
			value = base.inputValue * this.multiplier + this.addend;
			base.CalculateValue(value, timestamp);
		}
	}
}
