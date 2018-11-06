using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PointerImageInstance : BaseGaugeImageInstance
	{
		private ReportColor m_hueColor;

		private double? m_transparency;

		private ReportSize m_offsetX;

		private ReportSize m_offsetY;

		public ReportColor HueColor
		{
			get
			{
				if (this.m_hueColor == null)
				{
					this.m_hueColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage)base.m_defObject.BaseGaugeImageDef).EvaluateHueColor(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext), true);
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
					this.m_transparency = ((AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage)base.m_defObject.BaseGaugeImageDef).EvaluateTransparency(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_transparency.Value;
			}
		}

		public ReportSize OffsetX
		{
			get
			{
				if (this.m_offsetX == null)
				{
					this.m_offsetX = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage)base.m_defObject.BaseGaugeImageDef).EvaluateOffsetX(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext));
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
					this.m_offsetY = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.PointerImage)base.m_defObject.BaseGaugeImageDef).EvaluateOffsetY(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext));
				}
				return this.m_offsetY;
			}
		}

		internal PointerImageInstance(PointerImage defObject)
			: base(defObject)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_hueColor = null;
			this.m_transparency = null;
			this.m_offsetX = null;
			this.m_offsetY = null;
		}
	}
}
