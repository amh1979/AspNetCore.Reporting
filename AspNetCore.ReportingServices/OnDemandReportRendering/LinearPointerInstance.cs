using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearPointerInstance : GaugePointerInstance
	{
		private LinearPointer m_defObject;

		private LinearPointerTypes? m_type;

		public LinearPointerTypes Type
		{
			get
			{
				if (!this.m_type.HasValue)
				{
					this.m_type = ((AspNetCore.ReportingServices.ReportIntermediateFormat.LinearPointer)this.m_defObject.GaugePointerDef).EvaluateType(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_type.Value;
			}
		}

		internal LinearPointerInstance(LinearPointer defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_type = null;
		}
	}
}
