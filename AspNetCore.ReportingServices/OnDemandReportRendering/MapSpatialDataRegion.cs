using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSpatialDataRegion : MapSpatialData
	{
		private ReportVariantProperty m_vectorData;

		public ReportVariantProperty VectorData
		{
			get
			{
				if (this.m_vectorData == null && this.MapSpatialDataRegionDef.VectorData != null)
				{
					this.m_vectorData = new ReportVariantProperty(this.MapSpatialDataRegionDef.VectorData);
				}
				return this.m_vectorData;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				return base.m_mapVectorLayer.ReportScope;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion MapSpatialDataRegionDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion)base.MapSpatialDataDef;
			}
		}

		public new MapSpatialDataRegionInstance Instance
		{
			get
			{
				return (MapSpatialDataRegionInstance)this.GetInstance();
			}
		}

		internal MapSpatialDataRegion(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override MapSpatialDataInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapSpatialDataRegionInstance(this);
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
		}
	}
}
