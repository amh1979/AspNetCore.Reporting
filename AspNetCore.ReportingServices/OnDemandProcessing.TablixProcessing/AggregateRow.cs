using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class AggregateRow : DataFieldRow
	{
		[NonSerialized]
		private AggregateRowInfo m_aggregateInfo;

		private bool m_isAggregateRow;

		private int m_aggregationFieldCount;

		private bool m_validAggregateRow;

		private static Declaration m_declaration = AggregateRow.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + 1 + 4 + 1;
			}
		}

		internal AggregateRow()
		{
		}

		internal AggregateRow(FieldsImpl fields, bool getAndSave)
			: base(fields, getAndSave)
		{
			this.m_isAggregateRow = fields.IsAggregateRow;
			this.m_aggregationFieldCount = fields.AggregationFieldCount;
			this.m_validAggregateRow = fields.ValidAggregateRow;
		}

		internal override void SetFields(FieldsImpl fields)
		{
			fields.SetFields(base.m_fields, base.m_streamOffset, this.m_isAggregateRow, this.m_aggregationFieldCount, this.m_validAggregateRow);
		}

		internal override void RestoreDataSetAndSetFields(OnDemandProcessingContext odpContext, FieldsContext fieldsContext)
		{
			base.RestoreDataSetAndSetFields(odpContext, fieldsContext);
			if (this.m_aggregateInfo != null)
			{
				this.m_aggregateInfo.RestoreAggregateInfo(odpContext);
			}
		}

		internal override void SaveAggregateInfo(OnDemandProcessingContext odpContext)
		{
			this.m_aggregateInfo = new AggregateRowInfo();
			this.m_aggregateInfo.SaveAggregateInfo(odpContext);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(AggregateRow.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsAggregateRow:
					writer.Write(this.m_isAggregateRow);
					break;
				case MemberName.AggregationFieldCount:
					writer.Write(this.m_aggregationFieldCount);
					break;
				case MemberName.ValidAggregateRow:
					writer.Write(this.m_validAggregateRow);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(AggregateRow.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsAggregateRow:
					this.m_isAggregateRow = reader.ReadBoolean();
					break;
				case MemberName.AggregationFieldCount:
					this.m_aggregationFieldCount = reader.ReadInt32();
					break;
				case MemberName.ValidAggregateRow:
					this.m_validAggregateRow = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow;
		}

		public new static Declaration GetDeclaration()
		{
			if (AggregateRow.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.IsAggregateRow, Token.Boolean));
				list.Add(new MemberInfo(MemberName.AggregationFieldCount, Token.Int32));
				list.Add(new MemberInfo(MemberName.ValidAggregateRow, Token.Boolean));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow, list);
			}
			return AggregateRow.m_declaration;
		}
	}
}
