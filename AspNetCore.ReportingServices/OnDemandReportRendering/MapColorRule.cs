using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapColorRule : MapAppearanceRule
	{
		private ReportBoolProperty m_showInColorScale;

		public ReportBoolProperty ShowInColorScale
		{
			get
			{
				if (this.m_showInColorScale == null && this.MapColorRuleDef.ShowInColorScale != null)
				{
					this.m_showInColorScale = new ReportBoolProperty(this.MapColorRuleDef.ShowInColorScale);
				}
				return this.m_showInColorScale;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule MapColorRuleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule)base.MapAppearanceRuleDef;
			}
		}

		internal new MapColorRuleInstance Instance
		{
			get
			{
				return (MapColorRuleInstance)this.GetInstance();
			}
		}

		internal MapColorRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
