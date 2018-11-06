using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialGaugeInstance : GaugeInstance
	{
		private RadialGauge m_defObject;

		private double? m_pivotX;

		private double? m_pivotY;

		public double PivotX
		{
			get
			{
				if (!this.m_pivotX.HasValue)
				{
					this.m_pivotX = ((AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge)this.m_defObject.GaugeDef).EvaluatePivotX(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_pivotX.Value;
			}
		}

		public double PivotY
		{
			get
			{
				if (!this.m_pivotY.HasValue)
				{
					this.m_pivotY = ((AspNetCore.ReportingServices.ReportIntermediateFormat.RadialGauge)this.m_defObject.GaugeDef).EvaluatePivotY(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_pivotY.Value;
			}
		}

		internal RadialGaugeInstance(RadialGauge defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_pivotX = null;
			this.m_pivotY = null;
		}
	}
}
