using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalChartSeriesCollection : ChartSeriesCollection
	{
		private ChartSeriesList m_seriesDefs;

		public override ChartSeries this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_chartSeriesCollection == null)
					{
						base.m_chartSeriesCollection = new ChartSeries[this.Count];
					}
					if (base.m_chartSeriesCollection[index] == null)
					{
						base.m_chartSeriesCollection[index] = new InternalChartSeries(base.m_owner, index, this.m_seriesDefs[index]);
					}
					return base.m_chartSeriesCollection[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_seriesDefs.Count;
			}
		}

		internal InternalChartSeriesCollection(Chart owner, ChartSeriesList seriesDefs)
			: base(owner)
		{
			this.m_seriesDefs = seriesDefs;
		}

		internal InternalChartSeries GetByName(string seriesName)
		{
			for (int i = 0; i < this.Count; i++)
			{
				InternalChartSeries internalChartSeries = (InternalChartSeries)((ReportElementCollectionBase<ChartSeries>)this)[i];
				if (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(seriesName, internalChartSeries.Name, false) == 0)
				{
					return internalChartSeries;
				}
			}
			return null;
		}
	}
}
