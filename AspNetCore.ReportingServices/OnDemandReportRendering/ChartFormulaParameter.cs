using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartFormulaParameter : ChartObjectCollectionItem<ChartFormulaParameterInstance>
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter m_chartFormulaParameterDef;

		private ReportVariantProperty m_value;

		private ChartDerivedSeries m_chartDerivedSeries;

		public string Name
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return null;
				}
				return this.m_chartFormulaParameterDef.FormulaParameterName;
			}
		}

		public ReportVariantProperty Value
		{
			get
			{
				if (this.m_value == null && !this.m_chart.IsOldSnapshot && this.m_chartFormulaParameterDef.Value != null)
				{
					this.m_value = new ReportVariantProperty(this.m_chartFormulaParameterDef.Value);
				}
				return this.m_value;
			}
		}

		public string Source
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return null;
				}
				if (this.m_chartFormulaParameterDef.Source != null)
				{
					return this.m_chartFormulaParameterDef.Source;
				}
				return null;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				return this.m_chartDerivedSeries.ReportScope;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter ChartFormulaParameterDef
		{
			get
			{
				return this.m_chartFormulaParameterDef;
			}
		}

		public ChartFormulaParameterInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartFormulaParameterInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartFormulaParameter(ChartDerivedSeries chartDerivedSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartFormulaParameter chartFormulaParameterDef, Chart chart)
		{
			this.m_chartDerivedSeries = chartDerivedSeries;
			this.m_chartFormulaParameterDef = chartFormulaParameterDef;
			this.m_chart = chart;
		}
	}
}
