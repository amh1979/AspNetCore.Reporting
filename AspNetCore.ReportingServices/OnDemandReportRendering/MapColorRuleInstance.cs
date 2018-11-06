using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapColorRuleInstance : MapAppearanceRuleInstance
	{
		private MapColorRule m_defObject;

		private bool? m_showInColorScale;

		public bool ShowInColorScale
		{
			get
			{
				if (!this.m_showInColorScale.HasValue)
				{
					this.m_showInColorScale = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule)this.m_defObject.MapAppearanceRuleDef).EvaluateShowInColorScale(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_showInColorScale.Value;
			}
		}

		internal MapColorRuleInstance(MapColorRule defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_showInColorScale = null;
		}
	}
}
