using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineRules
	{
		private Map m_map;

		private MapLineLayer m_mapLineLayer;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineRules m_defObject;

		private MapLineRulesInstance m_instance;

		private MapSizeRule m_mapSizeRule;

		private MapColorRule m_mapColorRule;

		public MapSizeRule MapSizeRule
		{
			get
			{
				if (this.m_mapSizeRule == null && this.m_defObject.MapSizeRule != null)
				{
					this.m_mapSizeRule = new MapSizeRule(this.m_defObject.MapSizeRule, this.m_mapLineLayer, this.m_map);
				}
				return this.m_mapSizeRule;
			}
		}

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
							this.m_mapColorRule = new MapColorRangeRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)this.m_defObject.MapColorRule, this.m_mapLineLayer, this.m_map);
						}
						else if (mapColorRule is AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)
						{
							this.m_mapColorRule = new MapColorPaletteRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)this.m_defObject.MapColorRule, this.m_mapLineLayer, this.m_map);
						}
						else if (mapColorRule is AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)
						{
							this.m_mapColorRule = new MapCustomColorRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)this.m_defObject.MapColorRule, this.m_mapLineLayer, this.m_map);
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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineRules MapLineRulesDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapLineRulesInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapLineRulesInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapLineRules(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineRules defObject, MapLineLayer mapLineLayer, Map map)
		{
			this.m_defObject = defObject;
			this.m_mapLineLayer = mapLineLayer;
			this.m_map = map;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_mapSizeRule != null)
			{
				this.m_mapSizeRule.SetNewContext();
			}
			if (this.m_mapColorRule != null)
			{
				this.m_mapColorRule.SetNewContext();
			}
		}
	}
}
