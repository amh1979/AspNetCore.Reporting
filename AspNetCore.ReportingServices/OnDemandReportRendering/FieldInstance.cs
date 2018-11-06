using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class FieldInstance
	{
		private RecordField m_recordField;

		private FieldInfo m_fieldInfo;

		private ExtendedPropertyCollection m_extendedProperties;

		public object Value
		{
			get
			{
				if (this.IsMissingRecord)
				{
					return null;
				}
				return this.m_recordField.FieldValue;
			}
		}

		public bool IsAggregationField
		{
			get
			{
				if (this.IsMissingRecord)
				{
					return false;
				}
				return this.m_recordField.IsAggregationField;
			}
		}

		public bool IsOverflow
		{
			get
			{
				if (this.IsMissingRecord)
				{
					return false;
				}
				return this.m_recordField.IsOverflow;
			}
		}

		public bool IsUnSupportedDataType
		{
			get
			{
				if (this.IsMissingRecord)
				{
					return false;
				}
				return this.m_recordField.IsUnSupportedDataType;
			}
		}

		public bool IsError
		{
			get
			{
				if (this.IsMissingRecord)
				{
					return true;
				}
				return this.m_recordField.IsError;
			}
		}

		public ExtendedPropertyCollection ExtendedProperties
		{
			get
			{
				if (this.m_extendedProperties == null && this.m_fieldInfo != null)
				{
					this.m_extendedProperties = new ExtendedPropertyCollection(this.m_recordField, this.m_fieldInfo.PropertyNames);
				}
				return this.m_extendedProperties;
			}
		}

		private bool IsMissingRecord
		{
			get
			{
				return this.m_recordField == null;
			}
		}

		internal FieldInstance(FieldInfo fieldInfo, RecordField recordField)
		{
			this.m_fieldInfo = fieldInfo;
			this.m_recordField = recordField;
		}

		internal void UpdateRecordField(RecordField field)
		{
			this.m_recordField = field;
			if (this.m_extendedProperties != null)
			{
				this.m_extendedProperties.UpdateRecordField(this.m_recordField);
			}
		}
	}
}
