using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ExtendedPropertyCollection : ReportElementCollectionBase<ExtendedProperty>
	{
		private ExtendedProperty[] m_extendedProperties;

		private List<string> m_extendedPropertyNames;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField m_recordField;

		public override int Count
		{
			get
			{
				if (this.m_extendedPropertyNames == null)
				{
					return 0;
				}
				return this.m_extendedPropertyNames.Count;
			}
		}

		public override ExtendedProperty this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_extendedProperties == null)
					{
						this.m_extendedProperties = new ExtendedProperty[this.Count];
					}
					if (this.m_extendedProperties[index] == null)
					{
						this.m_extendedProperties[index] = new ExtendedProperty(this.m_extendedPropertyNames[index], this.GetFieldPropertyValue(index));
					}
					return this.m_extendedProperties[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public ExtendedProperty this[string name]
		{
			get
			{
				if (this.m_extendedPropertyNames != null)
				{
					return ((ReportElementCollectionBase<ExtendedProperty>)this)[this.m_extendedPropertyNames.IndexOf(name)];
				}
				return null;
			}
		}

		internal ExtendedPropertyCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField field, List<string> extendedPropertyNames)
		{
			this.m_recordField = field;
			this.m_extendedPropertyNames = extendedPropertyNames;
		}

		internal void UpdateRecordField(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordField field)
		{
			this.m_recordField = field;
			if (this.m_extendedProperties != null)
			{
				for (int i = 0; i < this.m_extendedProperties.Length; i++)
				{
					ExtendedProperty extendedProperty = this.m_extendedProperties[i];
					if (extendedProperty != null)
					{
						extendedProperty.UpdateValue(this.GetFieldPropertyValue(i));
					}
				}
			}
		}

		private object GetFieldPropertyValue(int index)
		{
			if (this.m_recordField == null)
			{
				return null;
			}
			return this.m_recordField.FieldPropertyValues[index];
		}
	}
}
