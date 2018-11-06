using AspNetCore.ReportingServices.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal struct InitializationContext
	{
		private enum GroupingType
		{
			Normal,
			MatrixRow,
			MatrixColumn
		}

		private sealed class ScopeInfo
		{
			private bool m_allowCustomAggregates;

			private DataAggregateInfoList m_aggregates;

			private DataAggregateInfoList m_postSortAggregates;

			private DataAggregateInfoList m_recursiveAggregates;

			private Grouping m_groupingScope;

			private DataRegion m_dataRegionScope;

			private DataSet m_dataSetScope;

			internal bool AllowCustomAggregates
			{
				get
				{
					return this.m_allowCustomAggregates;
				}
			}

			internal DataAggregateInfoList Aggregates
			{
				get
				{
					return this.m_aggregates;
				}
			}

			internal DataAggregateInfoList PostSortAggregates
			{
				get
				{
					return this.m_postSortAggregates;
				}
			}

			internal DataAggregateInfoList RecursiveAggregates
			{
				get
				{
					return this.m_recursiveAggregates;
				}
			}

			internal Grouping GroupingScope
			{
				get
				{
					return this.m_groupingScope;
				}
			}

			internal DataSet DataSetScope
			{
				get
				{
					return this.m_dataSetScope;
				}
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates)
			{
				this.m_allowCustomAggregates = allowCustomAggregates;
				this.m_aggregates = aggregates;
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates, DataAggregateInfoList postSortAggregates)
			{
				this.m_allowCustomAggregates = allowCustomAggregates;
				this.m_aggregates = aggregates;
				this.m_postSortAggregates = postSortAggregates;
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates, DataAggregateInfoList postSortAggregates, DataRegion dataRegion)
			{
				this.m_allowCustomAggregates = allowCustomAggregates;
				this.m_aggregates = aggregates;
				this.m_postSortAggregates = postSortAggregates;
				this.m_dataRegionScope = dataRegion;
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates, DataAggregateInfoList postSortAggregates, DataSet dataset)
			{
				this.m_allowCustomAggregates = allowCustomAggregates;
				this.m_aggregates = aggregates;
				this.m_postSortAggregates = postSortAggregates;
				this.m_dataSetScope = dataset;
			}

			internal ScopeInfo(bool allowCustomAggregates, DataAggregateInfoList aggregates, DataAggregateInfoList postSortAggregates, DataAggregateInfoList recursiveAggregates, Grouping groupingScope)
			{
				this.m_allowCustomAggregates = allowCustomAggregates;
				this.m_aggregates = aggregates;
				this.m_postSortAggregates = postSortAggregates;
				this.m_recursiveAggregates = recursiveAggregates;
				this.m_groupingScope = groupingScope;
			}
		}

		private sealed class GroupingScopesForTablix
		{
			private bool m_rowScopeFound;

			private bool m_columnScopeFound;

			private ObjectType m_containerType;

			private string m_containerName;

			private Hashtable m_rowScopes;

			private Hashtable m_columnScopes;

			internal GroupingScopesForTablix(bool forceRows, ObjectType containerType, string containerName)
			{
				this.m_rowScopeFound = forceRows;
				this.m_columnScopeFound = false;
				this.m_containerType = containerType;
				this.m_containerName = containerName;
				this.m_rowScopes = new Hashtable();
				this.m_columnScopes = new Hashtable();
			}

			internal void RegisterRowGrouping(string groupName)
			{
				Global.Tracer.Assert(null != groupName);
				this.m_rowScopes[groupName] = null;
			}

			internal void UnRegisterRowGrouping(string groupName)
			{
				Global.Tracer.Assert(null != groupName);
				this.m_rowScopes.Remove(groupName);
			}

			internal void RegisterColumnGrouping(string groupName)
			{
				Global.Tracer.Assert(null != groupName);
				this.m_columnScopes[groupName] = null;
			}

			internal void UnRegisterColumnGrouping(string groupName)
			{
				Global.Tracer.Assert(null != groupName);
				this.m_columnScopes.Remove(groupName);
			}

			private ProcessingErrorCode getErrorCode()
			{
				switch (this.m_containerType)
				{
				case ObjectType.Matrix:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInMatrix;
				case ObjectType.CustomReportItem:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				case ObjectType.Chart:
					return ProcessingErrorCode.rsConflictingRunningValueScopesInTablix;
				default:
					Global.Tracer.Assert(false, string.Empty);
					return ProcessingErrorCode.rsConflictingRunningValueScopesInMatrix;
				}
			}

			internal bool HasRowColScopeConflict(string textboxSortActionScope, string sortTargetScope, bool sortExpressionScopeIsColumnScope)
			{
				if (this.HasRowColScopeConflict(textboxSortActionScope, sortExpressionScopeIsColumnScope))
				{
					return true;
				}
				if (sortTargetScope != null)
				{
					return this.HasRowColScopeConflict(sortTargetScope, sortExpressionScopeIsColumnScope);
				}
				return false;
			}

			private bool HasRowColScopeConflict(string scope, bool sortExpressionScopeIsColumnScope)
			{
				if (!sortExpressionScopeIsColumnScope && this.m_columnScopes.ContainsKey(scope))
				{
					return true;
				}
				if (sortExpressionScopeIsColumnScope)
				{
					return this.m_rowScopes.ContainsKey(scope);
				}
				return false;
			}

			internal bool ContainsScope(string scope, ErrorContext errorContext, bool checkConflictingScope)
			{
				Global.Tracer.Assert(null != scope, "(null != scope)");
				if (this.m_rowScopes.ContainsKey(scope))
				{
					if (checkConflictingScope)
					{
						if (this.m_columnScopeFound)
						{
							errorContext.Register(this.getErrorCode(), Severity.Error, this.m_containerType, this.m_containerName, null);
						}
						this.m_rowScopeFound = true;
					}
					return true;
				}
				if (this.m_columnScopes.ContainsKey(scope))
				{
					if (checkConflictingScope)
					{
						if (this.m_rowScopeFound)
						{
							errorContext.Register(this.getErrorCode(), Severity.Error, this.m_containerType, this.m_containerName, null);
						}
						this.m_columnScopeFound = true;
					}
					return true;
				}
				return false;
			}

			internal bool IsRunningValueDirectionColumn()
			{
				return this.m_columnScopeFound;
			}
		}

		private ICatalogItemContext m_reportContext;

		private LocationFlags m_location;

		private ObjectType m_objectType;

		private string m_objectName;

		private ObjectType m_detailObjectType;

		private string m_matrixName;

		private EmbeddedImageHashtable m_embeddedImages;

		private ImageStreamNames m_imageStreamNames;

		private ErrorContext m_errorContext;

		private Hashtable m_parameters;

		private ArrayList m_dynamicParameters;

		private Hashtable m_dataSetQueryInfo;

		private ExprHostBuilder m_exprHostBuilder;

		private Report m_report;

		private StringList m_aggregateEscalateScopes;

		private Hashtable m_aggregateRewriteScopes;

		private Hashtable m_aggregateRewriteMap;

		private int m_dataRegionCount;

		private string m_outerGroupName;

		private string m_currentGroupName;

		private string m_currentDataregionName;

		private RunningValueInfoList m_runningValues;

		private Hashtable m_groupingScopesForRunningValues;

		private GroupingScopesForTablix m_groupingScopesForRunningValuesInTablix;

		private Hashtable m_dataregionScopesForRunningValues;

		private bool m_hasFilters;

		private ScopeInfo m_currentScope;

		private ScopeInfo m_outermostDataregionScope;

		private Hashtable m_groupingScopes;

		private Hashtable m_dataregionScopes;

		private Hashtable m_datasetScopes;

		private int m_numberOfDataSets;

		private string m_oneDataSetName;

		private string m_currentDataSetName;

		private Hashtable m_fieldNameMap;

		private Hashtable m_dataSetNameToDataRegionsMap;

		private StringDictionary m_dataSources;

		private Hashtable m_reportItemsInScope;

		private Hashtable m_toggleItemInfos;

		private bool m_registerReceiver;

		private CultureInfo m_reportLanguage;

		private bool m_reportDataElementStyleAttribute;

		private bool m_tableColumnVisible;

		private bool m_hasUserSortPeerScopes;

		private Hashtable m_userSortExpressionScopes;

		private Hashtable m_userSortTextboxes;

		private Hashtable m_peerScopes;

		private int m_lastPeerScopeId;

		private Hashtable m_reportScopes;

		private Hashtable m_reportScopeDatasetIDs;

		private GroupingList m_groupingList;

		private Hashtable m_reportGroupingLists;

		private Hashtable m_scopesInMatrixCells;

		private StringList m_parentMatrixList;

		private TextBoxList m_reportSortFilterTextboxes;

		private TextBoxList m_detailSortExpressionScopeTextboxes;

		internal ICatalogItemContext ReportContext
		{
			get
			{
				return this.m_reportContext;
			}
		}

		internal LocationFlags Location
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

		internal ObjectType ObjectType
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

		internal bool TableColumnVisible
		{
			get
			{
				return this.m_tableColumnVisible;
			}
			set
			{
				this.m_tableColumnVisible = value;
			}
		}

		internal ObjectType DetailObjectType
		{
			set
			{
				this.m_detailObjectType = value;
			}
		}

		internal string MatrixName
		{
			get
			{
				return this.m_matrixName;
			}
			set
			{
				this.m_matrixName = value;
			}
		}

		internal EmbeddedImageHashtable EmbeddedImages
		{
			get
			{
				return this.m_embeddedImages;
			}
		}

		internal ImageStreamNames ImageStreamNames
		{
			get
			{
				return this.m_imageStreamNames;
			}
		}

		internal ErrorContext ErrorContext
		{
			get
			{
				return this.m_errorContext;
			}
		}

		internal bool RegisterHiddenReceiver
		{
			get
			{
				return this.m_registerReceiver;
			}
			set
			{
				this.m_registerReceiver = value;
			}
		}

		internal ExprHostBuilder ExprHostBuilder
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

		internal int DataRegionCount
		{
			get
			{
				return this.m_dataRegionCount;
			}
		}

		internal CultureInfo ReportLanguage
		{
			get
			{
				return this.m_reportLanguage;
			}
		}

		internal StringList AggregateEscalateScopes
		{
			get
			{
				return this.m_aggregateEscalateScopes;
			}
			set
			{
				this.m_aggregateEscalateScopes = value;
			}
		}

		internal Hashtable AggregateRewriteScopes
		{
			get
			{
				return this.m_aggregateRewriteScopes;
			}
			set
			{
				this.m_aggregateRewriteScopes = value;
			}
		}

		internal Hashtable AggregateRewriteMap
		{
			get
			{
				return this.m_aggregateRewriteMap;
			}
			set
			{
				this.m_aggregateRewriteMap = value;
			}
		}

		internal InitializationContext(ICatalogItemContext reportContext, bool hasFilters, StringDictionary dataSources, DataSetList dataSets, ArrayList dynamicParameters, Hashtable dataSetQueryInfo, ErrorContext errorContext, ExprHostBuilder exprHostBuilder, Report report, CultureInfo reportLanguage, Hashtable reportScopes, bool hasUserSortPeerScopes, int dataRegionCount)
		{
			Global.Tracer.Assert(null != dataSets, "(null != dataSets)");
			Global.Tracer.Assert(null != errorContext, "(null != errorContext)");
			this.m_reportContext = reportContext;
			this.m_location = LocationFlags.None;
			this.m_objectType = ObjectType.Report;
			this.m_objectName = null;
			this.m_detailObjectType = ObjectType.Report;
			this.m_matrixName = null;
			this.m_embeddedImages = report.EmbeddedImages;
			this.m_imageStreamNames = report.ImageStreamNames;
			this.m_errorContext = errorContext;
			this.m_parameters = null;
			this.m_dynamicParameters = dynamicParameters;
			this.m_dataSetQueryInfo = dataSetQueryInfo;
			this.m_registerReceiver = true;
			this.m_exprHostBuilder = exprHostBuilder;
			this.m_dataSources = dataSources;
			this.m_report = report;
			this.m_aggregateEscalateScopes = null;
			this.m_aggregateRewriteScopes = null;
			this.m_aggregateRewriteMap = null;
			this.m_reportLanguage = reportLanguage;
			this.m_outerGroupName = null;
			this.m_currentGroupName = null;
			this.m_currentDataregionName = null;
			this.m_runningValues = null;
			this.m_groupingScopesForRunningValues = new Hashtable();
			this.m_groupingScopesForRunningValuesInTablix = null;
			this.m_dataregionScopesForRunningValues = new Hashtable();
			this.m_hasFilters = hasFilters;
			this.m_currentScope = null;
			this.m_outermostDataregionScope = null;
			this.m_groupingScopes = new Hashtable();
			this.m_dataregionScopes = new Hashtable();
			this.m_datasetScopes = new Hashtable();
			for (int i = 0; i < dataSets.Count; i++)
			{
				this.m_datasetScopes[dataSets[i].Name] = new ScopeInfo(true, dataSets[i].Aggregates, dataSets[i].PostSortAggregates, dataSets[i]);
			}
			this.m_numberOfDataSets = dataSets.Count;
			this.m_oneDataSetName = ((1 == dataSets.Count) ? dataSets[0].Name : null);
			this.m_currentDataSetName = null;
			this.m_fieldNameMap = new Hashtable();
			this.m_dataSetNameToDataRegionsMap = new Hashtable();
			bool flag = false;
			if (this.m_dynamicParameters != null && this.m_dynamicParameters.Count > 0)
			{
				flag = true;
			}
			for (int j = 0; j < dataSets.Count; j++)
			{
				DataSet dataSet = dataSets[j];
				Global.Tracer.Assert(null != dataSet, "(null != dataSet)");
				Global.Tracer.Assert(null != dataSet.Query, "(null != dataSet.Query)");
				bool flag2 = false;
				if (report.DataSources != null)
				{
					int num = 0;
					while (num < report.DataSources.Count)
					{
						if (!(dataSet.Query.DataSourceName == report.DataSources[num].Name))
						{
							num++;
							continue;
						}
						flag2 = true;
						if (report.DataSources[num].DataSets == null)
						{
							report.DataSources[num].DataSets = new DataSetList();
						}
						if (flag)
						{
							YukonDataSetInfo yukonDataSetInfo = (YukonDataSetInfo)dataSetQueryInfo[dataSet.Name];
							Global.Tracer.Assert(null != yukonDataSetInfo, "(null != dataSetInfo)");
							yukonDataSetInfo.DataSourceIndex = num;
							yukonDataSetInfo.DataSetIndex = report.DataSources[num].DataSets.Count;
							yukonDataSetInfo.MergeFlagsFromDataSource(report.DataSources[num].IsComplex, report.DataSources[num].ParameterNames);
						}
						report.DataSources[num].DataSets.Add(dataSet);
						break;
					}
				}
				if (!flag2)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSourceReference, Severity.Error, dataSet.ObjectType, dataSet.Name, "DataSourceName", dataSet.Query.DataSourceName);
				}
				Hashtable hashtable = new Hashtable();
				if (dataSet.Fields != null)
				{
					for (int k = 0; k < dataSet.Fields.Count; k++)
					{
						hashtable[dataSet.Fields[k].Name] = k;
					}
				}
				this.m_fieldNameMap[dataSet.Name] = hashtable;
				this.m_dataSetNameToDataRegionsMap[dataSet.Name] = dataSet.DataRegions;
			}
			if (report.Parameters != null)
			{
				this.m_parameters = new Hashtable();
				for (int l = 0; l < report.Parameters.Count; l++)
				{
					ParameterDef parameterDef = report.Parameters[l];
					if (parameterDef != null)
					{
						try
						{
							this.m_parameters.Add(parameterDef.Name, parameterDef);
						}
						catch
						{
						}
					}
				}
			}
			this.m_reportItemsInScope = new Hashtable();
			this.m_toggleItemInfos = new Hashtable();
			this.m_reportDataElementStyleAttribute = true;
			this.m_tableColumnVisible = true;
			this.m_hasUserSortPeerScopes = hasUserSortPeerScopes;
			this.m_userSortExpressionScopes = new Hashtable();
			this.m_userSortTextboxes = new Hashtable();
			this.m_peerScopes = (hasUserSortPeerScopes ? new Hashtable() : null);
			this.m_lastPeerScopeId = 0;
			this.m_reportScopes = reportScopes;
			this.m_reportScopeDatasetIDs = new Hashtable();
			this.m_groupingList = new GroupingList();
			this.m_scopesInMatrixCells = new Hashtable();
			this.m_parentMatrixList = new StringList();
			this.m_reportSortFilterTextboxes = new TextBoxList();
			this.m_detailSortExpressionScopeTextboxes = new TextBoxList();
			this.m_reportGroupingLists = new Hashtable();
			this.m_reportGroupingLists.Add("0_ReportScope", this.m_groupingList.Clone());
			this.m_dataRegionCount = dataRegionCount;
		}

		private void RegisterDataSetScope(string scopeName, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates)
		{
			Global.Tracer.Assert(null != scopeName, "(null != scopeName)");
			Global.Tracer.Assert(null != scopeAggregates, "(null != scopeAggregates)");
			Global.Tracer.Assert(null != scopePostSortAggregates, "(null != scopePostSortAggregates)");
			this.m_currentScope = new ScopeInfo(true, scopeAggregates, scopePostSortAggregates);
			if (!this.m_reportGroupingLists.ContainsKey(scopeName))
			{
				this.m_reportGroupingLists.Add(scopeName, this.GetGroupingList());
				this.m_reportScopeDatasetIDs.Add(scopeName, this.GetDataSetID());
			}
		}

		private void UnRegisterDataSetScope(string scopeName)
		{
			Global.Tracer.Assert(null != scopeName, "(null != scopeName)");
			this.m_currentScope = null;
		}

		private void RegisterDataRegionScope(DataRegion dataRegion)
		{
			Global.Tracer.Assert(null != dataRegion.Name, "(null != dataRegion.Name)");
			Global.Tracer.Assert(null != dataRegion.Aggregates, "(null != dataRegion.Aggregates)");
			Global.Tracer.Assert(null != dataRegion.PostSortAggregates, "(null != dataRegion.PostSortAggregates)");
			this.m_currentDataregionName = dataRegion.Name;
			this.m_dataregionScopesForRunningValues[dataRegion.Name] = this.m_currentGroupName;
			ScopeInfo scopeInfo = this.m_currentScope = new ScopeInfo(this.m_currentScope == null || this.m_currentScope.AllowCustomAggregates, dataRegion.Aggregates, dataRegion.PostSortAggregates, dataRegion);
			if ((this.m_location & LocationFlags.InDataRegion) == (LocationFlags)0)
			{
				this.m_outermostDataregionScope = scopeInfo;
			}
			this.m_dataregionScopes[dataRegion.Name] = scopeInfo;
			if (dataRegion is Matrix)
			{
				this.m_parentMatrixList.Add(dataRegion.Name);
			}
			if (!this.m_reportGroupingLists.ContainsKey(dataRegion.Name))
			{
				this.m_reportGroupingLists.Add(dataRegion.Name, this.GetGroupingList());
				this.m_reportScopeDatasetIDs.Add(dataRegion.Name, this.GetDataSetID());
			}
			if ((LocationFlags)0 < (this.m_location & LocationFlags.InMatrixCell))
			{
				this.RegisterScopeInMatrixCell(this.m_matrixName, dataRegion.Name, false);
			}
			this.ProcessUserSortInnerScope(dataRegion.Name, false, false);
		}

		private void UnRegisterDataRegionScope(string scopeName)
		{
			Global.Tracer.Assert(null != scopeName, "(null != scopeName)");
			this.m_currentDataregionName = null;
			this.m_dataregionScopesForRunningValues.Remove(scopeName);
			this.m_currentScope = null;
			if ((this.m_location & LocationFlags.InDataRegion) == (LocationFlags)0)
			{
				this.m_outermostDataregionScope = null;
			}
			this.m_dataregionScopes.Remove(scopeName);
			int count = this.m_parentMatrixList.Count;
			if (0 < count && ReportProcessing.CompareWithInvariantCulture(this.m_parentMatrixList[count - 1], scopeName, false) == 0)
			{
				this.m_parentMatrixList.RemoveAt(count - 1);
			}
			this.ValidateUserSortInnerScope(scopeName);
			this.TextboxesWithDetailSortExpressionInitialize();
		}

		internal void RegisterGroupingScope(string scopeName, bool simpleGroupExpressions, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates, DataAggregateInfoList scopeRecursiveAggregates, Grouping groupingScope)
		{
			this.RegisterGroupingScope(scopeName, simpleGroupExpressions, scopeAggregates, scopePostSortAggregates, scopeRecursiveAggregates, groupingScope, false);
		}

		internal void RegisterGroupingScope(string scopeName, bool simpleGroupExpressions, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates, DataAggregateInfoList scopeRecursiveAggregates, Grouping groupingScope, bool isMatrixGrouping)
		{
			Global.Tracer.Assert(null != scopeName);
			Global.Tracer.Assert(null != scopeAggregates);
			Global.Tracer.Assert(null != scopePostSortAggregates);
			Global.Tracer.Assert(null != scopeRecursiveAggregates);
			this.m_outerGroupName = this.m_currentGroupName;
			this.m_currentGroupName = scopeName;
			this.m_groupingScopesForRunningValues[scopeName] = null;
			ScopeInfo value = this.m_currentScope = new ScopeInfo((this.m_currentScope == null) ? simpleGroupExpressions : (simpleGroupExpressions && this.m_currentScope.AllowCustomAggregates), scopeAggregates, scopePostSortAggregates, scopeRecursiveAggregates, groupingScope);
			this.m_groupingScopes[scopeName] = value;
			this.m_groupingList.Add(groupingScope);
			if (!this.m_reportGroupingLists.ContainsKey(scopeName))
			{
				this.m_reportGroupingLists.Add(scopeName, this.GetGroupingList());
				this.m_reportScopeDatasetIDs.Add(scopeName, this.GetDataSetID());
				if ((LocationFlags)0 < (this.m_location & LocationFlags.InMatrixCell))
				{
					this.RegisterScopeInMatrixCell(this.m_matrixName, scopeName, false);
				}
			}
			if (!isMatrixGrouping)
			{
				this.ProcessUserSortInnerScope(scopeName, false, false);
			}
		}

		internal void UnRegisterGroupingScope(string scopeName)
		{
			this.UnRegisterGroupingScope(scopeName, false);
		}

		internal void UnRegisterGroupingScope(string scopeName, bool isMatrixGrouping)
		{
			Global.Tracer.Assert(null != scopeName);
			this.m_outerGroupName = null;
			this.m_currentGroupName = null;
			this.m_groupingScopesForRunningValues.Remove(scopeName);
			this.m_currentScope = null;
			this.m_groupingScopes.Remove(scopeName);
			Global.Tracer.Assert(0 < this.m_groupingList.Count, "(0 < m_groupingList.Count)");
			this.m_groupingList.RemoveAt(this.m_groupingList.Count - 1);
			if (!isMatrixGrouping)
			{
				this.ValidateUserSortInnerScope(scopeName);
				this.TextboxesWithDetailSortExpressionInitialize();
			}
		}

		internal void ValidateHideDuplicateScope(string hideDuplicateScope, ReportItem reportItem)
		{
			if (hideDuplicateScope != null)
			{
				bool flag = true;
				ScopeInfo scopeInfo = null;
				if ((this.m_location & LocationFlags.InDetail) == (LocationFlags)0 && hideDuplicateScope.Equals(this.m_currentGroupName))
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

		internal void RegisterGroupingScopeForTablixCell(string scopeName, bool column, bool simpleGroupExpressions, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates, DataAggregateInfoList scopeRecursiveAggregates, Grouping groupingScope)
		{
			Global.Tracer.Assert(null != scopeName);
			Global.Tracer.Assert(null != scopeAggregates);
			Global.Tracer.Assert(null != scopePostSortAggregates);
			Global.Tracer.Assert(null != scopeRecursiveAggregates);
			if (column)
			{
				this.m_groupingScopesForRunningValuesInTablix.RegisterColumnGrouping(scopeName);
			}
			else
			{
				this.m_groupingScopesForRunningValuesInTablix.RegisterRowGrouping(scopeName);
			}
			ScopeInfo value = new ScopeInfo((this.m_currentScope == null) ? simpleGroupExpressions : (simpleGroupExpressions && this.m_currentScope.AllowCustomAggregates), scopeAggregates, scopePostSortAggregates, scopeRecursiveAggregates, groupingScope);
			this.m_groupingScopes[scopeName] = value;
		}

		internal void UnRegisterGroupingScopeForTablixCell(string scopeName, bool column)
		{
			Global.Tracer.Assert(null != scopeName);
			if (column)
			{
				this.m_groupingScopesForRunningValuesInTablix.UnRegisterColumnGrouping(scopeName);
			}
			else
			{
				this.m_groupingScopesForRunningValuesInTablix.UnRegisterRowGrouping(scopeName);
			}
			this.m_groupingScopes.Remove(scopeName);
		}

		internal void RegisterTablixCellScope(bool forceRows, DataAggregateInfoList scopeAggregates, DataAggregateInfoList scopePostSortAggregates)
		{
			Global.Tracer.Assert(null != scopeAggregates);
			this.m_groupingScopesForRunningValues = new Hashtable();
			this.m_groupingScopesForRunningValuesInTablix = new GroupingScopesForTablix(forceRows, this.m_objectType, this.m_objectName);
			this.m_dataregionScopesForRunningValues = new Hashtable();
			this.m_currentScope = new ScopeInfo(this.m_currentScope == null || this.m_currentScope.AllowCustomAggregates, scopeAggregates, scopePostSortAggregates);
		}

		internal void UnRegisterTablixCellScope()
		{
			this.m_groupingScopesForRunningValues = null;
			this.m_groupingScopesForRunningValuesInTablix = null;
			this.m_dataregionScopesForRunningValues = null;
			this.m_currentScope = null;
		}

		internal void RegisterPageSectionScope(DataAggregateInfoList scopeAggregates)
		{
			Global.Tracer.Assert(null != scopeAggregates);
			this.m_currentScope = new ScopeInfo(false, scopeAggregates);
		}

		internal void UnRegisterPageSectionScope()
		{
			this.m_currentScope = null;
		}

		internal void RegisterRunningValues(RunningValueInfoList runningValues)
		{
			Global.Tracer.Assert(null != runningValues);
			this.m_runningValues = runningValues;
		}

		internal void UnRegisterRunningValues(RunningValueInfoList runningValues)
		{
			Global.Tracer.Assert(null != runningValues);
			Global.Tracer.Assert(null != this.m_runningValues);
			Global.Tracer.Assert(object.ReferenceEquals(this.m_runningValues, runningValues));
			this.m_runningValues = null;
		}

		internal void TransferGroupExpressionRowNumbers(RunningValueInfoList rowNumbers)
		{
			if (rowNumbers != null)
			{
				for (int num = rowNumbers.Count - 1; num >= 0; num--)
				{
					Global.Tracer.Assert((LocationFlags)0 != (this.m_location & LocationFlags.InGrouping));
					RunningValueInfo runningValueInfo = rowNumbers[num];
					Global.Tracer.Assert(null != runningValueInfo);
					string scope = runningValueInfo.Scope;
					bool flag = true;
					ScopeInfo scopeInfo = null;
					if ((this.m_location & LocationFlags.InMatrixCell) != 0)
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
					else if (this.m_currentDataregionName == scope)
					{
						Global.Tracer.Assert(null != this.m_currentDataregionName, "(null != m_currentDataregionName)");
						scopeInfo = (ScopeInfo)this.m_dataregionScopes[this.m_currentDataregionName];
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

		internal bool IsRunningValueDirectionColumn()
		{
			return this.m_groupingScopesForRunningValuesInTablix.IsRunningValueDirectionColumn();
		}

		internal void TransferRunningValues(RunningValueInfoList runningValues, string propertyName)
		{
			this.TransferRunningValues(runningValues, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void TransferRunningValues(RunningValueInfoList runningValues, ObjectType objectType, string objectName, string propertyName)
		{
			if (runningValues != null && (this.m_location & LocationFlags.InPageSection) == (LocationFlags)0)
			{
				for (int num = runningValues.Count - 1; num >= 0; num--)
				{
					RunningValueInfo runningValueInfo = runningValues[num];
					Global.Tracer.Assert(null != runningValueInfo, "(null != runningValue)");
					string scope = runningValueInfo.Scope;
					bool flag = true;
					string text = null;
					DataAggregateInfoList dataAggregateInfoList = null;
					RunningValueInfoList runningValueInfoList = null;
					if (scope == null)
					{
						if ((this.m_location & LocationFlags.InDataRegion) == (LocationFlags)0)
						{
							flag = false;
						}
						else if ((this.m_location & LocationFlags.InMatrixCell) != 0)
						{
							flag = false;
						}
						else if ((this.m_location & LocationFlags.InGrouping) != 0 || (this.m_location & LocationFlags.InDetail) != 0)
						{
							text = this.GetDataSetName();
							runningValueInfoList = this.m_runningValues;
						}
						else
						{
							text = this.GetDataSetName();
							if (text != null)
							{
								ScopeInfo scopeInfo = (ScopeInfo)this.m_datasetScopes[text];
								Global.Tracer.Assert(null != scopeInfo, "(null != destinationScope)");
								dataAggregateInfoList = scopeInfo.Aggregates;
							}
						}
					}
					else if (this.m_groupingScopesForRunningValuesInTablix != null && this.m_groupingScopesForRunningValuesInTablix.ContainsScope(scope, this.m_errorContext, true))
					{
						Global.Tracer.Assert((LocationFlags)0 != (this.m_location & LocationFlags.InMatrixCell));
						text = this.GetDataSetName();
						runningValueInfoList = this.m_runningValues;
					}
					else if (this.m_groupingScopesForRunningValues.ContainsKey(scope))
					{
						Global.Tracer.Assert((LocationFlags)0 != (this.m_location & LocationFlags.InGrouping), "(0 != (m_location & LocationFlags.InGrouping))");
						text = this.GetDataSetName();
						runningValueInfoList = this.m_runningValues;
					}
					else if (this.m_dataregionScopesForRunningValues.ContainsKey(scope))
					{
						Global.Tracer.Assert((LocationFlags)0 != (this.m_location & LocationFlags.InDataRegion), "(0 != (m_location & LocationFlags.InDataRegion))");
						runningValueInfo.Scope = (string)this.m_dataregionScopesForRunningValues[scope];
						if ((this.m_location & LocationFlags.InGrouping) != 0 || (this.m_location & LocationFlags.InDetail) != 0)
						{
							text = this.GetDataSetName();
							runningValueInfoList = this.m_runningValues;
						}
						else
						{
							text = this.GetDataSetName();
							if (text != null)
							{
								ScopeInfo scopeInfo2 = (ScopeInfo)this.m_datasetScopes[text];
								Global.Tracer.Assert(null != scopeInfo2, "(null != destinationScope)");
								dataAggregateInfoList = scopeInfo2.Aggregates;
							}
						}
					}
					else if (this.m_datasetScopes.ContainsKey(scope))
					{
						if (((this.m_location & LocationFlags.InGrouping) != 0 || (this.m_location & LocationFlags.InDetail) != 0) && scope == this.GetDataSetName())
						{
							if ((this.m_location & LocationFlags.InMatrixCell) != 0)
							{
								flag = false;
							}
							else
							{
								text = scope;
								runningValueInfo.Scope = null;
								runningValueInfoList = this.m_runningValues;
							}
							goto IL_02b9;
						}
						text = scope;
						ScopeInfo scopeInfo3 = (ScopeInfo)this.m_datasetScopes[scope];
						Global.Tracer.Assert(null != scopeInfo3, "(null != destinationScope)");
						dataAggregateInfoList = scopeInfo3.Aggregates;
					}
					else
					{
						flag = false;
					}
					goto IL_02b9;
					IL_02b9:
					if (!flag)
					{
						if (!runningValueInfo.SuppressExceptions)
						{
							if ((this.m_location & LocationFlags.InMatrixCell) != 0)
							{
								if (DataAggregateInfo.AggregateTypes.Previous == runningValueInfo.AggregateType)
								{
									this.m_errorContext.Register(ProcessingErrorCode.rsInvalidPreviousAggregateInMatrixCell, Severity.Error, objectType, objectName, propertyName, this.m_matrixName);
								}
								else
								{
									this.m_errorContext.Register(ProcessingErrorCode.rsInvalidScopeInMatrix, Severity.Error, objectType, objectName, propertyName, this.m_matrixName);
								}
							}
							else
							{
								this.m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, objectType, objectName, propertyName);
							}
						}
					}
					else
					{
						if (runningValueInfo.Expressions != null)
						{
							for (int i = 0; i < runningValueInfo.Expressions.Length; i++)
							{
								Global.Tracer.Assert(null != runningValueInfo.Expressions[i], "(null != runningValue.Expressions[j])");
								runningValueInfo.Expressions[i].AggregateInitialize(text, objectType, objectName, propertyName, this);
							}
						}
						if (dataAggregateInfoList != null)
						{
							dataAggregateInfoList.Add(runningValueInfo);
						}
						else if (runningValueInfoList != null)
						{
							Global.Tracer.Assert(!object.ReferenceEquals(runningValues, runningValueInfoList));
							runningValueInfoList.Add(runningValueInfo);
						}
					}
					runningValues.RemoveAt(num);
				}
			}
		}

		internal void SpecialTransferRunningValues(RunningValueInfoList runningValues)
		{
			if (runningValues != null)
			{
				for (int num = runningValues.Count - 1; num >= 0; num--)
				{
					Global.Tracer.Assert(null != this.m_runningValues, "(null != m_runningValues)");
					Global.Tracer.Assert(!object.ReferenceEquals(runningValues, this.m_runningValues));
					this.m_runningValues.Add(runningValues[num]);
					runningValues.RemoveAt(num);
				}
			}
		}

		internal void CopyRunningValues(RunningValueInfoList runningValues, DataAggregateInfoList tablixAggregates)
		{
			Global.Tracer.Assert(null != runningValues);
			Global.Tracer.Assert((LocationFlags)0 != (this.m_location & LocationFlags.InMatrixCell));
			Global.Tracer.Assert(null != tablixAggregates);
			Global.Tracer.Assert(null != this.m_groupingScopesForRunningValuesInTablix);
			for (int i = 0; i < runningValues.Count; i++)
			{
				RunningValueInfo runningValueInfo = runningValues[i];
				if (runningValueInfo.Scope != null && this.m_groupingScopesForRunningValuesInTablix.ContainsScope(runningValueInfo.Scope, this.m_errorContext, false))
				{
					tablixAggregates.Add(runningValueInfo);
				}
			}
		}

		internal void TransferAggregates(DataAggregateInfoList aggregates, string propertyName)
		{
			this.TransferAggregates(aggregates, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void TransferAggregates(DataAggregateInfoList aggregates, ObjectType objectType, string objectName, string propertyName)
		{
			if (aggregates != null)
			{
				for (int num = aggregates.Count - 1; num >= 0; num--)
				{
					DataAggregateInfo dataAggregateInfo = aggregates[num];
					Global.Tracer.Assert(null != dataAggregateInfo, "(null != aggregate)");
					if (this.m_hasFilters && DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType && !dataAggregateInfo.SuppressExceptions)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsCustomAggregateAndFilter, Severity.Error, objectType, objectName, propertyName);
					}
					string text = default(string);
					bool scope = dataAggregateInfo.GetScope(out text);
					bool flag = true;
					string text2 = null;
					ScopeInfo scopeInfo = null;
					if ((this.m_location & LocationFlags.InPageSection) != 0 && scope)
					{
						flag = false;
						if (!dataAggregateInfo.SuppressExceptions)
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsScopeInPageSectionExpression, Severity.Error, objectType, objectName, propertyName);
						}
					}
					else if ((this.m_location & LocationFlags.InPageSection) == (LocationFlags)0 && this.m_numberOfDataSets == 0)
					{
						flag = false;
						if (!dataAggregateInfo.SuppressExceptions)
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, objectType, objectName, propertyName);
						}
					}
					else if (!scope)
					{
						text2 = this.GetDataSetName();
						if (LocationFlags.None == this.m_location)
						{
							if (1 != this.m_numberOfDataSets)
							{
								flag = false;
								if (!dataAggregateInfo.SuppressExceptions)
								{
									this.m_errorContext.Register(ProcessingErrorCode.rsMissingAggregateScope, Severity.Error, objectType, objectName, propertyName);
								}
							}
							else if (text2 != null)
							{
								scopeInfo = (ScopeInfo)this.m_datasetScopes[text2];
							}
						}
						else
						{
							Global.Tracer.Assert((this.m_location & LocationFlags.InDataSet) != 0 || (LocationFlags)0 != (this.m_location & LocationFlags.InPageSection));
							scopeInfo = this.m_currentScope;
						}
						if (scopeInfo != null && scopeInfo.DataSetScope != null)
						{
							scopeInfo.DataSetScope.UsedInAggregates = true;
						}
					}
					else if (text == null)
					{
						flag = false;
					}
					else if (this.m_groupingScopes.ContainsKey(text))
					{
						Global.Tracer.Assert((LocationFlags)0 != (this.m_location & LocationFlags.InGrouping), "(0 != (m_location & LocationFlags.InGrouping))");
						text2 = this.GetDataSetName();
						scopeInfo = (ScopeInfo)this.m_groupingScopes[text];
					}
					else if (this.m_dataregionScopes.ContainsKey(text))
					{
						Global.Tracer.Assert((LocationFlags)0 != (this.m_location & LocationFlags.InDataRegion), "(0 != (m_location & LocationFlags.InDataRegion))");
						text2 = this.GetDataSetName();
						scopeInfo = (ScopeInfo)this.m_dataregionScopes[text];
					}
					else if (this.m_datasetScopes.ContainsKey(text))
					{
						text2 = text;
						scopeInfo = (ScopeInfo)this.m_datasetScopes[text];
						scopeInfo.DataSetScope.UsedInAggregates = true;
					}
					else
					{
						flag = false;
						if (!dataAggregateInfo.SuppressExceptions)
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidAggregateScope, Severity.Error, objectType, objectName, propertyName);
						}
					}
					if (flag && scopeInfo != null)
					{
						if (DataAggregateInfo.AggregateTypes.Aggregate == dataAggregateInfo.AggregateType && !scopeInfo.AllowCustomAggregates && !dataAggregateInfo.SuppressExceptions)
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidCustomAggregateScope, Severity.Error, objectType, objectName, propertyName);
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
							DataSet dataSet = this.m_reportScopes[text2] as DataSet;
							if (dataSet != null && dataSet.InterpretSubtotalsAsDetailsIsAuto)
							{
								dataSet.InterpretSubtotalsAsDetails = false;
							}
						}
						DataAggregateInfoList dataAggregateInfoList = (!dataAggregateInfo.Recursive) ? ((scopeInfo.PostSortAggregates == null || !dataAggregateInfo.IsPostSortAggregate()) ? scopeInfo.Aggregates : scopeInfo.PostSortAggregates) : ((scopeInfo.GroupingScope != null && scopeInfo.GroupingScope.Parent != null) ? scopeInfo.RecursiveAggregates : scopeInfo.Aggregates);
						Global.Tracer.Assert(null != dataAggregateInfoList, "(null != destinationAggregates)");
						Global.Tracer.Assert(!object.ReferenceEquals(aggregates, dataAggregateInfoList));
						dataAggregateInfoList.Add(dataAggregateInfo);
					}
					aggregates.RemoveAt(num);
				}
			}
		}

		internal string EscalateScope(string oldScope)
		{
			if (this.m_aggregateRewriteScopes != null && this.m_aggregateRewriteScopes.ContainsKey(oldScope))
			{
				Global.Tracer.Assert(this.m_aggregateEscalateScopes != null && 1 <= this.m_aggregateEscalateScopes.Count);
				return this.m_aggregateEscalateScopes[this.m_aggregateEscalateScopes.Count - 1];
			}
			return oldScope;
		}

		internal void InitializeParameters(ParameterDefList parameters, DataSetList dataSetList)
		{
			if (this.m_dynamicParameters != null && this.m_dynamicParameters.Count != 0)
			{
				Hashtable hashtable = new Hashtable();
				DynamicParameter dynamicParameter = null;
				int i = 0;
				for (int j = 0; j < this.m_dynamicParameters.Count; j++)
				{
					for (dynamicParameter = (DynamicParameter)this.m_dynamicParameters[j]; i < dynamicParameter.Index; i++)
					{
						hashtable.Add(parameters[i].Name, i);
					}
					this.InitializeParameter(parameters[dynamicParameter.Index], dynamicParameter, hashtable, dataSetList);
				}
			}
		}

		private void InitializeParameter(ParameterDef parameter, DynamicParameter dynamicParameter, Hashtable dependencies, DataSetList dataSetList)
		{
			Global.Tracer.Assert(null != dynamicParameter, "(null != dynamicParameter)");
			DataSetReference dataSetReference = null;
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

		private void InitializeParameterDataSource(ParameterDef parameter, DataSetReference dataSetRef, bool isDefault, Hashtable dependencies, ref bool isComplex, DataSetList dataSetList)
		{
			ParameterDataSource parameterDataSource = null;
			YukonDataSetInfo yukonDataSetInfo = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			yukonDataSetInfo = (YukonDataSetInfo)this.m_dataSetQueryInfo[dataSetRef.DataSet];
			if (yukonDataSetInfo == null)
			{
				if (isDefault)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDefaultValueDataSetReference, Severity.Error, ObjectType.ReportParameter, parameter.Name, "DataSetReference", dataSetRef.DataSet);
				}
				else
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsInvalidValidValuesDataSetReference, Severity.Error, ObjectType.ReportParameter, parameter.Name, "DataSetReference", dataSetRef.DataSet);
				}
			}
			else
			{
				DataSet dataSet = dataSetList[yukonDataSetInfo.DataSetDefIndex];
				if (!dataSet.UsedInAggregates)
				{
					DataRegionList dataRegionList = (DataRegionList)this.m_dataSetNameToDataRegionsMap[dataSetRef.DataSet];
					if (dataRegionList == null || dataRegionList.Count == 0)
					{
						dataSet.UsedOnlyInParameters = true;
					}
				}
				parameterDataSource = new ParameterDataSource(yukonDataSetInfo.DataSourceIndex, yukonDataSetInfo.DataSetIndex);
				Hashtable hashtable = (Hashtable)this.m_fieldNameMap[dataSetRef.DataSet];
				if (hashtable != null)
				{
					if (hashtable.ContainsKey(dataSetRef.ValueAlias))
					{
						parameterDataSource.ValueFieldIndex = (int)hashtable[dataSetRef.ValueAlias];
						if (parameterDataSource.ValueFieldIndex >= yukonDataSetInfo.CalculatedFieldIndex)
						{
							flag3 = (dataSet.Fields == null || parameterDataSource.ValueFieldIndex > dataSet.Fields.Count || !(dataSet.Fields[parameterDataSource.ValueFieldIndex].Value is ExpressionInfoExtended) || !((ExpressionInfoExtended)dataSet.Fields[parameterDataSource.ValueFieldIndex].Value).IsExtendedSimpleFieldReference);
						}
						flag = true;
					}
					if (dataSetRef.LabelAlias != null)
					{
						if (hashtable.ContainsKey(dataSetRef.LabelAlias))
						{
							parameterDataSource.LabelFieldIndex = (int)hashtable[dataSetRef.LabelAlias];
							if (parameterDataSource.LabelFieldIndex >= yukonDataSetInfo.CalculatedFieldIndex)
							{
								flag3 = (dataSet.Fields == null || parameterDataSource.LabelFieldIndex > dataSet.Fields.Count || !(dataSet.Fields[parameterDataSource.LabelFieldIndex].Value is ExpressionInfoExtended) || !((ExpressionInfoExtended)dataSet.Fields[parameterDataSource.LabelFieldIndex].Value).IsExtendedSimpleFieldReference);
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
					this.ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetReferenceField, Severity.Error, ObjectType.ReportParameter, parameter.Name, "Field", dataSetRef.ValueAlias, dataSetRef.DataSet);
				}
				if (!flag2)
				{
					this.ErrorContext.Register(ProcessingErrorCode.rsInvalidDataSetReferenceField, Severity.Error, ObjectType.ReportParameter, parameter.Name, "Field", dataSetRef.LabelAlias, dataSetRef.DataSet);
				}
				if (!isComplex)
				{
					if (yukonDataSetInfo.IsComplex || flag3)
					{
						isComplex = true;
						parameter.Dependencies = (Hashtable)dependencies.Clone();
					}
					else if (yukonDataSetInfo.ParameterNames != null && yukonDataSetInfo.ParameterNames.Count != 0)
					{
						string text = null;
						Hashtable hashtable2 = parameter.Dependencies;
						if (hashtable2 == null)
						{
							hashtable2 = (parameter.Dependencies = new Hashtable());
						}
						for (int i = 0; i < yukonDataSetInfo.ParameterNames.Count; i++)
						{
							text = yukonDataSetInfo.ParameterNames[i];
							if (dependencies.ContainsKey(text))
							{
								if (!hashtable2.ContainsKey(text))
								{
									hashtable2.Add(text, dependencies[text]);
								}
							}
							else
							{
								this.ErrorContext.Register(ProcessingErrorCode.rsInvalidReportParameterDependency, Severity.Error, ObjectType.ReportParameter, parameter.Name, "DataSetReference", text);
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

		internal void MergeFieldPropertiesIntoDataset(ExpressionInfo expressionInfo)
		{
			if (expressionInfo.ReferencedFieldProperties == null && !expressionInfo.DynamicFieldReferences)
			{
				return;
			}
			string dataSetName = this.GetDataSetName();
			if (dataSetName != null)
			{
				DataSet dataSet = this.m_reportScopes[dataSetName] as DataSet;
				if (dataSet != null)
				{
					dataSet.MergeFieldProperties(expressionInfo);
				}
			}
		}

		internal void RegisterDataRegion(DataRegion dataRegion)
		{
			if (this.m_numberOfDataSets == 0)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionWithoutDataSet, Severity.Error, this.m_objectType, this.m_objectName, null);
			}
			if ((this.m_location & LocationFlags.InDetail) != 0 && ObjectType.List == this.m_detailObjectType)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsDataRegionInDetailList, Severity.Error, this.m_objectType, this.m_objectName, null);
			}
			if ((this.m_location & LocationFlags.InDataRegion) == (LocationFlags)0)
			{
				this.ValidateDataSetNameForTopLevelDataRegion(dataRegion.DataSetName);
				string dataSetName = this.GetDataSetName();
				if (dataSetName != null)
				{
					DataRegionList dataRegionList = (DataRegionList)this.m_dataSetNameToDataRegionsMap[dataSetName];
					Global.Tracer.Assert(null != dataRegionList, "(null != dataRegions)");
					dataRegionList.Add(dataRegion);
					ScopeInfo scopeInfo = (ScopeInfo)this.m_datasetScopes[dataSetName];
					Global.Tracer.Assert(null != scopeInfo, "(null != dataSetScope)");
					this.RegisterDataSetScope(dataSetName, scopeInfo.Aggregates, scopeInfo.PostSortAggregates);
				}
			}
			this.RegisterDataRegionScope(dataRegion);
		}

		internal void UnRegisterDataRegion(DataRegion dataRegion)
		{
			if ((this.m_location & LocationFlags.InDataRegion) == (LocationFlags)0)
			{
				string dataSetName = this.GetDataSetName();
				if (dataSetName != null)
				{
					this.UnRegisterDataSetScope(dataSetName);
				}
			}
			this.UnRegisterDataRegionScope(dataRegion.Name);
		}

		internal void RegisterDataSet(DataSet dataSet)
		{
			this.m_currentDataSetName = dataSet.Name;
			this.RegisterDataSetScope(dataSet.Name, dataSet.Aggregates, dataSet.PostSortAggregates);
		}

		internal void UnRegisterDataSet(DataSet dataSet)
		{
			this.m_currentDataSetName = null;
			this.UnRegisterDataSetScope(dataSet.Name);
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
			return this.m_currentDataSetName;
		}

		private int GetDataSetID()
		{
			string dataSetName = this.GetDataSetName();
			if (dataSetName == null)
			{
				return -1;
			}
			ISortFilterScope sortFilterScope = this.m_reportScopes[dataSetName] as ISortFilterScope;
			if (sortFilterScope == null)
			{
				return -1;
			}
			return sortFilterScope.ID;
		}

		private void ValidateDataSetNameForTopLevelDataRegion(string dataSetName)
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
				if (dataSetName == null)
				{
					this.m_errorContext.Register(ProcessingErrorCode.rsMissingDataSetName, Severity.Error, this.m_objectType, this.m_objectName, "DataSetName");
				}
				else
				{
					flag = this.m_fieldNameMap.ContainsKey(dataSetName);
				}
			}
			if (!flag)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidDataSetName, Severity.Error, this.m_objectType, this.m_objectName, "DataSetName", dataSetName);
			}
			else
			{
				this.m_currentDataSetName = dataSetName;
			}
		}

		internal void CheckFieldReferences(StringList fieldNames, string propertyName)
		{
			this.InternalCheckFieldReference(fieldNames, this.GetDataSetName(), this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckFieldReferences(StringList fieldNames, string dataSetName, ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckFieldReference(fieldNames, dataSetName, objectType, objectName, propertyName);
		}

		private void InternalCheckFieldReference(StringList fieldNames, string dataSetName, ObjectType objectType, string objectName, string propertyName)
		{
			if (fieldNames != null && (this.m_location & LocationFlags.InPageSection) == (LocationFlags)0)
			{
				Hashtable hashtable = null;
				if (dataSetName != null)
				{
					hashtable = (Hashtable)this.m_fieldNameMap[dataSetName];
				}
				for (int i = 0; i < fieldNames.Count; i++)
				{
					string text = fieldNames[i];
					if (this.m_numberOfDataSets == 0)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsFieldReference, Severity.Error, objectType, objectName, propertyName, text);
					}
					else
					{
						Global.Tracer.Assert(1 <= this.m_numberOfDataSets);
						if (hashtable != null && !hashtable.ContainsKey(text))
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsFieldReference, Severity.Error, objectType, objectName, propertyName, text);
						}
					}
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
			if (exprInfo != null && exprInfo.Type == ExpressionInfo.Types.Field && (this.m_location & LocationFlags.InPageSection) == (LocationFlags)0 && dataSetName != null)
			{
				Hashtable hashtable = (Hashtable)this.m_fieldNameMap[dataSetName];
				if (hashtable != null && hashtable.ContainsKey(exprInfo.Value))
				{
					exprInfo.IntValue = (int)hashtable[exprInfo.Value];
				}
			}
		}

		internal void FillInTokenIndex(ExpressionInfo exprInfo)
		{
			if (exprInfo != null && exprInfo.Type == ExpressionInfo.Types.Token)
			{
				string value = exprInfo.Value;
				if (value != null)
				{
					DataSet dataSet = this.m_reportScopes[value] as DataSet;
					if (dataSet != null)
					{
						exprInfo.IntValue = dataSet.ID;
					}
				}
			}
		}

		internal void CheckDataSetReference(StringList referencedDataSets, string propertyName)
		{
			this.InternalCheckDataSetReference(referencedDataSets, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckDataSetReference(StringList referencedDataSets, ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckDataSetReference(referencedDataSets, objectType, objectName, propertyName);
		}

		private void InternalCheckDataSetReference(StringList dataSetNames, ObjectType objectType, string objectName, string propertyName)
		{
			if (dataSetNames != null && (this.m_location & LocationFlags.InPageSection) == (LocationFlags)0)
			{
				for (int i = 0; i < dataSetNames.Count; i++)
				{
					if (!this.m_dataSetNameToDataRegionsMap.ContainsKey(dataSetNames[i]))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsDataSetReference, Severity.Error, objectType, objectName, propertyName, dataSetNames[i]);
					}
				}
			}
		}

		internal void CheckDataSourceReference(StringList referencedDataSources, string propertyName)
		{
			this.InternalCheckDataSourceReference(referencedDataSources, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckDataSourceReference(StringList referencedDataSources, ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckDataSetReference(referencedDataSources, objectType, objectName, propertyName);
		}

		private void InternalCheckDataSourceReference(StringList dataSourceNames, ObjectType objectType, string objectName, string propertyName)
		{
			if (dataSourceNames != null && (this.m_location & LocationFlags.InPageSection) == (LocationFlags)0)
			{
				for (int i = 0; i < dataSourceNames.Count; i++)
				{
					if (!this.m_dataSources.ContainsKey(dataSourceNames[i]))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsDataSourceReference, Severity.Error, objectType, objectName, propertyName, dataSourceNames[i]);
					}
				}
			}
		}

		internal int GenerateSubtotalID()
		{
			Global.Tracer.Assert(null != this.m_report);
			this.m_report.LastID++;
			return this.m_report.LastID;
		}

		internal string GenerateAggregateID(string oldAggregateID)
		{
			Global.Tracer.Assert(null != this.m_report);
			this.m_report.LastAggregateID++;
			string text = "Aggregate" + this.m_report.LastAggregateID;
			if (this.m_aggregateRewriteMap == null)
			{
				this.m_aggregateRewriteMap = new Hashtable();
			}
			this.m_aggregateRewriteMap.Add(oldAggregateID, text);
			return text;
		}

		internal void RegisterReportItems(ReportItemCollection reportItems)
		{
			Global.Tracer.Assert(null != reportItems, "(null != reportItems)");
			for (int i = 0; i < reportItems.Count; i++)
			{
				ReportItem reportItem = reportItems[i];
				if (reportItem != null)
				{
					this.m_reportItemsInScope[reportItem.Name] = reportItem;
					if (reportItem is Rectangle)
					{
						this.RegisterReportItems(((Rectangle)reportItem).ReportItems);
					}
					if (reportItem is Table)
					{
						((Table)reportItem).RegisterHeaderAndFooter(this);
					}
					if (reportItem is Matrix)
					{
						this.RegisterReportItems(((Matrix)reportItem).CornerReportItems);
					}
				}
			}
		}

		internal void UnRegisterReportItems(ReportItemCollection reportItems)
		{
			Global.Tracer.Assert(null != reportItems, "(null != reportItems)");
			for (int i = 0; i < reportItems.Count; i++)
			{
				ReportItem reportItem = reportItems[i];
				if (reportItem != null)
				{
					this.m_reportItemsInScope.Remove(reportItem.Name);
					if (reportItem is Rectangle)
					{
						this.UnRegisterReportItems(((Rectangle)reportItem).ReportItems);
					}
					if (reportItem is Table)
					{
						((Table)reportItem).UnRegisterHeaderAndFooter(this);
					}
					if (reportItem is Matrix)
					{
						this.UnRegisterReportItems(((Matrix)reportItem).CornerReportItems);
					}
				}
			}
		}

		internal void CheckReportItemReferences(StringList referencedReportItems, string propertyName)
		{
			this.InternalCheckReportItemReferences(referencedReportItems, this.m_objectType, this.m_objectName, propertyName);
		}

		internal void AggregateCheckReportItemReferences(StringList referencedReportItems, ObjectType objectType, string objectName, string propertyName)
		{
			this.InternalCheckReportItemReferences(referencedReportItems, objectType, objectName, propertyName);
		}

		private void InternalCheckReportItemReferences(StringList referencedReportItems, ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedReportItems != null && (this.m_location & LocationFlags.InPageSection) == (LocationFlags)0)
			{
				for (int i = 0; i < referencedReportItems.Count; i++)
				{
					if (!this.m_reportItemsInScope.ContainsKey(referencedReportItems[i]))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsReportItemReference, Severity.Error, objectType, objectName, propertyName, referencedReportItems[i]);
					}
				}
			}
		}

		internal void CheckReportParameterReferences(StringList referencedParameters, string propertyName)
		{
			this.InternalCheckReportParameterReferences(referencedParameters, this.m_objectType, this.m_objectName, propertyName);
		}

		private void InternalCheckReportParameterReferences(StringList referencedParameters, ObjectType objectType, string objectName, string propertyName)
		{
			if (referencedParameters != null)
			{
				for (int i = 0; i < referencedParameters.Count; i++)
				{
					if (this.m_parameters == null || !this.m_parameters.ContainsKey(referencedParameters[i]))
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsParameterReference, Severity.Error, objectType, objectName, propertyName, referencedParameters[i]);
					}
				}
			}
		}

		internal ToggleItemInfo RegisterReceiver(string senderName, Visibility visibility, bool isContainer)
		{
			if (senderName == null)
			{
				return null;
			}
			if ((this.m_location & LocationFlags.InPageSection) != 0)
			{
				return null;
			}
			ReportItem reportItem = (ReportItem)this.m_reportItemsInScope[senderName];
			if (!(reportItem is TextBox))
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsInvalidToggleItem, Severity.Error, this.m_objectType, this.m_objectName, "Item", senderName);
			}
			else
			{
				((TextBox)reportItem).IsToggle = true;
				do
				{
					reportItem.Computed = true;
					reportItem = reportItem.Parent;
				}
				while (reportItem is Rectangle);
				if (isContainer)
				{
					ToggleItemInfoList toggleItemInfoList = (ToggleItemInfoList)this.m_toggleItemInfos[senderName];
					if (toggleItemInfoList == null)
					{
						toggleItemInfoList = new ToggleItemInfoList();
						this.m_toggleItemInfos[senderName] = toggleItemInfoList;
					}
					ToggleItemInfo toggleItemInfo = new ToggleItemInfo();
					toggleItemInfo.ObjectName = this.m_objectName;
					toggleItemInfo.ObjectType = this.m_objectType;
					toggleItemInfo.Visibility = visibility;
					toggleItemInfo.GroupName = this.m_currentGroupName;
					toggleItemInfoList.Add(toggleItemInfo);
					return toggleItemInfo;
				}
			}
			return null;
		}

		internal void UnRegisterReceiver(string senderName, ToggleItemInfo toggleItemInfo)
		{
			Global.Tracer.Assert(null != toggleItemInfo, "(null != toggleItemInfo)");
			ToggleItemInfoList toggleItemInfoList = (ToggleItemInfoList)this.m_toggleItemInfos[senderName];
			if (toggleItemInfoList != null)
			{
				toggleItemInfoList.Remove(toggleItemInfo);
			}
		}

		internal void RegisterSender(TextBox textbox)
		{
			Global.Tracer.Assert(null != textbox);
			ToggleItemInfoList toggleItemInfoList = (ToggleItemInfoList)this.m_toggleItemInfos[textbox.Name];
			if (toggleItemInfoList != null && 0 < toggleItemInfoList.Count)
			{
				bool flag = false;
				ScopeInfo scopeInfo = null;
				if (this.m_currentGroupName != null)
				{
					scopeInfo = (ScopeInfo)this.m_groupingScopes[this.m_currentGroupName];
					Global.Tracer.Assert(scopeInfo != null && null != scopeInfo.GroupingScope);
					if (scopeInfo.GroupingScope.Parent != null)
					{
						flag = true;
					}
				}
				for (int i = 0; i < toggleItemInfoList.Count; i++)
				{
					ToggleItemInfo toggleItemInfo = toggleItemInfoList[i];
					if (flag && toggleItemInfo.GroupName == this.m_currentGroupName)
					{
						textbox.RecursiveSender = true;
						toggleItemInfo.Visibility.RecursiveReceiver = true;
					}
					else
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidToggleItem, Severity.Error, toggleItemInfo.ObjectType, toggleItemInfo.ObjectName, "Item", textbox.Name);
					}
				}
				this.m_toggleItemInfos.Remove(textbox.Name);
			}
		}

		internal double ValidateSize(string size, string propertyName)
		{
			double result = default(double);
			string text = default(string);
			PublishingValidator.ValidateSize(size, this.m_objectType, this.m_objectName, propertyName, true, this.m_errorContext, out result, out text);
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
			PublishingValidator.ValidateSize(size, this.m_objectType, this.m_objectName, propertyName, restrictMaxValue, this.m_errorContext, out result, out text);
			size = text;
			return result;
		}

		internal void CheckInternationalSettings(StyleAttributeHashtable styleAttributes)
		{
			if (styleAttributes != null && styleAttributes.Count != 0)
			{
				CultureInfo cultureInfo = null;
				AttributeInfo attributeInfo = styleAttributes["Language"];
				if (attributeInfo == null)
				{
					cultureInfo = this.m_reportLanguage;
				}
				else if (!attributeInfo.IsExpression)
				{
					PublishingValidator.ValidateLanguage(attributeInfo.Value, this.ObjectType, this.ObjectName, "Language", this.ErrorContext, out cultureInfo);
				}
				if (cultureInfo != null)
				{
					AttributeInfo attributeInfo2 = styleAttributes["Calendar"];
					if (attributeInfo2 != null && !attributeInfo2.IsExpression)
					{
						PublishingValidator.ValidateCalendar(cultureInfo, attributeInfo2.Value, this.ObjectType, this.ObjectName, "Calendar", this.ErrorContext);
					}
				}
				attributeInfo = styleAttributes["NumeralLanguage"];
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						cultureInfo = null;
					}
					else
					{
						PublishingValidator.ValidateLanguage(attributeInfo.Value, this.ObjectType, this.ObjectName, "NumeralLanguage", this.ErrorContext, out cultureInfo);
					}
				}
				if (cultureInfo != null)
				{
					AttributeInfo attributeInfo3 = styleAttributes["NumeralVariant"];
					if (attributeInfo3 != null && !attributeInfo3.IsExpression)
					{
						PublishingValidator.ValidateNumeralVariant(cultureInfo, attributeInfo3.IntValue, this.ObjectType, this.ObjectName, "NumeralVariant", this.ErrorContext);
					}
				}
			}
		}

		internal string GetCurrentScope()
		{
			if (this.m_currentScope != null)
			{
				if (this.m_currentScope.GroupingScope != null)
				{
					return this.m_currentScope.GroupingScope.Name;
				}
				Global.Tracer.Assert(null != this.m_currentDataregionName, "(null != m_currentDataregionName)");
				return this.m_currentDataregionName;
			}
			return "0_ReportScope";
		}

		internal bool IsScope(string scope)
		{
			if (scope == null)
			{
				return false;
			}
			return this.m_reportScopes.ContainsKey(scope);
		}

		internal bool IsAncestorScope(string targetScope, bool inMatrixGrouping, bool checkAllGroupingScopes)
		{
			string dataSetName = this.GetDataSetName();
			if (dataSetName != null && ReportProcessing.CompareWithInvariantCulture(dataSetName, targetScope, false) == 0)
			{
				return true;
			}
			if (this.m_dataregionScopes != null && this.m_dataregionScopes.ContainsKey(targetScope))
			{
				return true;
			}
			if ((checkAllGroupingScopes || inMatrixGrouping) && this.m_groupingScopesForRunningValuesInTablix != null && this.m_groupingScopesForRunningValuesInTablix.ContainsScope(targetScope, null, false))
			{
				return true;
			}
			if ((checkAllGroupingScopes || !inMatrixGrouping) && this.m_groupingScopesForRunningValues != null && this.m_groupingScopesForRunningValues.ContainsKey(targetScope))
			{
				return true;
			}
			return false;
		}

		internal bool IsCurrentScope(string targetScope)
		{
			if (this.m_currentScope != null)
			{
				if (this.m_currentScope.GroupingScope == null && ReportProcessing.CompareWithInvariantCulture(targetScope, this.m_currentDataregionName, false) == 0)
				{
					return true;
				}
				if (this.m_currentScope.GroupingScope != null && ReportProcessing.CompareWithInvariantCulture(targetScope, this.m_currentScope.GroupingScope.Name, false) == 0)
				{
					return true;
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
			string currentScope = this.GetCurrentScope();
			Global.Tracer.Assert(currentScope != null && null != this.m_peerScopes, "(null != currentScope && null != m_peerScopes)");
			object obj = this.m_peerScopes[currentScope];
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
			return null == this.m_currentScope;
		}

		internal ISortFilterScope GetSortFilterScope()
		{
			return this.GetSortFilterScope(this.GetCurrentScope());
		}

		internal ISortFilterScope GetSortFilterScope(string scopeName)
		{
			Global.Tracer.Assert(scopeName != null && "0_ReportScope" != scopeName && this.m_reportScopes.ContainsKey(scopeName));
			return this.m_reportScopes[scopeName] as ISortFilterScope;
		}

		internal GroupingList GetGroupingList()
		{
			Global.Tracer.Assert(null != this.m_groupingList);
			return this.m_groupingList.Clone();
		}

		internal void RegisterScopeInMatrixCell(string matrixName, string scope, bool registerMatrixCellScope)
		{
			Global.Tracer.Assert(matrixName != null && null != this.m_scopesInMatrixCells);
			StringList stringList = this.m_scopesInMatrixCells[matrixName] as StringList;
			if (stringList != null)
			{
				if (!stringList.Contains(scope))
				{
					stringList.Add(scope);
				}
			}
			else
			{
				stringList = new StringList();
				stringList.Add(scope);
				this.m_scopesInMatrixCells.Add(matrixName, stringList);
			}
			if (registerMatrixCellScope && !this.m_reportGroupingLists.ContainsKey(scope))
			{
				this.m_reportGroupingLists.Add(scope, this.GetGroupingList());
			}
		}

		internal void UpdateScopesInMatrixCells(string matrixName, GroupingList matrixGroups)
		{
			int count = this.m_groupingList.Count;
			int count2 = this.m_parentMatrixList.Count;
			Global.Tracer.Assert(1 <= count2 && 0 == ReportProcessing.CompareWithInvariantCulture(this.m_parentMatrixList[count2 - 1], matrixName, false));
			string text = null;
			if (1 < count2)
			{
				text = this.m_parentMatrixList[count2 - 2];
			}
			StringList stringList = this.m_scopesInMatrixCells[matrixName] as StringList;
			Global.Tracer.Assert(null != stringList);
			int count3 = stringList.Count;
			for (int i = 0; i < count3; i++)
			{
				string text2 = stringList[i];
				Global.Tracer.Assert(null != text2);
				if (matrixGroups != null)
				{
					GroupingList groupingList = this.m_reportGroupingLists[text2] as GroupingList;
					Global.Tracer.Assert(groupingList != null && count <= groupingList.Count);
					groupingList.InsertRange(count, matrixGroups);
				}
				if (text != null)
				{
					this.RegisterScopeInMatrixCell(text, text2, false);
				}
			}
			this.m_scopesInMatrixCells.Remove(matrixName);
		}

		internal void RegisterPeerScopes(ReportItemCollection reportItems)
		{
			this.RegisterPeerScopes(reportItems, ++this.m_lastPeerScopeId, true);
		}

		private void RegisterMatrixPeerScopes(MatrixHeading headings, int scopeID)
		{
			if (headings != null)
			{
				while (true)
				{
					if (headings != null)
					{
						if (headings.Grouping != null)
						{
							break;
						}
						this.RegisterPeerScopes(headings.ReportItems, scopeID, false);
						headings = headings.SubHeading;
						continue;
					}
					return;
				}
				if (headings.Subtotal != null)
				{
					this.RegisterPeerScopes(headings.Subtotal.ReportItems, scopeID, false);
				}
			}
		}

		private void RegisterPeerScopes(ReportItemCollection reportItems, int scopeID, bool traverse)
		{
			if (reportItems != null && this.m_hasUserSortPeerScopes)
			{
				string currentScope = this.GetCurrentScope();
				if (!this.m_peerScopes.ContainsKey(currentScope))
				{
					int count = reportItems.Count;
					for (int i = 0; i < count; i++)
					{
						ReportItem reportItem = reportItems[i];
						if (reportItem is Rectangle)
						{
							this.RegisterPeerScopes(((Rectangle)reportItem).ReportItems, scopeID, traverse);
						}
						else if (reportItem is DataRegion && !this.m_peerScopes.ContainsKey(reportItem.Name))
						{
							this.m_peerScopes.Add(reportItem.Name, scopeID);
						}
						if (reportItem is CustomReportItem)
						{
							this.RegisterPeerScopes(((CustomReportItem)reportItem).AltReportItem, scopeID, traverse);
						}
						else if (traverse)
						{
							if (reportItem is Matrix)
							{
								this.RegisterPeerScopes(((Matrix)reportItem).CornerReportItems, scopeID, false);
								this.RegisterMatrixPeerScopes(((Matrix)reportItem).Columns, scopeID);
								this.RegisterMatrixPeerScopes(((Matrix)reportItem).Rows, scopeID);
							}
							else if (reportItem is Table)
							{
								Table table = reportItem as Table;
								if (table.HeaderRows != null)
								{
									int count2 = table.HeaderRows.Count;
									for (int j = 0; j < count2; j++)
									{
										this.RegisterPeerScopes(table.HeaderRows[j].ReportItems, scopeID, false);
									}
								}
								if (table.FooterRows != null)
								{
									int count2 = table.FooterRows.Count;
									for (int k = 0; k < count2; k++)
									{
										this.RegisterPeerScopes(table.FooterRows[k].ReportItems, scopeID, false);
									}
								}
							}
						}
					}
					if (!this.m_peerScopes.ContainsKey(currentScope))
					{
						this.m_peerScopes.Add(currentScope, scopeID);
					}
				}
			}
		}

		internal void RegisterUserSortInnerScope(TextBox textbox)
		{
			Global.Tracer.Assert(textbox.UserSort != null && textbox.UserSort.SortExpressionScopeString != null && this.m_userSortExpressionScopes != null && null != this.m_userSortTextboxes);
			string currentScope = this.GetCurrentScope();
			TextBoxList textBoxList = this.m_userSortExpressionScopes[textbox.UserSort.SortExpressionScopeString] as TextBoxList;
			if (textBoxList != null)
			{
				if (!textBoxList.Contains(textbox))
				{
					textBoxList.Add(textbox);
				}
			}
			else
			{
				textBoxList = new TextBoxList();
				textBoxList.Add(textbox);
				this.m_userSortExpressionScopes.Add(textbox.UserSort.SortExpressionScopeString, textBoxList);
			}
			textBoxList = (this.m_userSortTextboxes[currentScope] as TextBoxList);
			if (textBoxList != null)
			{
				if (!textBoxList.Contains(textbox))
				{
					textBoxList.Add(textbox);
				}
			}
			else
			{
				textBoxList = new TextBoxList();
				textBoxList.Add(textbox);
				this.m_userSortTextboxes.Add(currentScope, textBoxList);
			}
		}

		internal void ProcessUserSortInnerScope(string scopeName, bool isMatrixGroup, bool isMatrixColumnGroup)
		{
			TextBoxList textBoxList = this.m_userSortExpressionScopes[scopeName] as TextBoxList;
			if (textBoxList != null)
			{
				int count = textBoxList.Count;
				for (int i = 0; i < count; i++)
				{
					TextBox textBox = textBoxList[i];
					Global.Tracer.Assert(null != textBox.UserSort, "(null != textbox.UserSort)");
					if (isMatrixGroup && this.m_groupingScopesForRunningValuesInTablix != null)
					{
						string sortTargetScope = (textBox.UserSort.SortTarget != null) ? textBox.UserSort.SortTarget.ScopeName : null;
						textBox.UserSort.FoundSortExpressionScope = !this.m_groupingScopesForRunningValuesInTablix.HasRowColScopeConflict(textBox.TextBoxScope, sortTargetScope, isMatrixColumnGroup);
					}
					else
					{
						textBox.UserSort.FoundSortExpressionScope = true;
					}
					textBox.InitializeSortExpression(this, false);
				}
			}
		}

		internal void ValidateUserSortInnerScope(string scopeName)
		{
			TextBoxList textBoxList = this.m_userSortTextboxes[scopeName] as TextBoxList;
			if (textBoxList != null)
			{
				int count = textBoxList.Count;
				for (int i = 0; i < count; i++)
				{
					TextBox textBox = textBoxList[i];
					Global.Tracer.Assert(null != textBox.UserSort, "(null != textbox.UserSort)");
					if (!textBox.UserSort.FoundSortExpressionScope)
					{
						this.m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScope, Severity.Error, textBox.ObjectType, textBox.Name, "SortExpressionScope", textBox.UserSort.SortExpressionScopeString);
					}
					else
					{
						textBox.UserSort.SortExpressionScope = this.GetSortFilterScope(textBox.UserSort.SortExpressionScopeString);
					}
				}
				this.m_userSortTextboxes.Remove(scopeName);
			}
		}

		internal void RegisterSortFilterTextbox(TextBox textbox)
		{
			this.m_reportSortFilterTextboxes.Add(textbox);
		}

		internal void SetDataSetDetailUserSortFilter()
		{
			string dataSetName = this.GetDataSetName();
			Global.Tracer.Assert(null != dataSetName, "(null != currentDataset)");
			DataSet dataSet = this.m_reportScopes[dataSetName] as DataSet;
			Global.Tracer.Assert(null != dataSet, "(null != dataset)");
			dataSet.HasDetailUserSortFilter = true;
		}

		internal void CalculateSortFilterGroupingLists()
		{
			int count = this.m_reportSortFilterTextboxes.Count;
			for (int i = 0; i < count; i++)
			{
				TextBox textBox = this.m_reportSortFilterTextboxes[i];
				Global.Tracer.Assert(textBox != null && null != textBox.TextBoxScope);
				if (textBox.IsMatrixCellScope)
				{
					textBox.ContainingScopes = (this.m_reportGroupingLists["0_CellScope" + textBox.TextBoxScope] as GroupingList);
				}
				if (!textBox.IsMatrixCellScope || textBox.ContainingScopes == null)
				{
					textBox.ContainingScopes = (this.m_reportGroupingLists[textBox.TextBoxScope] as GroupingList);
				}
				Global.Tracer.Assert(null != textBox.ContainingScopes, "(null != textbox.ContainingScopes)");
				for (int j = 0; j < textBox.ContainingScopes.Count; j++)
				{
					textBox.ContainingScopes[j].SaveGroupExprValues = true;
				}
				if (textBox.IsDetailScope)
				{
					textBox.ContainingScopes = textBox.ContainingScopes.Clone();
					textBox.ContainingScopes.Add(null);
				}
				if (textBox.UserSort != null && textBox.UserSort.SortTarget != null)
				{
					string scopeName = textBox.UserSort.SortTarget.ScopeName;
					int num = (int)this.m_reportScopeDatasetIDs[scopeName];
					textBox.UserSort.DataSetID = num;
					if (textBox.UserSort.SortExpressionScope != null)
					{
						string scopeName2 = textBox.UserSort.SortExpressionScope.ScopeName;
						Global.Tracer.Assert(scopeName2 != null && null != scopeName, "(null != esScope && null != stScope)");
						int num2 = (int)this.m_reportScopeDatasetIDs[scopeName2];
						Global.Tracer.Assert(0 <= num2 && 0 <= num);
						if (num2 == num)
						{
							textBox.UserSort.GroupsInSortTarget = this.CalculateGroupingDifference(this.m_reportGroupingLists[scopeName2] as GroupingList, this.m_reportGroupingLists[scopeName] as GroupingList);
						}
						else
						{
							this.m_errorContext.Register(ProcessingErrorCode.rsInvalidExpressionScopeDataSet, Severity.Error, textBox.ObjectType, textBox.Name, "SortExpressionScope", textBox.UserSort.SortExpressionScopeString, "SortTarget");
						}
					}
					if (!this.m_errorContext.HasError)
					{
						textBox.AddToScopeSortFilterList();
					}
				}
			}
			count = ((this.m_report.SubReports != null) ? this.m_report.SubReports.Count : 0);
			for (int k = 0; k < count; k++)
			{
				SubReport subReport = this.m_report.SubReports[k];
				if (subReport.SubReportScope != null)
				{
					if (subReport.IsMatrixCellScope)
					{
						subReport.ContainingScopes = (this.m_reportGroupingLists["0_CellScope" + subReport.SubReportScope] as GroupingList);
					}
					else
					{
						subReport.ContainingScopes = (this.m_reportGroupingLists[subReport.SubReportScope] as GroupingList);
					}
					Global.Tracer.Assert(null != subReport.ContainingScopes, "(null != subReport.ContainingScopes)");
					for (int l = 0; l < subReport.ContainingScopes.Count; l++)
					{
						subReport.ContainingScopes[l].SaveGroupExprValues = true;
					}
				}
				if (subReport.IsDetailScope)
				{
					subReport.ContainingScopes = subReport.ContainingScopes.Clone();
					subReport.ContainingScopes.Add(null);
				}
			}
		}

		private GroupingList CalculateGroupingDifference(GroupingList expressionScope, GroupingList targetScope)
		{
			if (expressionScope != null && expressionScope.Count != 0)
			{
				if (targetScope != null && targetScope.Count != 0)
				{
					if (expressionScope.Count < targetScope.Count)
					{
						return null;
					}
					GroupingList groupingList = expressionScope.Clone();
					int count = targetScope.Count;
					int num = expressionScope.IndexOf(targetScope[0]);
					if (num < 0)
					{
						return groupingList;
					}
					Global.Tracer.Assert(num + count <= expressionScope.Count, "(startIndex + count <= expressionScope.Count)");
					groupingList.RemoveRange(0, num + 1);
					for (int i = 1; i < count; i++)
					{
						if (expressionScope[num + i] != targetScope[i])
						{
							return groupingList;
						}
						groupingList.RemoveAt(0);
					}
					return groupingList;
				}
				return expressionScope;
			}
			return null;
		}

		internal void TextboxWithDetailSortExpressionAdd(TextBox textbox)
		{
			Global.Tracer.Assert(null != this.m_detailSortExpressionScopeTextboxes, "(null != m_detailSortExpressionScopeTextboxes)");
			this.m_detailSortExpressionScopeTextboxes.Add(textbox);
		}

		internal void TextboxesWithDetailSortExpressionInitialize()
		{
			Global.Tracer.Assert(null != this.m_detailSortExpressionScopeTextboxes, "(null != m_detailSortExpressionScopeTextboxes)");
			int count = this.m_detailSortExpressionScopeTextboxes.Count;
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					this.m_detailSortExpressionScopeTextboxes[i].InitializeSortExpression(this, true);
				}
				this.m_detailSortExpressionScopeTextboxes.RemoveRange(0, count);
			}
		}
	}
}
