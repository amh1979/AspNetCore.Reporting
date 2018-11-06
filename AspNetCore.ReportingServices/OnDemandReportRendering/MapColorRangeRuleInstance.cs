using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorRangeRuleInstance : MapColorRuleInstance
	{
		private MapColorRangeRule m_defObject;

		private ReportColor m_startColor;

		private ReportColor m_middleColor;

		private ReportColor m_endColor;

		public ReportColor StartColor
		{
			get
			{
				if (this.m_startColor == null)
				{
					this.m_startColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)this.m_defObject.MapColorRuleDef).EvaluateStartColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_startColor;
			}
		}

		public ReportColor MiddleColor
		{
			get
			{
				if (this.m_middleColor == null)
				{
					this.m_middleColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)this.m_defObject.MapColorRuleDef).EvaluateMiddleColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_middleColor;
			}
		}

		public ReportColor EndColor
		{
			get
			{
				if (this.m_endColor == null)
				{
					this.m_endColor = new ReportColor(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)this.m_defObject.MapColorRuleDef).EvaluateEndColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_endColor;
			}
		}

		internal MapColorRangeRuleInstance(MapColorRangeRule defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_startColor = null;
			this.m_middleColor = null;
			this.m_endColor = null;
		}
	}
}
