using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ChartDataPointCollection
	{
		private Chart m_owner;

		private int m_categoryCount;

		private int m_seriesCount;

		private ChartDataPoint m_firstDataPoint;

		private ChartSeriesDataPoints m_firstCategoryDataPoints;

		private ChartSeriesDataPoints m_firstSeriesDataPoints;

		private ChartSeriesDataPoints[] m_dataPoints;

		public ChartDataPoint this[int series, int category]
		{
			get
			{
				if (series >= 0 && series < this.m_seriesCount)
				{
					if (category >= 0 && category < this.m_categoryCount)
					{
						ChartDataPoint chartDataPoint = null;
						if (series == 0 && category == 0)
						{
							chartDataPoint = this.m_firstDataPoint;
						}
						else if (series == 0)
						{
							if (this.m_firstSeriesDataPoints != null)
							{
								chartDataPoint = this.m_firstSeriesDataPoints[category - 1];
							}
						}
						else if (category == 0)
						{
							if (this.m_firstCategoryDataPoints != null)
							{
								chartDataPoint = this.m_firstCategoryDataPoints[series - 1];
							}
						}
						else if (this.m_dataPoints != null && this.m_dataPoints[series - 1] != null)
						{
							chartDataPoint = this.m_dataPoints[series - 1][category - 1];
						}
						if (chartDataPoint == null)
						{
							chartDataPoint = new ChartDataPoint(this.m_owner, series, category);
							if (this.m_owner.RenderingContext.CacheState)
							{
								if (series == 0 && category == 0)
								{
									this.m_firstDataPoint = chartDataPoint;
								}
								else if (series == 0)
								{
									if (this.m_firstSeriesDataPoints == null)
									{
										this.m_firstSeriesDataPoints = new ChartSeriesDataPoints(this.m_categoryCount - 1);
									}
									this.m_firstSeriesDataPoints[category - 1] = chartDataPoint;
								}
								else if (category == 0)
								{
									if (this.m_firstCategoryDataPoints == null)
									{
										this.m_firstCategoryDataPoints = new ChartSeriesDataPoints(this.m_seriesCount - 1);
									}
									this.m_firstCategoryDataPoints[series - 1] = chartDataPoint;
								}
								else
								{
									if (this.m_dataPoints == null)
									{
										this.m_dataPoints = new ChartSeriesDataPoints[this.m_seriesCount - 1];
									}
									if (this.m_dataPoints[series - 1] == null)
									{
										this.m_dataPoints[series - 1] = new ChartSeriesDataPoints(this.m_categoryCount - 1);
									}
									this.m_dataPoints[series - 1][category - 1] = chartDataPoint;
								}
							}
						}
						return chartDataPoint;
					}
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, category, 0, this.m_categoryCount);
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, series, 0, this.m_seriesCount);
			}
		}

		public int Count
		{
			get
			{
				return this.m_seriesCount * this.m_categoryCount;
			}
		}

		public int SeriesCount
		{
			get
			{
				return this.m_seriesCount;
			}
		}

		public int CategoryCount
		{
			get
			{
				return this.m_categoryCount;
			}
		}

		internal ChartDataPointCollection(Chart owner, int seriesCount, int categoryCount)
		{
			this.m_owner = owner;
			this.m_seriesCount = seriesCount;
			this.m_categoryCount = categoryCount;
		}
	}
}
