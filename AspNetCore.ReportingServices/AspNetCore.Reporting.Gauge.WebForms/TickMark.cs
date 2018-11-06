using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class TickMark : CustomTickMark
	{
		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeInterval3")]
		[SRCategory("CategoryBehavior")]
		public virtual double Interval
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

		[SRDescription("DescriptionAttributeIntervalOffset")]
		[DefaultValue(double.NaN)]
		[SRCategory("CategoryBehavior")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		public virtual double IntervalOffset
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

		public TickMark()
			: this(null)
		{
		}

		public TickMark(object parent)
			: base(parent)
		{
		}

		public TickMark(object parent, MarkerStyle shape, float length, float width)
			: base(parent, shape, length, width)
		{
		}
	}
}
