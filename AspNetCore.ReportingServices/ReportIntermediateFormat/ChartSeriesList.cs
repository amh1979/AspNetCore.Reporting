using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartSeriesList : RowList
	{
		internal new ChartSeries this[int index]
		{
			get
			{
				return (ChartSeries)base[index];
			}
		}

		public ChartSeriesList()
		{
		}

		internal ChartSeriesList(int capacity)
			: base(capacity)
		{
		}

		internal ChartSeries GetByName(string seriesName)
		{
			for (int i = 0; i < this.Count; i++)
			{
				ChartSeries chartSeries = this[i];
				if (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(seriesName, chartSeries.Name, false) == 0)
				{
					return chartSeries;
				}
			}
			return null;
		}
	}
}
