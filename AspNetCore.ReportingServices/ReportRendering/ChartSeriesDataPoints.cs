using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ChartSeriesDataPoints
	{
		private int m_count;

		private ChartDataPoint[] m_seriesCells;

		internal ChartDataPoint this[int index]
		{
			get
			{
				if (index >= 0 && index < this.m_count)
				{
					if (this.m_seriesCells != null)
					{
						return this.m_seriesCells[index];
					}
					return null;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.m_count);
			}
			set
			{
				if (index >= 0 && index < this.m_count)
				{
					if (this.m_seriesCells == null)
					{
						this.m_seriesCells = new ChartDataPoint[this.m_count];
					}
					this.m_seriesCells[index] = value;
					return;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.m_count);
			}
		}

		internal ChartSeriesDataPoints(int count)
		{
			this.m_count = count;
		}
	}
}
