using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisCollection : ChartObjectCollectionBase<ChartAxis, ChartAxisInstance>
	{
		private Chart m_chart;

		private ChartArea m_chartArea;

		private bool m_isCategory;

		public override int Count
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return 1;
				}
				if (!this.m_isCategory)
				{
					return this.m_chartArea.ChartAreaDef.ValueAxes.Count;
				}
				return this.m_chartArea.ChartAreaDef.CategoryAxes.Count;
			}
		}

		internal ChartAxisCollection(ChartArea chartArea, Chart chart, bool isCategory)
		{
			this.m_chartArea = chartArea;
			this.m_chart = chart;
			this.m_isCategory = isCategory;
		}

		protected override ChartAxis CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				if (!this.m_isCategory)
				{
					return new ChartAxis(this.m_chart.RenderChartDef.ValueAxis, this.m_chart.ChartInstanceInfo.ValueAxis, this.m_chart, this.m_isCategory);
				}
				return new ChartAxis(this.m_chart.RenderChartDef.CategoryAxis, this.m_chart.ChartInstanceInfo.CategoryAxis, this.m_chart, this.m_isCategory);
			}
			if (!this.m_isCategory)
			{
				return new ChartAxis(this.m_chartArea.ChartAreaDef.ValueAxes[index], this.m_chart);
			}
			return new ChartAxis(this.m_chartArea.ChartAreaDef.CategoryAxes[index], this.m_chart);
		}

		internal ChartAxis GetByName(string axisName)
		{
			for (int i = 0; i < this.Count; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxis chartAxis = this.m_isCategory ? this.m_chartArea.ChartAreaDef.CategoryAxes[i] : this.m_chartArea.ChartAreaDef.ValueAxes[i];
				if (string.CompareOrdinal(axisName, chartAxis.AxisName) == 0)
				{
					return base[i];
				}
			}
			return null;
		}
	}
}
