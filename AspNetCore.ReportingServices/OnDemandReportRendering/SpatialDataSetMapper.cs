using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.ReportProcessing;

using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class SpatialDataSetMapper : SpatialDataMapper
	{
		private class FieldInfo
		{
			public string UniqueName;

			public int Index;

			public bool DefinitionAdded;
		}

		private CoreSpatialElementManager m_spatialElementManager;

		private MapSpatialDataSet m_spatialDataSet;

		private DataSet m_dataSet;

		private DataSetInstance m_dataSetInstance;

		internal SpatialDataSetMapper(VectorLayerMapper vectorLayerMapper, Dictionary<SpatialElementKey, SpatialElementInfoGroup> spatialElementsDictionary, CoreSpatialElementManager spatialElementManager, MapControl coreMap, MapMapper mapMapper)
			: base(vectorLayerMapper, spatialElementsDictionary, coreMap, mapMapper)
		{
			this.m_spatialElementManager = spatialElementManager;
			this.m_spatialDataSet = (MapSpatialDataSet)base.m_mapVectorLayer.MapSpatialData;
			this.m_dataSet = this.m_spatialDataSet.DataSet;
			this.m_dataSetInstance = this.m_dataSet.Instance;
		}

		internal override void Populate()
		{
			int spatialFieldIndex = this.GetSpatialFieldIndex();
			FieldInfo[] nonSpatialFieldInfos = this.GetNonSpatialFieldInfos();
			this.m_dataSetInstance.ResetContext();
			while (this.m_dataSetInstance.MoveNext())
			{
				this.ProcessRow(spatialFieldIndex, nonSpatialFieldInfos);
			}
			this.EnsureFieldDefinitionsCreated(nonSpatialFieldInfos);
			this.m_dataSetInstance.Close();
		}

		private void EnsureFieldDefinitionsCreated(FieldInfo[] nonSpatialFieldInfos)
		{
			if (nonSpatialFieldInfos != null)
			{
				foreach (FieldInfo fieldInfo in nonSpatialFieldInfos)
				{
					if (!fieldInfo.DefinitionAdded)
					{
						this.m_spatialElementManager.AddFieldDefinition(fieldInfo.UniqueName, typeof(string));
					}
				}
			}
		}

		private void ProcessRow(int spatialFieldIndex, FieldInfo[] nonSpatialFieldInfos)
		{
            if (!this.m_mapMapper.CanAddSpatialElement)
            {
                return;
            }
            /*
            RowInstance row = this.m_dataSetInstance.Row;
            object value = row[spatialFieldIndex].Value;
            ISpatialElement spatialElement;
            if (value is SqlGeography)
            {
                spatialElement = this.m_spatialElementManager.AddGeography((SqlGeography)value, this.m_mapVectorLayer.Name);
            }
            else
            {
                if (!(value is SqlGeometry))
                {
                    throw new RenderingObjectModelException(RPRes.rsMapInvalidSpatialFieldType(RPRes.rsObjectTypeMap, this.m_mapVectorLayer.MapDef.Name, this.m_mapVectorLayer.Name));
                }
                spatialElement = this.m_spatialElementManager.AddGeometry((SqlGeometry)value, this.m_mapVectorLayer.Name);
            }
            if (spatialElement != null)
            {
                this.ProcessNonSpatialFields(spatialElement, nonSpatialFieldInfos);
                base.OnSpatialElementAdded(new SpatialElementInfo
                {
                    CoreSpatialElement = spatialElement
                });
            }
            */
        }

		private void ProcessNonSpatialFields(ISpatialElement spatialElement, FieldInfo[] nonSpatialFieldInfos)
		{
			if (nonSpatialFieldInfos != null)
			{
				foreach (FieldInfo fieldInfo in nonSpatialFieldInfos)
				{
					if (!fieldInfo.DefinitionAdded)
					{
						fieldInfo.DefinitionAdded = this.AddFieldDefinition(fieldInfo.UniqueName, this.m_dataSetInstance.Row[fieldInfo.Index].Value);
					}
					this.m_spatialElementManager.AddFieldValue(spatialElement, fieldInfo.UniqueName, this.m_dataSetInstance.Row[fieldInfo.Index].Value);
				}
			}
		}

		private bool AddFieldDefinition(string fieldUniqueName, object value)
		{
			if (value != null)
			{
				this.m_spatialElementManager.AddFieldDefinition(fieldUniqueName, CoreSpatialElementManager.GetFieldType(value));
				return true;
			}
			return false;
		}

		private FieldInfo[] GetNonSpatialFieldInfos()
		{
			MapFieldNameCollection mapFieldNames = this.m_spatialDataSet.MapFieldNames;
			if (mapFieldNames == null)
			{
				return null;
			}
			FieldInfo[] array = new FieldInfo[mapFieldNames.Count];
			for (int i = 0; i < mapFieldNames.Count; i++)
			{
				FieldInfo fieldInfo = new FieldInfo();
				string fieldName = base.GetFieldName(((ReportElementCollectionBase<MapFieldName>)mapFieldNames)[i]);
				fieldInfo.UniqueName = base.GetUniqueFieldName(fieldName);
				fieldInfo.Index = this.GetFieldIndex(fieldName);
				fieldInfo.DefinitionAdded = false;
				array[i] = fieldInfo;
			}
			return array;
		}

		private int GetSpatialFieldIndex()
		{
			return this.GetFieldIndex(this.GetSpatialFieldName());
		}

		private int GetFieldIndex(string fieldName)
		{
			int fieldIndex = this.m_dataSet.NonCalculatedFields.GetFieldIndex(fieldName);
			if (fieldIndex == -1)
			{
				throw new RenderingObjectModelException(RPRes.rsMapInvalidFieldName(RPRes.rsObjectTypeMap, base.m_mapVectorLayer.MapDef.Name, base.m_mapVectorLayer.Name, fieldName));
			}
			return fieldIndex;
		}

		private string GetSpatialFieldName()
		{
			ReportStringProperty spatialField = this.m_spatialDataSet.SpatialField;
			if (!spatialField.IsExpression)
			{
				return spatialField.Value;
			}
			return this.m_spatialDataSet.Instance.SpatialField;
		}
	}
}
