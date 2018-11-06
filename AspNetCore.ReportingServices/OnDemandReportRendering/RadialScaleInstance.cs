using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RadialScaleInstance : GaugeScaleInstance
	{
		private RadialScale m_defObject;

		private double? m_radius;

		private double? m_startAngle;

		private double? m_sweepAngle;

		public double Radius
		{
			get
			{
				if (!this.m_radius.HasValue)
				{
					this.m_radius = ((AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale)this.m_defObject.GaugeScaleDef).EvaluateRadius(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_radius.Value;
			}
		}

		public double StartAngle
		{
			get
			{
				if (!this.m_startAngle.HasValue)
				{
					this.m_startAngle = ((AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale)this.m_defObject.GaugeScaleDef).EvaluateStartAngle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_startAngle.Value;
			}
		}

		public double SweepAngle
		{
			get
			{
				if (!this.m_sweepAngle.HasValue)
				{
					this.m_sweepAngle = ((AspNetCore.ReportingServices.ReportIntermediateFormat.RadialScale)this.m_defObject.GaugeScaleDef).EvaluateSweepAngle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_sweepAngle.Value;
			}
		}

		internal RadialScaleInstance(RadialScale defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_radius = null;
			this.m_startAngle = null;
			this.m_sweepAngle = null;
		}
	}
}
