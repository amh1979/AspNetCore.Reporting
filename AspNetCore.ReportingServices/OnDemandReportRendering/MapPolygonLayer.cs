using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonLayer : MapVectorLayer
	{
		private MapPolygonTemplate m_mapPolygonTemplate;

		private MapPolygonRules m_mapPolygonRules;

		private MapPointTemplate m_mapCenterPointTemplate;

		private MapPointRules m_mapcenterPointRules;

		private MapPolygonCollection m_mapPolygons;

		public MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				if (this.m_mapPolygonTemplate == null && this.MapPolygonLayerDef.MapPolygonTemplate != null)
				{
					this.m_mapPolygonTemplate = new MapPolygonTemplate(this.MapPolygonLayerDef.MapPolygonTemplate, this, base.m_map);
				}
				return this.m_mapPolygonTemplate;
			}
		}

		public MapPolygonRules MapPolygonRules
		{
			get
			{
				if (this.m_mapPolygonRules == null && this.MapPolygonLayerDef.MapPolygonRules != null)
				{
					this.m_mapPolygonRules = new MapPolygonRules(this.MapPolygonLayerDef.MapPolygonRules, this, base.m_map);
				}
				return this.m_mapPolygonRules;
			}
		}

		public MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				if (this.m_mapCenterPointTemplate == null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapCenterPointTemplate = this.MapPolygonLayerDef.MapCenterPointTemplate;
					if (mapCenterPointTemplate != null && mapCenterPointTemplate is AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)
					{
						this.m_mapCenterPointTemplate = new MapMarkerTemplate((AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)mapCenterPointTemplate, this, base.m_map);
					}
				}
				return this.m_mapCenterPointTemplate;
			}
		}

		public MapPointRules MapCenterPointRules
		{
			get
			{
				if (this.m_mapcenterPointRules == null && this.MapPolygonLayerDef.MapCenterPointRules != null)
				{
					this.m_mapcenterPointRules = new MapPointRules(this.MapPolygonLayerDef.MapCenterPointRules, this, base.m_map);
				}
				return this.m_mapcenterPointRules;
			}
		}

		public MapPolygonCollection MapPolygons
		{
			get
			{
				if (this.m_mapPolygons == null && this.MapPolygonLayerDef.MapPolygons != null)
				{
					this.m_mapPolygons = new MapPolygonCollection(this, base.m_map);
				}
				return this.m_mapPolygons;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer MapPolygonLayerDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer)base.MapLayerDef;
			}
		}

		public new MapPolygonLayerInstance Instance
		{
			get
			{
				return (MapPolygonLayerInstance)this.GetInstance();
			}
		}

		internal MapPolygonLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygonLayer defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapLayerInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapPolygonLayerInstance(this);
			}
			return (MapVectorLayerInstance)base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapPolygonTemplate != null)
			{
				this.m_mapPolygonTemplate.SetNewContext();
			}
			if (this.m_mapPolygonRules != null)
			{
				this.m_mapPolygonRules.SetNewContext();
			}
			if (this.m_mapCenterPointTemplate != null)
			{
				this.m_mapCenterPointTemplate.SetNewContext();
			}
			if (this.m_mapcenterPointRules != null)
			{
				this.m_mapcenterPointRules.SetNewContext();
			}
			if (this.m_mapPolygons != null)
			{
				this.m_mapPolygons.SetNewContext();
			}
		}
	}
}
