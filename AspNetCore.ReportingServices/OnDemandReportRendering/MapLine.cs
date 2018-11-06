using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLine : MapSpatialElement
	{
		private ReportBoolProperty m_useCustomLineTemplate;

		private MapLineTemplate m_mapLineTemplate;

		public ReportBoolProperty UseCustomLineTemplate
		{
			get
			{
				if (this.m_useCustomLineTemplate == null && this.MapLineDef.UseCustomLineTemplate != null)
				{
					this.m_useCustomLineTemplate = new ReportBoolProperty(this.MapLineDef.UseCustomLineTemplate);
				}
				return this.m_useCustomLineTemplate;
			}
		}

		public MapLineTemplate MapLineTemplate
		{
			get
			{
				if (this.m_mapLineTemplate == null && this.MapLineDef.MapLineTemplate != null)
				{
					this.m_mapLineTemplate = new MapLineTemplate(this.MapLineDef.MapLineTemplate, (MapLineLayer)base.m_mapVectorLayer, base.m_map);
				}
				return this.m_mapLineTemplate;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine MapLineDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine)base.MapSpatialElementDef;
			}
		}

		public new MapLineInstance Instance
		{
			get
			{
				return (MapLineInstance)this.GetInstance();
			}
		}

		internal MapLine(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLine defObject, MapLineLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
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
				base.m_instance = new MapLineInstance(this);
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
			if (this.m_mapLineTemplate != null)
			{
				this.m_mapLineTemplate.SetNewContext();
			}
		}
	}
}
