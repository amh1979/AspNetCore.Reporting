using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class ShapefileMapper : SpatialDataMapper
	{
		private MapShapefile m_shapefile;

		private bool m_shapefileMatchingLayer;

		internal ShapefileMapper(VectorLayerMapper vectorLayerMapper, Dictionary<SpatialElementKey, SpatialElementInfoGroup> spatialElementsDictionary, MapControl coreMap, MapMapper mapMapper)
			: base(vectorLayerMapper, spatialElementsDictionary, coreMap, mapMapper)
		{
			this.m_shapefile = (MapShapefile)base.m_mapVectorLayer.MapSpatialData;
		}

		internal void SubscribeToAddedEvent()
		{
			base.m_coreMap.ElementAdded += this.CoreMap_ElementAdded;
		}

		internal void UnsubscribeToAddedEvent()
		{
			base.m_coreMap.ElementAdded -= this.CoreMap_ElementAdded;
		}

		private void CoreMap_ElementAdded(object sender, ElementEventArgs e)
		{
			if (e.MapElement is ISpatialElement)
			{
				if (!this.m_shapefileMatchingLayer)
				{
					if (!base.m_vectorLayerMapper.IsValidSpatialElement((ISpatialElement)e.MapElement))
					{
						throw new RenderingObjectModelException(RPRes.rsMapShapefileTypeMismatch(RPRes.rsObjectTypeMap, base.m_mapVectorLayer.MapDef.Name, base.m_mapVectorLayer.Name, this.m_shapefile.Instance.Source));
					}
					this.m_shapefileMatchingLayer = true;
				}
				SpatialElementInfo spatialElementInfo = new SpatialElementInfo();
				spatialElementInfo.CoreSpatialElement = (ISpatialElement)e.MapElement;
				base.OnSpatialElementAdded(spatialElementInfo);
			}
		}

		internal override void Populate()
		{
			this.SubscribeToAddedEvent();
			string[] columnsToImport = default(string[]);
			string[] destinationFields = default(string[]);
			this.GetFieldNameMapping(out columnsToImport, out destinationFields);
			Stream stream = this.m_shapefile.Instance.Stream;
			if (stream != null)
			{
				base.m_coreMap.mapCore.MaxSpatialElementCount = base.m_mapMapper.RemainingSpatialElementCount;
				base.m_coreMap.mapCore.MaxSpatialPointCount = base.m_mapMapper.RemainingTotalPointCount;
				switch (base.m_coreMap.mapCore.LoadFromShapeFileStreams(stream, this.m_shapefile.Instance.DBFStream, columnsToImport, destinationFields, base.m_mapVectorLayer.Name, base.m_mapVectorLayer.Name))
				{
				case SpatialLoadResult.MaxSpatialElementCountReached:
					base.m_coreMap.Viewport.ErrorMessage = RPRes.rsMapMaximumSpatialElementCountReached(RPRes.rsObjectTypeMap, base.m_mapVectorLayer.MapDef.Name);
					break;
				case SpatialLoadResult.MaxSpatialPointCountReached:
					base.m_coreMap.Viewport.ErrorMessage = RPRes.rsMapMaximumTotalPointCountReached(RPRes.rsObjectTypeMap, base.m_mapVectorLayer.MapDef.Name);
					break;
				}
				this.UnsubscribeToAddedEvent();
				return;
			}
			throw new RenderingObjectModelException(RPRes.rsMapCannotLoadShapefile(RPRes.rsObjectTypeMap, base.m_mapVectorLayer.MapDef.Name, this.m_shapefile.Instance.Source));
		}

		private void GetFieldNameMapping(out string[] dbfNames, out string[] uniqueNames)
		{
			MapFieldNameCollection mapFieldNames = this.m_shapefile.MapFieldNames;
			if (mapFieldNames == null)
			{
				dbfNames = null;
				uniqueNames = null;
			}
			else
			{
				dbfNames = new string[mapFieldNames.Count];
				uniqueNames = new string[mapFieldNames.Count];
				for (int i = 0; i < mapFieldNames.Count; i++)
				{
					string fieldName = base.GetFieldName(((ReportElementCollectionBase<MapFieldName>)mapFieldNames)[i]);
					dbfNames[i] = fieldName;
					uniqueNames[i] = SpatialDataMapper.GetUniqueFieldName(base.m_mapVectorLayer.Name, fieldName);
				}
			}
		}
	}
}
