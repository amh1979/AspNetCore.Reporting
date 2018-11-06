using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartFormulaParameterCollection : ChartObjectCollectionBase<ChartFormulaParameter, ChartFormulaParameterInstance>
	{
		private Chart m_chart;

		private ChartDerivedSeries m_derivedSeries;

		public ChartFormulaParameter this[string name]
		{
			get
			{
				if (!this.m_chart.IsOldSnapshot)
				{
					for (int i = 0; i < this.Count; i++)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameter = this.m_derivedSeries.ChartDerivedSeriesDef.FormulaParameters[i];
						if (string.CompareOrdinal(name, chartFormulaParameter.FormulaParameterName) == 0)
						{
							return base[i];
						}
					}
				}
				return null;
			}
		}

		public override int Count
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return 0;
				}
				return this.m_derivedSeries.ChartDerivedSeriesDef.FormulaParameters.Count;
			}
		}

		internal ChartFormulaParameterCollection(ChartDerivedSeries derivedSeries, Chart chart)
		{
			this.m_derivedSeries = derivedSeries;
			this.m_chart = chart;
		}

		protected override ChartFormulaParameter CreateChartObject(int index)
		{
			if (this.m_chart.IsOldSnapshot)
			{
				return null;
			}
			return new ChartFormulaParameter(this.m_derivedSeries, this.m_derivedSeries.ChartDerivedSeriesDef.FormulaParameters[index], this.m_chart);
		}
	}
}
