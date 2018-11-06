using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class LookupMatchesWithRows : LookupMatches
	{
		private int m_firstRow = -1;

		private ScalableList<int> m_rows;

		[NonSerialized]
		private bool m_hasBeenTransferred;

		[NonSerialized]
		private static readonly Declaration m_Declaration = LookupMatchesWithRows.GetDeclaration();

		public override int Size
		{
			get
			{
				return base.Size + 4 + ItemSizes.SizeOf(this.m_rows);
			}
		}

		internal LookupMatchesWithRows()
		{
		}

		internal override void AddRow(long rowOffset, int rowIndex, IScalabilityCache scaleCache)
		{
			if (base.HasRow)
			{
				if (this.m_rows == null)
				{
					this.m_rows = new ScalableList<int>(0, scaleCache, 500, 10);
				}
				this.m_rows.Add(rowIndex);
			}
			else
			{
				this.m_firstRow = rowIndex;
			}
			base.AddRow(rowOffset, rowIndex, scaleCache);
		}

		internal override void SetupRow(int matchIndex, OnDemandProcessingContext odpContext)
		{
			if (this.m_hasBeenTransferred)
			{
				base.SetupRow(matchIndex, odpContext);
			}
			else
			{
				CommonRowCache tablixProcessingLookupRowCache = odpContext.TablixProcessingLookupRowCache;
				int num = -1;
				num = ((matchIndex != 0) ? this.m_rows[matchIndex - 1] : this.m_firstRow);
				tablixProcessingLookupRowCache.SetupRow(num, odpContext);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			if (!this.m_hasBeenTransferred)
			{
				writer.RegisterDeclaration(LookupMatchesWithRows.m_Declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.FirstRow:
						writer.Write(this.m_firstRow);
						break;
					case MemberName.Rows:
						writer.Write(this.m_rows);
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(LookupMatchesWithRows.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FirstRow:
					this.m_firstRow = reader.ReadInt32();
					break;
				case MemberName.Rows:
					this.m_rows = reader.ReadRIFObject<ScalableList<int>>();
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
			if (this.m_hasBeenTransferred)
			{
				return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches;
			}
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatchesWithRows;
		}

		public new static Declaration GetDeclaration()
		{
			if (LookupMatchesWithRows.m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.FirstRow, Token.Int32));
				list.Add(new MemberInfo(MemberName.Rows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatchesWithRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupMatches, list);
			}
			return LookupMatchesWithRows.m_Declaration;
		}

		public override void TransferTo(IScalabilityCache scaleCache)
		{
			base.TransferTo(scaleCache);
			if (this.m_rows != null)
			{
				this.m_rows.Dispose();
				this.m_rows = null;
			}
			this.m_hasBeenTransferred = true;
		}
	}
}
