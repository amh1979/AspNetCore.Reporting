using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RecordField : IPersistable
	{
		private object m_fieldValue;

		private bool m_isAggregationField;

		private List<object> m_fieldPropertyValues;

		private DataFieldStatus m_fieldStatus;

		[NonSerialized]
		private static readonly Declaration m_Declaration = RecordField.GetDeclaration();

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

		internal List<object> FieldPropertyValues
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
				Global.Tracer.Assert(DataFieldStatus.None == this.m_fieldStatus, "(DataFieldStatus.None == m_fieldStatus)");
				this.m_fieldStatus = value;
			}
		}

		internal RecordField()
		{
		}

		internal RecordField(FieldImpl field, FieldInfo fieldInfo)
		{
			this.m_fieldStatus = field.FieldStatus;
			if (this.m_fieldStatus == DataFieldStatus.None)
			{
				this.m_fieldValue = field.Value;
				this.m_isAggregationField = field.IsAggregationField;
			}
			if (fieldInfo != null && 0 < fieldInfo.PropertyCount)
			{
				this.m_fieldPropertyValues = new List<object>(fieldInfo.PropertyCount);
				for (int i = 0; i < fieldInfo.PropertyCount; i++)
				{
					object property = field.GetProperty(fieldInfo.PropertyNames[i]);
					this.m_fieldPropertyValues.Add(property);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.FieldValue, Token.Object));
			list.Add(new MemberInfo(MemberName.FieldValueSerializable, Token.Serializable));
			list.Add(new MemberInfo(MemberName.FieldStatus, Token.Enum));
			list.Add(new MemberInfo(MemberName.IsAggregateField, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FieldPropertyValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Object));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordField, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RecordField.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FieldStatus:
					writer.WriteEnum((int)this.m_fieldStatus);
					break;
				case MemberName.FieldValueSerializable:
					if (!writer.TryWriteSerializable(this.m_fieldValue))
					{
						this.m_fieldValue = null;
						writer.WriteNull();
						this.m_fieldStatus = DataFieldStatus.UnSupportedDataType;
					}
					break;
				case MemberName.IsAggregateField:
					writer.Write(this.m_isAggregationField);
					break;
				case MemberName.FieldPropertyValues:
					writer.WriteListOfPrimitives(this.m_fieldPropertyValues);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RecordField.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FieldStatus:
					this.m_fieldStatus = (DataFieldStatus)reader.ReadEnum();
					break;
				case MemberName.FieldValue:
					this.m_fieldValue = reader.ReadVariant();
					break;
				case MemberName.FieldValueSerializable:
					this.m_fieldValue = reader.ReadSerializable();
					break;
				case MemberName.IsAggregateField:
					this.m_isAggregationField = reader.ReadBoolean();
					break;
				case MemberName.FieldPropertyValues:
					this.m_fieldPropertyValues = reader.ReadListOfPrimitives<object>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordField;
		}
	}
}
