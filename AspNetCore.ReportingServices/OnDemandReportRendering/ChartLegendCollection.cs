namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCollection : ChartObjectCollectionBase<ChartLegend, ChartLegendInstance>
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
				if (this.m_chart.ChartDef.Legends != null)
				{
					return this.m_chart.ChartDef.Legends.Count;
				}
				return 0;
			}
		}

		internal ChartLegendCollection(Chart chart)
		{
			this.m_chart = chart;
		}

		protected override ChartLegend CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				if (this.m_chart.RenderChartDef.Legend != null)
				{
					return new ChartLegend(this.m_chart.RenderChartDef.Legend, this.m_chart.ChartInstanceInfo.LegendStyleAttributeValues, this.m_chart);
				}
				return null;
			}
			return new ChartLegend(this.m_chart.ChartDef.Legends[index], this.m_chart);
		}
	}
}
