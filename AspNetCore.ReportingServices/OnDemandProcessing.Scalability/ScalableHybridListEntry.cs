using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableHybridListEntry : IStorable, IPersistable
	{
		internal object Item;

		internal int Next;

		internal int Previous;

		private static readonly Declaration m_declaration = ScalableHybridListEntry.GetDeclaration();

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.Item) + 4 + 4;
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ScalableHybridListEntry.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Item:
					writer.WriteVariantOrPersistable(this.Item);
					break;
				case MemberName.NextLeaf:
					writer.Write(this.Next);
					break;
				case MemberName.PrevLeaf:
					writer.Write(this.Previous);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ScalableHybridListEntry.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Item:
					this.Item = reader.ReadVariant();
					break;
				case MemberName.NextLeaf:
					this.Next = reader.ReadInt32();
					break;
				case MemberName.PrevLeaf:
					this.Previous = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "No references to resolve");
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableHybridListEntry;
		}

		public static Declaration GetDeclaration()
		{
			if (ScalableHybridListEntry.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Item, Token.Object));
				list.Add(new MemberInfo(MemberName.NextLeaf, Token.Int32));
				list.Add(new MemberInfo(MemberName.PrevLeaf, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableHybridListEntry, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return ScalableHybridListEntry.m_declaration;
		}
	}
}
