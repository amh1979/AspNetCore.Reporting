using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeGroupRootObj : RuntimeGroupObj, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner, IStorable, IPersistable, IDataCorrelation
	{
		protected AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode m_hierarchyDef;

		protected IReference<IScope> m_outerScope;

		private ProcessingStages m_processingStage = ProcessingStages.Grouping;

		protected List<string> m_scopedRunningValues;

		protected List<string> m_runningValuesInGroup;

		protected List<string> m_previousValuesInGroup;

		protected Dictionary<string, IReference<RuntimeGroupRootObj>> m_groupCollection;

		protected DataActions m_dataAction;

		protected DataActions m_outerDataAction;

		protected RuntimeGroupingObj.GroupingTypes m_groupingType;

		[Reference]
		protected Filters m_groupFilters;

		protected RuntimeExpressionInfo m_parentExpression;

		protected object m_currentGroupExprValue;

		protected bool m_saveGroupExprValues = true;

		protected int[] m_sortFilterExpressionScopeInfoIndices;

		private bool[] m_builtinSortOverridden;

		protected bool m_isDetailGroup;

		protected RuntimeUserSortTargetInfo m_detailUserSortTargetInfo;

		protected ScalableList<DataFieldRow> m_detailDataRows;

		private static Declaration m_declaration = RuntimeGroupRootObj.GetDeclaration();

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode HierarchyDef
		{
			get
			{
				return this.m_hierarchyDef;
			}
		}

		internal List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> GroupExpressions
		{
			get
			{
				return this.m_hierarchyDef.Grouping.GroupExpressions;
			}
		}

		internal GroupExprHost GroupExpressionHost
		{
			get
			{
				return this.m_hierarchyDef.Grouping.ExprHost;
			}
		}

		internal List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> SortExpressions
		{
			get
			{
				if (this.m_hierarchyDef.Sorting != null && this.m_hierarchyDef.Sorting.ShouldApplySorting)
				{
					return this.m_hierarchyDef.Sorting.SortExpressions;
				}
				return null;
			}
		}

		internal SortExprHost SortExpressionHost
		{
			get
			{
				return this.m_hierarchyDef.Sorting.ExprHost;
			}
		}

		internal List<bool> GroupDirections
		{
			get
			{
				return this.m_hierarchyDef.Grouping.SortDirections;
			}
		}

		internal List<bool> SortDirections
		{
			get
			{
				return this.m_hierarchyDef.Sorting.SortDirections;
			}
		}

		internal RuntimeExpressionInfo Expression
		{
			get
			{
				return base.m_expression;
			}
		}

		internal List<string> ScopedRunningValues
		{
			get
			{
				return this.m_scopedRunningValues;
			}
		}

		internal Dictionary<string, IReference<RuntimeGroupRootObj>> GroupCollection
		{
			get
			{
				return this.m_groupCollection;
			}
		}

		internal DataActions DataAction
		{
			get
			{
				return this.m_dataAction;
			}
		}

		internal ProcessingStages ProcessingStage
		{
			get
			{
				return this.m_processingStage;
			}
			set
			{
				this.m_processingStage = value;
			}
		}

		internal DataRegionInstance DataRegionInstance
		{
			get
			{
				return this.m_hierarchyDef.DataRegionDef.CurrentDataRegionInstance;
			}
		}

		internal RuntimeGroupingObj.GroupingTypes GroupingType
		{
			get
			{
				return this.m_groupingType;
			}
		}

		internal Filters GroupFilters
		{
			get
			{
				return this.m_groupFilters;
			}
		}

		internal bool HasParent
		{
			get
			{
				return null != this.m_parentExpression;
			}
		}

		protected override IReference<IScope> OuterScope
		{
			get
			{
				return this.m_outerScope;
			}
		}

		internal IReference<IScope> GroupRootOuterScope
		{
			get
			{
				return this.m_outerScope;
			}
		}

		internal bool SaveGroupExprValues
		{
			get
			{
				return true;
			}
		}

		internal bool IsDetailGroup
		{
			get
			{
				return this.m_isDetailGroup;
			}
		}

		protected override bool IsDetail
		{
			get
			{
				if (this.m_isDetailGroup)
				{
					return true;
				}
				return base.IsDetail;
			}
		}

		protected override int ExpressionIndex
		{
			get
			{
				return 0;
			}
		}

		protected override List<int> SortFilterInfoIndices
		{
			get
			{
				if (this.m_detailUserSortTargetInfo != null)
				{
					return this.m_detailUserSortTargetInfo.SortFilterInfoIndices;
				}
				return null;
			}
		}

		internal BTree GroupOrDetailSortTree
		{
			get
			{
				return this.SortTree;
			}
		}

		protected override BTree SortTree
		{
			get
			{
				if (this.m_detailUserSortTargetInfo != null)
				{
					return this.m_detailUserSortTargetInfo.SortTree;
				}
				if (base.m_grouping != null)
				{
					return base.m_grouping.Tree;
				}
				return null;
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

		private bool BuiltinSortOverridden
		{
			get
			{
				if (this.m_detailUserSortTargetInfo != null && this.IsDetailGroup)
				{
					return true;
				}
				if (base.m_odpContext.RuntimeSortFilterInfo != null && this.m_builtinSortOverridden != null)
				{
					for (int i = 0; i < base.m_odpContext.RuntimeSortFilterInfo.Count; i++)
					{
						if (base.m_odpContext.UserSortFilterContext.InProcessUserSortPhase(i) && this.m_builtinSortOverridden[i])
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		internal bool ProcessSecondPassSorting
		{
			get
			{
				if ((SecondPassOperations.Sorting & base.m_odpContext.SecondPassOperation) != 0 && !this.BuiltinSortOverridden && this.HierarchyDef.Sorting != null)
				{
					return this.HierarchyDef.Sorting.ShouldApplySorting;
				}
				return false;
			}
		}

		internal ScalableList<DataFieldRow> DetailDataRows
		{
			get
			{
				return this.m_detailDataRows;
			}
			set
			{
				this.m_detailDataRows = value;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_outerScope) + 4 + ItemSizes.SizeOf(this.m_scopedRunningValues) + ItemSizes.SizeOf(this.m_runningValuesInGroup) + ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_groupCollection) + 4 + 4 + 4 + ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_parentExpression) + ItemSizes.SizeOf(this.m_currentGroupExprValue) + 1 + ItemSizes.SizeOf(this.m_sortFilterExpressionScopeInfoIndices) + ItemSizes.SizeOf(this.m_builtinSortOverridden) + ItemSizes.SizeOf(this.m_detailUserSortTargetInfo) + ItemSizes.SizeOf(this.m_detailDataRows) + 1;
			}
		}

		protected RuntimeGroupRootObj()
		{
		}

		protected RuntimeGroupRootObj(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef, DataActions dataAction, OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(odpContext, objectType, outerScope.Value().Depth + 1)
		{
			base.m_hierarchyRoot = (RuntimeHierarchyObjReference)base.m_selfReference;
			this.m_outerScope = outerScope;
			this.m_hierarchyDef = hierarchyDef;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = hierarchyDef.Grouping;
			Global.Tracer.Assert(null != grouping, "(null != groupDef)");
			this.m_isDetailGroup = grouping.IsDetail;
			if (this.m_isDetailGroup)
			{
				base.m_expression = null;
			}
			else
			{
				base.m_expression = new RuntimeExpressionInfo(grouping.GroupExpressions, grouping.ExprHost, grouping.SortDirections, 0);
			}
			if (base.m_odpContext.RuntimeSortFilterInfo != null)
			{
				int count = base.m_odpContext.RuntimeSortFilterInfo.Count;
				using (outerScope.PinValue())
				{
					IScope scope = outerScope.Value();
					for (int i = 0; i < count; i++)
					{
						IReference<RuntimeSortFilterEventInfo> reference = base.m_odpContext.RuntimeSortFilterInfo[i];
						using (reference.PinValue())
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
							if (runtimeSortFilterEventInfo.EventSource.ContainingScopes == null || runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count == 0 || runtimeSortFilterEventInfo.HasEventSourceScope || (this.m_isDetailGroup && runtimeSortFilterEventInfo.EventSource.IsSubReportTopLevelScope))
							{
								bool flag = false;
								if (this.m_isDetailGroup)
								{
									if (!scope.TargetForNonDetailSort && this.IsTargetForSort(i, true) && runtimeSortFilterEventInfo.EventTarget != base.SelfReference && scope.TargetScopeMatched(i, true))
									{
										flag = true;
										if (this.m_detailUserSortTargetInfo == null)
										{
											this.m_detailUserSortTargetInfo = new RuntimeUserSortTargetInfo((IReference<IHierarchyObj>)base.SelfReference, i, reference);
										}
										else
										{
											this.m_detailUserSortTargetInfo.AddSortInfo((IReference<IHierarchyObj>)base.SelfReference, i, reference);
										}
									}
								}
								else if (grouping.IsSortFilterExpressionScope != null)
								{
									flag = (grouping.IsSortFilterExpressionScope[i] && base.m_odpContext.UserSortFilterContext.InProcessUserSortPhase(i) && this.TargetScopeMatched(i, false));
								}
								if (flag)
								{
									if (this.m_builtinSortOverridden == null)
									{
										this.m_builtinSortOverridden = new bool[count];
									}
									this.m_builtinSortOverridden[i] = true;
								}
							}
						}
					}
				}
			}
			if (this.m_detailUserSortTargetInfo != null)
			{
				this.m_groupingType = RuntimeGroupingObj.GroupingTypes.DetailUserSort;
			}
			else if (grouping.GroupAndSort && !this.BuiltinSortOverridden)
			{
				this.m_groupingType = RuntimeGroupingObj.GroupingTypes.Sort;
			}
			else if (grouping.IsDetail && grouping.Parent == null && !this.BuiltinSortOverridden)
			{
				this.m_groupingType = RuntimeGroupingObj.GroupingTypes.Detail;
			}
			else if (grouping.NaturalGroup)
			{
				this.m_groupingType = RuntimeGroupingObj.GroupingTypes.NaturalGroup;
			}
			else
			{
				this.m_groupingType = RuntimeGroupingObj.GroupingTypes.Hash;
			}
			base.m_grouping = RuntimeGroupingObj.CreateGroupingObj(this.m_groupingType, this, objectType);
			if (grouping.Filters == null)
			{
				this.m_dataAction = dataAction;
				this.m_outerDataAction = dataAction;
			}
			if (grouping.RecursiveAggregates != null)
			{
				this.m_dataAction |= DataActions.RecursiveAggregates;
			}
			if (grouping.PostSortAggregates != null)
			{
				this.m_dataAction |= DataActions.PostSortAggregates;
			}
			if (grouping.Parent != null)
			{
				this.m_parentExpression = new RuntimeExpressionInfo(grouping.Parent, grouping.ParentExprHost, null, 0);
			}
		}

		internal override void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			if (targetScopeObj != null && this == targetScopeObj.Value())
			{
				return;
			}
			if (this.m_isDetailGroup)
			{
				base.DetailGetScopeValues(this.m_outerScope, targetScopeObj, scopeValues, ref index);
			}
			else
			{
				this.m_outerScope.Value().GetScopeValues(targetScopeObj, scopeValues, ref index);
			}
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			if (this.m_isDetailGroup)
			{
				return base.DetailTargetScopeMatched(this.m_hierarchyDef.DataRegionDef, this.m_outerScope, this.m_hierarchyDef.IsColumn, index);
			}
			return this.m_outerScope.Value().TargetScopeMatched(index, detailSort);
		}

		protected abstract void UpdateDataRegionGroupRootInfo();

		internal override void NextRow()
		{
			if (this.m_hierarchyDef.DataScopeInfo != null && this.m_hierarchyDef.DataScopeInfo.NeedsIDC)
			{
				return;
			}
			this.NextRegularRow();
		}

		bool IDataCorrelation.NextCorrelatedRow()
		{
			return this.NextRegularRow();
		}

		private bool NextRegularRow()
		{
			this.UpdateDataRegionGroupRootInfo();
			if (!this.ProcessThisRow())
			{
				return false;
			}
			DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
			DomainScopeContext.DomainScopeInfo domainScopeInfo = null;
			if (domainScopeContext != null)
			{
				domainScopeInfo = domainScopeContext.CurrentDomainScope;
			}
			if (domainScopeInfo != null)
			{
				domainScopeInfo.MoveNext();
				this.m_currentGroupExprValue = domainScopeInfo.CurrentKey;
			}
			else if (base.m_expression != null)
			{
				this.m_currentGroupExprValue = this.EvaluateGroupExpression(base.m_expression, "Group");
			}
			else
			{
				this.m_currentGroupExprValue = base.m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex();
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = this.m_hierarchyDef.Grouping;
			if (this.SaveGroupExprValues)
			{
				grouping.CurrentGroupExpressionValues = new List<object>(1);
				grouping.CurrentGroupExpressionValues.Add(this.m_currentGroupExprValue);
			}
			if (this.m_isDetailGroup)
			{
				if (this.m_detailUserSortTargetInfo != null)
				{
					this.ProcessDetailSort();
				}
				else
				{
					base.m_grouping.NextRow(this.m_currentGroupExprValue, false, null);
				}
			}
			else
			{
				if (base.m_odpContext.RuntimeSortFilterInfo != null)
				{
					int count = base.m_odpContext.RuntimeSortFilterInfo.Count;
					if (grouping.SortFilterScopeMatched == null)
					{
						grouping.SortFilterScopeMatched = new bool[count];
					}
					for (int i = 0; i < count; i++)
					{
						grouping.SortFilterScopeMatched[i] = true;
					}
				}
				base.MatchSortFilterScope(this.m_outerScope, grouping, this.m_currentGroupExprValue, 0);
				object parentKey = null;
				bool flag = null != this.m_parentExpression;
				if (flag)
				{
					parentKey = this.EvaluateGroupExpression(this.m_parentExpression, "Parent");
				}
				base.m_grouping.NextRow(this.m_currentGroupExprValue, flag, parentKey);
			}
			if (domainScopeInfo != null)
			{
				domainScopeInfo.MovePrevious();
			}
			return true;
		}

		protected void ProcessDetailSort()
		{
			if (this.m_detailUserSortTargetInfo != null && !this.m_detailUserSortTargetInfo.TargetForNonDetailSort)
			{
				IReference<RuntimeSortFilterEventInfo> reference = base.m_odpContext.RuntimeSortFilterInfo[this.m_detailUserSortTargetInfo.SortFilterInfoIndices[0]];
				object sortOrder = default(object);
				using (reference.PinValue())
				{
					sortOrder = reference.Value().GetSortOrder(base.m_odpContext.ReportRuntime);
				}
				this.m_detailUserSortTargetInfo.SortTree.NextRow(sortOrder, this);
			}
		}

		internal IHierarchyObj CreateDetailSortHierarchyObj(RuntimeGroupLeafObj rootSortDetailLeafObj)
		{
			Global.Tracer.Assert(null != this.m_detailUserSortTargetInfo, "(null != m_detailUserSortTargetInfo)");
			return new RuntimeSortHierarchyObj(this, base.Depth);
		}

		public override IHierarchyObj CreateHierarchyObjForSortTree()
		{
			if (this.m_detailUserSortTargetInfo != null)
			{
				return new RuntimeSortHierarchyObj(this, base.Depth);
			}
			return base.CreateHierarchyObjForSortTree();
		}

		protected override void ProcessUserSort()
		{
			if (this.m_detailUserSortTargetInfo != null)
			{
				base.m_odpContext.ProcessUserSortForTarget((IReference<IHierarchyObj>)base.SelfReference, ref this.m_detailDataRows, this.m_detailUserSortTargetInfo.TargetForNonDetailSort);
			}
			else
			{
				base.ProcessUserSort();
			}
		}

		protected override void MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			if (this.m_detailUserSortTargetInfo != null)
			{
				this.m_detailUserSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, (IReference<IHierarchyObj>)base.SelfReference);
			}
			else
			{
				base.MarkSortInfoProcessed(runtimeSortFilterInfo);
			}
		}

		protected override void AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			if (this.m_detailUserSortTargetInfo != null)
			{
				this.m_detailUserSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
			else
			{
				base.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
		}

		private object EvaluateGroupExpression(RuntimeExpressionInfo expression, string propertyName)
		{
			Global.Tracer.Assert(null != this.m_hierarchyDef.Grouping, "(null != m_hierarchyDef.Grouping)");
			return base.m_odpContext.ReportRuntime.EvaluateRuntimeExpression(expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, this.m_hierarchyDef.Grouping.Name, propertyName);
		}

		protected bool ProcessThisRow()
		{
			FieldsImpl fieldsImpl = base.m_odpContext.ReportObjectModel.FieldsImpl;
			if (fieldsImpl.IsAggregateRow && 0 > fieldsImpl.AggregationFieldCount)
			{
				return false;
			}
			int[] groupExpressionFieldIndices = this.m_hierarchyDef.Grouping.GetGroupExpressionFieldIndices();
			if (groupExpressionFieldIndices == null)
			{
				fieldsImpl.ValidAggregateRow = false;
			}
			else
			{
				foreach (int num in groupExpressionFieldIndices)
				{
					if (-1 > num || (0 <= num && !fieldsImpl[num].IsAggregationField))
					{
						fieldsImpl.ValidAggregateRow = false;
					}
				}
			}
			if (fieldsImpl.IsAggregateRow && !fieldsImpl.ValidAggregateRow)
			{
				return false;
			}
			return true;
		}

		internal void AddChildWithNoParent(RuntimeGroupLeafObjReference child)
		{
			if (RuntimeGroupingObj.GroupingTypes.Sort == this.m_groupingType)
			{
				using (child.PinValue())
				{
					child.Value().Parent = (RuntimeGroupObjReference)base.m_selfReference;
				}
			}
			else
			{
				base.AddChild(child);
			}
		}

		private bool DetermineTraversalDirection()
		{
			bool result = true;
			if (this.m_detailUserSortTargetInfo != null && this.IsDetailGroup && this.GroupOrDetailSortTree != null)
			{
				result = this.GetDetailSortDirection();
			}
			else if (base.m_expression != null)
			{
				result = base.m_expression.Direction;
			}
			return result;
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				this.CopyDomainScopeGroupInstancesFromTarget();
			}
			RuntimeGroupingObj grouping = base.m_grouping;
			bool ascending = this.DetermineTraversalDirection();
			bool result = true;
			bool processSecondPassSorting = this.ProcessSecondPassSorting;
			bool flag = (SecondPassOperations.FilteringOrAggregatesOrDomainScope & base.m_odpContext.SecondPassOperation) != 0 && (this.m_hierarchyDef.HasFilters || this.m_hierarchyDef.HasInnerFilters);
			if (processSecondPassSorting)
			{
				base.m_expression = new RuntimeExpressionInfo(this.m_hierarchyDef.Sorting.SortExpressions, this.m_hierarchyDef.Sorting.ExprHost, this.m_hierarchyDef.Sorting.SortDirections, 0);
				this.m_groupingType = RuntimeGroupingObj.GroupingTypes.Sort;
				base.m_grouping = new RuntimeGroupingObjTree(this, base.m_objectType);
			}
			else if (flag)
			{
				this.m_groupingType = RuntimeGroupingObj.GroupingTypes.None;
				base.m_grouping = new RuntimeGroupingObjLinkedList(this, base.m_objectType);
			}
			if (flag)
			{
				this.m_groupFilters = new Filters(Filters.FilterTypes.GroupFilter, (IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>)base.SelfReference, this.m_hierarchyDef.Grouping.Filters, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, this.m_hierarchyDef.Grouping.Name, base.m_odpContext, base.Depth + 1);
			}
			this.m_processingStage = ProcessingStages.SortAndFilter;
			base.m_lastChild = null;
			grouping.Traverse(ProcessingStages.SortAndFilter, ascending, aggContext);
			if (flag)
			{
				this.m_groupFilters.FinishReadingGroups(aggContext);
				if (!processSecondPassSorting && (BaseReference)null == (object)base.m_lastChild)
				{
					if ((BaseReference)base.m_firstChild != (object)null)
					{
						base.m_firstChild.Free();
					}
					base.m_firstChild = null;
					result = false;
				}
			}
			if (grouping != base.m_grouping)
			{
				grouping.Cleanup();
			}
			return result;
		}

		private void CopyDomainScopeGroupInstancesFromTarget()
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = this.HierarchyDef.Grouping;
			if (grouping != null && grouping.DomainScope != null)
			{
				IReference<RuntimeGroupRootObj> reference = default(IReference<RuntimeGroupRootObj>);
				bool condition = base.OdpContext.DomainScopeContext.DomainScopes.TryGetValue(grouping.ScopeIDForDomainScope, out reference);
				Global.Tracer.Assert(condition, "DomainScopes should contain the target group root for the specified group");
				using (reference.PinValue())
				{
					RuntimeGroupRootObj runtimeGroupRootObj = reference.Value();
					this.ProcessingStage = ProcessingStages.Grouping;
					runtimeGroupRootObj.m_grouping.CopyDomainScopeGroupInstances(this);
					this.ProcessingStage = ProcessingStages.SortAndFilter;
				}
			}
		}

		public override void UpdateAggregates(AggregateUpdateContext context)
		{
			base.m_grouping.Traverse(ProcessingStages.UpdateAggregates, this.DetermineTraversalDirection(), context);
		}

		void AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner.PostFilterNextRow()
		{
			Global.Tracer.Assert(false);
		}

		internal virtual void AddScopedRunningValue(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj runningValueObj)
		{
			if (this.m_scopedRunningValues == null)
			{
				this.m_scopedRunningValues = new List<string>();
			}
			if (!this.m_scopedRunningValues.Contains(runningValueObj.Name))
			{
				this.m_scopedRunningValues.Add(runningValueObj.Name);
			}
		}

		internal override void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			this.SetupRunningValues(groupCol);
		}

		protected void SetupRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol)
		{
			this.m_groupCollection = groupCol;
			if (this.m_hierarchyDef.Grouping.Name != null)
			{
				groupCol[this.m_hierarchyDef.Grouping.Name] = (RuntimeGroupRootObjReference)base.m_selfReference;
			}
		}

		protected void AddRunningValues(List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues)
		{
			this.AddRunningValues(runningValues, ref this.m_runningValuesInGroup, ref this.m_previousValuesInGroup, this.m_groupCollection, false, false);
		}

		protected void AddRunningValuesOfAggregates()
		{
			if (this.m_hierarchyDef.DataScopeInfo != null)
			{
				List<string> list = null;
				List<string> list2 = null;
				this.AddRunningValues(this.m_hierarchyDef.DataScopeInfo.RunningValuesOfAggregates, ref list, ref list2, this.m_groupCollection, false, false);
			}
		}

		protected bool AddRunningValues(List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues, ref List<string> runningValuesInGroup, ref List<string> previousValuesInGroup, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection, bool cellRunningValues, bool outermostStatics)
		{
			bool result = false;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef;
			bool isColumn;
			List<int> list2;
			List<int> list;
			if (runningValues != null && 0 < runningValues.Count)
			{
				if (runningValuesInGroup == null)
				{
					runningValuesInGroup = new List<string>();
				}
				if (previousValuesInGroup == null)
				{
					previousValuesInGroup = new List<string>();
				}
				if (cellRunningValues)
				{
					list = null;
					list2 = null;
					dataRegionDef = this.m_hierarchyDef.DataRegionDef;
					isColumn = this.m_hierarchyDef.IsColumn;
					RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = this as RuntimeDataTablixGroupRootObj;
					if (outermostStatics)
					{
						if (runtimeDataTablixGroupRootObj != null && runtimeDataTablixGroupRootObj.InnerGroupings != null)
						{
							goto IL_006b;
						}
						if (dataRegionDef.CurrentOuterGroupRoot == null)
						{
							goto IL_006b;
						}
					}
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode hierarchyDef = dataRegionDef.CurrentOuterGroupRoot.Value().HierarchyDef;
					if (isColumn)
					{
						list2 = this.m_hierarchyDef.GetCellIndexes();
						list = ((!outermostStatics) ? hierarchyDef.GetCellIndexes() : dataRegionDef.OutermostStaticRowIndexes);
					}
					else
					{
						list = this.m_hierarchyDef.GetCellIndexes();
						list2 = ((!outermostStatics) ? hierarchyDef.GetCellIndexes() : dataRegionDef.OutermostStaticColumnIndexes);
					}
					goto IL_00f3;
				}
				result = true;
				for (int i = 0; i < runningValues.Count; i++)
				{
					this.AddRunningValue(runningValues[i], runningValuesInGroup, previousValuesInGroup, groupCollection);
				}
				goto IL_01ed;
			}
			return result;
			IL_01ed:
			if (previousValuesInGroup.Count == 0)
			{
				previousValuesInGroup = null;
			}
			if (runningValuesInGroup.Count == 0)
			{
				runningValuesInGroup = null;
			}
			return result;
			IL_006b:
			if (isColumn)
			{
				list2 = this.m_hierarchyDef.GetCellIndexes();
				list = dataRegionDef.OutermostStaticRowIndexes;
			}
			else
			{
				list2 = dataRegionDef.OutermostStaticColumnIndexes;
				list = this.m_hierarchyDef.GetCellIndexes();
			}
			goto IL_00f3;
			IL_00f3:
			if (list != null && list2 != null)
			{
				foreach (int item in list)
				{
					foreach (int item2 in list2)
					{
						Cell cell = dataRegionDef.Rows[item].Cells[item2];
						if (cell.RunningValueIndexes != null)
						{
							result = true;
							for (int j = 0; j < cell.RunningValueIndexes.Count; j++)
							{
								int index = cell.RunningValueIndexes[j];
								this.AddRunningValue(runningValues[index], runningValuesInGroup, previousValuesInGroup, groupCollection);
							}
						}
					}
				}
			}
			goto IL_01ed;
		}

		private void AddRunningValue(AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValue, List<string> runningValuesInGroup, List<string> previousValuesInGroup, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection)
		{
			AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
			bool flag = runningValue.AggregateType == AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous;
			List<string> list = (!flag) ? runningValuesInGroup : previousValuesInGroup;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregatesImpl.GetAggregateObj(runningValue.Name);
			if (dataAggregateObj == null)
			{
				dataAggregateObj = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj(runningValue, base.m_odpContext);
				aggregatesImpl.Add(dataAggregateObj);
			}
			else if (flag && (runningValue.Scope == null || runningValue.IsScopedInEvaluationScope))
			{
				dataAggregateObj.Init();
			}
			if (runningValue.Scope != null)
			{
				IReference<RuntimeGroupRootObj> reference = default(IReference<RuntimeGroupRootObj>);
				if (groupCollection.TryGetValue(runningValue.Scope, out reference))
				{
					using (reference.PinValue())
					{
						reference.Value().AddScopedRunningValue(dataAggregateObj);
					}
				}
				else
				{
					Global.Tracer.Assert(false, "RV with runtime scope escalation");
				}
			}
			if (!list.Contains(dataAggregateObj.Name))
			{
				list.Add(dataAggregateObj.Name);
			}
		}

		internal override void CalculatePreviousAggregates()
		{
			if (this.m_previousValuesInGroup != null)
			{
				AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < this.m_previousValuesInGroup.Count; i++)
				{
					string text = this.m_previousValuesInGroup[i];
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
					Global.Tracer.Assert(aggregateObj != null, "Missing expected previous aggregate: {0}", text);
					aggregateObj.Update();
				}
			}
			if (this.m_outerScope != null && (this.m_outerDataAction & DataActions.PostSortAggregates) != 0)
			{
				using (this.m_outerScope.PinValue())
				{
					this.m_outerScope.Value().CalculatePreviousAggregates();
				}
			}
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (DataActions.PostSortAggregates == dataAction && this.m_runningValuesInGroup != null)
			{
				AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < this.m_runningValuesInGroup.Count; i++)
				{
					string text = this.m_runningValuesInGroup[i];
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
					Global.Tracer.Assert(aggregateObj != null, "Missing expected running value aggregate: {0}", text);
					aggregateObj.Update();
				}
			}
			if (this.m_outerScope != null && (dataAction & this.m_outerDataAction) != 0)
			{
				using (this.m_outerScope.PinValue())
				{
					IScope scope = this.m_outerScope.Value();
					scope.ReadRow(dataAction, context);
				}
			}
		}

		internal void CreateInstances(ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			CreateInstancesTraversalContext traversalContext = new CreateInstancesTraversalContext(parentInstance, innerMembers, innerGroupLeafRef);
			this.m_hierarchyDef.ResetInstancePathCascade();
			this.TraverseGroupOrSortTree(ProcessingStages.CreateGroupTree, traversalContext);
			if (this.m_detailDataRows != null)
			{
				this.m_detailDataRows.Dispose();
			}
			this.m_detailDataRows = null;
			this.m_detailUserSortTargetInfo = null;
		}

		protected void TraverseGroupOrSortTree(ProcessingStages operation, ITraversalContext traversalContext)
		{
			if (this.m_detailUserSortTargetInfo != null && this.m_groupingType != 0)
			{
				this.m_detailUserSortTargetInfo.SortTree.Traverse(operation, this.GetDetailSortDirection(), traversalContext);
			}
			else
			{
				base.m_grouping.Traverse(operation, base.m_expression == null || base.m_expression.Direction, traversalContext);
			}
		}

		internal void TraverseLinkedGroupLeaves(ProcessingStages operation, bool ascending, ITraversalContext traversalContext)
		{
			if ((BaseReference)null != (object)base.m_firstChild)
			{
				using (base.m_firstChild.PinValue())
				{
					base.m_firstChild.Value().TraverseAllLeafNodes(operation, traversalContext);
				}
			}
		}

		private bool GetDetailSortDirection()
		{
			int index = this.m_detailUserSortTargetInfo.SortFilterInfoIndices[0];
			return base.m_odpContext.RuntimeSortFilterInfo[index].Value().SortDirection;
		}

		internal RuntimeGroupLeafObjReference CreateGroupLeaf()
		{
			RuntimeGroupLeafObj runtimeGroupLeafObj = null;
			switch (base.ObjectType)
			{
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix:
				runtimeGroupLeafObj = new RuntimeTablixGroupLeafObj((RuntimeDataTablixGroupRootObjReference)base.SelfReference, base.ObjectType);
				break;
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel:
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart:
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.MapDataRegion:
				runtimeGroupLeafObj = new RuntimeChartCriGroupLeafObj((RuntimeDataTablixGroupRootObjReference)base.SelfReference, base.ObjectType);
				break;
			default:
				Global.Tracer.Assert(false, "Invalid ObjectType");
				break;
			}
			RuntimeGroupLeafObjReference runtimeGroupLeafObjReference = (RuntimeGroupLeafObjReference)runtimeGroupLeafObj.SelfReference;
			runtimeGroupLeafObjReference.UnPinValue();
			return runtimeGroupLeafObjReference;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			writer.RegisterDeclaration(RuntimeGroupRootObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HierarchyDef:
				{
					int value2 = scalabilityCache.StoreStaticReference(this.m_hierarchyDef);
					writer.Write(value2);
					break;
				}
				case MemberName.OuterScope:
					writer.Write(this.m_outerScope);
					break;
				case MemberName.ProcessingStage:
					writer.WriteEnum((int)this.m_processingStage);
					break;
				case MemberName.ScopedRunningValues:
					writer.WriteListOfPrimitives(this.m_scopedRunningValues);
					break;
				case MemberName.RunningValuesInGroup:
					writer.WriteListOfPrimitives(this.m_runningValuesInGroup);
					break;
				case MemberName.PreviousValuesInGroup:
					writer.WriteListOfPrimitives(this.m_previousValuesInGroup);
					break;
				case MemberName.GroupCollection:
					writer.WriteStringRIFObjectDictionary(this.m_groupCollection);
					break;
				case MemberName.DataAction:
					writer.WriteEnum((int)this.m_dataAction);
					break;
				case MemberName.OuterDataAction:
					writer.WriteEnum((int)this.m_outerDataAction);
					break;
				case MemberName.GroupingType:
					writer.WriteEnum((int)this.m_groupingType);
					break;
				case MemberName.Filters:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_groupFilters);
					writer.Write(value);
					break;
				}
				case MemberName.ParentExpression:
					writer.Write(this.m_parentExpression);
					break;
				case MemberName.CurrentGroupExprValue:
					writer.Write(this.m_currentGroupExprValue);
					break;
				case MemberName.SaveGroupExprValues:
					writer.Write(this.m_saveGroupExprValues);
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					writer.Write(this.m_sortFilterExpressionScopeInfoIndices);
					break;
				case MemberName.BuiltinSortOverridden:
					writer.Write(this.m_builtinSortOverridden);
					break;
				case MemberName.IsDetailGroup:
					writer.Write(this.m_isDetailGroup);
					break;
				case MemberName.DetailUserSortTargetInfo:
					writer.Write(this.m_detailUserSortTargetInfo);
					break;
				case MemberName.DetailRows:
					writer.Write(this.m_detailDataRows);
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
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			reader.RegisterDeclaration(RuntimeGroupRootObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HierarchyDef:
				{
					int id2 = reader.ReadInt32();
					this.m_hierarchyDef = (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.OuterScope:
					this.m_outerScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.ProcessingStage:
					this.m_processingStage = (ProcessingStages)reader.ReadEnum();
					break;
				case MemberName.ScopedRunningValues:
					this.m_scopedRunningValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.RunningValuesInGroup:
					this.m_runningValuesInGroup = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.PreviousValuesInGroup:
					this.m_previousValuesInGroup = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.GroupCollection:
					this.m_groupCollection = reader.ReadStringRIFObjectDictionary<IReference<RuntimeGroupRootObj>>();
					break;
				case MemberName.DataAction:
					this.m_dataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.OuterDataAction:
					this.m_outerDataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.GroupingType:
					this.m_groupingType = (RuntimeGroupingObj.GroupingTypes)reader.ReadEnum();
					break;
				case MemberName.Filters:
				{
					int id = reader.ReadInt32();
					this.m_groupFilters = (Filters)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.ParentExpression:
					this.m_parentExpression = (RuntimeExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CurrentGroupExprValue:
					this.m_currentGroupExprValue = reader.ReadVariant();
					break;
				case MemberName.SaveGroupExprValues:
					this.m_saveGroupExprValues = reader.ReadBoolean();
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					this.m_sortFilterExpressionScopeInfoIndices = reader.ReadInt32Array();
					break;
				case MemberName.BuiltinSortOverridden:
					this.m_builtinSortOverridden = reader.ReadBooleanArray();
					break;
				case MemberName.IsDetailGroup:
					this.m_isDetailGroup = reader.ReadBoolean();
					break;
				case MemberName.DetailUserSortTargetInfo:
					this.m_detailUserSortTargetInfo = (RuntimeUserSortTargetInfo)reader.ReadRIFObject();
					break;
				case MemberName.DetailRows:
					this.m_detailDataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeGroupRootObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.HierarchyDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.OuterScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.ProcessingStage, Token.Enum));
				list.Add(new MemberInfo(MemberName.ScopedRunningValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.RunningValuesInGroup, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.PreviousValuesInGroup, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.GroupCollection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference));
				list.Add(new MemberInfo(MemberName.DataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.OuterDataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.GroupingType, Token.Enum));
				list.Add(new MemberInfo(MemberName.Filters, Token.Int32));
				list.Add(new MemberInfo(MemberName.ParentExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.CurrentGroupExprValue, Token.Object));
				list.Add(new MemberInfo(MemberName.SaveGroupExprValues, Token.Boolean));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeInfoIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.BuiltinSortOverridden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Boolean));
				list.Add(new MemberInfo(MemberName.IsDetailGroup, Token.Boolean));
				list.Add(new MemberInfo(MemberName.DetailUserSortTargetInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo));
				list.Add(new MemberInfo(MemberName.DetailRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj, list);
			}
			return RuntimeGroupRootObj.m_declaration;
		}

		public override void SetReference(IReference selfRef)
		{
			base.SetReference(selfRef);
			base.m_hierarchyRoot = (RuntimeGroupRootObjReference)selfRef;
		}
	}
}
