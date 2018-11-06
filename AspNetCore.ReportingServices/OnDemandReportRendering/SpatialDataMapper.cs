using AspNetCore.Reporting.Map.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class SpatialDataMapper
	{
		protected MapMapper m_mapMapper;

		protected MapVectorLayer m_mapVectorLayer;

		protected VectorLayerMapper m_vectorLayerMapper;

		protected MapControl m_coreMap;

		private List<Type> m_keyTypes;

		private Dictionary<SpatialElementKey, SpatialElementInfoGroup> m_spatialElementsDictionary;

		internal List<Type> KeyTypes
		{
			get
			{
				return this.m_keyTypes;
			}
		}

		internal SpatialDataMapper(VectorLayerMapper vectorLayerMapper, Dictionary<SpatialElementKey, SpatialElementInfoGroup> spatialElementsDictionary, MapControl coreMap, MapMapper mapMapper)
		{
			this.m_vectorLayerMapper = vectorLayerMapper;
			this.m_mapVectorLayer = this.m_vectorLayerMapper.m_mapVectorLayer;
			this.m_spatialElementsDictionary = spatialElementsDictionary;
			this.m_coreMap = coreMap;
			this.m_mapMapper = mapMapper;
		}

		internal abstract void Populate();

		protected void OnSpatialElementAdded(SpatialElementInfo spatialElementInfo)
		{
			this.m_vectorLayerMapper.OnSpatialElementAdded(spatialElementInfo.CoreSpatialElement);
			this.m_mapMapper.OnSpatialElementAdded(spatialElementInfo);
			SpatialElementKey spatialElementKey = this.CreateCoreSpatialElementKey(spatialElementInfo.CoreSpatialElement);
			if (this.m_mapVectorLayer.MapDataRegion != null && this.m_keyTypes == null && spatialElementKey.KeyValues != null)
			{
				this.RegisterKeyTypes(spatialElementKey);
			}
			SpatialElementInfoGroup spatialElementInfoGroup;
			if (!this.m_spatialElementsDictionary.ContainsKey(spatialElementKey))
			{
				spatialElementInfoGroup = new SpatialElementInfoGroup();
				this.m_spatialElementsDictionary.Add(spatialElementKey, spatialElementInfoGroup);
			}
			else
			{
				spatialElementInfoGroup = this.m_spatialElementsDictionary[spatialElementKey];
			}
			spatialElementInfoGroup.Elements.Add(spatialElementInfo);
		}

		private void RegisterKeyTypes(SpatialElementKey key)
		{
			this.m_keyTypes = new List<Type>();
			foreach (object keyValue in key.KeyValues)
			{
				if (keyValue == null)
				{
					this.m_keyTypes.Add(null);
				}
				else
				{
					this.m_keyTypes.Add(keyValue.GetType());
				}
			}
		}

		private SpatialElementKey CreateCoreSpatialElementKey(ISpatialElement coreSpatialElement)
		{
			return SpatialDataMapper.CreateCoreSpatialElementKey(coreSpatialElement, this.m_mapVectorLayer.MapBindingFieldPairs, this.m_mapVectorLayer.MapDef.Name, this.m_mapVectorLayer.Name);
		}

		internal static SpatialElementKey CreateCoreSpatialElementKey(ISpatialElement coreSpatialElement, MapBindingFieldPairCollection mapBindingFieldPairs, string mapName, string layerName)
		{
			if (mapBindingFieldPairs == null)
			{
				return new SpatialElementKey(null);
			}
			List<object> list = new List<object>();
			for (int i = 0; i < mapBindingFieldPairs.Count; i++)
			{
				list.Add(SpatialDataMapper.GetBindingFieldValue(coreSpatialElement, ((ReportElementCollectionBase<MapBindingFieldPair>)mapBindingFieldPairs)[i], mapName, layerName));
			}
			return new SpatialElementKey(list);
		}

		private static object GetBindingFieldValue(ISpatialElement coreSpatialElement, MapBindingFieldPair bindingFieldPair, string mapName, string layerName)
		{
			string bindingFieldName = SpatialDataMapper.GetBindingFieldName(bindingFieldPair);
			if (bindingFieldName == null)
			{
				return null;
			}
			return coreSpatialElement[SpatialDataMapper.GetUniqueFieldName(layerName, bindingFieldName)];
		}

		protected string GetUniqueFieldName(string fieldName)
		{
			return SpatialDataMapper.GetUniqueFieldName(this.m_mapVectorLayer.Name, fieldName);
		}

		internal static string GetBindingFieldName(MapBindingFieldPair bindingFieldPair)
		{
			ReportStringProperty fieldName = bindingFieldPair.FieldName;
			if (!fieldName.IsExpression)
			{
				return fieldName.Value;
			}
			return bindingFieldPair.Instance.FieldName;
		}

		internal static string GetUniqueFieldName(string layerName, string fieldName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", layerName, fieldName);
		}

		protected string GetFieldName(MapFieldName fieldName)
		{
			ReportStringProperty name = fieldName.Name;
			if (!fieldName.Name.IsExpression)
			{
				return fieldName.Name.Value;
			}
			return fieldName.Instance.Name;
		}
	}
}
