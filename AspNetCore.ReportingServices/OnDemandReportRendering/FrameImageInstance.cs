using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class FrameImageInstance : BaseGaugeImageInstance
	{
		private ReportColor m_hueColor;

		private double? m_transparency;

		private bool? m_clipImage;

		public ReportColor HueColor
		{
			get
			{
				if (this.m_hueColor == null)
				{
					this.m_hueColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage)base.m_defObject.BaseGaugeImageDef).EvaluateHueColor(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext), true);
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
					this.m_transparency = ((AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage)base.m_defObject.BaseGaugeImageDef).EvaluateTransparency(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_transparency.Value;
			}
		}

		public bool ClipImage
		{
			get
			{
				if (!this.m_clipImage.HasValue)
				{
					this.m_clipImage = ((AspNetCore.ReportingServices.ReportIntermediateFormat.FrameImage)base.m_defObject.BaseGaugeImageDef).EvaluateClipImage(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_clipImage.Value;
			}
		}

		internal FrameImageInstance(FrameImage defObject)
			: base(defObject)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_hueColor = null;
			this.m_transparency = null;
			this.m_clipImage = null;
		}
	}
}
