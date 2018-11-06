using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class DataFieldRow : IStorable, IPersistable
	{
		protected FieldImpl[] m_fields;

		protected long m_streamOffset = DataFieldRow.UnInitializedStreamOffset;

		internal static readonly long UnInitializedStreamOffset = -1L;

		private static readonly Declaration m_declaration = DataFieldRow.GetDeclaration();

		internal FieldImpl this[int index]
		{
			get
			{
				return this.m_fields[index];
			}
		}

		internal long StreamOffset
		{
			get
			{
				return this.m_streamOffset;
			}
		}

		public virtual int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_fields) + 8;
			}
		}

		internal DataFieldRow()
		{
		}

		internal DataFieldRow(FieldsImpl fields, bool getAndSave)
		{
			if (getAndSave)
			{
				this.m_fields = fields.GetAndSaveFields();
			}
			else
			{
				this.m_fields = fields.GetFields();
			}
			this.m_streamOffset = fields.StreamOffset;
		}

		internal virtual void SetFields(FieldsImpl fields)
		{
			fields.SetFields(this.m_fields, this.m_streamOffset);
		}

		internal virtual void RestoreDataSetAndSetFields(OnDemandProcessingContext odpContext, FieldsContext fieldsContext)
		{
			odpContext.ReportObjectModel.RestoreFields(fieldsContext);
			this.SetFields(odpContext.ReportObjectModel.FieldsImpl);
		}

		internal virtual void SaveAggregateInfo(OnDemandProcessingContext odpContext)
		{
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DataFieldRow.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Fields:
					writer.Write(this.m_fields);
					break;
				case MemberName.Offset:
					writer.Write(this.m_streamOffset);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DataFieldRow.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Fields:
					this.m_fields = reader.ReadArrayOfRIFObjects<FieldImpl>();
					break;
				case MemberName.Offset:
					this.m_streamOffset = reader.ReadInt64();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow;
		}

		public static Declaration GetDeclaration()
		{
			if (DataFieldRow.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Fields, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldImpl));
				list.Add(new MemberInfo(MemberName.Offset, Token.Int64));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return DataFieldRow.m_declaration;
		}
	}
}
