using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomColorRule : MapColorRule
	{
		private MapCustomColorCollection m_mapCustomColors;

		public MapCustomColorCollection MapCustomColors
		{
			get
			{
				if (this.m_mapCustomColors == null && this.MapCustomColorRuleDef.MapCustomColors != null)
				{
					this.m_mapCustomColors = new MapCustomColorCollection(this, base.m_map);
				}
				return this.m_mapCustomColors;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule MapCustomColorRuleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)base.MapAppearanceRuleDef;
			}
		}

		public new MapCustomColorRuleInstance Instance
		{
			get
			{
				return (MapCustomColorRuleInstance)this.GetInstance();
			}
		}

		internal MapCustomColorRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapAppearanceRuleInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapCustomColorRuleInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapCustomColors != null)
			{
				this.m_mapCustomColors.SetNewContext();
			}
		}
	}
}
