using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class BTreeNodeTuple : IStorable, IPersistable
	{
		private BTreeNodeValue m_value;

		private int m_childIndex = -1;

		[NonSerialized]
		private static Declaration m_declaration = BTreeNodeTuple.GetDeclaration();

		internal BTreeNodeValue Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal int ChildIndex
		{
			get
			{
				return this.m_childIndex;
			}
			set
			{
				this.m_childIndex = value;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_value) + 4;
			}
		}

		internal BTreeNodeTuple()
		{
		}

		internal BTreeNodeTuple(BTreeNodeValue value, int childIndex)
		{
			this.m_value = value;
			this.m_childIndex = childIndex;
		}

		internal void Traverse(ProcessingStages operation, bool ascending, ScalableList<BTreeNode> nodeList, ITraversalContext traversalContext)
		{
			if (ascending)
			{
				if (this.m_value != null)
				{
					this.m_value.Traverse(operation, traversalContext);
				}
				this.VisitChild(operation, ascending, nodeList, traversalContext);
			}
			else
			{
				this.VisitChild(operation, ascending, nodeList, traversalContext);
				if (this.m_value != null)
				{
					this.m_value.Traverse(operation, traversalContext);
				}
			}
		}

		internal void VisitChild(ProcessingStages operation, bool ascending, ScalableList<BTreeNode> nodeList, ITraversalContext traversalContext)
		{
			if (-1 != this.m_childIndex)
			{
				BTreeNode bTreeNode = default(BTreeNode);
				using (nodeList.GetAndPin(this.m_childIndex, out bTreeNode))
				{
					bTreeNode.Traverse(operation, ascending, nodeList, traversalContext);
				}
			}
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(BTreeNodeTuple.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.Child:
					writer.Write(this.m_childIndex);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(BTreeNodeTuple.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Value:
					this.m_value = (BTreeNodeValue)reader.ReadRIFObject();
					break;
				case MemberName.Child:
					this.m_childIndex = reader.ReadInt32();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTuple;
		}

		internal static Declaration GetDeclaration()
		{
			if (BTreeNodeTuple.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeValue));
				list.Add(new MemberInfo(MemberName.Child, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTreeNodeTuple, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return BTreeNodeTuple.m_declaration;
		}
	}
}
