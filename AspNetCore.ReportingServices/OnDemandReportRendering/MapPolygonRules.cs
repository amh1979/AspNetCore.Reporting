using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonRules
	{
		private Map m_map;

		private MapPolygonLayer m_mapPolygonLayer;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonRules m_defObject;

		private MapPolygonRulesInstance m_instance;

		private MapColorRule m_mapColorRule;

		public MapColorRule MapColorRule
		{
			get
			{
				if (this.m_mapColorRule == null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRule mapColorRule = this.m_defObject.MapColorRule;
					if (mapColorRule != null)
					{
						if (mapColorRule is AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)
						{
							this.m_mapColorRule = new MapColorRangeRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)this.m_defObject.MapColorRule, this.m_mapPolygonLayer, this.m_map);
						}
						else if (mapColorRule is AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)
						{
							this.m_mapColorRule = new MapColorPaletteRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)this.m_defObject.MapColorRule, this.m_mapPolygonLayer, this.m_map);
						}
						else if (mapColorRule is AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)
						{
							this.m_mapColorRule = new MapCustomColorRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)this.m_defObject.MapColorRule, this.m_mapPolygonLayer, this.m_map);
						}
					}
				}
				return this.m_mapColorRule;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonRules MapPolygonRulesDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapPolygonRulesInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapPolygonRulesInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapPolygonRules(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonRules defObject, MapPolygonLayer mapPolygonLayer, Map map)
		{
			this.m_defObject = defObject;
			this.m_mapPolygonLayer = mapPolygonLayer;
			this.m_map = map;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_mapColorRule != null)
			{
				this.m_mapColorRule.SetNewContext();
			}
		}
	}
}
