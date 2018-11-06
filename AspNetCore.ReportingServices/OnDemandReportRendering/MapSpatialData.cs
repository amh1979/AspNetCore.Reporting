using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialData
	{
		protected Map m_map;

		protected MapVectorLayer m_mapVectorLayer;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialData m_defObject;

		protected MapSpatialDataInstance m_instance;

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialData MapSpatialDataDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		internal MapSpatialDataInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal MapSpatialData(MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_defObject = mapVectorLayer.MapVectorLayerDef.MapSpatialData;
			this.m_mapVectorLayer = mapVectorLayer;
			this.m_map = map;
		}

		internal abstract MapSpatialDataInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
