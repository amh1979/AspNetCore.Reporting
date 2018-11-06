using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeGroupLeafObj : RuntimeGroupObj, IDataRowHolder
	{
		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_nonCustomAggregates;

		protected BucketedDataAggregateObjs m_aggregatesOfAggregates;

		protected BucketedDataAggregateObjs m_postSortAggregatesOfAggregates;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_customAggregates;

		protected DataFieldRow m_firstRow;

		protected bool m_firstRowIsAggregate;

		protected RuntimeGroupLeafObjReference m_nextLeaf;

		protected RuntimeGroupLeafObjReference m_prevLeaf;

		protected ScalableList<DataFieldRow> m_dataRows;

		protected IReference<RuntimeGroupObj> m_parent;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_recursiveAggregates;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_postSortAggregates;

		protected int m_recursiveLevel;

		protected List<object> m_groupExprValues;

		protected bool[] m_targetScopeMatched;

		protected DataActions m_dataAction;

		protected RuntimeUserSortTargetInfo m_userSortTargetInfo;

		protected int[] m_sortFilterExpressionScopeInfoIndices;

		protected object[] m_variableValues;

		protected int m_detailRowCounter;

		protected List<IHierarchyObj> m_detailSortAdditionalGroupLeafs;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode m_hierarchyDef;

		protected bool m_isOuterGrouping;

		protected bool m_hasProcessedAggregateRow;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeGroupLeafObj.GetDeclaration();

		internal bool IsOuterGrouping
		{
			get
			{
				return this.m_isOuterGrouping;
			}
		}

		internal RuntimeGroupLeafObjReference NextLeaf
		{
			set
			{
				this.m_nextLeaf = value;
			}
		}

		internal RuntimeGroupLeafObjReference PrevLeaf
		{
			set
			{
				this.m_prevLeaf = value;
			}
		}

		internal IReference<RuntimeGroupObj> Parent
		{
			get
			{
				return this.m_parent;
			}
			set
			{
				this.m_parent = value;
			}
		}

		protected override IReference<IScope> OuterScope
		{
			get
			{
				return base.m_hierarchyRoot;
			}
		}

		internal override int RecursiveLevel
		{
			get
			{
				return this.m_recursiveLevel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode MemberDef
		{
			get
			{
				return this.m_hierarchyDef;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping GroupingDef
		{
			get
			{
				return this.m_hierarchyDef.Grouping;
			}
		}

		protected override string ScopeName
		{
			get
			{
				return this.m_hierarchyDef.Grouping.Name;
			}
		}

		protected override IReference<IHierarchyObj> HierarchyRoot
		{
			get
			{
				if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).ProcessingStage)
				{
					return (RuntimeGroupLeafObjReference)base.m_selfReference;
				}
				return base.m_hierarchyRoot;
			}
		}

		protected override BTree SortTree
		{
			get
			{
				if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).ProcessingStage)
				{
					if (this.m_userSortTargetInfo != null)
					{
						return this.m_userSortTargetInfo.SortTree;
					}
					return null;
				}
				return base.m_grouping.Tree;
			}
		}

		protected override int ExpressionIndex
		{
			get
			{
				if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).ProcessingStage)
				{
					return 0;
				}
				Global.Tracer.Assert(false);
				return -1;
			}
		}

		protected override List<int> SortFilterInfoIndices
		{
			get
			{
				if (this.m_userSortTargetInfo != null)
				{
					return this.m_userSortTargetInfo.SortFilterInfoIndices;
				}
				return null;
			}
		}

		protected RuntimeGroupRootObjReference GroupRoot
		{
			get
			{
				return base.m_hierarchyRoot as RuntimeGroupRootObjReference;
			}
		}

		internal DataFieldRow FirstRow
		{
			get
			{
				return this.m_firstRow;
			}
		}

		internal override bool TargetForNonDetailSort
		{
			get
			{
				if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					return true;
				}
				return base.m_hierarchyRoot.Value().TargetForNonDetailSort;
			}
		}

		protected override int[] SortFilterExpressionScopeInfoIndices
		{
			get
			{
				if (this.m_sortFilterExpressionScopeInfoIndices == null)
				{
					this.m_sortFilterExpressionScopeInfoIndices = new int[base.m_odpContext.RuntimeSortFilterInfo.Count];
					for (int i = 0; i < base.m_odpContext.RuntimeSortFilterInfo.Count; i++)
					{
						this.m_sortFilterExpressionScopeInfoIndices[i] = -1;
					}
				}
				return this.m_sortFilterExpressionScopeInfoIndices;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				return this.m_hierarchyDef;
			}
		}

		internal int DetailSortRowCounter
		{
			get
			{
				return this.m_detailRowCounter;
			}
		}

		internal ScalableList<DataFieldRow> DataRows
		{
			get
			{
				return this.m_dataRows;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_nonCustomAggregates) + ItemSizes.SizeOf(this.m_customAggregates) + ItemSizes.SizeOf(this.m_firstRow) + 1 + ItemSizes.SizeOf(this.m_nextLeaf) + ItemSizes.SizeOf(this.m_prevLeaf) + ItemSizes.SizeOf(this.m_dataRows) + ItemSizes.SizeOf(this.m_parent) + ItemSizes.SizeOf(this.m_recursiveAggregates) + ItemSizes.SizeOf(this.m_postSortAggregates) + 4 + ItemSizes.SizeOf(this.m_groupExprValues) + ItemSizes.SizeOf(this.m_targetScopeMatched) + 4 + ItemSizes.SizeOf(this.m_userSortTargetInfo) + ItemSizes.SizeOf(this.m_sortFilterExpressionScopeInfoIndices) + 1 + ItemSizes.SizeOf(this.m_variableValues) + 4 + ItemSizes.SizeOf(this.m_detailSortAdditionalGroupLeafs) + ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_aggregatesOfAggregates) + ItemSizes.SizeOf(this.m_postSortAggregatesOfAggregates) + 1;
			}
		}

		protected RuntimeGroupLeafObj()
		{
		}

		protected RuntimeGroupLeafObj(RuntimeGroupRootObjReference groupRootRef, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef.Value().OdpContext, objectType, ((IScope)groupRootRef.Value()).Depth + 1)
		{
			RuntimeGroupRootObj runtimeGroupRootObj = groupRootRef.Value();
			this.m_hierarchyDef = runtimeGroupRootObj.HierarchyDef;
			base.m_hierarchyRoot = groupRootRef;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = this.m_hierarchyDef.Grouping;
			RuntimeDataRegionObj.CreateAggregates(base.m_odpContext, grouping.Aggregates, ref this.m_nonCustomAggregates, ref this.m_customAggregates);
			RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, grouping.RecursiveAggregates, ref this.m_recursiveAggregates);
			RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, grouping.PostSortAggregates, ref this.m_postSortAggregates);
			if (this.m_hierarchyDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = this.m_hierarchyDef.DataScopeInfo;
				RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, (BucketedAggregatesCollection<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>)dataScopeInfo.AggregatesOfAggregates, ref this.m_aggregatesOfAggregates);
				RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, (BucketedAggregatesCollection<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>)dataScopeInfo.PostSortAggregatesOfAggregates, ref this.m_postSortAggregatesOfAggregates);
			}
			if (runtimeGroupRootObj.SaveGroupExprValues)
			{
				this.m_groupExprValues = grouping.CurrentGroupExpressionValues;
			}
			RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = runtimeGroupRootObj as RuntimeDataTablixGroupRootObj;
			this.m_isOuterGrouping = (runtimeDataTablixGroupRootObj != null && runtimeDataTablixGroupRootObj.InnerGroupings != null);
		}

		internal override bool IsTargetForSort(int index, bool detailSort)
		{
			if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.IsTargetForSort(index, detailSort))
			{
				return true;
			}
			return base.m_hierarchyRoot.Value().IsTargetForSort(index, detailSort);
		}

		protected virtual void ConstructRuntimeStructure(ref bool handleMyDataAction, out DataActions innerDataAction)
		{
			if (this.m_postSortAggregates != null)
			{
				handleMyDataAction = true;
			}
			if (this.m_recursiveAggregates != null && (base.m_odpContext.SpecialRecursiveAggregates || this.MemberDef.HasInnerDynamic))
			{
				handleMyDataAction = true;
			}
			if (handleMyDataAction)
			{
				innerDataAction = DataActions.None;
			}
			else
			{
				innerDataAction = ((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).DataAction;
			}
		}

		protected bool HandleSortFilterEvent(bool isColumnAxis)
		{
			if (base.m_odpContext.RuntimeSortFilterInfo == null)
			{
				return false;
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping groupingDef = this.GroupingDef;
			if (groupingDef.IsDetail)
			{
				return false;
			}
			int count = base.m_odpContext.RuntimeSortFilterInfo.Count;
			if (groupingDef.SortFilterScopeMatched != null || groupingDef.NeedScopeInfoForSortFilterExpression != null)
			{
				this.m_targetScopeMatched = new bool[count];
				for (int i = 0; i < count; i++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = base.m_odpContext.RuntimeSortFilterInfo[i];
					using (reference.PinValue())
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
						if (groupingDef.SortFilterScopeMatched != null && groupingDef.SortFilterScopeIndex != null && -1 != groupingDef.SortFilterScopeIndex[i])
						{
							this.m_targetScopeMatched[i] = groupingDef.SortFilterScopeMatched[i];
							if (this.m_targetScopeMatched[i])
							{
								if (groupingDef.IsSortFilterTarget != null && groupingDef.IsSortFilterTarget[i] && !base.m_hierarchyRoot.Value().TargetForNonDetailSort)
								{
									runtimeSortFilterEventInfo.EventTarget = (IReference<IHierarchyObj>)base.SelfReference;
									if (this.m_userSortTargetInfo == null)
									{
										this.m_userSortTargetInfo = new RuntimeUserSortTargetInfo((IReference<IHierarchyObj>)base.SelfReference, i, reference);
									}
									else
									{
										this.m_userSortTargetInfo.AddSortInfo((IReference<IHierarchyObj>)base.SelfReference, i, reference);
									}
								}
								Global.Tracer.Assert(null != runtimeSortFilterEventInfo.EventSource.ContainingScopes, "(null != sortFilterInfo.EventSource.ContainingScopes)");
								if (groupingDef == runtimeSortFilterEventInfo.EventSource.ContainingScopes.LastEntry && !runtimeSortFilterEventInfo.EventSource.IsTablixCellScope && !base.m_hierarchyRoot.Value().TargetForNonDetailSort)
								{
									runtimeSortFilterEventInfo.SetEventSourceScope(isColumnAxis, base.SelfReference, -1);
								}
							}
						}
						else
						{
							this.m_targetScopeMatched[i] = ((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).TargetScopeMatched(i, false);
						}
					}
				}
			}
			base.m_odpContext.RegisterSortFilterExpressionScope(base.m_hierarchyRoot, base.SelfReference, groupingDef.IsSortFilterExpressionScope);
			if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.TargetForNonDetailSort)
			{
				return true;
			}
			return false;
		}

		internal override void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			if (this.GroupingDef.IsDetail)
			{
				base.DetailGetScopeValues(this.GroupRoot.Value().GroupRootOuterScope, targetScopeObj, scopeValues, ref index);
			}
			else
			{
				if (targetScopeObj != null && this == targetScopeObj.Value())
				{
					return;
				}
				using (base.m_hierarchyRoot.PinValue())
				{
					((RuntimeDataRegionObj)base.m_hierarchyRoot.Value()).GetScopeValues(targetScopeObj, scopeValues, ref index);
				}
				Global.Tracer.Assert(null != this.m_groupExprValues, "(null != m_groupExprValues)");
				Global.Tracer.Assert(index < scopeValues.Length, "(index < scopeValues.Length)");
				scopeValues[index++] = this.m_groupExprValues;
			}
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			if (this.GroupingDef.IsDetail)
			{
				RuntimeGroupRootObj runtimeGroupRootObj = this.GroupRoot.Value();
				return base.DetailTargetScopeMatched(this.MemberDef.DataRegionDef, runtimeGroupRootObj.GroupRootOuterScope, runtimeGroupRootObj.HierarchyDef.IsColumn, index);
			}
			if (detailSort && this.GroupingDef.SortFilterScopeInfo == null)
			{
				return true;
			}
			if (this.m_targetScopeMatched != null)
			{
				return this.m_targetScopeMatched[index];
			}
			return false;
		}

		protected void UpdateAggregateInfo()
		{
			FieldsImpl fieldsImpl = base.m_odpContext.ReportObjectModel.FieldsImpl;
			if (fieldsImpl.ValidAggregateRow)
			{
				int[] groupExpressionFieldIndices = this.GroupingDef.GetGroupExpressionFieldIndices();
				if (groupExpressionFieldIndices != null)
				{
					foreach (int num in groupExpressionFieldIndices)
					{
						if (num >= 0)
						{
							fieldsImpl.ConsumeAggregationField(num);
						}
					}
				}
				if (fieldsImpl.AggregationFieldCount == 0 && this.m_customAggregates != null)
				{
					if (!this.IsOuterGrouping && base.m_odpContext.PeerOuterGroupProcessing)
					{
						return;
					}
					this.m_hasProcessedAggregateRow = true;
					RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, this.m_customAggregates, false);
				}
			}
		}

		protected void InternalNextRow()
		{
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = base.m_hierarchyRoot.Value() as RuntimeGroupRootObj;
				ProcessingStages processingStage = runtimeGroupRootObj.ProcessingStage;
				runtimeGroupRootObj.ProcessingStage = ProcessingStages.UserSortFilter;
				if (runtimeGroupRootObj.IsDetailGroup)
				{
					base.DetailHandleSortFilterEvent(this.MemberDef.DataRegionDef, runtimeGroupRootObj.GroupRootOuterScope, runtimeGroupRootObj.HierarchyDef.IsColumn, base.m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex());
				}
				RuntimeDataRegionObj.CommonFirstRow(base.m_odpContext, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
				if (this.IsOuterGrouping || !base.m_odpContext.PeerOuterGroupProcessing)
				{
					if (base.m_odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
					{
						base.ScopeNextAggregateRow(this.m_userSortTargetInfo);
						if (this.MemberDef.DataRegionDef.IsMatrixIDC)
						{
							AggregateRow aggregateRow = new AggregateRow(base.m_odpContext.ReportObjectModel.FieldsImpl, true);
							aggregateRow.SaveAggregateInfo(base.m_odpContext);
							this.m_dataRows.Add(aggregateRow);
						}
					}
					else
					{
						base.ScopeNextNonAggregateRow(this.m_nonCustomAggregates, this.m_dataRows);
					}
				}
				else
				{
					this.SendToInner();
				}
				runtimeGroupRootObj.ProcessingStage = processingStage;
			}
		}

		protected override void SendToInner()
		{
			using (base.m_hierarchyRoot.PinValue())
			{
				((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).ProcessingStage = ProcessingStages.Grouping;
			}
		}

		internal void RemoveFromParent(RuntimeGroupObjReference parentRef)
		{
			using (parentRef.PinValue())
			{
				RuntimeGroupObj runtimeGroupObj = parentRef.Value();
				if ((BaseReference)null == (object)this.m_prevLeaf)
				{
					runtimeGroupObj.FirstChild = this.m_nextLeaf;
				}
				else
				{
					using (this.m_prevLeaf.PinValue())
					{
						this.m_prevLeaf.Value().m_nextLeaf = this.m_nextLeaf;
					}
				}
				if ((BaseReference)null == (object)this.m_nextLeaf)
				{
					runtimeGroupObj.LastChild = this.m_prevLeaf;
				}
				else
				{
					using (this.m_nextLeaf.PinValue())
					{
						this.m_nextLeaf.Value().m_prevLeaf = this.m_prevLeaf;
					}
				}
			}
		}

		private IReference<RuntimeGroupLeafObj> Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			IReference<RuntimeGroupLeafObj> nextLeaf = this.m_nextLeaf;
			if (((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).HasParent)
			{
				this.m_recursiveLevel = this.m_parent.Value().RecursiveLevel + 1;
			}
			bool flag = this.IsSpecialFilteringPass(operation);
			if (flag)
			{
				base.m_lastChild = null;
				this.ProcessChildren(operation, traversalContext);
			}
			switch (operation)
			{
			case ProcessingStages.SortAndFilter:
				this.SortAndFilter((AggregateUpdateContext)traversalContext);
				break;
			case ProcessingStages.PreparePeerGroupRunningValues:
				this.PrepareCalculateRunningValues();
				break;
			case ProcessingStages.RunningValues:
			{
				bool flag2 = false;
				if (base.m_odpContext.HasPreviousAggregates)
				{
					flag2 = ((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).IsDetailGroup;
					if (this.m_groupExprValues != null && !flag2)
					{
						base.m_odpContext.GroupExpressionValues.AddRange(this.m_groupExprValues);
					}
				}
				this.CalculateRunningValues((AggregateUpdateContext)traversalContext);
				if (base.m_odpContext.HasPreviousAggregates && this.m_groupExprValues != null && !flag2)
				{
					base.m_odpContext.GroupExpressionValues.RemoveRange(base.m_odpContext.GroupExpressionValues.Count - this.m_groupExprValues.Count, this.m_groupExprValues.Count);
				}
				break;
			}
			case ProcessingStages.CreateGroupTree:
				this.CreateInstance((CreateInstancesTraversalContext)traversalContext);
				break;
			case ProcessingStages.UpdateAggregates:
				this.UpdateAggregates((AggregateUpdateContext)traversalContext);
				break;
			}
			if (!flag)
			{
				this.ProcessChildren(operation, traversalContext);
			}
			return nextLeaf;
		}

		internal void TraverseAllLeafNodes(ProcessingStages operation, ITraversalContext traversalContext)
		{
			IReference<RuntimeGroupLeafObj> reference = base.SelfReference as IReference<RuntimeGroupLeafObj>;
			while (reference != null)
			{
				this.TablixProcessingMoveNext(operation);
				using (reference.PinValue())
				{
					reference = reference.Value().Traverse(operation, traversalContext);
				}
			}
		}

		protected void TablixProcessingMoveNext(ProcessingStages operation)
		{
			if (operation == ProcessingStages.CreateGroupTree)
			{
				if (!base.m_odpContext.ReportDefinition.ReportOrDescendentHasUserSortFilter && !base.m_odpContext.ReportDefinition.HasSubReports)
				{
					return;
				}
				this.MemberDef.MoveNextForUserSort(base.m_odpContext);
			}
		}

		private void ProcessChildren(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (!((BaseReference)null != (object)base.m_firstChild) && base.m_grouping == null)
			{
				return;
			}
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot.Value();
				if ((BaseReference)null != (object)base.m_firstChild)
				{
					using (base.m_firstChild.PinValue())
					{
						base.m_firstChild.Value().TraverseAllLeafNodes(operation, traversalContext);
					}
					if (operation == ProcessingStages.SortAndFilter)
					{
						if ((SecondPassOperations.FilteringOrAggregatesOrDomainScope & base.m_odpContext.SecondPassOperation) != 0 && runtimeGroupRootObj.HierarchyDef.Grouping.Filters != null)
						{
							if ((BaseReference)null == (object)base.m_lastChild)
							{
								base.m_firstChild = null;
							}
						}
						else if (base.m_grouping != null)
						{
							base.m_firstChild = null;
						}
					}
				}
				else if (base.m_grouping != null)
				{
					base.m_grouping.Traverse(operation, runtimeGroupRootObj.Expression.Direction, traversalContext);
				}
			}
		}

		private bool IsSpecialFilteringPass(ProcessingStages operation)
		{
			if (ProcessingStages.SortAndFilter == operation && base.m_odpContext.SpecialRecursiveAggregates && (SecondPassOperations.FilteringOrAggregatesOrDomainScope & base.m_odpContext.SecondPassOperation) != 0)
			{
				return true;
			}
			return false;
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			bool flag = true;
			bool flag2 = false;
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot.Value();
				Global.Tracer.Assert(null != runtimeGroupRootObj, "(null != groupRoot)");
				if (this.MemberDef.Grouping.Variables != null)
				{
					ScopeInstance.ResetVariables(base.m_odpContext, this.MemberDef.Grouping.Variables);
					ScopeInstance.CalculateVariables(base.m_odpContext, this.MemberDef.Grouping.Variables, out this.m_variableValues);
				}
				if (runtimeGroupRootObj.ProcessSecondPassSorting && (BaseReference)null != (object)base.m_firstChild)
				{
					base.m_expression = runtimeGroupRootObj.Expression;
					base.m_grouping = new RuntimeGroupingObjTree(this, base.m_objectType);
				}
				base.m_lastChild = null;
				if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
				{
					if (base.m_odpContext.SpecialRecursiveAggregates && this.m_recursiveAggregates != null)
					{
						Global.Tracer.Assert(null != this.m_dataRows, "(null != m_dataRows)");
						this.ReadRows(false);
					}
					if (runtimeGroupRootObj.GroupFilters != null)
					{
						this.SetupEnvironment();
						flag = runtimeGroupRootObj.GroupFilters.PassFilters((object)base.SelfReference, out flag2);
					}
				}
				if (flag)
				{
					this.PostFilterNextRow(aggContext);
					return flag;
				}
				if (!flag2)
				{
					this.FailFilter();
					return flag;
				}
				return flag;
			}
		}

		internal void FailFilter()
		{
			RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = null;
			bool flag = false;
			if (this.IsSpecialFilteringPass(ProcessingStages.SortAndFilter))
			{
				flag = true;
			}
			if ((BaseReference)base.m_firstChild != (object)null)
			{
				using (this.m_parent.PinValue())
				{
					RuntimeGroupObj runtimeGroupObj = this.m_parent.Value();
					RuntimeGroupLeafObjReference runtimeGroupLeafObjReference2 = base.m_firstChild;
					while ((BaseReference)runtimeGroupLeafObjReference2 != (object)null)
					{
						using (runtimeGroupLeafObjReference2.PinValue())
						{
							RuntimeGroupLeafObj runtimeGroupLeafObj = runtimeGroupLeafObjReference2.Value();
							runtimeGroupLeafObjReference = runtimeGroupLeafObj.m_nextLeaf;
							runtimeGroupLeafObj.m_parent = this.m_parent;
							if (flag)
							{
								runtimeGroupObj.AddChild(runtimeGroupLeafObjReference2);
							}
						}
						runtimeGroupLeafObjReference2 = runtimeGroupLeafObjReference;
					}
				}
			}
		}

		internal virtual void PostFilterNextRow(AggregateUpdateContext context)
		{
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot.Value();
				if ((SecondPassOperations.FilteringOrAggregatesOrDomainScope & base.m_odpContext.SecondPassOperation) != 0 && this.m_dataRows != null && (this.m_dataAction & DataActions.RecursiveAggregates) != 0)
				{
					if (base.m_odpContext.SpecialRecursiveAggregates)
					{
						this.ReadRows(true);
					}
					else
					{
						this.ReadRows(DataActions.RecursiveAggregates, null);
					}
					base.ReleaseDataRows(DataActions.RecursiveAggregates, ref this.m_dataAction, ref this.m_dataRows);
				}
				bool processSecondPassSorting = runtimeGroupRootObj.ProcessSecondPassSorting;
				if (processSecondPassSorting)
				{
					this.SetupEnvironment();
				}
				if (processSecondPassSorting || runtimeGroupRootObj.GroupFilters != null)
				{
					this.m_nextLeaf = null;
					using (this.m_parent.PinValue())
					{
						this.m_parent.Value().InsertToSortTree((RuntimeGroupLeafObjReference)base.m_selfReference);
					}
				}
			}
		}

		protected abstract void PrepareCalculateRunningValues();

		internal override void CalculateRunningValues(AggregateUpdateContext aggContext)
		{
			this.ResetScopedRunningValues();
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			Global.Tracer.Assert(DataActions.UserSort != dataAction, "(DataActions.UserSort != dataAction)");
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregatesOfAggregates))
			{
				AggregateUpdateContext aggregateUpdateContext = (AggregateUpdateContext)context;
				aggregateUpdateContext.UpdateAggregatesForRow();
			}
			if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates))
			{
				if (this.m_postSortAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, this.m_postSortAggregates, false);
				}
				Global.Tracer.Assert((BaseReference)null != (object)base.m_hierarchyRoot, "(null != m_hierarchyRoot)");
				using (base.m_hierarchyRoot.PinValue())
				{
					IScope scope = base.m_hierarchyRoot.Value();
					scope.ReadRow(DataActions.PostSortAggregates, context);
				}
			}
			else if (FlagUtils.HasFlag(dataAction, DataActions.AggregatesOfAggregates))
			{
				AggregateUpdateContext aggregateUpdateContext2 = (AggregateUpdateContext)context;
				aggregateUpdateContext2.UpdateAggregatesForRow();
			}
			else
			{
				Global.Tracer.Assert(DataActions.RecursiveAggregates == dataAction, "(DataActions.RecursiveAggregates == dataAction)");
				if (this.m_recursiveAggregates != null)
				{
					RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, this.m_recursiveAggregates, false);
				}
				using (this.m_parent.PinValue())
				{
					IScope scope2 = this.m_parent.Value();
					scope2.ReadRow(DataActions.RecursiveAggregates, context);
				}
			}
		}

		private void ReadRow(bool sendToParent)
		{
			if (!sendToParent)
			{
				Global.Tracer.Assert(null != this.m_recursiveAggregates, "(null != m_recursiveAggregates)");
				RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, this.m_recursiveAggregates, false);
			}
			else
			{
				using (this.m_parent.PinValue())
				{
					IScope scope = this.m_parent.Value();
					scope.ReadRow(DataActions.RecursiveAggregates, null);
				}
			}
		}

		public override void SetupEnvironment()
		{
			if (this.m_hierarchyDef.DataScopeInfo != null && this.m_hierarchyDef.DataScopeInfo.DataSet != null)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = this.m_hierarchyDef.DataScopeInfo.DataSet;
				base.SetupNewDataSet(dataSet);
				if (this.m_hierarchyDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext != null)
				{
					base.m_odpContext.ReportObjectModel.RestoreFields(this.m_hierarchyDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext);
				}
			}
			base.SetupEnvironment(this.m_nonCustomAggregates, this.m_customAggregates, this.m_firstRow);
			this.MemberDef.SetUserSortDetailRowIndex(base.m_odpContext);
			base.SetupAggregates(this.m_aggregatesOfAggregates);
			base.SetupAggregates(this.m_postSortAggregatesOfAggregates);
			base.SetupAggregates(this.m_recursiveAggregates);
			base.SetupAggregates(this.m_postSortAggregates);
			RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot.Value();
			if (runtimeGroupRootObj.HasParent)
			{
				this.GroupingDef.RecursiveLevel = this.m_recursiveLevel;
			}
			if (runtimeGroupRootObj.SaveGroupExprValues)
			{
				this.GroupingDef.CurrentGroupExpressionValues = this.m_groupExprValues;
			}
			this.SetupGroupVariables();
		}

		internal void SetupGroupVariables()
		{
			if (this.m_variableValues != null)
			{
				ScopeInstance.SetupVariables(base.m_odpContext, this.GroupingDef.Variables, this.m_variableValues);
			}
		}

		internal void CalculatePreviousAggregates(bool setupEnvironment)
		{
			if (setupEnvironment)
			{
				this.SetupEnvironment();
			}
			using (base.m_hierarchyRoot.PinValue())
			{
				((IScope)base.m_hierarchyRoot.Value()).CalculatePreviousAggregates();
			}
		}

		internal override void CalculatePreviousAggregates()
		{
			this.CalculatePreviousAggregates(true);
		}

		public void ReadRows(DataActions action, ITraversalContext context)
		{
			if (this.m_dataRows != null)
			{
				for (int i = 0; i < this.m_dataRows.Count; i++)
				{
					DataFieldRow dataFieldRow = this.m_dataRows[i];
					dataFieldRow.SetFields(base.m_odpContext.ReportObjectModel.FieldsImpl);
					this.ReadRow(action, context);
				}
			}
		}

		private void ReadRows(bool sendToParent)
		{
			for (int i = 0; i < this.m_dataRows.Count; i++)
			{
				DataFieldRow dataFieldRow = this.m_dataRows[i];
				dataFieldRow.SetFields(base.m_odpContext.ReportObjectModel.FieldsImpl);
				this.ReadRow(sendToParent);
			}
		}

		protected virtual void ResetScopedRunningValues()
		{
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot.Value();
				if (runtimeGroupRootObj.ScopedRunningValues != null)
				{
					AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
					foreach (string scopedRunningValue in runtimeGroupRootObj.ScopedRunningValues)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(scopedRunningValue);
						Global.Tracer.Assert(aggregateObj != null, "Expected aggregate: {0} not in global collection", scopedRunningValue);
						aggregateObj.Init();
					}
				}
			}
		}

		internal override bool InScope(string scope)
		{
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot.Value();
				AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					bool result = default(bool);
					grouping.ScopeNames = base.GetScopeNames(base.SelfReference, scope, out result);
					return result;
				}
				return grouping.ScopeNames.Contains(scope);
			}
		}

		protected override int GetRecursiveLevel(string scope)
		{
			if (scope == null)
			{
				return this.m_recursiveLevel;
			}
			using (base.m_hierarchyRoot.PinValue())
			{
				RuntimeGroupRootObj runtimeGroupRootObj = (RuntimeGroupRootObj)base.m_hierarchyRoot.Value();
				AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = runtimeGroupRootObj.HierarchyDef.Grouping;
				if (grouping.ScopeNames == null)
				{
					int result = default(int);
					grouping.ScopeNames = base.GetScopeNames(base.SelfReference, scope, out result);
					return result;
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping2 = grouping.ScopeNames[scope] as AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping;
				if (grouping2 != null)
				{
					return grouping2.RecursiveLevel;
				}
				return -1;
			}
		}

		protected override void ProcessUserSort()
		{
			using (base.m_hierarchyRoot.PinValue())
			{
				((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).ProcessingStage = ProcessingStages.UserSortFilter;
			}
			base.m_odpContext.ProcessUserSortForTarget((IReference<IHierarchyObj>)base.SelfReference, ref this.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
			if (this.m_userSortTargetInfo.TargetForNonDetailSort)
			{
				this.m_dataAction &= ~DataActions.UserSort;
				this.m_userSortTargetInfo.ResetTargetForNonDetailSort();
				this.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_odpContext);
				bool flag = false;
				DataActions dataActions = default(DataActions);
				this.ConstructRuntimeStructure(ref flag, out dataActions);
				if (!flag)
				{
					Global.Tracer.Assert(dataActions == this.m_dataAction, "(innerDataAction == m_dataAction)");
				}
				if (this.m_dataAction != 0)
				{
					this.m_dataRows = new ScalableList<DataFieldRow>(base.Depth, base.m_odpContext.TablixProcessingScalabilityCache);
				}
				base.ScopeFinishSorting(ref this.m_firstRow, this.m_userSortTargetInfo);
				this.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_odpContext);
			}
		}

		protected override void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			if (this.m_userSortTargetInfo != null)
			{
				this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, (IReference<IHierarchyObj>)base.SelfReference);
			}
		}

		protected override void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			if (this.m_userSortTargetInfo != null)
			{
				this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
		}

		public override IHierarchyObj CreateHierarchyObjForSortTree()
		{
			if (ProcessingStages.UserSortFilter == ((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).ProcessingStage)
			{
				return new RuntimeSortHierarchyObj(this, base.m_depth + 1);
			}
			return base.CreateHierarchyObjForSortTree();
		}

		protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = ((RuntimeGroupRootObj)base.m_hierarchyRoot.Value()).HierarchyDef.Grouping;
			if (grouping.ScopeNames == null)
			{
				grouping.ScopeNames = base.GetScopeNames(base.SelfReference, pairs);
			}
			else
			{
				IEnumerator enumerator = grouping.ScopeNames.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					RuntimeDataRegionObj.AddGroupNameValuePair(base.m_odpContext, enumerator.Current as AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping, pairs);
				}
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeGroupLeafObj.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.NonCustomAggregates:
					writer.Write(this.m_nonCustomAggregates);
					break;
				case MemberName.CustomAggregates:
					writer.Write(this.m_customAggregates);
					break;
				case MemberName.FirstRow:
					writer.Write(this.m_firstRow);
					break;
				case MemberName.FirstRowIsAggregate:
					writer.Write(this.m_firstRowIsAggregate);
					break;
				case MemberName.NextLeaf:
					writer.Write(this.m_nextLeaf);
					break;
				case MemberName.PrevLeaf:
					writer.Write(this.m_prevLeaf);
					break;
				case MemberName.DataRows:
					writer.Write(this.m_dataRows);
					break;
				case MemberName.Parent:
					writer.Write(this.m_parent);
					break;
				case MemberName.RecursiveAggregates:
					writer.Write(this.m_recursiveAggregates);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(this.m_postSortAggregates);
					break;
				case MemberName.RecursiveLevel:
					writer.Write(this.m_recursiveLevel);
					break;
				case MemberName.GroupExprValues:
					writer.WriteListOfVariant(this.m_groupExprValues);
					break;
				case MemberName.TargetScopeMatched:
					writer.Write(this.m_targetScopeMatched);
					break;
				case MemberName.DataAction:
					writer.WriteEnum((int)this.m_dataAction);
					break;
				case MemberName.UserSortTargetInfo:
					writer.Write(this.m_userSortTargetInfo);
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					writer.Write(this.m_sortFilterExpressionScopeInfoIndices);
					break;
				case MemberName.HierarchyDef:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_hierarchyDef);
					writer.Write(value);
					break;
				}
				case MemberName.IsOuterGrouping:
					writer.Write(this.m_isOuterGrouping);
					break;
				case MemberName.Variables:
					writer.WriteSerializableArray(this.m_variableValues);
					break;
				case MemberName.DetailRowCounter:
					writer.Write(this.m_detailRowCounter);
					break;
				case MemberName.DetailSortAdditionalGroupLeafs:
					writer.Write(this.m_detailSortAdditionalGroupLeafs);
					break;
				case MemberName.AggregatesOfAggregates:
					writer.Write(this.m_aggregatesOfAggregates);
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					writer.Write(this.m_postSortAggregatesOfAggregates);
					break;
				case MemberName.HasProcessedAggregateRow:
					writer.Write(this.m_hasProcessedAggregateRow);
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
			reader.RegisterDeclaration(RuntimeGroupLeafObj.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.NonCustomAggregates:
					this.m_nonCustomAggregates = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CustomAggregates:
					this.m_customAggregates = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.FirstRow:
					this.m_firstRow = (DataFieldRow)reader.ReadRIFObject();
					break;
				case MemberName.FirstRowIsAggregate:
					this.m_firstRowIsAggregate = reader.ReadBoolean();
					break;
				case MemberName.NextLeaf:
					this.m_nextLeaf = (RuntimeGroupLeafObjReference)reader.ReadRIFObject();
					break;
				case MemberName.PrevLeaf:
					this.m_prevLeaf = (RuntimeGroupLeafObjReference)reader.ReadRIFObject();
					break;
				case MemberName.DataRows:
					this.m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				case MemberName.Parent:
					this.m_parent = (IReference<RuntimeGroupObj>)reader.ReadRIFObject();
					break;
				case MemberName.RecursiveAggregates:
					this.m_recursiveAggregates = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.PostSortAggregates:
					this.m_postSortAggregates = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.RecursiveLevel:
					this.m_recursiveLevel = reader.ReadInt32();
					break;
				case MemberName.GroupExprValues:
					this.m_groupExprValues = reader.ReadListOfVariant<List<object>>();
					break;
				case MemberName.TargetScopeMatched:
					this.m_targetScopeMatched = reader.ReadBooleanArray();
					break;
				case MemberName.DataAction:
					this.m_dataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.UserSortTargetInfo:
					this.m_userSortTargetInfo = (RuntimeUserSortTargetInfo)reader.ReadRIFObject();
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					this.m_sortFilterExpressionScopeInfoIndices = reader.ReadInt32Array();
					break;
				case MemberName.HierarchyDef:
				{
					int id = reader.ReadInt32();
					this.m_hierarchyDef = (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.IsOuterGrouping:
					this.m_isOuterGrouping = reader.ReadBoolean();
					break;
				case MemberName.Variables:
					this.m_variableValues = reader.ReadSerializableArray();
					break;
				case MemberName.DetailRowCounter:
					this.m_detailRowCounter = reader.ReadInt32();
					break;
				case MemberName.DetailSortAdditionalGroupLeafs:
					this.m_detailSortAdditionalGroupLeafs = reader.ReadListOfRIFObjects<List<IHierarchyObj>>();
					break;
				case MemberName.AggregatesOfAggregates:
					this.m_aggregatesOfAggregates = (BucketedDataAggregateObjs)reader.ReadRIFObject();
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					this.m_postSortAggregatesOfAggregates = (BucketedDataAggregateObjs)reader.ReadRIFObject();
					break;
				case MemberName.HasProcessedAggregateRow:
					this.m_hasProcessedAggregateRow = reader.ReadBoolean();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeGroupLeafObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.NonCustomAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CustomAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.FirstRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.FirstRowIsAggregate, Token.Boolean));
				list.Add(new MemberInfo(MemberName.NextLeaf, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.PrevLeaf, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.DataRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				list.Add(new MemberInfo(MemberName.Parent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference));
				list.Add(new MemberInfo(MemberName.RecursiveAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.PostSortAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.RecursiveLevel, Token.Int32));
				list.Add(new MemberInfo(MemberName.GroupExprValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantList));
				list.Add(new MemberInfo(MemberName.TargetScopeMatched, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Boolean));
				list.Add(new MemberInfo(MemberName.DataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.UserSortTargetInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeInfoIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.HierarchyDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.IsOuterGrouping, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Variables, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SerializableArray, Token.Serializable));
				list.Add(new MemberInfo(MemberName.DetailRowCounter, Token.Int32));
				list.Add(new MemberInfo(MemberName.DetailSortAdditionalGroupLeafs, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObj));
				list.Add(new MemberInfo(MemberName.AggregatesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.PostSortAggregatesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.HasProcessedAggregateRow, Token.Boolean));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj, list);
			}
			return RuntimeGroupLeafObj.m_declaration;
		}
	}
}
