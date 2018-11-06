namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumnCollection : ChartObjectCollectionBase<ChartLegendColumn, ChartLegendColumnInstance>
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
				return this.m_legend.ChartLegendDef.LegendColumns.Count;
			}
		}

		internal ChartLegendColumnCollection(ChartLegend legend, Chart chart)
		{
			this.m_legend = legend;
			this.m_chart = chart;
		}

		protected override ChartLegendColumn CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartLegendColumn(this.m_legend.ChartLegendDef.LegendColumns[index], this.m_chart);
		}
	}
}
