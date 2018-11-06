using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLineLayer : MapVectorLayer
	{
		private MapLineTemplate m_mapLineTemplate;

		private MapLineRules m_mapLineRules;

		private MapLineCollection m_mapLines;

		public MapLineTemplate MapLineTemplate
		{
			get
			{
				if (this.m_mapLineTemplate == null && this.MapLineLayerDef.MapLineTemplate != null)
				{
					this.m_mapLineTemplate = new MapLineTemplate(this.MapLineLayerDef.MapLineTemplate, this, base.m_map);
				}
				return this.m_mapLineTemplate;
			}
		}

		public MapLineRules MapLineRules
		{
			get
			{
				if (this.m_mapLineRules == null && this.MapLineLayerDef.MapLineRules != null)
				{
					this.m_mapLineRules = new MapLineRules(this.MapLineLayerDef.MapLineRules, this, base.m_map);
				}
				return this.m_mapLineRules;
			}
		}

		public MapLineCollection MapLines
		{
			get
			{
				if (this.m_mapLines == null && this.MapLineLayerDef.MapLines != null)
				{
					this.m_mapLines = new MapLineCollection(this, base.m_map);
				}
				return this.m_mapLines;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer MapLineLayerDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer)base.MapLayerDef;
			}
		}

		public new MapLineLayerInstance Instance
		{
			get
			{
				return (MapLineLayerInstance)this.GetInstance();
			}
		}

		internal MapLineLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLineLayer defObject, Map map)
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
				base.m_instance = new MapLineLayerInstance(this);
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
			if (this.m_mapLineTemplate != null)
			{
				this.m_mapLineTemplate.SetNewContext();
			}
			if (this.m_mapLineRules != null)
			{
				this.m_mapLineRules.SetNewContext();
			}
			if (this.m_mapLines != null)
			{
				this.m_mapLines.SetNewContext();
			}
		}
	}
}
