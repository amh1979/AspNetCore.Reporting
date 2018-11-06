using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeUserSortTargetInfo : IStorable, IPersistable
	{
		private BTree m_sortTree;

		private List<AggregateRow> m_aggregateRows;

		private List<int> m_sortFilterInfoIndices;

		private Hashtable m_targetForNonDetailSort;

		private Hashtable m_targetForDetailSort;

		private static Declaration m_declaration = RuntimeUserSortTargetInfo.GetDeclaration();

		internal BTree SortTree
		{
			get
			{
				return this.m_sortTree;
			}
			set
			{
				this.m_sortTree = value;
			}
		}

		internal List<AggregateRow> AggregateRows
		{
			get
			{
				return this.m_aggregateRows;
			}
			set
			{
				this.m_aggregateRows = value;
			}
		}

		internal List<int> SortFilterInfoIndices
		{
			get
			{
				return this.m_sortFilterInfoIndices;
			}
			set
			{
				this.m_sortFilterInfoIndices = value;
			}
		}

		internal bool TargetForNonDetailSort
		{
			get
			{
				return null != this.m_targetForNonDetailSort;
			}
		}

		public int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_sortTree) + ItemSizes.SizeOf(this.m_aggregateRows) + ItemSizes.SizeOf(this.m_sortFilterInfoIndices) + ItemSizes.SizeOf(this.m_targetForNonDetailSort) + ItemSizes.SizeOf(this.m_targetForDetailSort);
			}
		}

		internal RuntimeUserSortTargetInfo()
		{
		}

		internal RuntimeUserSortTargetInfo(IReference<IHierarchyObj> owner, int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			this.AddSortInfo(owner, sortInfoIndex, sortInfo);
		}

		internal void AddSortInfo(IReference<IHierarchyObj> owner, int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			IInScopeEventSource eventSource = sortInfo.Value().EventSource;
			if (eventSource.UserSort.SortExpressionScope != null || owner.Value().IsDetail)
			{
				if (eventSource.UserSort.SortExpressionScope == null)
				{
					this.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
				if (this.m_sortTree == null)
				{
					IHierarchyObj hierarchyObj = owner.Value();
					this.m_sortTree = new BTree(hierarchyObj, hierarchyObj.OdpContext, hierarchyObj.Depth + 1);
				}
			}
			if (eventSource.UserSort.SortExpressionScope != null)
			{
				if (this.m_targetForNonDetailSort == null)
				{
					this.m_targetForNonDetailSort = new Hashtable();
				}
				this.m_targetForNonDetailSort.Add(sortInfoIndex, null);
			}
			else
			{
				if (this.m_targetForDetailSort == null)
				{
					this.m_targetForDetailSort = new Hashtable();
				}
				this.m_targetForDetailSort.Add(sortInfoIndex, null);
			}
		}

		internal void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfoRef)
		{
			using (sortInfoRef.PinValue())
			{
				RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = sortInfoRef.Value();
				Global.Tracer.Assert(runtimeSortFilterEventInfo.EventSource.UserSort.SortExpressionScope == null || !runtimeSortFilterEventInfo.TargetSortFilterInfoAdded);
				if (this.m_sortFilterInfoIndices == null)
				{
					this.m_sortFilterInfoIndices = new List<int>();
				}
				this.m_sortFilterInfoIndices.Add(sortInfoIndex);
				runtimeSortFilterEventInfo.TargetSortFilterInfoAdded = true;
			}
		}

		internal void ResetTargetForNonDetailSort()
		{
			this.m_targetForNonDetailSort = null;
		}

		internal bool IsTargetForSort(int index, bool detailSort)
		{
			Hashtable hashtable = this.m_targetForNonDetailSort;
			if (detailSort)
			{
				hashtable = this.m_targetForDetailSort;
			}
			if (hashtable != null && hashtable.Contains(index))
			{
				return true;
			}
			return false;
		}

		internal void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo, IReference<IHierarchyObj> sortTarget)
		{
			if (this.m_targetForNonDetailSort != null)
			{
				this.MarkSortInfoProcessed(runtimeSortFilterInfo, sortTarget, this.m_targetForNonDetailSort.Keys);
			}
			if (this.m_targetForDetailSort != null)
			{
				this.MarkSortInfoProcessed(runtimeSortFilterInfo, sortTarget, this.m_targetForDetailSort.Keys);
			}
		}

		private void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo, IReference<IHierarchyObj> sortTarget, ICollection indices)
		{
			foreach (int index in indices)
			{
				IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[index];
				using (reference.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
					if (runtimeSortFilterEventInfo.EventTarget.Equals(sortTarget))
					{
						Global.Tracer.Assert(!runtimeSortFilterEventInfo.Processed, "(!runtimeSortInfo.Processed)");
						runtimeSortFilterEventInfo.Processed = true;
					}
				}
			}
		}

		internal void EnterProcessUserSortPhase(OnDemandProcessingContext odpContext)
		{
			if (this.m_sortFilterInfoIndices != null)
			{
				int count = this.m_sortFilterInfoIndices.Count;
				for (int i = 0; i < count; i++)
				{
					odpContext.UserSortFilterContext.EnterProcessUserSortPhase(this.m_sortFilterInfoIndices[i]);
				}
			}
		}

		internal void LeaveProcessUserSortPhase(OnDemandProcessingContext odpContext)
		{
			if (this.m_sortFilterInfoIndices != null)
			{
				int count = this.m_sortFilterInfoIndices.Count;
				for (int i = 0; i < count; i++)
				{
					odpContext.UserSortFilterContext.LeaveProcessUserSortPhase(this.m_sortFilterInfoIndices[i]);
				}
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeUserSortTargetInfo.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.SortTree:
					writer.Write(this.m_sortTree);
					break;
				case MemberName.AggregateRows:
					writer.Write(this.m_aggregateRows);
					break;
				case MemberName.SortFilterInfoIndices:
					writer.WriteListOfPrimitives(this.m_sortFilterInfoIndices);
					break;
				case MemberName.TargetForNonDetailSort:
					writer.WriteVariantVariantHashtable(this.m_targetForNonDetailSort);
					break;
				case MemberName.TargetForDetailSort:
					writer.WriteVariantVariantHashtable(this.m_targetForDetailSort);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeUserSortTargetInfo.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.SortTree:
					this.m_sortTree = (BTree)reader.ReadRIFObject();
					break;
				case MemberName.AggregateRows:
					this.m_aggregateRows = reader.ReadListOfRIFObjects<List<AggregateRow>>();
					break;
				case MemberName.SortFilterInfoIndices:
					this.m_sortFilterInfoIndices = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.TargetForNonDetailSort:
					this.m_targetForNonDetailSort = reader.ReadVariantVariantHashtable();
					break;
				case MemberName.TargetForDetailSort:
					this.m_targetForDetailSort = reader.ReadVariantVariantHashtable();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo;
		}

		public static Declaration GetDeclaration()
		{
			if (RuntimeUserSortTargetInfo.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.SortTree, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
				list.Add(new MemberInfo(MemberName.AggregateRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow));
				list.Add(new MemberInfo(MemberName.SortFilterInfoIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.TargetForNonDetailSort, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantVariantHashtable));
				list.Add(new MemberInfo(MemberName.TargetForDetailSort, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantVariantHashtable));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeUserSortTargetInfo.m_declaration;
		}
	}
}
