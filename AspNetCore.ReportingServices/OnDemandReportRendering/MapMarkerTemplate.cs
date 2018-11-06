using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerTemplate : MapPointTemplate
	{
		private MapMarker m_mapMarker;

		public MapMarker MapMarker
		{
			get
			{
				if (this.m_mapMarker == null && this.MapMarkerTemplateDef.MapMarker != null)
				{
					this.m_mapMarker = new MapMarker(this.MapMarkerTemplateDef.MapMarker, base.m_map);
				}
				return this.m_mapMarker;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate MapMarkerTemplateDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)base.MapSpatialElementTemplateDef;
			}
		}

		public new MapMarkerTemplateInstance Instance
		{
			get
			{
				return (MapMarkerTemplateInstance)this.GetInstance();
			}
		}

		internal MapMarkerTemplate(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapSpatialElementTemplateInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapMarkerTemplateInstance(this);
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
			if (this.m_mapMarker != null)
			{
				this.m_mapMarker.SetNewContext();
			}
		}
	}
}
