namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDerivedSeriesCollection : ChartObjectCollectionBase<ChartDerivedSeries, BaseInstance>
	{
		private Chart m_chart;

		public override int Count
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return this.m_chart.ChartDef.DerivedSeriesCollection.Count;
			}
		}

		internal ChartDerivedSeriesCollection(Chart chart)
		{
			this.m_chart = chart;
		}

		protected override ChartDerivedSeries CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartDerivedSeries(this.m_chart.ChartDef.DerivedSeriesCollection[index], this.m_chart);
		}
	}
}
