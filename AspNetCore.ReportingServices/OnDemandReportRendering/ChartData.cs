namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartData
	{
		private Chart m_owner;

		private ChartSeriesCollection m_seriesCollection;

		private ChartDerivedSeriesCollection m_chartDerivedSeriesCollection;

		internal bool HasSeriesCollection
		{
			get
			{
				return this.m_seriesCollection != null;
			}
		}

		public ChartSeriesCollection SeriesCollection
		{
			get
			{
				if (this.m_seriesCollection == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						this.m_seriesCollection = new ShimChartSeriesCollection(this.m_owner);
					}
					else
					{
						this.m_seriesCollection = new InternalChartSeriesCollection(this.m_owner, this.m_owner.ChartDef.ChartSeriesCollection);
					}
				}
				return this.m_seriesCollection;
			}
		}

		public ChartDerivedSeriesCollection DerivedSeriesCollection
		{
			get
			{
				if (this.m_chartDerivedSeriesCollection == null && !this.m_owner.IsOldSnapshot && this.m_owner.ChartDef.DerivedSeriesCollection != null)
				{
					this.m_chartDerivedSeriesCollection = new ChartDerivedSeriesCollection(this.m_owner);
				}
				return this.m_chartDerivedSeriesCollection;
			}
		}

		internal ChartData(Chart owner)
		{
			this.m_owner = owner;
		}
	}
}
