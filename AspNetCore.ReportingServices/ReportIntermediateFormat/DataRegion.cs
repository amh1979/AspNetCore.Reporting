using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class DataRegion : ReportItem, IPageBreakOwner, IAggregateHolder, IRunningValueHolder, ISortFilterScope, IReferenceable, IPersistable, IIndexedInCollection, IGloballyReferenceable, IGlobalIDOwner, IDomainScopeMemberCreator, IRIFReportDataScope, IRIFReportScope, IInstancePath, IRIFDataScope
	{
		internal enum ProcessingInnerGroupings
		{
			Column,
			Row
		}

		protected string m_dataSetName;

		protected ExpressionInfo m_noRowsMessage;

		protected int m_columnCount;

		protected int m_rowCount;

		protected List<int> m_repeatSiblings;

		protected ProcessingInnerGroupings m_processingInnerGrouping;

		protected Sorting m_sorting;

		protected List<Filter> m_filters;

		protected List<DataAggregateInfo> m_aggregates;

		protected List<DataAggregateInfo> m_postSortAggregates;

		protected List<RunningValueInfo> m_runningValues;

		protected List<DataAggregateInfo> m_cellAggregates;

		protected List<DataAggregateInfo> m_cellPostSortAggregates;

		protected List<RunningValueInfo> m_cellRunningValues;

		protected List<ExpressionInfo> m_userSortExpressions;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private bool m_needToCacheDataRows;

		private List<IInScopeEventSource> m_inScopeEventSources;

		protected InScopeSortFilterHashtable m_detailSortFiltersInScope;

		protected int m_indexInCollection = -1;

		protected int m_outerGroupingMaximumDynamicLevel;

		protected int m_outerGroupingDynamicMemberCount;

		protected int m_outerGroupingDynamicPathCount;

		protected int m_innerGroupingMaximumDynamicLevel;

		protected int m_innerGroupingDynamicMemberCount;

		protected int m_innerGroupingDynamicPathCount;

		protected PageBreak m_pageBreak;

		protected ExpressionInfo m_pageName;

		protected DataScopeInfo m_dataScopeInfo;

		private int? m_rowDomainScopeCount = null;

		private int? m_colDomainScopeCount = null;

		private bool m_isMatrixIDC;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataRegion.GetDeclaration();

		[NonSerialized]
		private bool m_rowScopeFound;

		[NonSerialized]
		private bool m_columnScopeFound;

		[NonSerialized]
		private bool m_hasDynamicColumnMember;

		[NonSerialized]
		private bool m_hasDynamicRowMember;

		[NonSerialized]
		private InitializationContext.ScopeChainInfo m_scopeChainInfo;

		[NonSerialized]
		protected DataSet m_cachedDataSet;

		[NonSerialized]
		protected PageBreakStates m_pagebreakState;

		[NonSerialized]
		protected RuntimeDataRegionObjReference m_runtimeDataRegionObj;

		[NonSerialized]
		protected List<int> m_outermostStaticColumnIndexes;

		[NonSerialized]
		protected List<int> m_outermostStaticRowIndexes;

		[NonSerialized]
		protected int m_currentCellInnerIndex;

		[NonSerialized]
		protected int m_sequentialColMemberInstanceIndex;

		[NonSerialized]
		protected int m_sequentialRowMemberInstanceIndex;

		[NonSerialized]
		protected Hashtable m_scopeNames;

		[NonSerialized]
		protected bool m_inTablixCell;

		[NonSerialized]
		protected bool[] m_isSortFilterTarget;

		[NonSerialized]
		protected bool[] m_isSortFilterExpressionScope;

		[NonSerialized]
		protected int[] m_sortFilterSourceDetailScopeInfo;

		[NonSerialized]
		protected int m_currentColDetailIndex = -1;

		[NonSerialized]
		protected int m_currentRowDetailIndex = -1;

		[NonSerialized]
		protected bool m_noRows;

		[NonSerialized]
		protected bool m_processCellRunningValues;

		[NonSerialized]
		protected bool m_processOutermostStaticCellRunningValues;

		[NonSerialized]
		private bool m_inOutermostStaticCells;

		[NonSerialized]
		protected DataRegionInstance m_currentDataRegionInstance;

		[NonSerialized]
		protected AggregateRowInfo m_dataTablixAggregateRowInfo;

		[NonSerialized]
		protected AggregateRowInfo[] m_outerGroupingAggregateRowInfo;

		[NonSerialized]
		protected int[] m_outerGroupingIndexes;

		[NonSerialized]
		protected IReference<RuntimeDataTablixGroupRootObj>[] m_currentOuterGroupRootObjs;

		[NonSerialized]
		protected IReference<RuntimeDataTablixGroupRootObj> m_currentOuterGroupRoot;

		[NonSerialized]
		private bool m_populatedParentReportScope;

		[NonSerialized]
		private IRIFReportDataScope m_parentReportScope;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_currentStreamingScopeInstance;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_cachedNoRowsStreamingScopeInstance;

		[NonSerialized]
		private List<ReportItem> m_dataRegionScopedItemsForDataProcessing;

		string IRIFDataScope.Name
		{
			get
			{
				return base.Name;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IRIFDataScope.DataScopeObjectType
		{
			get
			{
				return this.ObjectType;
			}
		}

		internal bool IsMatrixIDC
		{
			get
			{
				return this.m_isMatrixIDC;
			}
			set
			{
				this.m_isMatrixIDC = value;
			}
		}

		internal override bool IsDataRegion
		{
			get
			{
				return true;
			}
		}

		internal abstract HierarchyNodeList ColumnMembers
		{
			get;
		}

		internal abstract HierarchyNodeList RowMembers
		{
			get;
		}

		internal HierarchyNodeList OuterMembers
		{
			get
			{
				if (this.m_processingInnerGrouping == ProcessingInnerGroupings.Column)
				{
					return this.RowMembers;
				}
				return this.ColumnMembers;
			}
		}

		internal HierarchyNodeList InnerMembers
		{
			get
			{
				if (this.m_processingInnerGrouping == ProcessingInnerGroupings.Column)
				{
					return this.ColumnMembers;
				}
				return this.RowMembers;
			}
		}

		internal abstract RowList Rows
		{
			get;
		}

		internal string DataSetName
		{
			get
			{
				return this.m_dataSetName;
			}
			set
			{
				this.m_dataSetName = value;
			}
		}

		internal bool NoRows
		{
			get
			{
				return this.m_noRows;
			}
			set
			{
				this.m_noRows = value;
			}
		}

		internal ExpressionInfo NoRowsMessage
		{
			get
			{
				return this.m_noRowsMessage;
			}
			set
			{
				this.m_noRowsMessage = value;
			}
		}

		internal int ColumnCount
		{
			get
			{
				return this.m_columnCount;
			}
			set
			{
				this.m_columnCount = value;
			}
		}

		internal int RowCount
		{
			get
			{
				return this.m_rowCount;
			}
			set
			{
				this.m_rowCount = value;
			}
		}

		internal ProcessingInnerGroupings ProcessingInnerGrouping
		{
			get
			{
				return this.m_processingInnerGrouping;
			}
			set
			{
				this.m_processingInnerGrouping = value;
			}
		}

		internal List<int> RepeatSiblings
		{
			get
			{
				return this.m_repeatSiblings;
			}
			set
			{
				this.m_repeatSiblings = value;
			}
		}

		internal Sorting Sorting
		{
			get
			{
				return this.m_sorting;
			}
			set
			{
				this.m_sorting = value;
			}
		}

		internal List<Filter> Filters
		{
			get
			{
				return this.m_filters;
			}
			set
			{
				this.m_filters = value;
			}
		}

		internal bool HasFilters
		{
			get
			{
				if (this.m_filters != null)
				{
					return this.m_filters.Count > 0;
				}
				return false;
			}
		}

		internal List<DataAggregateInfo> Aggregates
		{
			get
			{
				return this.m_aggregates;
			}
			set
			{
				this.m_aggregates = value;
			}
		}

		internal List<DataAggregateInfo> PostSortAggregates
		{
			get
			{
				return this.m_postSortAggregates;
			}
			set
			{
				this.m_postSortAggregates = value;
			}
		}

		internal List<RunningValueInfo> RunningValues
		{
			get
			{
				return this.m_runningValues;
			}
			set
			{
				this.m_runningValues = value;
			}
		}

		internal List<DataAggregateInfo> CellAggregates
		{
			get
			{
				return this.m_cellAggregates;
			}
			set
			{
				this.m_cellAggregates = value;
			}
		}

		internal List<DataAggregateInfo> CellPostSortAggregates
		{
			get
			{
				return this.m_cellPostSortAggregates;
			}
			set
			{
				this.m_cellPostSortAggregates = value;
			}
		}

		internal List<RunningValueInfo> CellRunningValues
		{
			get
			{
				return this.m_cellRunningValues;
			}
			set
			{
				this.m_cellRunningValues = value;
			}
		}

		internal List<ExpressionInfo> UserSortExpressions
		{
			get
			{
				return this.m_userSortExpressions;
			}
			set
			{
				this.m_userSortExpressions = value;
			}
		}

		internal InScopeSortFilterHashtable DetailSortFiltersInScope
		{
			get
			{
				return this.m_detailSortFiltersInScope;
			}
			set
			{
				this.m_detailSortFiltersInScope = value;
			}
		}

		internal Hashtable ScopeNames
		{
			get
			{
				return this.m_scopeNames;
			}
			set
			{
				this.m_scopeNames = value;
			}
		}

		public IRIFReportDataScope ParentReportScope
		{
			get
			{
				if (!this.m_populatedParentReportScope)
				{
					this.m_parentReportScope = IDOwner.FindReportDataScope(base.ParentInstancePath);
					this.m_populatedParentReportScope = true;
				}
				return this.m_parentReportScope;
			}
		}

		public bool IsDataIntersectionScope
		{
			get
			{
				return false;
			}
		}

		public bool IsScope
		{
			get
			{
				return this.IsDataRegion;
			}
		}

		public bool IsGroup
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsColumnGroupingSwitched
		{
			get
			{
				return false;
			}
		}

		public IReference<IOnDemandScopeInstance> CurrentStreamingScopeInstance
		{
			get
			{
				return this.m_currentStreamingScopeInstance;
			}
		}

		public bool IsBoundToStreamingScopeInstance
		{
			get
			{
				return this.m_currentStreamingScopeInstance != null;
			}
		}

		internal RuntimeDataRegionObjReference RuntimeDataRegionObj
		{
			get
			{
				return this.m_runtimeDataRegionObj;
			}
			set
			{
				this.m_runtimeDataRegionObj = value;
			}
		}

		internal List<int> OutermostStaticColumnIndexes
		{
			get
			{
				return this.m_outermostStaticColumnIndexes;
			}
			set
			{
				this.m_outermostStaticColumnIndexes = value;
			}
		}

		internal List<int> OutermostStaticRowIndexes
		{
			get
			{
				return this.m_outermostStaticRowIndexes;
			}
			set
			{
				this.m_outermostStaticRowIndexes = value;
			}
		}

		internal int CurrentCellInnerIndex
		{
			get
			{
				return this.m_currentCellInnerIndex;
			}
		}

		internal IReference<RuntimeDataTablixGroupRootObj> CurrentOuterGroupRoot
		{
			get
			{
				return this.m_currentOuterGroupRoot;
			}
			set
			{
				this.m_currentOuterGroupRoot = value;
			}
		}

		internal IReference<RuntimeDataTablixGroupRootObj>[] CurrentOuterGroupRootObjs
		{
			get
			{
				return this.m_currentOuterGroupRootObjs;
			}
			set
			{
				this.m_currentOuterGroupRootObjs = value;
			}
		}

		internal int[] OuterGroupingIndexes
		{
			get
			{
				return this.m_outerGroupingIndexes;
			}
		}

		internal bool InTablixCell
		{
			get
			{
				return this.m_inTablixCell;
			}
			set
			{
				this.m_inTablixCell = value;
			}
		}

		internal bool[] IsSortFilterTarget
		{
			get
			{
				return this.m_isSortFilterTarget;
			}
			set
			{
				this.m_isSortFilterTarget = value;
			}
		}

		internal bool[] IsSortFilterExpressionScope
		{
			get
			{
				return this.m_isSortFilterExpressionScope;
			}
			set
			{
				this.m_isSortFilterExpressionScope = value;
			}
		}

		internal int[] SortFilterSourceDetailScopeInfo
		{
			get
			{
				return this.m_sortFilterSourceDetailScopeInfo;
			}
			set
			{
				this.m_sortFilterSourceDetailScopeInfo = value;
			}
		}

		internal int CurrentColDetailIndex
		{
			get
			{
				return this.m_currentColDetailIndex;
			}
			set
			{
				this.m_currentColDetailIndex = value;
			}
		}

		internal int CurrentRowDetailIndex
		{
			get
			{
				return this.m_currentRowDetailIndex;
			}
			set
			{
				this.m_currentRowDetailIndex = value;
			}
		}

		internal bool ProcessCellRunningValues
		{
			get
			{
				return this.m_processCellRunningValues;
			}
			set
			{
				this.m_processCellRunningValues = value;
			}
		}

		internal bool ProcessOutermostStaticCellRunningValues
		{
			get
			{
				return this.m_processOutermostStaticCellRunningValues;
			}
			set
			{
				this.m_processOutermostStaticCellRunningValues = value;
			}
		}

		internal bool InOutermostStaticCells
		{
			get
			{
				return this.m_inOutermostStaticCells;
			}
			set
			{
				this.m_inOutermostStaticCells = value;
			}
		}

		int ISortFilterScope.ID
		{
			get
			{
				return base.m_ID;
			}
		}

		string ISortFilterScope.ScopeName
		{
			get
			{
				return base.m_name;
			}
		}

		bool[] ISortFilterScope.IsSortFilterTarget
		{
			get
			{
				return this.m_isSortFilterTarget;
			}
			set
			{
				this.m_isSortFilterTarget = value;
			}
		}

		bool[] ISortFilterScope.IsSortFilterExpressionScope
		{
			get
			{
				return this.m_isSortFilterExpressionScope;
			}
			set
			{
				this.m_isSortFilterExpressionScope = value;
			}
		}

		List<ExpressionInfo> ISortFilterScope.UserSortExpressions
		{
			get
			{
				return this.m_userSortExpressions;
			}
			set
			{
				this.m_userSortExpressions = value;
			}
		}

		IndexedExprHost ISortFilterScope.UserSortExpressionsHost
		{
			get
			{
				return this.UserSortExpressionsHost;
			}
		}

		protected abstract IndexedExprHost UserSortExpressionsHost
		{
			get;
		}

		internal bool ColumnScopeFound
		{
			get
			{
				return this.m_columnScopeFound;
			}
			set
			{
				this.m_columnScopeFound = value;
			}
		}

		internal bool RowScopeFound
		{
			get
			{
				return this.m_rowScopeFound;
			}
			set
			{
				this.m_rowScopeFound = value;
			}
		}

		public int IndexInCollection
		{
			get
			{
				return this.m_indexInCollection;
			}
			set
			{
				this.m_indexInCollection = value;
			}
		}

		public IndexedInCollectionType IndexedInCollectionType
		{
			get
			{
				return IndexedInCollectionType.DataRegion;
			}
		}

		internal DataRegionInstance CurrentDataRegionInstance
		{
			get
			{
				return this.m_currentDataRegionInstance;
			}
			set
			{
				this.m_currentDataRegionInstance = value;
			}
		}

		internal List<IInScopeEventSource> InScopeEventSources
		{
			get
			{
				return this.m_inScopeEventSources;
			}
		}

		internal int OuterGroupingMaximumDynamicLevel
		{
			get
			{
				return this.m_outerGroupingMaximumDynamicLevel;
			}
		}

		internal int OuterGroupingDynamicMemberCount
		{
			get
			{
				return this.m_outerGroupingDynamicMemberCount;
			}
		}

		internal int OuterGroupingDynamicPathCount
		{
			get
			{
				return this.m_outerGroupingDynamicPathCount;
			}
		}

		internal InitializationContext.ScopeChainInfo ScopeChainInfo
		{
			get
			{
				return this.m_scopeChainInfo;
			}
			set
			{
				this.m_scopeChainInfo = value;
			}
		}

		internal int InnerGroupingMaximumDynamicLevel
		{
			get
			{
				return this.m_innerGroupingMaximumDynamicLevel;
			}
		}

		internal int InnerGroupingDynamicMemberCount
		{
			get
			{
				return this.m_innerGroupingDynamicMemberCount;
			}
		}

		internal int InnerGroupingDynamicPathCount
		{
			get
			{
				return this.m_innerGroupingDynamicPathCount;
			}
		}

		internal int RowDomainScopeCount
		{
			get
			{
				if (this.RowMembers == null)
				{
					this.m_rowDomainScopeCount = 0;
				}
				else if (!this.m_rowDomainScopeCount.HasValue)
				{
					this.m_rowDomainScopeCount = this.RowMembers.Count - this.RowMembers.OriginalNodeCount;
				}
				return this.m_rowDomainScopeCount.Value;
			}
		}

		internal int ColumnDomainScopeCount
		{
			get
			{
				if (this.ColumnMembers == null)
				{
					this.m_colDomainScopeCount = 0;
				}
				else if (!this.m_colDomainScopeCount.HasValue)
				{
					this.m_colDomainScopeCount = this.ColumnMembers.Count - this.ColumnMembers.OriginalNodeCount;
				}
				return this.m_colDomainScopeCount.Value;
			}
		}

		internal List<ReportItem> DataRegionScopedItemsForDataProcessing
		{
			get
			{
				if (this.m_dataRegionScopedItemsForDataProcessing == null)
				{
					this.m_dataRegionScopedItemsForDataProcessing = this.ComputeDataRegionScopedItemsForDataProcessing();
				}
				return this.m_dataRegionScopedItemsForDataProcessing;
			}
		}

		bool IRIFReportScope.NeedToCacheDataRows
		{
			get
			{
				return this.m_needToCacheDataRows;
			}
			set
			{
				if (!this.m_needToCacheDataRows)
				{
					this.m_needToCacheDataRows = value;
				}
			}
		}

		public DataScopeInfo DataScopeInfo
		{
			get
			{
				return this.m_dataScopeInfo;
			}
		}

		internal ExpressionInfo PageName
		{
			get
			{
				return this.m_pageName;
			}
			set
			{
				this.m_pageName = value;
			}
		}

		internal PageBreak PageBreak
		{
			get
			{
				return this.m_pageBreak;
			}
			set
			{
				this.m_pageBreak = value;
			}
		}

		PageBreak IPageBreakOwner.PageBreak
		{
			get
			{
				return this.m_pageBreak;
			}
			set
			{
				this.m_pageBreak = value;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IPageBreakOwner.ObjectType
		{
			get
			{
				return this.ObjectType;
			}
		}

		string IPageBreakOwner.ObjectName
		{
			get
			{
				return base.m_name;
			}
		}

		IInstancePath IPageBreakOwner.InstancePath
		{
			get
			{
				return this;
			}
		}

		protected DataRegion(ReportItem parent)
			: base(parent)
		{
		}

		protected DataRegion(int id, ReportItem parent)
			: base(id, parent)
		{
			this.m_aggregates = new List<DataAggregateInfo>();
			this.m_postSortAggregates = new List<DataAggregateInfo>();
			this.m_runningValues = new List<RunningValueInfo>();
			this.m_cellRunningValues = new List<RunningValueInfo>();
			this.m_dataScopeInfo = new DataScopeInfo(id);
		}

		bool IRIFReportScope.VariableInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(this.m_variablesInScope, sequenceIndex, true);
		}

		bool IRIFReportScope.TextboxInScope(int sequenceIndex)
		{
			return SequenceIndex.GetBit(this.m_textboxesInScope, sequenceIndex, true);
		}

		public bool IsSameOrChildScopeOf(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsSameOrChildScope(this, candidateScope);
		}

		public bool IsChildScopeOf(IRIFReportDataScope candidateScope)
		{
			return DataScopeInfo.IsChildScopeOf(this, candidateScope);
		}

		public void ResetAggregates(AggregatesImpl reportOmAggregates)
		{
			reportOmAggregates.ResetAll(this.m_aggregates);
			reportOmAggregates.ResetAll(this.m_postSortAggregates);
			reportOmAggregates.ResetAll(this.m_runningValues);
			if (this.m_dataScopeInfo != null)
			{
				this.m_dataScopeInfo.ResetAggregates(reportOmAggregates);
			}
		}

		public bool HasServerAggregate(string aggregateName)
		{
			return DataScopeInfo.ContainsServerAggregate(this.m_aggregates, aggregateName);
		}

		public void BindToStreamingScopeInstance(IReference<IOnDemandScopeInstance> scopeInstance)
		{
			this.m_currentStreamingScopeInstance = scopeInstance;
		}

		public void BindToNoRowsScopeInstance(OnDemandProcessingContext odpContext)
		{
			if (this.m_cachedNoRowsStreamingScopeInstance == null)
			{
				StreamingNoRowsDataRegionInstance memberOwner = new StreamingNoRowsDataRegionInstance(odpContext, this);
				this.m_cachedNoRowsStreamingScopeInstance = new SyntheticOnDemandMemberOwnerInstanceReference(memberOwner);
			}
			this.m_currentStreamingScopeInstance = this.m_cachedNoRowsStreamingScopeInstance;
		}

		public void ClearStreamingScopeInstanceBinding()
		{
			this.m_currentStreamingScopeInstance = null;
		}

		protected virtual List<ReportItem> ComputeDataRegionScopedItemsForDataProcessing()
		{
			List<ReportItem> result = null;
			if (this.OutermostStaticRowIndexes != null && this.OutermostStaticColumnIndexes != null)
			{
				foreach (int outermostStaticRowIndex in this.OutermostStaticRowIndexes)
				{
					foreach (int outermostStaticColumnIndex in this.OutermostStaticColumnIndexes)
					{
						Cell rifCell = this.Rows[outermostStaticRowIndex].Cells[outermostStaticColumnIndex];
						DataRegion.MergeDataProcessingItems(rifCell, ref result);
					}
				}
			}
			if (this.OuterMembers != null)
			{
				DataRegion.MergeDataProcessingItems(this.OuterMembers.StaticMembersInSameScope, ref result);
			}
			if (this.InnerMembers != null)
			{
				DataRegion.MergeDataProcessingItems(this.InnerMembers.StaticMembersInSameScope, ref result);
			}
			return result;
		}

		private static void MergeDataProcessingItems(HierarchyNodeList staticMembers, ref List<ReportItem> results)
		{
			if (staticMembers != null)
			{
				for (int i = 0; i < staticMembers.Count; i++)
				{
					RuntimeRICollection.MergeDataProcessingItems(staticMembers[i].MemberContentCollection, ref results);
				}
			}
		}

		protected static void MergeDataProcessingItems(Cell rifCell, ref List<ReportItem> results)
		{
			if (rifCell != null)
			{
				RuntimeRICollection.MergeDataProcessingItems(rifCell.CellContentCollection, ref results);
			}
		}

		void IRIFReportScope.AddInScopeTextBox(TextBox textbox)
		{
			this.AddInScopeTextBox(textbox);
		}

		protected virtual void AddInScopeTextBox(TextBox textbox)
		{
		}

		void IRIFReportScope.ResetTextBoxImpls(OnDemandProcessingContext context)
		{
			this.ResetTextBoxImpls(context);
		}

		internal virtual void ResetTextBoxImpls(OnDemandProcessingContext context)
		{
		}

		void IRIFReportScope.AddInScopeEventSource(IInScopeEventSource eventSource)
		{
			if (this.m_inScopeEventSources == null)
			{
				this.m_inScopeEventSources = new List<IInScopeEventSource>();
			}
			this.m_inScopeEventSources.Add(eventSource);
		}

		public virtual void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context)
		{
			ReportHierarchyNode reportHierarchyNode = this.CreateHierarchyNode(context.GenerateID());
			reportHierarchyNode.Grouping = grouping.CloneForDomainScope(context, reportHierarchyNode);
			bool isColumn = parentNode != null && parentNode.IsColumn;
			HierarchyNodeList hierarchyNodeList = (parentNode != null) ? parentNode.InnerHierarchy : this.RowMembers;
			if (hierarchyNodeList != null)
			{
				hierarchyNodeList.Add(reportHierarchyNode);
				reportHierarchyNode.IsColumn = isColumn;
				this.CreateDomainScopeRowsAndCells(context, reportHierarchyNode);
			}
		}

		protected virtual void CreateDomainScopeRowsAndCells(AutomaticSubtotalContext context, ReportHierarchyNode member)
		{
			if (!member.IsColumn)
			{
				Row row = this.CreateRow(context.GenerateID(), this.ColumnCount);
				for (int i = 0; i < this.ColumnCount; i++)
				{
					row.Cells.Add(this.CreateCell(context.GenerateID(), -1, i));
				}
				this.Rows.Insert(this.RowMembers.GetMemberIndex(member), row);
				this.RowCount++;
			}
			else
			{
				int memberIndex = this.ColumnMembers.GetMemberIndex(member);
				for (int j = 0; j < this.RowCount; j++)
				{
					this.Rows[j].Cells.Insert(memberIndex, this.CreateCell(context.GenerateID(), j, -1));
				}
				this.ColumnCount++;
			}
		}

		protected virtual ReportHierarchyNode CreateHierarchyNode(int id)
		{
			return null;
		}

		protected virtual Row CreateRow(int id, int columnCount)
		{
			return null;
		}

		protected virtual Cell CreateCell(int id, int rowIndex, int colIndex)
		{
			return null;
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.IsDataRegionScopedCell = true;
			base.Initialize(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context);
			}
			this.m_dataScopeInfo.ValidateScopeRulesForIdc(context, this);
			if (context.PublishingContext.PublishingVersioning.IsRdlFeatureRestricted(RdlFeatures.PeerGroups) && context.HasPeerGroups(this))
			{
				string propertyName = "TablixMembers";
				if (this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataShape)
				{
					propertyName = "DataShapeMembers";
				}
				else if (this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart)
				{
					propertyName = "ChartMembers";
				}
				else if (this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem)
				{
					propertyName = "DataMembers";
				}
				else if (this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map)
				{
					propertyName = "MapMember";
				}
				else if (this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel)
				{
					propertyName = "GaugeMember";
				}
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidPeerGroupsNotSupported, Severity.Error, context.ObjectType, context.ObjectName, propertyName);
			}
			if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion) == (AspNetCore.ReportingServices.ReportPublishing.LocationFlags)0)
			{
				return false;
			}
			if (this.IsDataRegion)
			{
				this.m_dataScopeInfo.Initialize(context, this);
			}
			context.InitializeAbsolutePosition(this);
			context.UpdateTopLeftDataRegion(this);
			context.InAutoSubtotalClone = false;
			if (this.m_pageBreak != null)
			{
				this.m_pageBreak.Initialize(context);
			}
			if (this.m_pageName != null)
			{
				this.m_pageName.Initialize("PageName", context);
				context.ExprHostBuilder.PageName(this.m_pageName);
			}
			if (this.m_sorting != null)
			{
				this.m_sorting.Initialize(context);
			}
			if (this.m_filters != null)
			{
				for (int i = 0; i < this.m_filters.Count; i++)
				{
					this.m_filters[i].Initialize(context);
				}
			}
			if (this.m_noRowsMessage != null)
			{
				this.m_noRowsMessage.Initialize("NoRows", context);
				context.ExprHostBuilder.GenericNoRows(this.m_noRowsMessage);
			}
			if (this.m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				for (int j = 0; j < this.m_userSortExpressions.Count; j++)
				{
					ExpressionInfo expression = this.m_userSortExpressions[j];
					context.ExprHostBuilder.UserSortExpression(expression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			context.RegisterRunningValues(this.m_runningValues, this.m_dataScopeInfo.RunningValuesOfAggregates);
			context.IsTopLevelCellContents = false;
			this.InitializeCorner(context);
			context.ResetMemberAndCellIndexInCollectionTable();
			context.Location &= ~AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem;
			bool flag = this.InitializeRows(context);
			if (this.ValidateInnerStructure(context))
			{
				context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionGroupHeader;
				bool flag2 = this.InitializeMembers(context);
				context.Location &= ~AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionGroupHeader;
				if (flag2 && flag)
				{
					this.InitializeData(context);
					this.m_outerGroupingMaximumDynamicLevel = this.GetMaximumDynamicLevelAndAssignHierarchyIndexes(this.OuterMembers, 0, ref this.m_outerGroupingDynamicMemberCount, ref this.m_outerGroupingDynamicPathCount);
					this.m_innerGroupingMaximumDynamicLevel = this.GetMaximumDynamicLevelAndAssignHierarchyIndexes(this.InnerMembers, 0, ref this.m_innerGroupingDynamicMemberCount, ref this.m_innerGroupingDynamicPathCount);
				}
			}
			context.UnRegisterRunningValues(this.m_runningValues, this.m_dataScopeInfo.RunningValuesOfAggregates);
			if (this.IsDataRegion)
			{
				if (context.EvaluateAtomicityCondition(this.m_sorting != null && !this.m_sorting.NaturalSort, this, AtomicityReason.Sorts) || context.EvaluateAtomicityCondition(this.m_filters != null, this, AtomicityReason.Filters) || context.EvaluateAtomicityCondition(this.HasAggregatesForAtomicityCheck(), this, AtomicityReason.Aggregates) || context.EvaluateAtomicityCondition(context.HasMultiplePeerChildScopes(this), this, AtomicityReason.PeerChildScopes))
				{
					context.FoundAtomicScope(this);
				}
				else
				{
					this.m_dataScopeInfo.IsDecomposable = true;
				}
			}
			return false;
		}

		private bool HasAggregatesForAtomicityCheck()
		{
			if (!DataScopeInfo.HasNonServerAggregates(this.m_aggregates) && !DataScopeInfo.HasAggregates(this.m_postSortAggregates) && !DataScopeInfo.HasAggregates(this.m_runningValues))
			{
				return this.m_dataScopeInfo.HasAggregatesOrRunningValues;
			}
			return true;
		}

		private int GetMaximumDynamicLevelAndAssignHierarchyIndexes(HierarchyNodeList members, int parentDynamicLevels, ref int hierarchyDynamicIndex, ref int hierarchyPathIndex)
		{
			if (members == null)
			{
				return parentDynamicLevels;
			}
			int count = members.Count;
			int num = parentDynamicLevels;
			for (int i = 0; i < count; i++)
			{
				int val = parentDynamicLevels;
				ReportHierarchyNode reportHierarchyNode = members[i];
				if (!reportHierarchyNode.IsStatic)
				{
					reportHierarchyNode.HierarchyDynamicIndex = hierarchyDynamicIndex++;
					if (reportHierarchyNode.HasInnerDynamic)
					{
						reportHierarchyNode.HierarchyPathIndex = hierarchyPathIndex;
						val = this.GetMaximumDynamicLevelAndAssignHierarchyIndexes(reportHierarchyNode.InnerDynamicMembers, parentDynamicLevels + 1, ref hierarchyDynamicIndex, ref hierarchyPathIndex);
					}
					else
					{
						reportHierarchyNode.HierarchyPathIndex = hierarchyPathIndex++;
						val = parentDynamicLevels + 1;
					}
				}
				else if (reportHierarchyNode.HasInnerDynamic)
				{
					val = this.GetMaximumDynamicLevelAndAssignHierarchyIndexes(reportHierarchyNode.InnerDynamicMembers, parentDynamicLevels, ref hierarchyDynamicIndex, ref hierarchyPathIndex);
				}
				num = Math.Max(num, val);
			}
			return num;
		}

		protected GroupingList GenerateUserSortGroupingList(bool rowIsInnerGrouping)
		{
			GroupingList groupingList = new GroupingList();
			HierarchyNodeList members = rowIsInnerGrouping ? this.RowMembers : this.ColumnMembers;
			this.AddGroupsToList(members, groupingList);
			members = (rowIsInnerGrouping ? this.ColumnMembers : this.RowMembers);
			this.AddGroupsToList(members, groupingList);
			return groupingList;
		}

		private void AddGroupsToList(HierarchyNodeList members, GroupingList groups)
		{
			foreach (ReportHierarchyNode member in members)
			{
				if (member.Grouping != null)
				{
					groups.Add(member.Grouping);
				}
				if (member.InnerHierarchy != null)
				{
					this.AddGroupsToList(member.InnerHierarchy, groups);
				}
			}
		}

		protected virtual bool InitializeRows(InitializationContext context)
		{
			bool result = true;
			if (this.ColumnMembers != null && this.RowMembers != null)
			{
				goto IL_001a;
			}
			if (this.Rows == null)
			{
				goto IL_001a;
			}
			goto IL_004d;
			IL_004d:
			context.ErrorContext.Register((ProcessingErrorCode)((context.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart) ? 228 : 235), Severity.Error, context.ObjectType, context.ObjectName, this.m_rowCount.ToString(CultureInfo.InvariantCulture));
			return false;
			IL_001a:
			if (this.ColumnMembers != null && this.RowMembers != null && this.Rows == null)
			{
				goto IL_004d;
			}
			if (this.Rows != null && this.Rows.Count != this.m_rowCount)
			{
				goto IL_004d;
			}
			if (this.Rows != null)
			{
				for (int i = 0; i < this.Rows.Count; i++)
				{
					Row row = this.Rows[i];
					if (row == null || row.Cells == null || row.Cells.Count != this.m_columnCount)
					{
						context.ErrorContext.Register((ProcessingErrorCode)((context.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem) ? 236 : 229), Severity.Error, context.ObjectType, context.ObjectName, (context.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem) ? "DataCell" : "ChartDataPoint", i.ToString(CultureInfo.CurrentCulture));
						result = false;
					}
					row.Initialize(context);
				}
			}
			return result;
		}

		protected virtual void InitializeCorner(InitializationContext context)
		{
		}

		protected abstract bool ValidateInnerStructure(InitializationContext context);

		protected virtual bool InitializeMembers(InitializationContext context)
		{
			bool flag = true;
			if (this.m_rowCount != 0 && this.m_columnCount != 0 && (this.Rows == null || this.Rows.Count == 0))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsMissingDataCells, Severity.Error, context.ObjectType, context.ObjectName, "DataRows");
				flag = false;
			}
			context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixColumnHierarchy;
			flag &= this.InitializeColumnMembers(context);
			context.ResetMemberAndCellIndexInCollectionTable();
			context.Location &= ~AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixColumnHierarchy;
			context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixRowHierarchy;
			flag &= this.InitializeRowMembers(context);
			context.ResetMemberAndCellIndexInCollectionTable();
			context.Location &= ~AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixRowHierarchy;
			return flag;
		}

		protected virtual bool InitializeColumnMembers(InitializationContext context)
		{
			HierarchyNodeList columnMembers = this.ColumnMembers;
			context.MemberCellIndex = 0;
			if (columnMembers != null && columnMembers.Count != 0)
			{
				foreach (ReportHierarchyNode item in columnMembers)
				{
					context.InAutoSubtotalClone = item.IsAutoSubtotal;
					this.m_hasDynamicColumnMember |= item.Initialize(context);
				}
				if (columnMembers.Count == 1 && columnMembers[0].IsStatic)
				{
					context.SpecialTransferRunningValues(columnMembers[0].RunningValues, columnMembers[0].DataScopeInfo.RunningValuesOfAggregates);
				}
				return true;
			}
			return false;
		}

		protected virtual bool InitializeRowMembers(InitializationContext context)
		{
			HierarchyNodeList rowMembers = this.RowMembers;
			context.MemberCellIndex = 0;
			if (rowMembers != null && rowMembers.Count != 0)
			{
				foreach (ReportHierarchyNode item in rowMembers)
				{
					this.m_hasDynamicRowMember |= item.Initialize(context);
				}
				if (rowMembers.Count == 1 && rowMembers[0].IsStatic)
				{
					context.SpecialTransferRunningValues(rowMembers[0].RunningValues, rowMembers[0].DataScopeInfo.RunningValuesOfAggregates);
				}
				return true;
			}
			return false;
		}

		protected virtual void InitializeData(InitializationContext context)
		{
			this.m_textboxesInScope = context.GetCurrentReferencableTextboxes();
			this.m_variablesInScope = context.GetCurrentReferencableVariables();
			if (context.ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart)
			{
				context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem;
			}
			context.TablixName = base.m_name;
			DataRegion dataRegion = context.RegisterDataRegionCellScope(this, this.m_columnCount == 1 && this.ColumnMembers[0].Grouping == null, this.m_aggregates, this.m_postSortAggregates);
			int num = 0;
			for (int i = 0; i < this.RowMembers.Count; i++)
			{
				this.InitializeDataRows(ref num, this.RowMembers[i], context);
			}
			if (context.IsRunningValueDirectionColumn || (!this.m_hasDynamicRowMember && this.m_hasDynamicColumnMember))
			{
				this.m_processingInnerGrouping = ProcessingInnerGroupings.Row;
			}
			if (this.IsColumnGroupingSwitched && this.m_hasDynamicColumnMember)
			{
				this.m_processingInnerGrouping = ProcessingInnerGroupings.Row;
			}
			context.UnRegisterTablixCellScope(dataRegion);
		}

		protected void InitializeDataRows(ref int index, ReportHierarchyNode member, InitializationContext context)
		{
			member.HierarchyParentGroups = context.GetContainingScopesInCurrentDataRegion();
			bool suspendErrors = context.ErrorContext.SuspendErrors;
			bool inRecursiveHierarchyRows = context.InRecursiveHierarchyRows;
			context.ErrorContext.SuspendErrors |= member.IsAutoSubtotal;
			context.InAutoSubtotalClone = member.IsAutoSubtotal;
			bool registeredVisibility = member.PreInitializeDataMember(context);
			member.CaptureReferencableTextboxes(context);
			if (member.Grouping != null)
			{
				context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping;
				if (member.Grouping.IsDetail)
				{
					context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail;
				}
				context.IsDataRegionScopedCell = false;
				if (member.Grouping.Variables != null)
				{
					context.RegisterVariables(member.Grouping.Variables);
				}
				context.RegisterGroupingScopeForDataRegionCell(member);
				context.InRecursiveHierarchyRows = (member.Grouping.Parent != null);
			}
			else if (member.IsNonToggleableHiddenMember)
			{
				context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InNonToggleableHiddenStaticTablixMember;
			}
			if (member.InnerHierarchy == null)
			{
				this.InitializeDataColumns(member.ID, index, context);
				index++;
			}
			else
			{
				HierarchyNodeList innerHierarchy = member.InnerHierarchy;
				for (int i = 0; i < innerHierarchy.Count; i++)
				{
					this.InitializeDataRows(ref index, innerHierarchy[i], context);
				}
			}
			member.PostInitializeDataMember(context, registeredVisibility);
			if (member.Grouping != null)
			{
				context.UnRegisterGroupingScopeForDataRegionCell(member);
				if (member.Grouping.Variables != null)
				{
					context.UnregisterVariables(member.Grouping.Variables);
				}
			}
			context.InRecursiveHierarchyRows = inRecursiveHierarchyRows;
			context.ErrorContext.SuspendErrors = suspendErrors;
		}

		protected virtual void InitializeDataColumns(int parentRowID, int rowIndex, InitializationContext context)
		{
			int num = 0;
			for (int i = 0; i < this.ColumnMembers.Count; i++)
			{
				this.InitializeDataColumns(parentRowID, rowIndex, ref num, this.ColumnMembers[i], context);
			}
		}

		protected virtual void InitializeDataColumns(int parentRowID, int rowIndex, ref int columnIndex, ReportHierarchyNode member, InitializationContext context)
		{
			member.HierarchyParentGroups = context.GetContainingScopesInCurrentDataRegion();
			bool suspendErrors = context.ErrorContext.SuspendErrors;
			bool inRecursiveHierarchyColumns = context.InRecursiveHierarchyColumns;
			context.ErrorContext.SuspendErrors |= member.IsAutoSubtotal;
			context.InAutoSubtotalClone = member.IsAutoSubtotal;
			bool registeredVisibility = member.PreInitializeDataMember(context);
			member.CaptureReferencableTextboxes(context);
			if (member.Grouping != null)
			{
				context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping;
				if (member.Grouping.IsDetail)
				{
					context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail;
				}
				context.IsDataRegionScopedCell = false;
				if (member.Grouping.Variables != null)
				{
					context.RegisterVariables(member.Grouping.Variables);
				}
				context.RegisterGroupingScopeForDataRegionCell(member);
				context.InRecursiveHierarchyColumns = (member.Grouping.Parent != null);
			}
			else if (member.IsNonToggleableHiddenMember)
			{
				context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InNonToggleableHiddenStaticTablixMember;
			}
			if (member.InnerHierarchy == null)
			{
				context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InTablixCell;
				if (context.CellHasDynamicRowsAndColumns)
				{
					context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDynamicTablixCell;
				}
				if (this.Rows[rowIndex].Cells != null && rowIndex < this.Rows.Count && columnIndex < this.Rows[rowIndex].Cells.Count)
				{
					Cell cell = this.Rows[rowIndex].Cells[columnIndex];
					cell.Initialize(parentRowID, member.ID, rowIndex, columnIndex, context);
					if ((context.ObjectType != AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix || !context.HasUserSorts) && !context.IsDataRegionScopedCell)
					{
						this.CopyCellAggregates(cell);
					}
				}
				columnIndex++;
			}
			else
			{
				HierarchyNodeList innerHierarchy = member.InnerHierarchy;
				for (int i = 0; i < innerHierarchy.Count; i++)
				{
					this.InitializeDataColumns(parentRowID, rowIndex, ref columnIndex, innerHierarchy[i], context);
				}
			}
			member.PostInitializeDataMember(context, registeredVisibility);
			if (member.Grouping != null)
			{
				context.UnRegisterGroupingScopeForDataRegionCell(member);
				if (member.Grouping.Variables != null)
				{
					context.UnregisterVariables(member.Grouping.Variables);
				}
			}
			context.InRecursiveHierarchyColumns = inRecursiveHierarchyColumns;
			context.ErrorContext.SuspendErrors = suspendErrors;
		}

		internal override void InitializeRVDirectionDependentItems(InitializationContext context)
		{
			if (this.IsDataRegion && context.RegisterDataRegion(this))
			{
				DataRegion dataRegion = null;
				context.IsDataRegionScopedCell = true;
				context.Location |= (AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataSet | AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegion);
				context.ObjectType = this.ObjectType;
				context.ObjectName = base.Name;
				context.RegisterRunningValues(this.m_runningValues, this.m_dataScopeInfo.RunningValuesOfAggregates);
				this.InitializeRVDirectionDependentItemsInCorner(context);
				dataRegion = context.RegisterDataRegionCellScope(this, this.m_columnCount == 1 && this.ColumnMembers[0].Grouping == null, this.m_aggregates, this.m_postSortAggregates);
				context.Location &= ~AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDataRegionCellTopLevelItem;
				this.InitializeRVDirectionDependentItems(context, false);
				this.InitializeRVDirectionDependentItems(context, true);
				int num = 0;
				int num2 = 0;
				this.InitializeRVDirectionDependentItems(ref num, ref num2, context, false, true);
				context.ProcessUserSortScopes(base.m_name);
				context.EventSourcesWithDetailSortExpressionInitialize(base.m_name);
				context.UnRegisterRunningValues(this.m_runningValues, this.m_dataScopeInfo.RunningValuesOfAggregates);
				context.UnRegisterTablixCellScope(dataRegion);
				context.UnRegisterDataRegion(this);
			}
		}

		private void InitializeRVDirectionDependentItems(InitializationContext context, bool traverseInner)
		{
			int num = 0;
			int num2 = 0;
			this.InitializeRVDirectionDependentItems(ref num, ref num2, context, traverseInner, false);
		}

		private void InitializeRVDirectionDependentItems(ref int outerIndex, ref int innerIndex, InitializationContext context, bool traverseInner, bool initializeCells)
		{
			HierarchyNodeList hierarchyNodeList = (this.m_processingInnerGrouping == ProcessingInnerGroupings.Column == traverseInner) ? this.ColumnMembers : this.RowMembers;
			for (int i = 0; i < hierarchyNodeList.Count; i++)
			{
				this.InitializeRVDirectionDependentItems(ref outerIndex, ref innerIndex, hierarchyNodeList[i], context, traverseInner, initializeCells);
			}
		}

		private void InitializeRVDirectionDependentItems(ref int outerIndex, ref int innerIndex, ReportHierarchyNode member, InitializationContext context, bool traverseInner, bool initializeCells)
		{
			member.HierarchyParentGroups = context.GetContainingScopesInCurrentDataRegion();
			if (member.Grouping != null)
			{
				context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping;
				context.ObjectName = member.Grouping.Name;
				context.IsDataRegionScopedCell = false;
				context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping;
				List<ExpressionInfo> groupExpressions = member.Grouping.GroupExpressions;
				if (groupExpressions == null || groupExpressions.Count == 0)
				{
					context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail;
				}
				context.RegisterGroupingScopeForDataRegionCell(member);
				if (member.Grouping.Variables != null)
				{
					context.RegisterVariables(member.Grouping.Variables);
				}
			}
			if (!initializeCells)
			{
				member.InitializeRVDirectionDependentItems(context);
			}
			if (member.InnerHierarchy == null)
			{
				if (initializeCells)
				{
					if (traverseInner)
					{
						this.InitializeRVDirectionDependentItems(outerIndex, innerIndex, context);
						innerIndex++;
					}
					else
					{
						innerIndex = 0;
						this.InitializeRVDirectionDependentItems(ref outerIndex, ref innerIndex, context, true, initializeCells);
						outerIndex++;
					}
				}
			}
			else
			{
				HierarchyNodeList innerHierarchy = member.InnerHierarchy;
				for (int i = 0; i < innerHierarchy.Count; i++)
				{
					this.InitializeRVDirectionDependentItems(ref outerIndex, ref innerIndex, innerHierarchy[i], context, traverseInner, initializeCells);
				}
			}
			if (member.Grouping != null)
			{
				if (initializeCells)
				{
					context.ProcessUserSortScopes(member.Grouping.Name);
					context.EventSourcesWithDetailSortExpressionInitialize(member.Grouping.Name);
				}
				context.UnRegisterGroupingScopeForDataRegionCell(member);
				if (member.Grouping.Variables != null)
				{
					context.UnregisterVariables(member.Grouping.Variables);
				}
			}
		}

		protected virtual void InitializeRVDirectionDependentItemsInCorner(InitializationContext context)
		{
		}

		protected virtual void InitializeRVDirectionDependentItems(int outerIndex, int innerIndex, InitializationContext context)
		{
		}

		internal override void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
			this.DetermineGroupingExprValueCountInCorner(context, groupingExprCount);
			int num = 0;
			int num2 = 0;
			this.DetermineGroupingExprValueCount(ref num, ref num2, context, false, groupingExprCount);
		}

		private void DetermineGroupingExprValueCount(ref int outerIndex, ref int innerIndex, InitializationContext context, bool traverseInner, int groupingExprCount)
		{
			HierarchyNodeList hierarchyNodeList = (this.m_processingInnerGrouping == ProcessingInnerGroupings.Column == traverseInner) ? this.ColumnMembers : this.RowMembers;
			for (int i = 0; i < hierarchyNodeList.Count; i++)
			{
				this.DetermineGroupingExprValueCount(ref outerIndex, ref innerIndex, hierarchyNodeList[i], context, traverseInner, groupingExprCount);
			}
		}

		private void DetermineGroupingExprValueCount(ref int outerIndex, ref int innerIndex, ReportHierarchyNode member, InitializationContext context, bool traverseInner, int groupingExprCount)
		{
			if (member.Grouping != null)
			{
				List<ExpressionInfo> groupExpressions = member.Grouping.GroupExpressions;
				if (groupExpressions != null)
				{
					groupingExprCount += groupExpressions.Count;
				}
				context.AddGroupingExprCountForGroup(member.Grouping.Name, groupingExprCount);
			}
			member.DetermineGroupingExprValueCount(context, groupingExprCount);
			if (member.InnerHierarchy == null)
			{
				if (traverseInner)
				{
					this.DetermineGroupingExprValueCount(outerIndex, innerIndex, context, groupingExprCount);
					innerIndex++;
				}
				else
				{
					innerIndex = 0;
					this.DetermineGroupingExprValueCount(ref outerIndex, ref innerIndex, context, true, groupingExprCount);
					outerIndex++;
				}
			}
			else
			{
				HierarchyNodeList innerHierarchy = member.InnerHierarchy;
				for (int i = 0; i < innerHierarchy.Count; i++)
				{
					this.DetermineGroupingExprValueCount(ref outerIndex, ref innerIndex, innerHierarchy[i], context, traverseInner, groupingExprCount);
				}
			}
		}

		protected virtual void DetermineGroupingExprValueCountInCorner(InitializationContext context, int groupingExprCount)
		{
		}

		protected virtual void DetermineGroupingExprValueCount(int outerIndex, int innerIndex, InitializationContext context, int groupingExprCount)
		{
		}

		protected void CopyCellAggregates(Cell cell)
		{
			this.CopyCellAggregates<DataAggregateInfo>(cell.Aggregates, ref this.m_cellAggregates);
			this.CopyCellAggregates<DataAggregateInfo>(cell.PostSortAggregates, ref this.m_cellPostSortAggregates);
			this.CopyCellAggregates<RunningValueInfo>(cell.RunningValues, ref this.m_cellRunningValues);
		}

		private void CopyCellAggregates<AggregateType>(List<AggregateType> aggregates, ref List<AggregateType> dataRegionCellAggregates) where AggregateType : DataAggregateInfo, new()
		{
			if (aggregates != null && aggregates.Count != 0)
			{
				if (dataRegionCellAggregates == null)
				{
					dataRegionCellAggregates = new List<AggregateType>();
				}
				dataRegionCellAggregates.AddRange((IEnumerable<AggregateType>)aggregates);
			}
		}

		internal DataSet GetDataSet(Report reportDefinition)
		{
			if (this.m_cachedDataSet == null)
			{
				Global.Tracer.Assert(null != reportDefinition, "(null != reportDefinition)");
				if (this.m_dataScopeInfo != null && this.m_dataScopeInfo.DataSet != null)
				{
					this.m_cachedDataSet = this.m_dataScopeInfo.DataSet;
				}
				else if (this.m_dataSetName == null)
				{
					this.m_dataSetName = reportDefinition.FirstDataSet.Name;
					this.m_cachedDataSet = reportDefinition.FirstDataSet;
				}
				else
				{
					this.m_cachedDataSet = reportDefinition.MappingNameToDataSet[this.m_dataSetName];
				}
			}
			return this.m_cachedDataSet;
		}

		List<DataAggregateInfo> IAggregateHolder.GetAggregateList()
		{
			return this.m_aggregates;
		}

		List<DataAggregateInfo> IAggregateHolder.GetPostSortAggregateList()
		{
			return this.m_postSortAggregates;
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_aggregates, "(null != m_aggregates)");
			if (this.m_aggregates.Count == 0)
			{
				this.m_aggregates = null;
			}
			Global.Tracer.Assert(null != this.m_postSortAggregates, "(null != m_postSortAggregates)");
			if (this.m_postSortAggregates.Count == 0)
			{
				this.m_postSortAggregates = null;
			}
		}

		List<RunningValueInfo> IRunningValueHolder.GetRunningValueList()
		{
			return this.m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			if (this.m_runningValues != null && this.m_runningValues.Count == 0)
			{
				this.m_runningValues = null;
			}
			if (this.m_cellRunningValues != null && this.m_cellRunningValues.Count == 0)
			{
				this.m_cellRunningValues = null;
			}
		}

		internal void ConvertCellAggregatesToIndexes()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			if (this.m_cellAggregates != null)
			{
				DataRegion.GenerateAggregateIndexMapping(this.m_cellAggregates, dictionary);
			}
			if (this.m_cellPostSortAggregates != null)
			{
				DataRegion.GenerateAggregateIndexMapping(this.m_cellPostSortAggregates, dictionary2);
			}
			if (this.m_cellRunningValues != null)
			{
				DataRegion.GenerateAggregateIndexMapping(this.m_cellRunningValues, dictionary3);
			}
			for (int i = 0; i < this.m_rowCount && this.Rows.Count > i; i++)
			{
				Row row = this.Rows[i];
				for (int j = 0; j < this.m_columnCount; j++)
				{
					if (row.Cells == null)
					{
						break;
					}
					if (row.Cells.Count <= j)
					{
						break;
					}
					Cell cell = row.Cells[j];
					if (cell != null)
					{
						cell.GenerateAggregateIndexes(dictionary, dictionary2, dictionary3);
					}
				}
			}
		}

		private static void GenerateAggregateIndexMapping<AggregateType>(List<AggregateType> cellAggregates, Dictionary<string, int> aggregateIndexes) where AggregateType : DataAggregateInfo
		{
			int count = cellAggregates.Count;
			for (int i = 0; i < count; i++)
			{
				AggregateType val = cellAggregates[i];
				aggregateIndexes.Add(val.Name, i);
				int num = (val.DuplicateNames != null) ? val.DuplicateNames.Count : 0;
				for (int j = 0; j < num; j++)
				{
					string key = val.DuplicateNames[j];
					if (!aggregateIndexes.ContainsKey(key))
					{
						aggregateIndexes.Add(key, i);
					}
				}
			}
		}

		protected override InstancePathItem CreateInstancePathItem()
		{
			if (this.IsDataRegion)
			{
				return new InstancePathItem(InstancePathItemType.DataRegion, this.IndexInCollection);
			}
			return new InstancePathItem();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			DataRegion dataRegion = (DataRegion)base.PublishClone(context);
			dataRegion.m_dataScopeInfo = this.m_dataScopeInfo.PublishClone(context, dataRegion.ID);
			context.CurrentDataRegionClone = dataRegion;
			context.AddAggregateHolder(dataRegion);
			context.AddRunningValueHolder(dataRegion);
			if (this.m_dataSetName != null)
			{
				dataRegion.m_dataSetName = (string)this.m_dataSetName.Clone();
			}
			context.RegisterClonedScopeName(base.m_name, dataRegion.m_name);
			context.AddSortTarget(dataRegion.m_name, dataRegion);
			if (this.m_noRowsMessage != null)
			{
				dataRegion.m_noRowsMessage = (ExpressionInfo)this.m_noRowsMessage.PublishClone(context);
			}
			if (this.m_repeatSiblings != null)
			{
				dataRegion.m_repeatSiblings = new List<int>(this.m_repeatSiblings.Count);
				foreach (int repeatSibling in this.m_repeatSiblings)
				{
					dataRegion.m_repeatSiblings.Add(repeatSibling);
				}
			}
			if (this.m_sorting != null)
			{
				dataRegion.m_sorting = (Sorting)this.m_sorting.PublishClone(context);
			}
			if (this.m_filters != null)
			{
				dataRegion.m_filters = new List<Filter>(this.m_filters.Count);
				foreach (Filter filter in this.m_filters)
				{
					dataRegion.m_filters.Add((Filter)filter.PublishClone(context));
				}
			}
			if (this.m_pageBreak != null)
			{
				dataRegion.m_pageBreak = (PageBreak)this.m_pageBreak.PublishClone(context);
			}
			if (this.m_pageName != null)
			{
				dataRegion.m_pageName = (ExpressionInfo)this.m_pageName.PublishClone(context);
			}
			if (this.m_detailSortFiltersInScope != null)
			{
				dataRegion.m_detailSortFiltersInScope = new InScopeSortFilterHashtable(this.m_detailSortFiltersInScope.Count);
				IDictionaryEnumerator enumerator3 = this.m_detailSortFiltersInScope.GetEnumerator();
				try
				{
					while (enumerator3.MoveNext())
					{
						object current3 = enumerator3.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)current3;
						List<int> list = (List<int>)dictionaryEntry.Value;
						List<int> list2 = new List<int>(list.Count);
						foreach (int item in list)
						{
							list2.Add(item);
						}
						dataRegion.m_detailSortFiltersInScope.Add(dictionaryEntry.Key, list2);
					}
					return dataRegion;
				}
				finally
				{
					IDisposable disposable = enumerator3 as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			return dataRegion;
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			if (this.IsDataRegion)
			{
				visitor.PreVisit(this);
			}
			this.TraverseDataRegionLevelScopes(visitor);
			this.TraverseMembers(visitor, this.RowMembers);
			this.TraverseMembers(visitor, this.ColumnMembers);
			int num = 0;
			int num2 = 0;
			this.TraverseScopes(visitor, this.RowMembers, ref num, ref num2);
			if (this.IsDataRegion)
			{
				visitor.PostVisit(this);
			}
		}

		private void TraverseMembers(IRIFScopeVisitor visitor, HierarchyNodeList members)
		{
			if (members != null)
			{
				foreach (ReportHierarchyNode member in members)
				{
					this.TraversMembers(visitor, member);
				}
			}
		}

		private void TraversMembers(IRIFScopeVisitor visitor, ReportHierarchyNode member)
		{
			if (!member.IsStatic)
			{
				visitor.PreVisit(member);
			}
			member.TraverseMemberScopes(visitor);
			this.TraverseMembers(visitor, member.InnerHierarchy);
			if (!member.IsStatic)
			{
				visitor.PostVisit(member);
			}
		}

		protected virtual void TraverseDataRegionLevelScopes(IRIFScopeVisitor visitor)
		{
		}

		private void TraverseScopes(IRIFScopeVisitor visitor, HierarchyNodeList members, ref int rowCellIndex, ref int colCellIndex)
		{
			if (members != null)
			{
				foreach (ReportHierarchyNode member in members)
				{
					this.TraverseScopes(visitor, member, ref rowCellIndex, ref colCellIndex);
				}
			}
		}

		private void TraverseScopes(IRIFScopeVisitor visitor, ReportHierarchyNode member, ref int rowCellIndex, ref int colCellIndex)
		{
			if (member != null)
			{
				if (!member.IsStatic)
				{
					visitor.PreVisit(member);
				}
				if (member.InnerHierarchy == null || member.InnerHierarchy.Count == 0)
				{
					if (member.IsColumn)
					{
						RowList rows = this.Rows;
						if (rows != null && rows.Count > rowCellIndex)
						{
							Row row = rows[rowCellIndex];
							if (row != null && row.Cells != null && row.Cells.Count > colCellIndex)
							{
								Cell cell = row.Cells[colCellIndex];
								this.TraverseScopes(visitor, cell, rowCellIndex, colCellIndex);
							}
						}
						colCellIndex++;
					}
					else
					{
						colCellIndex = 0;
						this.TraverseScopes(visitor, this.ColumnMembers, ref rowCellIndex, ref colCellIndex);
						rowCellIndex++;
					}
				}
				else
				{
					this.TraverseScopes(visitor, member.InnerHierarchy, ref rowCellIndex, ref colCellIndex);
				}
				if (!member.IsStatic)
				{
					visitor.PostVisit(member);
				}
			}
		}

		protected void TraverseScopes(IRIFScopeVisitor visitor, Cell cell, int rowIndex, int colIndex)
		{
			if (cell != null)
			{
				cell.TraverseScopes(visitor, rowIndex, colIndex);
			}
		}

		protected void BuildAndSetupAxisScopeTreeForAutoSubtotals(ref AutomaticSubtotalContext context, ReportHierarchyNode member)
		{
			int startIndex = context.StartIndex;
			this.FindClonedScopesForAutoSubtotals(false, member, context.ScopeNamesToClone, ref startIndex);
		}

		private void FindClonedScopesForAutoSubtotals(bool register, ReportHierarchyNode member, Dictionary<string, IRIFDataScope> scopesToClone, ref int memberCellIndex)
		{
			if (member != null)
			{
				if (!member.IsStatic && register)
				{
					scopesToClone.Add(member.Grouping.Name, member);
				}
				TablixMember tablixMember = member as TablixMember;
				if (tablixMember != null && tablixMember.TablixHeader != null)
				{
					TablixHeader tablixHeader = tablixMember.TablixHeader;
					this.FindClonedScopesForAutoSubtotals(tablixHeader.CellContents, scopesToClone);
					this.FindClonedScopesForAutoSubtotals(tablixHeader.AltCellContents, scopesToClone);
				}
				if (member.InnerHierarchy == null || member.InnerHierarchy.Count == 0)
				{
					RowList rows = this.Rows;
					if (rows != null && this.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix)
					{
						if (member.IsColumn)
						{
							foreach (Row item in rows)
							{
								if (item != null && item.Cells != null && item.Cells.Count > memberCellIndex)
								{
									this.FindClonedScopesForAutoSubtotals((TablixCellBase)item.Cells[memberCellIndex], scopesToClone);
								}
							}
						}
						else if (rows.Count > memberCellIndex)
						{
							Row row2 = rows[memberCellIndex];
							if (row2 != null && row2.Cells != null)
							{
								foreach (TablixCellBase cell in row2.Cells)
								{
									this.FindClonedScopesForAutoSubtotals(cell, scopesToClone);
								}
							}
						}
					}
					memberCellIndex++;
				}
				else
				{
					this.FindClonedScopesForAutoSubtotals(register, member.InnerHierarchy, scopesToClone, ref memberCellIndex);
				}
			}
		}

		private void FindClonedScopesForAutoSubtotals(TablixCellBase cell, Dictionary<string, IRIFDataScope> scopesToClone)
		{
			if (cell != null)
			{
				this.FindClonedScopesForAutoSubtotals(cell.CellContents, scopesToClone);
				this.FindClonedScopesForAutoSubtotals(cell.AltCellContents, scopesToClone);
			}
		}

		private void FindClonedScopesForAutoSubtotals(ReportItem item, Dictionary<string, IRIFDataScope> scopesToClone)
		{
			if (item != null)
			{
				switch (item.ObjectType)
				{
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel:
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart:
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix:
				{
					DataRegion dataRegion = (DataRegion)item;
					scopesToClone.Add(dataRegion.Name, dataRegion);
					int num = 0;
					dataRegion.FindClonedScopesForAutoSubtotals(true, dataRegion.OuterMembers, scopesToClone, ref num);
					int num2 = 0;
					dataRegion.FindClonedScopesForAutoSubtotals(true, dataRegion.InnerMembers, scopesToClone, ref num2);
					Tablix tablix = dataRegion as Tablix;
					if (tablix != null && tablix.Corner != null)
					{
						foreach (List<TablixCornerCell> item2 in tablix.Corner)
						{
							if (item2 != null)
							{
								foreach (TablixCornerCell item3 in item2)
								{
									dataRegion.FindClonedScopesForAutoSubtotals(item3, scopesToClone);
								}
							}
						}
					}
					break;
				}
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map:
				{
					Map map = (Map)item;
					if (map.MapDataRegions != null)
					{
						foreach (MapDataRegion mapDataRegion in map.MapDataRegions)
						{
							this.FindClonedScopesForAutoSubtotals(mapDataRegion, scopesToClone);
						}
					}
					break;
				}
				case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle:
				{
					Rectangle rectangle = (Rectangle)item;
					if (rectangle.ReportItems != null)
					{
						foreach (ReportItem reportItem in rectangle.ReportItems)
						{
							this.FindClonedScopesForAutoSubtotals(reportItem, scopesToClone);
						}
					}
					break;
				}
				}
			}
		}

		private void FindClonedScopesForAutoSubtotals(bool register, HierarchyNodeList members, Dictionary<string, IRIFDataScope> scopesToClone, ref int memberCellIndex)
		{
			if (members != null)
			{
				foreach (ReportHierarchyNode member in members)
				{
					this.FindClonedScopesForAutoSubtotals(register, member, scopesToClone, ref memberCellIndex);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			list.Add(new MemberInfo(MemberName.NoRowsMessage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColumnCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.RowCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.ProcessingInnerGrouping, Token.Enum));
			list.Add(new MemberInfo(MemberName.RepeatSiblings, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.Sorting, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sorting));
			list.Add(new MemberInfo(MemberName.Filters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter));
			list.Add(new MemberInfo(MemberName.Aggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.PostSortAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.RunningValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo));
			list.Add(new MemberInfo(MemberName.CellAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.CellPostSortAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.CellRunningValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo));
			list.Add(new MemberInfo(MemberName.UserSortExpressions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.NeedToCacheDataRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeEventSources, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.OuterGroupingMaximumDynamicLevel, Token.Int32));
			list.Add(new MemberInfo(MemberName.OuterGroupingDynamicMemberCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.OuterGroupingDynamicPathCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.InnerGroupingMaximumDynamicLevel, Token.Int32));
			list.Add(new MemberInfo(MemberName.InnerGroupingDynamicMemberCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.InnerGroupingDynamicPathCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.TextboxesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.PageBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak));
			list.Add(new MemberInfo(MemberName.PageName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataScopeInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo));
			list.Add(new MemberInfo(MemberName.RowDomainScopeCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColumnDomainScopeCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.IsMatrixIDC, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(DataRegion.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					writer.Write(this.m_dataSetName);
					break;
				case MemberName.NoRowsMessage:
					writer.Write(this.m_noRowsMessage);
					break;
				case MemberName.ColumnCount:
					writer.Write(this.m_columnCount);
					break;
				case MemberName.RowCount:
					writer.Write(this.m_rowCount);
					break;
				case MemberName.ProcessingInnerGrouping:
					writer.WriteEnum((int)this.m_processingInnerGrouping);
					break;
				case MemberName.RepeatSiblings:
					writer.WriteListOfPrimitives(this.m_repeatSiblings);
					break;
				case MemberName.Sorting:
					writer.Write(this.m_sorting);
					break;
				case MemberName.Filters:
					writer.Write(this.m_filters);
					break;
				case MemberName.Aggregates:
					writer.Write(this.m_aggregates);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(this.m_postSortAggregates);
					break;
				case MemberName.RunningValues:
					writer.Write(this.m_runningValues);
					break;
				case MemberName.CellAggregates:
					writer.Write(this.m_cellAggregates);
					break;
				case MemberName.CellPostSortAggregates:
					writer.Write(this.m_cellPostSortAggregates);
					break;
				case MemberName.CellRunningValues:
					writer.Write(this.m_cellRunningValues);
					break;
				case MemberName.UserSortExpressions:
					writer.Write(this.m_userSortExpressions);
					break;
				case MemberName.DetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(this.m_detailSortFiltersInScope);
					break;
				case MemberName.IndexInCollection:
					writer.Write(this.m_indexInCollection);
					break;
				case MemberName.NeedToCacheDataRows:
					writer.Write(this.m_needToCacheDataRows);
					break;
				case MemberName.InScopeEventSources:
					writer.WriteListOfReferences(this.m_inScopeEventSources);
					break;
				case MemberName.OuterGroupingMaximumDynamicLevel:
					writer.Write(this.m_outerGroupingMaximumDynamicLevel);
					break;
				case MemberName.OuterGroupingDynamicMemberCount:
					writer.Write(this.m_outerGroupingDynamicMemberCount);
					break;
				case MemberName.OuterGroupingDynamicPathCount:
					writer.Write(this.m_outerGroupingDynamicPathCount);
					break;
				case MemberName.InnerGroupingMaximumDynamicLevel:
					writer.Write(this.m_innerGroupingMaximumDynamicLevel);
					break;
				case MemberName.InnerGroupingDynamicMemberCount:
					writer.Write(this.m_innerGroupingDynamicMemberCount);
					break;
				case MemberName.InnerGroupingDynamicPathCount:
					writer.Write(this.m_innerGroupingDynamicPathCount);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(this.m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(this.m_variablesInScope);
					break;
				case MemberName.PageBreak:
					writer.Write(this.m_pageBreak);
					break;
				case MemberName.PageName:
					writer.Write(this.m_pageName);
					break;
				case MemberName.DataScopeInfo:
					writer.Write(this.m_dataScopeInfo);
					break;
				case MemberName.RowDomainScopeCount:
					writer.Write(this.RowDomainScopeCount);
					break;
				case MemberName.ColumnDomainScopeCount:
					writer.Write(this.ColumnDomainScopeCount);
					break;
				case MemberName.IsMatrixIDC:
					writer.Write(this.m_isMatrixIDC);
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
			reader.RegisterDeclaration(DataRegion.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					this.m_dataSetName = reader.ReadString();
					break;
				case MemberName.NoRowsMessage:
					this.m_noRowsMessage = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColumnCount:
					this.m_columnCount = reader.ReadInt32();
					break;
				case MemberName.RowCount:
					this.m_rowCount = reader.ReadInt32();
					break;
				case MemberName.ProcessingInnerGrouping:
					this.m_processingInnerGrouping = (ProcessingInnerGroupings)reader.ReadEnum();
					break;
				case MemberName.RepeatSiblings:
					this.m_repeatSiblings = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.Sorting:
					this.m_sorting = (Sorting)reader.ReadRIFObject();
					break;
				case MemberName.Filters:
					this.m_filters = reader.ReadGenericListOfRIFObjects<Filter>();
					break;
				case MemberName.Aggregates:
					this.m_aggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.PostSortAggregates:
					this.m_postSortAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.RunningValues:
					this.m_runningValues = reader.ReadGenericListOfRIFObjects<RunningValueInfo>();
					break;
				case MemberName.CellAggregates:
					this.m_cellAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.CellPostSortAggregates:
					this.m_cellPostSortAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.CellRunningValues:
					this.m_cellRunningValues = reader.ReadGenericListOfRIFObjects<RunningValueInfo>();
					break;
				case MemberName.UserSortExpressions:
					this.m_userSortExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.DetailSortFiltersInScope:
					this.m_detailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.PageBreakLocation:
					this.m_pageBreak = new PageBreak();
					this.m_pageBreak.BreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.IndexInCollection:
					this.m_indexInCollection = reader.ReadInt32();
					break;
				case MemberName.NeedToCacheDataRows:
					this.m_needToCacheDataRows = reader.ReadBoolean();
					break;
				case MemberName.InScopeEventSources:
					this.m_inScopeEventSources = reader.ReadGenericListOfReferences<IInScopeEventSource>(this);
					break;
				case MemberName.OuterGroupingMaximumDynamicLevel:
					this.m_outerGroupingMaximumDynamicLevel = reader.ReadInt32();
					break;
				case MemberName.OuterGroupingDynamicMemberCount:
					this.m_outerGroupingDynamicMemberCount = reader.ReadInt32();
					break;
				case MemberName.OuterGroupingDynamicPathCount:
					this.m_outerGroupingDynamicPathCount = reader.ReadInt32();
					break;
				case MemberName.InnerGroupingMaximumDynamicLevel:
					this.m_innerGroupingMaximumDynamicLevel = reader.ReadInt32();
					break;
				case MemberName.InnerGroupingDynamicMemberCount:
					this.m_innerGroupingDynamicMemberCount = reader.ReadInt32();
					break;
				case MemberName.InnerGroupingDynamicPathCount:
					this.m_innerGroupingDynamicPathCount = reader.ReadInt32();
					break;
				case MemberName.TextboxesInScope:
					this.m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					this.m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.PageBreak:
					this.m_pageBreak = (PageBreak)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					this.m_pageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataScopeInfo:
					this.m_dataScopeInfo = (DataScopeInfo)reader.ReadRIFObject();
					break;
				case MemberName.RowDomainScopeCount:
					this.m_rowDomainScopeCount = reader.ReadInt32();
					break;
				case MemberName.ColumnDomainScopeCount:
					this.m_colDomainScopeCount = reader.ReadInt32();
					break;
				case MemberName.IsMatrixIDC:
					this.m_isMatrixIDC = reader.ReadBoolean();
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
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(DataRegion.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item2 in list)
				{
					MemberName memberName = item2.MemberName;
					if (memberName == MemberName.InScopeEventSources)
					{
						IReferenceable referenceable = default(IReferenceable);
						referenceableItems.TryGetValue(item2.RefID, out referenceable);
						IInScopeEventSource item = (IInScopeEventSource)referenceable;
						if (this.m_inScopeEventSources == null)
						{
							this.m_inScopeEventSources = new List<IInScopeEventSource>();
						}
						this.m_inScopeEventSources.Add(item);
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion;
		}

		internal abstract object EvaluateNoRowsMessageExpression();

		internal string EvaluateNoRowsMessage(AspNetCore.ReportingServices.OnDemandReportRendering.DataRegionInstance romInstance, OnDemandProcessingContext odpContext)
		{
			odpContext.SetupContext(this, romInstance);
			return odpContext.ReportRuntime.EvaluateDataRegionNoRowsExpression(this, this.ObjectType, base.m_name, "NoRowsMessage");
		}

		internal string EvaluatePageName(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateDataRegionPageNameExpression(this, this.m_pageName, this.ObjectType, base.Name);
		}

		protected void DataRegionSetExprHost(ReportItemExprHost exprHost, SortExprHost sortExprHost, IList<FilterExprHost> FilterHostsRemotable, IndexedExprHost UserSortExpressionsHost, PageBreakExprHost pageBreakExprHost, IList<JoinConditionExprHost> joinConditionExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null, "(exprHost != null)");
			base.ReportItemSetExprHost(exprHost, reportObjectModel);
			if (sortExprHost != null)
			{
				Global.Tracer.Assert(null != this.m_sorting, "(null != m_sorting)");
				this.m_sorting.SetExprHost(sortExprHost, reportObjectModel);
			}
			if (FilterHostsRemotable != null)
			{
				Global.Tracer.Assert(this.m_filters != null, "(m_filters != null)");
				int count = this.m_filters.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_filters[i].SetExprHost(FilterHostsRemotable, reportObjectModel);
				}
			}
			if (UserSortExpressionsHost != null)
			{
				UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
			}
			if (this.m_pageBreak != null && pageBreakExprHost != null)
			{
				this.m_pageBreak.SetExprHost(pageBreakExprHost, reportObjectModel);
			}
			if (this.m_dataScopeInfo != null && this.m_dataScopeInfo.JoinInfo != null && joinConditionExprHost != null)
			{
				this.m_dataScopeInfo.JoinInfo.SetJoinConditionExprHost(joinConditionExprHost, reportObjectModel);
			}
		}

		internal abstract void DataRegionContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions);

		internal void SaveOuterGroupingAggregateRowInfo(int dynamicIndex, OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(null != this.m_outerGroupingAggregateRowInfo, "(null != m_outerGroupingAggregateRowInfo)");
			if (this.m_outerGroupingAggregateRowInfo[dynamicIndex] == null)
			{
				this.m_outerGroupingAggregateRowInfo[dynamicIndex] = new AggregateRowInfo();
			}
			this.m_outerGroupingAggregateRowInfo[dynamicIndex].SaveAggregateInfo(odpContext);
		}

		internal void SetDataTablixAggregateRowInfo(AggregateRowInfo aggregateRowInfo)
		{
			this.m_dataTablixAggregateRowInfo = aggregateRowInfo;
		}

		internal void SetCellAggregateRowInfo(int dynamicIndex, OnDemandProcessingContext odpContext)
		{
			Global.Tracer.Assert(this.m_outerGroupingAggregateRowInfo != null && null != this.m_dataTablixAggregateRowInfo, "(null != m_outerGroupingAggregateRowInfo && null != m_dataTablixAggregateRowInfo)");
			this.m_dataTablixAggregateRowInfo.CombineAggregateInfo(odpContext, this.m_outerGroupingAggregateRowInfo[dynamicIndex]);
		}

		internal void ResetInstancePathCascade()
		{
			int num = (this.RowMembers != null) ? this.RowMembers.Count : 0;
			for (int i = 0; i < num; i++)
			{
				this.RowMembers[i].ResetInstancePathCascade();
			}
			num = ((this.ColumnMembers != null) ? this.ColumnMembers.Count : 0);
			for (int j = 0; j < num; j++)
			{
				this.ColumnMembers[j].ResetInstancePathCascade();
			}
		}

		internal void ResetInstanceIndexes()
		{
			this.m_currentCellInnerIndex = 0;
			this.m_sequentialColMemberInstanceIndex = 0;
			this.m_sequentialRowMemberInstanceIndex = 0;
			this.m_outerGroupingIndexes = new int[this.OuterGroupingDynamicMemberCount];
			this.m_currentOuterGroupRootObjs = new IReference<RuntimeDataTablixGroupRootObj>[this.OuterGroupingDynamicMemberCount];
			this.m_outerGroupingAggregateRowInfo = new AggregateRowInfo[this.OuterGroupingDynamicMemberCount];
			this.m_currentOuterGroupRoot = null;
		}

		internal void UpdateOuterGroupingIndexes(IReference<RuntimeDataTablixGroupRootObj> groupRoot, int groupLeafIndex)
		{
			int hierarchyDynamicIndex = groupRoot.Value().HierarchyDef.HierarchyDynamicIndex;
			this.m_currentOuterGroupRootObjs[hierarchyDynamicIndex] = groupRoot;
			this.m_outerGroupingIndexes[hierarchyDynamicIndex] = groupLeafIndex;
		}

		internal void ResetOuterGroupingIndexesForOuterPeerGroup(int index)
		{
			for (int i = index; i < this.OuterGroupingDynamicMemberCount; i++)
			{
				this.m_currentOuterGroupRootObjs[i] = null;
				this.m_outerGroupingIndexes[i] = 0;
			}
		}

		internal void ResetOuterGroupingAggregateRowInfo()
		{
			Global.Tracer.Assert(null != this.m_outerGroupingAggregateRowInfo, "(null != m_outerGroupingAggregateRowInfo)");
			for (int i = 0; i < this.m_outerGroupingAggregateRowInfo.Length; i++)
			{
				this.m_outerGroupingAggregateRowInfo[i] = null;
			}
		}

		internal int AddMemberInstance(bool isColumn)
		{
			if (isColumn)
			{
				return ++this.m_sequentialColMemberInstanceIndex;
			}
			return this.m_sequentialRowMemberInstanceIndex;
		}

		internal void AddCell()
		{
			this.m_currentCellInnerIndex++;
		}

		internal void NewOuterCells()
		{
			if (0 < this.m_currentCellInnerIndex)
			{
				this.m_currentCellInnerIndex = 0;
			}
		}

		internal void ResetTopLevelDynamicMemberInstanceCount()
		{
			this.ResetTopLevelDynamicMemberInstanceCount(this.RowMembers);
			this.ResetTopLevelDynamicMemberInstanceCount(this.ColumnMembers);
		}

		private void ResetTopLevelDynamicMemberInstanceCount(HierarchyNodeList topLevelMembers)
		{
			if (topLevelMembers != null)
			{
				int count = topLevelMembers.Count;
				for (int i = 0; i < count; i++)
				{
					if (!topLevelMembers[i].IsStatic)
					{
						topLevelMembers[i].InstanceCount = 0;
					}
				}
			}
		}
	}
}
