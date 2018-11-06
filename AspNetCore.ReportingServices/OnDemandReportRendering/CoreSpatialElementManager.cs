using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportProcessing;
//
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class CoreSpatialElementManager
	{
		protected MapControl m_coreMap;

		protected MapVectorLayer m_mapVectorLayer;

		internal abstract AspNetCore.Reporting.Map.WebForms.FieldCollection FieldDefinitions
		{
			get;
		}

		protected abstract NamedCollection SpatialElements
		{
			get;
		}

		internal CoreSpatialElementManager(MapControl coreMap, MapVectorLayer mapVectorLayer)
		{
			this.m_coreMap = coreMap;
			this.m_mapVectorLayer = mapVectorLayer;
		}
        /*
		internal ISpatialElement AddGeography(SqlGeography geography, string layerName)
		{
			if (geography == null)
			{
				return null;
			}
			if (geography.IsNull)
			{
				return null;
			}
			ISpatialElement spatialElement = this.CreateSpatialElement();
			spatialElement.Layer = layerName;
			spatialElement.Category = layerName;
			this.AddSpatialElement(spatialElement);
			if (!spatialElement.AddGeography(geography))
			{
				this.RemoveSpatialElement(spatialElement);
				return null;
			}
			return spatialElement;
		}
        
		internal ISpatialElement AddGeometry(SqlGeometry geometry, string layerName)
		{
			if (geometry == null)
			{
				return null;
			}
			if (geometry.IsNull)
			{
				return null;
			}
			ISpatialElement spatialElement = this.CreateSpatialElement();
			spatialElement.Layer = layerName;
			spatialElement.Category = layerName;
			if (spatialElement.AddGeometry(geometry))
			{
				this.AddSpatialElement(spatialElement);
				return spatialElement;
			}
			return null;
		}
       */
        internal ISpatialElement AddWKB(string wkb, string layerName)
		{
			ISpatialElement spatialElement = this.CreateSpatialElement();
			spatialElement.Layer = layerName;
			spatialElement.Category = layerName;
			if (spatialElement.AddWKB(Convert.FromBase64String(wkb)))
			{
				this.AddSpatialElement(spatialElement);
				return spatialElement;
			}
			return null;
		}

		internal static Type GetFieldType(object value)
		{
			Type type = value.GetType();
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
				return typeof(int);
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Decimal:
				return typeof(decimal);
			case TypeCode.Single:
			case TypeCode.Double:
				return typeof(double);
			case TypeCode.DateTime:
				return typeof(DateTime);
			case TypeCode.Boolean:
				return typeof(bool);
			default:
				return typeof(string);
			}
		}

		internal void AddFieldDefinition(string fieldName, Type type)
		{
			AspNetCore.Reporting.Map.WebForms.Field field = new AspNetCore.Reporting.Map.WebForms.Field();
			field.Name = fieldName;
			field.Type = type;
			this.FieldDefinitions.Add(field);
		}

		internal void AddFieldValue(ISpatialElement spatialElement, string fieldName, object value)
		{
			try
			{
				spatialElement[fieldName] = value;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				spatialElement[fieldName] = value.ToString();
				this.m_mapVectorLayer.MapDef.RenderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsMapUnsupportedValueFieldType, Severity.Warning, this.m_mapVectorLayer.MapDef.MapDef.ObjectType, this.m_mapVectorLayer.MapDef.Name, this.m_mapVectorLayer.Name, fieldName);
			}
		}

		internal string AddRuleField(object dataValue)
		{
			string text = this.GenerateUniqueFieldName();
			this.AddFieldDefinition(text, CoreSpatialElementManager.GetFieldType(dataValue));
			return text;
		}

		private string GenerateUniqueFieldName()
		{
			int num = this.FieldDefinitions.Count;
			string text;
			while (true)
			{
				text = num.ToString(CultureInfo.InvariantCulture);
				if (this.FieldDefinitions.GetByName(text) != null)
				{
					num++;
					continue;
				}
				break;
			}
			return text;
		}

		internal int GetSpatialElementCount()
		{
			int num = 0;
			foreach (ISpatialElement spatialElement in this.SpatialElements)
			{
				if (!(spatialElement.Layer != this.m_mapVectorLayer.Name))
				{
					num++;
				}
			}
			return num;
		}

		internal int GetDistinctValuesCount(string fieldName)
		{
			NamedCollection spatialElements = this.SpatialElements;
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			foreach (ISpatialElement item in spatialElements)
			{
				if (!(item.Layer != this.m_mapVectorLayer.Name))
				{
					object obj = item[fieldName];
					if (obj != null && !dictionary.ContainsKey(obj))
					{
						dictionary.Add(obj, null);
					}
				}
			}
			return dictionary.Count;
		}

		internal abstract void AddSpatialElement(ISpatialElement spatialElement);

		internal abstract void RemoveSpatialElement(ISpatialElement spatialElement);

		internal abstract ISpatialElement CreateSpatialElement();
	}
}
