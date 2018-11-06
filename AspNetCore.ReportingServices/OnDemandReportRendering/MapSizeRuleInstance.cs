using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSizeRuleInstance : MapAppearanceRuleInstance
	{
		private MapSizeRule m_defObject;

		private ReportSize m_startSize;

		private ReportSize m_endSize;

		public ReportSize StartSize
		{
			get
			{
				if (this.m_startSize == null)
				{
					this.m_startSize = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule)this.m_defObject.MapAppearanceRuleDef).EvaluateStartSize(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_startSize;
			}
		}

		public ReportSize EndSize
		{
			get
			{
				if (this.m_endSize == null)
				{
					this.m_endSize = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule)this.m_defObject.MapAppearanceRuleDef).EvaluateEndSize(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_endSize;
			}
		}

		internal MapSizeRuleInstance(MapSizeRule defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_startSize = null;
			this.m_endSize = null;
		}
	}
}
