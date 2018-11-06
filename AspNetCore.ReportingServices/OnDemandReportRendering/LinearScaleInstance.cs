using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class LinearScaleInstance : GaugeScaleInstance
	{
		private LinearScale m_defObject;

		private double? m_startMargin;

		private double? m_endMargin;

		private double? m_position;

		public double StartMargin
		{
			get
			{
				if (!this.m_startMargin.HasValue)
				{
					this.m_startMargin = ((AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale)this.m_defObject.GaugeScaleDef).EvaluateStartMargin(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_startMargin.Value;
			}
		}

		public double EndMargin
		{
			get
			{
				if (!this.m_endMargin.HasValue)
				{
					this.m_endMargin = ((AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale)this.m_defObject.GaugeScaleDef).EvaluateEndMargin(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_endMargin.Value;
			}
		}

		public double Position
		{
			get
			{
				if (!this.m_position.HasValue)
				{
					this.m_position = ((AspNetCore.ReportingServices.ReportIntermediateFormat.LinearScale)this.m_defObject.GaugeScaleDef).EvaluatePosition(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_position.Value;
			}
		}

		internal LinearScaleInstance(LinearScale defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_startMargin = null;
			this.m_endMargin = null;
			this.m_position = null;
		}
	}
}
