using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class IndicatorImageInstance : BaseGaugeImageInstance
	{
		private ReportColor m_hueColor;

		private double? m_transparency;

		public ReportColor HueColor
		{
			get
			{
				if (this.m_hueColor == null)
				{
					this.m_hueColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage)base.m_defObject.BaseGaugeImageDef).EvaluateHueColor(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return this.m_hueColor;
			}
		}

		public double Transparency
		{
			get
			{
				if (!this.m_transparency.HasValue)
				{
					this.m_transparency = ((AspNetCore.ReportingServices.ReportIntermediateFormat.IndicatorImage)base.m_defObject.BaseGaugeImageDef).EvaluateTransparency(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_transparency.Value;
			}
		}

		internal IndicatorImageInstance(IndicatorImage defObject)
			: base(defObject)
		{
			base.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_hueColor = null;
			this.m_transparency = null;
		}
	}
}
