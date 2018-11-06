using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialPointerInstance : GaugePointerInstance
	{
		private RadialPointer m_defObject;

		private RadialPointerTypes? m_type;

		private RadialPointerNeedleStyles? m_needleStyle;

		public RadialPointerTypes Type
		{
			get
			{
				if (!this.m_type.HasValue)
				{
					this.m_type = ((AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer)this.m_defObject.GaugePointerDef).EvaluateType(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_type.Value;
			}
		}

		public RadialPointerNeedleStyles NeedleStyle
		{
			get
			{
				if (!this.m_needleStyle.HasValue)
				{
					this.m_needleStyle = ((AspNetCore.ReportingServices.ReportIntermediateFormat.RadialPointer)this.m_defObject.GaugePointerDef).EvaluateNeedleStyle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_needleStyle.Value;
			}
		}

		internal RadialPointerInstance(RadialPointer defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_type = null;
			this.m_needleStyle = null;
		}
	}
}
