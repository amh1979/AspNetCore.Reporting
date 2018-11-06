using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class FieldImpl : Field
	{
		private object m_value;

		private bool m_isAggregationField;

		private bool m_aggregationFieldChecked;

		private DataFieldStatus m_fieldStatus;

		private string m_exceptionMessage;

		private Hashtable m_properties;

		private AspNetCore.ReportingServices.ReportProcessing.Field m_fieldDef;

		private bool m_usedInExpression;

		public override object this[string key]
		{
			get
			{
				if (key == null)
				{
					return null;
				}
				if (ReportProcessing.CompareWithInvariantCulture(key, "Value", true) == 0)
				{
					return this.Value;
				}
				if (ReportProcessing.CompareWithInvariantCulture(key, "IsMissing", true) == 0)
				{
					return this.IsMissing;
				}
				if (ReportProcessing.CompareWithInvariantCulture(key, "LevelNumber", true) == 0)
				{
					return this.LevelNumber;
				}
				return this.GetProperty(key);
			}
		}

		public override object Value
		{
			get
			{
				if (this.m_fieldStatus == DataFieldStatus.None)
				{
					if (this.m_value is CalculatedFieldWrapper)
					{
						return ((CalculatedFieldWrapper)this.m_value).Value;
					}
					return this.m_value;
				}
				throw new ReportProcessingException_FieldError(this.m_fieldStatus, this.m_exceptionMessage);
			}
		}

		public override bool IsMissing
		{
			get
			{
				return DataFieldStatus.IsMissing == this.m_fieldStatus;
			}
		}

		public override string UniqueName
		{
			get
			{
				return this.GetProperty("UniqueName") as string;
			}
		}

		public override string BackgroundColor
		{
			get
			{
				return this.GetProperty("BackgroundColor") as string;
			}
		}

		public override string Color
		{
			get
			{
				return this.GetProperty("Color") as string;
			}
		}

		public override string FontFamily
		{
			get
			{
				return this.GetProperty("FontFamily") as string;
			}
		}

		public override string FontSize
		{
			get
			{
				return this.GetProperty("FontSize") as string;
			}
		}

		public override string FontWeight
		{
			get
			{
				return this.GetProperty("FontWeight") as string;
			}
		}

		public override string FontStyle
		{
			get
			{
				return this.GetProperty("FontStyle") as string;
			}
		}

		public override string TextDecoration
		{
			get
			{
				return this.GetProperty("TextDecoration") as string;
			}
		}

		public override string FormattedValue
		{
			get
			{
				return this.GetProperty("FormattedValue") as string;
			}
		}

		public override object Key
		{
			get
			{
				return this.GetProperty("Key");
			}
		}

		public override int LevelNumber
		{
			get
			{
				object property = this.GetProperty("LevelNumber");
				if (property == null)
				{
					return 0;
				}
				if (property is int)
				{
					return (int)property;
				}
				bool flag = default(bool);
				int result = DataTypeUtility.ConvertToInt32(DataAggregate.GetTypeCode(property), property, out flag);
				if (flag)
				{
					return result;
				}
				return 0;
			}
		}

		public override string ParentUniqueName
		{
			get
			{
				return this.GetProperty("ParentUniqueName") as string;
			}
		}

		internal DataFieldStatus FieldStatus
		{
			get
			{
				return this.m_fieldStatus;
			}
		}

		internal string ExceptionMessage
		{
			get
			{
				return this.m_exceptionMessage;
			}
		}

		internal bool IsAggregationField
		{
			get
			{
				return this.m_isAggregationField;
			}
		}

		internal bool AggregationFieldChecked
		{
			get
			{
				return this.m_aggregationFieldChecked;
			}
			set
			{
				this.m_aggregationFieldChecked = value;
			}
		}

		internal new Hashtable Properties
		{
			get
			{
				return this.m_properties;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.Field FieldDef
		{
			get
			{
				return this.m_fieldDef;
			}
		}

		internal bool UsedInExpression
		{
			get
			{
				return this.m_usedInExpression;
			}
			set
			{
				this.m_usedInExpression = value;
			}
		}

		internal FieldImpl(object value, bool isAggregationField, AspNetCore.ReportingServices.ReportProcessing.Field fieldDef)
		{
			this.m_value = value;
			this.m_isAggregationField = isAggregationField;
			this.m_aggregationFieldChecked = false;
			this.m_fieldStatus = DataFieldStatus.None;
			this.m_fieldDef = fieldDef;
			this.m_usedInExpression = false;
		}

		internal FieldImpl(DataFieldStatus status, string exceptionMessage, AspNetCore.ReportingServices.ReportProcessing.Field fieldDef)
		{
			this.m_value = null;
			this.m_isAggregationField = false;
			this.m_aggregationFieldChecked = false;
			Global.Tracer.Assert(DataFieldStatus.None != status, "(DataFieldStatus.None != status)");
			this.m_fieldStatus = status;
			this.m_exceptionMessage = exceptionMessage;
			this.m_fieldDef = fieldDef;
			this.m_usedInExpression = false;
		}

		internal void SetValue(object value)
		{
			this.m_value = value;
		}

		internal void SetProperty(string propertyName, object propertyValue)
		{
			if (this.m_properties == null)
			{
				this.m_properties = new Hashtable();
			}
			this.m_properties[propertyName] = propertyValue;
		}

		private object GetProperty(string propertyName)
		{
			if (this.m_properties == null)
			{
				return null;
			}
			return this.m_properties[propertyName];
		}
	}
}
