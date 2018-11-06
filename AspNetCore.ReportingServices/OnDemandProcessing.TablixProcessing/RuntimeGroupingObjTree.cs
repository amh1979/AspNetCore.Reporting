using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeGroupingObjTree : RuntimeGroupingObj
	{
		private BTree m_tree;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeGroupingObjTree.GetDeclaration();

		internal override BTree Tree
		{
			get
			{
				return this.m_tree;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_tree);
			}
		}

		internal RuntimeGroupingObjTree()
		{
		}

		internal RuntimeGroupingObjTree(RuntimeHierarchyObj owner, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
			OnDemandProcessingContext odpContext = base.m_owner.OdpContext;
			this.m_tree = new BTree(owner, odpContext, owner.Depth + 1);
		}

		internal override void Cleanup()
		{
			if (this.m_tree != null)
			{
				this.m_tree.Dispose();
				this.m_tree = null;
			}
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			this.m_tree.NextRow(keyValue, base.m_owner);
		}

		internal override void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			this.m_tree.Traverse(operation, ascending, traversalContext);
		}

		internal override void CopyDomainScopeGroupInstances(RuntimeGroupRootObj destination)
		{
			Global.Tracer.Assert(false, "Domain Scope should only be applied to Hash groups");
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeGroupingObjTree.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Tree)
				{
					writer.Write(this.m_tree);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RuntimeGroupingObjTree.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Tree)
				{
					this.m_tree = (BTree)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjTree;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeGroupingObjTree.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Tree, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjTree, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj, list);
			}
			return RuntimeGroupingObjTree.m_declaration;
		}
	}
}
