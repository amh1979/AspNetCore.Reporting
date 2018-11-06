using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal class FieldImpl : AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.Field, IStorable, IPersistable
	{
		[StaticReference]
		private ObjectModelImpl m_reportOM;

		private object m_value;

		private bool m_isAggregationField;

		private bool m_aggregationFieldChecked;

		private DataFieldStatus m_fieldStatus;

		private string m_exceptionMessage;

		private Hashtable m_properties;

		[StaticReference]
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Field m_fieldDef;

		private bool m_usedInExpression;

		private static Declaration m_declaration = FieldImpl.GetDeclaration();

		public override object this[string key]
		{
			get
			{
				if (key == null)
				{
					return null;
				}
				this.m_reportOM.PerformPendingFieldValueUpdate();
				this.m_usedInExpression = true;
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
				this.m_reportOM.PerformPendingFieldValueUpdate();
				this.m_usedInExpression = true;
				if (this.m_fieldStatus == DataFieldStatus.None)
				{
					if (this.IsCalculatedField && this.m_value != null)
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
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return DataFieldStatus.IsMissing == this.m_fieldStatus;
			}
		}

		public override string UniqueName
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("UniqueName") as string;
			}
		}

		public override string BackgroundColor
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("BackgroundColor") as string;
			}
		}

		public override string Color
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("Color") as string;
			}
		}

		public override string FontFamily
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("FontFamily") as string;
			}
		}

		public override string FontSize
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("FontSize") as string;
			}
		}

		public override string FontWeight
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("FontWeight") as string;
			}
		}

		public override string FontStyle
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("FontStyle") as string;
			}
		}

		public override string TextDecoration
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("TextDecoration") as string;
			}
		}

		public override string FormattedValue
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("FormattedValue") as string;
			}
		}

		public override object Key
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("Key");
			}
		}

		public override int LevelNumber
		{
			get
			{
				this.m_reportOM.PerformPendingFieldValueUpdate();
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
				this.m_reportOM.PerformPendingFieldValueUpdate();
				return this.GetProperty("ParentUniqueName") as string;
			}
		}

		internal DataFieldStatus FieldStatus
		{
			get
			{
				if (DataFieldStatus.IsMissing != this.m_fieldStatus)
				{
					this.m_reportOM.PerformPendingFieldValueUpdate();
				}
				if (this.IsCalculatedField)
				{
					if (this.m_value == null)
					{
						return DataFieldStatus.None;
					}
					if (((CalculatedFieldWrapperImpl)this.m_value).ErrorOccurred)
					{
						this.m_exceptionMessage = ((CalculatedFieldWrapperImpl)this.m_value).ExceptionMessage;
						return DataFieldStatus.IsError;
					}
				}
				return this.m_fieldStatus;
			}
		}

		internal string ExceptionMessage
		{
			get
			{
				if (!this.IsCalculatedField)
				{
					this.m_reportOM.PerformPendingFieldValueUpdate();
				}
				return this.m_exceptionMessage;
			}
		}

		internal bool IsAggregationField
		{
			get
			{
				if (!this.IsCalculatedField)
				{
					this.m_reportOM.PerformPendingFieldValueUpdate();
				}
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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Field FieldDef
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

		internal bool IsCalculatedField
		{
			get
			{
				if (this.m_fieldDef != null)
				{
					return this.m_fieldDef.IsCalculatedField;
				}
				return false;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_value) + 1 + 1 + 4 + ItemSizes.SizeOf(this.m_exceptionMessage) + ItemSizes.SizeOf(this.m_properties) + ItemSizes.ReferenceSize + 1;
			}
		}

		internal FieldImpl()
		{
		}

		internal FieldImpl(ObjectModelImpl reportOM, object value, bool isAggregationField, AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			this.m_reportOM = reportOM;
			this.m_fieldDef = fieldDef;
			this.UpdateValue(value, isAggregationField, DataFieldStatus.None, null);
		}

		internal FieldImpl(ObjectModelImpl reportOM, DataFieldStatus status, string exceptionMessage, AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			this.m_reportOM = reportOM;
			this.m_fieldDef = fieldDef;
			Global.Tracer.Assert(DataFieldStatus.None != status, "(DataFieldStatus.None != status)");
			this.UpdateValue(null, false, status, exceptionMessage);
		}

		internal void UpdateValue(object value, bool isAggregationField, DataFieldStatus status, string exceptionMessage)
		{
			this.m_value = value;
			this.m_isAggregationField = isAggregationField;
			this.m_aggregationFieldChecked = false;
			this.m_fieldStatus = status;
			this.m_exceptionMessage = exceptionMessage;
			this.m_usedInExpression = false;
			this.m_properties = null;
		}

		internal bool ResetCalculatedField()
		{
			if (this.m_value == null)
			{
				return false;
			}
			this.m_fieldStatus = DataFieldStatus.None;
			((CalculatedFieldWrapperImpl)this.m_value).ResetValue();
			this.m_usedInExpression = false;
			return true;
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

		internal object GetProperty(string propertyName)
		{
			if (this.m_properties == null)
			{
				return null;
			}
			this.m_usedInExpression = true;
			return this.m_properties[propertyName];
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(FieldImpl.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReportObjectModel:
				{
					int value2 = scalabilityCache.StoreStaticReference(this.m_reportOM);
					writer.Write(value2);
					break;
				}
				case MemberName.Value:
					writer.WriteVariantOrPersistable(this.m_value);
					break;
				case MemberName.IsAggregateField:
					writer.Write(this.m_isAggregationField);
					break;
				case MemberName.AggregationFieldChecked:
					writer.Write(this.m_aggregationFieldChecked);
					break;
				case MemberName.FieldStatus:
					writer.WriteEnum((int)this.m_fieldStatus);
					break;
				case MemberName.Message:
					writer.Write(this.m_exceptionMessage);
					break;
				case MemberName.Properties:
					writer.WriteStringObjectHashtable(this.m_properties);
					break;
				case MemberName.FieldDef:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_fieldDef);
					writer.Write(value);
					break;
				}
				case MemberName.UsedInExpression:
					writer.Write(this.m_usedInExpression);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(FieldImpl.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReportObjectModel:
				{
					int id2 = reader.ReadInt32();
					this.m_reportOM = (ObjectModelImpl)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.Value:
					this.m_value = reader.ReadVariant();
					break;
				case MemberName.IsAggregateField:
					this.m_isAggregationField = reader.ReadBoolean();
					break;
				case MemberName.AggregationFieldChecked:
					this.m_aggregationFieldChecked = reader.ReadBoolean();
					break;
				case MemberName.FieldStatus:
					this.m_fieldStatus = (DataFieldStatus)reader.ReadEnum();
					break;
				case MemberName.Message:
					this.m_exceptionMessage = reader.ReadString();
					break;
				case MemberName.Properties:
					this.m_properties = reader.ReadStringObjectHashtable<Hashtable>();
					break;
				case MemberName.FieldDef:
				{
					int id = reader.ReadInt32();
					this.m_fieldDef = (AspNetCore.ReportingServices.ReportIntermediateFormat.Field)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.UsedInExpression:
					this.m_usedInExpression = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldImpl;
		}

		public static Declaration GetDeclaration()
		{
			if (FieldImpl.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ReportObjectModel, Token.Int32));
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				list.Add(new MemberInfo(MemberName.IsAggregateField, Token.Boolean));
				list.Add(new MemberInfo(MemberName.AggregationFieldChecked, Token.Boolean));
				list.Add(new MemberInfo(MemberName.FieldStatus, Token.Enum));
				list.Add(new MemberInfo(MemberName.Message, Token.String));
				list.Add(new MemberInfo(MemberName.Properties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringObjectHashtable));
				list.Add(new MemberInfo(MemberName.FieldDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.UsedInExpression, Token.Boolean));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldImpl, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return FieldImpl.m_declaration;
		}
	}
}
