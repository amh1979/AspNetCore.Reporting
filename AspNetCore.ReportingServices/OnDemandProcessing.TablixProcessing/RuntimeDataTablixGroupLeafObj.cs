using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeDataTablixGroupLeafObj : RuntimeGroupLeafObj, ISortDataHolder, IOnDemandMemberInstance, IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		private const int BeforeFirstRowInBuffer = -1;

		private const int AfterLastRowInBuffer = -2;

		protected IReference<RuntimeMemberObj>[] m_memberObjs;

		protected bool m_hasInnerHierarchy;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_firstPassCellNonCustomAggs;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_firstPassCellCustomAggs;

		protected RuntimeCells[] m_cellsList;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_cellPostSortAggregates;

		protected int m_groupLeafIndex = -1;

		protected bool m_processHeading = true;

		[NonSerialized]
		protected DataRegionMemberInstance m_memberInstance;

		protected int m_sequentialMemberIndexWithinScopeLevel = -1;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_runningValueValues;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_runningValueOfAggregateValues;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_cellRunningValueValues;

		protected int m_instanceIndex = -1;

		private long m_scopeInstanceNumber;

		[NonSerialized]
		private int m_bufferIndex = -1;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeDataTablixGroupLeafObj.GetDeclaration();

		internal List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> CellPostSortAggregates
		{
			get
			{
				return this.m_cellPostSortAggregates;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion DataRegionDef
		{
			get
			{
				return base.MemberDef.DataRegionDef;
			}
		}

		internal int HeadingLevel
		{
			get
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = (RuntimeDataTablixGroupRootObj)base.m_hierarchyRoot.Value();
				return runtimeDataTablixGroupRootObj.HeadingLevel;
			}
		}

		internal int InstanceIndex
		{
			get
			{
				return this.m_instanceIndex;
			}
		}

		internal int GroupLeafIndex
		{
			get
			{
				return this.m_groupLeafIndex;
			}
		}

		protected bool HasInnerStaticMembersInSameScope
		{
			get
			{
				if (base.MemberDef.InnerStaticMembersInSameScope != null && base.MemberDef.InnerStaticMembersInSameScope.Count != 0)
				{
					return true;
				}
				return false;
			}
		}

		List<object> IOnDemandMemberInstance.GroupExprValues
		{
			get
			{
				return base.m_groupExprValues;
			}
		}

		private List<int> OutermostRowIndexes
		{
			get
			{
				if (base.MemberDef.IsColumn)
				{
					return base.MemberDef.DataRegionDef.OutermostStaticRowIndexes;
				}
				return base.MemberDef.GetCellIndexes();
			}
		}

		private List<int> OutermostColumnIndexes
		{
			get
			{
				if (base.MemberDef.IsColumn)
				{
					return base.MemberDef.GetCellIndexes();
				}
				return base.MemberDef.DataRegionDef.OutermostStaticColumnIndexes;
			}
		}

		public bool IsNoRows
		{
			get
			{
				return base.m_firstRow == null;
			}
		}

		public bool IsMostRecentlyCreatedScopeInstance
		{
			get
			{
				return base.m_hierarchyDef.DataScopeInfo.IsLastScopeInstanceNumber(this.m_scopeInstanceNumber);
			}
		}

		public bool HasUnProcessedServerAggregate
		{
			get
			{
				if (base.m_customAggregates != null && base.m_customAggregates.Count > 0)
				{
					return !base.m_hasProcessedAggregateRow;
				}
				return false;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_memberObjs) + 1 + ItemSizes.SizeOf(this.m_firstPassCellNonCustomAggs) + ItemSizes.SizeOf(this.m_firstPassCellCustomAggs) + ItemSizes.SizeOf(this.m_cellsList) + ItemSizes.SizeOf(this.m_cellPostSortAggregates) + 4 + 1 + ItemSizes.ReferenceSize + 4 + 4 + ItemSizes.SizeOf(this.m_runningValueValues) + ItemSizes.SizeOf(this.m_runningValueOfAggregateValues) + ItemSizes.SizeOf(this.m_cellRunningValueValues) + 8;
			}
		}

		internal RuntimeDataTablixGroupLeafObj()
		{
		}

		internal RuntimeDataTablixGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
			using (groupRootRef.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = groupRootRef.Value();
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef = runtimeDataTablixGroupRootObj.HierarchyDef;
				bool flag = false;
				bool flag2 = base.HandleSortFilterEvent(hierarchyDef.IsColumn);
				DataActions dataAction = default(DataActions);
				this.ConstructorHelper(runtimeDataTablixGroupRootObj, this.DataRegionDef, out flag, out dataAction);
				this.InitializeGroupScopedItems(hierarchyDef, ref dataAction);
				if (!flag)
				{
					base.m_dataAction = dataAction;
				}
				if (flag2)
				{
					base.m_dataAction |= DataActions.UserSort;
				}
				if (base.m_dataAction != 0 || this.DataRegionDef.IsMatrixIDC)
				{
					base.m_dataRows = new ScalableList<DataFieldRow>(base.m_depth + 1, base.m_odpContext.TablixProcessingScalabilityCache, 30);
				}
			}
			base.m_odpContext.CreatedScopeInstance(base.m_hierarchyDef);
			this.m_scopeInstanceNumber = RuntimeDataRegionObj.AssignScopeInstanceNumber(base.m_hierarchyDef.DataScopeInfo);
		}

		protected virtual void InitializeGroupScopedItems(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member, ref DataActions innerDataAction)
		{
		}

		void ISortDataHolder.NextRow()
		{
			if (base.m_detailRowCounter == 0)
			{
				this.NextRow();
			}
			else
			{
				if (base.m_detailSortAdditionalGroupLeafs == null)
				{
					base.m_detailSortAdditionalGroupLeafs = new List<IHierarchyObj>();
				}
				IHierarchyObj hierarchyObj = base.GroupRoot.Value().CreateDetailSortHierarchyObj(this);
				base.m_detailSortAdditionalGroupLeafs.Add(hierarchyObj);
				hierarchyObj.NextRow(this);
			}
			base.m_detailRowCounter++;
		}

		void ISortDataHolder.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			base.TablixProcessingMoveNext(operation);
			switch (operation)
			{
			case ProcessingStages.SortAndFilter:
				this.SortAndFilter((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.PreparePeerGroupRunningValues:
				this.PrepareCalculateRunningValues();
				break;
			case ProcessingStages.RunningValues:
				this.CalculateDetailSortRunningValues((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.CreateGroupTree:
				this.CreateInstance((CreateInstancesTraversalContext)traversalContext);
				break;
			case ProcessingStages.UpdateAggregates:
				this.UpdateAggregates((AggregateUpdateContext)traversalContext);
				break;
			default:
				Global.Tracer.Assert(false);
				break;
			}
			int num = (base.m_detailSortAdditionalGroupLeafs != null) ? base.m_detailSortAdditionalGroupLeafs.Count : 0;
			for (int i = 0; i < num; i++)
			{
				IHierarchyObj hierarchyObj = base.m_detailSortAdditionalGroupLeafs[i];
				hierarchyObj.Traverse(operation, traversalContext);
			}
		}

		private void CalculateDetailSortRunningValues(AggregateUpdateContext aggContext)
		{
			if (base.m_dataRows != null && (base.m_dataAction & DataActions.PostSortAggregates) != 0)
			{
				int count = base.m_dataRows.Count;
				using (base.m_hierarchyRoot.PinValue())
				{
					RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = base.m_hierarchyRoot.Value() as RuntimeDataTablixGroupRootObj;
					if (runtimeDataTablixGroupRootObj.DetailDataRows == null)
					{
						runtimeDataTablixGroupRootObj.DetailDataRows = new ScalableList<DataFieldRow>(runtimeDataTablixGroupRootObj.Depth, base.m_odpContext.TablixProcessingScalabilityCache);
					}
					this.UpdateSortFilterInfo(runtimeDataTablixGroupRootObj, runtimeDataTablixGroupRootObj.HierarchyDef.IsColumn, runtimeDataTablixGroupRootObj.DetailDataRows.Count);
					runtimeDataTablixGroupRootObj.DetailDataRows.AddRange(base.m_dataRows);
				}
				this.CalculateRunningValues(aggContext);
			}
		}

		private void UpdateSortFilterInfo(RuntimeGroupRootObj detailRoot, bool isColumnAxis, int rootRowCount)
		{
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = base.m_odpContext.RuntimeSortFilterInfo;
			if (runtimeSortFilterInfo != null && detailRoot.HierarchyDef.DataRegionDef.SortFilterSourceDetailScopeInfo != null)
			{
				for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[i];
					using (reference.PinValue())
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
						if ((BaseReference)base.SelfReference == (object)runtimeSortFilterEventInfo.GetEventSourceScope(isColumnAxis))
						{
							runtimeSortFilterEventInfo.UpdateEventSourceScope(isColumnAxis, detailRoot.SelfReference, rootRowCount);
						}
						if (runtimeSortFilterEventInfo.HasDetailScopeInfo)
						{
							runtimeSortFilterEventInfo.UpdateDetailScopeInfo(detailRoot, isColumnAxis, rootRowCount, base.SelfReference);
						}
					}
				}
			}
		}

		protected void ConstructorHelper(RuntimeDataTablixGroupRootObj groupRoot, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, out bool handleMyDataAction, out DataActions innerDataAction)
		{
			base.m_dataAction = groupRoot.DataAction;
			handleMyDataAction = false;
			if (base.m_hierarchyDef.DataScopeInfo != null && base.m_hierarchyDef.DataScopeInfo.HasAggregatesToUpdateAtRowScope)
			{
				base.m_dataAction |= DataActions.AggregatesOfAggregates;
				handleMyDataAction = true;
			}
			if (groupRoot.ProcessOutermostStaticCells)
			{
				List<int> outermostColumnIndexes = this.OutermostColumnIndexes;
				foreach (int outermostRowIndex in this.OutermostRowIndexes)
				{
					foreach (int item in outermostColumnIndexes)
					{
						handleMyDataAction |= this.CreateCellAggregates(dataRegionDef, outermostRowIndex, item);
					}
				}
			}
            this.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
			if (base.IsOuterGrouping)
			{
				RuntimeDataTablixObjReference runtimeDataTablixObjReference = (RuntimeDataTablixObjReference)dataRegionDef.RuntimeDataRegionObj;
				using (runtimeDataTablixObjReference.PinValue())
				{
					RuntimeDataTablixObj runtimeDataTablixObj = runtimeDataTablixObjReference.Value();
					int hierarchyDynamicIndex = groupRoot.HierarchyDef.HierarchyDynamicIndex;
					this.m_groupLeafIndex = runtimeDataTablixObj.OuterGroupingCounters[hierarchyDynamicIndex] + 1;
					runtimeDataTablixObj.OuterGroupingCounters[hierarchyDynamicIndex] = this.m_groupLeafIndex;
				}
				dataRegionDef.UpdateOuterGroupingIndexes(base.m_hierarchyRoot as RuntimeDataTablixGroupRootObjReference, this.m_groupLeafIndex);
			}
		}

		private bool CreateCellAggregates(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, int rowIndex, int colIndex)
		{
			bool result = false;
			Cell cell = dataRegionDef.Rows[rowIndex].Cells[colIndex];
			if (cell != null && !cell.SimpleGroupTreeCell)
			{
				if (cell.AggregateIndexes != null)
				{
					RuntimeDataRegionObj.CreateAggregates(base.m_odpContext, dataRegionDef.CellAggregates, cell.AggregateIndexes, ref this.m_firstPassCellNonCustomAggs, ref this.m_firstPassCellCustomAggs);
				}
				if (cell.HasInnerGroupTreeHierarchy)
				{
					this.ConstructOutermostCellContents(cell);
				}
				if (cell.PostSortAggregateIndexes != null)
				{
					result = true;
					RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, dataRegionDef.CellPostSortAggregates, cell.PostSortAggregateIndexes, ref base.m_postSortAggregates);
				}
			}
			return result;
		}

		protected abstract void ConstructOutermostCellContents(Cell cell);

		protected override void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
		{
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = base.m_hierarchyRoot.Value() as RuntimeDataTablixGroupRootObj;
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef = runtimeDataTablixGroupRootObj.HierarchyDef;
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = hierarchyDef.DataRegionDef;
				base.ConstructRuntimeStructure(ref handleMyDataAction, out innerDataAction);
				if (!base.IsOuterGrouping && (!hierarchyDef.HasInnerDynamic || runtimeDataTablixGroupRootObj.HasLeafCells))
				{
					if (this.m_cellsList == null)
					{
						this.m_cellsList = new RuntimeCells[dataRegionDef.OuterGroupingDynamicMemberCount];
						RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, dataRegionDef.CellPostSortAggregates, ref this.m_cellPostSortAggregates);
					}
					int outerGroupingDynamicMemberCount = dataRegionDef.OuterGroupingDynamicMemberCount;
					for (int i = 0; i < outerGroupingDynamicMemberCount; i++)
					{
						IReference<RuntimeDataTablixGroupRootObj> reference = dataRegionDef.CurrentOuterGroupRootObjs[i];
						if (reference == null)
						{
							break;
						}
						this.CreateRuntimeCells(reference.Value());
					}
				}
				int num = hierarchyDef.HasInnerDynamic ? hierarchyDef.InnerDynamicMembers.Count : 0;
				this.m_hasInnerHierarchy = (num > 0);
				this.m_memberObjs = new IReference<RuntimeMemberObj>[Math.Max(1, num)];
				if (num == 0)
				{
					IReference<RuntimeMemberObj> reference2 = RuntimeDataTablixMemberObj.CreateRuntimeMemberObject((IReference<IScope>)base.m_selfReference, (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)null, ref innerDataAction, runtimeDataTablixGroupRootObj.OdpContext, runtimeDataTablixGroupRootObj.InnerGroupings, hierarchyDef.InnerStaticMembersInSameScope, runtimeDataTablixGroupRootObj.OutermostStatics, runtimeDataTablixGroupRootObj.HeadingLevel + 1, base.ObjectType);
					this.m_memberObjs[0] = reference2;
				}
				else
				{
					for (int j = 0; j < num; j++)
					{
						IReference<RuntimeMemberObj> reference3 = RuntimeDataTablixMemberObj.CreateRuntimeMemberObject((IReference<IScope>)base.m_selfReference, hierarchyDef.InnerDynamicMembers[j], ref innerDataAction, runtimeDataTablixGroupRootObj.OdpContext, runtimeDataTablixGroupRootObj.InnerGroupings, (j == 0) ? hierarchyDef.InnerStaticMembersInSameScope : null, runtimeDataTablixGroupRootObj.OutermostStatics, runtimeDataTablixGroupRootObj.HeadingLevel + 1, base.ObjectType);
						this.m_memberObjs[j] = reference3;
					}
				}
			}
		}

		private void CreateRuntimeCells(RuntimeDataTablixGroupRootObj outerGroupRoot)
		{
			if (outerGroupRoot != null && outerGroupRoot.HasLeafCells)
			{
				int hierarchyDynamicIndex = outerGroupRoot.HierarchyDef.HierarchyDynamicIndex;
				if (this.m_cellsList[hierarchyDynamicIndex] == null)
				{
					this.m_cellsList[hierarchyDynamicIndex] = new RuntimeCells(base.Depth, base.m_odpContext.TablixProcessingScalabilityCache);
				}
			}
		}

		internal abstract void CreateCell(RuntimeCells cellsCollection, int collectionKey, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef);

		internal override void NextRow()
		{
			DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
			if (domainScopeContext != null)
			{
				DomainScopeContext.DomainScopeInfo currentDomainScope = domainScopeContext.CurrentDomainScope;
				if (currentDomainScope != null)
				{
					if (base.m_firstRow == null)
					{
						base.m_firstRow = currentDomainScope.CurrentRow;
					}
					return;
				}
			}
			if (base.IsOuterGrouping)
			{
				this.DataRegionDef.CurrentOuterGroupRoot = (RuntimeDataTablixGroupRootObjReference)base.m_hierarchyRoot;
				this.DataRegionDef.UpdateOuterGroupingIndexes((RuntimeDataTablixGroupRootObjReference)base.m_hierarchyRoot, this.m_groupLeafIndex);
			}
			if (base.IsOuterGrouping || !base.m_odpContext.PeerOuterGroupProcessing)
			{
				base.UpdateAggregateInfo();
			}
			if (base.IsOuterGrouping)
			{
				this.DataRegionDef.SaveOuterGroupingAggregateRowInfo(base.m_hierarchyDef.HierarchyDynamicIndex, base.m_odpContext);
			}
			if (base.IsOuterGrouping || !base.m_odpContext.PeerOuterGroupProcessing)
			{
				FieldsImpl fieldsImpl = base.m_odpContext.ReportObjectModel.FieldsImpl;
				if (fieldsImpl.AggregationFieldCount == 0 && fieldsImpl.ValidAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, this.m_firstPassCellCustomAggs, false);
				}
				if (!base.m_odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, this.m_firstPassCellNonCustomAggs, false);
				}
			}
			base.InternalNextRow();
		}

		internal void PeerOuterGroupProcessCells()
		{
			Global.Tracer.Assert(!base.IsOuterGrouping && base.m_odpContext.PeerOuterGroupProcessing);
			DataScopeInfo dataScopeInfo = base.m_hierarchyDef.DataScopeInfo;
			if (dataScopeInfo != null && dataScopeInfo.DataSet != null && dataScopeInfo.DataSet.HasScopeWithCustomAggregates)
			{
				dataScopeInfo.ApplyGroupingFieldsForServerAggregates(base.m_odpContext.ReportObjectModel.FieldsImpl);
			}
			this.ProcessCells();
		}

		protected override void SendToInner()
		{
			RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = (RuntimeDataTablixGroupRootObjReference)base.m_hierarchyRoot;
			if (base.IsOuterGrouping)
			{
				this.DataRegionDef.ResetOuterGroupingIndexesForOuterPeerGroup(runtimeDataTablixGroupRootObjReference.Value().HierarchyDef.HierarchyDynamicIndex);
				this.DataRegionDef.CurrentOuterGroupRoot = runtimeDataTablixGroupRootObjReference;
				this.DataRegionDef.UpdateOuterGroupingIndexes(runtimeDataTablixGroupRootObjReference, this.m_groupLeafIndex);
			}
			base.SendToInner();
			if (!this.DataRegionDef.IsMatrixIDC)
			{
				this.ProcessCells();
			}
			int num = (this.m_memberObjs != null) ? this.m_memberObjs.Length : 0;
			if (num != 0)
			{
				bool peerOuterGroupProcessing = base.m_odpContext.PeerOuterGroupProcessing;
				AggregateRowInfo aggregateRowInfo = null;
				if (base.IsOuterGrouping || num > 1)
				{
					if (aggregateRowInfo == null)
					{
						aggregateRowInfo = new AggregateRowInfo();
					}
					aggregateRowInfo.SaveAggregateInfo(base.m_odpContext);
				}
				for (int i = 0; i < num; i++)
				{
					if (base.IsOuterGrouping)
					{
						if (i != 0)
						{
							base.m_odpContext.PeerOuterGroupProcessing = true;
						}
						this.DataRegionDef.SetDataTablixAggregateRowInfo(aggregateRowInfo);
					}
					IReference<RuntimeMemberObj> reference = this.m_memberObjs[i];
					using (reference.PinValue())
					{
						reference.Value().NextRow(base.IsOuterGrouping, base.m_odpContext);
					}
					if (aggregateRowInfo != null)
					{
						aggregateRowInfo.RestoreAggregateInfo(base.m_odpContext);
					}
				}
				base.m_odpContext.PeerOuterGroupProcessing = peerOuterGroupProcessing;
			}
		}

		internal RuntimeCell GetOrCreateCell(RuntimeDataTablixGroupLeafObj rowGroupLeaf)
		{
			Global.Tracer.Assert(!base.IsOuterGrouping, "(!IsOuterGrouping)");
			RuntimeCell result = null;
			if (this.m_cellsList != null)
			{
				IReference<RuntimeDataTablixGroupRootObj> reference = (IReference<RuntimeDataTablixGroupRootObj>)rowGroupLeaf.HierarchyRoot;
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = reference.Value();
				int hierarchyDynamicIndex = runtimeDataTablixGroupRootObj.HierarchyDef.HierarchyDynamicIndex;
				if (this.m_cellsList[hierarchyDynamicIndex] == null)
				{
					this.CreateRuntimeCells(runtimeDataTablixGroupRootObj);
				}
				RuntimeCells runtimeCells = this.m_cellsList[hierarchyDynamicIndex];
				if (runtimeCells != null)
				{
					IDisposable disposable = default(IDisposable);
					result = runtimeCells.GetOrCreateCell(this.DataRegionDef, (IReference<RuntimeDataTablixGroupLeafObj>)(RuntimeDataTablixGroupLeafObjReference)base.SelfReference, reference, rowGroupLeaf.GroupLeafIndex, out disposable);
				}
			}
			return result;
		}

		private void ProcessCells()
		{
			if (this.m_cellsList != null)
			{
				Global.Tracer.Assert(!base.IsOuterGrouping, "(!IsOuterGrouping)");
				if (!base.m_odpContext.PeerOuterGroupProcessing)
				{
					IReference<RuntimeDataTablixObj> ownerDataTablix = this.GetOwnerDataTablix();
					using (ownerDataTablix.PinValue())
					{
						RuntimeDataTablixObj runtimeDataTablixObj = ownerDataTablix.Value();
						if (runtimeDataTablixObj.InnerGroupsWithCellsForOuterPeerGroupProcessing != null)
						{
							runtimeDataTablixObj.InnerGroupsWithCellsForOuterPeerGroupProcessing.Add((RuntimeDataTablixGroupLeafObjReference)base.m_selfReference);
						}
					}
				}
				int[] outerGroupingIndexes = this.DataRegionDef.OuterGroupingIndexes;
				for (int i = 0; i < outerGroupingIndexes.Length; i++)
				{
					IReference<RuntimeDataTablixGroupRootObj> reference = this.DataRegionDef.CurrentOuterGroupRootObjs[i];
					if (reference != null)
					{
						RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = reference.Value();
						int hierarchyDynamicIndex = runtimeDataTablixGroupRootObj.HierarchyDef.HierarchyDynamicIndex;
						int num = outerGroupingIndexes[i];
						AggregateRowInfo aggregateRowInfo = AggregateRowInfo.CreateAndSaveAggregateInfo(base.m_odpContext);
						this.DataRegionDef.SetCellAggregateRowInfo(i, base.m_odpContext);
						if (this.m_cellsList[hierarchyDynamicIndex] == null)
						{
							this.CreateRuntimeCells(runtimeDataTablixGroupRootObj);
						}
						RuntimeCells runtimeCells = this.m_cellsList[hierarchyDynamicIndex];
						if (runtimeCells != null)
						{
							IDisposable disposable2 = default(IDisposable);
							RuntimeCell andPinCell = runtimeCells.GetAndPinCell(num, out disposable2);
							if (andPinCell == null)
							{
								this.CreateCell(runtimeCells, num, runtimeDataTablixGroupRootObj.HierarchyDef, base.MemberDef, this.DataRegionDef);
								andPinCell = runtimeCells.GetAndPinCell(num, out disposable2);
							}
							andPinCell.NextRow();
							if (disposable2 != null)
							{
								disposable2.Dispose();
							}
						}
						aggregateRowInfo.RestoreAggregateInfo(base.m_odpContext);
					}
				}
			}
		}

		internal override void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			if (base.GroupRoot.Value().IsDetailGroup && (targetScopeObj == null || this != targetScopeObj.Value()))
			{
				base.DetailGetScopeValues(this.OuterScope, targetScopeObj, scopeValues, ref index);
			}
			else
			{
				base.GetScopeValues(targetScopeObj, scopeValues, ref index);
			}
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			if (base.GroupRoot.Value().IsDetailGroup)
			{
				return base.DetailTargetScopeMatched(base.MemberDef.DataRegionDef, this.OuterScope, base.MemberDef.IsColumn, index);
			}
			if (detailSort && base.GroupingDef.SortFilterScopeInfo == null)
			{
				return true;
			}
			if (base.m_targetScopeMatched != null && base.m_targetScopeMatched[index])
			{
				return true;
			}
			return false;
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			this.SetupEnvironment();
			if (base.m_userSortTargetInfo != null)
			{
				base.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_odpContext);
			}
			AggregateUpdateQueue workQueue = null;
			if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, base.m_hierarchyDef.DataScopeInfo, base.m_aggregatesOfAggregates, AggregateUpdateFlags.Both, false);
			}
			bool result = default(bool);
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = (RuntimeDataTablixGroupRootObj)base.m_hierarchyRoot.Value();
				bool flag = false;
				int innerDomainScopeCount = runtimeDataTablixGroupRootObj.HierarchyDef.InnerDomainScopeCount;
				DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
				if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope) && innerDomainScopeCount > 0)
				{
					domainScopeContext.AddDomainScopes(this.m_memberObjs, this.m_memberObjs.Length - innerDomainScopeCount);
				}
				this.TraverseStaticContents(ProcessingStages.SortAndFilter, aggContext);
				if (this.m_hasInnerHierarchy)
				{
					bool flag2 = true;
					for (int i = 0; i < this.m_memberObjs.Length; i++)
					{
						IReference<RuntimeMemberObj> reference = this.m_memberObjs[i];
						using (reference.PinValue())
						{
							RuntimeMemberObj runtimeMemberObj = reference.Value();
							if (runtimeMemberObj.SortAndFilter(aggContext))
							{
								flag2 = false;
							}
							else if ((BaseReference)null == (object)runtimeMemberObj.GroupRoot)
							{
								flag2 = false;
							}
							else if (runtimeMemberObj.GroupRoot.Value().HierarchyDef.InnerStaticMembersInSameScope != null)
							{
								flag2 = false;
							}
						}
					}
					if (flag2)
					{
						Global.Tracer.Assert(SecondPassOperations.None != (SecondPassOperations.FilteringOrAggregatesOrDomainScope & base.m_odpContext.SecondPassOperation), "(0 != (SecondPassOperations.Filtering & m_odpContext.SecondPassOperation))");
						Global.Tracer.Assert(null != runtimeDataTablixGroupRootObj.GroupFilters, "(null != groupRoot.GroupFilters)");
						runtimeDataTablixGroupRootObj.GroupFilters.FailFilters = true;
						flag = true;
					}
				}
				if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
				{
					RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, base.m_hierarchyDef.DataScopeInfo, base.m_aggregatesOfAggregates, false);
				}
				result = base.SortAndFilter(aggContext);
				if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope) && innerDomainScopeCount > 0)
				{
					domainScopeContext.RemoveDomainScopes(this.m_memberObjs, this.m_memberObjs.Length - innerDomainScopeCount);
				}
				if (flag)
				{
					runtimeDataTablixGroupRootObj.GroupFilters.FailFilters = false;
				}
			}
			if (base.m_userSortTargetInfo != null)
			{
				base.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_odpContext);
			}
			return result;
		}

		internal override void PostFilterNextRow(AggregateUpdateContext context)
		{
			AggregateUpdateQueue workQueue = null;
			if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(context, this, base.m_hierarchyDef.DataScopeInfo, base.m_aggregatesOfAggregates, AggregateUpdateFlags.None, false);
			}
			this.TraverseCellList(ProcessingStages.SortAndFilter, context);
			if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, context, workQueue, base.m_hierarchyDef.DataScopeInfo, base.m_aggregatesOfAggregates, true);
			}
			base.PostFilterNextRow(context);
		}

		protected virtual void TraverseStaticContents(ProcessingStages operation, AggregateUpdateContext context)
		{
		}

		private void TraverseCellList(ProcessingStages operation, AggregateUpdateContext aggContext)
		{
			if (this.m_cellsList != null)
			{
				for (int i = 0; i < this.m_cellsList.Length; i++)
				{
					if (this.m_cellsList[i] != null)
					{
						switch (operation)
						{
						case ProcessingStages.SortAndFilter:
							this.m_cellsList[i].SortAndFilter(aggContext);
							break;
						case ProcessingStages.UpdateAggregates:
							this.m_cellsList[i].UpdateAggregates(aggContext);
							break;
						default:
							Global.Tracer.Assert(false, "Unknown operation in TraverseCellList");
							break;
						}
					}
				}
			}
		}

		public override void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			this.SetupEnvironment();
			if (RuntimeDataRegionObj.UpdateAggregatesAtScope(aggContext, this, base.m_hierarchyDef.DataScopeInfo, AggregateUpdateFlags.Both, false))
			{
				this.TraverseStaticContents(ProcessingStages.UpdateAggregates, aggContext);
				if (this.m_hasInnerHierarchy)
				{
					for (int i = 0; i < this.m_memberObjs.Length; i++)
					{
						IReference<RuntimeMemberObj> reference = this.m_memberObjs[i];
						using (reference.PinValue())
						{
							RuntimeMemberObj runtimeMemberObj = reference.Value();
							runtimeMemberObj.UpdateAggregates(aggContext);
						}
					}
				}
				this.TraverseCellList(ProcessingStages.UpdateAggregates, aggContext);
			}
		}

		protected static void CalculateInnerRunningValues(IReference<RuntimeMemberObj>[] memberObjs, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (memberObjs != null)
			{
				foreach (IReference<RuntimeMemberObj> reference in memberObjs)
				{
					using (reference.PinValue())
					{
						reference.Value().CalculateRunningValues(groupCol, lastGroup, aggContext);
					}
				}
			}
		}

		protected override void PrepareCalculateRunningValues()
		{
			this.m_processHeading = true;
			if (this.m_hasInnerHierarchy)
			{
				for (int i = 0; i < this.m_memberObjs.Length; i++)
				{
					IReference<RuntimeMemberObj> reference = this.m_memberObjs[i];
					using (reference.PinValue())
					{
						reference.Value().PrepareCalculateRunningValues();
					}
				}
			}
		}

		internal override void CalculateRunningValues(AggregateUpdateContext aggContext)
		{
			this.SetupEnvironment();
			AggregateUpdateQueue workQueue = null;
			bool flag = false;
			DataActions dataActions = DataActions.PostSortAggregates;
			if (this.m_processHeading)
			{
				flag = FlagUtils.HasFlag(base.m_dataAction, DataActions.PostSortAggregates);
				workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, base.m_hierarchyDef.DataScopeInfo, base.m_postSortAggregatesOfAggregates, (AggregateUpdateFlags)(flag ? 1 : 3), false);
				if (flag && aggContext.LastScopeNeedsRowAggregateProcessing())
				{
					dataActions |= DataActions.PostSortAggregatesOfAggregates;
				}
			}
			bool isOuterGrouping = base.IsOuterGrouping;
			RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = (RuntimeDataTablixGroupRootObjReference)base.m_hierarchyRoot;
			using (runtimeDataTablixGroupRootObjReference.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeDataTablixGroupRootObjReference.Value();
				Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection = runtimeDataTablixGroupRootObj.GroupCollection;
				if (this.m_processHeading)
				{
					if (flag)
					{
						base.ReadRows(dataActions, aggContext);
						if (isOuterGrouping)
						{
							base.m_dataRows = null;
						}
					}
					RuntimeDataTablixGroupLeafObj.CalculateInnerRunningValues(this.m_memberObjs, groupCollection, runtimeDataTablixGroupRootObjReference, aggContext);
				}
				else if (this.m_hasInnerHierarchy)
				{
					RuntimeDataTablixGroupLeafObj.CalculateInnerRunningValues(this.m_memberObjs, groupCollection, runtimeDataTablixGroupRootObjReference, aggContext);
				}
				RuntimeGroupRootObjReference lastGroup = runtimeDataTablixGroupRootObjReference;
				if (isOuterGrouping)
				{
					if (!this.m_hasInnerHierarchy || this.HasInnerStaticMembersInSameScope)
					{
						this.DataRegionDef.CurrentOuterGroupRoot = runtimeDataTablixGroupRootObjReference;
						this.DataRegionDef.OuterGroupingIndexes[runtimeDataTablixGroupRootObj.HierarchyDef.HierarchyDynamicIndex] = this.m_groupLeafIndex;
						RuntimeDataTablixGroupLeafObj.CalculateInnerRunningValues(runtimeDataTablixGroupRootObj.InnerGroupings, groupCollection, lastGroup, aggContext);
					}
				}
				else if (this.m_cellsList != null)
				{
					IReference<RuntimeDataTablixGroupRootObj> currentOuterGroupRoot = this.DataRegionDef.CurrentOuterGroupRoot;
					using (currentOuterGroupRoot.PinValue())
					{
						RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj2 = currentOuterGroupRoot.Value();
						int hierarchyDynamicIndex = runtimeDataTablixGroupRootObj2.HierarchyDef.HierarchyDynamicIndex;
						if (this.m_cellsList[hierarchyDynamicIndex] == null && base.m_odpContext.PeerOuterGroupProcessing)
						{
							this.CreateRuntimeCells(runtimeDataTablixGroupRootObj2);
						}
						RuntimeCells runtimeCells = this.m_cellsList[hierarchyDynamicIndex];
						if (runtimeCells != null)
						{
							this.DataRegionDef.ProcessCellRunningValues = true;
							runtimeCells.CalculateRunningValues(this.DataRegionDef, groupCollection, lastGroup, (RuntimeDataTablixGroupLeafObjReference)base.m_selfReference, aggContext);
							this.DataRegionDef.ProcessCellRunningValues = false;
						}
					}
				}
			}
			this.CalculateRunningValuesForStaticContents(aggContext);
			if (this.m_processHeading)
			{
				RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, base.m_hierarchyDef.DataScopeInfo, base.m_postSortAggregatesOfAggregates, true);
				if (base.m_odpContext.HasPreviousAggregates)
				{
					base.CalculatePreviousAggregates(isOuterGrouping);
				}
			}
			this.StoreCalculatedRunningValues();
		}

		protected virtual void CalculateRunningValuesForStaticContents(AggregateUpdateContext aggContext)
		{
		}

		protected void StoreCalculatedRunningValues()
		{
			if (this.m_processHeading)
			{
				using (base.m_hierarchyRoot.PinValue())
				{
					RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = (RuntimeDataTablixGroupRootObj)base.m_hierarchyRoot.Value();
					runtimeDataTablixGroupRootObj.DoneReadingRows(ref this.m_runningValueValues, ref this.m_runningValueOfAggregateValues, ref this.m_cellRunningValueValues);
					this.m_processHeading = false;
				}
			}
			this.ResetScopedRunningValues();
		}

		protected override void ResetScopedRunningValues()
		{
			this.m_processHeading = false;
			base.ResetScopedRunningValues();
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (DataActions.UserSort == dataAction)
			{
				RuntimeDataRegionObj.CommonFirstRow(base.m_odpContext, ref base.m_firstRowIsAggregate, ref base.m_firstRow);
				base.CommonNextRow(base.m_dataRows);
			}
			else if (this.DataRegionDef.ProcessCellRunningValues)
			{
				if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates) && this.m_cellPostSortAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, this.m_cellPostSortAggregates, false);
				}
				using (base.m_hierarchyRoot.PinValue())
				{
					IScope scope = base.m_hierarchyRoot.Value();
					scope.ReadRow(dataAction, context);
				}
			}
			else
			{
				base.ReadRow(dataAction, context);
			}
		}

		public override void SetupEnvironment()
		{
			base.SetupEnvironment();
			this.SetupAggregateValues(this.m_firstPassCellNonCustomAggs, this.m_firstPassCellCustomAggs);
		}

		private void SetupAggregateValues(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> nonCustomAggCollection, List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> customAggCollection)
		{
			base.SetupAggregates(nonCustomAggCollection);
			base.SetupAggregates(customAggCollection);
		}

		protected virtual void CreateInstanceHeadingContents()
		{
		}

		internal override void CreateInstance(CreateInstancesTraversalContext traversalContext)
		{
			this.SetupEnvironment();
			RuntimeDataTablixGroupRootObjReference runtimeDataTablixGroupRootObjReference = (RuntimeDataTablixGroupRootObjReference)base.m_hierarchyRoot;
			using (runtimeDataTablixGroupRootObjReference.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeDataTablixGroupRootObjReference.Value();
				ScopeInstance scopeInstance = traversalContext.ParentInstance;
				base.SetupRunningValues(base.MemberDef.RunningValues, this.m_runningValueValues);
				if (base.MemberDef.DataScopeInfo != null)
				{
					base.SetupRunningValues(base.MemberDef.DataScopeInfo.RunningValuesOfAggregates, this.m_runningValueOfAggregateValues);
				}
				if (base.m_targetScopeMatched != null)
				{
					base.MemberDef.Grouping.SortFilterScopeMatched = base.m_targetScopeMatched;
				}
				IReference<RuntimeDataTablixGroupRootObj> reference;
				if (base.IsOuterGrouping)
				{
					reference = runtimeDataTablixGroupRootObjReference;
					this.DataRegionDef.CurrentOuterGroupRoot = reference;
					this.DataRegionDef.UpdateOuterGroupingIndexes(runtimeDataTablixGroupRootObjReference, this.m_groupLeafIndex);
					this.DataRegionDef.NewOuterCells();
				}
				else
				{
					reference = this.DataRegionDef.CurrentOuterGroupRoot;
				}
				bool flag = false;
				if (this.m_sequentialMemberIndexWithinScopeLevel == -1)
				{
					this.m_sequentialMemberIndexWithinScopeLevel = this.DataRegionDef.AddMemberInstance(base.MemberDef.IsColumn);
					this.m_memberInstance = DataRegionMemberInstance.CreateInstance((IMemberHierarchy)scopeInstance, base.m_odpContext, base.MemberDef, base.m_firstRow.StreamOffset, this.m_sequentialMemberIndexWithinScopeLevel, base.m_recursiveLevel, runtimeDataTablixGroupRootObj.IsDetailGroup ? null : base.m_groupExprValues, base.m_variableValues, out this.m_instanceIndex);
					flag = true;
					if (runtimeDataTablixGroupRootObj.HasParent)
					{
						runtimeDataTablixGroupRootObj.SetRecursiveParentIndex(this.m_instanceIndex, base.m_recursiveLevel);
						if (base.m_recursiveLevel > 0)
						{
							this.m_memberInstance.RecursiveParentIndex = runtimeDataTablixGroupRootObj.GetRecursiveParentIndex(base.m_recursiveLevel - 1);
						}
						this.m_memberInstance.HasRecursiveChildren = ((BaseReference)base.m_firstChild != (object)null || base.m_grouping != null);
					}
					scopeInstance = this.m_memberInstance;
					this.CreateInstanceHeadingContents();
				}
				runtimeDataTablixGroupRootObj.CurrentMemberIndexWithinScopeLevel = this.m_sequentialMemberIndexWithinScopeLevel;
				runtimeDataTablixGroupRootObj.CurrentMemberInstance = this.m_memberInstance;
				if (this.m_memberObjs != null)
				{
					for (int i = 0; i < this.m_memberObjs.Length; i++)
					{
						IReference<RuntimeMemberObj> reference2 = this.m_memberObjs[i];
						using (reference2.PinValue())
						{
							reference2.Value().CreateInstances(base.SelfReference, base.m_odpContext, runtimeDataTablixGroupRootObj.DataRegionInstance, base.IsOuterGrouping, reference, scopeInstance, traversalContext.InnerMembers, traversalContext.InnerGroupLeafRef);
						}
					}
				}
				if (flag)
				{
					this.m_memberInstance.InstanceComplete();
					this.m_memberInstance = null;
				}
			}
		}

		internal void CreateInnerGroupingsOrCells(DataRegionInstance dataRegionInstance, ScopeInstance parentInstance, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			this.SetupEnvironment();
			if (innerMembers != null)
			{
				innerGroupLeafRef = ((!base.IsOuterGrouping) ? ((IReference<RuntimeDataTablixGroupLeafObj>)base.SelfReference) : null);
				foreach (IReference<RuntimeMemberObj> reference in innerMembers)
				{
					using (reference.PinValue())
					{
						reference.Value().CreateInstances(base.SelfReference, base.m_odpContext, dataRegionInstance, !base.IsOuterGrouping, currOuterGroupRoot, dataRegionInstance, null, innerGroupLeafRef);
					}
				}
			}
			else
			{
				IDisposable disposable2 = null;
				RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj;
				if (base.IsOuterGrouping && innerGroupLeafRef != null)
				{
					disposable2 = innerGroupLeafRef.PinValue();
					runtimeDataTablixGroupLeafObj = innerGroupLeafRef.Value();
				}
				else
				{
					runtimeDataTablixGroupLeafObj = this;
				}
				if (currOuterGroupRoot != null)
				{
					runtimeDataTablixGroupLeafObj.CreateCellInstance(dataRegionInstance, currOuterGroupRoot);
				}
				else if (base.MemberDef.IsColumn)
				{
					runtimeDataTablixGroupLeafObj.CreateOutermostStatics(dataRegionInstance, this.m_sequentialMemberIndexWithinScopeLevel);
				}
				else
				{
					runtimeDataTablixGroupLeafObj.CreateOutermostStatics(this.m_memberInstance, 0);
				}
				if (disposable2 != null)
				{
					disposable2.Dispose();
				}
			}
		}

		protected virtual void CreateCellInstance(DataRegionInstance dataRegionInstance, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot)
		{
			this.SetupEnvironment();
			int hierarchyDynamicIndex = currOuterGroupRoot.Value().HierarchyDef.HierarchyDynamicIndex;
			if (this.m_cellsList[hierarchyDynamicIndex] == null)
			{
				this.CreateRuntimeCells(currOuterGroupRoot.Value());
			}
			Global.Tracer.Assert(null != this.m_cellsList[hierarchyDynamicIndex], "(null != m_cellsList[index])");
			dataRegionInstance.DataRegionDef.AddCell();
			RuntimeCells runtimeCells = this.m_cellsList[hierarchyDynamicIndex];
			IDisposable disposable = default(IDisposable);
			RuntimeCell orCreateCell = runtimeCells.GetOrCreateCell(this.DataRegionDef, (IReference<RuntimeDataTablixGroupLeafObj>)(RuntimeDataTablixGroupLeafObjReference)base.m_selfReference, currOuterGroupRoot, out disposable);
			if (orCreateCell != null)
			{
				if (base.MemberDef.IsColumn)
				{
					orCreateCell.CreateInstance(currOuterGroupRoot.Value().CurrentMemberInstance, this.m_sequentialMemberIndexWithinScopeLevel);
				}
				else
				{
					orCreateCell.CreateInstance(this.m_memberInstance, currOuterGroupRoot.Value().CurrentMemberIndexWithinScopeLevel);
				}
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal void CreateStaticCells(DataRegionInstance dataRegionInstance, ScopeInstance parentInstance, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, bool outerGroupings, List<int> staticLeafCellIndexes, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			if (base.IsOuterGrouping && !outerGroupings)
			{
				goto IL_001c;
			}
			if (base.IsOuterGrouping && innerMembers == null && innerGroupLeafRef == null)
			{
				goto IL_001c;
			}
			this.CreateInnerGroupingsOrCells(dataRegionInstance, parentInstance, currOuterGroupRoot, innerMembers, innerGroupLeafRef);
			return;
			IL_001c:
			if (base.MemberDef.IsColumn)
			{
				this.CreateOutermostStatics(dataRegionInstance, this.m_sequentialMemberIndexWithinScopeLevel);
			}
			else
			{
				this.CreateOutermostStatics(this.m_memberInstance, 0);
			}
		}

		protected void CreateOutermostStatics(IMemberHierarchy dataRegionOrRowMemberInstance, int columnMemberSequenceId)
		{
			this.SetupEnvironment();
			base.SetupRunningValues(base.MemberDef.RunningValues, this.m_runningValueValues);
			if (base.MemberDef.DataScopeInfo != null)
			{
				base.SetupRunningValues(base.MemberDef.DataScopeInfo.RunningValuesOfAggregates, this.m_runningValueOfAggregateValues);
			}
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = (RuntimeDataTablixGroupRootObj)base.m_hierarchyRoot.Value();
				runtimeDataTablixGroupRootObj.SetupCellRunningValues(this.m_cellRunningValueValues);
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = this.DataRegionDef;
			long firstRowOffset = (base.m_firstRow != null) ? base.m_firstRow.StreamOffset : 0;
			dataRegionDef.AddCell();
			List<int> outermostColumnIndexes = this.OutermostColumnIndexes;
			foreach (int outermostRowIndex in this.OutermostRowIndexes)
			{
				foreach (int item in outermostColumnIndexes)
				{
					Cell cell = dataRegionDef.Rows[outermostRowIndex].Cells[item];
					if (cell != null && !cell.SimpleGroupTreeCell)
					{
						DataCellInstance dataCellInstance = DataCellInstance.CreateInstance(dataRegionOrRowMemberInstance, base.m_odpContext, cell, firstRowOffset, columnMemberSequenceId);
						this.CreateOutermostStaticCellContents(cell, dataCellInstance);
						dataCellInstance.InstanceComplete();
					}
				}
			}
		}

		protected virtual void CreateOutermostStaticCellContents(Cell cell, DataCellInstance cellInstance)
		{
		}

		internal bool GetCellTargetForNonDetailSort()
		{
			return ((RuntimeDataTablixGroupRootObj)base.m_hierarchyRoot.Value()).GetCellTargetForNonDetailSort();
		}

		internal bool GetCellTargetForSort(int index, bool detailSort)
		{
			return ((RuntimeDataTablixGroupRootObj)base.m_hierarchyRoot.Value()).GetCellTargetForSort(index, detailSort);
		}

		internal bool NeedHandleCellSortFilterEvent()
		{
			if (base.GroupingDef.SortFilterScopeMatched == null)
			{
				return null != base.GroupingDef.NeedScopeInfoForSortFilterExpression;
			}
			return true;
		}

		internal IReference<RuntimeDataTablixObj> GetOwnerDataTablix()
		{
			IReference<IScope> outerScope = this.OuterScope;
			while (!(outerScope is RuntimeDataTablixObjReference))
			{
				outerScope = outerScope.Value().GetOuterScope(false);
			}
			Global.Tracer.Assert(outerScope is RuntimeDataTablixObjReference, "(outerScopeRef is RuntimeDataTablixObjReference)");
			return (RuntimeDataTablixObjReference)outerScope;
		}

		internal bool ReadStreamingModeIdcRowFromBufferOrDataSet(FieldsContext fieldsContext)
		{
			if (this.m_bufferIndex == -2)
			{
				return false;
			}
			this.m_bufferIndex++;
			if (this.m_bufferIndex >= base.m_dataRows.Count)
			{
				base.m_odpContext.StateManager.ProcessOneRow(base.GroupingDef.Owner);
				if (this.m_bufferIndex >= base.m_dataRows.Count)
				{
					this.m_bufferIndex = -2;
					return false;
				}
				DataFieldRow dataFieldRow = base.m_dataRows[this.m_bufferIndex];
				dataFieldRow.SaveAggregateInfo(base.m_odpContext);
				return true;
			}
			DataFieldRow dataFieldRow2 = base.m_dataRows[this.m_bufferIndex];
			dataFieldRow2.RestoreDataSetAndSetFields(base.m_odpContext, fieldsContext);
			return true;
		}

		internal void PushBackStreamingModeIdcRowToBuffer()
		{
			if (this.m_bufferIndex != -2)
			{
				this.m_bufferIndex--;
			}
		}

		internal void ResetStreamingModeIdcRowBuffer()
		{
			this.m_bufferIndex = -1;
		}

		public IOnDemandMemberInstanceReference GetFirstMemberInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember)
		{
			return RuntimeDataRegionObj.GetFirstMemberInstance(rifMember, this.m_memberObjs);
		}

		public IOnDemandMemberInstanceReference GetNextMemberInstance()
		{
			IOnDemandMemberInstanceReference result = null;
			if ((BaseReference)base.m_nextLeaf != (object)null)
			{
				result = (IOnDemandMemberInstanceReference)base.m_nextLeaf;
			}
			return result;
		}

		public IOnDemandScopeInstance GetCellInstance(IOnDemandMemberInstanceReference outerGroupInstanceRef, out IReference<IOnDemandScopeInstance> cellScopeRef)
		{
			RuntimeCell result = null;
			cellScopeRef = null;
			RuntimeDataTablixGroupLeafObjReference runtimeDataTablixGroupLeafObjReference = (RuntimeDataTablixGroupLeafObjReference)outerGroupInstanceRef;
			RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = runtimeDataTablixGroupLeafObjReference.Value();
			int hierarchyDynamicIndex = runtimeDataTablixGroupLeafObj.MemberDef.HierarchyDynamicIndex;
			RuntimeCells runtimeCells = this.m_cellsList[hierarchyDynamicIndex];
			if (runtimeCells != null)
			{
				RuntimeCellReference runtimeCellReference = default(RuntimeCellReference);
				result = runtimeCells.GetCell(runtimeDataTablixGroupLeafObj.GroupLeafIndex, out runtimeCellReference);
				cellScopeRef = runtimeCellReference;
			}
			return result;
		}

		public IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			return this.GetNestedDataRegion(rifDataRegion);
		}

		internal abstract RuntimeDataTablixObjReference GetNestedDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion);

		public IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			if (scope.IsGroup)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember = scope as AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
				return RuntimeDataRegionObj.GetGroupRoot(rifMember, this.m_memberObjs);
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion = scope as AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion;
			return this.GetNestedDataRegion(rifDataRegion);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeDataTablixGroupLeafObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MemberObjs:
					writer.Write(this.m_memberObjs);
					break;
				case MemberName.HasInnerHierarchy:
					writer.Write(this.m_hasInnerHierarchy);
					break;
				case MemberName.FirstPassCellNonCustomAggs:
					writer.Write(this.m_firstPassCellNonCustomAggs);
					break;
				case MemberName.FirstPassCellCustomAggs:
					writer.Write(this.m_firstPassCellCustomAggs);
					break;
				case MemberName.CellsList:
					writer.Write(this.m_cellsList);
					break;
				case MemberName.CellPostSortAggregates:
					writer.Write(this.m_cellPostSortAggregates);
					break;
				case MemberName.GroupLeafIndex:
					writer.Write(this.m_groupLeafIndex);
					break;
				case MemberName.ProcessHeading:
					writer.Write(this.m_processHeading);
					break;
				case MemberName.SequentialMemberIndexWithinScopeLevel:
					writer.Write(this.m_sequentialMemberIndexWithinScopeLevel);
					break;
				case MemberName.RunningValueValues:
					writer.Write(this.m_runningValueValues);
					break;
				case MemberName.RunningValueOfAggregateValues:
					writer.Write(this.m_runningValueOfAggregateValues);
					break;
				case MemberName.CellRunningValueValues:
					writer.Write(this.m_cellRunningValueValues);
					break;
				case MemberName.InstanceIndex:
					writer.Write(this.m_instanceIndex);
					break;
				case MemberName.ScopeInstanceNumber:
					writer.Write(this.m_scopeInstanceNumber);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(RuntimeDataTablixGroupLeafObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MemberObjs:
					this.m_memberObjs = reader.ReadArrayOfRIFObjects<IReference<RuntimeMemberObj>>();
					break;
				case MemberName.HasInnerHierarchy:
					this.m_hasInnerHierarchy = reader.ReadBoolean();
					break;
				case MemberName.FirstPassCellNonCustomAggs:
					this.m_firstPassCellNonCustomAggs = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.FirstPassCellCustomAggs:
					this.m_firstPassCellCustomAggs = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CellsList:
					this.m_cellsList = reader.ReadArrayOfRIFObjects<RuntimeCells>();
					break;
				case MemberName.CellPostSortAggregates:
					this.m_cellPostSortAggregates = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.GroupLeafIndex:
					this.m_groupLeafIndex = reader.ReadInt32();
					break;
				case MemberName.ProcessHeading:
					this.m_processHeading = reader.ReadBoolean();
					break;
				case MemberName.SequentialMemberIndexWithinScopeLevel:
					this.m_sequentialMemberIndexWithinScopeLevel = reader.ReadInt32();
					break;
				case MemberName.RunningValueValues:
					this.m_runningValueValues = reader.ReadArrayOfRIFObjects<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValueOfAggregateValues:
					this.m_runningValueOfAggregateValues = reader.ReadArrayOfRIFObjects<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.CellRunningValueValues:
					this.m_cellRunningValueValues = reader.ReadArrayOfRIFObjects<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.InstanceIndex:
					this.m_instanceIndex = reader.ReadInt32();
					break;
				case MemberName.ScopeInstanceNumber:
					this.m_scopeInstanceNumber = reader.ReadInt64();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeDataTablixGroupLeafObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.MemberObjs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference));
				list.Add(new MemberInfo(MemberName.HasInnerHierarchy, Token.Boolean));
				list.Add(new MemberInfo(MemberName.FirstPassCellNonCustomAggs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.FirstPassCellCustomAggs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CellsList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells));
				list.Add(new MemberInfo(MemberName.CellPostSortAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.GroupLeafIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.ProcessHeading, Token.Boolean));
				list.Add(new MemberInfo(MemberName.SequentialMemberIndexWithinScopeLevel, Token.Int32));
				list.Add(new MemberInfo(MemberName.RunningValueValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.CellRunningValueValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.InstanceIndex, Token.Int32));
				list.Add(new MemberInfo(MemberName.RunningValueOfAggregateValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.ScopeInstanceNumber, Token.Int64));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj, list);
			}
			return RuntimeDataTablixGroupLeafObj.m_declaration;
		}
	}
}
