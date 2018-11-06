namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartCustomPaletteColorInstance : BaseInstance
	{
		private ChartCustomPaletteColor m_chartCustomPaletteColorDef;

		private bool m_colorEvaluated;

		private ReportColor m_color;

		public ReportColor Color
		{
			get
			{
				if (!this.m_colorEvaluated)
				{
					this.m_colorEvaluated = true;
					if (!this.m_chartCustomPaletteColorDef.ChartDef.IsOldSnapshot)
					{
						this.m_color = new ReportColor(this.m_chartCustomPaletteColorDef.ChartCustomPaletteColorDef.EvaluateColor(this.ReportScopeInstance, this.m_chartCustomPaletteColorDef.ChartDef.RenderingContext.OdpContext), true);
					}
				}
				return this.m_color;
			}
		}

		internal ChartCustomPaletteColorInstance(ChartCustomPaletteColor chartCustomPaletteColorDef)
			: base(chartCustomPaletteColorDef.ChartDef)
		{
			this.m_chartCustomPaletteColorDef = chartCustomPaletteColorDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_colorEvaluated = false;
		}
	}
}
