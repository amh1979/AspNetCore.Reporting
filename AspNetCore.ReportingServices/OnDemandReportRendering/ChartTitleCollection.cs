namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartTitleCollection : ChartObjectCollectionBase<ChartTitle, ChartTitleInstance>
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
				if (this.m_chart.ChartDef.Titles != null)
				{
					return this.m_chart.ChartDef.Titles.Count;
				}
				return 0;
			}
		}

		internal ChartTitleCollection(Chart chart)
		{
			this.m_chart = chart;
		}

		protected override ChartTitle CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return new ChartTitle(this.m_chart.RenderChartDef.Title, this.m_chart.ChartInstanceInfo.Title, this.m_chart);
			}
			return new ChartTitle(this.m_chart.ChartDef.Titles[index], this.m_chart);
		}
	}
}
