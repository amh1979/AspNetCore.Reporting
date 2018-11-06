using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointList : ArrayList
	{
		internal new ChartDataPoint this[int index]
		{
			get
			{
				return (ChartDataPoint)base[index];
			}
		}

		internal ChartDataPointList()
		{
		}

		internal ChartDataPointList(int capacity)
			: base(capacity)
		{
		}
	}
}
