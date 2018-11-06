using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MultiChartInstanceList : ArrayList
	{
		internal new MultiChartInstance this[int index]
		{
			get
			{
				return (MultiChartInstance)base[index];
			}
		}

		internal MultiChartInstanceList()
		{
		}

		internal MultiChartInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
