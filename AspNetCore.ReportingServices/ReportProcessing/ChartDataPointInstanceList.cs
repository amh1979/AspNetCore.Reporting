using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointInstanceList : ArrayList
	{
		internal new ChartDataPointInstance this[int index]
		{
			get
			{
				return (ChartDataPointInstance)base[index];
			}
		}

		internal ChartDataPointInstanceList()
		{
		}

		internal ChartDataPointInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
