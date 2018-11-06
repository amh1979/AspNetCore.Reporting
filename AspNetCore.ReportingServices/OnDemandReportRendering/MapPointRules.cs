using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointRules
	{
		private Map m_map;

		private MapVectorLayer m_mapVectorLayer;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointRules m_defObject;

		private MapPointRulesInstance m_instance;

		private MapSizeRule m_mapSizeRule;

		private MapColorRule m_mapColorRule;

		private MapMarkerRule m_mapMarkerRule;

		public MapSizeRule MapSizeRule
		{
			get
			{
				if (this.m_mapSizeRule == null && this.m_defObject.MapSizeRule != null)
				{
					this.m_mapSizeRule = new MapSizeRule(this.m_defObject.MapSizeRule, this.m_mapVectorLayer, this.m_map);
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
							this.m_mapColorRule = new MapColorRangeRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)this.m_defObject.MapColorRule, this.m_mapVectorLayer, this.m_map);
						}
						else if (mapColorRule is AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)
						{
							this.m_mapColorRule = new MapColorPaletteRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)this.m_defObject.MapColorRule, this.m_mapVectorLayer, this.m_map);
						}
						else if (mapColorRule is AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)
						{
							this.m_mapColorRule = new MapCustomColorRule((AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomColorRule)this.m_defObject.MapColorRule, this.m_mapVectorLayer, this.m_map);
						}
					}
				}
				return this.m_mapColorRule;
			}
		}

		public MapMarkerRule MapMarkerRule
		{
			get
			{
				if (this.m_mapMarkerRule == null && this.m_defObject.MapMarkerRule != null)
				{
					this.m_mapMarkerRule = new MapMarkerRule(this.m_defObject.MapMarkerRule, this.m_mapVectorLayer, this.m_map);
				}
				return this.m_mapMarkerRule;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointRules MapMarkerRulesDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapPointRulesInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapPointRulesInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapPointRules(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointRules defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_defObject = defObject;
			this.m_mapVectorLayer = mapVectorLayer;
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
			if (this.m_mapMarkerRule != null)
			{
				this.m_mapMarkerRule.SetNewContext();
			}
		}
	}
}
