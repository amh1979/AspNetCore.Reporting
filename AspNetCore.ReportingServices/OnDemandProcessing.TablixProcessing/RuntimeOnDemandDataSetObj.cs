using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeOnDemandDataSetObj : AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner, IHierarchyObj, IScope, ISelfReferential, IOnDemandScopeInstance, IStorable, IPersistable
	{
		private readonly OnDemandProcessingContext m_odpContext;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		private readonly DataSetInstance m_dataSetInstance;

		private ScalableList<DataFieldRow> m_dataRows;

		private RuntimeUserSortTargetInfo m_userSortTargetInfo;

		private RuntimeOnDemandDataSetObjReference m_selfReference;

		private int[] m_sortFilterExpressionScopeInfoIndices;

		private RuntimeRICollection m_runtimeDataRegions;

		private bool m_firstNonAggregateRow = true;

		private Filters m_filters;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_nonCustomAggregates;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_customAggregates;

		private RuntimeLookupProcessing m_lookupProcessor;

		public bool IsNoRows
		{
			get
			{
				return false;
			}
		}

		public bool IsMostRecentlyCreatedScopeInstance
		{
			get
			{
				return false;
			}
		}

		public bool HasUnProcessedServerAggregate
		{
			get
			{
				return false;
			}
		}

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot
		{
			get
			{
				return this.m_selfReference;
			}
		}

		OnDemandProcessingContext IHierarchyObj.OdpContext
		{
			get
			{
				return this.m_odpContext;
			}
		}

		BTree IHierarchyObj.SortTree
		{
			get
			{
				if (this.m_userSortTargetInfo != null)
				{
					return this.m_userSortTargetInfo.SortTree;
				}
				return null;
			}
		}

		int IHierarchyObj.ExpressionIndex
		{
			get
			{
				return 0;
			}
		}

		List<int> IHierarchyObj.SortFilterInfoIndices
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

		bool IHierarchyObj.IsDetail
		{
			get
			{
				return false;
			}
		}

		bool IHierarchyObj.InDataRowSortPhase
		{
			get
			{
				return false;
			}
		}

		int IStorable.Size
		{
			get
			{
				return 0;
			}
		}

		public IReference<IHierarchyObj> SelfReference
		{
			get
			{
				return this.m_selfReference;
			}
		}

		public int Depth
		{
			get
			{
				return 0;
			}
		}

		bool IScope.TargetForNonDetailSort
		{
			get
			{
				if (this.m_userSortTargetInfo != null)
				{
					return this.m_userSortTargetInfo.TargetForNonDetailSort;
				}
				return false;
			}
		}

		int[] IScope.SortFilterExpressionScopeInfoIndices
		{
			get
			{
				if (this.m_sortFilterExpressionScopeInfoIndices == null)
				{
					this.m_sortFilterExpressionScopeInfoIndices = new int[this.m_odpContext.RuntimeSortFilterInfo.Count];
					for (int i = 0; i < this.m_odpContext.RuntimeSortFilterInfo.Count; i++)
					{
						this.m_sortFilterExpressionScopeInfoIndices[i] = -1;
					}
				}
				return this.m_sortFilterExpressionScopeInfoIndices;
			}
		}

		IRIFReportScope IScope.RIFReportScope
		{
			get
			{
				return null;
			}
		}

		public RuntimeOnDemandDataSetObj(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance)
		{
			this.m_odpContext = odpContext;
			this.m_dataSet = dataSet;
			this.m_dataSetInstance = dataSetInstance;
			this.m_odpContext.TablixProcessingScalabilityCache.GenerateFixedReference(this);
			UserSortFilterContext userSortFilterContext = odpContext.UserSortFilterContext;
			if (this.m_odpContext.IsSortFilterTarget(dataSet.IsSortFilterTarget, userSortFilterContext.CurrentContainingScope, this.SelfReference, ref this.m_userSortTargetInfo) && this.m_userSortTargetInfo.TargetForNonDetailSort)
			{
				this.m_dataRows = new ScalableList<DataFieldRow>(0, odpContext.TablixProcessingScalabilityCache, 100, 10);
			}
			if (!this.m_odpContext.StreamingMode)
			{
				this.CreateRuntimeStructure();
			}
			this.m_dataSet.SetupRuntimeEnvironment(this.m_odpContext);
			if (this.m_dataSet.Filters != null)
			{
				this.m_filters = new Filters(Filters.FilterTypes.DataSetFilter, (IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>)this.SelfReference, this.m_dataSet.Filters, this.m_dataSet.ObjectType, this.m_dataSet.Name, this.m_odpContext, 0);
			}
			this.RegisterAggregates();
		}

		public void Initialize()
		{
			if (this.m_dataSet.LookupDestinationInfos != null)
			{
				this.m_lookupProcessor = new RuntimeLookupProcessing(this.m_odpContext, this.m_dataSet, this.m_dataSetInstance, this);
			}
		}

		public void NextRow()
		{
			if (this.m_odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
			{
				this.NextAggregateRow();
			}
			else
			{
				bool flag = true;
				if (this.m_filters != null)
				{
					flag = this.m_filters.PassFilters(new DataFieldRow(this.m_odpContext.ReportObjectModel.FieldsImpl, false));
				}
				if (flag)
				{
					this.PostFilterNextRow();
				}
			}
		}

		private void NextAggregateRow()
		{
			if (this.m_odpContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0 && this.m_customAggregates != null)
			{
				for (int i = 0; i < this.m_customAggregates.Count; i++)
				{
					this.m_customAggregates[i].Update();
				}
			}
			if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.SortTree != null)
			{
				if (this.m_userSortTargetInfo.AggregateRows == null)
				{
					this.m_userSortTargetInfo.AggregateRows = new List<AggregateRow>();
				}
				AggregateRow item = new AggregateRow(this.m_odpContext.ReportObjectModel.FieldsImpl, true);
				this.m_userSortTargetInfo.AggregateRows.Add(item);
				if (!this.m_userSortTargetInfo.TargetForNonDetailSort)
				{
					return;
				}
			}
			this.SendToInner();
		}

		public void PostFilterNextRow()
		{
			if (this.m_nonCustomAggregates != null)
			{
				for (int i = 0; i < this.m_nonCustomAggregates.Count; i++)
				{
					this.m_nonCustomAggregates[i].Update();
				}
			}
			if (this.m_dataRows != null)
			{
				RuntimeDataTablixObj.SaveData(this.m_dataRows, this.m_odpContext);
			}
			if (this.m_firstNonAggregateRow)
			{
				this.m_firstNonAggregateRow = false;
				this.m_dataSetInstance.FirstRowOffset = this.m_odpContext.ReportObjectModel.FieldsImpl.StreamOffset;
			}
			if (this.m_lookupProcessor != null)
			{
				this.m_lookupProcessor.NextRow();
			}
			else
			{
				this.PostLookupNextRow();
			}
		}

		internal void PostLookupNextRow()
		{
			this.SendToInner();
		}

		private void SendToInner()
		{
			if (this.m_runtimeDataRegions == null)
			{
				this.CreateRuntimeStructure();
			}
			this.m_runtimeDataRegions.FirstPassNextDataRow(this.m_odpContext);
		}

		internal void CompleteLookupProcessing()
		{
			if (this.m_lookupProcessor != null)
			{
				this.m_lookupProcessor.CompleteLookupProcessing();
				this.m_lookupProcessor = null;
			}
		}

		public void FinishReadingRows()
		{
			if (this.m_filters != null)
			{
				this.m_filters.FinishReadingRows();
			}
			this.m_odpContext.CheckAndThrowIfAborted();
			if (this.m_lookupProcessor != null && this.m_lookupProcessor.MustBufferAllRows)
			{
				this.m_lookupProcessor.FinishReadingRows();
			}
		}

		public void SortAndFilter(AggregateUpdateContext aggContext)
		{
			if (this.m_runtimeDataRegions != null)
			{
				this.m_runtimeDataRegions.SortAndFilter(aggContext);
			}
		}

		public void EnterProcessUserSortPhase()
		{
			if (this.m_userSortTargetInfo != null)
			{
				this.m_userSortTargetInfo.EnterProcessUserSortPhase(this.m_odpContext);
			}
		}

		public void LeaveProcessUserSortPhase()
		{
			if (this.m_userSortTargetInfo != null)
			{
				this.m_userSortTargetInfo.LeaveProcessUserSortPhase(this.m_odpContext);
			}
		}

		public void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection, AggregateUpdateContext aggContext)
		{
			if (this.m_runtimeDataRegions != null)
			{
				this.m_runtimeDataRegions.CalculateRunningValues(groupCollection, null, aggContext);
			}
		}

		public void CreateInstances()
		{
			if (this.m_runtimeDataRegions != null)
			{
				this.m_runtimeDataRegions.CreateAllDataRegionInstances(this.m_odpContext.CurrentReportInstance, this.m_odpContext, this.m_selfReference);
			}
		}

		public void Teardown()
		{
			this.m_selfReference = null;
		}

		public IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			if (this.m_runtimeDataRegions == null)
			{
				return null;
			}
			return this.m_runtimeDataRegions.GetDataRegionObj(rifDataRegion);
		}

		public IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			return null;
		}

		private void CreateRuntimeStructure()
		{
			this.m_runtimeDataRegions = new RuntimeRICollection(this.m_selfReference, this.m_dataSet.DataRegions, this.m_odpContext, this.m_odpContext.ReportDefinition.MergeOnePass);
		}

		private void RegisterAggregates()
		{
			if (this.m_odpContext.InSubreport)
			{
				this.m_odpContext.ReportObjectModel.AggregatesImpl.ClearAll();
			}
			this.CreateAggregates(this.m_dataSet.Aggregates);
			this.CreateAggregates(this.m_dataSet.PostSortAggregates);
		}

		private void CreateAggregates(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> aggDefs)
		{
			if (aggDefs != null && 0 < aggDefs.Count)
			{
				AggregatesImpl aggregatesImpl = this.m_odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < aggDefs.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = aggDefs[i];
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregatesImpl.GetAggregateObj(dataAggregateInfo.Name);
					if (dataAggregateObj == null)
					{
						dataAggregateObj = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj(dataAggregateInfo, this.m_odpContext);
						aggregatesImpl.Add(dataAggregateObj);
					}
					if (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Previous != dataAggregateInfo.AggregateType)
					{
						if (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
						{
							RuntimeDataRegionObj.AddAggregate(ref this.m_customAggregates, dataAggregateObj);
						}
						else
						{
							RuntimeDataRegionObj.AddAggregate(ref this.m_nonCustomAggregates, dataAggregateObj);
						}
					}
				}
			}
		}

		IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
		{
			return new RuntimeSortHierarchyObj(this, 1);
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			return this.m_odpContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
		}

		internal ProcessingMessageList RegisterSpatialElementComparisonError(string type)
		{
			this.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsCannotCompareSpatialType, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, this.m_dataSet.Name, type);
			return this.m_odpContext.ErrorContext.Messages;
		}

		void IHierarchyObj.NextRow(IHierarchyObj owner)
		{
			Global.Tracer.Assert(false);
		}

		void IHierarchyObj.Traverse(ProcessingStages operation, ITraversalContext traversalContext)
		{
			Global.Tracer.Assert(false);
		}

		void IHierarchyObj.ReadRow()
		{
			this.SendToInner();
		}

		void IHierarchyObj.ProcessUserSort()
		{
			Global.Tracer.Assert(null != this.m_userSortTargetInfo, "(null != m_userSortTargetInfo)");
			this.m_odpContext.ProcessUserSortForTarget(this.SelfReference, ref this.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
			if (this.m_userSortTargetInfo.TargetForNonDetailSort)
			{
				this.m_userSortTargetInfo.ResetTargetForNonDetailSort();
				this.m_userSortTargetInfo.EnterProcessUserSortPhase(this.m_odpContext);
				this.CreateRuntimeStructure();
				this.m_userSortTargetInfo.SortTree.Traverse(ProcessingStages.UserSortFilter, true, null);
				this.m_userSortTargetInfo.SortTree.Dispose();
				this.m_userSortTargetInfo.SortTree = null;
				if (this.m_userSortTargetInfo.AggregateRows != null)
				{
					for (int i = 0; i < this.m_userSortTargetInfo.AggregateRows.Count; i++)
					{
						this.m_userSortTargetInfo.AggregateRows[i].SetFields(this.m_odpContext.ReportObjectModel.FieldsImpl);
						this.SendToInner();
					}
					this.m_userSortTargetInfo.AggregateRows = null;
				}
				this.m_userSortTargetInfo.LeaveProcessUserSortPhase(this.m_odpContext);
			}
		}

		void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			if (this.m_userSortTargetInfo != null)
			{
				this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, this.SelfReference);
			}
		}

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			if (this.m_userSortTargetInfo != null)
			{
				this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			Global.Tracer.Assert(false, "RuntimeOnDemandDataSetObj should not be serialized");
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(false, "RuntimeOnDemandDataSetObj should not be de-serialized");
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "RuntimeOnDemandDataSetObj should not need references resolved");
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObj;
		}

		public void SetReference(IReference selfRef)
		{
			this.m_selfReference = (RuntimeOnDemandDataSetObjReference)selfRef;
		}

		bool IScope.IsTargetForSort(int index, bool detailSort)
		{
			if (this.m_userSortTargetInfo != null)
			{
				return this.m_userSortTargetInfo.IsTargetForSort(index, detailSort);
			}
			return false;
		}

		bool IScope.InScope(string scope)
		{
			if (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(this.m_dataSet.Name, scope, false) == 0)
			{
				return true;
			}
			return false;
		}

		void IScope.ReadRow(DataActions dataAction, ITraversalContext context)
		{
			Global.Tracer.Assert(false);
		}

		IReference<IScope> IScope.GetOuterScope(bool includeSubReportContainingScope)
		{
			if (includeSubReportContainingScope)
			{
				return this.m_odpContext.UserSortFilterContext.CurrentContainingScope;
			}
			return null;
		}

		void IScope.CalculatePreviousAggregates()
		{
		}

		string IScope.GetScopeName()
		{
			return this.m_dataSet.Name;
		}

		int IScope.RecursiveLevel(string scope)
		{
			return 0;
		}

		bool IScope.TargetScopeMatched(int index, bool detailSort)
		{
			if (this.m_odpContext.UserSortFilterContext.CurrentContainingScope != null)
			{
				return this.m_odpContext.UserSortFilterContext.CurrentContainingScope.Value().TargetScopeMatched(index, detailSort);
			}
			if (this.m_odpContext.RuntimeSortFilterInfo != null)
			{
				return true;
			}
			return false;
		}

		void IScope.GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			IReference<IScope> currentContainingScope = this.m_odpContext.UserSortFilterContext.CurrentContainingScope;
			if (currentContainingScope != null)
			{
				if (targetScopeObj != null && this == targetScopeObj.Value())
				{
					return;
				}
				currentContainingScope.Value().GetScopeValues((IReference<IHierarchyObj>)null, scopeValues, ref index);
			}
		}

		void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
		}

		public void SetupEnvironment()
		{
		}

		void IScope.UpdateAggregates(AggregateUpdateContext context)
		{
		}
	}
}
