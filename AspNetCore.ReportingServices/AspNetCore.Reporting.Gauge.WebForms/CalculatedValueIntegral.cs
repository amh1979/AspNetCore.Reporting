using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueIntegralConverter))]
	internal class CalculatedValueIntegral : CalculatedValue
	{
		private GaugePeriod interval = new GaugePeriod(double.NaN, AspNetCore.Reporting.Gauge.WebForms.PeriodType.Seconds);

		private double integralBase;

		private double integralResult;

		private DataSampleRC oldValue = new DataSampleRC();

		[SRCategory("CategoryBehavior")]
		[DefaultValue(double.NaN)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCalculatedValueIntegral_IntegralInterval")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public double IntegralInterval
		{
			get
			{
				return this.interval.Duration;
			}
			set
			{
				this.interval.Duration = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(AspNetCore.Reporting.Gauge.WebForms.PeriodType.Seconds)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeCalculatedValueIntegral_IntegralIntervalType")]
		public PeriodType IntegralIntervalType
		{
			get
			{
				return this.interval.PeriodType;
			}
			set
			{
				this.interval.PeriodType = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeCalculatedValueIntegral_IntegralBase")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(0.0)]
		public double IntegralBase
		{
			get
			{
				return this.integralBase;
			}
			set
			{
				this.integralBase = value;
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
			TimeSpan timeSpan = this.interval.ToTimeSpan();
			base.noMoreData = false;
			if (double.IsNaN(value))
			{
				base.CalculateValue(this.integralBase + this.integralResult, timestamp);
			}
			else if (timeSpan.Ticks == 0)
			{
				this.integralResult += value;
				base.CalculateValue(this.integralBase + this.integralResult, timestamp);
			}
			else
			{
				if (!this.oldValue.Invalid)
				{
					this.integralResult += value * ((double)(timestamp.Ticks - this.oldValue.Timestamp.Ticks) / (double)timeSpan.Ticks);
				}
				this.oldValue.Timestamp = timestamp;
				this.oldValue.Value = value;
				base.CalculateValue(this.integralBase + this.integralResult, timestamp);
			}
		}

		private void RegenerateIntegralResult()
		{
			if (((IValueConsumer)this).GetProvider() is ValueBase)
			{
				TimeSpan timeSpan = this.interval.ToTimeSpan();
				ValueBase valueBase = (ValueBase)((IValueConsumer)this).GetProvider();
				HistoryCollection history = valueBase.History;
				this.integralResult = history.AccumulatedValue / (double)timeSpan.Ticks;
				int num = history.Locate(valueBase.Date);
				for (int i = 1; i < num; i++)
				{
					this.integralResult += history[i].Value * (double)(history[i].Timestamp.Ticks - history[i - 1].Timestamp.Ticks) / (double)timeSpan.Ticks;
					this.oldValue.Timestamp = history[i].Timestamp;
					this.oldValue.Value = history[i].Value;
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.integralResult = 0.0;
			this.oldValue = new DataSampleRC();
		}

		internal override void RefreshConsumers()
		{
			this.RegenerateIntegralResult();
			base.RefreshConsumers();
		}

		internal override object CloneInternals(object copy)
		{
			copy = base.CloneInternals(copy);
			((CalculatedValueIntegral)copy).interval = this.interval.Clone();
			((CalculatedValueIntegral)copy).oldValue = (DataSampleRC)this.oldValue.Clone();
			return copy;
		}
	}
}
