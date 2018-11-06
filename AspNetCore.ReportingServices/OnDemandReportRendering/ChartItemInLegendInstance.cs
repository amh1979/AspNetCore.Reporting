namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartItemInLegendInstance : BaseInstance
	{
		private ChartItemInLegend m_chartItemInLegendDef;

		private string m_legendText;

		private string m_toolTip;

		private bool? m_hidden;

		public string LegendText
		{
			get
			{
				if (this.m_legendText == null && !this.m_chartItemInLegendDef.ChartDef.IsOldSnapshot)
				{
					this.m_legendText = this.m_chartItemInLegendDef.ChartItemInLegendDef.EvaluateLegendText(this.ReportScopeInstance, this.m_chartItemInLegendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_legendText;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_chartItemInLegendDef.ChartItemInLegendDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartItemInLegendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_chartItemInLegendDef.ChartItemInLegendDef.EvaluateHidden(this.ReportScopeInstance, this.m_chartItemInLegendDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		internal ChartItemInLegendInstance(ChartItemInLegend chartItemInLegendDef)
			: base(chartItemInLegendDef.ReportScope)
		{
			this.m_chartItemInLegendDef = chartItemInLegendDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_legendText = null;
			this.m_toolTip = null;
			this.m_hidden = null;
		}
	}
}
