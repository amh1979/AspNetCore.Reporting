namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartStripLineCollection : ChartObjectCollectionBase<ChartStripLine, ChartStripLineInstance>
	{
		private Chart m_chart;

		private ChartAxis m_axis;

		public override int Count
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return this.m_axis.AxisDef.StripLines.Count;
			}
		}

		internal ChartStripLineCollection(ChartAxis axis, Chart chart)
		{
			this.m_axis = axis;
			this.m_chart = chart;
		}

		protected override ChartStripLine CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartStripLine(this.m_axis.AxisDef.StripLines[index], this.m_chart);
		}
	}
}
