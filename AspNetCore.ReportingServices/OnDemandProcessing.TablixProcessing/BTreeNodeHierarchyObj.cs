using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTreeNodeHierarchyObj : BTreeNodeValue
	{
		private object m_key;

		private IHierarchyObj m_hierarchyNode;

		private static Declaration m_declaration = BTreeNodeHierarchyObj.GetDeclaration();

		protected override object Key
		{
			get
			{
				return this.m_key;
			}
		}

		public override int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_key) + ItemSizes.SizeOf(this.m_hierarchyNode);
			}
		}

		internal BTreeNodeHierarchyObj()
		{
		}

		internal BTreeNodeHierarchyObj(object key, IHierarchyObj owner)
		{
			this.m_key = key;
			if (key != null)
			{
				this.m_hierarchyNode = owner.CreateHierarchyObjForSortTree();
				this.m_hierarchyNode.NextRow(owner);
			}
		}

		internal override void AddRow(IHierarchyObj owner)
		{
			this.m_hierarchyNode.NextRow(owner);
		}

		internal override void Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (this.m_hierarchyNode != null)
			{
				this.m_hierarchyNode.Traverse(operation, traversalContext);
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(BTreeNodeHierarchyObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Key:
					writer.Write(this.m_key);
					break;
				case MemberName.HierarchyNode:
					writer.Write(this.m_hierarchyNode);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BTreeNodeHierarchyObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Key:
					this.m_key = reader.ReadVariant();
					break;
				case MemberName.HierarchyNode:
					this.m_hierarchyNode = (IHierarchyObj)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeHierarchyObj;
		}

		internal static Declaration GetDeclaration()
		{
			if (BTreeNodeHierarchyObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Key, Token.Object));
				list.Add(new MemberInfo(MemberName.HierarchyNode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObj));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeHierarchyObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeValue, list);
			}
			return BTreeNodeHierarchyObj.m_declaration;
		}
	}
}
