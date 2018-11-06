using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygon : MapSpatialElement
	{
		private ReportBoolProperty m_useCustomPolygonTemplate;

		private MapPolygonTemplate m_mapPolygonTemplate;

		private ReportBoolProperty m_useCustomCenterPointTemplate;

		private MapPointTemplate m_mapCenterPointTemplate;

		public ReportBoolProperty UseCustomPolygonTemplate
		{
			get
			{
				if (this.m_useCustomPolygonTemplate == null && this.MapPolygonDef.UseCustomPolygonTemplate != null)
				{
					this.m_useCustomPolygonTemplate = new ReportBoolProperty(this.MapPolygonDef.UseCustomPolygonTemplate);
				}
				return this.m_useCustomPolygonTemplate;
			}
		}

		public MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				if (this.m_mapPolygonTemplate == null && this.MapPolygonDef.MapPolygonTemplate != null)
				{
					this.m_mapPolygonTemplate = new MapPolygonTemplate(this.MapPolygonDef.MapPolygonTemplate, (MapPolygonLayer)base.m_mapVectorLayer, base.m_map);
				}
				return this.m_mapPolygonTemplate;
			}
		}

		public ReportBoolProperty UseCustomCenterPointTemplate
		{
			get
			{
				if (this.m_useCustomCenterPointTemplate == null && this.MapPolygonDef.UseCustomCenterPointTemplate != null)
				{
					this.m_useCustomCenterPointTemplate = new ReportBoolProperty(this.MapPolygonDef.UseCustomCenterPointTemplate);
				}
				return this.m_useCustomCenterPointTemplate;
			}
		}

		public MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				if (this.m_mapCenterPointTemplate == null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapCenterPointTemplate = this.MapPolygonDef.MapCenterPointTemplate;
					if (mapCenterPointTemplate != null && mapCenterPointTemplate is AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)
					{
						this.m_mapCenterPointTemplate = new MapMarkerTemplate((AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)mapCenterPointTemplate, base.m_mapVectorLayer, base.m_map);
					}
				}
				return this.m_mapCenterPointTemplate;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon MapPolygonDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon)base.MapSpatialElementDef;
			}
		}

		public new MapPolygonInstance Instance
		{
			get
			{
				return (MapPolygonInstance)this.GetInstance();
			}
		}

		internal MapPolygon(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPolygon defObject, MapPolygonLayer mapPolygonLayer, Map map)
			: base(defObject, mapPolygonLayer, map)
		{
		}

		internal override MapSpatialElementInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapPolygonInstance(this);
			}
			return (MapSpatialElementInstance)base.m_instance;
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
			if (this.m_mapCenterPointTemplate != null)
			{
				this.m_mapCenterPointTemplate.SetNewContext();
			}
		}
	}
}
