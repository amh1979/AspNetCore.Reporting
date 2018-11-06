using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class LookupMatches : IStorable, IPersistable, ITransferable
	{
		private long m_firstRowOffset = DataFieldRow.UnInitializedStreamOffset;

		private ScalableList<long> m_rowOffsets;

		[NonSerialized]
		private static readonly Declaration m_Declaration = LookupMatches.GetDeclaration();

		internal bool HasRow
		{
			get
			{
				return this.m_firstRowOffset != DataFieldRow.UnInitializedStreamOffset;
			}
		}

		internal int MatchCount
		{
			get
			{
				int num = 0;
				if (this.m_rowOffsets != null)
				{
					num = this.m_rowOffsets.Count;
				}
				if (this.HasRow)
				{
					num++;
				}
				return num;
			}
		}

		public virtual int Size
		{
			get
			{
				return 8 + ItemSizes.SizeOf(this.m_rowOffsets);
			}
		}

		internal LookupMatches()
		{
		}

		internal virtual void AddRow(long rowOffset, int rowIndex, IScalabilityCache scaleCache)
		{
			if (this.HasRow)
			{
				if (this.m_rowOffsets == null)
				{
					this.m_rowOffsets = new ScalableList<long>(0, scaleCache, 500, 10);
				}
				this.m_rowOffsets.Add(rowOffset);
			}
			else
			{
				this.m_firstRowOffset = rowOffset;
			}
		}

		internal virtual void SetupRow(int matchIndex, OnDemandProcessingContext odpContext)
		{
			long unInitializedStreamOffset = DataFieldRow.UnInitializedStreamOffset;
			unInitializedStreamOffset = ((matchIndex != 0) ? this.m_rowOffsets[matchIndex - 1] : this.m_firstRowOffset);
			odpContext.ReportObjectModel.UpdateFieldValues(unInitializedStreamOffset);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(LookupMatches.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FirstRowOffset:
					writer.Write(this.m_firstRowOffset);
					break;
				case MemberName.RowOffsets:
					writer.Write(this.m_rowOffsets);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(LookupMatches.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstRowOffset:
					this.m_firstRowOffset = reader.ReadInt64();
					break;
				case MemberName.RowOffsets:
					this.m_rowOffsets = reader.ReadRIFObject<ScalableList<long>>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches;
		}

		public static Declaration GetDeclaration()
		{
			if (LookupMatches.m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FirstRowOffset, Token.Int64));
				list.Add(new MemberInfo(MemberName.RowOffsets, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Token.Int64));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return LookupMatches.m_Declaration;
		}

		public virtual void TransferTo(IScalabilityCache scaleCache)
		{
			if (this.m_rowOffsets != null)
			{
				this.m_rowOffsets.TransferTo(scaleCache);
			}
		}
	}
}
