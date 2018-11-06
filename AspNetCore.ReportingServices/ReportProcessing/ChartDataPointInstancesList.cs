using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointInstancesList : ArrayList
	{
		internal new ChartDataPointInstanceList this[int index]
		{
			get
			{
				return (ChartDataPointInstanceList)base[index];
			}
		}

		internal ChartDataPointInstancesList()
		{
		}

		internal ChartDataPointInstancesList(int capacity)
			: base(capacity)
		{
		}
	}
}
