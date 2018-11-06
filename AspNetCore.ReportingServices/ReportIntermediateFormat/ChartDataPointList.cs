using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartDataPointList : CellList
	{
		internal new ChartDataPoint this[int index]
		{
			get
			{
				return (ChartDataPoint)base[index];
			}
		}

		public ChartDataPointList()
		{
		}

		internal ChartDataPointList(int capacity)
			: base(capacity)
		{
		}
	}
}
