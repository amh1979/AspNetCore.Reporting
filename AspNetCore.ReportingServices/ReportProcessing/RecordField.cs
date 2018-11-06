using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordField
	{
		private object m_fieldValue;

		private bool m_isAggregationField;

		private VariantList m_fieldPropertyValues;

		[NonSerialized]
		private DataFieldStatus m_fieldStatus;

		[NonSerialized]
		private Hashtable m_properties;

		internal object FieldValue
		{
			get
			{
				return this.m_fieldValue;
			}
			set
			{
				this.m_fieldValue = value;
			}
		}

		internal bool IsAggregationField
		{
			get
			{
				return this.m_isAggregationField;
			}
			set
			{
				this.m_isAggregationField = value;
			}
		}

		internal VariantList FieldPropertyValues
		{
			get
			{
				return this.m_fieldPropertyValues;
			}
			set
			{
				this.m_fieldPropertyValues = value;
			}
		}

		internal bool IsOverflow
		{
			get
			{
				return DataFieldStatus.Overflow == this.m_fieldStatus;
			}
		}

		internal bool IsUnSupportedDataType
		{
			get
			{
				return DataFieldStatus.UnSupportedDataType == this.m_fieldStatus;
			}
		}

		internal bool IsError
		{
			get
			{
				return DataFieldStatus.IsError == this.m_fieldStatus;
			}
		}

		internal DataFieldStatus FieldStatus
		{
			get
			{
				return this.m_fieldStatus;
			}
			set
			{
				Global.Tracer.Assert(DataFieldStatus.None == this.m_fieldStatus);
				this.m_fieldStatus = value;
			}
		}

		internal RecordField(FieldImpl field)
		{
			this.m_fieldStatus = field.FieldStatus;
			this.m_properties = field.Properties;
			if (this.m_fieldStatus == DataFieldStatus.None)
			{
				this.m_fieldValue = field.Value;
				this.m_isAggregationField = field.IsAggregationField;
			}
		}

		internal RecordField()
		{
		}

		internal void SetProperty(string propertyName, object propertyValue)
		{
			if (this.m_properties == null)
			{
				this.m_properties = new Hashtable();
			}
			this.m_properties[propertyName] = propertyValue;
		}

		internal object GetProperty(string propertyName)
		{
			Global.Tracer.Assert(this.m_properties != null);
			return this.m_properties[propertyName];
		}

		internal void PopulateFieldPropertyValues(StringList propertyNames)
		{
			if (propertyNames != null)
			{
				int count = propertyNames.Count;
				this.m_fieldPropertyValues = new VariantList(count);
				for (int i = 0; i < count; i++)
				{
					object value = null;
					if (this.m_properties != null)
					{
						value = this.m_properties[propertyNames[i]];
					}
					this.m_fieldPropertyValues.Add(value);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.FieldValue, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.IsAggregateField, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.FieldPropertyValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.VariantList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
