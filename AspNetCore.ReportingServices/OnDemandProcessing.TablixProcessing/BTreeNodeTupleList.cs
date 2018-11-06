using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTreeNodeTupleList : IStorable, IPersistable
	{
		private List<BTreeNodeTuple> m_list;

		private int m_capacity;

		[NonSerialized]
		private static Declaration m_declaration = BTreeNodeTupleList.GetDeclaration();

		internal BTreeNodeTuple this[int index]
		{
			get
			{
				return this.m_list[index];
			}
		}

		internal int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_list) + 4;
			}
		}

		internal BTreeNodeTupleList()
		{
		}

		internal BTreeNodeTupleList(int capacity)
		{
			this.m_list = new List<BTreeNodeTuple>(capacity);
			this.m_capacity = capacity;
		}

		internal void Add(BTreeNodeTuple tuple, ScalableList<BTreeNode> nodes)
		{
			if (this.m_list.Count == this.m_capacity)
			{
				throw new InvalidOperationException();
			}
			this.m_list.Add(tuple);
			if (-1 != tuple.ChildIndex)
			{
				BTreeNode bTreeNode = default(BTreeNode);
				using (nodes.GetAndPin(tuple.ChildIndex, out bTreeNode))
				{
					bTreeNode.IndexInParent = this.m_list.Count - 1;
				}
			}
		}

		internal void Insert(int index, BTreeNodeTuple tuple, ScalableList<BTreeNode> nodes)
		{
			if (this.m_list.Count == this.m_capacity)
			{
				throw new InvalidOperationException();
			}
			this.m_list.Insert(index, tuple);
			for (int i = index; i < this.m_list.Count; i++)
			{
				int childIndex = this.m_list[i].ChildIndex;
				if (childIndex != -1)
				{
					BTreeNode bTreeNode = default(BTreeNode);
					using (nodes.GetAndPin(childIndex, out bTreeNode))
					{
						bTreeNode.IndexInParent = i;
					}
				}
			}
		}

		internal void RemoveAtEnd()
		{
			this.m_list.RemoveAt(this.m_list.Count - 1);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(BTreeNodeTupleList.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.List:
					writer.Write(this.m_list);
					break;
				case MemberName.Capacity:
					writer.Write(this.m_capacity);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BTreeNodeTupleList.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.List:
					this.m_list = reader.ReadListOfRIFObjects<List<BTreeNodeTuple>>();
					break;
				case MemberName.Capacity:
					this.m_capacity = reader.ReadInt32();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTupleList;
		}

		public static Declaration GetDeclaration()
		{
			if (BTreeNodeTupleList.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.List, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTuple));
				list.Add(new MemberInfo(MemberName.Capacity, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTupleList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return BTreeNodeTupleList.m_declaration;
		}
	}
}
