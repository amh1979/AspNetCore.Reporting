using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPoint : MapSpatialElement
	{
		private ReportBoolProperty m_useCustomPointTemplate;

		private MapPointTemplate m_mapPointTemplate;

		public ReportBoolProperty UseCustomPointTemplate
		{
			get
			{
				if (this.m_useCustomPointTemplate == null && this.MapPointDef.UseCustomPointTemplate != null)
				{
					this.m_useCustomPointTemplate = new ReportBoolProperty(this.MapPointDef.UseCustomPointTemplate);
				}
				return this.m_useCustomPointTemplate;
			}
		}

		public MapPointTemplate MapPointTemplate
		{
			get
			{
				if (this.m_mapPointTemplate == null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate = this.MapPointDef.MapPointTemplate;
					if (mapPointTemplate != null && mapPointTemplate is AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)
					{
						this.m_mapPointTemplate = new MapMarkerTemplate((AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)mapPointTemplate, base.m_mapVectorLayer, base.m_map);
					}
				}
				return this.m_mapPointTemplate;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint MapPointDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint)base.MapSpatialElementDef;
			}
		}

		public new MapPointInstance Instance
		{
			get
			{
				return (MapPointInstance)this.GetInstance();
			}
		}

		internal MapPoint(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPoint defObject, MapPointLayer mapPointLayer, Map map)
			: base(defObject, mapPointLayer, map)
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
				base.m_instance = new MapPointInstance(this);
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
			if (this.m_mapPointTemplate != null)
			{
				this.m_mapPointTemplate.SetNewContext();
			}
		}
	}
}
