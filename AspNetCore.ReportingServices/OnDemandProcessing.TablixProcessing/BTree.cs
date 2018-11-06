using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTree : IStorable, IPersistable, IDisposable
	{
		private BTreeNode m_root;

		private ScalableList<BTreeNode> m_nodes;

		private static Declaration m_declaration = BTree.GetDeclaration();

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_root) + ItemSizes.SizeOf(this.m_nodes);
			}
		}

		internal BTree()
		{
		}

		internal BTree(IHierarchyObj owner, OnDemandProcessingContext odpContext, int level)
		{
			this.m_nodes = new ScalableList<BTreeNode>(level, odpContext.TablixProcessingScalabilityCache);
			this.m_root = new BTreeNode(owner);
		}

		internal void NextRow(object keyValue, IHierarchyObj owner)
		{
			try
			{
				BTreeNodeValue value = default(BTreeNodeValue);
				int num = default(int);
				int childIndex = default(int);
				if (!this.m_root.SearchAndInsert(keyValue, this.m_nodes, owner, out value, out num, out childIndex))
				{
					int count = this.m_nodes.Count;
					this.m_nodes.Add(this.m_root);
					this.m_root = new BTreeNode(owner);
					this.m_root.SetFirstChild(this.m_nodes, count);
					this.m_root.Tuples.Add(new BTreeNodeTuple(value, childIndex), this.m_nodes);
				}
			}
			catch (ReportProcessingException_SpatialTypeComparisonError)
			{
				throw new ReportProcessingException(owner.RegisterComparisonError("SortExpression"));
			}
			catch (ReportProcessingException_ComparisonError)
			{
				throw new ReportProcessingException(owner.RegisterComparisonError("SortExpression"));
			}
		}

		internal void Traverse(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			this.m_root.Traverse(operation, ascending, this.m_nodes, traversalContext);
		}

		public void Dispose()
		{
			this.m_nodes.Dispose();
			this.m_nodes = null;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(BTree.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Root:
					writer.Write(this.m_root);
					break;
				case MemberName.SortTreeNodes:
					writer.Write(this.m_nodes);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BTree.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Root:
					this.m_root = (BTreeNode)reader.ReadRIFObject();
					break;
				case MemberName.SortTreeNodes:
					this.m_nodes = reader.ReadRIFObject<ScalableList<BTreeNode>>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree;
		}

		internal static Declaration GetDeclaration()
		{
			if (BTree.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Root, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode));
				list.Add(new MemberInfo(MemberName.SortTreeNodes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return BTree.m_declaration;
		}
	}
}
