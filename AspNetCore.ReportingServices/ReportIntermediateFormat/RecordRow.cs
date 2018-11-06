using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RecordRow : IPersistable
	{
		private RecordField[] m_recordFields;

		private bool m_isAggregateRow;

		private int m_aggregationFieldCount;

		[NonSerialized]
		private long m_streamPosition;

		[NonSerialized]
		private static readonly Declaration m_Declaration = RecordRow.GetDeclaration();

		internal RecordField[] RecordFields
		{
			get
			{
				return this.m_recordFields;
			}
			set
			{
				this.m_recordFields = value;
			}
		}

		internal bool IsAggregateRow
		{
			get
			{
				return this.m_isAggregateRow;
			}
			set
			{
				this.m_isAggregateRow = value;
			}
		}

		internal int AggregationFieldCount
		{
			get
			{
				return this.m_aggregationFieldCount;
			}
			set
			{
				this.m_aggregationFieldCount = value;
			}
		}

		internal long StreamPosition
		{
			get
			{
				return this.m_streamPosition;
			}
			set
			{
				this.m_streamPosition = value;
			}
		}

		internal RecordRow()
		{
		}

		internal RecordRow(FieldsImpl fields, int fieldCount, FieldInfo[] fieldInfos)
		{
			this.m_recordFields = new RecordField[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				if (!fields[i].IsMissing)
				{
					FieldInfo fieldInfo = null;
					if (fieldInfos != null && i < fieldInfos.Length)
					{
						fieldInfo = fieldInfos[i];
					}
					this.m_recordFields[i] = new RecordField(fields[i], fieldInfo);
				}
			}
			this.m_isAggregateRow = fields.IsAggregateRow;
			this.m_aggregationFieldCount = fields.AggregationFieldCount;
		}

		internal RecordRow(RecordRow original, int[] mappingDataSetFieldIndexesToDataChunk)
		{
			this.m_streamPosition = original.m_streamPosition;
			this.m_isAggregateRow = original.m_isAggregateRow;
			this.m_recordFields = original.m_recordFields;
			this.ApplyFieldMapping(mappingDataSetFieldIndexesToDataChunk);
		}

		internal void ApplyFieldMapping(int[] mappingDataSetFieldIndexesToDataChunk)
		{
			if (mappingDataSetFieldIndexesToDataChunk != null)
			{
				RecordField[] recordFields = this.m_recordFields;
				this.m_recordFields = new RecordField[mappingDataSetFieldIndexesToDataChunk.Length];
				this.m_aggregationFieldCount = 0;
				for (int i = 0; i < mappingDataSetFieldIndexesToDataChunk.Length; i++)
				{
					if (mappingDataSetFieldIndexesToDataChunk[i] >= 0)
					{
						this.m_recordFields[i] = recordFields[mappingDataSetFieldIndexesToDataChunk[i]];
						if (this.m_recordFields[i] != null && this.m_recordFields[i].IsAggregationField)
						{
							this.m_aggregationFieldCount++;
						}
					}
				}
			}
		}

		internal object GetFieldValue(int aliasIndex)
		{
			RecordField recordField = this.m_recordFields[aliasIndex];
			if (recordField == null)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.IsMissing, null);
			}
			if (recordField.FieldStatus != 0)
			{
				throw new ReportProcessingException_FieldError(recordField.FieldStatus, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime.GetErrorName(recordField.FieldStatus, null));
			}
			return recordField.FieldValue;
		}

		internal bool IsAggregationField(int aliasIndex)
		{
			return this.m_recordFields[aliasIndex].IsAggregationField;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RecordFields, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordField));
			list.Add(new MemberInfo(MemberName.IsAggregateRow, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AggregationFieldCount, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RecordRow.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RecordFields:
					writer.Write(this.m_recordFields);
					break;
				case MemberName.IsAggregateRow:
					writer.Write(this.m_isAggregateRow);
					break;
				case MemberName.AggregationFieldCount:
					writer.Write(this.m_aggregationFieldCount);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			this.m_streamPosition = reader.ObjectStartPosition;
			reader.RegisterDeclaration(RecordRow.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RecordFields:
					this.m_recordFields = reader.ReadArrayOfRIFObjects<RecordField>();
					break;
				case MemberName.IsAggregateRow:
					this.m_isAggregateRow = reader.ReadBoolean();
					break;
				case MemberName.AggregationFieldCount:
					this.m_aggregationFieldCount = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordRow;
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}
	}
}
