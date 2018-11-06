using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearGaugeInstance : GaugeInstance
	{
		private LinearGauge m_defObject;

		private GaugeOrientations? m_orientation;

		public GaugeOrientations Orientation
		{
			get
			{
				if (!this.m_orientation.HasValue)
				{
					this.m_orientation = ((AspNetCore.ReportingServices.ReportIntermediateFormat.LinearGauge)this.m_defObject.GaugeDef).EvaluateOrientation(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_orientation.Value;
			}
		}

		internal LinearGaugeInstance(LinearGauge defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_orientation = null;
		}
	}
}
