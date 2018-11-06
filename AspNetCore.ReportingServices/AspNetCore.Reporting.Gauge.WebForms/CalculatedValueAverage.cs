using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CalculatedValueAverageConverter))]
	internal class CalculatedValueAverage : CalculatedValue
	{
		internal override void CalculateValue(double value, DateTime timestamp)
		{
			GaugeDuration aggregateDuration = base.aggregateDuration;
			if (aggregateDuration.IsEmpty)
			{
				base.CalculateValue(value, timestamp);
			}
			else
			{
				base.noMoreData = false;
				double num = value;
				if (((IValueConsumer)this).GetProvider() != null)
				{
					HistoryCollection data = ((IValueConsumer)this).GetProvider().GetData(aggregateDuration, timestamp);
					HistoryEntry[] array = data.Select(aggregateDuration, timestamp);
					if (array.Length > 1)
					{
						num = 0.0;
						long num2 = 0L;
						long num3 = 0L;
						for (int i = 0; i < array.Length - 1; i++)
						{
							num2 = array[i + 1].Timestamp.Ticks - array[i].Timestamp.Ticks;
							if (num2 == 0)
							{
								num += (array[i + 1].Value - array[i].Value) / 2.0;
							}
							else
							{
								num += array[i + 1].Value * (double)num2;
								num3 += num2;
							}
						}
						if (num3 > 0)
						{
							num /= (double)num3;
						}
					}
					else if (array.Length > 0)
					{
						num = array[0].Value;
					}
					else
					{
						base.noMoreData = true;
					}
				}
				base.CalculateValue(num, timestamp);
			}
		}
	}
}
