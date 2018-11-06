namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorStateInstance : BaseInstance
	{
		private IndicatorState m_defObject;

		private ReportColor m_color;

		private double? m_scaleFactor;

		private GaugeStateIndicatorStyles? m_indicatorStyle;

		public ReportColor Color
		{
			get
			{
				if (this.m_color == null)
				{
					this.m_color = new ReportColor(this.m_defObject.IndicatorStateDef.EvaluateColor(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return this.m_color;
			}
		}

		public double ScaleFactor
		{
			get
			{
				if (!this.m_scaleFactor.HasValue)
				{
					this.m_scaleFactor = this.m_defObject.IndicatorStateDef.EvaluateScaleFactor(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_scaleFactor.Value;
			}
		}

		public GaugeStateIndicatorStyles IndicatorStyle
		{
			get
			{
				if (!this.m_indicatorStyle.HasValue)
				{
					this.m_indicatorStyle = this.m_defObject.IndicatorStateDef.EvaluateIndicatorStyle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_indicatorStyle.Value;
			}
		}

		internal IndicatorStateInstance(IndicatorState defObject)
			: base(defObject.GaugePanelDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_color = null;
			this.m_scaleFactor = null;
			this.m_indicatorStyle = null;
		}
	}
}
