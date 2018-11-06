using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal struct InitializationContext
	{
		private enum GroupingType
		{
			Normal,
			TablixRow,
			TablixColumn
		}

		private sealed class VisibilityContainmentInfo
		{
			internal IVisibilityOwner ContainingVisibility;

			internal IVisibilityOwner ContainingRowVisibility;

			internal IVisibilityOwner ContainingColumnVisibility;

			internal VisibilityContainmentInfo()
			{
			}
		}

		private sealed class ScopeInfo
		{
			private bool m_isTopLevelScope;

			private bool m_allowCustomAggregates;

			private List<DataAggregateInfo> m_aggregates;

			private List<DataAggregateInfo> m_postSortAggregates;

			private List<DataAggregateInfo> m_recursiveAggregates;

			private bool m_duplicateScope;

			private Grouping m_groupingScope;

			private DataRegion m_dataRegionScope;

			private DataSet m_dataSetScope;

			private IRIFReportScope m_reportScope;

			internal bool AllowCustomAggregates
			{
				get
				{
					return this.m_allowCustomAggregates;
				}
			}

			internal List<DataAggregateInfo> Aggregates
			{
				get
				{
					return this.m_aggregates;
				}
			}

			internal List<DataAggregateInfo> PostSortAggregates
			{
				get
				{
					return this.m_postSortAggregates;
				}
			}

			internal List<DataAggregateInfo> RecursiveAggregates
			{
				get
				{
					return this.m_recursiveAggregates;
				}
			}

			internal IRIFReportScope ReportScope
			{
				get
				{
					return this.m_reportScope;
				}
			}

			internal Grouping GroupingScope
			{
				get
				{
					return this.m_groupingScope;
				}
			}

			internal DataRegion DataRegionScope
			{
				get
				{
					return this.m_dataRegionScope;
				}
			}

			internal DataSet DataSetScope
			{
				get
				{
					return this.m_dataSetScope;
				}
			}

			internal IRIFDataScope DataScope
			{
				get
				{
					if (this.m_groupingScope != null)
					{
						return this.m_groupingScope.Owner;
					}
					if (this.m_dataRegionScope != null)
					{
						return this.m_dataRegionScope;
					}
					if (this.m_dataSetScope != null)
					{
						return this.m_dataSetScope;
					}
					return this.m_reportScope as IRIFDataScope;
				}
			}

			internal DataScopeInfo DataScopeInfo
			{
				get
				{
					IRIFDataScope dataScope = this.DataScope;
					if (dataScope != null)
					{
						return dataScope.DataScopeInfo;
					}
					return null;
				}
			}

			internal bool IsTopLevelScope
			{
				get
				{
					return this.m_isTopLevelScope;
				}
			}

			internal bool IsDuplicateScope
			{
				get
				{
					return this.m_duplicateScope;
				}
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, IRIFReportScope reportScope)
				: this(allowCustomAggregates, aggregates, null, null, null, reportScope)
			{
				this.m_isTopLevelScope = true;
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, IRIFReportScope reportScope)
				: this(allowCustomAggregates, aggregates, postSortAggregates, null, null, reportScope)
			{
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, DataRegion dataRegion)
				: this(allowCustomAggregates, aggregates, postSortAggregates, null, null, dataRegion)
			{
				this.m_dataRegionScope = dataRegion;
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, DataSet dataset, bool duplicateScope)
				: this(allowCustomAggregates, aggregates, postSortAggregates, dataset)
			{
				this.m_duplicateScope = duplicateScope;
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, DataSet dataset)
				: this(allowCustomAggregates, aggregates, postSortAggregates, null, null, null)
			{
				this.m_dataSetScope = dataset;
			}

			internal ScopeInfo(bool allowCustomAggregates, List<DataAggregateInfo> aggregates, List<DataAggregateInfo> postSortAggregates, List<DataAggregateInfo> recursiveAggregates, Grouping groupingScope, IRIFReportScope reportScope)
			{
				this.m_allowCustomAggregates = allowCustomAggregates;
				this.m_aggregates = aggregates;
				this.m_postSortAggregates = postSortAggregates;
				this.m_recursiveAggregates = recursiveAggregates;
				this.m_reportScope = reportScope;
				this.m_groupingScope = groupingScope;
			}
		}

		internal class HashtableKeyComparer : IEqualityComparer<Hashtable>
		{
			private static HashtableKeyComparer m_Instance;

			internal static HashtableKeyComparer Instance
			{
				get
				{
					if (HashtableKeyComparer.m_Instance == null)
					{
						HashtableKeyComparer.m_Instance = new HashtableKeyComparer();
					}
					return HashtableKeyComparer.m_Instance;
				}
			}

			private HashtableKeyComparer()
			{
			}

			public bool Equals(Hashtable x, Hashtable y)
			{
				if (x != y)
				{
					if (x.Count != y.Count)
					{
						return false;
					}
					foreach (object key in x.Keys)
					{
						if (!y.ContainsKey(key))
						{
							return false;
						}
					}
				}
				return true;
			}

			public int GetHashCode(Hashtable obj)
			{
				int num = obj.Count;
				foreach (object key in obj.Keys)
				{
					num ^= key.GetHashCode();
				}
				return num;
			}
		}

		private sealed class AxisGroupingScopesForRunningValues
		{
			private int m_columnGroupExprCount;

			private int m_rowGroupExprCount;

			private int m_previousDRsColumnGroupExprCount;

			private int m_previousDRsRowGroupExprCount;

			private int m_rowDetailCount;

			private int m_colDetailCount;

			private int m_previousDRsColumnDetailCount;

			private int m_previousDRsRowDetailCount;

			internal bool InCurrentDataRegionDynamicRow
			{
				get
				{
					if (this.m_rowGroupExprCount <= this.m_previousDRsRowGroupExprCount)
					{
						return this.m_rowDetailCount > this.m_previousDRsRowDetailCount;
					}
					return true;
				}
			}

			internal bool InCurrentDataRegionDynamicColumn
			{
				get
				{
					if (this.m_columnGroupExprCount <= this.m_previousDRsColumnGroupExprCount)
					{
						return this.m_colDetailCount > this.m_previousDRsColumnDetailCount;
					}
					return true;
				}
			}

			internal AxisGroupingScopesForRunningValues()
			{
			}

			internal AxisGroupingScopesForRunningValues(AxisGroupingScopesForRunningValues axisGroupingScopesForRunningValues)
			{
				this.m_columnGroupExprCount = axisGroupingScopesForRunningValues.m_columnGroupExprCount;
				this.m_rowGroupExprCount = axisGroupingScopesForRunningValues.m_rowGroupExprCount;
				this.m_colDetailCount = axisGroupingScopesForRunningValues.m_colDetailCount;
				this.m_rowDetailCount = axisGroupingScopesForRunningValues.m_rowDetailCount;
				this.SetCountsForDataRegion();
			}

			internal void RegisterColumnGrouping(Grouping grouping)
			{
				if (!grouping.IsDetail)
				{
					this.m_columnGroupExprCount += grouping.GroupExpressions.Count;
				}
				else
				{
					this.m_colDetailCount++;
				}
			}

			internal void RegisterRowGrouping(Grouping grouping)
			{
				if (!grouping.IsDetail)
				{
					this.m_rowGroupExprCount += grouping.GroupExpressions.Count;
				}
				else
				{
					this.m_rowDetailCount++;
				}
			}

			internal void UnregisterColumnGrouping(Grouping grouping)
			{
				if (!grouping.IsDetail)
				{
					this.m_columnGroupExprCount -= grouping.GroupExpressions.Count;
				}
				else
				{
					this.m_colDetailCount--;
				}
			}

			internal void UnregisterRowGrouping(Grouping grouping)
			{
				if (!grouping.IsDetail)
				{
					this.m_rowGroupExprCount -= grouping.GroupExpressions.Count;
				}
				else
				{
					this.m_rowDetailCount--;
				}
			}

			private void SetCountsForDataRegion()
			{
				this.m_previousDRsColumnGroupExprCount = this.m_columnGroupExprCount;
				this.m_previousDRsRowGroupExprCount = this.m_rowGroupExprCount;
				this.m_previousDRsColumnDetailCount = this.m_colDetailCount;
				this.m_previousDRsRowDetailCount = this.m_rowDetailCount;
			}
		}

		private sealed class GroupingScopesForTablix
		{
			private AspNetCore.ReportingServices.ReportProcessing.ObjectType m_containerType;

			private DataRegion m_containerScope;

			private Hashtable m_rowScopes;

			private Hashtable m_columnScopes;

			private FunctionalList<Grouping> m_rowScopeStack;

			private FunctionalList<Grouping> m_columnScopeStack;

			internal bool CurrentRowScopeIsDetail
			{
				get
				{
					if (this.m_rowScopeStack.Count != 0)
					{
						return this.m_rowScopeStack.First.IsDetail;
					}
					return true;
				}
			}

			internal bool CurrentColumnScopeIsDetail
			{
				get
				{
					if (this.m_columnScopeStack.Count != 0)
					{
						return this.m_columnScopeStack.First.IsDetail;
					}
					return true;
				}
			}

			internal string CurrentRowScopeName
			{
				get
				{
					if (this.m_rowScopeStack.Count != 0)
					{
						return this.m_rowScopeStack.First.Name;
					}
					return null;
				}
			}

			internal string CurrentColumnScopeName
			{
				get
				{
					if (this.m_columnScopeStack.Count != 0)
					{
						return this.m_columnScopeStack.First.Name;
					}
					return null;
				}
			}

			internal DataRegion ContainerScope
			{
				get
				{
					return this.m_containerScope;
				}
			}

			internal string ContainerName
			{
				get
				{
					return this.m_containerScope.Name;
				}
			}

			internal bool IsRunningValueDirectionColumn
			{
				get
				{
					return this.m_containerScope.ColumnScopeFound;
				}
			}

			internal GroupingScopesForTablix(bool forceRows, AspNetCore.ReportingServices.ReportProcessing.ObjectType containerType, DataRegion containerScope)
			{
				containerScope.RowScopeFound = forceRows;
				containerScope.ColumnScopeFound = false;
				this.m_containerType = containerType;
				this.m_rowScopes = new Hashtable();
				this.m_columnScopes = new Hashtable();
				this.m_rowScopeStack = FunctionalList<Grouping>.Empty;
				this.m_columnScopeStack = FunctionalList<Grouping>.Empty;
				this.m_containerScope = containerScope;
			}

			internal ScopeChainInfo GetScopeChainInfo()
			{
				return new ScopeChainInfo(this.m_containerScope, this.m_rowScopeStack, this.m_columnScopeStack);
			}

			internal DataRegion SetContainerScope(DataRegion dataRegion)
			{
				DataRegion containerScope = this.m_containerScope;
				this.m_containerScope = dataRegion;
				return containerScope;
			}

			internal void RegisterRowGrouping(Grouping grouping)
			{
				if (!this.m_rowScopes.ContainsKey(grouping.Name))
				{
					this.m_rowScopes[grouping.Name] = null;
					this.m_rowScopeStack = this.m_rowScopeStack.Add(grouping);
				}
			}

			internal void UnRegisterRowGrouping(string groupName)
			{
				this.m_rowScopes.Remove(groupName);
				this.m_rowScopeStack = this.m_rowScopeStack.Rest;
			}

			internal void RegisterColumnGrouping(Grouping grouping)
			{
				if (!this.m_columnScopes.ContainsKey(grouping.Name))
				{
					this.m_columnScopes[grouping.Name] = null;
					this.m_columnScopeStack = this.m_columnScopeStack.Add(grouping);
				}
			}

			internal void UnRegisterColumnGrouping(string groupName)
			{
				this.m_columnScopes.Remove(groupName);
				this.m_columnScopeStack = this.m_columnScopeStack.Rest;
			}

			private ProcessingErrorCode getErrorCode()
			{
				switch (this.m_containerType)
				{
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				default:
					Global.Tracer.Assert(false);
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				}
			}

			internal bool HasRowColScopeConflict(string textboxSortActionScope, string sortTargetScope, out bool bothGroups)
			{
				bothGroups = false;
				if (textboxSortActionScope != null && sortTargetScope != null)
				{
					if (this.m_rowScopes.ContainsKey(textboxSortActionScope))
					{
						if (this.m_rowScopes.ContainsKey(sortTargetScope))
						{
							bothGroups = true;
							return false;
						}
						if (this.m_columnScopes.ContainsKey(sortTargetScope))
						{
							bothGroups = true;
							return true;
						}
					}
					else if (this.m_columnScopes.ContainsKey(textboxSortActionScope))
					{
						if (this.m_columnScopes.ContainsKey(sortTargetScope))
						{
							bothGroups = true;
							return false;
						}
						if (this.m_rowScopes.ContainsKey(sortTargetScope))
						{
							bothGroups = true;
							return true;
						}
					}
				}
				return false;
			}

			internal bool ContainsScope(string scope)
			{
				return this.ContainsScope(scope, null, false, null);
			}

			internal bool ContainsScope(string scope, ErrorContext errorContext, bool checkConflictingScope, Hashtable groupingScopes)
			{
				Global.Tracer.Assert(null != scope, "(null != scope)");
				if (this.m_rowScopes.ContainsKey(scope))
				{
					if (checkConflictingScope)
					{
						DataRegion dataRegionDef = ((ScopeInfo)groupingScopes[scope]).GroupingScope.Owner.DataRegionDef;
						if (dataRegionDef.ColumnScopeFound)
						{
							errorContext.Register(this.getErrorCode(), Severity.Error, dataRegionDef.ObjectType, this.ContainerName, null);
						}
						dataRegionDef.RowScopeFound = true;
					}
					return true;
				}
				if (this.m_columnScopes.ContainsKey(scope))
				{
					if (checkConflictingScope)
					{
						DataRegion dataRegionDef2 = ((ScopeInfo)groupingScopes[scope]).GroupingScope.Owner.DataRegionDef;
						if (dataRegionDef2.RowScopeFound)
						{
							errorContext.Register(this.getErrorCode(), Severity.Error, dataRegionDef2.ObjectType, this.ContainerName, null);
						}
						dataRegionDef2.ColumnScopeFound = true;
					}
					return true;
				}
				return false;
			}
		}

		private sealed class LookupDestinationCompactionTable : Dictionary<string, int>
		{
			internal LookupDestinationCompactionTable()
			{
			}
		}

		internal sealed class ScopeChainInfo
		{
			private DataRegion m_containingDataRegion;

			private FunctionalList<Grouping> m_rowGroupList;

			private FunctionalList<Grouping> m_columnGroupList;

			internal ScopeChainInfo(DataRegion containingDataRegion, FunctionalList<Grouping> rowGroupList, FunctionalList<Grouping> columnGroupList)
			{
				this.m_containingDataRegion = containingDataRegion;
				this.m_rowGroupList = rowGroupList;
				this.m_columnGroupList = columnGroupList;
			}

			internal Grouping GetInnermostGrouping()
			{
				if (this.m_containingDataRegion.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Column)
				{
					if (!this.m_columnGroupList.IsEmpty() && this.m_columnGroupList.First.Owner.DataRegionDef == this.m_containingDataRegion)
					{
						return this.m_columnGroupList.First;
					}
					if (!this.m_rowGroupList.IsEmpty() && this.m_rowGroupList.First.Owner.DataRegionDef == this.m_containingDataRegion)
					{
						return this.m_rowGroupList.First;
					}
				}
				else
				{
					if (!this.m_rowGroupList.IsEmpty() && this.m_rowGroupList.First.Owner.DataRegionDef == this.m_containingDataRegion)
					{
						return this.m_rowGroupList.First;
					}
					if (!this.m_columnGroupList.IsEmpty() && this.m_columnGroupList.First.Owner.DataRegionDef == this.m_containingDataRegion)
					{
						return this.m_columnGroupList.First;
					}
				}
				ScopeChainInfo scopeChainInfo = this.m_containingDataRegion.ScopeChainInfo;
				if (scopeChainInfo != null)
				{
					return scopeChainInfo.GetInnermostGrouping();
				}
				return null;
			}

			internal GroupingList GetGroupingList()
			{
				GroupingList groupingList = new GroupingList();
				this.AddAllGroups(groupingList);
				groupingList.Reverse();
				return groupingList;
			}

			private void AddAllGroups(GroupingList groups)
			{
				this.AddGroupsForContainingDataRegion(groups);
				ScopeChainInfo scopeChainInfo = this.m_containingDataRegion.ScopeChainInfo;
				if (scopeChainInfo != null)
				{
					scopeChainInfo.AddAllGroups(groups);
				}
			}

			internal GroupingList GetGroupingListForContainingDataRegion()
			{
				GroupingList groupingList = new GroupingList();
				this.AddGroupsForContainingDataRegion(groupingList);
				groupingList.Reverse();
				return groupingList;
			}

			internal GroupingList GetGroupsFromCurrentTablixAxisToGrouping(Grouping fromGroup)
			{
				Grouping innermostGrouping = this.GetInnermostGrouping();
				if (innermostGrouping != null)
				{
					return this.GetGroupsFromCurrentTablixAxisToGrouping(innermostGrouping.Owner.DataRegionDef, innermostGrouping.Owner.IsColumn, fromGroup);
				}
				return null;
			}

			internal GroupingList GetGroupsFromCurrentTablixAxisToGrouping(DataRegion dataRegion, bool isColumn, Grouping fromGroup)
			{
				if (dataRegion == this.m_containingDataRegion)
				{
					GroupingList groupingList = new GroupingList();
					if (isColumn)
					{
						this.AddGroupsToList(this.m_columnGroupList, groupingList, fromGroup);
					}
					else
					{
						this.AddGroupsToList(this.m_rowGroupList, groupingList, fromGroup);
					}
					if (fromGroup == null || fromGroup.Owner.DataRegionDef != this.m_containingDataRegion)
					{
						ScopeChainInfo scopeChainInfo = this.m_containingDataRegion.ScopeChainInfo;
						if (scopeChainInfo != null)
						{
							scopeChainInfo.GetGroupsFromCurrentTablixAxisToGrouping(groupingList, fromGroup);
						}
					}
					if (groupingList.Count > 0)
					{
						groupingList.Reverse();
						return groupingList;
					}
					return null;
				}
				ScopeChainInfo scopeChainInfo2 = this.m_containingDataRegion.ScopeChainInfo;
				if (scopeChainInfo2 != null)
				{
					return scopeChainInfo2.GetGroupsFromCurrentTablixAxisToGrouping(dataRegion, isColumn, fromGroup);
				}
				return null;
			}

			internal void GetGroupsFromCurrentTablixAxisToGrouping(GroupingList groups, Grouping fromGroup)
			{
				bool flag = this.m_containingDataRegion.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Column;
				bool flag2 = false;
				bool flag3 = false;
				if (fromGroup != null && fromGroup.Owner.DataRegionDef == this.m_containingDataRegion)
				{
					flag2 = true;
					if (flag == fromGroup.Owner.IsColumn)
					{
						flag3 = true;
					}
				}
				if (flag)
				{
					if (!flag3)
					{
						this.AddGroupsToList(this.m_rowGroupList, groups, fromGroup);
					}
					this.AddGroupsToList(this.m_columnGroupList, groups, fromGroup);
				}
				else
				{
					if (!flag3)
					{
						this.AddGroupsToList(this.m_columnGroupList, groups, fromGroup);
					}
					this.AddGroupsToList(this.m_rowGroupList, groups, fromGroup);
				}
				if (!flag2)
				{
					ScopeChainInfo scopeChainInfo = this.m_containingDataRegion.ScopeChainInfo;
					if (scopeChainInfo != null)
					{
						scopeChainInfo.GetGroupsFromCurrentTablixAxisToGrouping(groups, fromGroup);
					}
				}
			}

			private void AddGroupsForContainingDataRegion(GroupingList groups)
			{
				if (this.m_containingDataRegion.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Column)
				{
					this.AddGroupsToList(this.m_rowGroupList, groups, null);
					this.AddGroupsToList(this.m_columnGroupList, groups, null);
				}
				else
				{
					this.AddGroupsToList(this.m_columnGroupList, groups, null);
					this.AddGroupsToList(this.m_rowGroupList, groups, null);
				}
			}

			private void AddGroupsToList(FunctionalList<Grouping> groupings, GroupingList containingGroups, Grouping fromGroup)
			{
				while (!groupings.IsEmpty())
				{
					Grouping first = groupings.First;
					groupings = groupings.Rest;
					if (first.Owner.DataRegionDef != this.m_containingDataRegion || first == fromGroup)
					{
						break;
					}
					containingGroups.Add(first);
				}
			}

			internal bool IsSameOrChildScope(string parentScope, string childScope)
			{
				return this.IsSameOrChildScope(parentScope, childScope, false);
			}

			private bool IsSameOrChildScope(string parentScope, string childScope, bool foundChild)
			{
				bool flag = this.m_containingDataRegion.ProcessingInnerGrouping == DataRegion.ProcessingInnerGroupings.Column;
				bool flag2 = false;
				if (flag)
				{
					foundChild = this.IsSameOrChildScope(this.m_rowGroupList, parentScope, childScope, foundChild, out flag2);
					if (!flag2)
					{
						foundChild = this.IsSameOrChildScope(this.m_columnGroupList, parentScope, childScope, foundChild, out flag2);
					}
				}
				else
				{
					foundChild = this.IsSameOrChildScope(this.m_columnGroupList, parentScope, childScope, foundChild, out flag2);
					if (!flag2)
					{
						foundChild = this.IsSameOrChildScope(this.m_rowGroupList, parentScope, childScope, foundChild, out flag2);
					}
				}
				if (!flag2)
				{
					ScopeChainInfo scopeChainInfo = this.m_containingDataRegion.ScopeChainInfo;
					if (scopeChainInfo != null)
					{
						return scopeChainInfo.IsSameOrChildScope(parentScope, childScope, foundChild);
					}
				}
				if (foundChild)
				{
					return flag2;
				}
				return false;
			}

			private bool IsSameOrChildScope(FunctionalList<Grouping> groupings, string parentScope, string childScope, bool foundChild, out bool foundParent)
			{
				foundParent = false;
				while (!groupings.IsEmpty())
				{
					Grouping first = groupings.First;
					groupings = groupings.Rest;
					if (first.Owner.DataRegionDef != this.m_containingDataRegion)
					{
						break;
					}
					if (first.Name == childScope)
					{
						foundChild = true;
					}
					if (first.Name == parentScope)
					{
						foundParent = true;
						break;
					}
				}
				return foundChild;
			}
		}

		private PublishingContextBase m_publishingContext;

		private ICatalogItemContext m_reportContext;

		private AspNetCore.ReportingServices.ReportPublishing.LocationFlags m_location;

		private AspNetCore.ReportingServices.ReportProcessing.ObjectType m_objectType;

		private string m_objectName;

		private string m_tablixName;

		private Dictionary<string, ImageInfo> m_embeddedImages;

		private ErrorContext m_errorContext;

		private Hashtable m_parameters;

		private ArrayList m_dynamicParameters;

		private Hashtable m_dataSetQueryInfo;

		private AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder m_exprHostBuilder;

		private Report m_report;

		private Dictionary<string, Variable> m_variablesInScope;

		private byte[] m_referencableTextboxes;

		private byte[] m_referencableTextboxesInSection;

		private byte[] m_referencableVariables;

		private string m_outerGroupName;

		private string m_currentGroupName;

		private string m_currentDataRegionName;

		private List<RunningValueInfo> m_runningValues;

		private List<RunningValueInfo> m_runningValuesOfAggregates;

		private Hashtable m_groupingScopesForRunningValues;

		private GroupingScopesForTablix m_groupingScopesForRunningValuesInTablix;

		private Hashtable m_dataregionScopesForRunningValues;

		private AxisGroupingScopesForRunningValues m_axisGroupingScopesForRunningValues;

		private Dictionary<string, int> m_groupingExprCountAtScope;

		private bool m_hasFilters;

		private ScopeInfo m_currentScope;

		private ScopeInfo m_outermostDataregionScope;

		private Hashtable m_groupingScopes;

		private Hashtable m_dataregionScopes;

		private Hashtable m_datasetScopes;

		private ScopeTree m_scopeTree;

		private Dictionary<int, IRIFDataScope> m_dataSetsForNonStructuralIdc;

		private Dictionary<int, IRIFDataScope> m_dataSetsForIdcInNestedDR;

		private Dictionary<int, IRIFDataScope> m_dataSetsForIdc;

		private int m_numberOfDataSets;

		private string m_oneDataSetName;

		private FunctionalList<DataSet> m_activeDataSets;

		private FunctionalList<ScopeInfo> m_activeScopeInfos;

		private Hashtable m_fieldNameMap;

		private Dictionary<string, List<DataRegion>> m_dataSetNameToDataRegionsMap;

		private Dictionary<string, LookupDestinationCompactionTable> m_lookupCompactionTable;

		private StringDictionary m_dataSources;

		private Dictionary<string, Pair<ReportItem, int>> m_reportItemsInScope;

		private Dictionary<string, ReportItem> m_reportItemsInSection;

		private Holder<bool> m_handledCellContents;

		private CultureInfo m_reportLanguage;

		private bool m_reportDataElementStyleAttribute;

		private bool m_hasUserSortPeerScopes;

		private bool m_hasUserSorts;

		private Dictionary<string, List<IInScopeEventSource>> m_userSortExpressionScopes;

		private Dictionary<string, List<IInScopeEventSource>> m_userSortEventSources;

		private Hashtable m_peerScopes;

		private int m_lastPeerScopeId;

		private Dictionary<string, ISortFilterScope> m_reportScopes;

		private Hashtable m_reportScopeDatasets;

		private bool m_initializingUserSorts;

		private List<IInScopeEventSource> m_detailSortExpressionScopeEventSources;

		private IList<Pair<double, int>> m_columnHeaderLevelSizeList;

		private IList<Pair<double, int>> m_rowHeaderLevelSizeList;

		private bool m_hasPreviousAggregates;

		private bool m_inAutoSubtotalClone;

		private List<VisibilityToggleInfo> m_visibilityToggleInfos;

		private Dictionary<string, ToggleItemInfo> m_toggleItems;

		private Stack<VisibilityContainmentInfo> m_visibilityContainmentInfos;

		private bool m_isTopLevelCellContents;

		private bool m_isDataRegionScopedCell;

		private bool m_inRecursiveHierarchyRows;

		private bool m_inRecursiveHierarchyColumns;

		private Holder<int> m_memberCellIndex;

		private Dictionary<Hashtable, int> m_indexInCollectionTableForDataRegions;

		private Dictionary<Hashtable, int> m_indexInCollectionTableForSubReports;

		private Dictionary<Hashtable, int> m_indexInCollectionTable;

		private Dictionary<string, List<ExpressionInfo>> m_naturalGroupExpressionsByDataSetName;

		private Dictionary<string, List<ExpressionInfo>> m_naturalSortExpressionsByDataSetName;

		private double m_currentAbsoluteTop;

		private double m_currentAbsoluteLeft;

		internal ICatalogItemContext ReportContext
		{
			get
			{
				return this.m_reportContext;
			}
		}

		internal AspNetCore.ReportingServices.ReportPublishing.LocationFlags Location
		{
			get
			{
				return this.m_location;
			}
			set
			{
				this.m_location = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return this.m_objectType;
			}
			set
			{
				this.m_objectType = value;
			}
		}

		internal string ObjectName
		{
			get
			{
				return this.m_objectName;
			}
			set
			{
				this.m_objectName = value;
			}
		}

		internal bool IsTopLevelCellContents
		{
			get
			{
				return this.m_isTopLevelCellContents;
			}
			set
			{
				this.m_isTopLevelCellContents = value;
			}
		}

		internal bool HasUserSorts
		{
			get
			{
				return this.m_hasUserSorts;
			}
		}

		internal bool InitializingUserSorts
		{
			get
			{
				return this.m_initializingUserSorts;
			}
			set
			{
				this.m_initializingUserSorts = value;
			}
		}

		internal bool IsDataRegionCellScope
		{
			get
			{
				if (this.m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicRow)
				{
					return this.m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicColumn;
				}
				return false;
			}
		}

		internal bool CellHasDynamicRowsAndColumns
		{
			get
			{
				if (this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
				{
					return this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null;
				}
				return false;
			}
		}

		internal bool IsDataRegionScopedCell
		{
			get
			{
				return this.m_isDataRegionScopedCell;
			}
			set
			{
				this.m_isDataRegionScopedCell = value;
			}
		}

		internal bool ReportDataElementStyleAttribute
		{
			get
			{
				return this.m_reportDataElementStyleAttribute;
			}
			set
			{
				this.m_reportDataElementStyleAttribute = value;
			}
		}

		internal string TablixName
		{
			get
			{
				return this.m_tablixName;
			}
			set
			{
				this.m_tablixName = value;
			}
		}

		internal Dictionary<string, ImageInfo> EmbeddedImages
		{
			get
			{
				return this.m_embeddedImages;
			}
		}

		internal ErrorContext ErrorContext
		{
			get
			{
				return this.m_errorContext;
			}
		}

		internal AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder ExprHostBuilder
		{
			get
			{
				return this.m_exprHostBuilder;
			}
		}

		internal bool MergeOnePass
		{
			get
			{
				return this.m_report.MergeOnePass;
			}
		}

		internal CultureInfo ReportLanguage
		{
			get
			{
				return this.m_reportLanguage;
			}
		}

		internal IList<Pair<double, int>> ColumnHeaderLevelSizeList
		{
			get
			{
				return this.m_columnHeaderLevelSizeList;
			}
			set
			{
				this.m_columnHeaderLevelSizeList = value;
			}
		}

		internal IList<Pair<double, int>> RowHeaderLevelSizeList
		{
			get
			{
				return this.m_rowHeaderLevelSizeList;
			}
			set
			{
				this.m_rowHeaderLevelSizeList = value;
			}
		}

		internal bool InAutoSubtotalClone
		{
			get
			{
				return this.m_inAutoSubtotalClone;
			}
			set
			{
				this.m_inAutoSubtotalClone = value;
			}
		}

		internal int MemberCellIndex
		{
			get
			{
				return this.m_memberCellIndex.Value;
			}
			set
			{
				this.m_memberCellIndex.Value = value;
			}
		}

		internal double CurrentAbsoluteTop
		{
			get
			{
				return this.m_currentAbsoluteTop;
			}
		}

		internal double CurrentAbsoluteLeft
		{
			get
			{
				return this.m_currentAbsoluteLeft;
			}
		}

		internal bool HasPreviousAggregates
		{
			get
			{
				return this.m_hasPreviousAggregates;
			}
		}

		internal Dictionary<string, int> GroupingExprCountAtScope
		{
			get
			{
				return this.m_groupingExprCountAtScope;
			}
		}

		internal bool IsRunningValueDirectionColumn
		{
			get
			{
				if (this.m_groupingScopesForRunningValuesInTablix != null)
				{
					return this.m_groupingScopesForRunningValuesInTablix.IsRunningValueDirectionColumn;
				}
				return true;
			}
		}

		internal bool HasLookups
		{
			get
			{
				return this.m_lookupCompactionTable.Count > 0;
			}
		}

		internal Report Report
		{
			get
			{
				return this.m_report;
			}
		}

		internal ScopeTree ScopeTree
		{
			get
			{
				return this.m_scopeTree;
			}
		}

		internal bool HandledCellContents
		{
			get
			{
				return this.m_handledCellContents.Value;
			}
			set
			{
				this.m_handledCellContents.Value = value;
			}
		}

		internal bool InRecursiveHierarchyColumns
		{
			get
			{
				return this.m_inRecursiveHierarchyColumns;
			}
			set
			{
				this.m_inRecursiveHierarchyColumns = value;
			}
		}

		internal bool InRecursiveHierarchyRows
		{
			get
			{
				return this.m_inRecursiveHierarchyRows;
			}
			set
			{
				this.m_inRecursiveHierarchyRows = value;
			}
		}

		internal PublishingContextBase PublishingContext
		{
			get
			{
				return this.m_publishingContext;
			}
		}

		internal InitializationContext(ICatalogItemContext reportContext, List<DataSet> datasets, ErrorContext errorContext, AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder, Report artificialReportContainerForCodeGeneration, Dictionary<string, ISortFilterScope> reportScopes, PublishingContextBase publishingContext)
		{
			this = new InitializationContext(reportContext, false, null, datasets, null, null, errorContext, exprHostBuilder, artificialReportContainerForCodeGeneration, null, reportScopes, false, false, 0, 0, 0, publishingContext, new ScopeTree(), true);
		}

		internal InitializationContext(ICatalogItemContext reportContext, bool hasFilters, StringDictionary dataSources, List<DataSet> dataSets, ArrayList dynamicParameters, Hashtable dataSetQueryInfo, ErrorContext errorContext, AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder, Report report, CultureInfo reportLanguage, Dictionary<string, ISortFilterScope> reportScopes, bool hasUserSortPeerScopes, bool hasUserSort, int dataRegionCount, int textboxCount, int variableCount, PublishingContextBase publishingContext, ScopeTree scopeTree, bool isSharedDataSetContext)
		{
			Global.Tracer.Assert(null != errorContext, "(null != errorContext)");
			Global.Tracer.Assert(null != reportContext, "(null != reportContext)");
			this.m_publishingContext = publishingContext;
			this.m_reportContext = reportContext;
			this.m_location = AspNetCore.ReportingServices.ReportPublishing.LocationFlags.None;
			this.m_objectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.Report;
			this.m_objectName = null;
			this.m_tablixName = null;
			this.m_embeddedImages = report.EmbeddedImages;
			this.m_errorContext = errorContext;
			this.m_parameters = null;
			this.m_dynamicParameters = dynamicParameters;
			this.m_dataSetQueryInfo = dataSetQueryInfo;
			this.m_exprHostBuilder = exprHostBuilder;
			this.m_dataSources = dataSources;
			this.m_rowHeaderLevelSizeList = null;
			this.m_columnHeaderLevelSizeList = null;
			this.m_report = report;
			this.m_reportLanguage = reportLanguage;
			this.m_outerGroupName = null;
			this.m_currentGroupName = null;
			this.m_currentDataRegionName = null;
			this.m_runningValues = null;
			this.m_runningValuesOfAggregates = null;
			this.m_groupingScopesForRunningValues = new Hashtable();
			this.m_groupingScopesForRunningValuesInTablix = null;
			this.m_dataregionScopesForRunningValues = new Hashtable();
			this.m_scopeTree = scopeTree;
			this.m_hasFilters = hasFilters;
			this.m_currentScope = null;
			this.m_outermostDataregionScope = null;
			this.m_groupingScopes = new Hashtable();
			this.m_dataregionScopes = new Hashtable();
			this.m_datasetScopes = new Hashtable();
			this.m_numberOfDataSets = 0;
			this.m_oneDataSetName = null;
			this.m_activeDataSets = FunctionalList<DataSet>.Empty;
			this.m_activeScopeInfos = FunctionalList<ScopeInfo>.Empty;
			this.m_fieldNameMap = new Hashtable();
			this.m_dataSetNameToDataRegionsMap = new Dictionary<string, List<DataRegion>>();
			this.m_isTopLevelCellContents = false;
			this.m_isDataRegionScopedCell = false;
			if (dataSets != null)
			{
				this.m_numberOfDataSets = dataSets.Count;
				this.m_oneDataSetName = ((1 == dataSets.Count) ? dataSets[0].Name : null);
				for (int i = 0; i < dataSets.Count; i++)
				{
					DataSet dataSet = dataSets[i];
					bool flag = this.m_datasetScopes.ContainsKey(dataSets[i].Name);
					this.m_datasetScopes[dataSets[i].Name] = new ScopeInfo(true, dataSets[i].Aggregates, dataSets[i].PostSortAggregates, dataSets[i], flag);
					Hashtable hashtable = new Hashtable();
					if (dataSet.Fields != null)
					{
						for (int j = 0; j < dataSet.Fields.Count; j++)
						{
							hashtable[dataSet.Fields[j].Name] = j;
						}
					}
					this.m_fieldNameMap[dataSet.Name] = hashtable;
					if (!flag)
					{
						this.m_dataSetNameToDataRegionsMap.Add(dataSet.Name, dataSet.DataRegions);
					}
				}
			}
			if (report != null && report.Parameters != null)
			{
				this.m_parameters = new Hashtable();
				for (int k = 0; k < report.Parameters.Count; k++)
				{
					ParameterDef parameterDef = report.Parameters[k];
					if (parameterDef != null && !this.m_parameters.ContainsKey(parameterDef.Name))
					{
						this.m_parameters.Add(parameterDef.Name, parameterDef);
					}
				}
			}
			this.m_reportItemsInScope = new Dictionary<string, Pair<ReportItem, int>>();
			this.m_reportItemsInSection = new Dictionary<string, ReportItem>();
			this.m_variablesInScope = new Dictionary<string, Variable>();
			this.m_referencableVariables = new byte[(variableCount >> 3) + 1];
			this.m_referencableTextboxes = new byte[(textboxCount >> 3) + 1];
			this.m_referencableTextboxesInSection = new byte[(textboxCount >> 3) + 1];
			this.m_reportDataElementStyleAttribute = true;
			this.m_hasUserSorts = hasUserSort;
			this.m_hasUserSortPeerScopes = hasUserSortPeerScopes;
			this.m_lastPeerScopeId = 0;
			this.m_reportScopes = reportScopes;
			this.m_initializingUserSorts = false;
			if (hasUserSort || this.m_report.HasSubReports)
			{
				this.m_userSortExpressionScopes = new Dictionary<string, List<IInScopeEventSource>>();
				this.m_userSortEventSources = new Dictionary<string, List<IInScopeEventSource>>();
				this.m_peerScopes = (hasUserSortPeerScopes ? new Hashtable() : null);
				this.m_reportScopeDatasets = new Hashtable();
				this.m_detailSortExpressionScopeEventSources = new List<IInScopeEventSource>();
			}
			else
			{
				this.m_userSortExpressionScopes = null;
				this.m_userSortEventSources = null;
				this.m_peerScopes = null;
				this.m_reportScopeDatasets = null;
				this.m_detailSortExpressionScopeEventSources = null;
			}
			this.m_inAutoSubtotalClone = false;
			this.m_visibilityToggleInfos = new List<VisibilityToggleInfo>();
			this.m_toggleItems = new Dictionary<string, ToggleItemInfo>();
			this.m_visibilityContainmentInfos = new Stack<VisibilityContainmentInfo>();
			this.m_memberCellIndex = null;
			this.m_indexInCollectionTableForDataRegions = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
			this.m_indexInCollectionTableForSubReports = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
			this.m_indexInCollectionTable = null;
			this.m_currentAbsoluteTop = 0.0;
			this.m_currentAbsoluteLeft = 0.0;
			this.m_hasPreviousAggregates = report.HasPreviousAggregates;
			this.m_axisGroupingScopesForRunningValues = null;
			this.m_groupingExprCountAtScope = new Dictionary<string, int>();
			this.m_lookupCompactionTable = new Dictionary<string, LookupDestinationCompactionTable>();
			this.m_handledCellContents = new Holder<bool>();
			this.m_handledCellContents.Value = false;
			this.m_inRecursiveHierarchyColumns = false;
			this.m_inRecursiveHierarchyRows = false;
			this.m_naturalGroupExpressionsByDataSetName = new Dictionary<string, List<ExpressionInfo>>();
			this.m_naturalSortExpressionsByDataSetName = new Dictionary<string, List<ExpressionInfo>>();
			this.m_dataSetsForIdcInNestedDR = new Dictionary<int, IRIFDataScope>();
			this.m_dataSetsForIdc = new Dictionary<int, IRIFDataScope>();
			this.m_dataSetsForNonStructuralIdc = new Dictionary<int, IRIFDataScope>();
		}

		internal void RSDRegisterDataSetParameters(DataSetCore sharedDataset)
		{
			this.m_parameters = new Hashtable();
			if (sharedDataset != null && sharedDataset.Query != null && sharedDataset.Query.Parameters != null)
			{
				int count = sharedDataset.Query.Parameters.Count;
				for (int i = 0; i < count; i++)
				{
					DataSetParameterValue dataSetParameterValue = sharedDataset.Query.Parameters[i] as DataSetParameterValue;
					if (dataSetParameterValue != null && dataSetParameterValue.UniqueName != null && !this.m_parameters.ContainsKey(dataSetParameterValue.UniqueName))
					{
						this.m_parameters.Add(dataSetParameterValue.UniqueName, null);
					}
				}
			}
		}

		internal void ValidateScopeRulesForNaturalGroup(ReportHierarchyNode member)
		{
			if (member.Grouping != null && member.Grouping.NaturalGroup)
			{
				ErrorContext errorContext = this.m_errorContext;
				List<ExpressionInfo> groupExpressions = new List<ExpressionInfo>();
				ScopeTree.DirectedScopeTreeVisitor validator = delegate(IRIFDataScope scope)
				{
					ReportHierarchyNode reportHierarchyNode = scope as ReportHierarchyNode;
					if (reportHierarchyNode != null && DataSet.AreEqualById(member.DataScopeInfo.DataSet, reportHierarchyNode.DataScopeInfo.DataSet))
					{
						if (!reportHierarchyNode.Grouping.NaturalGroup)
						{
							errorContext.Register(ProcessingErrorCode.rsInvalidGroupingContainerNotNaturalGroup, Severity.Warning, member.DataRegionDef.ObjectType, member.DataRegionDef.Name, "Grouping", member.Grouping.Name.MarkAsModelInfo(), reportHierarchyNode.Grouping.Name.MarkAsModelInfo());
							member.Grouping.NaturalGroup = false;
							return false;
						}
						InitializationContext.AppendExpressions(groupExpressions, reportHierarchyNode.Grouping.GroupExpressions);
					}
					return true;
				};
				this.ValidateNaturalGroupOrSortRulesForScopeTree(this.m_naturalGroupExpressionsByDataSetName, member, groupExpressions, validator, ProcessingErrorCode.rsConflictingNaturalGroupRequirements, "NaturalGroup");
			}
		}

		internal void ValidateScopeRulesForNaturalSort(ReportHierarchyNode member)
		{
			if (member.Grouping != null && member.Sorting != null && member.Sorting.NaturalSort)
			{
				ErrorContext errorContext = this.m_errorContext;
				List<ExpressionInfo> sortExpressions = new List<ExpressionInfo>();
				ScopeTree.DirectedScopeTreeVisitor validator = delegate(IRIFDataScope scope)
				{
					Sorting sorting = null;
					string plainString = null;
					ReportHierarchyNode reportHierarchyNode = scope as ReportHierarchyNode;
					DataRegion dataRegion = scope as DataRegion;
					if (reportHierarchyNode != null)
					{
						sorting = reportHierarchyNode.Sorting;
						plainString = reportHierarchyNode.Grouping.Name;
					}
					else if (dataRegion != null)
					{
						sorting = dataRegion.Sorting;
						plainString = dataRegion.Name;
					}
					if (sorting != null && member.DataScopeInfo.DataSet.ID == scope.DataScopeInfo.DataSet.ID)
					{
						if (!sorting.NaturalSort)
						{
							errorContext.Register(ProcessingErrorCode.rsInvalidSortingContainerNotNaturalSort, Severity.Warning, member.DataRegionDef.ObjectType, member.DataRegionDef.Name, "SortExpressions", member.Grouping.Name.MarkAsModelInfo(), plainString.MarkAsPrivate());
							member.Sorting.NaturalSort = false;
							return false;
						}
						InitializationContext.AppendExpressions(sortExpressions, reportHierarchyNode.Sorting.SortExpressions);
					}
					return true;
				};
				this.ValidateNaturalGroupOrSortRulesForScopeTree(this.m_naturalSortExpressionsByDataSetName, member, sortExpressions, validator, ProcessingErrorCode.rsConflictingNaturalSortRequirements, "NaturalSort");
			}
		}

		internal void ValidateScopeRulesForIdcNaturalJoin(IRIFDataScope startScope)
		{
			ErrorContext errorContext = this.m_errorContext;
			ScopeTree.DirectedScopeTreeVisitor visitor = delegate(IRIFDataScope scope)
			{
				if (scope.DataScopeInfo != null && scope.DataScopeInfo.JoinInfo != null)
				{
					scope.DataScopeInfo.JoinInfo.CheckContainerJoinForNaturalJoin(startScope, errorContext, scope);
				}
				ReportHierarchyNode reportHierarchyNode = scope as ReportHierarchyNode;
				if (reportHierarchyNode != null && !reportHierarchyNode.Grouping.NaturalGroup)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidRelationshipGroupingContainerNotNaturalGroup, Severity.Error, startScope.DataScopeObjectType, startScope.Name, "Grouping", scope.Name);
					return false;
				}
				return true;
			};
			this.m_scopeTree.Traverse(visitor, startScope);
		}

		private void ValidateNaturalGroupOrSortRulesForScopeTree(Dictionary<string, List<ExpressionInfo>> expressionsByDataSetName, ReportHierarchyNode member, List<ExpressionInfo> expressions, ScopeTree.DirectedScopeTreeVisitor validator, ProcessingErrorCode conflictingNaturalRequirementErrorCode, string naturalElementName)
		{
			if (this.m_scopeTree.Traverse(validator, member))
			{
				string currentDataSetName = this.GetCurrentDataSetName();
				if (currentDataSetName != null)
				{
					List<ExpressionInfo> list = default(List<ExpressionInfo>);
					if (expressionsByDataSetName.TryGetValue(currentDataSetName, out list))
					{
						if (!ListUtils.AreSameOrSuffix(expressions, list, RdlExpressionComparer.Instance))
						{
							this.m_errorContext.Register(conflictingNaturalRequirementErrorCode, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, currentDataSetName, naturalElementName);
						}
						if (list.Count > expressions.Count)
						{
							expressions = list;
						}
					}
					expressionsByDataSetName[currentDataSetName] = expressions;
				}
			}
		}

		private static void AppendExpressions(List<ExpressionInfo> allExpressions, List<ExpressionInfo> localExpressions)
		{
			if (localExpressions != null)
			{
				allExpressions.AddRange(localExpressions);
			}
		}

		internal void EnsureDataSetUsedOnceForIdcUnderTopDataRegion(DataSet dataSet, IRIFDataScope currentScope)
		{
			if (currentScope.DataScopeInfo.NeedsIDC && !this.IsErrorForDuplicateIdcDataSet(this.m_dataSetsForIdcInNestedDR, dataSet, currentScope, ProcessingErrorCode.rsInvalidRelationshipDataSetUsedMoreThanOnce))
			{
				this.IsErrorForDuplicateIdcDataSet(this.m_dataSetsForIdc, dataSet, currentScope, ProcessingErrorCode.rsInvalidRelationshipDataSet);
			}
		}

		private bool IsErrorForDuplicateIdcDataSet(Dictionary<int, IRIFDataScope> mappingDataSetIdToFirstIdcScope, DataSet dataSet, IRIFDataScope currentScope, ProcessingErrorCode errorCode)
		{
			IRIFDataScope iRIFDataScope = default(IRIFDataScope);
			if (mappingDataSetIdToFirstIdcScope.TryGetValue(dataSet.ID, out iRIFDataScope))
			{
				if (!this.IsOtherParentOfCurrentIntersection(iRIFDataScope, currentScope) && !this.IsOtherSameIntersectionScope(iRIFDataScope, currentScope))
				{
					this.m_errorContext.Register(errorCode, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSet, dataSet.Name.MarkAsPrivate(), currentScope.DataScopeObjectType.ToString(), currentScope.Name, iRIFDataScope.DataScopeObjectType.ToString(), iRIFDataScope.Name);
				}
				return true;
			}
			mappingDataSetIdToFirstIdcScope.Add(dataSet.ID, currentScope);
			return false;
		}

		private bool IsOtherParentOfCurrentIntersection(IRIFDataScope otherScope, IRIFDataScope currentScope)
		{
			if (this.m_scopeTree.IsIntersectionScope(currentScope))
			{
				if (!this.m_scopeTree.IsSameOrParentScope(otherScope, this.m_scopeTree.GetParentRowScopeForIntersection(currentScope)))
				{
					return this.m_scopeTree.IsSameOrParentScope(otherScope, this.m_scopeTree.GetParentColumnScopeForIntersection(currentScope));
				}
				return true;
			}
			return false;
		}

		private bool IsOtherSameIntersectionScope(IRIFDataScope otherScope, IRIFDataScope currentScope)
		{
			if (this.m_scopeTree.IsIntersectionScope(currentScope) && this.m_scopeTree.IsIntersectionScope(otherScope) && ScopeTree.SameScope(this.m_scopeTree.GetParentRowScopeForIntersection(currentScope), this.m_scopeTree.GetParentRowScopeForIntersection(otherScope)))
			{
				return ScopeTree.SameScope(this.m_scopeTree.GetParentColumnScopeForIntersection(currentScope), this.m_scopeTree.GetParentColumnScopeForIntersection(otherScope));
			}
			return false;
		}

		private string GetCurrentDataSetName()
		{
			DataSet first = this.m_activeDataSets.First;
			if (first != null)
			{
				return first.Name;
			}
			return null;
		}

		private int GetCurrentDataSetIndex()
		{
			DataSet first = this.m_activeDataSets.First;
			if (first != null)
			{
				return first.IndexInCollection;
			}
			return -1;
		}

		private void RegisterDataSetScope(DataSet dataSet, List<DataAggregateInfo> scopeAggregates, List<DataAggregateInfo> scopePostSortAggregates, int datasetIndexInCollection)
		{
			Global.Tracer.Assert(null != dataSet);
			Global.Tracer.Assert(null != scopeAggregates);
			Global.Tracer.Assert(null != scopePostSortAggregates);
			this.m_currentScope = new ScopeInfo(true, scopeAggregates, scopePostSortAggregates, dataSet);
			if (this.m_initializingUserSorts && !this.m_reportScopeDatasets.ContainsKey(dataSet.Name))
			{
				this.m_reportScopeDatasets.Add(dataSet.Name, dataSet);
			}
		}

		private void UnRegisterDataSetScope(string scopeName)
		{
			Global.Tracer.Assert(null != scopeName);
			this.m_currentScope = null;
		}

		private void RegisterDataRegionScope(DataRegion dataRegion)
		{
			Global.Tracer.Assert(null != dataRegion.Name);
			Global.Tracer.Assert(null != dataRegion.Aggregates);
			Global.Tracer.Assert(null != dataRegion.PostSortAggregates);
			this.m_currentDataRegionName = dataRegion.Name;
			this.m_dataregionScopesForRunningValues[dataRegion.Name] = this.m_currentGroupName;
			if (!this.m_initializingUserSorts)
			{
				this.SetIndexInCollection(dataRegion);
				this.m_memberCellIndex = new Holder<int>();
				this.m_indexInCollectionTableForDataRegions = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
				this.m_indexInCollectionTableForSubReports = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
				this.m_indexInCollectionTable = new Dictionary<Hashtable, int>(HashtableKeyComparer.Instance);
			}
			else
			{
				this.m_reportScopeDatasets.Add(dataRegion.Name, this.GetDataSet());
			}
			this.m_currentScope = this.CreateScopeInfo(dataRegion, this.m_currentScope == null || this.m_currentScope.AllowCustomAggregates);
			if (this.m_axisGroupingScopesForRunningValues != null)
			{
				this.m_axisGroupingScopesForRunningValues = new AxisGroupingScopesForRunningValues(this.m_axisGroupingScopesForRunningValues);
			}
			else
			{
				this.m_axisGroupingScopesForRunningValues = new AxisGroupingScopesForRunningValues();
			}
			if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				this.m_outermostDataregionScope = this.m_currentScope;
			}
			this.m_dataregionScopes[dataRegion.Name] = this.m_currentScope;
		}

		private ScopeInfo CreateScopeInfo(DataRegion dataRegion, bool allowCustomAggregates)
		{
			return new ScopeInfo(allowCustomAggregates, dataRegion.Aggregates, dataRegion.PostSortAggregates, dataRegion);
		}

		private void UnRegisterDataRegionScope(DataRegion dataRegion)
		{
			string name = dataRegion.Name;
			Global.Tracer.Assert(null != name);
			this.m_currentDataRegionName = null;
			this.m_dataregionScopesForRunningValues.Remove(name);
			this.m_currentScope = null;
			if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				this.m_outermostDataregionScope = null;
			}
			this.m_dataregionScopes.Remove(name);
			this.m_axisGroupingScopesForRunningValues = null;
		}

		internal void RegisterGroupingScope(ReportHierarchyNode member)
		{
			this.RegisterGroupingScope(member, false);
		}

		private void RegisterGroupingScope(ReportHierarchyNode member, bool forTablixCell)
		{
			if (forTablixCell)
			{
				Global.Tracer.Assert(null != this.m_groupingScopesForRunningValuesInTablix, "(null != m_groupingScopesForRunningValuesInTablix)");
				Global.Tracer.Assert(null != this.m_groupingScopesForRunningValues, "(null != m_groupingScopesForRunningValues)");
			}
			DataSet dataSet = member.DataScopeInfo.DataSet;
			DataSet first = this.m_activeDataSets.First;
			if (first != dataSet)
			{
				this.RegisterDataSet(dataSet);
			}
			Grouping grouping = member.Grouping;
			this.m_outerGroupName = this.m_currentGroupName;
			this.m_currentGroupName = grouping.Name;
			string name = grouping.Name;
			this.m_groupingScopesForRunningValues[name] = null;
			if (member.IsColumn)
			{
				if (forTablixCell)
				{
					this.m_groupingScopesForRunningValuesInTablix.RegisterColumnGrouping(grouping);
				}
				this.m_axisGroupingScopesForRunningValues.RegisterColumnGrouping(grouping);
			}
			else
			{
				if (forTablixCell)
				{
					this.m_groupingScopesForRunningValuesInTablix.RegisterRowGrouping(grouping);
				}
				this.m_axisGroupingScopesForRunningValues.RegisterRowGrouping(grouping);
			}
			this.m_currentScope = this.CreateScopeInfo(member);
			this.m_groupingScopes[name] = this.m_currentScope;
			if (forTablixCell && this.m_initializingUserSorts && !this.m_reportScopeDatasets.ContainsKey(name))
			{
				this.m_reportScopeDatasets.Add(name, this.GetDataSet());
			}
			this.RegisterRunningValues(member.RunningValues, member.DataScopeInfo.RunningValuesOfAggregates);
		}

		private ScopeInfo CreateScopeInfo(ReportHierarchyNode member)
		{
			return this.CreateScopeInfo(member, (this.m_currentScope == null) ? member.Grouping.SimpleGroupExpressions : (member.Grouping.SimpleGroupExpressions && this.m_currentScope.AllowCustomAggregates));
		}

		private ScopeInfo CreateScopeInfo(ReportHierarchyNode member, bool allowCustomAggregates)
		{
			return new ScopeInfo(allowCustomAggregates, member.Grouping.Aggregates, member.Grouping.PostSortAggregates, member.Grouping.RecursiveAggregates, member.Grouping, member);
		}

		internal void UnRegisterGroupingScope(ReportHierarchyNode member)
		{
			this.UnRegisterGroupingScope(member, false);
		}

		private void UnRegisterGroupingScope(ReportHierarchyNode member, bool forTablixCell)
		{
			if (forTablixCell)
			{
				Global.Tracer.Assert(null != this.m_groupingScopesForRunningValuesInTablix, "(null != m_groupingScopesForRunningValuesInTablix)");
				Global.Tracer.Assert(null != this.m_groupingScopesForRunningValues, "(null != m_groupingScopesForRunningValues)");
			}
			Grouping grouping = member.Grouping;
			string name = grouping.Name;
			this.m_outerGroupName = null;
			this.m_currentGroupName = null;
			this.m_groupingScopesForRunningValues.Remove(name);
			if (member.IsColumn)
			{
				if (forTablixCell)
				{
					this.m_groupingScopesForRunningValuesInTablix.UnRegisterColumnGrouping(name);
				}
				this.m_axisGroupingScopesForRunningValues.UnregisterColumnGrouping(grouping);
			}
			else
			{
				if (forTablixCell)
				{
					this.m_groupingScopesForRunningValuesInTablix.UnRegisterRowGrouping(name);
				}
				this.m_axisGroupingScopesForRunningValues.UnregisterRowGrouping(grouping);
			}
			this.m_currentScope = null;
			this.m_groupingScopes.Remove(name);
			this.UnRegisterRunningValues(member.RunningValues, member.DataScopeInfo.RunningValuesOfAggregates);
			DataSet dataSet = member.DataScopeInfo.DataSet;
			this.UnRegisterDataSet(dataSet);
		}

		internal void ValidateHideDuplicateScope(string hideDuplicateScope, ReportItem reportItem)
		{
			if (hideDuplicateScope != null)
			{
				bool flag = true;
				ScopeInfo scopeInfo = null;
				if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 && hideDuplicateScope.Equals(this.m_currentGroupName))
				{
					flag = false;
				}
				else if (this.m_groupingScopes.Contains(hideDuplicateScope))
				{
					scopeInfo = (ScopeInfo)this.m_groupingScopes[hideDuplicateScope];
				}
				else if (!this.m_datasetScopes.ContainsKey(hideDuplicateScope))
				{
					flag = false;
				}
				if (flag)
				{
					if (scopeInfo != null)
					{
						Global.Tracer.Assert(null != scopeInfo.GroupingScope, "(null != scope.GroupingScope)");
						scopeInfo.GroupingScope.AddReportItemWithHideDuplicates(reportItem);
					}
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidHideDuplicateScope, Severity.Error, this.m_objectType, this.m_objectName, "HideDuplicates", hideDuplicateScope);
				}
			}
		}

		internal void RegisterGroupingScopeForDataRegionCell(ReportHierarchyNode member)
		{
			this.RegisterGroupingScope(member, true);
		}

		internal void UnRegisterGroupingScopeForDataRegionCell(ReportHierarchyNode member)
		{
			this.UnRegisterGroupingScope(member, true);
		}

		internal void RegisterIndividualCellScope(Cell cell)
		{
			DataSet dataSet = cell.DataScopeInfo.DataSet;
			this.RegisterDataSet(dataSet);
			this.m_currentScope = new ScopeInfo(this.m_currentScope == null || this.m_currentScope.AllowCustomAggregates, cell.Aggregates, cell.PostSortAggregates, cell);
			this.m_runningValues = cell.RunningValues;
			this.m_runningValuesOfAggregates = cell.DataScopeInfo.RunningValuesOfAggregates;
			IRIFDataScope canonicalCellScope = this.m_scopeTree.GetCanonicalCellScope(cell);
			if (canonicalCellScope != null && cell != canonicalCellScope)
			{
				cell.DataScopeInfo.ScopeID = canonicalCellScope.DataScopeInfo.ScopeID;
			}
		}

		internal void UnRegisterIndividualCellScope(Cell cell)
		{
			this.UnRegisterCell(cell);
		}

		private void UnRegisterCell(Cell cell)
		{
			this.UnRegisterNonScopeCell(cell);
		}

		internal void RegisterNonScopeCell(Cell cell)
		{
			DataSet dataSet = cell.DataScopeInfo.DataSet;
			this.RegisterDataSet(dataSet);
		}

		internal void UnRegisterNonScopeCell(Cell cell)
		{
			DataSet dataSet = cell.DataScopeInfo.DataSet;
			this.UnRegisterDataSet(dataSet);
		}

		internal void FoundAtomicScope(IRIFDataScope scope)
		{
			this.MarkChildScopesWithAtomicParent(scope);
		}

		private void MarkChildScopesWithAtomicParent(IRIFDataScope scope)
		{
			foreach (IRIFDataScope childScope in this.m_scopeTree.GetChildScopes(scope))
			{
				if (childScope.DataScopeInfo.IsDecomposable)
				{
					childScope.DataScopeInfo.IsDecomposable = false;
					this.MarkChildScopesWithAtomicParent(childScope);
				}
			}
		}

		internal bool HasMultiplePeerChildScopes(IRIFDataScope scope)
		{
			int[] dataSetGroupBindingCounts = new int[this.m_report.MappingDataSetIndexToDataSet.Count];
			foreach (IRIFDataScope childScope in this.m_scopeTree.GetChildScopes(scope))
			{
				if (this.IsOrHasSecondGroupBoundToDataSet(childScope, dataSetGroupBindingCounts))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsOrHasSecondGroupBoundToDataSet(IRIFDataScope scope, int[] dataSetGroupBindingCounts)
		{
			if (scope is ReportHierarchyNode)
			{
				DataSet dataSet = scope.DataScopeInfo.DataSet;
				if (dataSet == null)
				{
					return false;
				}
				int num = dataSetGroupBindingCounts[dataSet.IndexInCollection];
				num++;
				dataSetGroupBindingCounts[dataSet.IndexInCollection] = num;
				return num >= 2;
			}
			foreach (IRIFDataScope childScope in this.m_scopeTree.GetChildScopes(scope))
			{
				if (this.IsOrHasSecondGroupBoundToDataSet(childScope, dataSetGroupBindingCounts))
				{
					return true;
				}
			}
			return false;
		}

		internal bool EvaluateAtomicityCondition(bool isAtomic, IRIFDataScope scope, AtomicityReason reason)
		{
			if (isAtomic && this.m_publishingContext.TraceAtomicScopes)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Report {0} contains a scope '{1}' which uses or contains {2}.  This may prevent optimizations from being applied to parent or child scopes.", this.m_reportContext.ItemPathAsString.MarkAsPrivate(), this.m_scopeTree.GetScopeName(scope), reason.ToString());
			}
			return isAtomic;
		}

		internal bool IsAncestor(ReportHierarchyNode child, string parentName)
		{
			ISortFilterScope sortFilterScope = default(ISortFilterScope);
			if (!this.m_reportScopes.TryGetValue(parentName, out sortFilterScope))
			{
				return false;
			}
			IRIFDataScope iRIFDataScope = (!(sortFilterScope is Grouping)) ? ((IRIFDataScope)sortFilterScope) : ((Grouping)sortFilterScope).Owner;
			if (!object.ReferenceEquals(iRIFDataScope, child))
			{
				return this.m_scopeTree.IsSameOrParentScope(iRIFDataScope, child);
			}
			return false;
		}

		internal DataRegion RegisterDataRegionCellScope(DataRegion dataRegion, bool forceRows, List<DataAggregateInfo> scopeAggregates, List<DataAggregateInfo> scopePostSortAggregates)
		{
			Global.Tracer.Assert(null != scopeAggregates, "(null != scopeAggregates)");
			Global.Tracer.Assert(null != scopePostSortAggregates, "(null != scopePostSortAggregates)");
			DataRegion result = null;
			if (this.m_groupingScopesForRunningValuesInTablix == null)
			{
				this.m_groupingScopesForRunningValuesInTablix = new GroupingScopesForTablix(forceRows, this.m_objectType, dataRegion);
			}
			else
			{
				dataRegion.ScopeChainInfo = this.m_groupingScopesForRunningValuesInTablix.GetScopeChainInfo();
				result = this.m_groupingScopesForRunningValuesInTablix.SetContainerScope(dataRegion);
			}
			this.m_currentScope = new ScopeInfo(this.m_currentScope == null || this.m_currentScope.AllowCustomAggregates, scopeAggregates, scopePostSortAggregates, dataRegion);
			return result;
		}

		internal void UnRegisterTablixCellScope(DataRegion dataRegion)
		{
			Global.Tracer.Assert(null != this.m_groupingScopesForRunningValuesInTablix, "(null != m_groupingScopesForRunningValuesInTablix)");
			if (dataRegion == null)
			{
				this.m_groupingScopesForRunningValuesInTablix = null;
			}
			else
			{
				this.m_groupingScopesForRunningValuesInTablix.SetContainerScope(dataRegion);
			}
		}

		internal void ResetMemberAndCellIndexInCollectionTable()
		{
			this.m_indexInCollectionTable.Clear();
		}

		internal void SetIndexInCollection(IIndexedInCollection indexedInCollection)
		{
			Dictionary<Hashtable, int> dictionary;
			switch (indexedInCollection.IndexedInCollectionType)
			{
			case IndexedInCollectionType.DataRegion:
				dictionary = this.m_indexInCollectionTableForDataRegions;
				break;
			case IndexedInCollectionType.SubReport:
				dictionary = this.m_indexInCollectionTableForSubReports;
				break;
			default:
				dictionary = this.m_indexInCollectionTable;
				break;
			}
			Hashtable key = (Hashtable)this.m_groupingScopes.Clone();
			int num = default(int);
			if (dictionary.TryGetValue(key, out num))
			{
				num = (dictionary[key] = num + 1);
			}
			else
			{
				num = 0;
				dictionary.Add(key, num);
			}
			indexedInCollection.IndexInCollection = num;
		}

		internal void RegisterPageSectionScope(Page rifPage, List<DataAggregateInfo> scopeAggregates)
		{
			Global.Tracer.Assert(null != scopeAggregates, "(null != scopeAggregates)");
			this.m_currentScope = new ScopeInfo(false, scopeAggregates, rifPage);
		}

		internal void UnRegisterPageSectionScope()
		{
			this.m_currentScope = null;
		}

		internal void RegisterRunningValues(List<RunningValueInfo> runningValues, List<RunningValueInfo> runningValuesOfAggregates)
		{
			Global.Tracer.Assert(runningValues != null, "(runningValues != null)");
			Global.Tracer.Assert(runningValuesOfAggregates != null, "(runningValuesOfAggregates != null)");
			this.m_runningValues = runningValues;
			this.m_runningValuesOfAggregates = runningValuesOfAggregates;
		}

		internal void RegisterRunningValues(string groupName, List<RunningValueInfo> runningValues, List<RunningValueInfo> runningValuesOfAggregates)
		{
			Global.Tracer.Assert(runningValues != null, "(runningValues != null)");
			Global.Tracer.Assert(runningValuesOfAggregates != null, "(runningValuesOfAggregates != null)");
			this.m_groupingScopesForRunningValues[groupName] = null;
			this.m_runningValues = runningValues;
			this.m_runningValuesOfAggregates = runningValuesOfAggregates;
		}

		internal void UnRegisterRunningValues(List<RunningValueInfo> runningValues, List<RunningValueInfo> runningValuesOfAggregates)
		{
			Global.Tracer.Assert(runningValues != null, "(runningValues != null)");
			Global.Tracer.Assert(runningValuesOfAggregates != null, "(runningValuesOfAggregates != null)");
			Global.Tracer.Assert(object.ReferenceEquals(this.m_runningValues, runningValues));
			Global.Tracer.Assert(object.ReferenceEquals(this.m_runningValuesOfAggregates, runningValuesOfAggregates));
			this.m_runningValues = null;
			this.m_runningValuesOfAggregates = null;
		}

		internal void TransferGroupExpressionRowNumbers(List<RunningValueInfo> rowNumbers)
		{
			if (rowNumbers != null)
			{
				for (int num = rowNumbers.Count - 1; num >= 0; num--)
				{
					Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 != (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping), "(0 != (m_location & LocationFlags.InGrouping))");
					RunningValueInfo runningValueInfo = rowNumbers[num];
					Global.Tracer.Assert(null != runningValueInfo, "(null != rowNumber)");
					string scope = runningValueInfo.Scope;
					bool flag = true;
					ScopeInfo scopeInfo = null;
					if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDynamicTablixCell) != 0)
					{
						flag = false;
					}
					else if (scope == null)
					{
						if (this.m_outerGroupName != null)
						{
							flag = false;
						}
						else
						{
							scopeInfo = this.m_outermostDataregionScope;
						}
					}
					else if (this.m_outerGroupName == scope)
					{
						Global.Tracer.Assert(null != this.m_outerGroupName, "(null != m_outerGroupName)");
						scopeInfo = (ScopeInfo)this.m_groupingScopes[this.m_outerGroupName];
					}
					else if (this.m_currentDataRegionName == scope)
					{
						Global.Tracer.Assert(null != this.m_currentDataRegionName, "(null != m_currentDataRegionName)");
						scopeInfo = (ScopeInfo)this.m_dataregionScopes[this.m_currentDataRegionName];
					}
					else
					{
						flag = false;
					}
					if (!flag)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidGroupExpressionScope, Severity.Error, this.m_objectType, this.m_objectName, "GroupExpression");
					}
					else if (scopeInfo != null)
					{
						Global.Tracer.Assert(null != scopeInfo.Aggregates, "(null != destinationScope.Aggregates)");
						scopeInfo.Aggregates.Add(runningValueInfo);
					}
					rowNumbers.RemoveAt(num);
				}
			}
		}

		internal void TransferRunningValues(List<RunningValueInfo> runningValues, string propertyName)
		{
			this.TransferRunningValues(runningValues, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void TransferRunningValues(List<RunningValueInfo> runningValues, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (runningValues != null && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				for (int num = runningValues.Count - 1; num >= 0; num--)
				{
					RunningValueInfo runningValueInfo = runningValues[num];
					Global.Tracer.Assert(null != runningValueInfo, "(null != runningValue)");
					bool flag = runningValueInfo.AggregateType == DataAggregateInfo.AggregateTypes.Previous;
					string scope = runningValueInfo.Scope;
					bool flag2 = true;
					string text = null;
					ScopeInfo scopeInfo = null;
					List<DataAggregateInfo> list = null;
					List<RunningValueInfo> list2 = null;
					if (scope == null)
					{
						if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
						{
							flag2 = false;
						}
						else if (flag && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem) != 0)
						{
							if (runningValueInfo.HasDirectFieldReferences)
							{
								if (this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail && this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail)
								{
									text = this.GetDataSetName();
									list2 = this.GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
									runningValueInfo.IsScopedInEvaluationScope = true;
								}
								else
								{
									flag2 = false;
								}
							}
							else
							{
								text = this.GetDataSetName();
								list2 = this.GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
								if (this.m_groupingScopesForRunningValuesInTablix.IsRunningValueDirectionColumn && this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null)
								{
									runningValueInfo.Scope = this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
									if (this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail)
									{
										runningValueInfo.IsScopedInEvaluationScope = true;
									}
								}
								else if (this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
								{
									runningValueInfo.Scope = this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
									if (this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail)
									{
										runningValueInfo.IsScopedInEvaluationScope = true;
									}
								}
							}
						}
						else if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0 || (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0)
						{
							text = this.GetDataSetName();
							list2 = this.GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
							if (flag)
							{
								runningValueInfo.Scope = this.m_currentGroupName;
								runningValueInfo.IsScopedInEvaluationScope = true;
							}
						}
						else
						{
							text = this.GetDataSetName();
							if (text != null)
							{
								scopeInfo = (ScopeInfo)this.m_datasetScopes[text];
								Global.Tracer.Assert(null != scopeInfo, "(null != destinationScope)");
								list = scopeInfo.Aggregates;
							}
						}
					}
					else if (this.m_groupingScopesForRunningValuesInTablix != null && this.m_groupingScopesForRunningValuesInTablix.ContainsScope(scope, this.m_errorContext, true, this.m_groupingScopes))
					{
						text = this.GetDataSetName();
						list2 = this.GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
						Global.Tracer.Assert((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixCell) != 0 || (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 != (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem));
						if (flag)
						{
							if (runningValueInfo.HasDirectFieldReferences && (!this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail || !this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail))
							{
								flag2 = false;
							}
							else
							{
								ScopeInfo scopeInfo2 = (ScopeInfo)this.m_groupingScopes[scope];
								if (scopeInfo2.GroupingScope.Owner.IsColumn)
								{
									if (this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail && scope == this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName)
									{
										runningValueInfo.IsScopedInEvaluationScope = true;
									}
								}
								else if (this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail && scope == this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName)
								{
									runningValueInfo.IsScopedInEvaluationScope = true;
								}
							}
						}
					}
					else if (this.m_groupingScopesForRunningValues.ContainsKey(scope))
					{
						Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 != (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping), "(0 != (m_location & LocationFlags.InGrouping))");
						if (flag && runningValueInfo.HasDirectFieldReferences && this.m_groupingScopesForRunningValuesInTablix != null && (!this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail || !this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail))
						{
							flag2 = false;
						}
						else if (this.m_groupingScopesForRunningValuesInTablix == null || this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeIsDetail || this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeIsDetail)
						{
							text = this.GetDataSetName();
							list2 = this.GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
						}
						else
						{
							flag2 = false;
						}
					}
					else if (this.m_dataregionScopesForRunningValues.ContainsKey(scope))
					{
						Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 != (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion), "(0 != (m_location & LocationFlags.InDataRegion))");
						runningValueInfo.Scope = (string)this.m_dataregionScopesForRunningValues[scope];
						if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0 || (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0 || flag)
						{
							text = this.GetDataSetName();
							list2 = this.GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
							if (flag && runningValueInfo.Scope == null)
							{
								runningValueInfo = null;
								list2 = null;
							}
						}
						else
						{
							text = this.GetDataSetName();
							if (text != null)
							{
								scopeInfo = (ScopeInfo)this.m_datasetScopes[text];
								Global.Tracer.Assert(null != scopeInfo, "(null != destinationScope)");
								list = scopeInfo.Aggregates;
							}
						}
					}
					else if (this.m_datasetScopes.ContainsKey(scope))
					{
						if (flag)
						{
							runningValueInfo = null;
							list2 = null;
						}
						else
						{
							if (((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping) != 0 || (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0) && scope == this.GetDataSetName())
							{
								if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixCell) != 0 && this.IsDataRegionCellScope)
								{
									flag2 = false;
								}
								else
								{
									text = scope;
									runningValueInfo.Scope = null;
									list2 = this.GetActiveRunningValuesCollection(runningValueInfo.IsAggregateOfAggregate, flag);
									runningValueInfo.IsScopedInEvaluationScope = true;
								}
								goto IL_0566;
							}
							text = scope;
							scopeInfo = (ScopeInfo)this.m_datasetScopes[scope];
							Global.Tracer.Assert(null != scopeInfo, "(null != destinationScope)");
							list = scopeInfo.Aggregates;
						}
					}
					else
					{
						flag2 = false;
					}
					goto IL_0566;
					IL_0566:
					if (!flag2)
					{
						if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDynamicTablixCell) != 0)
						{
							if (DataAggregateInfo.AggregateTypes.Previous == runningValueInfo.AggregateType)
							{
								this.m_errorContext.Register(ProcessingErrorCode.rsInvalidPreviousAggregateInTablixCell, Severity.Error, objectType, objectName, propertyName, this.m_tablixName);
							}
							else
							{
								this.m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeInTablix, Severity.Error, objectType, objectName, propertyName, this.m_tablixName);
							}
						}
						else
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, objectType, objectName, propertyName);
						}
					}
					else if (runningValueInfo != null)
					{
						if (scopeInfo == null)
						{
							scopeInfo = (ScopeInfo)this.m_datasetScopes[this.GetCurrentDataSetName()];
						}
						runningValueInfo.DataSetIndexInCollection = scopeInfo.DataSetScope.IndexInCollection;
						this.RegisterDataSetLevelAggregateOrLookup(runningValueInfo.DataSetIndexInCollection);
						runningValueInfo.Initialize(this, text, objectType, objectName, propertyName);
						if (list != null)
						{
							list.Add(runningValueInfo);
						}
						else if (list2 != null)
						{
							Global.Tracer.Assert(!object.ReferenceEquals(runningValues, list2));
							if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0 && !this.m_isDataRegionScopedCell)
							{
								string text2 = "";
								if (this.m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicRow)
								{
									text2 = this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
								}
								if (this.m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicColumn)
								{
									text2 = text2 + "." + this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
								}
								runningValueInfo.EvaluationScopeName = text2;
							}
							else if (this.m_currentScope.DataRegionScope != null)
							{
								runningValueInfo.EvaluationScopeName = this.m_currentScope.DataRegionScope.Name;
							}
							list2.Add(runningValueInfo);
							if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0 && (!flag || runningValueInfo.HasDirectFieldReferences) && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0 && !this.m_isDataRegionScopedCell)
							{
								if (this.m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicRow)
								{
									((ScopeInfo)this.m_groupingScopes[this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName]).ReportScope.NeedToCacheDataRows = true;
								}
								if (this.m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicColumn)
								{
									((ScopeInfo)this.m_groupingScopes[this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName]).ReportScope.NeedToCacheDataRows = true;
								}
								if (this.m_currentScope.ReportScope != null)
								{
									this.m_currentScope.ReportScope.NeedToCacheDataRows = true;
								}
							}
						}
						this.StoreAggregateScopeAndLocationInfo(runningValueInfo, this.m_currentScope, objectType, objectName, propertyName);
					}
					runningValues.RemoveAt(num);
				}
			}
		}

		private List<RunningValueInfo> GetActiveRunningValuesCollection(bool isAggregateOfAggregates, bool isPrevious)
		{
			if (!isPrevious && isAggregateOfAggregates)
			{
				return this.m_runningValuesOfAggregates;
			}
			return this.m_runningValues;
		}

		private void StoreAggregateScopeAndLocationInfo(DataAggregateInfo aggregate, ScopeInfo destinationScope, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (destinationScope != null)
			{
				aggregate.EvaluationScope = destinationScope.DataScope;
			}
			aggregate.PublishingInfo.ObjectType = objectType;
			aggregate.PublishingInfo.ObjectName = objectName;
			aggregate.PublishingInfo.PropertyName = propertyName;
		}

		internal void SpecialTransferRunningValues(List<RunningValueInfo> runningValues, List<RunningValueInfo> runningValuesOfAggregates)
		{
			this.TransferRunningValueCollection(runningValues, this.m_runningValues);
			this.TransferRunningValueCollection(runningValuesOfAggregates, this.m_runningValuesOfAggregates);
		}

		private void TransferRunningValueCollection(List<RunningValueInfo> runningValues, List<RunningValueInfo> destRunningValues)
		{
			if (runningValues != null)
			{
				Global.Tracer.Assert(null != destRunningValues, "(null != m_runningValues)");
				for (int num = runningValues.Count - 1; num >= 0; num--)
				{
					Global.Tracer.Assert(!object.ReferenceEquals(runningValues, destRunningValues));
					destRunningValues.Add(runningValues[num]);
					runningValues.RemoveAt(num);
				}
			}
		}

		internal void TransferLookups(List<LookupInfo> lookups, string propertyName)
		{
			this.TransferLookups(lookups, this.m_objectType, this.m_objectName, propertyName, this.GetDataSetName());
		}

		internal void TransferLookups(List<LookupInfo> lookups, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, string dataSetName)
		{
			if (lookups != null)
			{
				for (int num = lookups.Count - 1; num >= 0; num--)
				{
					LookupInfo lookupInfo = lookups[num];
					Global.Tracer.Assert(null != lookupInfo, "(null != lookup)");
					LookupDestinationInfo destinationInfo = lookupInfo.DestinationInfo;
					Global.Tracer.Assert(null != destinationInfo, "(null != destinationInfo)");
					string scope = destinationInfo.Scope;
					if (string.IsNullOrEmpty(scope) || !this.m_datasetScopes.ContainsKey(scope))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidLookupScope, Severity.Error, objectType, objectName, propertyName);
					}
					else
					{
						lookupInfo.Initialize(this, scope, objectType, objectName, propertyName);
						ScopeInfo scopeInfo = (ScopeInfo)this.m_datasetScopes[scope];
						DataSet dataSetScope = scopeInfo.DataSetScope;
						LookupDestinationCompactionTable lookupDestinationCompactionTable = default(LookupDestinationCompactionTable);
						if (!this.m_lookupCompactionTable.TryGetValue(scope, out lookupDestinationCompactionTable))
						{
							lookupDestinationCompactionTable = new LookupDestinationCompactionTable();
							this.m_lookupCompactionTable[scope] = lookupDestinationCompactionTable;
						}
						destinationInfo.UsedInSameDataSetTablixProcessing = string.Equals(scope, dataSetName, StringComparison.Ordinal);
						string originalText = destinationInfo.DestinationExpr.OriginalText;
						int num2 = default(int);
						if (!((Dictionary<string, int>)lookupDestinationCompactionTable).TryGetValue(originalText, out num2))
						{
							if (dataSetScope.LookupDestinationInfos == null)
							{
								dataSetScope.LookupDestinationInfos = new List<LookupDestinationInfo>();
							}
							destinationInfo.Initialize(this, scope, objectType, objectName, propertyName);
							num2 = (destinationInfo.IndexInCollection = dataSetScope.LookupDestinationInfos.Count);
							dataSetScope.LookupDestinationInfos.Add(destinationInfo);
							((Dictionary<string, int>)lookupDestinationCompactionTable)[originalText] = num2;
						}
						else
						{
							LookupDestinationInfo lookupDestinationInfo = dataSetScope.LookupDestinationInfos[num2];
							lookupDestinationInfo.IsMultiValue |= destinationInfo.IsMultiValue;
							lookupDestinationInfo.UsedInSameDataSetTablixProcessing |= destinationInfo.UsedInSameDataSetTablixProcessing;
						}
						lookupInfo.DestinationIndexInCollection = num2;
						lookupInfo.DestinationInfo = null;
						lookupInfo.DataSetIndexInCollection = dataSetScope.IndexInCollection;
						if (dataSetScope.Lookups == null)
						{
							dataSetScope.Lookups = new List<LookupInfo>();
						}
						dataSetScope.Lookups.Add(lookupInfo);
						this.RegisterDataSetLevelAggregateOrLookup(dataSetScope.IndexInCollection);
					}
					lookups.RemoveAt(num);
				}
			}
		}

		internal void TransferAggregates(List<DataAggregateInfo> aggregates, string propertyName)
		{
			this.TransferAggregates(aggregates, this.m_objectType, this.m_objectName, propertyName, false);
		}

		internal void TransferNestedAggregates(List<DataAggregateInfo> aggregates, string propertyName)
		{
			this.TransferAggregates(aggregates, this.m_objectType, this.m_objectName, propertyName, true);
		}

		private void TransferAggregates(List<DataAggregateInfo> aggregates, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, bool isInAggregate)
		{
			if (aggregates != null)
			{
				for (int num = aggregates.Count - 1; num >= 0; num--)
				{
					DataAggregateInfo dataAggregateInfo = aggregates[num];
					Global.Tracer.Assert(null != dataAggregateInfo, "(null != aggregate)");
					if (this.m_hasFilters && DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsCustomAggregateAndFilter, Severity.Error, objectType, objectName, propertyName);
					}
					string text = default(string);
					bool scope = dataAggregateInfo.GetScope(out text);
					bool flag = true;
					string text2 = null;
					ScopeInfo scopeInfo = null;
					bool flag2 = false;
					if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 && this.m_numberOfDataSets == 0)
					{
						flag = false;
						flag2 = true;
					}
					else if (!scope)
					{
						text2 = this.GetDataSetName();
						if (AspNetCore.ReportingServices.ReportPublishing.LocationFlags.None == this.m_location)
						{
							if (1 != this.m_numberOfDataSets)
							{
								flag = false;
								this.m_errorContext.Register(ProcessingErrorCode.rsMissingAggregateScope, Severity.Error, objectType, objectName, propertyName);
							}
							else if (text2 != null)
							{
								scopeInfo = (ScopeInfo)this.m_datasetScopes[text2];
							}
						}
						else if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
						{
							if (dataAggregateInfo.Expressions != null && dataAggregateInfo.Expressions.Length > 0)
							{
								ExpressionInfo expressionInfo = dataAggregateInfo.Expressions[0];
								Global.Tracer.Assert(null != expressionInfo, "(null != paramExpr)");
								if (expressionInfo.HasAnyFieldReferences)
								{
									flag = false;
									this.m_errorContext.Register(ProcessingErrorCode.rsMissingAggregateScopeInPageSection, Severity.Error, objectType, objectName, propertyName);
								}
								else
								{
									scopeInfo = this.m_currentScope;
								}
							}
						}
						else
						{
							Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 != (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet));
							scopeInfo = this.m_currentScope;
						}
						if (scopeInfo != null && scopeInfo.DataSetScope != null)
						{
							scopeInfo.DataSetScope.UsedInAggregates = true;
							dataAggregateInfo.DataSetIndexInCollection = scopeInfo.DataSetScope.IndexInCollection;
							this.RegisterDataSetLevelAggregateOrLookup(dataAggregateInfo.DataSetIndexInCollection);
						}
					}
					else if (text == null)
					{
						flag = false;
					}
					else if (this.m_groupingScopes.ContainsKey(text))
					{
						Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 != (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping), "(0 != (m_location & LocationFlags.InGrouping))");
						text2 = this.GetDataSetName();
						scopeInfo = (ScopeInfo)this.m_groupingScopes[text];
					}
					else if (this.m_dataregionScopes.ContainsKey(text))
					{
						Global.Tracer.Assert((AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0 != (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion), "(0 != (m_location & LocationFlags.InDataRegion))");
						text2 = this.GetDataSetName();
						scopeInfo = (ScopeInfo)this.m_dataregionScopes[text];
					}
					else if (this.m_datasetScopes.ContainsKey(text))
					{
						if (isInAggregate)
						{
							flag = false;
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidNestedDataSetAggregate, Severity.Error, objectType, objectName, propertyName);
						}
						else if (dataAggregateInfo.IsAggregateOfAggregate)
						{
							flag = false;
							this.m_errorContext.Register(ProcessingErrorCode.rsDataSetAggregateOfAggregates, Severity.Error, objectType, objectName, propertyName);
						}
						if (flag && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0 && dataAggregateInfo.Expressions != null && dataAggregateInfo.Expressions.Length > 0)
						{
							ExpressionInfo expressionInfo2 = dataAggregateInfo.Expressions[0];
							Global.Tracer.Assert(null != expressionInfo2, "(null != paramExpr)");
							List<string> referencedReportItems = expressionInfo2.ReferencedReportItems;
							if (referencedReportItems != null && referencedReportItems.Count > 0)
							{
								flag = false;
								this.m_errorContext.Register(ProcessingErrorCode.rsReportItemInScopedAggregate, Severity.Error, objectType, objectName, propertyName, referencedReportItems[0]);
							}
							else if (expressionInfo2.ReferencedOverallPageGlobals)
							{
								flag = false;
								this.m_errorContext.Register(ProcessingErrorCode.rsOverallPageNumberInScopedAggregate, Severity.Error, objectType, objectName, propertyName);
							}
							else if (expressionInfo2.ReferencedPageGlobals)
							{
								flag = false;
								this.m_errorContext.Register(ProcessingErrorCode.rsPageNumberInScopedAggregate, Severity.Error, objectType, objectName, propertyName);
							}
						}
						if (flag)
						{
							text2 = text;
							scopeInfo = (ScopeInfo)this.m_datasetScopes[text];
							if (!scopeInfo.IsDuplicateScope)
							{
								scopeInfo.DataSetScope.UsedInAggregates = true;
								dataAggregateInfo.DataSetIndexInCollection = scopeInfo.DataSetScope.IndexInCollection;
								this.RegisterDataSetLevelAggregateOrLookup(dataAggregateInfo.DataSetIndexInCollection);
							}
						}
					}
					else if (isInAggregate)
					{
						ISortFilterScope sortFilterScope = default(ISortFilterScope);
						if (!this.m_reportScopes.TryGetValue(text, out sortFilterScope))
						{
							flag = false;
							flag2 = true;
						}
						else if (sortFilterScope is Grouping)
						{
							Grouping grouping = (Grouping)sortFilterScope;
							ReportHierarchyNode owner = grouping.Owner;
							text2 = this.GetDataSetName();
							scopeInfo = this.CreateScopeInfo(owner, false);
						}
						else if (sortFilterScope is DataRegion)
						{
							text2 = this.GetDataSetName();
							scopeInfo = this.CreateScopeInfo((DataRegion)sortFilterScope, false);
						}
						else
						{
							flag = false;
							flag2 = true;
						}
					}
					else
					{
						flag = false;
						flag2 = true;
					}
					if (flag2)
					{
						ProcessingErrorCode code = (ProcessingErrorCode)((!isInAggregate) ? 86 : 477);
						this.m_errorContext.Register(code, Severity.Error, objectType, objectName, propertyName);
					}
					if (flag && scopeInfo != null)
					{
						if (scopeInfo.DataSetScope == null && dataAggregateInfo.DataSetIndexInCollection < 0)
						{
							dataAggregateInfo.DataSetIndexInCollection = this.GetCurrentDataSetIndex();
						}
						if (DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType && !scopeInfo.AllowCustomAggregates)
						{
							ProcessingErrorCode code2 = (ProcessingErrorCode)((!isInAggregate) ? 99 : 478);
							this.m_errorContext.Register(code2, Severity.Error, objectType, objectName, propertyName);
						}
						if (dataAggregateInfo.Expressions != null)
						{
							for (int i = 0; i < dataAggregateInfo.Expressions.Length; i++)
							{
								Global.Tracer.Assert(null != dataAggregateInfo.Expressions[i], "(null != aggregate.Expressions[j])");
								dataAggregateInfo.Expressions[i].AggregateInitialize(text2, objectType, objectName, propertyName, this);
							}
						}
						if (DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType)
						{
							DataSet dataSet = this.GetDataSet(text2);
							if (dataSet != null)
							{
								dataSet.HasScopeWithCustomAggregates = true;
								if (dataSet.InterpretSubtotalsAsDetails == DataSet.TriState.Auto)
								{
									dataSet.InterpretSubtotalsAsDetails = DataSet.TriState.False;
								}
							}
						}
						List<DataAggregateInfo> list;
						if (dataAggregateInfo.Recursive)
						{
							list = ((scopeInfo.GroupingScope != null && scopeInfo.GroupingScope.Parent != null) ? scopeInfo.RecursiveAggregates : scopeInfo.Aggregates);
						}
						else if (dataAggregateInfo.IsAggregateOfAggregate && scopeInfo.DataScopeInfo != null)
						{
							AggregateBucket<DataAggregateInfo> aggregateBucket = (!dataAggregateInfo.IsPostSortAggregate()) ? scopeInfo.DataScopeInfo.AggregatesOfAggregates.GetOrCreateBucket(dataAggregateInfo.PublishingInfo.AggregateOfAggregatesLevel) : scopeInfo.DataScopeInfo.PostSortAggregatesOfAggregates.GetOrCreateBucket(0);
							list = aggregateBucket.Aggregates;
						}
						else if (scopeInfo.PostSortAggregates != null && dataAggregateInfo.IsPostSortAggregate())
						{
							list = scopeInfo.PostSortAggregates;
							if (scopeInfo.ReportScope != null)
							{
								scopeInfo.ReportScope.NeedToCacheDataRows = true;
							}
							if (this.m_groupingScopesForRunningValuesInTablix != null)
							{
								if (this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
								{
									((ScopeInfo)this.m_groupingScopes[this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName]).ReportScope.NeedToCacheDataRows = true;
								}
								if (this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null)
								{
									((ScopeInfo)this.m_groupingScopes[this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName]).ReportScope.NeedToCacheDataRows = true;
								}
							}
						}
						else
						{
							list = scopeInfo.Aggregates;
						}
						Global.Tracer.Assert(null != list, "(null != destinationAggregates)");
						Global.Tracer.Assert(!object.ReferenceEquals(aggregates, list));
						if (!scope && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) != 0)
						{
							if (!this.m_isDataRegionScopedCell)
							{
								string text3 = "";
								if (this.m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicRow)
								{
									text3 = this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
								}
								if (this.m_axisGroupingScopesForRunningValues.InCurrentDataRegionDynamicColumn)
								{
									text3 = text3 + "." + this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
								}
								dataAggregateInfo.EvaluationScopeName = text3;
							}
							else if (this.m_currentScope.DataRegionScope != null)
							{
								dataAggregateInfo.EvaluationScopeName = this.m_currentScope.DataRegionScope.Name;
							}
						}
						list.Add(dataAggregateInfo);
						this.StoreAggregateScopeAndLocationInfo(dataAggregateInfo, scopeInfo, objectType, objectName, propertyName);
					}
					aggregates.RemoveAt(num);
				}
			}
		}

		internal void RegisterReportSection(ReportSection sectionDef)
		{
			this.m_currentScope = new ScopeInfo(false, null, sectionDef);
			this.m_reportItemsInSection = new Dictionary<string, ReportItem>();
			this.m_referencableTextboxesInSection = new byte[this.m_referencableTextboxesInSection.Length];
		}

		internal void UnRegisterReportSection()
		{
			this.m_currentScope = null;
			this.m_reportItemsInSection = null;
		}

		internal void InitializeParameters(List<ParameterDef> parameters, List<DataSet> dataSetList)
		{
			if (this.m_dynamicParameters != null && this.m_dynamicParameters.Count != 0)
			{
				Hashtable hashtable = new Hashtable();
				AspNetCore.ReportingServices.ReportPublishing.DynamicParameter dynamicParameter = null;
				int i = 0;
				for (int j = 0; j < this.m_dynamicParameters.Count; j++)
				{
					for (dynamicParameter = (AspNetCore.ReportingServices.ReportPublishing.DynamicParameter)this.m_dynamicParameters[j]; i < dynamicParameter.Index; i++)
					{
						hashtable.Add(parameters[i].Name, i);
					}
					this.InitializeParameter(parameters[dynamicParameter.Index], dynamicParameter, hashtable, dataSetList);
				}
			}
		}

		private void InitializeParameter(ParameterDef parameter, AspNetCore.ReportingServices.ReportPublishing.DynamicParameter dynamicParameter, Hashtable dependencies, List<DataSet> dataSetList)
		{
			Global.Tracer.Assert(null != dynamicParameter, "(null != dynamicParameter)");
			AspNetCore.ReportingServices.ReportPublishing.DataSetReference dataSetReference = null;
			bool isComplex = dynamicParameter.IsComplex;
			dataSetReference = dynamicParameter.ValidValueDataSet;
			if (dataSetReference != null)
			{
				this.InitializeParameterDataSource(parameter, dataSetReference, false, dependencies, ref isComplex, dataSetList);
			}
			dataSetReference = dynamicParameter.DefaultDataSet;
			if (dataSetReference != null)
			{
				this.InitializeParameterDataSource(parameter, dataSetReference, true, dependencies, ref isComplex, dataSetList);
			}
		}

		private void InitializeParameterDataSource(ParameterDef parameter, AspNetCore.ReportingServices.ReportPublishing.DataSetReference dataSetRef, bool isDefault, Hashtable dependencies, ref bool isComplex, List<DataSet> dataSetList)
		{
			ParameterDataSource parameterDataSource = null;
			PublishingDataSetInfo publishingDataSetInfo = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			publishingDataSetInfo = (PublishingDataSetInfo)this.m_dataSetQueryInfo[dataSetRef.DataSet];
			if (publishingDataSetInfo == null)
			{
				if (isDefault)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDefaultValueDataSetReference, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "DataSetReference", dataSetRef.DataSet.MarkAsPrivate());
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidValidValuesDataSetReference, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "DataSetReference", dataSetRef.DataSet.MarkAsPrivate());
				}
			}
			else
			{
				DataSet dataSet = dataSetList[publishingDataSetInfo.DataSetDefIndex];
				if (!dataSet.UsedInAggregates && !dataSet.HasLookups && !dataSet.UsedOnlyInParametersSet)
				{
					List<DataRegion> list = default(List<DataRegion>);
					this.m_dataSetNameToDataRegionsMap.TryGetValue(dataSetRef.DataSet, out list);
					if (list == null || list.Count == 0)
					{
						dataSet.UsedOnlyInParameters = true;
					}
				}
				parameterDataSource = new ParameterDataSource(publishingDataSetInfo.DataSourceIndex, publishingDataSetInfo.DataSetIndex);
				Hashtable hashtable = (Hashtable)this.m_fieldNameMap[dataSetRef.DataSet];
				if (hashtable != null)
				{
					if (hashtable.ContainsKey(dataSetRef.ValueAlias))
					{
						parameterDataSource.ValueFieldIndex = (int)hashtable[dataSetRef.ValueAlias];
						if (parameterDataSource.ValueFieldIndex >= publishingDataSetInfo.CalculatedFieldIndex)
						{
							flag3 = true;
						}
						flag = true;
					}
					if (dataSetRef.LabelAlias != null)
					{
						if (hashtable.ContainsKey(dataSetRef.LabelAlias))
						{
							parameterDataSource.LabelFieldIndex = (int)hashtable[dataSetRef.LabelAlias];
							if (parameterDataSource.LabelFieldIndex >= publishingDataSetInfo.CalculatedFieldIndex)
							{
								flag3 = true;
							}
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else if (dataSetRef.LabelAlias == null)
				{
					flag2 = true;
				}
				if (!flag)
				{
					this.ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetReferenceField, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "Field", dataSetRef.ValueAlias.MarkAsPrivate(), dataSetRef.DataSet.MarkAsPrivate());
				}
				if (!flag2)
				{
					this.ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetReferenceField, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "Field", dataSetRef.LabelAlias.MarkAsPrivate(), dataSetRef.DataSet.MarkAsPrivate());
				}
				if (!isComplex)
				{
					if (publishingDataSetInfo.IsComplex || flag3)
					{
						isComplex = true;
						parameter.Dependencies = (Hashtable)dependencies.Clone();
					}
					else if (publishingDataSetInfo.ParameterNames != null && publishingDataSetInfo.ParameterNames.Count != 0)
					{
						Hashtable hashtable2 = parameter.Dependencies;
						if (hashtable2 == null)
						{
							hashtable2 = (parameter.Dependencies = new Hashtable());
						}
						foreach (string key in publishingDataSetInfo.ParameterNames.Keys)
						{
							if (dependencies.ContainsKey(key))
							{
								if (!hashtable2.ContainsKey(key))
								{
									hashtable2.Add(key, dependencies[key]);
								}
							}
							else
							{
								this.ErrorContext.Register(ProcessingErrorCode.rsInvalidReportParameterDependency, Severity.Error, AspNetCore.ReportingServices.ReportProcessing.ObjectType.ReportParameter, parameter.Name, "DataSetReference", key.MarkAsPrivate());
							}
						}
					}
				}
			}
			if (isDefault)
			{
				parameter.DefaultDataSource = parameterDataSource;
			}
			else
			{
				parameter.ValidValuesDataSource = parameterDataSource;
			}
		}

		internal bool ValidateSliderLabelData(Tablix tablix, LabelData labelData)
		{
			if (labelData == null)
			{
				return true;
			}
			DataSet dataSet = null;
			dataSet = ((labelData.DataSetName != null) ? this.GetDataSet(labelData.DataSetName) : this.GetDataSet());
			if (dataSet != null)
			{
				labelData.DataSetName = dataSet.Name;
				bool flag = true;
				if (labelData.Label != null)
				{
					flag &= this.ValidateSliderDataFieldReference(labelData.Label, "Label", tablix.ObjectType, tablix.Name, dataSet);
				}
				if (labelData.KeyFields != null)
				{
					{
						foreach (string keyField in labelData.KeyFields)
						{
							flag &= this.ValidateSliderDataFieldReference(keyField, "Key", tablix.ObjectType, tablix.Name, dataSet);
						}
						return flag;
					}
				}
				return flag;
			}
			this.ErrorContext.Register(ProcessingErrorCode.rsInvalidSliderDataSetReference, Severity.Error, tablix.ObjectType, tablix.Name, null, labelData.DataSetName.MarkAsPrivate());
			return false;
		}

		private bool ValidateSliderDataFieldReference(string fieldName, string propertyName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, DataSet dataSet)
		{
			Hashtable hashtable = (Hashtable)this.m_fieldNameMap[dataSet.Name];
			if (hashtable != null && hashtable.ContainsKey(fieldName))
			{
				return true;
			}
			this.ErrorContext.Register(ProcessingErrorCode.rsInvalidSliderDataSetReferenceField, Severity.Error, objectType, objectName, propertyName, fieldName.MarkAsModelInfo(), dataSet.Name.MarkAsPrivate());
			return false;
		}

		internal void RegisterDataSetLevelAggregateOrLookup(int referencedDataSetIndex)
		{
			if (this.GetCurrentDataSetIndex() != referencedDataSetIndex)
			{
				if (-1 == this.GetCurrentDataSetIndex())
				{
					this.m_report.SetDatasetDependency(referencedDataSetIndex, referencedDataSetIndex, false);
				}
				else
				{
					DataSet dataSet = this.GetDataSet();
					if (dataSet != null && dataSet.DataSource != null)
					{
						this.m_report.SetDatasetDependency(this.GetCurrentDataSetIndex(), referencedDataSetIndex, false);
					}
				}
			}
		}

		internal DataSet GetParentDataSet()
		{
			return this.m_activeDataSets.First;
		}

		internal bool RegisterDataRegion(DataRegion dataRegion)
		{
			DataSet first = this.m_activeDataSets.First;
			DataSet dataSet = dataRegion.DataScopeInfo.DataSet;
			if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				this.m_dataSetsForIdcInNestedDR.Clear();
				if (!this.m_initializingUserSorts)
				{
					if (this.m_report.TopLevelDataRegions == null)
					{
						this.m_report.TopLevelDataRegions = new List<DataRegion>();
					}
					this.m_report.TopLevelDataRegions.Add(dataRegion);
				}
				if (dataRegion.DataScopeInfo.DataSet != null)
				{
					if (dataSet != null && !this.m_initializingUserSorts)
					{
						List<DataRegion> list = default(List<DataRegion>);
						this.m_dataSetNameToDataRegionsMap.TryGetValue(dataSet.Name, out list);
						Global.Tracer.Assert(null != list, "(null != dataRegions)");
						list.Add(dataRegion);
					}
					goto IL_00b3;
				}
				return false;
			}
			goto IL_00b3;
			IL_00b3:
			this.RegisterDataSet(dataSet);
			this.RegisterDataRegionScope(dataRegion);
			return true;
		}

		internal void UnRegisterDataRegion(DataRegion dataRegion)
		{
			DataSet dataSet = dataRegion.DataScopeInfo.DataSet;
			this.UnRegisterDataSet(dataSet);
			this.UnRegisterDataRegionScope(dataRegion);
		}

		internal void RegisterDataSet(DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataSet first = this.m_activeDataSets.First;
				this.m_activeDataSets = this.m_activeDataSets.Add(dataSet);
				this.m_activeScopeInfos = this.m_activeScopeInfos.Add(this.m_currentScope);
				if (first != dataSet)
				{
					ScopeInfo scopeInfo = (ScopeInfo)this.m_datasetScopes[dataSet.Name];
					Global.Tracer.Assert(null != scopeInfo, "(null != dataSetScope)");
					this.RegisterDataSetScope(dataSet, scopeInfo.Aggregates, scopeInfo.PostSortAggregates, dataSet.IndexInCollection);
				}
			}
		}

		internal void UnRegisterDataSet(DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataSet first = this.m_activeDataSets.First;
				this.m_activeDataSets = this.m_activeDataSets.Rest;
				DataSet first2 = this.m_activeDataSets.First;
				if (first != first2)
				{
					this.UnRegisterDataSetScope(dataSet.Name);
				}
				this.m_currentScope = this.m_activeScopeInfos.First;
				this.m_activeScopeInfos = this.m_activeScopeInfos.Rest;
			}
		}

		private string GetDataSetName()
		{
			if (this.m_numberOfDataSets == 0)
			{
				return null;
			}
			if (1 == this.m_numberOfDataSets)
			{
				Global.Tracer.Assert(null != this.m_oneDataSetName);
				return this.m_oneDataSetName;
			}
			Global.Tracer.Assert(1 < this.m_numberOfDataSets);
			return this.GetCurrentDataSetName();
		}

		private DataSet GetDataSet()
		{
			string dataSetName = this.GetDataSetName();
			return this.GetDataSet(dataSetName);
		}

		private DataSet GetDataSet(string dataSetName)
		{
			DataSet dataSet = null;
			if (this.m_numberOfDataSets > 0)
			{
				Global.Tracer.Assert(null != dataSetName, "DataSet name must not be null");
				if (!this.m_reportScopes.ContainsKey(dataSetName))
				{
					return null;
				}
				dataSet = (this.m_reportScopes[dataSetName] as DataSet);
				Global.Tracer.Assert(null != dataSet, "DataSet {0} not found", dataSetName);
			}
			return dataSet;
		}

		internal void SetDataSetHasSubReports()
		{
			DataSet dataSet = this.GetDataSet();
			if (dataSet != null)
			{
				dataSet.HasSubReports = true;
			}
		}

		internal DataRegion GetCurrentDataRegion()
		{
			if (this.m_currentDataRegionName == null)
			{
				return null;
			}
			Global.Tracer.Assert(this.m_dataregionScopes.ContainsKey(this.m_currentDataRegionName));
			return ((ScopeInfo)this.m_dataregionScopes[this.m_currentDataRegionName]).DataRegionScope;
		}

		private bool ValidateDataSetNameForTopLevelDataRegion(string dataSetName, bool registerError)
		{
			bool flag = true;
			if (this.m_numberOfDataSets == 0)
			{
				flag = (null == dataSetName);
			}
			else if (1 == this.m_numberOfDataSets)
			{
				if (dataSetName == null)
				{
					dataSetName = this.m_oneDataSetName;
					flag = true;
				}
				else
				{
					flag = this.m_fieldNameMap.ContainsKey(dataSetName);
				}
			}
			else
			{
				Global.Tracer.Assert(1 < this.m_numberOfDataSets);
				flag = (dataSetName != null && this.m_fieldNameMap.ContainsKey(dataSetName));
			}
			if (!flag && registerError)
			{
				if (dataSetName == null)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsMissingDataSetName, Severity.Error, this.m_objectType, this.m_objectName, "DataSetName");
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSetName, Severity.Error, this.m_objectType, this.m_objectName, "DataSetName", dataSetName.MarkAsPrivate());
				}
			}
			return flag;
		}

		internal void CheckFieldReferences(List<string> fieldNames, string propertyName)
		{
			this.InternalCheckFieldReferences(fieldNames, this.GetDataSetName(), this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckFieldReferences(List<string> fieldNames, string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckFieldReferences(fieldNames, dataSetName, objectType, objectName, propertyName);
		}

		private void InternalCheckFieldReferences(List<string> fieldNames, string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (fieldNames != null)
			{
				for (int i = 0; i < fieldNames.Count; i++)
				{
					this.CheckFieldReference(fieldNames[i], dataSetName, objectType, objectName, propertyName);
				}
			}
		}

		private void CheckFieldReference(string fieldName, string dataSetName, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			Hashtable hashtable = null;
			if (dataSetName != null)
			{
				hashtable = (Hashtable)this.m_fieldNameMap[dataSetName];
			}
			if (this.m_numberOfDataSets == 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsFieldReference, Severity.Error, objectType, objectName, propertyName, fieldName.MarkAsModelInfo());
			}
			else
			{
				Global.Tracer.Assert(1 <= this.m_numberOfDataSets, "Expected 1 or more data sets");
				if (dataSetName != null)
				{
					DataSet dataSet = this.GetDataSet(dataSetName);
					if (dataSet != null && !dataSet.UsedOnlyInParametersSet && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
					{
						dataSet.UsedOnlyInParameters = false;
					}
				}
				if (hashtable == null && this.m_numberOfDataSets > 1 && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsFieldReferenceAmbiguous, Severity.Error, objectType, objectName, propertyName, fieldName.MarkAsModelInfo());
				}
				else
				{
					if (hashtable != null && hashtable.ContainsKey(fieldName))
					{
						return;
					}
					this.m_errorContext.Register(ProcessingErrorCode.rsFieldReference, Severity.Error, objectType, objectName, propertyName, fieldName.MarkAsModelInfo());
				}
			}
		}

		internal void FillInFieldIndex(ExpressionInfo exprInfo)
		{
			this.InternalFillInFieldIndex(exprInfo, this.GetDataSetName());
		}

		internal void FillInFieldIndex(ExpressionInfo exprInfo, string dataSetName)
		{
			this.InternalFillInFieldIndex(exprInfo, dataSetName);
		}

		private void InternalFillInFieldIndex(ExpressionInfo exprInfo, string dataSetName)
		{
			if (exprInfo != null && exprInfo.Type == ExpressionInfo.Types.Field && dataSetName != null)
			{
				exprInfo.IntValue = this.GetIndexForDataSetField(dataSetName, exprInfo.StringValue);
			}
		}

		private int GetIndexForDataSetField(string dataSetName, string fieldName)
		{
			Hashtable hashtable = (Hashtable)this.m_fieldNameMap[dataSetName];
			int result = -1;
			if (hashtable != null && hashtable.ContainsKey(fieldName))
			{
				result = (int)hashtable[fieldName];
			}
			return result;
		}

		internal void FillInTokenIndex(ExpressionInfo exprInfo)
		{
			if (exprInfo != null && exprInfo.Type == ExpressionInfo.Types.Token)
			{
				string stringValue = exprInfo.StringValue;
				if (stringValue != null)
				{
					DataSet dataSet = this.GetDataSet(stringValue);
					if (dataSet != null)
					{
						exprInfo.IntValue = dataSet.ID;
					}
				}
			}
		}

		internal void CheckDataSetReference(List<string> referencedDataSets, string propertyName)
		{
			this.InternalCheckDataSetReference(referencedDataSets, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckDataSetReference(List<string> referencedDataSets, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckDataSetReference(referencedDataSets, objectType, objectName, propertyName);
		}

		private void InternalCheckDataSetReference(List<string> dataSetNames, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (dataSetNames != null && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				for (int i = 0; i < dataSetNames.Count; i++)
				{
					DataSet dataSet = this.m_scopeTree.GetDataSet(dataSetNames[i]);
					if (dataSet == null)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsDataSetReference, Severity.Error, objectType, objectName, propertyName, dataSetNames[i].MarkAsPrivate());
					}
					else if (dataSet.IsReferenceToSharedDataSet)
					{
						dataSet.UsedOnlyInParameters = false;
					}
				}
			}
		}

		internal int ResolveScopedFieldReferenceToIndex(string scopeName, string fieldName)
		{
			DataSet targetDataSetForScopeReference = this.GetTargetDataSetForScopeReference(scopeName);
			if (targetDataSetForScopeReference != null)
			{
				return this.GetIndexForDataSetField(targetDataSetForScopeReference.Name, fieldName);
			}
			return -1;
		}

		internal void CheckScopeReferences(List<ScopeReference> referencedScopes, string propertyName)
		{
			if (referencedScopes != null && referencedScopes.Count != 0)
			{
				if (this.m_currentScope == null || this.m_currentScope.DataScope == null || this.m_currentScope.DataScope is DataSet)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeCollectionReference, Severity.Error, this.m_objectType, this.m_objectName, propertyName);
				}
				else
				{
					IRIFDataScope dataScope = this.m_currentScope.DataScope;
					foreach (ScopeReference referencedScope in referencedScopes)
					{
						IRIFDataScope sourceScope = null;
						DataSet targetDataSet = this.GetTargetDataSetForScopeReference(referencedScope.ScopeName);
						if (targetDataSet != null)
						{
							ScopeTree.DirectedScopeTreeVisitor visitor = delegate(IRIFDataScope scope)
							{
								if (scope.DataScopeInfo != null && scope.DataScopeInfo.DataSet != null && targetDataSet.HasDefaultRelationship(scope.DataScopeInfo.DataSet))
								{
									sourceScope = scope;
									return false;
								}
								return true;
							};
							this.m_scopeTree.Traverse(visitor, dataScope);
						}
						if (sourceScope != null)
						{
							if (referencedScope.HasFieldName)
							{
								this.CheckFieldReference(referencedScope.FieldName, targetDataSet.Name, this.m_objectType, this.m_objectName, propertyName);
							}
							IRIFDataScope iRIFDataScope = default(IRIFDataScope);
							if (this.m_dataSetsForNonStructuralIdc.TryGetValue(targetDataSet.IndexInCollection, out iRIFDataScope))
							{
								if (!sourceScope.DataScopeInfo.IsSameScope(iRIFDataScope.DataScopeInfo))
								{
									this.m_errorContext.Register(ProcessingErrorCode.rsScopeReferenceUsesDataSetMoreThanOnce, Severity.Error, this.m_objectType, this.m_objectName, propertyName, targetDataSet.Name.MarkAsPrivate());
								}
							}
							else
							{
								this.m_dataSetsForNonStructuralIdc.Add(targetDataSet.IndexInCollection, sourceScope);
							}
						}
						else
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeReference, Severity.Error, this.m_objectType, this.m_objectName, propertyName, referencedScope.ScopeName);
						}
					}
				}
			}
		}

		private DataSet GetTargetDataSetForScopeReference(string scopeName)
		{
			return this.m_scopeTree.GetDataSet(scopeName);
		}

		internal void CheckDataSourceReference(List<string> referencedDataSources, string propertyName)
		{
			this.InternalCheckDataSourceReference(referencedDataSources, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckDataSourceReference(List<string> referencedDataSources, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckDataSourceReference(referencedDataSources, objectType, objectName, propertyName);
		}

		private void InternalCheckDataSourceReference(List<string> dataSourceNames, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (dataSourceNames != null && (this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				for (int i = 0; i < dataSourceNames.Count; i++)
				{
					if (!this.m_dataSources.ContainsKey(dataSourceNames[i]))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsDataSourceReference, Severity.Error, objectType, objectName, propertyName, dataSourceNames[i].MarkAsPrivate());
					}
				}
			}
		}

		internal void RegisterGroupWithVariables(ReportHierarchyNode node)
		{
			if (node.Grouping != null && node.Grouping.Variables != null)
			{
				this.m_report.AddGroupWithVariables(node);
			}
		}

		internal void RegisterVariables(List<Variable> variables)
		{
			foreach (Variable variable in variables)
			{
				this.RegisterVariable(variable);
			}
		}

		internal void UnregisterVariables(List<Variable> variables)
		{
			foreach (Variable variable in variables)
			{
				this.UnregisterVariable(variable);
			}
		}

		internal void RegisterVariable(Variable variable)
		{
			if (!this.m_variablesInScope.ContainsKey(variable.Name))
			{
				this.m_variablesInScope.Add(variable.Name, variable);
				SequenceIndex.SetBit(ref this.m_referencableVariables, variable.SequenceID);
			}
		}

		internal void UnregisterVariable(Variable variable)
		{
			this.m_variablesInScope.Remove(variable.Name);
			SequenceIndex.ClearBit(ref this.m_referencableVariables, variable.SequenceID);
		}

		internal void CheckVariableReferences(List<string> referencedVariables, string propertyName)
		{
			this.InternalCheckVariableReferences(referencedVariables, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckVariableReferences(List<string> referencedVariables, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckVariableReferences(referencedVariables, objectType, objectName, propertyName);
		}

		private void InternalCheckVariableReferences(List<string> referencedVariables, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedVariables != null && !this.m_inAutoSubtotalClone)
			{
				for (int i = 0; i < referencedVariables.Count; i++)
				{
					if (!this.m_variablesInScope.ContainsKey(referencedVariables[i]))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidVariableReference, Severity.Error, objectType, objectName, propertyName, "Variable", referencedVariables[i].MarkAsPrivate());
					}
				}
			}
		}

		internal void RegisterTextBoxInScope(TextBox textbox)
		{
			if (this.m_currentDataRegionName != null)
			{
				if (this.m_groupingScopesForRunningValuesInTablix != null && this.m_groupingScopesForRunningValuesInTablix.ContainerName.Equals(this.m_currentDataRegionName))
				{
					string currentColumnScopeName = this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
					string currentRowScopeName = this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
					if (currentColumnScopeName != null || currentRowScopeName != null)
					{
						if (currentColumnScopeName != null)
						{
							ScopeInfo scopeInfo = (ScopeInfo)this.m_groupingScopes[currentColumnScopeName];
							scopeInfo.ReportScope.AddInScopeTextBox(textbox);
						}
						if (currentRowScopeName != null)
						{
							ScopeInfo scopeInfo2 = (ScopeInfo)this.m_groupingScopes[currentRowScopeName];
							scopeInfo2.ReportScope.AddInScopeTextBox(textbox);
						}
					}
					else
					{
						((IRIFReportScope)this.m_groupingScopesForRunningValuesInTablix.ContainerScope).AddInScopeTextBox(textbox);
					}
				}
				else
				{
					this.m_currentScope.ReportScope.AddInScopeTextBox(textbox);
				}
			}
			else
			{
				Global.Tracer.Assert(this.m_currentScope != null, "Top level scope should have been setup as either Page or ReportSection");
				this.m_currentScope.ReportScope.AddInScopeTextBox(textbox);
			}
		}

		internal void RegisterReportItem(ReportItem reportItem)
		{
			if (reportItem != null)
			{
				Pair<ReportItem, int> value = default(Pair<ReportItem, int>);
				if (this.m_reportItemsInScope.TryGetValue(reportItem.Name, out value))
				{
					value.Second++;
				}
				else
				{
					value = new Pair<ReportItem, int>(reportItem, 1);
				}
				this.m_reportItemsInScope[reportItem.Name] = value;
				this.m_reportItemsInSection[reportItem.Name] = reportItem;
				switch (reportItem.ObjectType)
				{
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle:
					this.RegisterReportItems(((Rectangle)reportItem).ReportItems);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix:
					this.RegisterReportItem((Tablix)reportItem);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Textbox:
				{
					int sequenceID = ((TextBox)reportItem).SequenceID;
					SequenceIndex.SetBit(ref this.m_referencableTextboxes, sequenceID);
					SequenceIndex.SetBit(ref this.m_referencableTextboxesInSection, sequenceID);
					break;
				}
				}
			}
		}

		internal void RegisterReportItems(ReportItemCollection reportItems)
		{
			for (int i = 0; i < reportItems.Count; i++)
			{
				this.RegisterReportItem(reportItems[i]);
			}
		}

		private void RegisterReportItems(List<List<TablixCornerCell>> corner)
		{
			if (corner != null)
			{
				foreach (List<TablixCornerCell> item in corner)
				{
					foreach (TablixCornerCell item2 in item)
					{
						this.RegisterReportItems(item2);
					}
				}
			}
		}

		internal void RegisterReportItems(TablixRowList rows)
		{
			foreach (TablixRow row in rows)
			{
				foreach (TablixCell tablixCell in row.TablixCells)
				{
					this.RegisterReportItems(tablixCell);
				}
			}
		}

		private void RegisterReportItems(TablixCellBase cell)
		{
			if (cell.CellContents != null)
			{
				this.RegisterReportItem(cell.CellContents);
				if (cell.AltCellContents != null)
				{
					this.RegisterReportItem(cell.AltCellContents);
				}
			}
		}

		private void RegisterReportItem(Tablix tablix)
		{
			this.RegisterReportItems(tablix.Corner);
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			int num = 0;
			this.RegisterStaticMemberReportItems(tablix.TablixRowMembers, true, list, ref num);
			num = 0;
			this.RegisterStaticMemberReportItems(tablix.TablixColumnMembers, true, list2, ref num);
			if (tablix.TablixRows != null)
			{
				foreach (int item in list)
				{
					if (item < tablix.TablixRows.Count)
					{
						TablixRow tablixRow = tablix.TablixRows[item];
						foreach (int item2 in list2)
						{
							if (item2 < tablixRow.TablixCells.Count)
							{
								TablixCell cell = tablixRow.TablixCells[item2];
								this.RegisterReportItems(cell);
							}
						}
					}
				}
			}
		}

		private void RegisterStaticMemberReportItems(TablixMemberList members, bool register, List<int> indexes, ref int index)
		{
			foreach (TablixMember member in members)
			{
				if (member.Grouping == null)
				{
					this.RegisterMemberReportItems(member, register, true, indexes, ref index);
				}
				else
				{
					this.RegisterMemberReportItems(member, false, true, indexes, ref index);
				}
			}
		}

		internal void RegisterMemberReportItems(TablixMember member, bool firstPass, bool restrictive)
		{
			int num = 0;
			this.RegisterMemberReportItems(member, true, false, null, ref num);
			if (firstPass)
			{
				this.HandleCellContents(member, true, restrictive);
			}
		}

		internal void RegisterMemberReportItems(TablixMember member, bool firstPass)
		{
			this.RegisterMemberReportItems(member, firstPass, true);
		}

		private void HandleCellContents(TablixMember member, bool register)
		{
			this.HandleCellContents(member, register, true);
		}

		private void HandleCellContents(TablixMember member, bool register, bool restrictive)
		{
			Tablix tablix = (Tablix)member.DataRegionDef;
			int num = 0;
			int num2 = tablix.RowCount - 1;
			int num3 = 0;
			int num4 = tablix.ColumnCount - 1;
			if (restrictive)
			{
				if (member.IsColumn)
				{
					num3 = member.CellStartIndex;
					num4 = member.CellEndIndex;
				}
				else
				{
					num = member.CellStartIndex;
					num2 = member.CellEndIndex;
				}
			}
			if (!restrictive && this.m_handledCellContents.Value)
			{
				if (register)
				{
					return;
				}
				if (member.IsColumn)
				{
					if (member.CellStartIndex < num4 && member.CellEndIndex < num4)
					{
						return;
					}
				}
				else if (member.CellStartIndex < num2 && member.CellEndIndex < num2)
				{
					return;
				}
			}
			if (tablix.TablixRows != null)
			{
				for (int i = num; i <= num2; i++)
				{
					if (i < tablix.TablixRows.Count)
					{
						TablixRow tablixRow = tablix.TablixRows[i];
						for (int j = num3; j <= num4; j++)
						{
							if (j < tablixRow.TablixCells.Count)
							{
								TablixCell tablixCell = tablixRow.TablixCells[j];
								if (tablixCell != null)
								{
									if (register)
									{
										this.RegisterReportItems(tablixCell);
									}
									else
									{
										this.UnRegisterReportItems(tablixCell);
									}
								}
							}
						}
					}
				}
				if (!restrictive)
				{
					this.m_handledCellContents.Value = ((byte)(register ? 1 : 0) != 0);
				}
			}
		}

		private void RegisterMemberReportItems(TablixMember member, bool register, bool registerStatic, List<int> indexes, ref int index)
		{
			if (register && (registerStatic || member.Grouping != null) && member.TablixHeader != null && member.TablixHeader.CellContents != null)
			{
				this.RegisterReportItem(member.TablixHeader.CellContents);
				if (member.TablixHeader.AltCellContents != null)
				{
					this.RegisterReportItem(member.TablixHeader.AltCellContents);
				}
			}
			if (member.SubMembers != null)
			{
				this.RegisterStaticMemberReportItems(member.SubMembers, register, indexes, ref index);
			}
			else if (indexes != null)
			{
				if (register && member.Grouping == null)
				{
					indexes.Add(index);
				}
				index++;
			}
		}

		internal void UnRegisterReportItem(ReportItem reportItem)
		{
			if (reportItem != null)
			{
				Pair<ReportItem, int> value = this.m_reportItemsInScope[reportItem.Name];
				value.Second--;
				bool flag = value.Second == 0;
				if (flag)
				{
					this.m_reportItemsInScope.Remove(reportItem.Name);
				}
				else
				{
					this.m_reportItemsInScope[reportItem.Name] = value;
				}
				switch (reportItem.ObjectType)
				{
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle:
					this.UnRegisterReportItems(((Rectangle)reportItem).ReportItems);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix:
					this.UnRegisterReportItem((Tablix)reportItem);
					break;
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Textbox:
					if (flag)
					{
						SequenceIndex.ClearBit(ref this.m_referencableTextboxes, ((TextBox)reportItem).SequenceID);
					}
					break;
				}
			}
		}

		private void UnRegisterReportItems(List<List<TablixCornerCell>> corner)
		{
			if (corner != null)
			{
				foreach (List<TablixCornerCell> item in corner)
				{
					foreach (TablixCornerCell item2 in item)
					{
						this.UnRegisterReportItems(item2);
					}
				}
			}
		}

		internal void UnRegisterReportItems(TablixRowList rows)
		{
			foreach (TablixRow row in rows)
			{
				foreach (TablixCell tablixCell in row.TablixCells)
				{
					this.UnRegisterReportItems(tablixCell);
				}
			}
		}

		private void UnRegisterReportItems(TablixCellBase cell)
		{
			if (cell.CellContents != null)
			{
				this.UnRegisterReportItem(cell.CellContents);
				if (cell.AltCellContents != null)
				{
					this.UnRegisterReportItem(cell.AltCellContents);
				}
			}
		}

		internal void UnRegisterReportItem(Tablix tablix)
		{
			this.UnRegisterReportItems(tablix.Corner);
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			int num = 0;
			this.UnRegisterStaticMemberReportItems(tablix.TablixRowMembers, true, list, ref num);
			num = 0;
			this.UnRegisterStaticMemberReportItems(tablix.TablixColumnMembers, true, list2, ref num);
			if (tablix.TablixRows != null)
			{
				foreach (int item in list)
				{
					if (item < tablix.TablixRows.Count)
					{
						TablixRow tablixRow = tablix.TablixRows[item];
						foreach (int item2 in list2)
						{
							if (item2 < tablixRow.TablixCells.Count)
							{
								TablixCell cell = tablixRow.TablixCells[item2];
								this.UnRegisterReportItems(cell);
							}
						}
					}
				}
			}
		}

		private void UnRegisterStaticMemberReportItems(TablixMemberList members, bool unregister, List<int> indexes, ref int index)
		{
			foreach (TablixMember member in members)
			{
				if (member.Grouping == null)
				{
					this.UnRegisterMemberReportItems(member, unregister, true, indexes, ref index);
				}
				else
				{
					this.UnRegisterMemberReportItems(member, false, true, indexes, ref index);
				}
			}
		}

		internal void UnRegisterMemberReportItems(TablixMember member, bool firstPass)
		{
			this.UnRegisterMemberReportItems(member, firstPass, true);
		}

		internal void UnRegisterMemberReportItems(TablixMember member, bool firstPass, bool restrictive)
		{
			int num = 0;
			this.UnRegisterMemberReportItems(member, true, false, null, ref num);
			if (firstPass)
			{
				this.HandleCellContents(member, false, restrictive);
			}
		}

		private void UnRegisterMemberReportItems(TablixMember member, bool unregister, bool unregisterStatic, List<int> indexes, ref int index)
		{
			if (unregister && (unregisterStatic || member.Grouping != null) && member.TablixHeader != null && member.TablixHeader.CellContents != null)
			{
				this.UnRegisterReportItem(member.TablixHeader.CellContents);
				if (member.TablixHeader.AltCellContents != null)
				{
					this.UnRegisterReportItem(member.TablixHeader.AltCellContents);
				}
			}
			if (member.SubMembers != null)
			{
				this.UnRegisterStaticMemberReportItems(member.SubMembers, unregister, indexes, ref index);
			}
			else if (indexes != null)
			{
				if (unregister && member.Grouping == null)
				{
					indexes.Add(index);
				}
				index++;
			}
		}

		internal void UnRegisterReportItems(ReportItemCollection reportItems)
		{
			Global.Tracer.Assert(null != reportItems);
			for (int i = 0; i < reportItems.Count; i++)
			{
				this.UnRegisterReportItem(reportItems[i]);
			}
		}

		internal void CheckReportItemReferences(List<string> referencedReportItems, string propertyName)
		{
			this.InternalCheckReportItemReferences(referencedReportItems, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckReportItemReferences(List<string> referencedReportItems, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckReportItemReferences(referencedReportItems, objectType, objectName, propertyName);
		}

		private void InternalCheckReportItemReferences(List<string> referencedReportItems, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedReportItems != null)
			{
				if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
				{
					for (int i = 0; i < referencedReportItems.Count; i++)
					{
						if (!this.m_reportItemsInSection.ContainsKey(referencedReportItems[i]))
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsReportItemReferenceInPageSection, Severity.Error, objectType, objectName, propertyName, referencedReportItems[i]);
						}
					}
				}
				else
				{
					for (int j = 0; j < referencedReportItems.Count; j++)
					{
						if (!this.m_reportItemsInScope.ContainsKey(referencedReportItems[j]))
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsReportItemReference, Severity.Error, objectType, objectName, propertyName, referencedReportItems[j]);
						}
					}
				}
			}
		}

		internal byte[] GetCurrentReferencableVariables()
		{
			if (this.m_referencableVariables == null)
			{
				return null;
			}
			return this.m_referencableVariables.Clone() as byte[];
		}

		internal byte[] GetCurrentReferencableTextboxes()
		{
			if (this.m_referencableTextboxes == null)
			{
				return null;
			}
			return this.m_referencableTextboxes.Clone() as byte[];
		}

		internal byte[] GetCurrentReferencableTextboxesInSection()
		{
			if (this.m_referencableTextboxesInSection == null)
			{
				return null;
			}
			return this.m_referencableTextboxesInSection.Clone() as byte[];
		}

		internal void CheckReportParameterReferences(List<string> referencedParameters, string propertyName)
		{
			this.InternalCheckReportParameterReferences(referencedParameters, this.m_objectType, this.m_objectName, propertyName);
		}

		private void InternalCheckReportParameterReferences(List<string> referencedParameters, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedParameters != null)
			{
				for (int i = 0; i < referencedParameters.Count; i++)
				{
					if (this.m_parameters == null || !this.m_parameters.ContainsKey(referencedParameters[i]))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsParameterReference, Severity.Error, objectType, objectName, propertyName, referencedParameters[i].MarkAsPrivate());
					}
				}
			}
		}

		internal VisibilityToggleInfo RegisterVisibilityToggle(Visibility visibility)
		{
			if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InPageSection) != 0)
			{
				return null;
			}
			VisibilityToggleInfo visibilityToggleInfo = null;
			if (visibility.IsToggleReceiver)
			{
				visibilityToggleInfo = new VisibilityToggleInfo();
				visibilityToggleInfo.ObjectName = this.m_objectName;
				visibilityToggleInfo.ObjectType = this.m_objectType;
				visibilityToggleInfo.Visibility = visibility;
				visibilityToggleInfo.GroupName = this.m_currentGroupName;
				visibilityToggleInfo.GroupingSet = (Hashtable)this.m_groupingScopes.Clone();
				this.m_visibilityToggleInfos.Add(visibilityToggleInfo);
			}
			return visibilityToggleInfo;
		}

		internal bool RegisterVisibility(Visibility visibility, IVisibilityOwner owner)
		{
			IVisibilityOwner visibilityOwner = default(IVisibilityOwner);
			IVisibilityOwner visibilityOwner2 = default(IVisibilityOwner);
			IVisibilityOwner visibilityOwner3 = default(IVisibilityOwner);
			if (this.NeedVisibilityLink(visibility, owner, out visibilityOwner, out visibilityOwner2, out visibilityOwner3))
			{
				VisibilityContainmentInfo visibilityContainmentInfo = new VisibilityContainmentInfo();
				if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixColumnHierarchy) != 0 && visibilityOwner3 != null)
				{
					owner.ContainingDynamicVisibility = visibilityOwner3;
				}
				else if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixRowHierarchy) != 0 && visibilityOwner2 != null)
				{
					owner.ContainingDynamicVisibility = visibilityOwner2;
				}
				else if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixCell) != 0 && (visibilityOwner3 != null || visibilityOwner2 != null))
				{
					owner.ContainingDynamicColumnVisibility = visibilityOwner3;
					owner.ContainingDynamicRowVisibility = visibilityOwner2;
				}
				else if (visibilityOwner != null)
				{
					owner.ContainingDynamicVisibility = visibilityOwner;
				}
				if (owner.GetObjectType() == AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixMember)
				{
					TablixMember tablixMember = (TablixMember)owner;
					if (!tablixMember.IsAutoSubtotal)
					{
						if (tablixMember.IsColumn)
						{
							visibilityContainmentInfo.ContainingColumnVisibility = owner;
							visibilityContainmentInfo.ContainingRowVisibility = visibilityOwner2;
						}
						else
						{
							visibilityContainmentInfo.ContainingRowVisibility = owner;
						}
					}
				}
				else
				{
					visibilityContainmentInfo.ContainingVisibility = owner;
				}
				this.m_visibilityContainmentInfos.Push(visibilityContainmentInfo);
				return true;
			}
			return false;
		}

		internal void UnRegisterVisibility(Visibility visibility, IVisibilityOwner owner)
		{
			this.m_visibilityContainmentInfos.Pop();
		}

		internal bool NeedVisibilityLink(Visibility visibility, IVisibilityOwner owner, out IVisibilityOwner outerContainer, out IVisibilityOwner outerRowContainer, out IVisibilityOwner outerColumnContainer)
		{
			outerContainer = null;
			outerRowContainer = null;
			outerColumnContainer = null;
			if (this.m_visibilityContainmentInfos.Count > 0)
			{
				VisibilityContainmentInfo visibilityContainmentInfo = this.m_visibilityContainmentInfos.Peek();
				outerContainer = visibilityContainmentInfo.ContainingVisibility;
				outerRowContainer = visibilityContainmentInfo.ContainingRowVisibility;
				outerColumnContainer = visibilityContainmentInfo.ContainingColumnVisibility;
			}
			bool flag = outerRowContainer != null || outerColumnContainer != null;
			if ((owner.GetObjectType() != AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tablix || !flag) && owner.GetObjectType() != AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TextBox)
			{
				if (visibility != null)
				{
					if (!visibility.IsConditional)
					{
						return visibility.IsToggleReceiver;
					}
					return true;
				}
				return false;
			}
			return true;
		}

		internal void RegisterToggleItem(TextBox textbox)
		{
			ToggleItemInfo toggleItemInfo = new ToggleItemInfo();
			toggleItemInfo.Textbox = textbox;
			toggleItemInfo.GroupName = this.m_currentGroupName;
			toggleItemInfo.GroupingSet = (Hashtable)this.m_groupingScopes.Clone();
			this.m_toggleItems.Add(textbox.Name, toggleItemInfo);
		}

		internal void ValidateToggleItems()
		{
			foreach (VisibilityToggleInfo visibilityToggleInfo in this.m_visibilityToggleInfos)
			{
				bool flag = false;
				ToggleItemInfo toggleItemInfo = null;
				if (this.m_toggleItems.ContainsKey(visibilityToggleInfo.Visibility.Toggle))
				{
					toggleItemInfo = this.m_toggleItems[visibilityToggleInfo.Visibility.Toggle];
					Hashtable groupingSet = toggleItemInfo.GroupingSet;
					Hashtable groupingSet2 = visibilityToggleInfo.GroupingSet;
					flag = this.ContainsSubsetOfKeys(groupingSet2, groupingSet);
					ScopeInfo scopeInfo = null;
					if (visibilityToggleInfo.GroupName != null)
					{
						scopeInfo = (ScopeInfo)groupingSet2[visibilityToggleInfo.GroupName];
					}
					if (!flag || (visibilityToggleInfo.IsTablixMember && scopeInfo != null && scopeInfo.GroupingScope.Parent != null && this.IsTargetVisibilityOnContainmentChain(toggleItemInfo.Textbox, visibilityToggleInfo.Visibility)))
					{
						if (visibilityToggleInfo.GroupName != null && toggleItemInfo.GroupName != null && visibilityToggleInfo.GroupName == toggleItemInfo.GroupName)
						{
							Global.Tracer.Assert(groupingSet.Contains(toggleItemInfo.GroupName), "(toggleItemGroupingSet.Contains(toggleItem.GroupName))");
							ScopeInfo scopeInfo2 = (ScopeInfo)groupingSet[toggleItemInfo.GroupName];
							if (scopeInfo2.GroupingScope.Parent != null)
							{
								flag = true;
								TextBox textbox = toggleItemInfo.Textbox;
								textbox.RecursiveSender = true;
								textbox.RecursiveMember = (TablixMember)scopeInfo2.ReportScope;
								Visibility visibility = visibilityToggleInfo.Visibility;
								visibility.RecursiveReceiver = true;
								if (scopeInfo != null)
								{
									visibility.RecursiveMember = (TablixMember)scopeInfo.ReportScope;
								}
							}
						}
					}
					else
					{
						ReportItem parent = toggleItemInfo.Textbox.Parent;
						while (parent != null)
						{
							if (parent.Visibility != visibilityToggleInfo.Visibility)
							{
								parent = parent.Parent;
								continue;
							}
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					TextBox textbox2 = toggleItemInfo.Textbox;
					textbox2.IsToggle = true;
					if (!visibilityToggleInfo.Visibility.RecursiveReceiver)
					{
						textbox2.HasNonRecursiveSender = true;
					}
					visibilityToggleInfo.Visibility.ToggleSender = textbox2;
					ReportItem reportItem = textbox2;
					do
					{
						reportItem.Computed = true;
						reportItem = reportItem.Parent;
					}
					while (reportItem is Rectangle);
				}
				else if (visibilityToggleInfo.Visibility.IsClone)
				{
					visibilityToggleInfo.Visibility.Toggle = null;
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidToggleItem, Severity.Error, visibilityToggleInfo.ObjectType, visibilityToggleInfo.ObjectName, "Item", visibilityToggleInfo.Visibility.Toggle);
				}
			}
			this.m_toggleItems.Clear();
			this.m_visibilityToggleInfos.Clear();
		}

		private bool IsTargetVisibilityOnContainmentChain(IVisibilityOwner visibilityOwner, Visibility targetVisibility)
		{
			if (visibilityOwner.Visibility == targetVisibility)
			{
				return true;
			}
			if (visibilityOwner.ContainingDynamicVisibility != null)
			{
				return this.IsTargetVisibilityOnContainmentChain(visibilityOwner.ContainingDynamicVisibility, targetVisibility);
			}
			bool flag = false;
			if (visibilityOwner.ContainingDynamicRowVisibility != null)
			{
				flag = this.IsTargetVisibilityOnContainmentChain(visibilityOwner.ContainingDynamicRowVisibility, targetVisibility);
			}
			if (!flag && visibilityOwner.ContainingDynamicColumnVisibility != null)
			{
				flag = this.IsTargetVisibilityOnContainmentChain(visibilityOwner.ContainingDynamicColumnVisibility, targetVisibility);
			}
			return flag;
		}

		private bool ContainsSubsetOfKeys(Hashtable set, Hashtable subSet)
		{
			int num = 0;
			foreach (object key in subSet.Keys)
			{
				if (!set.ContainsKey(key))
				{
					return false;
				}
				num++;
			}
			return num == subSet.Keys.Count;
		}

		internal void ValidateHeaderSize(double size, int startLevel, int span, bool isColumnHierarchy, int cellIndex)
		{
			double headerSize = this.GetHeaderSize(isColumnHierarchy, startLevel, span);
			double second = Math.Round(headerSize, 4);
			double first = Math.Round(size, 4);
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareDoubles(first, second) != 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidTablixHeaderSize, Severity.Error, this.m_objectType, this.m_objectName, isColumnHierarchy ? "TablixColumnHierarchy" : "TablixRowHierarchy", "TablixHeader.Size", (cellIndex + 1).ToString(CultureInfo.InvariantCulture.NumberFormat), second.ToString(CultureInfo.InvariantCulture.NumberFormat), first.ToString(CultureInfo.InvariantCulture.NumberFormat), isColumnHierarchy ? "TablixColumn" : "TablixRow");
			}
		}

		internal double GetTotalHeaderSize(bool isColumnHierarchy, int span)
		{
			return this.GetHeaderSize(isColumnHierarchy, 0, span);
		}

		internal double GetHeaderSize(bool isColumnHierarchy, int startLevel, int span)
		{
			double num = 0.0;
			IList<Pair<double, int>> headerLevelSizeList;
			if (isColumnHierarchy)
			{
				Global.Tracer.Assert(this.m_columnHeaderLevelSizeList != null, "(m_columnHeaderLevelSizeList != null)");
				headerLevelSizeList = this.m_columnHeaderLevelSizeList;
			}
			else
			{
				Global.Tracer.Assert(this.m_rowHeaderLevelSizeList != null, "(m_rowHeaderLevelSizeList != null)");
				headerLevelSizeList = this.m_rowHeaderLevelSizeList;
			}
			return this.GetHeaderSize(headerLevelSizeList, startLevel, span);
		}

		internal double GetHeaderSize(IList<Pair<double, int>> headerLevelSizeList, int startingLevel, int spans)
		{
			int level = startingLevel + spans;
			startingLevel = this.FindEntryForLevel(headerLevelSizeList, startingLevel);
			level = this.FindEntryForLevel(headerLevelSizeList, level);
			return headerLevelSizeList[level].First - headerLevelSizeList[startingLevel].First;
		}

		private int FindEntryForLevel(IList<Pair<double, int>> headerLevelSizeList, int level)
		{
			if (level > 0)
			{
				int num = -1;
				for (int i = 0; i < headerLevelSizeList.Count; i++)
				{
					num += headerLevelSizeList[i].Second + 1;
					if (num >= level)
					{
						return i;
					}
				}
			}
			return 0;
		}

		internal double ValidateSize(string size, string propertyName)
		{
			double result = default(double);
			string text = default(string);
			AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateSize(size, this.m_objectType, this.m_objectName, propertyName, true, this.m_errorContext, out result, out text);
			return result;
		}

		internal double ValidateSize(ref string size, string propertyName)
		{
			return this.ValidateSize(ref size, true, propertyName);
		}

		internal double ValidateSize(ref string size, bool restrictMaxValue, string propertyName)
		{
			double result = default(double);
			string text = default(string);
			AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateSize(size, this.m_objectType, this.m_objectName, propertyName, restrictMaxValue, this.m_errorContext, out result, out text);
			size = text;
			return result;
		}

		internal void CheckInternationalSettings(Dictionary<string, AttributeInfo> styleAttributes)
		{
			if (styleAttributes != null && styleAttributes.Count != 0)
			{
				CultureInfo cultureInfo = null;
				AttributeInfo attributeInfo = default(AttributeInfo);
				if (!styleAttributes.TryGetValue("Language", out attributeInfo))
				{
					cultureInfo = this.m_reportLanguage;
				}
				else if (!attributeInfo.IsExpression)
				{
					AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateLanguage(attributeInfo.Value, this.ObjectType, this.ObjectName, "Language", this.ErrorContext, out cultureInfo);
				}
				AttributeInfo attributeInfo2 = default(AttributeInfo);
				if (cultureInfo != null && styleAttributes.TryGetValue("Calendar", out attributeInfo2) && !attributeInfo2.IsExpression)
				{
					AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateCalendar(cultureInfo, attributeInfo2.Value, this.ObjectType, this.ObjectName, "Calendar", this.ErrorContext);
				}
				if (styleAttributes.TryGetValue("NumeralLanguage", out attributeInfo))
				{
					if (attributeInfo.IsExpression)
					{
						cultureInfo = null;
					}
					else
					{
						AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateLanguage(attributeInfo.Value, this.ObjectType, this.ObjectName, "NumeralLanguage", this.ErrorContext, out cultureInfo);
					}
				}
				AttributeInfo attributeInfo3 = default(AttributeInfo);
				if (cultureInfo != null && styleAttributes.TryGetValue("NumeralVariant", out attributeInfo3) && !attributeInfo3.IsExpression)
				{
					AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateNumeralVariant(cultureInfo, attributeInfo3.IntValue, this.ObjectType, this.ObjectName, "NumeralVariant", this.ErrorContext);
				}
				if (styleAttributes.TryGetValue("CurrencyLanguage", out attributeInfo) && !attributeInfo.IsExpression)
				{
					AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateLanguage(attributeInfo.Value, this.ObjectType, this.ObjectName, "CurrencyLanguage", this.ErrorContext, out cultureInfo);
				}
			}
		}

		internal string GetCurrentScopeName()
		{
			Global.Tracer.Assert(null != this.m_currentScope, "Missing ScopeInfo for current scope.");
			if (this.m_currentScope.IsTopLevelScope)
			{
				return "0_ReportScope";
			}
			if (this.m_currentScope.GroupingScope != null)
			{
				return this.m_currentScope.GroupingScope.Name;
			}
			if (!this.m_isDataRegionScopedCell)
			{
				if (!this.m_groupingScopesForRunningValuesInTablix.IsRunningValueDirectionColumn)
				{
					if (this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null)
					{
						return this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
					}
					if (this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
					{
						return this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
					}
				}
				else
				{
					if (this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName != null)
					{
						return this.m_groupingScopesForRunningValuesInTablix.CurrentRowScopeName;
					}
					if (this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName != null)
					{
						return this.m_groupingScopesForRunningValuesInTablix.CurrentColumnScopeName;
					}
				}
			}
			return this.m_currentDataRegionName;
		}

		internal bool IsScope(string scope)
		{
			if (scope == null)
			{
				return false;
			}
			return this.m_reportScopes.ContainsKey(scope);
		}

		internal bool IsAncestorScope(string targetScope)
		{
			string dataSetName = this.GetDataSetName();
			if (dataSetName != null && AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(dataSetName, targetScope, false) == 0)
			{
				goto IL_0041;
			}
			if (this.m_dataregionScopes != null && this.m_dataregionScopes.ContainsKey(targetScope))
			{
				goto IL_0041;
			}
			if (this.m_groupingScopesForRunningValuesInTablix != null)
			{
				return this.m_groupingScopesForRunningValuesInTablix.ContainsScope(targetScope);
			}
			return false;
			IL_0041:
			return true;
		}

		internal bool IsSameOrChildScope(string parentScope, string childScope)
		{
			if (parentScope == childScope)
			{
				return true;
			}
			if (this.m_datasetScopes.ContainsKey(parentScope))
			{
				if (((ScopeInfo)this.m_datasetScopes[parentScope]).DataSetScope == this.m_reportScopeDatasets[childScope])
				{
					return true;
				}
			}
			else if (this.m_dataregionScopes.ContainsKey(parentScope))
			{
				ReportItem reportItem = null;
				ReportItem reportItem2 = null;
				if (this.m_dataregionScopes.ContainsKey(childScope))
				{
					reportItem2 = ((ScopeInfo)this.m_dataregionScopes[childScope]).DataRegionScope;
					reportItem = ((ScopeInfo)this.m_dataregionScopes[parentScope]).DataRegionScope;
				}
				else if (this.m_groupingScopes.ContainsKey(childScope))
				{
					reportItem = ((ScopeInfo)this.m_dataregionScopes[parentScope]).DataRegionScope;
					reportItem2 = ((ScopeInfo)this.m_groupingScopes[childScope]).GroupingScope.Owner.DataRegionDef;
					if (reportItem2 == reportItem)
					{
						return true;
					}
				}
				while (reportItem2.Parent != null)
				{
					if (reportItem2.Parent == reportItem)
					{
						return true;
					}
					reportItem2 = reportItem2.Parent;
				}
			}
			else if (this.m_groupingScopes.ContainsKey(parentScope))
			{
				if (this.m_dataregionScopes.ContainsKey(childScope))
				{
					ReportItem reportItem3 = ((ScopeInfo)this.m_dataregionScopes[childScope]).DataRegionScope;
					ReportItem dataRegionDef = ((ScopeInfo)this.m_groupingScopes[parentScope]).GroupingScope.Owner.DataRegionDef;
					while (reportItem3.Parent != null)
					{
						if (reportItem3.Parent == dataRegionDef)
						{
							return true;
						}
						reportItem3 = reportItem3.Parent;
					}
				}
				else if (this.m_groupingScopes.ContainsKey(childScope))
				{
					return this.GetScopeChainInfo().IsSameOrChildScope(parentScope, childScope);
				}
			}
			return false;
		}

		internal bool IsCurrentScope(string targetScope)
		{
			return targetScope == this.GetCurrentScopeName();
		}

		internal bool HasPeerGroups(DataRegion dataRegion)
		{
			bool flag = default(bool);
			if (this.HasPeerGroups(dataRegion.RowMembers, out flag))
			{
				return true;
			}
			return this.HasPeerGroups(dataRegion.ColumnMembers, out flag);
		}

		private bool HasPeerGroups(HierarchyNodeList nodes, out bool hasGroup)
		{
			hasGroup = false;
			if (nodes != null)
			{
				foreach (ReportHierarchyNode node in nodes)
				{
					bool flag = false;
					if (node.InnerHierarchy != null && this.HasPeerGroups(node.InnerHierarchy, out flag))
					{
						hasGroup = true;
						return true;
					}
					if (!node.IsGroup && !flag)
					{
						continue;
					}
					if (hasGroup)
					{
						return true;
					}
					hasGroup = true;
				}
			}
			return false;
		}

		internal bool IsPeerScope(string targetScope)
		{
			if (!this.m_hasUserSortPeerScopes)
			{
				return false;
			}
			string currentScopeName = this.GetCurrentScopeName();
			Global.Tracer.Assert(currentScopeName != null && null != this.m_peerScopes, "(null != currentScope && null != m_peerScopes)");
			object obj = this.m_peerScopes[currentScopeName];
			int num = 0;
			int num2 = 0;
			if (obj == null)
			{
				return false;
			}
			num = (int)obj;
			obj = this.m_peerScopes[targetScope];
			if (obj == null)
			{
				return false;
			}
			num2 = (int)obj;
			return num == num2;
		}

		internal bool IsReportTopLevelScope()
		{
			if (this.m_currentScope != null)
			{
				return this.m_currentScope.IsTopLevelScope;
			}
			return true;
		}

		internal ISortFilterScope GetSortFilterScope()
		{
			return this.GetSortFilterScope(this.GetCurrentScopeName());
		}

		internal ISortFilterScope GetSortFilterScope(string scopeName)
		{
			Global.Tracer.Assert(scopeName != null && "0_ReportScope" != scopeName && this.m_reportScopes.ContainsKey(scopeName));
			return this.m_reportScopes[scopeName];
		}

		internal void RegisterPeerScopes(ReportItemCollection reportItems)
		{
			this.RegisterPeerScopes(reportItems, ++this.m_lastPeerScopeId, true);
		}

		private void RegisterPeerScopes(TablixMemberList members, int scopeID)
		{
			if (members != null)
			{
				foreach (TablixMember member in members)
				{
					if (member.Grouping == null)
					{
						if (member.TablixHeader != null && member.TablixHeader.CellContents != null)
						{
							this.RegisterPeerScope(member.TablixHeader.CellContents, scopeID, false);
							if (member.TablixHeader.AltCellContents != null)
							{
								this.RegisterPeerScope(member.TablixHeader.AltCellContents, scopeID, false);
							}
						}
						if (member.SubMembers != null)
						{
							this.RegisterPeerScopes(member.SubMembers, scopeID);
						}
					}
				}
			}
		}

		private void RegisterPeerScopes(List<List<TablixCornerCell>> cornerCells, int scopeID)
		{
			if (cornerCells != null)
			{
				foreach (List<TablixCornerCell> cornerCell in cornerCells)
				{
					foreach (TablixCornerCell item in cornerCell)
					{
						if (item.CellContents != null)
						{
							this.RegisterPeerScope(item.CellContents, scopeID, false);
							if (item.AltCellContents != null)
							{
								this.RegisterPeerScope(item.AltCellContents, scopeID, false);
							}
						}
					}
				}
			}
		}

		private void RegisterPeerScopes(ReportItemCollection reportItems, int scopeID, bool traverse)
		{
			if (reportItems != null && this.m_hasUserSortPeerScopes)
			{
				string currentScopeName = this.GetCurrentScopeName();
				if (!this.m_peerScopes.ContainsKey(currentScopeName))
				{
					this.InternalRegisterPeerScopes(reportItems, scopeID, traverse);
					if (!this.m_peerScopes.ContainsKey(currentScopeName))
					{
						this.m_peerScopes.Add(currentScopeName, scopeID);
					}
				}
			}
		}

		private void InternalRegisterPeerScopes(ReportItemCollection reportItems, int scopeID, bool traverse)
		{
			if (reportItems != null)
			{
				int count = reportItems.Count;
				for (int i = 0; i < count; i++)
				{
					ReportItem item = reportItems[i];
					this.RegisterPeerScope(item, scopeID, traverse);
				}
			}
		}

		private void RegisterPeerScope(ReportItem item, int scopeID, bool traverse)
		{
			if (item is Rectangle)
			{
				this.InternalRegisterPeerScopes(((Rectangle)item).ReportItems, scopeID, traverse);
			}
			else if (item.IsDataRegion && !this.m_peerScopes.ContainsKey(item.Name))
			{
				this.m_peerScopes.Add(item.Name, scopeID);
			}
			if (traverse && item is Tablix)
			{
				this.RegisterPeerScopes(((Tablix)item).Corner, scopeID);
				this.RegisterPeerScopes(((Tablix)item).TablixColumnMembers, scopeID);
				this.RegisterPeerScopes(((Tablix)item).TablixRowMembers, scopeID);
			}
		}

		private void RegisterUserSortInnerScope(IInScopeEventSource eventSource)
		{
			string sortExpressionScopeString = eventSource.UserSort.SortExpressionScopeString;
			List<IInScopeEventSource> list = default(List<IInScopeEventSource>);
			if (this.m_groupingScopes.ContainsKey(sortExpressionScopeString) && ((ScopeInfo)this.m_groupingScopes[sortExpressionScopeString]).GroupingScope.IsDetail)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidSortExpressionScope, Severity.Error, eventSource.ObjectType, eventSource.Name, "SortExpressionScope", sortExpressionScopeString);
				eventSource.UserSort.SortExpressionScopeString = null;
			}
			else if (this.m_userSortExpressionScopes.TryGetValue(sortExpressionScopeString, out list))
			{
				list.Add(eventSource);
			}
			else
			{
				ISortFilterScope sortFilterScope = null;
				if (this.m_reportScopes.TryGetValue(sortExpressionScopeString, out sortFilterScope) && sortFilterScope is Grouping && ((Grouping)sortFilterScope).DomainScope != null)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidSortExpressionScopeDomainScope, Severity.Error, eventSource.ObjectType, eventSource.Name, "SortExpressionScope", sortExpressionScopeString);
				}
				list = new List<IInScopeEventSource>();
				list.Add(eventSource);
				this.m_userSortExpressionScopes.Add(sortExpressionScopeString, list);
			}
		}

		private void UnregisterUserSortInnerScope(string sortExpressionScopeString, IInScopeEventSource eventSource)
		{
			List<IInScopeEventSource> list = default(List<IInScopeEventSource>);
			if (this.m_userSortExpressionScopes.TryGetValue(sortExpressionScopeString, out list))
			{
				list.Remove(eventSource);
			}
		}

		internal void ProcessUserSortScopes(string scopeName)
		{
			if (this.m_hasUserSorts)
			{
				List<IInScopeEventSource> list = default(List<IInScopeEventSource>);
				if (this.m_userSortExpressionScopes.TryGetValue(scopeName, out list))
				{
					int count = list.Count;
					for (int num = count - 1; num >= 0; num--)
					{
						IInScopeEventSource inScopeEventSource = list[num];
						Global.Tracer.Assert(null != inScopeEventSource.UserSort, "(null != eventSource.UserSort)");
						if (this.m_groupingScopes.ContainsKey(scopeName) && ((ScopeInfo)this.m_groupingScopes[scopeName]).GroupingScope.IsDetail)
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidSortExpressionScope, Severity.Error, inScopeEventSource.ObjectType, inScopeEventSource.Name, "SortExpressionScope", scopeName);
							inScopeEventSource.UserSort.SortExpressionScopeString = null;
						}
						else
						{
							inScopeEventSource.ScopeChainInfo = this.GetScopeChainInfo();
							inScopeEventSource.UserSort.SortExpressionScope = this.GetSortFilterScope(inScopeEventSource.UserSort.SortExpressionScopeString);
							this.InitializeSortExpression(inScopeEventSource, false);
						}
						list.RemoveAt(num);
					}
					this.m_userSortExpressionScopes.Remove(scopeName);
				}
				if (this.m_userSortEventSources.TryGetValue(scopeName, out list))
				{
					int count2 = list.Count;
					for (int num2 = count2 - 1; num2 >= 0; num2--)
					{
						IInScopeEventSource inScopeEventSource2 = list[num2];
						Global.Tracer.Assert(null != inScopeEventSource2.UserSort, "(null != eventSource.UserSort)");
						if (inScopeEventSource2.UserSort.SortTarget != null)
						{
							inScopeEventSource2.UserSort.DataSet = (DataSet)this.m_reportScopeDatasets[inScopeEventSource2.UserSort.SortTarget.ScopeName];
							if (inScopeEventSource2.UserSort.SortExpressionScopeString != null)
							{
								if (inScopeEventSource2.UserSort.SortExpressionScope != null)
								{
									if (this.m_reportScopeDatasets[inScopeEventSource2.UserSort.SortExpressionScope.ScopeName] != inScopeEventSource2.UserSort.DataSet)
									{
										this.m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScopeDataSet, Severity.Error, inScopeEventSource2.ObjectType, inScopeEventSource2.Name, "SortExpressionScope", inScopeEventSource2.UserSort.SortExpressionScope.ScopeName, "SortTarget");
									}
									else
									{
										ScopeChainInfo scopeChainInfo = this.GetScopeChainInfo();
										Grouping fromGroup = null;
										if (scopeChainInfo != null)
										{
											fromGroup = scopeChainInfo.GetInnermostGrouping();
										}
										if (inScopeEventSource2.ScopeChainInfo != null)
										{
											inScopeEventSource2.UserSort.GroupsInSortTarget = inScopeEventSource2.ScopeChainInfo.GetGroupsFromCurrentTablixAxisToGrouping(fromGroup);
										}
										if (inScopeEventSource2.ContainingScopes != null)
										{
											int num3 = inScopeEventSource2.ContainingScopes.Count - 1;
											if (0 <= num3 && string.CompareOrdinal(inScopeEventSource2.UserSort.SortExpressionScopeString, inScopeEventSource2.ContainingScopes[num3].Name) == 0)
											{
												this.m_errorContext.Register(ProcessingErrorCode.rsIneffectiveSortExpressionScope, Severity.Warning, inScopeEventSource2.ObjectType, inScopeEventSource2.Name, "SortExpressionScope", inScopeEventSource2.UserSort.SortExpressionScopeString);
											}
										}
									}
								}
								else
								{
									this.m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Error, inScopeEventSource2.ObjectType, inScopeEventSource2.Name, "SortExpressionScope", inScopeEventSource2.UserSort.SortExpressionScopeString);
								}
							}
							if (!this.m_errorContext.HasError)
							{
								this.AddToScopeSortFilterList(inScopeEventSource2);
							}
						}
						list.RemoveAt(num2);
					}
					this.m_userSortEventSources.Remove(scopeName);
				}
			}
		}

		internal void RegisterSortEventSource(IInScopeEventSource eventSource)
		{
			if (this.m_hasUserSorts && eventSource != null && eventSource.UserSort != null)
			{
				if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem) != 0)
				{
					eventSource.IsTablixCellScope = this.IsDataRegionCellScope;
				}
				string sortExpressionScopeString = eventSource.UserSort.SortExpressionScopeString;
				if (sortExpressionScopeString != null && this.IsScope(sortExpressionScopeString))
				{
					this.RegisterUserSortInnerScope(eventSource);
				}
				string sortTargetString = eventSource.UserSort.SortTargetString;
				if (sortTargetString != null && this.IsScope(sortTargetString))
				{
					this.RegisterUserSortWithSortTarget(eventSource);
				}
			}
		}

		internal void ProcessSortEventSource(IInScopeEventSource eventSource)
		{
			if (this.m_initializingUserSorts && this.m_hasUserSorts && eventSource != null && eventSource.UserSort != null)
			{
				this.AddEventSourceToScope(eventSource);
				GroupingList containingScopes = this.GetContainingScopes();
				for (int i = 0; i < containingScopes.Count; i++)
				{
					containingScopes[i].SaveGroupExprValues = true;
				}
				if ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0)
				{
					containingScopes.Add(null);
					this.SetDataSetDetailUserSortFilter();
				}
				eventSource.ContainingScopes = containingScopes;
				string sortExpressionScopeString = eventSource.UserSort.SortExpressionScopeString;
				if (sortExpressionScopeString == null)
				{
					this.EventSourceWithDetailSortExpressionAdd(eventSource);
				}
				else if (this.IsScope(sortExpressionScopeString))
				{
					if (this.IsCurrentScope(sortExpressionScopeString))
					{
						eventSource.UserSort.SortExpressionScope = this.GetSortFilterScope(sortExpressionScopeString);
						eventSource.ScopeChainInfo = this.GetScopeChainInfo();
						this.InitializeSortExpression(eventSource, false);
						this.UnregisterUserSortInnerScope(sortExpressionScopeString, eventSource);
					}
					else if (this.IsAncestorScope(sortExpressionScopeString))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Error, this.m_objectType, this.m_objectName, "SortExpressionScope", sortExpressionScopeString);
						this.UnregisterUserSortInnerScope(sortExpressionScopeString, eventSource);
					}
					else if (!this.m_scopeTree.IsSameOrProperParentScope(this.m_currentScope.DataScope, this.m_scopeTree.GetScopeByName(sortExpressionScopeString)))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Warning, eventSource.ObjectType, eventSource.Name, "SortExpressionScope", sortExpressionScopeString);
					}
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsNonExistingScope, Severity.Error, this.m_objectType, this.m_objectName, "SortExpressionScope", sortExpressionScopeString);
				}
				string sortTargetString = eventSource.UserSort.SortTargetString;
				if (sortTargetString != null)
				{
					if (this.IsScope(sortTargetString))
					{
						if (!this.IsCurrentScope(sortTargetString) && !this.IsAncestorScope(sortTargetString) && !this.IsPeerScope(sortTargetString))
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidTargetScope, Severity.Error, this.m_objectType, this.m_objectName, "SortTarget", sortTargetString);
							this.UnregisterUserSortWithSortTarget(sortTargetString, eventSource);
						}
						else
						{
							bool flag = false;
							if (this.m_groupingScopesForRunningValuesInTablix != null)
							{
								if (!this.m_groupingScopesForRunningValuesInTablix.HasRowColScopeConflict(eventSource.Scope, sortTargetString, out flag))
								{
									if (!flag)
									{
										return;
									}
									if (((ScopeInfo)this.m_groupingScopes[eventSource.Scope]).GroupingScope.Owner.DataRegionDef == ((ScopeInfo)this.m_groupingScopes[sortTargetString]).GroupingScope.Owner.DataRegionDef)
									{
										return;
									}
								}
								this.m_errorContext.Register(ProcessingErrorCode.rsInvalidTargetScope, Severity.Error, this.m_objectType, this.m_objectName, "SortTarget", sortTargetString);
								this.UnregisterUserSortWithSortTarget(sortTargetString, eventSource);
							}
						}
					}
					else
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsNonExistingScope, Severity.Error, this.m_objectType, this.m_objectName, "SortTarget", sortTargetString);
					}
				}
				else if (this.IsReportTopLevelScope())
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidOmittedTargetScope, Severity.Error, this.m_objectType, this.m_objectName, "SortTarget");
				}
				else
				{
					this.RegisterUserSortWithSortTarget(eventSource);
				}
			}
		}

		private void RegisterUserSortWithSortTarget(IInScopeEventSource eventSource)
		{
			string text = eventSource.UserSort.SortTargetString;
			if (text == null)
			{
				text = this.GetCurrentScopeName();
			}
			List<IInScopeEventSource> list = default(List<IInScopeEventSource>);
			if (this.m_userSortEventSources.TryGetValue(text, out list))
			{
				Global.Tracer.Assert(!list.Contains(eventSource), "(false == registeredEventSources.Contains(eventSource))");
				list.Add(eventSource);
			}
			else
			{
				list = new List<IInScopeEventSource>();
				list.Add(eventSource);
				this.m_userSortEventSources.Add(text, list);
			}
		}

		private void UnregisterUserSortWithSortTarget(string sortTarget, IInScopeEventSource eventSource)
		{
			List<IInScopeEventSource> list = default(List<IInScopeEventSource>);
			if (this.m_userSortEventSources.TryGetValue(sortTarget, out list))
			{
				Global.Tracer.Assert(list.Contains(eventSource), "(registeredEventSources.Contains(eventSource))");
				list.Remove(eventSource);
			}
		}

		internal GroupingList GetContainingScopesInCurrentDataRegion()
		{
			ScopeChainInfo scopeChainInfo = this.GetScopeChainInfo();
			if (scopeChainInfo != null)
			{
				return scopeChainInfo.GetGroupingListForContainingDataRegion();
			}
			return null;
		}

		internal GroupingList GetContainingScopes()
		{
			ScopeChainInfo scopeChainInfo = this.GetScopeChainInfo();
			if (scopeChainInfo != null)
			{
				return scopeChainInfo.GetGroupingList();
			}
			return new GroupingList();
		}

		private ScopeChainInfo GetScopeChainInfo()
		{
			if (this.m_groupingScopesForRunningValuesInTablix != null)
			{
				return this.m_groupingScopesForRunningValuesInTablix.GetScopeChainInfo();
			}
			return null;
		}

		private void AddEventSourceToScope(IInScopeEventSource eventSource)
		{
			eventSource.Scope = this.GetCurrentScopeName();
			IRIFReportScope iRIFReportScope = ((this.m_location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0) ? this.m_report : ((!eventSource.IsTablixCellScope) ? ((this.m_currentScope.DataRegionScope == null && this.m_currentScope.GroupingScope == null) ? ((ScopeInfo)this.m_groupingScopes[eventSource.Scope]).ReportScope : this.m_currentScope.ReportScope) : this.m_currentScope.ReportScope);
			iRIFReportScope.AddInScopeEventSource(eventSource);
			this.m_report.AddEventSource(eventSource);
		}

		private void InitializeSortExpression(IInScopeEventSource eventSource, bool needsExplicitAggregateScope)
		{
			EndUserSort userSort = eventSource.UserSort;
			if (userSort != null && userSort.SortExpression != null)
			{
				bool flag = true;
				if (needsExplicitAggregateScope && userSort.SortExpression.Aggregates != null)
				{
					int count = userSort.SortExpression.Aggregates.Count;
					for (int i = 0; i < count; i++)
					{
						string text = default(string);
						if (!userSort.SortExpression.Aggregates[i].GetScope(out text))
						{
							flag = false;
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidOmittedExpressionScope, Severity.Error, this.m_objectType, this.m_objectName, "SortExpression", "SortExpressionScope");
						}
					}
				}
				if (flag)
				{
					userSort.SortExpression.Initialize("SortExpression", this);
				}
			}
		}

		private void AddToScopeSortFilterList(IInScopeEventSource eventSource)
		{
			List<int> peerSortFilters = eventSource.GetPeerSortFilters(true);
			Global.Tracer.Assert(null != peerSortFilters, "(null != peerSorts)");
			peerSortFilters.Add(eventSource.ID);
		}

		internal void SetDataSetDetailUserSortFilter()
		{
			DataSet dataSet = this.GetDataSet();
			if (dataSet != null)
			{
				dataSet.HasDetailUserSortFilter = true;
			}
		}

		private void EventSourceWithDetailSortExpressionAdd(IInScopeEventSource eventSource)
		{
			Global.Tracer.Assert(null != this.m_detailSortExpressionScopeEventSources, "(null != m_detailSortExpressionScopeEventSources)");
			this.m_detailSortExpressionScopeEventSources.Add(eventSource);
		}

		internal void EventSourcesWithDetailSortExpressionInitialize(string sortExpressionScope)
		{
			if (this.m_hasUserSorts)
			{
				Global.Tracer.Assert(null != this.m_detailSortExpressionScopeEventSources, "(null != m_detailSortExpressionScopeEventSources)");
				int count = this.m_detailSortExpressionScopeEventSources.Count;
				if (count != 0)
				{
					for (int i = 0; i < count; i++)
					{
						IInScopeEventSource inScopeEventSource = this.m_detailSortExpressionScopeEventSources[i];
						inScopeEventSource.UserSort.SortExpressionScope = null;
						this.InitializeSortExpression(inScopeEventSource, true);
						if (sortExpressionScope != null && inScopeEventSource.ContainingScopes != null)
						{
							int num = inScopeEventSource.ContainingScopes.Count - 1;
							if (0 <= num && inScopeEventSource.ContainingScopes[num] == null)
							{
								this.m_errorContext.Register(ProcessingErrorCode.rsIneffectiveSortExpressionScope, Severity.Warning, inScopeEventSource.ObjectType, inScopeEventSource.Name, "SortExpressionScope", sortExpressionScope);
							}
						}
					}
					this.m_detailSortExpressionScopeEventSources.Clear();
				}
			}
		}

		internal void InitializeAbsolutePosition(ReportItem reportItem)
		{
			this.m_currentAbsoluteTop += reportItem.AbsoluteTopValue;
			this.m_currentAbsoluteLeft += reportItem.AbsoluteLeftValue;
		}

		internal void UpdateTopLeftDataRegion(DataRegion dataRegion)
		{
			this.m_report.UpdateTopLeftDataRegion(this, dataRegion);
		}

		internal void AddGroupingExprCountForGroup(string scope, int groupingExprCount)
		{
			if (!this.m_groupingExprCountAtScope.ContainsKey(scope))
			{
				this.m_groupingExprCountAtScope.Add(scope, groupingExprCount);
			}
		}

		internal void EnforceRdlSandboxContentRestrictions(CodeClass codeClass)
		{
		}

		internal void EnforceRdlSandboxContentRestrictions(ExpressionInfo expression, string propertyName)
		{
			this.EnforceRdlSandboxContentRestrictions(expression, this.ObjectType, this.ObjectName, propertyName);
		}

		internal void EnforceRdlSandboxContentRestrictions(ExpressionInfo expression, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
		}
	}
}
