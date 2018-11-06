namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegendColumnHeaderInstance : BaseInstance
	{
		private ChartLegendColumnHeader m_chartLegendColumnHeaderDef;

		private StyleInstance m_style;

		private string m_value;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartLegendColumnHeaderDef, this.m_chartLegendColumnHeaderDef.ChartDef, this.m_chartLegendColumnHeaderDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string Value
		{
			get
			{
				if (this.m_value == null && !this.m_chartLegendColumnHeaderDef.ChartDef.IsOldSnapshot)
				{
					this.m_value = this.m_chartLegendColumnHeaderDef.ChartLegendColumnHeaderDef.EvaluateValue(this.ReportScopeInstance, this.m_chartLegendColumnHeaderDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_value;
			}
		}

		internal ChartLegendColumnHeaderInstance(ChartLegendColumnHeader chartLegendColumnHeaderDef)
			: base(chartLegendColumnHeaderDef.ChartDef)
		{
			this.m_chartLegendColumnHeaderDef = chartLegendColumnHeaderDef;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_value = null;
		}
	}
}
