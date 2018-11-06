using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueRateOfChangeConverter))]
	internal class CalculatedValueRateOfChange : CalculatedValue
	{
		private DataSampleRC[] oldValues = new DataSampleRC[2]
		{
			new DataSampleRC(),
			new DataSampleRC()
		};

		private GaugePeriod rateOfChange = new GaugePeriod(double.NaN, AspNetCore.Reporting.Gauge.WebForms.PeriodType.Seconds);

		[SRCategory("CategoryBehavior")]
		[Bindable(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeCalculatedValueRateOfChange_RateOfChangePeriod")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public virtual double RateOfChangePeriod
		{
			get
			{
				return this.rateOfChange.Duration;
			}
			set
			{
				this.rateOfChange.Duration = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryBehavior")]
		[NotifyParentProperty(true)]
		[DefaultValue(AspNetCore.Reporting.Gauge.WebForms.PeriodType.Seconds)]
		[SRDescription("DescriptionAttributeCalculatedValueRateOfChange_RateOfChangePeriodType")]
		public virtual PeriodType RateOfChangePeriodType
		{
			get
			{
				return this.rateOfChange.PeriodType;
			}
			set
			{
				this.rateOfChange.PeriodType = value;
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

		internal override void CalculateValue(double value, DateTime timestamp)
		{
			TimeSpan period = this.rateOfChange.ToTimeSpan();
			double num = double.NaN;
			base.noMoreData = true;
			if (double.IsNaN(value))
			{
				base.CalculateValue(num, timestamp);
			}
			else if (period.Ticks == 0)
			{
				base.CalculateValue(num, timestamp);
			}
			else
			{
				if (!this.oldValues[0].Invalid)
				{
					if (this.oldValues[0].Timestamp == timestamp)
					{
						if (!this.oldValues[1].Invalid)
						{
							num = this.GetResult(value, timestamp, this.oldValues[1], period);
						}
						this.oldValues[0].Value = value;
					}
					else
					{
						num = this.GetResult(value, timestamp, this.oldValues[0], period);
						this.oldValues[1].Assign(this.oldValues[0]);
						this.oldValues[0].Value = value;
						this.oldValues[0].Timestamp = timestamp;
					}
				}
				else
				{
					this.oldValues[0].Value = value;
					this.oldValues[0].Timestamp = timestamp;
				}
				base.noMoreData = (num == 0.0);
				base.CalculateValue(num, timestamp);
			}
		}

		private double GetResult(double value, DateTime timestamp, DataSampleRC rc, TimeSpan period)
		{
			double result = 0.0;
			TimeSpan timeSpan = timestamp - rc.Timestamp;
			double num = value - rc.Value;
			double num2 = (double)period.Ticks / (double)timeSpan.Ticks;
			if (num2 != 0.0)
			{
				result = num * num2;
			}
			else if (num > 0.0)
			{
				result = double.PositiveInfinity;
			}
			else if (num < 0.0)
			{
				result = double.NegativeInfinity;
			}
			return result;
		}

		internal override object CloneInternals(object copy)
		{
			CalculatedValueRateOfChange calculatedValueRateOfChange = (CalculatedValueRateOfChange)base.CloneInternals(copy);
			calculatedValueRateOfChange.rateOfChange = this.rateOfChange.Clone();
			return calculatedValueRateOfChange;
		}
	}
}
