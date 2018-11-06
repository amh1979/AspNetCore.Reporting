using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class TickMark : CustomTickMark
	{
		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeTickMark_Interval")]
		[SRCategory("CategoryAttribute_Behavior")]
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

		[SRDescription("DescriptionAttributeTickMark_IntervalOffset")]
		[SRCategory("CategoryAttribute_Behavior")]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
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
					throw new ArgumentException(SR.interval_offset_negative);
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
