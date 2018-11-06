using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAreaCollection : ChartObjectCollectionBase<ChartArea, ChartAreaInstance>
	{
		private Chart m_chart;

		public override int Count
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return 1;
				}
				if (this.m_chart.ChartDef.ChartAreas != null)
				{
					return this.m_chart.ChartDef.ChartAreas.Count;
				}
				return 0;
			}
		}

		internal ChartAreaCollection(Chart chart)
		{
			this.m_chart = chart;
		}

		protected override ChartArea CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return new ChartArea(this.m_chart);
			}
			return new ChartArea(this.m_chart.ChartDef.ChartAreas[index], this.m_chart);
		}

		internal ChartArea GetByName(string areaName)
		{
			for (int i = 0; i < this.Count; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea chartArea = this.m_chart.ChartDef.ChartAreas[i];
				if (string.CompareOrdinal(areaName, chartArea.ChartAreaName) == 0)
				{
					return base[i];
				}
			}
			return null;
		}
	}
}
