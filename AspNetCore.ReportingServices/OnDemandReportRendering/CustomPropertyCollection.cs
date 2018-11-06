using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CustomPropertyCollection : ReportElementCollectionBase<CustomProperty>
	{
		private List<CustomProperty> m_list;

		private Dictionary<string, CustomProperty> m_lookupTable;

		private ReportElement m_reportElementOwner;

		public CustomProperty this[string name]
		{
			get
			{
				if (name != null && this.m_lookupTable != null)
				{
					CustomProperty result = null;
					if (this.m_lookupTable.TryGetValue(name, out result))
					{
						return result;
					}
				}
				return null;
			}
		}

		public override CustomProperty this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_list[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		internal CustomPropertyCollection()
		{
			this.m_list = new List<CustomProperty>();
		}

		internal CustomPropertyCollection(IReportScopeInstance romInstance, RenderingContext renderingContext, ReportElement reportElementOwner, ICustomPropertiesHolder customPropertiesHolder, ObjectType objectType, string objectName)
		{
			this.m_reportElementOwner = reportElementOwner;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList customProperties = customPropertiesHolder.CustomProperties;
			if (customProperties == null)
			{
				this.m_list = new List<CustomProperty>();
			}
			else
			{
				bool flag = InstancePathItem.IsValidContext(customPropertiesHolder.InstancePath.InstancePath);
				int count = customProperties.Count;
				this.m_list = new List<CustomProperty>(count);
				for (int i = 0; i < count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataValue dataValue = customProperties[i];
					string name = null;
					object value = null;
					TypeCode typeCode = TypeCode.Empty;
					if (flag)
					{
						dataValue.EvaluateNameAndValue(this.m_reportElementOwner, romInstance, customPropertiesHolder.InstancePath, renderingContext.OdpContext, objectType, objectName, out name, out value, out typeCode);
					}
					CustomProperty customProperty = new CustomProperty(this.m_reportElementOwner, renderingContext, dataValue.Name, dataValue.Value, name, value, typeCode);
					this.m_list.Add(customProperty);
					if (flag)
					{
						this.AddPropToLookupTable(name, customProperty);
					}
				}
			}
		}

		internal CustomPropertyCollection(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportRendering.CustomPropertyCollection collection)
		{
			if (collection == null)
			{
				this.m_list = new List<CustomProperty>();
			}
			else
			{
				int count = collection.Count;
				this.m_list = new List<CustomProperty>(count);
				for (int i = 0; i < count; i++)
				{
					AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo nameExpr = default(AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo);
					AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo valueExpr = default(AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo);
					string name = default(string);
					object value = default(object);
					collection.GetNameValueExpressions(i, out nameExpr, out valueExpr, out name, out value);
					CustomProperty customProperty = new CustomProperty(renderingContext, nameExpr, valueExpr, name, value, TypeCode.Empty);
					this.m_list.Add(customProperty);
					this.AddPropToLookupTable(name, customProperty);
				}
			}
		}

		internal CustomProperty Add(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo nameExpr, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo valueExpr)
		{
			CustomProperty customProperty = new CustomProperty(this.m_reportElementOwner, renderingContext, nameExpr, valueExpr, null, null, TypeCode.Empty);
			Global.Tracer.Assert(customProperty.Instance != null, "prop.Instance != null");
			this.m_list.Add(customProperty);
			return customProperty;
		}

		internal void UpdateCustomProperties(AspNetCore.ReportingServices.ReportRendering.CustomPropertyCollection collection)
		{
			int count = this.m_list.Count;
			for (int i = 0; i < count; i++)
			{
				string name = null;
				object value = null;
				if (collection != null)
				{
					collection.GetNameValue(i, out name, out value);
				}
				this.m_list[i].Update(name, value, TypeCode.Empty);
			}
		}

		internal void UpdateCustomProperties(IReportScopeInstance romInstance, ICustomPropertiesHolder customPropertiesHolder, OnDemandProcessingContext context, ObjectType objectType, string objectName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList customProperties = customPropertiesHolder.CustomProperties;
			int count = this.m_list.Count;
			bool flag = false;
			if (this.m_lookupTable == null)
			{
				flag = true;
			}
			for (int i = 0; i < count; i++)
			{
				string name = null;
				object value = null;
				TypeCode typeCode = TypeCode.Empty;
				if (customProperties != null && i < customProperties.Count)
				{
					customProperties[i].EvaluateNameAndValue(this.m_reportElementOwner, romInstance, customPropertiesHolder.InstancePath, context, objectType, objectName, out name, out value, out typeCode);
				}
				this.m_list[i].Update(name, value, typeCode);
				if (flag)
				{
					this.AddPropToLookupTable(name, this.m_list[i]);
				}
			}
		}

		private void AddPropToLookupTable(string name, CustomProperty property)
		{
			if (this.m_lookupTable == null)
			{
				this.m_lookupTable = new Dictionary<string, CustomProperty>(this.m_list.Count);
			}
			if (name != null && !this.m_lookupTable.ContainsKey(name))
			{
				this.m_lookupTable.Add(name, property);
			}
		}

		internal void ConstructCustomPropertyDefinitions(AspNetCore.ReportingServices.ReportIntermediateFormat.DataValueList dataValueDefs)
		{
			Global.Tracer.Assert(dataValueDefs != null && this.m_list.Count == dataValueDefs.Count, "m_list.Count == dataValueDefs.Count");
			for (int i = 0; i < this.m_list.Count; i++)
			{
				CustomProperty customProperty = this.m_list[i];
				customProperty.ConstructCustomPropertyDefinition(dataValueDefs[i]);
				if (customProperty.Instance != null && customProperty.Instance.Name != null)
				{
					this.AddPropToLookupTable(customProperty.Instance.Name, customProperty);
				}
			}
		}

		internal void GetDynamicValues(out List<string> customPropertyNames, out List<object> customPropertyValues)
		{
			customPropertyNames = new List<string>(this.m_list.Count);
			customPropertyValues = new List<object>(this.m_list.Count);
			bool flag = false;
			for (int i = 0; i < this.m_list.Count; i++)
			{
				CustomProperty customProperty = this.m_list[i];
				string item = null;
				if (customProperty.Name.IsExpression)
				{
					flag = true;
					item = customProperty.Instance.Name;
				}
				object item2 = null;
				if (customProperty.Value.IsExpression)
				{
					flag = true;
					item2 = customProperty.Instance.Value;
				}
				customPropertyNames.Add(item);
				customPropertyValues.Add(item2);
			}
			if (!flag)
			{
				customPropertyNames = null;
				customPropertyValues = null;
			}
		}

		internal void SetDynamicValues(List<string> customPropertyNames, List<object> customPropertyValues)
		{
			if (customPropertyNames == null && customPropertyValues == null)
			{
				return;
			}
			Global.Tracer.Assert(customPropertyNames != null && customPropertyValues != null && customPropertyNames.Count == customPropertyValues.Count && this.m_list.Count == customPropertyNames.Count, "Chck customPropertyNames and customPropertyValues consistency");
			for (int i = 0; i < this.m_list.Count; i++)
			{
				CustomProperty customProperty = this.m_list[i];
				if (customProperty.Name.IsExpression)
				{
					customProperty.Instance.Name = customPropertyNames[i];
				}
				if (customProperty.Value.IsExpression)
				{
					customProperty.Instance.Value = customPropertyValues[i];
				}
			}
		}
	}
}
