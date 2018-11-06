using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapVectorLayer : MapLayer
	{
		private IReportScope m_reportScope;

		private MapBindingFieldPairCollection m_mapBindingFieldPairs;

		private MapFieldDefinitionCollection m_mapFieldDefinitions;

		private MapSpatialData m_mapSpatialData;

		private MapDataRegion m_mapDataRegion;

		public string DataElementName
		{
			get
			{
				return this.MapVectorLayerDef.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.MapVectorLayerDef.DataElementOutput;
			}
		}

		public string MapDataRegionName
		{
			get
			{
				return this.MapVectorLayerDef.MapDataRegionName;
			}
		}

		public MapBindingFieldPairCollection MapBindingFieldPairs
		{
			get
			{
				if (this.m_mapBindingFieldPairs == null && this.MapVectorLayerDef.MapBindingFieldPairs != null)
				{
					this.m_mapBindingFieldPairs = new MapBindingFieldPairCollection(this, base.m_map);
				}
				return this.m_mapBindingFieldPairs;
			}
		}

		public MapFieldDefinitionCollection MapFieldDefinitions
		{
			get
			{
				if (this.m_mapFieldDefinitions == null && this.MapVectorLayerDef.MapFieldDefinitions != null)
				{
					this.m_mapFieldDefinitions = new MapFieldDefinitionCollection(this, base.m_map);
				}
				return this.m_mapFieldDefinitions;
			}
		}

		public MapSpatialData MapSpatialData
		{
			get
			{
				if (this.m_mapSpatialData == null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialData mapSpatialData = this.MapVectorLayerDef.MapSpatialData;
					if (mapSpatialData != null)
					{
						if (mapSpatialData is AspNetCore.ReportingServices.ReportIntermediateFormat.MapShapefile)
						{
							this.m_mapSpatialData = new MapShapefile(this, base.m_map);
						}
						else if (mapSpatialData is AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataSet)
						{
							this.m_mapSpatialData = new MapSpatialDataSet(this, base.m_map);
						}
						else if (mapSpatialData is AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialDataRegion)
						{
							this.m_mapSpatialData = new MapSpatialDataRegion(this, base.m_map);
						}
					}
				}
				return this.m_mapSpatialData;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (this.m_reportScope == null)
				{
					if (this.MapDataRegionName != null)
					{
						this.m_reportScope = base.m_map.MapDataRegions[this.MapDataRegionName].InnerMostMapMember;
					}
					else
					{
						this.m_reportScope = base.m_map.ReportScope;
					}
				}
				return this.m_reportScope;
			}
		}

		public MapDataRegion MapDataRegion
		{
			get
			{
				if (this.MapDataRegionName != null && this.m_mapDataRegion == null)
				{
					this.m_mapDataRegion = base.MapDef.MapDataRegions[this.MapDataRegionName];
				}
				return this.m_mapDataRegion;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer MapVectorLayerDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer)base.MapLayerDef;
			}
		}

		internal new MapVectorLayerInstance Instance
		{
			get
			{
				return (MapVectorLayerInstance)this.GetInstance();
			}
		}

		internal MapVectorLayer(AspNetCore.ReportingServices.ReportIntermediateFormat.MapVectorLayer defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapBindingFieldPairs != null)
			{
				this.m_mapBindingFieldPairs.SetNewContext();
			}
			if (this.m_mapFieldDefinitions != null)
			{
				this.m_mapFieldDefinitions.SetNewContext();
			}
			if (this.m_mapSpatialData != null)
			{
				this.m_mapSpatialData.SetNewContext();
			}
		}
	}
}
