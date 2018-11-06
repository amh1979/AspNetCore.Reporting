using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTreeNode : IStorable, IPersistable
	{
		private const int BTreeOrder = 3;

		private BTreeNodeTupleList m_tuples;

		private int m_indexInParent;

		private static Declaration m_declaration = BTreeNode.GetDeclaration();

		internal int IndexInParent
		{
			set
			{
				this.m_indexInParent = value;
			}
		}

		internal BTreeNodeTupleList Tuples
		{
			get
			{
				return this.m_tuples;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_tuples) + 4;
			}
		}

		internal BTreeNode()
		{
		}

		internal BTreeNode(IHierarchyObj owner)
		{
			this.m_tuples = new BTreeNodeTupleList(3);
			BTreeNodeTuple tuple = new BTreeNodeTuple(this.CreateBTreeNode(null, owner), -1);
			this.m_tuples.Add(tuple, null);
		}

		internal void Traverse(ProcessingStages operation, bool ascending, ScalableList<BTreeNode> nodes, ITraversalContext traversalContext)
		{
			if (ascending)
			{
				for (int i = 0; i < this.m_tuples.Count; i++)
				{
					this.m_tuples[i].Traverse(operation, ascending, nodes, traversalContext);
				}
			}
			else
			{
				for (int num = this.m_tuples.Count - 1; num >= 0; num--)
				{
					this.m_tuples[num].Traverse(operation, ascending, nodes, traversalContext);
				}
			}
		}

		internal void SetFirstChild(ScalableList<BTreeNode> nodes, int childIndex)
		{
			Global.Tracer.Assert(1 <= this.m_tuples.Count, "(1 <= m_tuples.Count)");
			this.m_tuples[0].ChildIndex = childIndex;
			if (childIndex != -1)
			{
				BTreeNode bTreeNode = default(BTreeNode);
				using (nodes.GetAndPin(childIndex, out bTreeNode))
				{
					bTreeNode.IndexInParent = 0;
				}
			}
		}

		private BTreeNodeValue CreateBTreeNode(object key, IHierarchyObj owner)
		{
			return new BTreeNodeHierarchyObj(key, owner);
		}

		internal bool SearchAndInsert(object keyValue, ScalableList<BTreeNode> nodes, IHierarchyObj owner, out BTreeNodeValue newSiblingValue, out int newSiblingIndex, out int globalNewSiblingIndex)
		{
			int num = -1;
			int i;
			for (i = 1; i < this.m_tuples.Count; i++)
			{
				BTreeNodeTuple bTreeNodeTuple = this.m_tuples[i];
				num = bTreeNodeTuple.Value.CompareTo(keyValue, owner.OdpContext);
				if (num >= 0)
				{
					break;
				}
			}
			if (num == 0)
			{
				this.m_tuples[i].Value.AddRow(owner);
			}
			else
			{
				int childIndex = this.m_tuples[i - 1].ChildIndex;
				if (childIndex == -1)
				{
					return this.InsertBTreeNode(nodes, this.CreateBTreeNode(keyValue, owner), i, -1, owner, out newSiblingValue, out newSiblingIndex, out globalNewSiblingIndex);
				}
				BTreeNode bTreeNode = default(BTreeNode);
				using (nodes.GetAndPin(childIndex, out bTreeNode))
				{
					BTreeNodeValue nodeValueToInsert = default(BTreeNodeValue);
					int nodeIndexToInsert = default(int);
					int globalNodeIndexToInsert = default(int);
					if (!bTreeNode.SearchAndInsert(keyValue, nodes, owner, out nodeValueToInsert, out nodeIndexToInsert, out globalNodeIndexToInsert))
					{
						return this.InsertBTreeNode(nodes, nodeValueToInsert, nodeIndexToInsert, globalNodeIndexToInsert, owner, out newSiblingValue, out newSiblingIndex, out globalNewSiblingIndex);
					}
				}
			}
			newSiblingValue = null;
			newSiblingIndex = -1;
			globalNewSiblingIndex = -1;
			return true;
		}

		private bool InsertBTreeNode(ScalableList<BTreeNode> nodes, BTreeNodeValue nodeValueToInsert, int nodeIndexToInsert, int globalNodeIndexToInsert, IHierarchyObj owner, out BTreeNodeValue newSiblingValue, out int newSiblingIndex, out int globalNewSibingIndex)
		{
			if (3 > this.m_tuples.Count)
			{
				this.m_tuples.Insert(nodeIndexToInsert, new BTreeNodeTuple(nodeValueToInsert, globalNodeIndexToInsert), nodes);
				newSiblingValue = null;
				newSiblingIndex = -1;
				globalNewSibingIndex = -1;
				return true;
			}
			int num = 2;
			BTreeNode bTreeNode = new BTreeNode(owner);
			BTreeNodeValue bTreeNodeValue;
			if (num < nodeIndexToInsert)
			{
				bTreeNodeValue = this.m_tuples[num].Value;
				bTreeNode.SetFirstChild(nodes, this.m_tuples[num].ChildIndex);
				for (int i = num + 1; i < ((this.m_tuples.Count <= nodeIndexToInsert) ? this.m_tuples.Count : nodeIndexToInsert); i++)
				{
					bTreeNode.m_tuples.Add(this.m_tuples[i], nodes);
				}
				bTreeNode.m_tuples.Add(new BTreeNodeTuple(nodeValueToInsert, globalNodeIndexToInsert), nodes);
				for (int j = nodeIndexToInsert; j < this.m_tuples.Count; j++)
				{
					bTreeNode.m_tuples.Add(this.m_tuples[j], nodes);
				}
				int count = this.m_tuples.Count;
				for (int k = num; k < count; k++)
				{
					this.m_tuples.RemoveAtEnd();
				}
			}
			else if (num > nodeIndexToInsert)
			{
				bTreeNodeValue = this.m_tuples[num - 1].Value;
				bTreeNode.SetFirstChild(nodes, this.m_tuples[num - 1].ChildIndex);
				for (int l = num; l < this.m_tuples.Count; l++)
				{
					bTreeNode.m_tuples.Add(this.m_tuples[l], nodes);
				}
				int count2 = this.m_tuples.Count;
				for (int m = num - 1; m < count2; m++)
				{
					this.m_tuples.RemoveAtEnd();
				}
				this.m_tuples.Insert(nodeIndexToInsert, new BTreeNodeTuple(nodeValueToInsert, globalNodeIndexToInsert), nodes);
			}
			else
			{
				bTreeNodeValue = nodeValueToInsert;
				bTreeNode.SetFirstChild(nodes, globalNodeIndexToInsert);
				for (int n = num; n < this.m_tuples.Count; n++)
				{
					bTreeNode.m_tuples.Add(this.m_tuples[n], nodes);
				}
				int count3 = this.m_tuples.Count;
				for (int num2 = num; num2 < count3; num2++)
				{
					this.m_tuples.RemoveAtEnd();
				}
			}
			newSiblingValue = bTreeNodeValue;
			newSiblingIndex = this.m_indexInParent + 1;
			globalNewSibingIndex = nodes.Count;
			nodes.Add(bTreeNode);
			return false;
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(BTreeNode.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Tuples:
					writer.Write(this.m_tuples);
					break;
				case MemberName.IndexInParent:
					writer.Write(this.m_indexInParent);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BTreeNode.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Tuples:
					this.m_tuples = (BTreeNodeTupleList)reader.ReadRIFObject();
					break;
				case MemberName.IndexInParent:
					this.m_indexInParent = reader.ReadInt32();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode;
		}

		internal static Declaration GetDeclaration()
		{
			if (BTreeNode.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Tuples, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTupleList));
				list.Add(new MemberInfo(MemberName.IndexInParent, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return BTreeNode.m_declaration;
		}
	}
}
