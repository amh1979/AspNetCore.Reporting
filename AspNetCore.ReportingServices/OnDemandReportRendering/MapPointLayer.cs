using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPointLayer : MapVectorLayer
	{
		private MapPointTemplate m_mapPointTemplate;

		private MapPointRules m_mapPointRules;

		private MapPointCollection m_mapPoints;

		public MapPointTemplate MapPointTemplate
		{
			get
			{
				if (this.m_mapPointTemplate == null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointTemplate mapPointTemplate = this.MapPointLayerDef.MapPointTemplate;
					if (mapPointTemplate != null && mapPointTemplate is AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)
					{
						this.m_mapPointTemplate = new MapMarkerTemplate((AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)mapPointTemplate, this, base.m_map);
					}
				}
				return this.m_mapPointTemplate;
			}
		}

		public MapPointRules MapPointRules
		{
			get
			{
				if (this.m_mapPointRules == null && this.MapPointLayerDef.MapPointRules != null)
				{
					this.m_mapPointRules = new MapPointRules(this.MapPointLayerDef.MapPointRules, this, base.m_map);
				}
				return this.m_mapPointRules;
			}
		}

		public MapPointCollection MapPoints
		{
			get
			{
				if (this.m_mapPoints == null && this.MapPointLayerDef.MapPoints != null)
				{
					this.m_mapPoints = new MapPointCollection(this, base.m_map);
				}
				return this.m_mapPoints;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer MapPointLayerDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer)base.MapLayerDef;
			}
		}

		public new MapPointLayerInstance Instance
		{
			get
			{
				return (MapPointLayerInstance)this.GetInstance();
			}
		}

		internal MapPointLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.MapPointLayer defObject, Map map)
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
				base.m_instance = new MapPointLayerInstance(this);
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
			if (this.m_mapPointTemplate != null)
			{
				this.m_mapPointTemplate.SetNewContext();
			}
			if (this.m_mapPointRules != null)
			{
				this.m_mapPointRules.SetNewContext();
			}
			if (this.m_mapPoints != null)
			{
				this.m_mapPoints.SetNewContext();
			}
		}
	}
}
