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
	internal abstract class RuntimeDataTablixObj : RuntimeRDLDataRegionObj, IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		protected int[] m_outerGroupingCounters;

		protected IReference<RuntimeMemberObj>[] m_outerGroupings;

		protected IReference<RuntimeMemberObj>[] m_innerGroupings;

		protected List<IReference<RuntimeDataTablixGroupLeafObj>> m_innerGroupsWithCellsForOuterPeerGroupProcessing;

		private long m_scopeInstanceNumber;

		[NonSerialized]
		private static Declaration m_declaration = RuntimeDataTablixObj.GetDeclaration();

		internal int[] OuterGroupingCounters
		{
			get
			{
				return this.m_outerGroupingCounters;
			}
		}

		internal List<IReference<RuntimeDataTablixGroupLeafObj>> InnerGroupsWithCellsForOuterPeerGroupProcessing
		{
			get
			{
				return this.m_innerGroupsWithCellsForOuterPeerGroupProcessing;
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
				return base.m_dataRegionDef.DataScopeInfo.IsLastScopeInstanceNumber(this.m_scopeInstanceNumber);
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
				return base.Size + ItemSizes.SizeOf(this.m_outerGroupingCounters) + ItemSizes.SizeOf(this.m_outerGroupings) + ItemSizes.SizeOf(this.m_innerGroupings) + ItemSizes.SizeOf(this.m_innerGroupsWithCellsForOuterPeerGroupProcessing) + 8;
			}
		}

		internal RuntimeDataTablixObj()
		{
		}

		internal RuntimeDataTablixObj(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataTablixDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType)
			: base(outerScope, dataTablixDef, ref dataAction, odpContext, onePassProcess, dataTablixDef.RunningValues, objectType, outerScope.Value().Depth + 1)
		{
			bool handleMyDataAction = default(bool);
			DataActions innerDataAction = default(DataActions);
			this.ConstructorHelper(ref dataAction, onePassProcess, out handleMyDataAction, out innerDataAction);
			base.m_innerDataAction = innerDataAction;
			DataActions userSortDataAction = base.HandleSortFilterEvent();
			this.ConstructRuntimeStructure(ref innerDataAction, onePassProcess);
			this.HandleDataAction(handleMyDataAction, innerDataAction, userSortDataAction);
			base.m_odpContext.CreatedScopeInstance(base.m_dataRegionDef);
			this.m_scopeInstanceNumber = RuntimeDataRegionObj.AssignScopeInstanceNumber(base.m_dataRegionDef.DataScopeInfo);
		}

		protected void ConstructorHelper(ref DataActions dataAction, bool onePassProcess, out bool handleMyDataAction, out DataActions innerDataAction)
		{
			innerDataAction = base.m_dataAction;
			handleMyDataAction = false;
			if (onePassProcess)
			{
				if (base.m_dataRegionDef.RunningValues != null && 0 < base.m_dataRegionDef.RunningValues.Count)
				{
					RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo>(base.m_odpContext, base.m_dataRegionDef.RunningValues, ref base.m_nonCustomAggregates);
				}
				RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, base.m_dataRegionDef.PostSortAggregates, ref base.m_nonCustomAggregates);
				RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, base.m_dataRegionDef.CellPostSortAggregates, ref base.m_nonCustomAggregates);
			}
			else
			{
				if (base.m_dataRegionDef.RunningValues != null && 0 < base.m_dataRegionDef.RunningValues.Count)
				{
					base.m_dataAction |= DataActions.PostSortAggregates;
				}
				if (base.m_dataRegionDef.PostSortAggregates != null && base.m_dataRegionDef.PostSortAggregates.Count != 0)
				{
					RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, base.m_dataRegionDef.PostSortAggregates, ref base.m_postSortAggregates);
					base.m_dataAction |= DataActions.PostSortAggregates;
					handleMyDataAction = true;
				}
				if (base.m_dataRegionDef.DataScopeInfo != null)
				{
					DataScopeInfo dataScopeInfo = base.m_dataRegionDef.DataScopeInfo;
					if (dataScopeInfo.PostSortAggregatesOfAggregates != null && !dataScopeInfo.PostSortAggregatesOfAggregates.IsEmpty)
					{
						RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, (BucketedAggregatesCollection<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>)dataScopeInfo.PostSortAggregatesOfAggregates, ref base.m_postSortAggregatesOfAggregates);
					}
					if (dataScopeInfo.HasAggregatesToUpdateAtRowScope)
					{
						base.m_dataAction |= DataActions.AggregatesOfAggregates;
						handleMyDataAction = true;
					}
				}
				if (handleMyDataAction)
				{
					innerDataAction = DataActions.None;
				}
				else
				{
					innerDataAction = base.m_dataAction;
				}
			}
			base.m_inDataRowSortPhase = (base.m_dataRegionDef.Sorting != null && base.m_dataRegionDef.Sorting.ShouldApplySorting);
			if (base.m_inDataRowSortPhase)
			{
				base.m_sortedDataRowTree = new BTree(this, base.m_odpContext, base.m_depth);
				base.m_dataRowSortExpression = new RuntimeExpressionInfo(base.m_dataRegionDef.Sorting.SortExpressions, base.m_dataRegionDef.Sorting.ExprHost, base.m_dataRegionDef.Sorting.SortDirections, 0);
				base.m_odpContext.AddSpecialDataRowSort((IReference<IDataRowSortOwner>)base.SelfReference);
			}
			base.m_dataRegionDef.ResetInstanceIndexes();
			this.m_outerGroupingCounters = new int[base.m_dataRegionDef.OuterGroupingDynamicMemberCount];
			for (int i = 0; i < this.m_outerGroupingCounters.Length; i++)
			{
				this.m_outerGroupingCounters[i] = -1;
			}
		}

		protected override void ConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess)
		{
			HierarchyNodeList hierarchyNodeList = default(HierarchyNodeList);
			HierarchyNodeList hierarchyNodeList2 = default(HierarchyNodeList);
			this.CreateRuntimeMemberObjects(base.m_dataRegionDef.OuterMembers, base.m_dataRegionDef.InnerMembers, out hierarchyNodeList, out hierarchyNodeList2, ref innerDataAction);
			if (((hierarchyNodeList != null && hierarchyNodeList.Count != 0) || (hierarchyNodeList2 != null && hierarchyNodeList2.Count != 0)) && base.DataRegionDef.OutermostStaticColumnIndexes == null && base.DataRegionDef.OutermostStaticRowIndexes == null)
			{
				List<int> list = (hierarchyNodeList2 != null) ? hierarchyNodeList2.LeafCellIndexes : null;
				List<int> list2 = (hierarchyNodeList != null) ? hierarchyNodeList.LeafCellIndexes : null;
				if (base.DataRegionDef.ProcessingInnerGrouping == AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column)
				{
					base.DataRegionDef.OutermostStaticColumnIndexes = list;
					base.DataRegionDef.OutermostStaticRowIndexes = list2;
				}
				else
				{
					base.DataRegionDef.OutermostStaticColumnIndexes = list2;
					base.DataRegionDef.OutermostStaticRowIndexes = list;
				}
			}
		}

		private void CreateRuntimeMemberObjects(HierarchyNodeList outerMembers, HierarchyNodeList innerMembers, out HierarchyNodeList outerTopLevelStaticMembers, out HierarchyNodeList innerTopLevelStaticMembers, ref DataActions innerDataAction)
		{
			bool hasOppositeStaticLeafMembers = false;
			HierarchyNodeList topLevelDynamicMembers = null;
			outerTopLevelStaticMembers = null;
			if (outerMembers != null)
			{
				hasOppositeStaticLeafMembers = outerMembers.HasStaticLeafMembers;
				topLevelDynamicMembers = outerMembers.DynamicMembersAtScope;
				outerTopLevelStaticMembers = outerMembers.StaticMembersInSameScope;
			}
			bool hasOppositeStaticLeafMembers2 = false;
			HierarchyNodeList topLevelDynamicMembers2 = null;
			innerTopLevelStaticMembers = null;
			if (innerMembers != null)
			{
				hasOppositeStaticLeafMembers2 = innerMembers.HasStaticLeafMembers;
				topLevelDynamicMembers2 = innerMembers.DynamicMembersAtScope;
				innerTopLevelStaticMembers = innerMembers.StaticMembersInSameScope;
			}
			DataActions dataActions = DataActions.None;
			this.CreateTopLevelRuntimeGroupings(ref dataActions, ref this.m_innerGroupings, innerTopLevelStaticMembers, topLevelDynamicMembers2, (IReference<RuntimeMemberObj>[])null, hasOppositeStaticLeafMembers);
			Global.Tracer.Assert(this.m_innerGroupings != null && this.m_innerGroupings.Length >= 1, "(null != m_innerGroupings && m_innerGroupings.Length >= 1)");
			this.CreateTopLevelRuntimeGroupings(ref innerDataAction, ref this.m_outerGroupings, outerTopLevelStaticMembers, topLevelDynamicMembers, this.m_innerGroupings, hasOppositeStaticLeafMembers2);
		}

		private void CreateTopLevelRuntimeGroupings(ref DataActions groupingDataAction, ref IReference<RuntimeMemberObj>[] groupings, HierarchyNodeList topLevelStaticMembers, HierarchyNodeList topLevelDynamicMembers, IReference<RuntimeMemberObj>[] innerGroupings, bool hasOppositeStaticLeafMembers)
		{
			int num = (topLevelDynamicMembers != null) ? topLevelDynamicMembers.Count : 0;
			groupings = new IReference<RuntimeMemberObj>[Math.Max(1, num)];
			if (num == 0)
			{
				IReference<RuntimeMemberObj> reference = RuntimeDataTablixMemberObj.CreateRuntimeMemberObject((IReference<IScope>)base.m_selfReference, (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)null, ref groupingDataAction, base.m_odpContext, innerGroupings, topLevelStaticMembers, hasOppositeStaticLeafMembers, 0, base.ObjectType);
				groupings[0] = reference;
			}
			else
			{
				for (int i = 0; i < num; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMemberDef = topLevelDynamicMembers[i];
					IReference<RuntimeMemberObj> reference2 = RuntimeDataTablixMemberObj.CreateRuntimeMemberObject((IReference<IScope>)base.m_selfReference, dynamicMemberDef, ref groupingDataAction, base.m_odpContext, innerGroupings, (i == 0) ? topLevelStaticMembers : null, hasOppositeStaticLeafMembers, 0, base.ObjectType);
					groupings[i] = reference2;
				}
			}
		}

		protected void HandleDataAction(bool handleMyDataAction, DataActions innerDataAction, DataActions userSortDataAction)
		{
			if (!handleMyDataAction)
			{
				base.m_dataAction = innerDataAction;
			}
			base.m_dataAction |= userSortDataAction;
			if (base.m_dataAction != 0)
			{
				base.m_dataRows = new ScalableList<DataFieldRow>(base.m_depth + 1, base.m_odpContext.TablixProcessingScalabilityCache, 30);
			}
		}

		protected override void SendToInner()
		{
			bool peerOuterGroupProcessing = base.m_odpContext.PeerOuterGroupProcessing;
			base.m_dataRegionDef.RuntimeDataRegionObj = base.m_selfReference;
			int num = (this.m_outerGroupings != null) ? this.m_outerGroupings.Length : 0;
			AggregateRowInfo aggregateRowInfo = AggregateRowInfo.CreateAndSaveAggregateInfo(base.m_odpContext);
			if (base.m_dataRegionDef.IsMatrixIDC)
			{
				if (this.m_innerGroupings != null)
				{
					this.ProcessInnerHierarchy(aggregateRowInfo);
				}
				for (int i = 0; i < num; i++)
				{
					this.ProcessOuterHierarchy(aggregateRowInfo, i);
				}
			}
			else
			{
				if (num == 0)
				{
					if (this.m_innerGroupings != null)
					{
						this.ProcessInnerHierarchy(aggregateRowInfo);
					}
				}
				else
				{
					if (this.m_innerGroupsWithCellsForOuterPeerGroupProcessing == null || !peerOuterGroupProcessing)
					{
						this.m_innerGroupsWithCellsForOuterPeerGroupProcessing = new List<IReference<RuntimeDataTablixGroupLeafObj>>();
					}
					for (int j = 0; j < num; j++)
					{
						this.ProcessOuterHierarchy(aggregateRowInfo, j);
						if (this.m_innerGroupings != null)
						{
							if (j == 0)
							{
								this.ProcessInnerHierarchy(aggregateRowInfo);
							}
							else
							{
								foreach (IReference<RuntimeDataTablixGroupLeafObj> item in this.m_innerGroupsWithCellsForOuterPeerGroupProcessing)
								{
									using (item.PinValue())
									{
										item.Value().PeerOuterGroupProcessCells();
									}
									aggregateRowInfo.RestoreAggregateInfo(base.m_odpContext);
								}
							}
						}
					}
				}
				base.m_odpContext.PeerOuterGroupProcessing = peerOuterGroupProcessing;
			}
		}

		private void ProcessInnerHierarchy(AggregateRowInfo aggregateRowInfo)
		{
			for (int i = 0; i < this.m_innerGroupings.Length; i++)
			{
				IReference<RuntimeMemberObj> reference = this.m_innerGroupings[i];
				using (reference.PinValue())
				{
					reference.Value().NextRow(false, base.m_odpContext);
				}
				aggregateRowInfo.RestoreAggregateInfo(base.m_odpContext);
			}
		}

		private void ProcessOuterHierarchy(AggregateRowInfo aggregateRowInfo, int outerGroupingIndex)
		{
			base.m_odpContext.PeerOuterGroupProcessing = (outerGroupingIndex != 0);
			base.m_dataRegionDef.ResetOuterGroupingIndexesForOuterPeerGroup(0);
			base.m_dataRegionDef.ResetOuterGroupingAggregateRowInfo();
			base.m_dataRegionDef.SetDataTablixAggregateRowInfo(aggregateRowInfo);
			IReference<RuntimeMemberObj> reference = this.m_outerGroupings[outerGroupingIndex];
			using (reference.PinValue())
			{
				reference.Value().NextRow(true, base.m_odpContext);
			}
			aggregateRowInfo.RestoreAggregateInfo(base.m_odpContext);
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			this.SetupEnvironment();
			if (base.m_userSortTargetInfo != null)
			{
				base.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_odpContext);
			}
			bool flag = base.DataRegionDef.ProcessingInnerGrouping == AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column;
			IReference<RuntimeMemberObj>[] array = flag ? this.m_outerGroupings : this.m_innerGroupings;
			IReference<RuntimeMemberObj>[] array2 = flag ? this.m_innerGroupings : this.m_outerGroupings;
			int rowDomainScopeCount = base.DataRegionDef.RowDomainScopeCount;
			int columnDomainScopeCount = base.DataRegionDef.ColumnDomainScopeCount;
			DomainScopeContext domainScopeContext = base.OdpContext.DomainScopeContext;
			AggregateUpdateQueue workQueue = null;
			if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, base.m_dataRegionDef.DataScopeInfo, base.m_aggregatesOfAggregates, AggregateUpdateFlags.Both, false);
				if (rowDomainScopeCount > 0)
				{
					domainScopeContext.AddDomainScopes(array, array.Length - rowDomainScopeCount);
				}
				if (columnDomainScopeCount > 0)
				{
					domainScopeContext.AddDomainScopes(array2, array2.Length - columnDomainScopeCount);
				}
			}
			this.Traverse(ProcessingStages.SortAndFilter, aggContext);
			base.SortAndFilter(aggContext);
			if (base.m_odpContext.HasSecondPassOperation(SecondPassOperations.FilteringOrAggregatesOrDomainScope))
			{
				RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, base.m_dataRegionDef.DataScopeInfo, base.m_aggregatesOfAggregates, true);
				if (rowDomainScopeCount > 0)
				{
					domainScopeContext.RemoveDomainScopes(array, array.Length - rowDomainScopeCount);
				}
				if (columnDomainScopeCount > 0)
				{
					domainScopeContext.RemoveDomainScopes(array2, array2.Length - columnDomainScopeCount);
				}
			}
			if (base.m_userSortTargetInfo != null)
			{
				base.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_odpContext);
			}
			return true;
		}

		public override void UpdateAggregates(AggregateUpdateContext context)
		{
			this.SetupEnvironment();
			if (RuntimeDataRegionObj.UpdateAggregatesAtScope(context, this, base.m_dataRegionDef.DataScopeInfo, AggregateUpdateFlags.Both, false))
			{
				this.Traverse(ProcessingStages.UpdateAggregates, context);
			}
		}

		protected virtual void Traverse(ProcessingStages operation, ITraversalContext context)
		{
			bool flag = base.DataRegionDef.ProcessingInnerGrouping == AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column;
			IReference<RuntimeMemberObj>[] array = flag ? this.m_outerGroupings : this.m_innerGroupings;
			IReference<RuntimeMemberObj>[] array2 = flag ? this.m_innerGroupings : this.m_outerGroupings;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					this.TraverseMember(array[i], operation, context);
				}
			}
			if (array2 != null)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					this.TraverseMember(array2[j], operation, context);
				}
			}
		}

		private void TraverseMember(IReference<RuntimeMemberObj> memberRef, ProcessingStages operation, ITraversalContext context)
		{
			using (memberRef.PinValue())
			{
				switch (operation)
				{
				case ProcessingStages.SortAndFilter:
					memberRef.Value().SortAndFilter((AggregateUpdateContext)context);
					break;
				case ProcessingStages.UpdateAggregates:
					memberRef.Value().UpdateAggregates((AggregateUpdateContext)context);
					break;
				default:
					Global.Tracer.Assert(false, "Unknown ProcessingStage in TraverseMember");
					break;
				}
			}
		}

		internal override void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (base.m_dataRegionDef.RunningValues != null && base.m_runningValues == null && base.m_previousValues == null)
			{
				RuntimeDataTablixObj.AddRunningValues(base.m_odpContext, base.m_dataRegionDef.RunningValues, ref base.m_runningValues, ref base.m_previousValues, groupCol, lastGroup);
			}
			if (base.m_dataRegionDef.DataScopeInfo != null)
			{
				List<string> list = null;
				List<string> list2 = null;
				RuntimeDataTablixObj.AddRunningValues(base.m_odpContext, base.m_dataRegionDef.DataScopeInfo.RunningValuesOfAggregates, ref list, ref list2, groupCol, lastGroup);
			}
			bool flag = base.m_dataRows != null && FlagUtils.HasFlag(base.m_dataAction, DataActions.PostSortAggregates);
			AggregateUpdateQueue workQueue = RuntimeDataRegionObj.AggregateOfAggregatesStart(aggContext, this, base.m_dataRegionDef.DataScopeInfo, base.m_postSortAggregatesOfAggregates, (AggregateUpdateFlags)(flag ? 1 : 3), true);
			if (flag)
			{
				DataActions dataActions = DataActions.PostSortAggregates;
				if (aggContext.LastScopeNeedsRowAggregateProcessing())
				{
					dataActions |= DataActions.PostSortAggregatesOfAggregates;
				}
				base.ReadRows(dataActions, aggContext);
				base.m_dataRows = null;
			}
			int num = (this.m_outerGroupings != null) ? this.m_outerGroupings.Length : 0;
			if (num == 0)
			{
				if (this.m_innerGroupings != null)
				{
					for (int i = 0; i < this.m_innerGroupings.Length; i++)
					{
						IReference<RuntimeMemberObj> reference = this.m_innerGroupings[i];
						using (reference.PinValue())
						{
							reference.Value().CalculateRunningValues(groupCol, lastGroup, aggContext);
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					IReference<RuntimeMemberObj> reference2 = this.m_outerGroupings[j];
					bool flag2 = default(bool);
					using (reference2.PinValue())
					{
						RuntimeMemberObj runtimeMemberObj = reference2.Value();
						runtimeMemberObj.CalculateRunningValues(groupCol, lastGroup, aggContext);
						flag2 = ((BaseReference)runtimeMemberObj.GroupRoot == (object)null);
					}
					if (flag2 && this.m_innerGroupings != null)
					{
						for (int k = 0; k < this.m_innerGroupings.Length; k++)
						{
							IReference<RuntimeMemberObj> reference3 = this.m_innerGroupings[k];
							using (reference3.PinValue())
							{
								RuntimeMemberObj runtimeMemberObj2 = reference3.Value();
								runtimeMemberObj2.PrepareCalculateRunningValues();
								runtimeMemberObj2.CalculateRunningValues(groupCol, lastGroup, aggContext);
							}
						}
					}
				}
			}
			this.CalculateRunningValuesForTopLevelStaticContents(groupCol, lastGroup, aggContext);
			RuntimeDataRegionObj.AggregatesOfAggregatesEnd(this, aggContext, workQueue, base.m_dataRegionDef.DataScopeInfo, base.m_postSortAggregatesOfAggregates, true);
			this.CalculateDRPreviousAggregates();
			RuntimeRICollection.StoreRunningValues(base.m_odpContext.ReportObjectModel.AggregatesImpl, base.m_dataRegionDef.RunningValues, ref base.m_runningValueValues);
			if (base.m_dataRegionDef.DataScopeInfo != null)
			{
				RuntimeRICollection.StoreRunningValues(base.m_odpContext.ReportObjectModel.AggregatesImpl, base.m_dataRegionDef.DataScopeInfo.RunningValuesOfAggregates, ref base.m_runningValueOfAggregateValues);
			}
		}

		protected virtual void CalculateRunningValuesForTopLevelStaticContents(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
		}

		internal static void AddRunningValues(OnDemandProcessingContext odpContext, List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues, ref List<string> runningValuesInGroup, ref List<string> previousValuesInGroup, Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection, IReference<RuntimeGroupRootObj> lastGroup)
		{
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
				AggregatesImpl aggregatesImpl = odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < runningValues.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = runningValues[i];
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregatesImpl.GetAggregateObj(runningValueInfo.Name);
					if (dataAggregateObj == null)
					{
						dataAggregateObj = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj(runningValueInfo, odpContext);
						aggregatesImpl.Add(dataAggregateObj);
					}
					if (runningValueInfo.Scope != null)
					{
						IReference<RuntimeGroupRootObj> reference = default(IReference<RuntimeGroupRootObj>);
						if (groupCollection.TryGetValue(runningValueInfo.Scope, out reference))
						{
							using (reference.PinValue())
							{
								reference.Value().AddScopedRunningValue(dataAggregateObj);
							}
						}
						else
						{
							Global.Tracer.Assert(false);
						}
					}
					if (runningValueInfo.AggregateType == AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous)
					{
						previousValuesInGroup.Add(dataAggregateObj.Name);
					}
					else
					{
						runningValuesInGroup.Add(dataAggregateObj.Name);
					}
				}
			}
		}

		internal static void UpdateRunningValues(OnDemandProcessingContext odpContext, List<string> runningValueNames)
		{
			AggregatesImpl aggregatesImpl = odpContext.ReportObjectModel.AggregatesImpl;
			for (int i = 0; i < runningValueNames.Count; i++)
			{
				string name = runningValueNames[i];
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(name);
				aggregateObj.Update();
			}
		}

		internal static void SaveData(ScalableList<DataFieldRow> dataRows, OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(null != dataRows, "(null != dataRows)");
			dataRows.Add(RuntimeDataTablixObj.SaveData(odpContext));
		}

		internal static DataFieldRow SaveData(OnDemandProcessingContext odpContext)
		{
			return new DataFieldRow(odpContext.ReportObjectModel.FieldsImpl, true);
		}

		internal override void CalculatePreviousAggregates()
		{
			if (base.m_outerScope != null && (DataActions.PostSortAggregates & base.m_outerDataAction) != 0)
			{
				using (base.m_outerScope.PinValue())
				{
					base.m_outerScope.Value().CalculatePreviousAggregates();
				}
			}
		}

		private void CalculateDRPreviousAggregates()
		{
			this.SetupEnvironment();
			if (base.m_previousValues != null)
			{
				AggregatesImpl aggregatesImpl = base.m_odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < base.m_previousValues.Count; i++)
				{
					string text = base.m_previousValues[i];
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = aggregatesImpl.GetAggregateObj(text);
					Global.Tracer.Assert(aggregateObj != null, "Missing expected previous aggregate: {0}", text);
					aggregateObj.Update();
				}
			}
		}

		public override void ReadRow(DataActions dataAction, ITraversalContext context)
		{
			if (DataActions.UserSort == dataAction)
			{
				RuntimeDataRegionObj.CommonFirstRow(base.m_odpContext, ref base.m_firstRowIsAggregate, ref base.m_firstRow);
				base.CommonNextRow(base.m_dataRows);
			}
			else if (DataActions.AggregatesOfAggregates == dataAction)
			{
				AggregateUpdateContext aggregateUpdateContext = (AggregateUpdateContext)context;
				aggregateUpdateContext.UpdateAggregatesForRow();
			}
			else
			{
				if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregatesOfAggregates))
				{
					AggregateUpdateContext aggregateUpdateContext2 = (AggregateUpdateContext)context;
					aggregateUpdateContext2.UpdateAggregatesForRow();
				}
				if (!base.m_dataRegionDef.ProcessCellRunningValues)
				{
					if (FlagUtils.HasFlag(dataAction, DataActions.PostSortAggregates))
					{
						if (base.m_postSortAggregates != null)
						{
							RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, base.m_postSortAggregates, false);
						}
						if (base.m_runningValues != null)
						{
							RuntimeDataTablixObj.UpdateRunningValues(base.m_odpContext, base.m_runningValues);
						}
					}
					if (base.m_outerScope != null && (dataAction & base.m_outerDataAction) != 0)
					{
						using (base.m_outerScope.PinValue())
						{
							IScope scope = base.m_outerScope.Value();
							scope.ReadRow(dataAction, context);
						}
					}
				}
			}
		}

		public override void SetupEnvironment()
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = base.m_dataRegionDef.GetDataSet(base.m_odpContext.ReportDefinition);
			base.SetupNewDataSet(dataSet);
			base.m_odpContext.ReportRuntime.CurrentScope = this;
			base.SetupEnvironment(base.m_dataRegionDef.RunningValues);
		}

		internal void CreateOutermostStaticCells(DataRegionInstance dataRegionInstance, bool outerGroupings, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			if (innerMembers != null)
			{
				this.SetupEnvironment();
				foreach (IReference<RuntimeMemberObj> reference in innerMembers)
				{
					using (reference.PinValue())
					{
						reference.Value().CreateInstances(base.SelfReference, base.m_odpContext, dataRegionInstance, !outerGroupings, null, dataRegionInstance, null, innerGroupLeafRef);
					}
				}
			}
			else if (base.DataRegionDef.OutermostStaticRowIndexes != null && base.DataRegionDef.OutermostStaticColumnIndexes != null)
			{
				base.m_dataRegionDef.AddCell();
			}
		}

		private bool OutermostSTCellTargetScopeMatched(int index, IReference<RuntimeSortFilterEventInfo> sortFilterInfo)
		{
			return true;
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			if (base.m_dataRegionDef.InOutermostStaticCells && !this.OutermostSTCellTargetScopeMatched(index, base.m_odpContext.RuntimeSortFilterInfo[index]))
			{
				return false;
			}
			return base.TargetScopeMatched(index, detailSort);
		}

		internal void CreateInstances(DataRegionInstance dataRegionInstance)
		{
			base.m_dataRegionDef.ResetInstanceIndexes();
			this.m_innerGroupsWithCellsForOuterPeerGroupProcessing = null;
			base.m_dataRegionDef.CurrentDataRegionInstance = dataRegionInstance;
			dataRegionInstance.StoreAggregates(base.m_odpContext, base.m_dataRegionDef.Aggregates);
			dataRegionInstance.StoreAggregates(base.m_odpContext, base.m_dataRegionDef.PostSortAggregates);
			dataRegionInstance.StoreAggregates(base.m_odpContext, base.m_dataRegionDef.RunningValues);
			if (base.m_dataRegionDef.DataScopeInfo != null)
			{
				DataScopeInfo dataScopeInfo = base.m_dataRegionDef.DataScopeInfo;
				dataRegionInstance.StoreAggregates(base.m_odpContext, dataScopeInfo.AggregatesOfAggregates);
				dataRegionInstance.StoreAggregates(base.m_odpContext, dataScopeInfo.PostSortAggregatesOfAggregates);
				dataRegionInstance.StoreAggregates(base.m_odpContext, dataScopeInfo.RunningValuesOfAggregates);
			}
			if (base.m_firstRow != null)
			{
				dataRegionInstance.FirstRowOffset = base.m_firstRow.StreamOffset;
			}
			base.m_dataRegionDef.ResetInstancePathCascade();
			if (base.m_dataRegionDef.InScopeEventSources != null)
			{
				UserSortFilterContext.ProcessEventSources(base.m_odpContext, this, base.m_dataRegionDef.InScopeEventSources);
			}
			this.CreateDataRegionScopedInstance(dataRegionInstance);
			IReference<RuntimeMemberObj>[] array;
			IReference<RuntimeMemberObj>[] innerMembers;
			bool outerGroupings;
			if (base.DataRegionDef.ProcessingInnerGrouping == AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column)
			{
				array = this.m_outerGroupings;
				innerMembers = this.m_innerGroupings;
				outerGroupings = true;
			}
			else
			{
				array = this.m_innerGroupings;
				innerMembers = this.m_outerGroupings;
				outerGroupings = false;
			}
			IReference<RuntimeMemberObj>[] array2 = array;
			foreach (IReference<RuntimeMemberObj> reference in array2)
			{
				using (reference.PinValue())
				{
					reference.Value().CreateInstances(base.SelfReference, base.m_odpContext, dataRegionInstance, outerGroupings, null, dataRegionInstance, innerMembers, null);
				}
			}
			base.m_dataRegionDef.ResetInstancePathCascade();
			base.m_dataRegionDef.ResetInstanceIndexes();
		}

		protected virtual void CreateDataRegionScopedInstance(DataRegionInstance dataRegionInstance)
		{
		}

		public IOnDemandMemberInstanceReference GetFirstMemberInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember)
		{
			IReference<RuntimeMemberObj>[] memberCollection = this.GetMemberCollection(rifMember);
			return RuntimeDataRegionObj.GetFirstMemberInstance(rifMember, memberCollection);
		}

		private IReference<RuntimeMemberObj>[] GetMemberCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember)
		{
			if (base.m_dataRegionDef.ProcessingInnerGrouping == AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion.ProcessingInnerGroupings.Column)
			{
				if (rifMember.IsColumn)
				{
					return this.m_innerGroupings;
				}
				return this.m_outerGroupings;
			}
			if (rifMember.IsColumn)
			{
				return this.m_outerGroupings;
			}
			return this.m_innerGroupings;
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
				IReference<RuntimeMemberObj>[] memberCollection = this.GetMemberCollection(rifMember);
				return RuntimeDataRegionObj.GetGroupRoot(rifMember, memberCollection);
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion = scope as AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion;
			return this.GetNestedDataRegion(rifDataRegion);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeDataTablixObj.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.OuterGroupingCounters:
					writer.Write(this.m_outerGroupingCounters);
					break;
				case MemberName.OuterGroupings:
					writer.Write(this.m_outerGroupings);
					break;
				case MemberName.InnerGroupings:
					writer.Write(this.m_innerGroupings);
					break;
				case MemberName.InnerGroupsWithCellsForOuterPeerGroupProcessing:
					writer.Write(this.m_innerGroupsWithCellsForOuterPeerGroupProcessing);
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
			reader.RegisterDeclaration(RuntimeDataTablixObj.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.OuterGroupingCounters:
					this.m_outerGroupingCounters = reader.ReadInt32Array();
					break;
				case MemberName.OuterGroupings:
					this.m_outerGroupings = reader.ReadArrayOfRIFObjects<IReference<RuntimeMemberObj>>();
					break;
				case MemberName.InnerGroupings:
					this.m_innerGroupings = reader.ReadArrayOfRIFObjects<IReference<RuntimeMemberObj>>();
					break;
				case MemberName.InnerGroupsWithCellsForOuterPeerGroupProcessing:
					this.m_innerGroupsWithCellsForOuterPeerGroupProcessing = reader.ReadListOfRIFObjects<List<IReference<RuntimeDataTablixGroupLeafObj>>>();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeDataTablixObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.OuterGroupingCounters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.OuterGroupings, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference));
				list.Add(new MemberInfo(MemberName.InnerGroupings, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference));
				list.Add(new MemberInfo(MemberName.InnerGroupsWithCellsForOuterPeerGroupProcessing, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference));
				list.Add(new MemberInfo(MemberName.ScopeInstanceNumber, Token.Int64));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj, list);
			}
			return RuntimeDataTablixObj.m_declaration;
		}
	}
}
