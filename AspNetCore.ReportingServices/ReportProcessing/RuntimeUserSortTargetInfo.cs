using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class RuntimeUserSortTargetInfo
	{
		private ReportProcessing.BTreeNode m_sortTree;

		private ReportProcessing.AggregateRowList m_aggregateRows;

		private IntList m_sortFilterInfoIndices;

		private Hashtable m_targetForNonDetailSort;

		private Hashtable m_targetForDetailSort;

		internal ReportProcessing.BTreeNode SortTree
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

		internal ReportProcessing.AggregateRowList AggregateRows
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

		internal IntList SortFilterInfoIndices
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

		internal RuntimeUserSortTargetInfo(ReportProcessing.IHierarchyObj owner, int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
		{
			this.AddSortInfo(owner, sortInfoIndex, sortInfo);
		}

		internal void AddSortInfo(ReportProcessing.IHierarchyObj owner, int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
		{
			if (sortInfo.EventSource.UserSort.SortExpressionScope != null || owner.IsDetail)
			{
				if (sortInfo.EventSource.UserSort.SortExpressionScope == null)
				{
					this.AddSortInfoIndex(sortInfoIndex, sortInfo);
				}
				if (this.m_sortTree == null)
				{
					this.m_sortTree = new ReportProcessing.BTreeNode(owner);
				}
			}
			if (sortInfo.EventSource.UserSort.SortExpressionScope != null)
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

		internal void AddSortInfoIndex(int sortInfoIndex, RuntimeSortFilterEventInfo sortInfo)
		{
			Global.Tracer.Assert(sortInfo.EventSource.UserSort.SortExpressionScope == null || !sortInfo.TargetSortFilterInfoAdded);
			if (this.m_sortFilterInfoIndices == null)
			{
				this.m_sortFilterInfoIndices = new IntList();
			}
			this.m_sortFilterInfoIndices.Add(sortInfoIndex);
			sortInfo.TargetSortFilterInfoAdded = true;
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

		internal void MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo, ReportProcessing.IHierarchyObj sortTarget)
		{
			if (this.m_targetForNonDetailSort != null)
			{
				foreach (int key in this.m_targetForNonDetailSort.Keys)
				{
					if (runtimeSortFilterInfo[key].EventTarget == sortTarget)
					{
						Global.Tracer.Assert(!runtimeSortFilterInfo[key].Processed, "(!runtimeSortFilterInfo[index].Processed)");
						runtimeSortFilterInfo[key].Processed = true;
					}
				}
			}
			if (this.m_targetForDetailSort != null)
			{
				foreach (int key2 in this.m_targetForDetailSort.Keys)
				{
					if (runtimeSortFilterInfo[key2].EventTarget == sortTarget)
					{
						Global.Tracer.Assert(!runtimeSortFilterInfo[key2].Processed, "(!runtimeSortFilterInfo[index].Processed)");
						runtimeSortFilterInfo[key2].Processed = true;
					}
				}
			}
		}

		internal void EnterProcessUserSortPhase(ReportProcessing.ProcessingContext pc)
		{
			if (this.m_sortFilterInfoIndices != null)
			{
				for (int i = 0; i < this.m_sortFilterInfoIndices.Count; i++)
				{
					pc.UserSortFilterContext.EnterProcessUserSortPhase(i);
				}
			}
		}

		internal void LeaveProcessUserSortPhase(ReportProcessing.ProcessingContext pc)
		{
			if (this.m_sortFilterInfoIndices != null)
			{
				for (int i = 0; i < this.m_sortFilterInfoIndices.Count; i++)
				{
					pc.UserSortFilterContext.LeaveProcessUserSortPhase(i);
				}
			}
		}
	}
}
