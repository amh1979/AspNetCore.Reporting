using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CapImageInstance : BaseGaugeImageInstance
	{
		private ReportColor m_hueColor;

		private ReportSize m_offsetX;

		private ReportSize m_offsetY;

		public ReportColor HueColor
		{
			get
			{
				if (this.m_hueColor == null)
				{
					this.m_hueColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage)base.m_defObject.BaseGaugeImageDef).EvaluateHueColor(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext), true);
				}
				return this.m_hueColor;
			}
		}

		public ReportSize OffsetX
		{
			get
			{
				if (this.m_offsetX == null)
				{
					this.m_offsetX = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage)base.m_defObject.BaseGaugeImageDef).EvaluateOffsetX(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return this.m_offsetX;
			}
		}

		public ReportSize OffsetY
		{
			get
			{
				if (this.m_offsetY == null)
				{
					this.m_offsetY = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.CapImage)base.m_defObject.BaseGaugeImageDef).EvaluateOffsetY(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return this.m_offsetY;
			}
		}

		internal CapImageInstance(CapImage defObject)
			: base(defObject)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_hueColor = null;
			this.m_offsetX = null;
			this.m_offsetY = null;
		}
	}
}
