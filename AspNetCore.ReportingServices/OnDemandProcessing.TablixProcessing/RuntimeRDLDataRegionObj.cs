using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeRDLDataRegionObj : RuntimeDataRegionObj, IHierarchyObj, IStorable, IPersistable, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner, IDataRowSortOwner, IDataRowHolder, IDataCorrelation
	{
		[StaticReference]
		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion m_dataRegionDef;

		protected IReference<IScope> m_outerScope;

		protected DataFieldRow m_firstRow;

		protected bool m_firstRowIsAggregate;

		[StaticReference]
		protected Filters m_filters;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_nonCustomAggregates;

		protected BucketedDataAggregateObjs m_aggregatesOfAggregates;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_customAggregates;

		protected DataActions m_dataAction;

		protected DataActions m_outerDataAction;

		protected List<string> m_runningValues;

		protected List<string> m_previousValues;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_runningValueValues;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] m_runningValueOfAggregateValues;

		protected List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> m_postSortAggregates;

		protected BucketedDataAggregateObjs m_postSortAggregatesOfAggregates;

		protected ScalableList<DataFieldRow> m_dataRows;

		protected DataActions m_innerDataAction;

		protected RuntimeUserSortTargetInfo m_userSortTargetInfo;

		protected int[] m_sortFilterExpressionScopeInfoIndices;

		protected bool m_inDataRowSortPhase;

		protected BTree m_sortedDataRowTree;

		protected RuntimeExpressionInfo m_dataRowSortExpression;

		protected bool m_hasProcessedAggregateRow;

		private static Declaration m_declaration = RuntimeRDLDataRegionObj.GetDeclaration();

		protected override IReference<IScope> OuterScope
		{
			get
			{
				return this.m_outerScope;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion DataRegionDef
		{
			get
			{
				return this.m_dataRegionDef;
			}
		}

		protected override string ScopeName
		{
			get
			{
				return this.m_dataRegionDef.Name;
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
				return this.m_outerScope.Value().TargetForNonDetailSort;
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
				return this.m_dataRegionDef;
			}
		}

		IReference<IHierarchyObj> IHierarchyObj.HierarchyRoot
		{
			get
			{
				return (IReference<IHierarchyObj>)base.m_selfReference;
			}
		}

		OnDemandProcessingContext IHierarchyObj.OdpContext
		{
			get
			{
				return base.m_odpContext;
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
				return this.m_inDataRowSortPhase;
			}
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Sorting IDataRowSortOwner.SortingDef
		{
			get
			{
				return this.m_dataRegionDef.Sorting;
			}
		}

		OnDemandProcessingContext IDataRowSortOwner.OdpContext
		{
			get
			{
				return base.m_odpContext;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + ItemSizes.SizeOf(this.m_outerScope) + ItemSizes.SizeOf(this.m_firstRow) + 1 + ItemSizes.ReferenceSize + ItemSizes.SizeOf(this.m_nonCustomAggregates) + ItemSizes.SizeOf(this.m_customAggregates) + 4 + 4 + ItemSizes.SizeOf(this.m_runningValues) + ItemSizes.SizeOf(this.m_runningValueValues) + ItemSizes.SizeOf(this.m_runningValueOfAggregateValues) + ItemSizes.SizeOf(this.m_postSortAggregates) + ItemSizes.SizeOf(this.m_dataRows) + 4 + ItemSizes.SizeOf(this.m_userSortTargetInfo) + ItemSizes.SizeOf(this.m_sortFilterExpressionScopeInfoIndices) + 1 + ItemSizes.SizeOf(this.m_sortedDataRowTree) + ItemSizes.SizeOf(this.m_dataRowSortExpression) + ItemSizes.SizeOf(this.m_aggregatesOfAggregates) + ItemSizes.SizeOf(this.m_postSortAggregatesOfAggregates) + 1;
			}
		}

		internal RuntimeRDLDataRegionObj()
		{
		}

		internal RuntimeRDLDataRegionObj(IReference<IScope> outerScope, AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, ref DataActions dataAction, OnDemandProcessingContext odpContext, bool onePassProcess, List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, int level)
			: base(odpContext, objectType, level)
		{
			this.m_dataRegionDef = dataRegionDef;
			this.m_outerScope = outerScope;
			RuntimeDataRegionObj.CreateAggregates(base.m_odpContext, dataRegionDef.Aggregates, ref this.m_nonCustomAggregates, ref this.m_customAggregates);
			if (dataRegionDef.DataScopeInfo != null)
			{
				RuntimeDataRegionObj.CreateAggregates<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>(base.m_odpContext, (BucketedAggregatesCollection<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>)dataRegionDef.DataScopeInfo.AggregatesOfAggregates, ref this.m_aggregatesOfAggregates);
			}
			if (dataRegionDef.Filters != null)
			{
				this.m_filters = new Filters(Filters.FilterTypes.DataRegionFilter, (IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>)base.SelfReference, dataRegionDef.Filters, dataRegionDef.ObjectType, dataRegionDef.Name, base.m_odpContext, level + 1);
			}
			else
			{
				this.m_outerDataAction = dataAction;
				this.m_dataAction = dataAction;
				dataAction = DataActions.None;
			}
		}

		internal override bool IsTargetForSort(int index, bool detailSort)
		{
			if (this.m_userSortTargetInfo != null && this.m_userSortTargetInfo.IsTargetForSort(index, detailSort))
			{
				return true;
			}
			return this.m_outerScope.Value().IsTargetForSort(index, detailSort);
		}

		IHierarchyObj IHierarchyObj.CreateHierarchyObjForSortTree()
		{
			if (this.m_inDataRowSortPhase)
			{
				return new RuntimeDataRowSortHierarchyObj(this, base.Depth + 1);
			}
			return new RuntimeSortHierarchyObj(this, base.m_depth + 1);
		}

		ProcessingMessageList IHierarchyObj.RegisterComparisonError(string propertyName)
		{
			if (this.m_inDataRowSortPhase)
			{
				base.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, this.m_dataRegionDef.ObjectType, this.m_dataRegionDef.Name, propertyName);
				return base.m_odpContext.ErrorContext.Messages;
			}
			return base.m_odpContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
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
			this.ReadRow(DataActions.UserSort, null);
		}

		void IHierarchyObj.ProcessUserSort()
		{
			base.m_odpContext.ProcessUserSortForTarget((IReference<IHierarchyObj>)(RuntimeRDLDataRegionObjReference)base.SelfReference, ref this.m_dataRows, this.m_userSortTargetInfo.TargetForNonDetailSort);
			this.m_dataAction &= ~DataActions.UserSort;
			if (this.m_userSortTargetInfo.TargetForNonDetailSort)
			{
				this.m_userSortTargetInfo.ResetTargetForNonDetailSort();
				this.m_userSortTargetInfo.EnterProcessUserSortPhase(base.m_odpContext);
				DataActions innerDataAction = this.m_innerDataAction;
				this.ConstructRuntimeStructure(ref innerDataAction, base.m_odpContext.ReportDefinition.MergeOnePass);
				if (this.m_dataAction != 0)
				{
					this.m_dataRows = new ScalableList<DataFieldRow>(base.m_depth, base.m_odpContext.TablixProcessingScalabilityCache);
				}
				base.ScopeFinishSorting(ref this.m_firstRow, this.m_userSortTargetInfo);
				this.m_userSortTargetInfo.LeaveProcessUserSortPhase(base.m_odpContext);
			}
		}

		void IHierarchyObj.MarkSortInfoProcessed(List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo)
		{
			if (this.m_userSortTargetInfo != null)
			{
				this.m_userSortTargetInfo.MarkSortInfoProcessed(runtimeSortFilterInfo, (IReference<IHierarchyObj>)base.SelfReference);
			}
		}

		void IHierarchyObj.AddSortInfoIndex(int sortInfoIndex, IReference<RuntimeSortFilterEventInfo> sortInfo)
		{
			if (this.m_userSortTargetInfo != null)
			{
				this.m_userSortTargetInfo.AddSortInfoIndex(sortInfoIndex, sortInfo);
			}
		}

		protected abstract void ConstructRuntimeStructure(ref DataActions innerDataAction, bool onePassProcess);

		protected DataActions HandleSortFilterEvent()
		{
			DataActions result = DataActions.None;
			if (base.m_odpContext.IsSortFilterTarget(this.DataRegionDef.IsSortFilterTarget, this.m_outerScope, (IReference<IHierarchyObj>)(RuntimeRDLDataRegionObjReference)base.SelfReference, ref this.m_userSortTargetInfo) && this.m_userSortTargetInfo.TargetForNonDetailSort)
			{
				result = DataActions.UserSort;
			}
			base.m_odpContext.RegisterSortFilterExpressionScope(this.m_outerScope, base.SelfReference, this.DataRegionDef.IsSortFilterExpressionScope);
			return result;
		}

		internal override bool TargetScopeMatched(int index, bool detailSort)
		{
			return this.m_outerScope.Value().TargetScopeMatched(index, detailSort);
		}

		internal override void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			if (targetScopeObj != null && this == targetScopeObj.Value())
			{
				return;
			}
			this.m_outerScope.Value().GetScopeValues(targetScopeObj, scopeValues, ref index);
		}

		internal override void NextRow()
		{
			if (this.m_dataRegionDef.DataScopeInfo != null && this.m_dataRegionDef.DataScopeInfo.NeedsIDC)
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
			if (base.m_odpContext.ReportObjectModel.FieldsImpl.AggregationFieldCount == 0)
			{
				this.m_hasProcessedAggregateRow = true;
				RuntimeDataRegionObj.UpdateAggregates(base.m_odpContext, this.m_customAggregates, false);
			}
			if (base.m_odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow)
			{
				base.ScopeNextAggregateRow(this.m_userSortTargetInfo);
				return false;
			}
			this.NextNonAggregateRow();
			return true;
		}

		private void NextNonAggregateRow()
		{
			bool flag = true;
			if (this.m_filters != null)
			{
				flag = this.m_filters.PassFilters(new DataFieldRow(base.m_odpContext.ReportObjectModel.FieldsImpl, false));
			}
			if (flag)
			{
				((AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)this).PostFilterNextRow();
			}
		}

		void AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner.PostFilterNextRow()
		{
			if (this.m_inDataRowSortPhase)
			{
				object keyValue = this.EvaluateDataRowSortExpression(this.m_dataRowSortExpression);
				this.m_sortedDataRowTree.NextRow(keyValue, this);
			}
			else
			{
				((IDataRowSortOwner)this).PostDataRowSortNextRow();
			}
		}

		public object EvaluateDataRowSortExpression(RuntimeExpressionInfo sortExpression)
		{
			return base.m_odpContext.ReportRuntime.EvaluateRuntimeExpression(sortExpression, base.ObjectType, this.m_dataRegionDef.Name, "Sort");
		}

		void IDataRowSortOwner.PostDataRowSortNextRow()
		{
			RuntimeDataRegionObj.CommonFirstRow(base.m_odpContext, ref this.m_firstRowIsAggregate, ref this.m_firstRow);
			base.ScopeNextNonAggregateRow(this.m_nonCustomAggregates, this.m_dataRows);
		}

		void IDataRowSortOwner.DataRowSortTraverse()
		{
			try
			{
				ITraversalContext traversalContext = new DataRowSortOwnerTraversalContext(this);
				this.m_sortedDataRowTree.Traverse(ProcessingStages.Grouping, this.m_dataRowSortExpression.Direction, traversalContext);
			}
			finally
			{
				this.m_inDataRowSortPhase = false;
				this.m_sortedDataRowTree.Dispose();
				this.m_sortedDataRowTree = null;
				this.m_dataRowSortExpression = null;
			}
		}

		internal override bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			if ((SecondPassOperations.FilteringOrAggregatesOrDomainScope & base.m_odpContext.SecondPassOperation) != 0 && this.m_dataRows != null && (this.m_outerDataAction & DataActions.RecursiveAggregates) != 0)
			{
				this.ReadRows(DataActions.RecursiveAggregates, null);
				base.ReleaseDataRows(DataActions.RecursiveAggregates, ref this.m_dataAction, ref this.m_dataRows);
			}
			return true;
		}

		public void ReadRows(DataActions action, ITraversalContext context)
		{
			for (int i = 0; i < this.m_dataRows.Count; i++)
			{
				DataFieldRow dataFieldRow = this.m_dataRows[i];
				dataFieldRow.SetFields(base.m_odpContext.ReportObjectModel.FieldsImpl);
				this.ReadRow(action, context);
			}
		}

		protected void SetupEnvironment(List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues)
		{
			if (this.m_dataRegionDef.DataScopeInfo != null && this.m_dataRegionDef.DataScopeInfo.DataSet != null && this.m_dataRegionDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext != null)
			{
				base.m_odpContext.ReportObjectModel.RestoreFields(this.m_dataRegionDef.DataScopeInfo.DataSet.DataSetCore.FieldsContext);
			}
			base.SetupEnvironment(this.m_nonCustomAggregates, this.m_customAggregates, this.m_firstRow);
			base.SetupAggregates(this.m_postSortAggregates);
			base.SetupRunningValues(runningValues, this.m_runningValueValues);
			base.SetupAggregates(this.m_aggregatesOfAggregates);
			base.SetupAggregates(this.m_postSortAggregatesOfAggregates);
			if (this.m_dataRegionDef.DataScopeInfo != null)
			{
				base.SetupRunningValues(this.m_dataRegionDef.DataScopeInfo.RunningValuesOfAggregates, this.m_runningValueOfAggregateValues);
			}
		}

		internal override bool InScope(string scope)
		{
			return base.DataRegionInScope(this.DataRegionDef, scope);
		}

		protected override int GetRecursiveLevel(string scope)
		{
			return base.DataRegionRecursiveLevel(this.DataRegionDef, scope);
		}

		protected override void GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
			base.DataRegionGetGroupNameValuePairs(this.DataRegionDef, pairs);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(RuntimeRDLDataRegionObj.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataRegionDef:
				{
					int value2 = scalabilityCache.StoreStaticReference(this.m_dataRegionDef);
					writer.Write(value2);
					break;
				}
				case MemberName.OuterScope:
					writer.Write(this.m_outerScope);
					break;
				case MemberName.FirstRow:
					writer.Write(this.m_firstRow);
					break;
				case MemberName.FirstRowIsAggregate:
					writer.Write(this.m_firstRowIsAggregate);
					break;
				case MemberName.Filters:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_filters);
					writer.Write(value);
					break;
				}
				case MemberName.NonCustomAggregates:
					writer.Write(this.m_nonCustomAggregates);
					break;
				case MemberName.CustomAggregates:
					writer.Write(this.m_customAggregates);
					break;
				case MemberName.DataAction:
					writer.WriteEnum((int)this.m_dataAction);
					break;
				case MemberName.OuterDataAction:
					writer.WriteEnum((int)this.m_outerDataAction);
					break;
				case MemberName.RunningValues:
					writer.WriteListOfPrimitives(this.m_runningValues);
					break;
				case MemberName.PreviousValues:
					writer.WriteListOfPrimitives(this.m_previousValues);
					break;
				case MemberName.RunningValueValues:
					writer.Write(this.m_runningValueValues);
					break;
				case MemberName.RunningValueOfAggregateValues:
					writer.Write(this.m_runningValueOfAggregateValues);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(this.m_postSortAggregates);
					break;
				case MemberName.DataRows:
					writer.Write(this.m_dataRows);
					break;
				case MemberName.InnerDataAction:
					writer.WriteEnum((int)this.m_innerDataAction);
					break;
				case MemberName.UserSortTargetInfo:
					writer.Write(this.m_userSortTargetInfo);
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					writer.Write(this.m_sortFilterExpressionScopeInfoIndices);
					break;
				case MemberName.InDataRowSortPhase:
					writer.Write(this.m_inDataRowSortPhase);
					break;
				case MemberName.SortedDataRowTree:
					writer.Write(this.m_sortedDataRowTree);
					break;
				case MemberName.DataRowSortExpression:
					writer.Write(this.m_dataRowSortExpression);
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
			reader.RegisterDeclaration(RuntimeRDLDataRegionObj.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataRegionDef:
				{
					int id2 = reader.ReadInt32();
					this.m_dataRegionDef = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.OuterScope:
					this.m_outerScope = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.FirstRow:
					this.m_firstRow = (DataFieldRow)reader.ReadRIFObject();
					break;
				case MemberName.FirstRowIsAggregate:
					this.m_firstRowIsAggregate = reader.ReadBoolean();
					break;
				case MemberName.Filters:
				{
					int id = reader.ReadInt32();
					this.m_filters = (Filters)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.NonCustomAggregates:
					this.m_nonCustomAggregates = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.CustomAggregates:
					this.m_customAggregates = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.DataAction:
					this.m_dataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.OuterDataAction:
					this.m_outerDataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.RunningValues:
					this.m_runningValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.PreviousValues:
					this.m_previousValues = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.RunningValueValues:
					this.m_runningValueValues = reader.ReadArrayOfRIFObjects<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.RunningValueOfAggregateValues:
					this.m_runningValueOfAggregateValues = reader.ReadArrayOfRIFObjects<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult>();
					break;
				case MemberName.PostSortAggregates:
					this.m_postSortAggregates = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>>();
					break;
				case MemberName.DataRows:
					this.m_dataRows = reader.ReadRIFObject<ScalableList<DataFieldRow>>();
					break;
				case MemberName.InnerDataAction:
					this.m_innerDataAction = (DataActions)reader.ReadEnum();
					break;
				case MemberName.UserSortTargetInfo:
					this.m_userSortTargetInfo = (RuntimeUserSortTargetInfo)reader.ReadRIFObject();
					break;
				case MemberName.SortFilterExpressionScopeInfoIndices:
					this.m_sortFilterExpressionScopeInfoIndices = reader.ReadInt32Array();
					break;
				case MemberName.InDataRowSortPhase:
					this.m_inDataRowSortPhase = reader.ReadBoolean();
					break;
				case MemberName.SortedDataRowTree:
					this.m_sortedDataRowTree = (BTree)reader.ReadRIFObject();
					break;
				case MemberName.DataRowSortExpression:
					this.m_dataRowSortExpression = (RuntimeExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (RuntimeRDLDataRegionObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DataRegionDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.OuterScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.FirstRow, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow));
				list.Add(new MemberInfo(MemberName.FirstRowIsAggregate, Token.Boolean));
				list.Add(new MemberInfo(MemberName.Filters, Token.Int32));
				list.Add(new MemberInfo(MemberName.NonCustomAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.CustomAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.DataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.OuterDataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.RunningValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.PreviousValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.RunningValueValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.PostSortAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj));
				list.Add(new MemberInfo(MemberName.DataRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableList));
				list.Add(new MemberInfo(MemberName.InnerDataAction, Token.Enum));
				list.Add(new MemberInfo(MemberName.UserSortTargetInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo));
				list.Add(new MemberInfo(MemberName.SortFilterExpressionScopeInfoIndices, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Int32));
				list.Add(new MemberInfo(MemberName.InDataRowSortPhase, Token.Boolean));
				list.Add(new MemberInfo(MemberName.SortedDataRowTree, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BTree));
				list.Add(new MemberInfo(MemberName.DataRowSortExpression, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeExpressionInfo));
				list.Add(new MemberInfo(MemberName.AggregatesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.PostSortAggregatesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateObjs));
				list.Add(new MemberInfo(MemberName.RunningValueOfAggregateValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjResult));
				list.Add(new MemberInfo(MemberName.HasProcessedAggregateRow, Token.Boolean));
				RuntimeRDLDataRegionObj.m_declaration = new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj, list);
			}
			return RuntimeRDLDataRegionObj.m_declaration;
		}
	}
}
