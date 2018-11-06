using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeTickMarksInstance : TickMarkStyleInstance
	{
		private double? m_interval;

		private double? m_intervalOffset;

		public double Interval
		{
			get
			{
				if (!this.m_interval.HasValue)
				{
					this.m_interval = ((AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks)base.m_defObject.TickMarkStyleDef).EvaluateInterval(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_interval.Value;
			}
		}

		public double IntervalOffset
		{
			get
			{
				if (!this.m_intervalOffset.HasValue)
				{
					this.m_intervalOffset = ((AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeTickMarks)base.m_defObject.TickMarkStyleDef).EvaluateIntervalOffset(this.ReportScopeInstance, base.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_intervalOffset.Value;
			}
		}

		internal GaugeTickMarksInstance(GaugeTickMarks defObject)
			: base(defObject)
		{
			base.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_interval = null;
			this.m_intervalOffset = null;
		}
	}
}
