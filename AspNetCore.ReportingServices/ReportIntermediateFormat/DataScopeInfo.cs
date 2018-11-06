using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class DataScopeInfo : IPersistable
	{
		[NonSerialized]
		internal const int DataPipelineIDUnassigned = -1;

		private bool m_aggregatesSpanGroupFilter;

		private bool m_hasAggregatesToUpdateAtRowScope;

		private BucketedDataAggregateInfos m_aggregatesOfAggregates;

		private BucketedDataAggregateInfos m_postSortAggregatesOfAggregates;

		private List<RunningValueInfo> m_runningValuesOfAggregates;

		private int m_scopeID;

		private int m_dataPipelineID = -1;

		[Reference]
		private DataSet m_dataSet;

		private bool m_isDecomposable;

		private JoinInfo m_joinInfo;

		private List<int> m_groupingFieldIndicesForServerAggregates;

		[NonSerialized]
		private string m_dataSetName;

		[NonSerialized]
		private long m_lastScopeInstanceNumber;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataScopeInfo.GetDeclaration();

		internal bool AggregatesSpanGroupFilter
		{
			get
			{
				return this.m_aggregatesSpanGroupFilter;
			}
			set
			{
				if (!this.m_aggregatesSpanGroupFilter)
				{
					this.m_aggregatesSpanGroupFilter = value;
				}
			}
		}

		internal bool HasAggregatesToUpdateAtRowScope
		{
			get
			{
				return this.m_hasAggregatesToUpdateAtRowScope;
			}
			set
			{
				if (!this.m_hasAggregatesToUpdateAtRowScope)
				{
					this.m_hasAggregatesToUpdateAtRowScope = value;
				}
			}
		}

		internal bool NeedsSeparateAofAPass
		{
			get
			{
				return this.AggregatesSpanGroupFilter;
			}
		}

		internal BucketedDataAggregateInfos AggregatesOfAggregates
		{
			get
			{
				return this.m_aggregatesOfAggregates;
			}
			set
			{
				this.m_aggregatesOfAggregates = value;
			}
		}

		internal BucketedDataAggregateInfos PostSortAggregatesOfAggregates
		{
			get
			{
				return this.m_postSortAggregatesOfAggregates;
			}
			set
			{
				this.m_postSortAggregatesOfAggregates = value;
			}
		}

		internal List<RunningValueInfo> RunningValuesOfAggregates
		{
			get
			{
				return this.m_runningValuesOfAggregates;
			}
			set
			{
				this.m_runningValuesOfAggregates = value;
			}
		}

		internal int ScopeID
		{
			get
			{
				return this.m_scopeID;
			}
			set
			{
				this.m_scopeID = value;
			}
		}

		internal DataSet DataSet
		{
			get
			{
				return this.m_dataSet;
			}
		}

		internal int DataPipelineID
		{
			get
			{
				return this.m_dataPipelineID;
			}
			set
			{
				this.m_dataPipelineID = value;
			}
		}

		internal bool HasAggregatesOrRunningValues
		{
			get
			{
				if (!DataScopeInfo.HasAggregates(this.m_aggregatesOfAggregates) && !DataScopeInfo.HasAggregates(this.m_postSortAggregatesOfAggregates))
				{
					return this.HasRunningValues;
				}
				return true;
			}
		}

		internal bool HasRunningValues
		{
			get
			{
				return this.m_runningValuesOfAggregates.Count > 0;
			}
		}

		internal bool IsDecomposable
		{
			get
			{
				return this.m_isDecomposable;
			}
			set
			{
				this.m_isDecomposable = value;
			}
		}

		internal bool NeedsIDC
		{
			get
			{
				return this.m_joinInfo != null;
			}
		}

		internal JoinInfo JoinInfo
		{
			get
			{
				return this.m_joinInfo;
			}
		}

		internal List<int> GroupingFieldIndicesForServerAggregates
		{
			get
			{
				return this.m_groupingFieldIndicesForServerAggregates;
			}
		}

		public DataScopeInfo()
		{
		}

		public DataScopeInfo(int scopeId)
		{
			this.m_scopeID = scopeId;
			this.m_runningValuesOfAggregates = new List<RunningValueInfo>();
			this.m_aggregatesOfAggregates = new BucketedDataAggregateInfos();
			this.m_postSortAggregatesOfAggregates = new BucketedDataAggregateInfos();
		}

		internal void ApplyGroupingFieldsForServerAggregates(FieldsImpl fields)
		{
			if (this.m_groupingFieldIndicesForServerAggregates != null)
			{
				for (int i = 0; i < this.m_groupingFieldIndicesForServerAggregates.Count; i++)
				{
					fields.ConsumeAggregationField(this.m_groupingFieldIndicesForServerAggregates[i]);
				}
			}
		}

		internal void SetRelationship(string dataSetName, IdcRelationship relationship)
		{
			this.m_dataSetName = dataSetName;
			if (relationship != null)
			{
				this.m_joinInfo = new LinearJoinInfo(relationship);
			}
		}

		internal void SetRelationship(string dataSetName, List<IdcRelationship> relationships)
		{
			this.m_dataSetName = dataSetName;
			if (relationships != null)
			{
				this.m_joinInfo = new IntersectJoinInfo(relationships);
			}
		}

		internal void ValidateScopeRulesForIdc(InitializationContext context, IRIFDataScope dataScope)
		{
			if (this.m_dataSet != null && this.NeedsIDC)
			{
				this.m_joinInfo.ValidateScopeRulesForIdcNaturalJoin(context, dataScope);
				context.EnsureDataSetUsedOnceForIdcUnderTopDataRegion(this.m_dataSet, dataScope);
			}
		}

		internal void ValidateDataSetBindingAndRelationships(ScopeTree scopeTree, IRIFReportDataScope scope, ErrorContext errorContext)
		{
			if (this.m_dataSet == null)
			{
				ParentDataSetContainer parentDataSetContainer = DataScopeInfo.DetermineParentDataSets(scopeTree, scope);
				if (this.m_dataSetName == null)
				{
					this.BindToParentDataSet(scopeTree, scope, errorContext, parentDataSetContainer);
				}
				else
				{
					this.BindToNamedDataSet(scopeTree, scope, errorContext, parentDataSetContainer);
				}
				if (this.m_dataSet != null)
				{
					if (scopeTree.GetParentDataRegion(scope) == null && this.m_joinInfo != null)
					{
						errorContext.Register(ProcessingErrorCode.rsInvalidRelationshipTopLevelDataRegion, Severity.Error, scope.DataScopeObjectType, scope.Name, "Relationship");
						this.m_joinInfo = null;
					}
					if (parentDataSetContainer != null && this.m_joinInfo == null)
					{
						if (parentDataSetContainer.Count == 1)
						{
							this.m_joinInfo = new LinearJoinInfo();
						}
						else
						{
							this.m_joinInfo = new IntersectJoinInfo();
						}
					}
					if (this.m_joinInfo != null)
					{
						if (!this.m_joinInfo.ValidateRelationships(scopeTree, errorContext, this.m_dataSet, parentDataSetContainer, scope))
						{
							this.m_joinInfo = null;
						}
						if (this.m_joinInfo == null && this.m_dataSetName != null && this.m_dataSet != null && !DataSet.AreEqualById(parentDataSetContainer.ParentDataSet, this.m_dataSet))
						{
							this.UpdateDataSet(parentDataSetContainer.ParentDataSet, scope);
						}
					}
				}
			}
		}

		private void BindToNamedDataSet(ScopeTree scopeTree, IRIFReportDataScope scope, ErrorContext errorContext, ParentDataSetContainer parentDataSets)
		{
			DataSet dataSet = scopeTree.GetDataSet(this.m_dataSetName);
			if (dataSet == null)
			{
				DataRegion parentDataRegion = scopeTree.GetParentDataRegion(scope);
				if (parentDataSets != null && parentDataSets.Count == 1 && scope is DataRegion && parentDataRegion != null)
				{
					this.UpdateDataSet(parentDataSets.ParentDataSet, scope);
					this.m_dataSetName = parentDataSets.ParentDataSet.Name;
				}
				else
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidDataSetName, Severity.Error, scope.DataScopeObjectType, scope.Name, "DataSetName", this.m_dataSetName.MarkAsPrivate());
					if (parentDataSets != null)
					{
						this.UpdateDataSet(parentDataSets.ParentDataSet, scope);
					}
				}
			}
			else
			{
				this.UpdateDataSet(dataSet, scope);
			}
		}

		private void BindToParentDataSet(ScopeTree scopeTree, IRIFReportDataScope scope, ErrorContext errorContext, ParentDataSetContainer parentDataSets)
		{
			if (parentDataSets == null)
			{
				DataRegion parentDataRegion = scopeTree.GetParentDataRegion(scope);
				if (parentDataRegion == null)
				{
					if (scopeTree.Report.MappingDataSetIndexToDataSet.Count == 0)
					{
						errorContext.Register(ProcessingErrorCode.rsDataRegionWithoutDataSet, Severity.Error, scope.DataScopeObjectType, scope.Name, null);
					}
					else
					{
						errorContext.Register(ProcessingErrorCode.rsMissingDataSetName, Severity.Error, scope.DataScopeObjectType, scope.Name, "DataSetName");
					}
				}
			}
			else if (parentDataSets.Count > 1 && !parentDataSets.AreAllSameDataSet())
			{
				DataRegion parentDataRegion2 = scopeTree.GetParentDataRegion(scope);
				IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(scope);
				IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(scope);
				errorContext.Register(ProcessingErrorCode.rsMissingIntersectionDataSetName, Severity.Error, scope.DataScopeObjectType, parentDataRegion2.Name, "DataSetName", parentDataRegion2.ObjectType.ToString(), parentRowScopeForIntersection.Name, parentColumnScopeForIntersection.Name);
			}
			else
			{
				this.UpdateDataSet(parentDataSets.ParentDataSet, scope);
			}
		}

		private static ParentDataSetContainer DetermineParentDataSets(ScopeTree scopeTree, IRIFReportDataScope scope)
		{
			if (scopeTree.IsIntersectionScope(scope))
			{
				IRIFDataScope parentRowScopeForIntersection = scopeTree.GetParentRowScopeForIntersection(scope);
				IRIFDataScope parentColumnScopeForIntersection = scopeTree.GetParentColumnScopeForIntersection(scope);
				return new ParentDataSetContainer(parentRowScopeForIntersection.DataScopeInfo.DataSet, parentColumnScopeForIntersection.DataScopeInfo.DataSet);
			}
			IRIFDataScope parentScope = scopeTree.GetParentScope(scope);
			DataSet dataSet = (parentScope != null) ? parentScope.DataScopeInfo.DataSet : scopeTree.GetDefaultTopLevelDataSet();
			if (dataSet == null)
			{
				return null;
			}
			return new ParentDataSetContainer(dataSet);
		}

		private void UpdateDataSet(DataSet targetDataSet, IRIFDataScope scope)
		{
			this.m_dataSet = targetDataSet;
			DataRegion dataRegion = scope as DataRegion;
			if (dataRegion != null && this.m_dataSet != null)
			{
				dataRegion.DataSetName = this.m_dataSet.Name;
			}
		}

		internal void ClearAggregatesIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_aggregatesOfAggregates, "(null != m_aggregatesOfAggregates)");
			if (this.m_aggregatesOfAggregates.IsEmpty)
			{
				this.m_aggregatesOfAggregates = null;
			}
			Global.Tracer.Assert(null != this.m_postSortAggregatesOfAggregates, "(null != m_postSortAggregatesOfAggregates)");
			if (this.m_postSortAggregatesOfAggregates.IsEmpty)
			{
				this.m_postSortAggregatesOfAggregates = null;
			}
		}

		internal void ClearRunningValuesIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_runningValuesOfAggregates, "(null != m_runningValuesOfAggregates)");
			if (this.m_runningValuesOfAggregates.Count == 0)
			{
				this.m_runningValuesOfAggregates.Clear();
			}
		}

		public void MergeFrom(DataScopeInfo otherScope)
		{
			this.m_aggregatesSpanGroupFilter |= otherScope.m_aggregatesSpanGroupFilter;
			this.m_hasAggregatesToUpdateAtRowScope |= otherScope.m_hasAggregatesToUpdateAtRowScope;
			this.m_runningValuesOfAggregates.AddRange(otherScope.m_runningValuesOfAggregates);
			this.m_aggregatesOfAggregates.MergeFrom(otherScope.m_aggregatesOfAggregates);
			this.m_postSortAggregatesOfAggregates.MergeFrom(otherScope.m_postSortAggregatesOfAggregates);
		}

		internal DataScopeInfo PublishClone(AutomaticSubtotalContext context, int scopeID)
		{
			DataScopeInfo dataScopeInfo = new DataScopeInfo(scopeID);
			dataScopeInfo.m_dataSetName = this.m_dataSetName;
			if (this.m_joinInfo != null)
			{
				dataScopeInfo.m_joinInfo = this.m_joinInfo.PublishClone(context);
			}
			return dataScopeInfo;
		}

		internal void Initialize(InitializationContext context, IRIFDataScope scope)
		{
			if (this.m_joinInfo != null)
			{
				this.m_joinInfo.Initialize(context);
				this.InjectAggregateIndicatorFieldJoinConditions(context, scope);
			}
			this.CaptureGroupingFieldsForServerAggregates(context, scope);
		}

		private void CaptureGroupingFieldsForServerAggregates(InitializationContext context, IRIFDataScope scope)
		{
			if (this.m_groupingFieldIndicesForServerAggregates == null)
			{
				this.m_groupingFieldIndicesForServerAggregates = new List<int>();
				if (this.m_dataSet != null)
				{
					ScopeTree scopeTree = context.ScopeTree;
					if (scopeTree.IsIntersectionScope(scope))
					{
						this.AddGroupingFieldIndicesFromParentScope(context, scopeTree.GetParentRowScopeForIntersection(scope));
						this.AddGroupingFieldIndicesFromParentScope(context, scopeTree.GetParentColumnScopeForIntersection(scope));
					}
					else
					{
						IRIFDataScope parentScope = scopeTree.GetParentScope(scope);
						if (parentScope != null)
						{
							this.AddGroupingFieldIndicesFromParentScope(context, parentScope);
						}
						ReportHierarchyNode reportHierarchyNode = scope as ReportHierarchyNode;
						if (reportHierarchyNode != null && !reportHierarchyNode.IsStatic && !reportHierarchyNode.Grouping.IsDetail)
						{
							foreach (ExpressionInfo groupExpression in reportHierarchyNode.Grouping.GroupExpressions)
							{
								if (groupExpression.Type == ExpressionInfo.Types.Field)
								{
									this.m_groupingFieldIndicesForServerAggregates.Add(groupExpression.FieldIndex);
								}
							}
						}
					}
				}
			}
		}

		private void AddGroupingFieldIndicesFromParentScope(InitializationContext context, IRIFDataScope parentScope)
		{
			Global.Tracer.Assert(parentScope.DataScopeInfo.GroupingFieldIndicesForServerAggregates != null, "Grouping fields for parent should have been captured first.");
			if (DataSet.AreEqualById(parentScope.DataScopeInfo.DataSet, this.m_dataSet))
			{
				this.m_groupingFieldIndicesForServerAggregates.AddRange(parentScope.DataScopeInfo.GroupingFieldIndicesForServerAggregates);
			}
			else if (this.m_joinInfo == null)
			{
				Global.Tracer.Assert(context.ErrorContext.HasError, "Missing expected error.");
			}
			else
			{
				this.m_joinInfo.AddMappedFieldIndices(parentScope.DataScopeInfo.GroupingFieldIndicesForServerAggregates, parentScope.DataScopeInfo.DataSet, this.m_dataSet, this.m_groupingFieldIndicesForServerAggregates);
			}
		}

		private void InjectAggregateIndicatorFieldJoinConditions(InitializationContext context, IRIFDataScope scope)
		{
			if (this.m_dataSet != null && this.m_joinInfo.Relationships != null)
			{
				PublishingContextKind publishingContextKind = context.PublishingContext.PublishingContextKind;
				if (context.PublishingContext.PublishingContextKind != PublishingContextKind.DataShape && scope.DataScopeObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping)
				{
					int count = this.m_dataSet.Fields.Count;
					foreach (IdcRelationship relationship in this.m_joinInfo.Relationships)
					{
						if (!relationship.IsCrossJoin)
						{
							for (int i = 0; i < count; i++)
							{
								Field field = this.m_dataSet.Fields[i];
								if (field.AggregateIndicatorFieldIndex >= 0)
								{
									Field field2 = this.m_dataSet.Fields[field.AggregateIndicatorFieldIndex];
									if (!field2.IsCalculatedField)
									{
										relationship.InsertAggregateIndicatorJoinCondition(field, i, field2, field.AggregateIndicatorFieldIndex, context);
									}
								}
							}
						}
					}
				}
			}
		}

		internal long AssignScopeInstanceNumber()
		{
			this.m_lastScopeInstanceNumber += 1L;
			return this.m_lastScopeInstanceNumber;
		}

		internal bool IsLastScopeInstanceNumber(long scopeInstanceNumber)
		{
			return this.m_lastScopeInstanceNumber == scopeInstanceNumber;
		}

		internal void ResetAggregates(AggregatesImpl reportOmAggregates)
		{
			reportOmAggregates.ResetAll(this.m_aggregatesOfAggregates);
			reportOmAggregates.ResetAll(this.m_postSortAggregatesOfAggregates);
			reportOmAggregates.ResetAll(this.m_runningValuesOfAggregates);
		}

		internal bool IsSameScope(DataScopeInfo candidateScopeInfo)
		{
			return this.m_scopeID == candidateScopeInfo.ScopeID;
		}

		internal static bool IsSameOrChildScope(IRIFReportDataScope childCandidate, IRIFReportDataScope parentCandidate)
		{
			while (childCandidate != null)
			{
				if (childCandidate.DataScopeInfo.ScopeID == parentCandidate.DataScopeInfo.ScopeID)
				{
					return true;
				}
				if (childCandidate.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)childCandidate;
					if (!DataScopeInfo.IsSameOrChildScope(iRIFReportIntersectionScope.ParentRowReportScope, parentCandidate))
					{
						return DataScopeInfo.IsSameOrChildScope(iRIFReportIntersectionScope.ParentColumnReportScope, parentCandidate);
					}
					return true;
				}
				childCandidate = childCandidate.ParentReportScope;
			}
			return false;
		}

		internal static bool IsChildScopeOf(IRIFReportDataScope childCandidate, IRIFReportDataScope parentCandidate)
		{
			if (!childCandidate.DataScopeInfo.IsSameScope(parentCandidate.DataScopeInfo))
			{
				return DataScopeInfo.IsSameOrChildScope(childCandidate, parentCandidate);
			}
			return false;
		}

		internal static bool HasDecomposableAncestorWithNonLatestInstanceBinding(IRIFReportDataScope candidate)
		{
			while (candidate != null)
			{
				if (candidate.IsScope && candidate.DataScopeInfo.IsDecomposable && candidate.IsBoundToStreamingScopeInstance && !candidate.CurrentStreamingScopeInstance.Value().IsMostRecentlyCreatedScopeInstance)
				{
					return true;
				}
				if (candidate.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)candidate;
					if (!DataScopeInfo.HasDecomposableAncestorWithNonLatestInstanceBinding(iRIFReportIntersectionScope.ParentRowReportScope))
					{
						return DataScopeInfo.HasDecomposableAncestorWithNonLatestInstanceBinding(iRIFReportIntersectionScope.ParentColumnReportScope);
					}
					return true;
				}
				candidate = candidate.ParentReportScope;
			}
			return false;
		}

		internal static bool TryGetInnermostParentScopeRelatedToTargetDataSet(DataSet targetDataSet, IRIFReportDataScope candidate, out IRIFReportDataScope targetScope)
		{
			while (candidate != null)
			{
				if (targetDataSet.HasDefaultRelationship(candidate.DataScopeInfo.DataSet))
				{
					targetScope = candidate;
					return true;
				}
				if (candidate.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)candidate;
					if (!DataScopeInfo.TryGetInnermostParentScopeRelatedToTargetDataSet(targetDataSet, iRIFReportIntersectionScope.ParentRowReportScope, out targetScope))
					{
						return DataScopeInfo.TryGetInnermostParentScopeRelatedToTargetDataSet(targetDataSet, iRIFReportIntersectionScope.ParentColumnReportScope, out targetScope);
					}
					return true;
				}
				candidate = candidate.ParentReportScope;
			}
			targetScope = null;
			return false;
		}

		internal static bool HasAggregates<T>(List<T> aggs) where T : DataAggregateInfo
		{
			if (aggs != null)
			{
				return aggs.Count > 0;
			}
			return false;
		}

		internal static bool HasNonServerAggregates<T>(List<T> aggs) where T : DataAggregateInfo
		{
			if (aggs != null)
			{
				return aggs.Any((T agg) => agg.AggregateType != DataAggregateInfo.AggregateTypes.Aggregate);
			}
			return false;
		}

		internal static bool HasAggregates<T>(BucketedAggregatesCollection<T> aggs) where T : IPersistable
		{
			if (aggs != null)
			{
				return !aggs.IsEmpty;
			}
			return false;
		}

		internal static bool ContainsServerAggregate<T>(List<T> aggs, string aggregateName) where T : DataAggregateInfo
		{
			if (aggs != null)
			{
				return ((IEnumerable<T>)aggs).Any((Func<T, bool>)((T agg) => DataScopeInfo.IsTargetServerAggregate((DataAggregateInfo)(object)agg, aggregateName)));
			}
			return false;
		}

		internal static bool IsTargetServerAggregate(DataAggregateInfo agg, string aggregateName)
		{
			if (agg.AggregateType == DataAggregateInfo.AggregateTypes.Aggregate)
			{
				if (!string.Equals(agg.Name, aggregateName, StringComparison.Ordinal))
				{
					if (agg.DuplicateNames != null)
					{
						return ListUtils.ContainsWithOrdinalComparer(aggregateName, agg.DuplicateNames);
					}
					return false;
				}
				return true;
			}
			return false;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.AggregatesSpanGroupFilter, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AggregatesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateInfos));
			list.Add(new MemberInfo(MemberName.PostSortAggregatesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BucketedDataAggregateInfos));
			list.Add(new MemberInfo(MemberName.RunningValuesOfAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo));
			list.Add(new MemberInfo(MemberName.ScopeID, Token.Int32));
			list.Add(new MemberInfo(MemberName.HasAggregatesToUpdateAtRowScope, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsDecomposable, Token.Boolean));
			list.Add(new MemberInfo(MemberName.DataSet, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			list.Add(new MemberInfo(MemberName.JoinInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.JoinInfo));
			list.Add(new MemberInfo(MemberName.DataPipelineID, Token.Int32));
			list.Add(new MemberInfo(MemberName.GroupingFieldIndicesForServerAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DataScopeInfo.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.AggregatesSpanGroupFilter:
					writer.Write(this.m_aggregatesSpanGroupFilter);
					break;
				case MemberName.AggregatesOfAggregates:
					writer.Write(this.m_aggregatesOfAggregates);
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					writer.Write(this.m_postSortAggregatesOfAggregates);
					break;
				case MemberName.RunningValuesOfAggregates:
					writer.Write(this.m_runningValuesOfAggregates);
					break;
				case MemberName.ScopeID:
					writer.Write(this.m_scopeID);
					break;
				case MemberName.HasAggregatesToUpdateAtRowScope:
					writer.Write(this.m_hasAggregatesToUpdateAtRowScope);
					break;
				case MemberName.IsDecomposable:
					writer.Write(this.m_isDecomposable);
					break;
				case MemberName.DataSet:
					writer.WriteReference(this.m_dataSet);
					break;
				case MemberName.JoinInfo:
					writer.Write(this.m_joinInfo);
					break;
				case MemberName.DataPipelineID:
					writer.Write(this.m_dataPipelineID);
					break;
				case MemberName.GroupingFieldIndicesForServerAggregates:
					writer.WriteListOfPrimitives(this.m_groupingFieldIndicesForServerAggregates);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DataScopeInfo.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.AggregatesSpanGroupFilter:
					this.m_aggregatesSpanGroupFilter = reader.ReadBoolean();
					break;
				case MemberName.AggregatesOfAggregates:
					this.m_aggregatesOfAggregates = (BucketedDataAggregateInfos)reader.ReadRIFObject();
					break;
				case MemberName.PostSortAggregatesOfAggregates:
					this.m_postSortAggregatesOfAggregates = (BucketedDataAggregateInfos)reader.ReadRIFObject();
					break;
				case MemberName.RunningValuesOfAggregates:
					this.m_runningValuesOfAggregates = reader.ReadGenericListOfRIFObjects<RunningValueInfo>();
					break;
				case MemberName.ScopeID:
					this.m_scopeID = reader.ReadInt32();
					break;
				case MemberName.HasAggregatesToUpdateAtRowScope:
					this.m_hasAggregatesToUpdateAtRowScope = reader.ReadBoolean();
					break;
				case MemberName.IsDecomposable:
					this.m_isDecomposable = reader.ReadBoolean();
					break;
				case MemberName.DataSet:
					this.m_dataSet = reader.ReadReference<DataSet>(this);
					break;
				case MemberName.JoinInfo:
					this.m_joinInfo = (JoinInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataPipelineID:
					this.m_dataPipelineID = reader.ReadInt32();
					break;
				case MemberName.GroupingFieldIndicesForServerAggregates:
					this.m_groupingFieldIndicesForServerAggregates = reader.ReadListOfPrimitives<int>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(DataScopeInfo.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.DataSet)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
						Global.Tracer.Assert(this.m_dataSet != (DataSet)referenceableItems[item.RefID]);
						this.m_dataSet = (DataSet)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo;
		}
	}
}
