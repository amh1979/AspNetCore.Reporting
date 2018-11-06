namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemCollection : ChartObjectCollectionBase<ChartLegendCustomItem, ChartLegendCustomItemInstance>
	{
		private Chart m_chart;

		private ChartLegend m_legend;

		public override int Count
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return this.m_legend.ChartLegendDef.LegendCustomItems.Count;
			}
		}

		internal ChartLegendCustomItemCollection(ChartLegend legend, Chart chart)
		{
			this.m_legend = legend;
			this.m_chart = chart;
		}

		protected override ChartLegendCustomItem CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartLegendCustomItem(this.m_legend.ChartLegendDef.LegendCustomItems[index], this.m_chart);
		}
	}
}
