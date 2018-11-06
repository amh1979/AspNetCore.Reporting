namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendCustomItemCellCollection : ChartObjectCollectionBase<ChartLegendCustomItemCell, ChartLegendCustomItemCellInstance>
	{
		private Chart m_chart;

		private ChartLegendCustomItem m_legendCustomItem;

		public override int Count
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return this.m_legendCustomItem.ChartLegendCustomItemDef.LegendCustomItemCells.Count;
			}
		}

		internal ChartLegendCustomItemCellCollection(ChartLegendCustomItem legendCustomItem, Chart chart)
		{
			this.m_legendCustomItem = legendCustomItem;
			this.m_chart = chart;
		}

		protected override ChartLegendCustomItemCell CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartLegendCustomItemCell(this.m_legendCustomItem.ChartLegendCustomItemDef.LegendCustomItemCells[index], this.m_chart);
		}
	}
}
