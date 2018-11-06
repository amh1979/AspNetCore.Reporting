using AspNetCore.ReportingServices.Common;
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
	internal abstract class RuntimeDataRegionObj : IScope, IStorable, IPersistable, ISelfReferential
	{
		[StaticReference]
		protected OnDemandProcessingContext m_odpContext;

		protected AspNetCore.ReportingServices.ReportProcessing.ObjectType m_objectType;

		[NonSerialized]
		protected RuntimeDataRegionObjReference m_selfReference;

		protected int m_depth;

		private static readonly Declaration m_declaration = RuntimeDataRegionObj.GetDeclaration();

		public RuntimeDataRegionObjReference SelfReference
		{
			get
			{
				return this.m_selfReference;
			}
		}

		internal OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_odpContext;
			}
		}

		protected abstract IReference<IScope> OuterScope
		{
			get;
		}

		protected virtual string ScopeName
		{
			get
			{
				return null;
			}
		}

		internal virtual bool TargetForNonDetailSort
		{
			get
			{
				if (this.OuterScope != null)
				{
					return this.OuterScope.Value().TargetForNonDetailSort;
				}
				return false;
			}
		}

		protected virtual int[] SortFilterExpressionScopeInfoIndices
		{
			get
			{
				Global.Tracer.Assert(false);
				return null;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return this.m_objectType;
			}
		}

		public int Depth
		{
			get
			{
				return this.m_depth;
			}
		}

		internal virtual IRIFReportScope RIFReportScope
		{
			get
			{
				return null;
			}
		}

		bool IScope.TargetForNonDetailSort
		{
			get
			{
				return this.TargetForNonDetailSort;
			}
		}

		int[] IScope.SortFilterExpressionScopeInfoIndices
		{
			get
			{
				return this.SortFilterExpressionScopeInfoIndices;
			}
		}

		IRIFReportScope IScope.RIFReportScope
		{
			get
			{
				return this.RIFReportScope;
			}
		}

		public virtual int Size
		{
			get
			{
				return 8 + ItemSizes.SizeOf(this.m_selfReference);
			}
		}

		protected RuntimeDataRegionObj()
		{
		}

		protected RuntimeDataRegionObj(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, int depth)
		{
			this.m_odpContext = odpContext;
			this.m_objectType = objectType;
			this.m_depth = depth;
			this.m_odpContext.TablixProcessingScalabilityCache.AllocateAndPin(this, this.m_depth);
		}

		internal virtual bool IsTargetForSort(int index, bool detailSort)
		{
			if (this.OuterScope != null)
			{
				return this.OuterScope.Value().IsTargetForSort(index, detailSort);
			}
			return false;
		}

		internal abstract void NextRow();

		internal abstract bool SortAndFilter(AggregateUpdateContext aggContext);

		internal abstract void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext);

		public abstract void SetupEnvironment();

		internal abstract void CalculatePreviousAggregates();

		public abstract void UpdateAggregates(AggregateUpdateContext context);

		bool IScope.IsTargetForSort(int index, bool detailSort)
		{
			return this.IsTargetForSort(index, detailSort);
		}

		void IScope.CalculatePreviousAggregates()
		{
			this.CalculatePreviousAggregates();
		}

		bool IScope.InScope(string scope)
		{
			return this.InScope(scope);
		}

		IReference<IScope> IScope.GetOuterScope(bool includeSubReportContainingScope)
		{
			return this.OuterScope;
		}

		string IScope.GetScopeName()
		{
			return this.ScopeName;
		}

		int IScope.RecursiveLevel(string scope)
		{
			return this.GetRecursiveLevel(scope);
		}

		bool IScope.TargetScopeMatched(int index, bool detailSort)
		{
			return this.TargetScopeMatched(index, detailSort);
		}

		void IScope.GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			this.GetScopeValues(targetScopeObj, scopeValues, ref index);
		}

		void IScope.GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
			this.GetGroupNameValuePairs(pairs);
		}

		internal static bool UpdateAggregatesAtScope(AggregateUpdateContext aggContext, IDataRowHolder scope, DataScopeInfo scopeInfo, AggregateUpdateFlags updateFlags, bool needsSetupEnvironment)
		{
			return aggContext.UpdateAggregates(scopeInfo, scope, updateFlags, needsSetupEnvironment);
		}

		internal static void AggregatesOfAggregatesEnd(IScope scopeObj, AggregateUpdateContext aggContext, AggregateUpdateQueue workQueue, DataScopeInfo dataScopeInfo, BucketedDataAggregateObjs aggregatesOfAggregates, bool updateAggsIfNeeded)
		{
			if (dataScopeInfo != null)
			{
				if (updateAggsIfNeeded)
				{
					while (aggContext.AdvanceQueue(workQueue))
					{
						scopeObj.UpdateAggregates(aggContext);
					}
				}
				aggContext.RestoreOriginalState(workQueue);
				if (aggContext.Mode == AggregateMode.Aggregates && dataScopeInfo.NeedsSeparateAofAPass && updateAggsIfNeeded)
				{
					scopeObj.UpdateAggregates(aggContext);
				}
			}
		}

		internal static AggregateUpdateQueue AggregateOfAggregatesStart(AggregateUpdateContext aggContext, IDataRowHolder scope, DataScopeInfo dataScopeInfo, BucketedDataAggregateObjs aggregatesOfAggregates, AggregateUpdateFlags updateFlags, bool needsSetupEnvironment)
		{
			if (dataScopeInfo == null)
			{
				return null;
			}
			AggregateUpdateQueue result = null;
			if (aggContext.Mode == AggregateMode.Aggregates)
			{
				if (dataScopeInfo.NeedsSeparateAofAPass)
				{
					result = aggContext.ReplaceAggregatesToUpdate(aggregatesOfAggregates);
				}
				else
				{
					result = aggContext.RegisterAggregatesToUpdate(aggregatesOfAggregates);
					if (updateFlags != 0)
					{
						RuntimeDataRegionObj.UpdateAggregatesAtScope(aggContext, scope, dataScopeInfo, updateFlags, needsSetupEnvironment);
					}
				}
			}
			else if (aggContext.Mode == AggregateMode.PostSortAggregates)
			{
				result = aggContext.RegisterAggregatesToUpdate(aggregatesOfAggregates);
				result = aggContext.RegisterRunningValuesToUpdate(result, dataScopeInfo.RunningValuesOfAggregates);
				if (updateFlags != 0)
				{
					RuntimeDataRegionObj.UpdateAggregatesAtScope(aggContext, scope, dataScopeInfo, updateFlags, needsSetupEnvironment);
				}
			}
			else
			{
				Global.Tracer.Assert(false, "Unknown AggregateMode for AggregateOfAggregatesStart");
			}
			return result;
		}

		internal static void AddAggregate(ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate)
		{
			if (aggregates == null)
			{
				aggregates = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>();
			}
			aggregates.Add(aggregate);
		}

		internal static void CreateAggregates(OnDemandProcessingContext odpContext, List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> aggDefs, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> nonCustomAggregates, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> customAggregates)
		{
			if (aggDefs != null && 0 < aggDefs.Count)
			{
				for (int i = 0; i < aggDefs.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj(aggDefs[i], odpContext);
					if (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == aggDefs[i].AggregateType)
					{
						RuntimeDataRegionObj.AddAggregate(ref customAggregates, aggregate);
					}
					else
					{
						RuntimeDataRegionObj.AddAggregate(ref nonCustomAggregates, aggregate);
					}
				}
			}
		}

		internal static void CreateAggregates(OnDemandProcessingContext odpContext, List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> aggDefs, List<int> aggregateIndexes, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> nonCustomAggregates, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> customAggregates)
		{
			if (aggregateIndexes != null && 0 < aggregateIndexes.Count)
			{
				for (int i = 0; i < aggregateIndexes.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = aggDefs[aggregateIndexes[i]];
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj(dataAggregateInfo, odpContext);
					if (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
					{
						RuntimeDataRegionObj.AddAggregate(ref customAggregates, aggregate);
					}
					else
					{
						RuntimeDataRegionObj.AddAggregate(ref nonCustomAggregates, aggregate);
					}
				}
			}
		}

		internal static void CreateAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggDefs, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates) where AggregateType : AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggDefs != null && 0 < aggDefs.Count)
			{
				for (int i = 0; i < aggDefs.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj((AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo)(object)aggDefs[i], odpContext);
					RuntimeDataRegionObj.AddAggregate(ref aggregates, aggregate);
				}
			}
		}

		internal static void CreateAggregates<AggregateType>(OnDemandProcessingContext odpContext, BucketedAggregatesCollection<AggregateType> aggDefs, ref BucketedDataAggregateObjs aggregates) where AggregateType : AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggDefs != null && !aggDefs.IsEmpty)
			{
				if (aggregates == null)
				{
					aggregates = new BucketedDataAggregateObjs();
				}
				foreach (AggregateBucket<AggregateType> bucket in aggDefs.Buckets)
				{
					foreach (AggregateType aggregate in bucket.Aggregates)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj item = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj((AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo)(object)aggregate, odpContext);
						((BucketedAggregatesCollection<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj>)aggregates).GetOrCreateBucket(bucket.Level).Aggregates.Add(item);
					}
				}
			}
		}

		internal static void CreateAggregates<AggregateType>(OnDemandProcessingContext odpContext, List<AggregateType> aggDefs, List<int> aggregateIndexes, ref List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates) where AggregateType : AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggregateIndexes != null && 0 < aggregateIndexes.Count)
			{
				for (int i = 0; i < aggregateIndexes.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj((AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo)(object)aggDefs[aggregateIndexes[i]], odpContext);
					RuntimeDataRegionObj.AddAggregate(ref aggregates, aggregate);
				}
			}
		}

		internal static void UpdateAggregates(OnDemandProcessingContext odpContext, List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates, bool updateAndSetup)
		{
			if (aggregates != null)
			{
				for (int i = 0; i < aggregates.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregates[i];
					dataAggregateObj.Update();
					if (updateAndSetup)
					{
						odpContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, dataAggregateObj.AggregateResult());
					}
				}
			}
		}

		protected void SetupAggregates(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates)
		{
			if (aggregates != null)
			{
				for (int i = 0; i < aggregates.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = aggregates[i];
					this.m_odpContext.ReportObjectModel.AggregatesImpl.Set(dataAggregateObj.Name, dataAggregateObj.AggregateDef, dataAggregateObj.DuplicateNames, dataAggregateObj.AggregateResult());
				}
			}
		}

		protected void SetupAggregates(BucketedDataAggregateObjs aggregates)
		{
			RuntimeDataRegionObj.SetupAggregates(this.m_odpContext, aggregates);
		}

		internal static void SetupAggregates(OnDemandProcessingContext odpContext, BucketedDataAggregateObjs aggregates)
		{
			if (aggregates != null)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregate in aggregates)
				{
					odpContext.ReportObjectModel.AggregatesImpl.Set(aggregate.Name, aggregate.AggregateDef, aggregate.DuplicateNames, aggregate.AggregateResult());
				}
			}
		}

		protected void SetupNewDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			this.m_odpContext.EnsureRuntimeEnvironmentForDataSet(dataSet, false);
		}

		protected void SetupEnvironment(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> nonCustomAggregates, List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> customAggregates, DataFieldRow dataRow)
		{
			this.SetupAggregates(nonCustomAggregates);
			this.SetupAggregates(customAggregates);
			this.SetupFields(dataRow);
			this.m_odpContext.ReportRuntime.CurrentScope = this;
		}

		protected void SetupFields(DataFieldRow dataRow)
		{
			if (dataRow == null)
			{
				this.m_odpContext.ReportObjectModel.CreateNoRows();
			}
			else
			{
				dataRow.SetFields(this.m_odpContext.ReportObjectModel.FieldsImpl);
			}
		}

		protected void SetupRunningValues(List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> rvDefs, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] rvValues)
		{
			int num = 0;
			RuntimeDataRegionObj.SetupRunningValues(this.m_odpContext, ref num, rvDefs, rvValues);
		}

		private static void SetupRunningValues(OnDemandProcessingContext odpContext, ref int startIndex, List<AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo> rvDefs, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] rvValues)
		{
			if (rvDefs != null && rvValues != null)
			{
				AggregatesImpl aggregatesImpl = odpContext.ReportObjectModel.AggregatesImpl;
				for (int i = 0; i < rvDefs.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = rvDefs[i];
					aggregatesImpl.Set(runningValueInfo.Name, runningValueInfo, runningValueInfo.DuplicateNames, rvValues[startIndex + i]);
				}
				startIndex += rvDefs.Count;
			}
		}

		protected static IOnDemandMemberInstanceReference GetFirstMemberInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember, IReference<RuntimeMemberObj>[] memberCol)
		{
			IOnDemandMemberInstanceReference result = null;
			RuntimeDataTablixGroupRootObjReference groupRoot = RuntimeDataRegionObj.GetGroupRoot(rifMember, memberCol);
			using (groupRoot.PinValue())
			{
				RuntimeDataTablixGroupRootObj runtimeDataTablixGroupRootObj = groupRoot.Value();
				RuntimeGroupLeafObjReference firstChild = runtimeDataTablixGroupRootObj.FirstChild;
				if ((BaseReference)firstChild != (object)null)
				{
					return (IOnDemandMemberInstanceReference)firstChild;
				}
				return result;
			}
		}

		protected static RuntimeDataTablixGroupRootObjReference GetGroupRoot(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember, IReference<RuntimeMemberObj>[] memberCol)
		{
			Global.Tracer.Assert(!rifMember.IsStatic, "Cannot GetGroupRoot of a static member");
			IReference<RuntimeMemberObj> reference = memberCol[rifMember.IndexInCollection];
			RuntimeMemberObj runtimeMemberObj = reference.Value();
			return runtimeMemberObj.GroupRoot;
		}

		internal static long AssignScopeInstanceNumber(DataScopeInfo dataScopeInfo)
		{
			if (dataScopeInfo == null)
			{
				return 0L;
			}
			return dataScopeInfo.AssignScopeInstanceNumber();
		}

		public abstract void ReadRow(DataActions dataAction, ITraversalContext context);

		internal abstract bool InScope(string scope);

		protected Hashtable GetScopeNames(RuntimeDataRegionObjReference currentScope, string targetScope, out bool inScope)
		{
			inScope = false;
			Hashtable hashtable = new Hashtable();
			IScope scope = null;
			for (IReference<IScope> reference = currentScope; reference != null; reference = scope.GetOuterScope(false))
			{
				scope = reference.Value();
				string scopeName = scope.GetScopeName();
				if (scopeName != null)
				{
					if (!inScope && scopeName.Equals(targetScope))
					{
						inScope = true;
					}
					AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping value = null;
					if (scope is RuntimeGroupLeafObj)
					{
						value = ((RuntimeGroupLeafObj)scope).GroupingDef;
					}
					hashtable.Add(scopeName, value);
				}
				else if (scope is RuntimeTablixCell && !inScope)
				{
					inScope = scope.InScope(targetScope);
				}
			}
			return hashtable;
		}

		protected Hashtable GetScopeNames(RuntimeDataRegionObjReference currentScope, string targetScope, out int level)
		{
			level = -1;
			Hashtable hashtable = new Hashtable();
			IScope scope = null;
			for (IReference<IScope> reference = currentScope; reference != null; reference = scope.GetOuterScope(false))
			{
				scope = reference.Value();
				string scopeName = scope.GetScopeName();
				if (scopeName != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = null;
					if (scope is RuntimeGroupLeafObj)
					{
						grouping = ((RuntimeGroupLeafObj)scope).GroupingDef;
						if (-1 == level && scopeName.Equals(targetScope))
						{
							level = grouping.RecursiveLevel;
						}
					}
					hashtable.Add(scopeName, grouping);
				}
				else if (scope is RuntimeTablixCell && -1 == level)
				{
					level = scope.RecursiveLevel(targetScope);
				}
			}
			return hashtable;
		}

		protected Hashtable GetScopeNames(RuntimeDataRegionObjReference currentScope, Dictionary<string, object> nameValuePairs)
		{
			Hashtable hashtable = new Hashtable();
			IScope scope = null;
			for (IReference<IScope> reference = currentScope; reference != null; reference = scope.GetOuterScope(false))
			{
				scope = reference.Value();
				string scopeName = scope.GetScopeName();
				if (scopeName != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = null;
					if (scope is RuntimeGroupLeafObj)
					{
						grouping = ((RuntimeGroupLeafObj)scope).GroupingDef;
						RuntimeDataRegionObj.AddGroupNameValuePair(this.m_odpContext, grouping, nameValuePairs);
					}
					hashtable.Add(scopeName, grouping);
				}
				else if (scope is RuntimeTablixCell)
				{
					scope.GetGroupNameValuePairs(nameValuePairs);
				}
			}
			return hashtable;
		}

		internal static void AddGroupNameValuePair(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping, Dictionary<string, object> nameValuePairs)
		{
			if (grouping != null)
			{
				Global.Tracer.Assert(grouping.GroupExpressions != null && 0 < grouping.GroupExpressions.Count);
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = grouping.GroupExpressions[0];
				if (expressionInfo.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field)
				{
					try
					{
						FieldImpl fieldImpl = odpContext.ReportObjectModel.FieldsImpl[expressionInfo.IntValue];
						if (fieldImpl.FieldDef != null)
						{
							object value = fieldImpl.Value;
							if (!nameValuePairs.ContainsKey(fieldImpl.FieldDef.DataField))
							{
								nameValuePairs.Add(fieldImpl.FieldDef.DataField, (value is DBNull) ? null : value);
							}
						}
					}
					catch (Exception e)
					{
						if (!AsynchronousExceptionDetection.IsStoppingException(e))
						{
							goto end_IL_0097;
						}
						throw;
						end_IL_0097:;
					}
				}
			}
		}

		protected bool DataRegionInScope(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, string scope)
		{
			if (dataRegionDef.ScopeNames == null)
			{
				bool result = default(bool);
				dataRegionDef.ScopeNames = this.GetScopeNames(this.SelfReference, scope, out result);
				return result;
			}
			return dataRegionDef.ScopeNames.Contains(scope);
		}

		protected virtual int GetRecursiveLevel(string scope)
		{
			return -1;
		}

		protected int DataRegionRecursiveLevel(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, string scope)
		{
			if (scope == null)
			{
				return -1;
			}
			if (dataRegionDef.ScopeNames == null)
			{
				int result = default(int);
				dataRegionDef.ScopeNames = this.GetScopeNames(this.SelfReference, scope, out result);
				return result;
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = dataRegionDef.ScopeNames[scope] as AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping;
			if (grouping != null)
			{
				return grouping.RecursiveLevel;
			}
			return -1;
		}

		protected void DataRegionGetGroupNameValuePairs(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, Dictionary<string, object> nameValuePairs)
		{
			if (dataRegionDef.ScopeNames == null)
			{
				dataRegionDef.ScopeNames = this.GetScopeNames(this.SelfReference, nameValuePairs);
			}
			else
			{
				IEnumerator enumerator = dataRegionDef.ScopeNames.Values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					RuntimeDataRegionObj.AddGroupNameValuePair(this.m_odpContext, enumerator.Current as AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping, nameValuePairs);
				}
			}
		}

		protected void ScopeNextNonAggregateRow(List<AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj> aggregates, ScalableList<DataFieldRow> dataRows)
		{
			RuntimeDataRegionObj.UpdateAggregates(this.m_odpContext, aggregates, true);
			this.CommonNextRow(dataRows);
		}

		internal static void CommonFirstRow(OnDemandProcessingContext odpContext, ref bool firstRowIsAggregate, ref DataFieldRow firstRow)
		{
			if (!firstRowIsAggregate && firstRow != null)
			{
				return;
			}
			firstRow = new DataFieldRow(odpContext.ReportObjectModel.FieldsImpl, true);
			firstRowIsAggregate = odpContext.ReportObjectModel.FieldsImpl.IsAggregateRow;
		}

		protected void CommonNextRow(ScalableList<DataFieldRow> dataRows)
		{
			if (dataRows != null)
			{
				RuntimeDataTablixObj.SaveData(dataRows, this.m_odpContext);
			}
			this.SendToInner();
		}

		protected virtual void SendToInner()
		{
			Global.Tracer.Assert(false);
		}

		protected void ScopeNextAggregateRow(RuntimeUserSortTargetInfo sortTargetInfo)
		{
			if (sortTargetInfo != null)
			{
				if (sortTargetInfo.AggregateRows == null)
				{
					sortTargetInfo.AggregateRows = new List<AggregateRow>();
				}
				AggregateRow item = new AggregateRow(this.m_odpContext.ReportObjectModel.FieldsImpl, true);
				sortTargetInfo.AggregateRows.Add(item);
				if (!sortTargetInfo.TargetForNonDetailSort)
				{
					return;
				}
			}
			this.SendToInner();
		}

		protected void ScopeFinishSorting(ref DataFieldRow firstRow, RuntimeUserSortTargetInfo sortTargetInfo)
		{
			Global.Tracer.Assert(null != sortTargetInfo, "(null != sortTargetInfo)");
			firstRow = null;
			sortTargetInfo.SortTree.Traverse(ProcessingStages.UserSortFilter, true, null);
			sortTargetInfo.SortTree.Dispose();
			sortTargetInfo.SortTree = null;
			if (sortTargetInfo.AggregateRows != null)
			{
				for (int i = 0; i < sortTargetInfo.AggregateRows.Count; i++)
				{
					sortTargetInfo.AggregateRows[i].SetFields(this.m_odpContext.ReportObjectModel.FieldsImpl);
					this.SendToInner();
				}
				sortTargetInfo.AggregateRows = null;
			}
		}

		internal virtual bool TargetScopeMatched(int index, bool detailSort)
		{
			Global.Tracer.Assert(false);
			return false;
		}

		internal virtual void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			Global.Tracer.Assert(false);
		}

		protected void ReleaseDataRows(DataActions finishedDataAction, ref DataActions dataAction, ref ScalableList<DataFieldRow> dataRows)
		{
			dataAction &= ~finishedDataAction;
			if (dataAction == DataActions.None)
			{
				dataRows.Clear();
				dataRows = null;
			}
		}

		protected void DetailHandleSortFilterEvent(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<IScope> outerScope, bool isColumnAxis, int rowIndex)
		{
			using (outerScope.PinValue())
			{
				IScope scope = outerScope.Value();
				List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = this.m_odpContext.RuntimeSortFilterInfo;
				if (runtimeSortFilterInfo != null && dataRegionDef.SortFilterSourceDetailScopeInfo != null && !scope.TargetForNonDetailSort)
				{
					int count = runtimeSortFilterInfo.Count;
					for (int i = 0; i < count; i++)
					{
						IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[i];
						using (reference.PinValue())
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
							if (runtimeSortFilterEventInfo.EventSource.ContainingScopes != null && 0 < runtimeSortFilterEventInfo.EventSource.ContainingScopes.Count && -1 != dataRegionDef.SortFilterSourceDetailScopeInfo[i] && scope.TargetScopeMatched(i, false) && this.m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex() == dataRegionDef.SortFilterSourceDetailScopeInfo[i])
							{
								if (runtimeSortFilterEventInfo.EventSource.ContainingScopes.LastEntry == null)
								{
									AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem parent = runtimeSortFilterEventInfo.EventSource.Parent;
									if (runtimeSortFilterEventInfo.EventSource.IsSubReportTopLevelScope)
									{
										while (parent != null && !(parent is AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport))
										{
											parent = parent.Parent;
										}
										Global.Tracer.Assert(parent is AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport, "(parent is SubReport)");
										parent = parent.Parent;
									}
									if (parent == dataRegionDef)
									{
										runtimeSortFilterEventInfo.SetEventSourceScope(isColumnAxis, this.SelfReference, rowIndex);
									}
								}
								runtimeSortFilterEventInfo.AddDetailScopeInfo(isColumnAxis, this.SelfReference, rowIndex);
							}
						}
					}
				}
			}
		}

		protected void DetailGetScopeValues(IReference<IScope> outerScope, IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index)
		{
			Global.Tracer.Assert(null == targetScopeObj, "(null == targetScopeObj)");
			outerScope.Value().GetScopeValues(targetScopeObj, scopeValues, ref index);
			Global.Tracer.Assert(index < scopeValues.Length, "(index < scopeValues.Length)");
			List<object> list = new List<object>(1);
			list.Add(this.m_odpContext.ReportObjectModel.FieldsImpl.GetRowIndex());
			scopeValues[index++] = list;
		}

		protected bool DetailTargetScopeMatched(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef, IReference<IScope> outerScope, bool isColumnAxis, int index)
		{
			if (this.m_odpContext.RuntimeSortFilterInfo != null)
			{
				IReference<RuntimeSortFilterEventInfo> reference = this.m_odpContext.RuntimeSortFilterInfo[index];
				using (reference.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
					if (runtimeSortFilterEventInfo != null)
					{
						List<IReference<RuntimeDataRegionObj>> list = null;
						List<int> list2 = null;
						int num = -1;
						if (isColumnAxis)
						{
							list = runtimeSortFilterEventInfo.DetailColScopes;
							list2 = runtimeSortFilterEventInfo.DetailColScopeIndices;
							num = dataRegionDef.CurrentColDetailIndex;
						}
						else
						{
							list = runtimeSortFilterEventInfo.DetailRowScopes;
							list2 = runtimeSortFilterEventInfo.DetailRowScopeIndices;
							num = dataRegionDef.CurrentRowDetailIndex;
						}
						if (list != null)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (this.SelfReference.Equals(list[i]) && num == list2[i])
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		protected virtual void GetGroupNameValuePairs(Dictionary<string, object> pairs)
		{
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(RuntimeDataRegionObj.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.OdpContext:
				{
					int value = scalabilityCache.StoreStaticReference(this.m_odpContext);
					writer.Write(value);
					break;
				}
				case MemberName.ObjectType:
					writer.WriteEnum((int)this.m_objectType);
					break;
				case MemberName.Depth:
					writer.Write(this.m_depth);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(RuntimeDataRegionObj.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.OdpContext:
				{
					int id = reader.ReadInt32();
					this.m_odpContext = (OnDemandProcessingContext)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.ObjectType:
					this.m_objectType = (AspNetCore.ReportingServices.ReportProcessing.ObjectType)reader.ReadEnum();
					break;
				case MemberName.Depth:
					this.m_depth = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj;
		}

		public static Declaration GetDeclaration()
		{
			if (RuntimeDataRegionObj.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.OdpContext, Token.Int32));
				list.Add(new MemberInfo(MemberName.ObjectType, Token.Enum));
				list.Add(new MemberInfo(MemberName.Depth, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return RuntimeDataRegionObj.m_declaration;
		}

		public virtual void SetReference(IReference selfRef)
		{
			this.m_selfReference = (RuntimeDataRegionObjReference)selfRef;
		}
	}
}
