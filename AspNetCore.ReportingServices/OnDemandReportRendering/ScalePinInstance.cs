using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScalePinInstance : TickMarkStyleInstance
	{
		private double? m_location;

		private bool? m_enable;

		public double Location
		{
			get
			{
				if (!this.m_location.HasValue)
				{
					this.m_location = ((AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin)base.m_defObject.TickMarkStyleDef).EvaluateLocation(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_location.Value;
			}
		}

		public bool Enable
		{
			get
			{
				if (!this.m_enable.HasValue)
				{
					this.m_enable = ((AspNetCore.ReportingServices.ReportIntermediateFormat.ScalePin)base.m_defObject.TickMarkStyleDef).EvaluateEnable(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_enable.Value;
			}
		}

		internal ScalePinInstance(ScalePin defObject)
			: base(defObject)
		{
			base.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_location = null;
			this.m_enable = null;
		}
	}
}
