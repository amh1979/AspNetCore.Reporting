using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class GaugeInstance : GaugePanelItemInstance
	{
		private Gauge m_defObject;

		private bool? m_clipContent;

		private double? m_aspectRatio;

		public bool ClipContent
		{
			get
			{
				if (!this.m_clipContent.HasValue)
				{
					this.m_clipContent = ((AspNetCore.ReportingServices.ReportIntermediateFormat.Gauge)this.m_defObject.GaugePanelItemDef).EvaluateClipContent(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_clipContent.Value;
			}
		}

		public double AspectRatio
		{
			get
			{
				if (!this.m_aspectRatio.HasValue)
				{
					this.m_aspectRatio = ((AspNetCore.ReportingServices.ReportIntermediateFormat.Gauge)this.m_defObject.GaugePanelItemDef).EvaluateAspectRatio(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_aspectRatio.Value;
			}
		}

		internal GaugeInstance(Gauge defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_clipContent = null;
			this.m_aspectRatio = null;
		}
	}
}
