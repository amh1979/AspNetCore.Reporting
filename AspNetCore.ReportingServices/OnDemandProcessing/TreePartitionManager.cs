using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class TreePartitionManager : IPersistable
	{
		private List<long> m_treePartitionOffsets;

		[NonSerialized]
		private bool m_treeChanged;

		[NonSerialized]
		internal static readonly long EmptyTreePartitionOffset = -1L;

		[NonSerialized]
		internal static readonly ReferenceID EmptyTreePartitionID = new ReferenceID(true, false, -1);

		[NonSerialized]
		private static readonly Declaration m_Declaration = TreePartitionManager.GetDeclaration();

		internal bool TreeHasChanged
		{
			get
			{
				return this.m_treeChanged;
			}
			set
			{
				this.m_treeChanged = value;
			}
		}

		internal TreePartitionManager()
		{
		}

		internal TreePartitionManager(List<long> partitionTable)
		{
			this.m_treePartitionOffsets = partitionTable;
		}

		internal ReferenceID AllocateNewTreePartition()
		{
			this.m_treeChanged = true;
			if (this.m_treePartitionOffsets == null)
			{
				this.m_treePartitionOffsets = new List<long>();
			}
			int count = this.m_treePartitionOffsets.Count;
			this.m_treePartitionOffsets.Add(TreePartitionManager.EmptyTreePartitionOffset);
			ReferenceID result = default(ReferenceID);
			result.HasMultiPart = true;
			result.IsTemporary = false;
			result.PartitionID = count;
			return result;
		}

		internal void UpdateTreePartitionOffset(ReferenceID id, long offset)
		{
			int partitionIndex = this.GetPartitionIndex(id);
			Global.Tracer.Assert(offset >= 0, "Invalid offset for Tree partition. ID: {0} Offset: {1}", id, offset);
			Global.Tracer.Assert(this.m_treePartitionOffsets[partitionIndex] == TreePartitionManager.EmptyTreePartitionOffset, "Cannot update offset for already persisted tree partition");
			this.m_treeChanged = true;
			this.m_treePartitionOffsets[partitionIndex] = offset;
		}

		internal long GetTreePartitionOffset(ReferenceID id)
		{
			int partitionIndex = this.GetPartitionIndex(id);
			return this.m_treePartitionOffsets[partitionIndex];
		}

		private int GetPartitionIndex(ReferenceID id)
		{
			int partitionID = id.PartitionID;
			Global.Tracer.Assert(partitionID >= 0, "Invalid tree partition id: {0}", partitionID);
			Global.Tracer.Assert(this.m_treePartitionOffsets != null && partitionID < this.m_treePartitionOffsets.Count, "Cannot update Tree partition: {0} without first allocating it", partitionID);
			return partitionID;
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(TreePartitionManager.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TreePartitionOffsets)
				{
					writer.WriteListOfPrimitives(this.m_treePartitionOffsets);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(TreePartitionManager.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TreePartitionOffsets)
				{
					this.m_treePartitionOffsets = reader.ReadListOfPrimitives<long>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TreePartitionManager;
		}

		public static Declaration GetDeclaration()
		{
			if (TreePartitionManager.m_Declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.TreePartitionOffsets, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int64));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TreePartitionManager, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return TreePartitionManager.m_Declaration;
		}
	}
}
