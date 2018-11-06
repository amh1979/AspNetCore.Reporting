namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartFormulaParameterInstance : BaseInstance
	{
		private ChartFormulaParameter m_chartFormulaParameterDef;

		private object m_value;

		public object Value
		{
			get
			{
				if (this.m_value == null && !this.m_chartFormulaParameterDef.ChartDef.IsOldSnapshot)
				{
					this.m_value = this.m_chartFormulaParameterDef.ChartFormulaParameterDef.EvaluateValue(this.ReportScopeInstance, this.m_chartFormulaParameterDef.ChartDef.RenderingContext.OdpContext).Value;
				}
				return this.m_value;
			}
		}

		internal ChartFormulaParameterInstance(ChartFormulaParameter chartFormulaParameterDef)
			: base(chartFormulaParameterDef.ReportScope)
		{
			this.m_chartFormulaParameterDef = chartFormulaParameterDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_value = null;
		}
	}
}
