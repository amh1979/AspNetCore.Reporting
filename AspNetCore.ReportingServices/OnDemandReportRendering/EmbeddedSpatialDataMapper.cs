using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class EmbeddedSpatialDataMapper : SpatialDataMapper
	{
		private ISpatialElementCollection m_embeddedCollection;

		private CoreSpatialElementManager m_spatialElementManager;

		internal EmbeddedSpatialDataMapper(VectorLayerMapper vectorLayerMapper, Dictionary<SpatialElementKey, SpatialElementInfoGroup> spatialElementsDictionary, ISpatialElementCollection embeddedCollection, CoreSpatialElementManager spatialElementManager, MapControl coreMap, MapMapper mapMapper)
			: base(vectorLayerMapper, spatialElementsDictionary, coreMap, mapMapper)
		{
			this.m_spatialElementManager = spatialElementManager;
			this.m_embeddedCollection = embeddedCollection;
		}

		internal override void Populate()
		{
			this.AddFieldDefinitions();
			this.AddSpatialElements();
		}

		private void AddFieldDefinitions()
		{
			MapFieldDefinitionCollection mapFieldDefinitions = base.m_mapVectorLayer.MapFieldDefinitions;
			if (mapFieldDefinitions != null)
			{
				foreach (MapFieldDefinition item in mapFieldDefinitions)
				{
					this.m_spatialElementManager.AddFieldDefinition(base.GetUniqueFieldName(item.Name), this.GetFieldType(item.DataType));
				}
			}
		}

		private void AddSpatialElements()
		{
			for (int i = 0; i < this.m_embeddedCollection.Count; i++)
			{
				this.AddSpatialElement(this.m_embeddedCollection.GetItem(i));
			}
		}

		private void AddSpatialElement(MapSpatialElement embeddedElement)
		{
			if (base.m_mapMapper.CanAddSpatialElement)
			{
				ISpatialElement spatialElement = this.m_spatialElementManager.AddWKB(embeddedElement.VectorData, base.m_mapVectorLayer.Name);
				if (spatialElement != null)
				{
					this.ProcessNonSpatialFields(embeddedElement, spatialElement);
					SpatialElementInfo spatialElementInfo = new SpatialElementInfo();
					spatialElementInfo.CoreSpatialElement = spatialElement;
					spatialElementInfo.MapSpatialElement = embeddedElement;
					base.OnSpatialElementAdded(spatialElementInfo);
				}
			}
		}

		private void ProcessNonSpatialFields(MapSpatialElement embeddedElement, ISpatialElement spatialElement)
		{
			MapFieldCollection mapFields = embeddedElement.MapFields;
			if (mapFields != null)
			{
				MapFieldDefinitionCollection mapFieldDefinitions = base.m_mapVectorLayer.MapFieldDefinitions;
				if (mapFieldDefinitions != null)
				{
					foreach (MapField item in mapFields)
					{
						MapFieldDefinition fieldDefinition = mapFieldDefinitions.GetFieldDefinition(item.Name);
						if (fieldDefinition == null)
						{
							throw new RenderingObjectModelException(RPRes.rsMapInvalidFieldName(RPRes.rsObjectTypeMap, base.m_mapVectorLayer.MapDef.Name, base.m_mapVectorLayer.Name, item.Name));
						}
						this.m_spatialElementManager.AddFieldValue(spatialElement, base.GetUniqueFieldName(item.Name), this.GetFieldValue(item.Value, fieldDefinition.DataType));
					}
				}
			}
		}

		private Type GetFieldType(MapDataType dataType)
		{
			switch (dataType)
			{
			case MapDataType.Boolean:
				return typeof(bool);
			case MapDataType.DateTime:
				return typeof(DateTime);
			case MapDataType.Float:
				return typeof(double);
			case MapDataType.Integer:
				return typeof(int);
			case MapDataType.Decimal:
				return typeof(decimal);
			default:
				return typeof(string);
			}
		}

		private object GetFieldValue(string stringValue, MapDataType dataType)
		{
			try
			{
				switch (dataType)
				{
				case MapDataType.Boolean:
					return Convert.ToBoolean(stringValue, CultureInfo.InvariantCulture);
				case MapDataType.DateTime:
					return Convert.ToDateTime(stringValue, CultureInfo.InvariantCulture);
				case MapDataType.Float:
					return Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
				case MapDataType.Integer:
					return Convert.ToInt32(stringValue, CultureInfo.InvariantCulture);
				case MapDataType.Decimal:
					return Convert.ToDecimal(stringValue, CultureInfo.InvariantCulture);
				default:
					return stringValue;
				}
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
		}
	}
}
