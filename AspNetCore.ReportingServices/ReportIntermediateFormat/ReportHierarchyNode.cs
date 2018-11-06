using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class ReportHierarchyNode : IDOwner, IRunningValueHolder, IPersistable, ICustomPropertiesHolder, IIndexedInCollection, IGloballyReferenceable, IGlobalIDOwner, IStaticReferenceable, IRIFReportDataScope, IRIFReportScope, IInstancePath, IRIFDataScope
	{
		protected bool m_isColumn;

		protected int m_originalScopeID = -1;

		protected int m_level;

		protected Grouping m_grouping;

		protected Sorting m_sorting;

		protected List<ScopeIDType> m_memberGroupAndSortExpressionFlag;

		protected int m_memberCellIndex;

		protected int m_exprHostID = -1;

		protected int m_rowSpan;

		protected int m_colSpan;

		protected bool m_isAutoSubtotal;

		protected List<RunningValueInfo> m_runningValues;

		protected DataValueList m_customProperties;

		[Reference]
		protected DataRegion m_dataRegionDef;

		private int m_indexInCollection = -1;

		private bool m_needToCacheDataRows;

		private List<IInScopeEventSource> m_inScopeEventSources;

		private byte[] m_textboxesInScope;

		private byte[] m_variablesInScope;

		private int m_hierarchyDynamicIndex = -1;

		private int m_hierarchyPathIndex = -1;

		private GroupingList m_hierarchyParentGroups;

		private DataScopeInfo m_dataScopeInfo;

		private int? m_innerDomainScopeCount = null;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ReportHierarchyNode.GetDeclaration();

		[NonSerialized]
		private IMemberNode m_exprHost;

		[NonSerialized]
		private bool? m_hasInnerFilters;

		[NonSerialized]
		protected int m_cellStartIndex = -1;

		[NonSerialized]
		protected int m_cellEndIndex = -1;

		[NonSerialized]
		private List<int> m_cellIndexes;

		[NonSerialized]
		private Dictionary<string, Grouping>[] m_cellScopes;

		[NonSerialized]
		protected AggregatesImpl m_outermostStaticCellRVCol;

		[NonSerialized]
		protected AggregatesImpl[] m_outermostStaticCellScopedRVCollections;

		[NonSerialized]
		protected AggregatesImpl m_cellRVCol;

		[NonSerialized]
		protected AggregatesImpl[] m_cellScopedRVCollections;

		[NonSerialized]
		protected int m_staticRefId = -2147483648;

		[NonSerialized]
		private int m_currentMemberIndex = -1;

		[NonSerialized]
		private int m_currentDynamicInstanceCount = -1;

		[NonSerialized]
		private IRIFReportDataScope m_parentReportScope;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_currentStreamingScopeInstance;

		[NonSerialized]
		private IReference<IOnDemandScopeInstance> m_cachedNoRowsStreamingScopeInstance;

		[NonSerialized]
		private List<ReportItem> m_groupScopedContentsForProcessing;

		string IRIFDataScope.Name
		{
			get
			{
				if (this.m_grouping != null)
				{
					return this.m_grouping.Name;
				}
				return null;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IRIFDataScope.DataScopeObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping;
			}
		}

		public DataScopeInfo DataScopeInfo
		{
			get
			{
				return this.m_dataScopeInfo;
			}
		}

		internal abstract string RdlElementName
		{
			get;
		}

		internal abstract HierarchyNodeList InnerHierarchy
		{
			get;
		}

		internal bool IsColumn
		{
			get
			{
				return this.m_isColumn;
			}
			set
			{
				this.m_isColumn = value;
			}
		}

		internal bool IsDomainScope
		{
			get
			{
				return this.m_originalScopeID != -1;
			}
		}

		internal int OriginalScopeID
		{
			get
			{
				return this.m_originalScopeID;
			}
			set
			{
				this.m_originalScopeID = value;
			}
		}

		internal int Level
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = value;
			}
		}

		internal Grouping Grouping
		{
			get
			{
				return this.m_grouping;
			}
			set
			{
				this.m_grouping = value;
				if (this.m_grouping != null)
				{
					this.m_grouping.Owner = this;
				}
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

		internal List<ScopeIDType> MemberGroupAndSortExpressionFlag
		{
			get
			{
				return this.m_memberGroupAndSortExpressionFlag;
			}
		}

		internal int MemberCellIndex
		{
			get
			{
				return this.m_memberCellIndex;
			}
			set
			{
				this.m_memberCellIndex = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal int RowSpan
		{
			get
			{
				return this.m_rowSpan;
			}
			set
			{
				this.m_rowSpan = value;
			}
		}

		internal int ColSpan
		{
			get
			{
				return this.m_colSpan;
			}
			set
			{
				this.m_colSpan = value;
			}
		}

		internal bool IsAutoSubtotal
		{
			get
			{
				return this.m_isAutoSubtotal;
			}
			set
			{
				this.m_isAutoSubtotal = value;
			}
		}

		DataValueList ICustomPropertiesHolder.CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
		}

		IInstancePath ICustomPropertiesHolder.InstancePath
		{
			get
			{
				return this;
			}
		}

		internal DataValueList CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
			set
			{
				this.m_customProperties = value;
			}
		}

		internal DataRegion DataRegionDef
		{
			get
			{
				return this.m_dataRegionDef;
			}
			set
			{
				this.m_dataRegionDef = value;
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

		internal bool IsStatic
		{
			get
			{
				return this.m_grouping == null;
			}
		}

		internal bool IsLeaf
		{
			get
			{
				if (this.InnerHierarchy != null)
				{
					return this.InnerHierarchy.Count == 0;
				}
				return true;
			}
		}

		internal bool IsInnermostDynamicMember
		{
			get
			{
				if (this.InnerHierarchy != null)
				{
					return this.InnerHierarchy.DynamicMembersAtScope.Count == 0;
				}
				return true;
			}
		}

		internal virtual bool IsTablixMember
		{
			get
			{
				return false;
			}
		}

		internal int CurrentMemberIndex
		{
			get
			{
				return this.m_currentMemberIndex;
			}
			set
			{
				this.m_currentMemberIndex = value;
			}
		}

		internal int InstanceCount
		{
			get
			{
				if (this.IsStatic)
				{
					return 1;
				}
				return this.m_currentDynamicInstanceCount;
			}
			set
			{
				Global.Tracer.Assert(!this.IsStatic, "Cannot set instance count on static tablix member");
				this.m_currentDynamicInstanceCount = value;
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
				return IndexedInCollectionType.Member;
			}
		}

		internal bool HasInnerDynamic
		{
			get
			{
				if (this.InnerHierarchy == null)
				{
					return false;
				}
				return 0 != this.InnerHierarchy.DynamicMembersAtScope.Count;
			}
		}

		internal HierarchyNodeList InnerDynamicMembers
		{
			get
			{
				if (this.InnerHierarchy == null)
				{
					return null;
				}
				return this.InnerHierarchy.DynamicMembersAtScope;
			}
		}

		internal HierarchyNodeList InnerStaticMembersInSameScope
		{
			get
			{
				if (this.InnerHierarchy == null)
				{
					return null;
				}
				return this.InnerHierarchy.StaticMembersInSameScope;
			}
		}

		internal int CellStartIndex
		{
			get
			{
				if (this.m_cellStartIndex < 0)
				{
					this.CalculateDependencies();
				}
				return this.m_cellStartIndex;
			}
		}

		internal int CellEndIndex
		{
			get
			{
				if (this.m_cellEndIndex < 0)
				{
					this.CalculateDependencies();
				}
				return this.m_cellEndIndex;
			}
		}

		internal bool HasFilters
		{
			get
			{
				if (this.m_grouping != null)
				{
					return null != this.m_grouping.Filters;
				}
				return false;
			}
		}

		internal bool HasVariables
		{
			get
			{
				if (this.m_grouping != null)
				{
					return null != this.m_grouping.Variables;
				}
				return false;
			}
		}

		internal bool HasInnerFilters
		{
			get
			{
				if (!this.m_hasInnerFilters.HasValue)
				{
					this.m_hasInnerFilters = false;
					if (this.InnerHierarchy != null)
					{
						int count = this.InnerHierarchy.Count;
						int num = 0;
						while (!this.m_hasInnerFilters.Value && num < count)
						{
							this.m_hasInnerFilters = this.InnerHierarchy[num].HasFilters;
							if (!this.m_hasInnerFilters.Value)
							{
								this.m_hasInnerFilters = this.InnerHierarchy[num].HasInnerFilters;
							}
							num++;
						}
					}
				}
				return this.m_hasInnerFilters.Value;
			}
			set
			{
				this.m_hasInnerFilters = value;
			}
		}

		internal AggregatesImpl OutermostStaticCellRVCol
		{
			get
			{
				return this.m_outermostStaticCellRVCol;
			}
			set
			{
				this.m_outermostStaticCellRVCol = value;
			}
		}

		internal AggregatesImpl[] OutermostStaticCellScopedRVCollections
		{
			get
			{
				return this.m_outermostStaticCellScopedRVCollections;
			}
			set
			{
				this.m_outermostStaticCellScopedRVCollections = value;
			}
		}

		internal AggregatesImpl CellRVCol
		{
			get
			{
				return this.m_cellRVCol;
			}
			set
			{
				this.m_cellRVCol = value;
			}
		}

		internal AggregatesImpl[] CellScopedRVCollections
		{
			get
			{
				return this.m_cellScopedRVCollections;
			}
			set
			{
				this.m_cellScopedRVCollections = value;
			}
		}

		internal List<IInScopeEventSource> InScopeEventSources
		{
			get
			{
				return this.m_inScopeEventSources;
			}
		}

		public IRIFReportDataScope ParentReportScope
		{
			get
			{
				if (this.m_parentReportScope == null)
				{
					this.m_parentReportScope = IDOwner.FindReportDataScope(base.ParentInstancePath);
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
				return this.IsGroup;
			}
		}

		public bool IsGroup
		{
			get
			{
				return !this.IsStatic;
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

		internal int HierarchyDynamicIndex
		{
			get
			{
				return this.m_hierarchyDynamicIndex;
			}
			set
			{
				this.m_hierarchyDynamicIndex = value;
			}
		}

		internal int HierarchyPathIndex
		{
			get
			{
				return this.m_hierarchyPathIndex;
			}
			set
			{
				this.m_hierarchyPathIndex = value;
			}
		}

		internal Dictionary<string, Grouping>[] CellScopes
		{
			get
			{
				return this.m_cellScopes;
			}
			set
			{
				this.m_cellScopes = value;
			}
		}

		internal GroupingList HierarchyParentGroups
		{
			get
			{
				return this.m_hierarchyParentGroups;
			}
			set
			{
				this.m_hierarchyParentGroups = ((value != null && value.Count == 0) ? null : value);
			}
		}

		internal int InnerDomainScopeCount
		{
			get
			{
				if (!this.m_innerDomainScopeCount.HasValue)
				{
					if (this.InnerHierarchy == null)
					{
						this.m_innerDomainScopeCount = 0;
					}
					else
					{
						this.m_innerDomainScopeCount = this.InnerHierarchy.Count - this.InnerHierarchy.OriginalNodeCount;
					}
				}
				return this.m_innerDomainScopeCount.Value;
			}
		}

		internal List<ReportItem> GroupScopedContentsForProcessing
		{
			get
			{
				if (this.m_groupScopedContentsForProcessing == null)
				{
					this.m_groupScopedContentsForProcessing = this.ComputeMemberScopedItems();
				}
				return this.m_groupScopedContentsForProcessing;
			}
		}

		internal virtual List<ReportItem> MemberContentCollection
		{
			get
			{
				return null;
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

		private bool HasNaturalGroupAndNaturalSort
		{
			get
			{
				if (this.m_grouping != null && this.m_sorting != null && this.m_grouping.NaturalGroup)
				{
					return this.m_sorting.NaturalSort;
				}
				return false;
			}
		}

		internal virtual bool IsNonToggleableHiddenMember
		{
			get
			{
				return false;
			}
		}

		int IStaticReferenceable.ID
		{
			get
			{
				return this.m_staticRefId;
			}
		}

		internal ReportHierarchyNode()
		{
		}

		internal ReportHierarchyNode(int id, DataRegion dataRegionDef)
			: base(id)
		{
			this.m_dataRegionDef = dataRegionDef;
			this.m_runningValues = new List<RunningValueInfo>();
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

		public bool IsSameScope(IRIFReportDataScope candidateScope)
		{
			return this.DataScopeInfo.IsSameScope(candidateScope.DataScopeInfo);
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
			if (this.m_grouping != null)
			{
				this.m_grouping.ResetAggregates(reportOmAggregates);
				reportOmAggregates.ResetAll(this.m_runningValues);
				if (this.m_dataScopeInfo != null)
				{
					this.m_dataScopeInfo.ResetAggregates(reportOmAggregates);
				}
			}
		}

		public bool HasServerAggregate(string aggregateName)
		{
			return DataScopeInfo.ContainsServerAggregate(this.m_grouping.Aggregates, aggregateName);
		}

		public void BindToStreamingScopeInstance(IReference<IOnDemandScopeInstance> scopeInstance)
		{
			this.m_currentStreamingScopeInstance = scopeInstance;
		}

		public void BindToNoRowsScopeInstance(OnDemandProcessingContext odpContext)
		{
			if (this.m_cachedNoRowsStreamingScopeInstance == null)
			{
				StreamingNoRowsMemberInstance member = new StreamingNoRowsMemberInstance(odpContext, this);
				this.m_cachedNoRowsStreamingScopeInstance = new SyntheticOnDemandMemberInstanceReference(member);
			}
			this.m_currentStreamingScopeInstance = this.m_cachedNoRowsStreamingScopeInstance;
		}

		public void ClearStreamingScopeInstanceBinding()
		{
			this.m_currentStreamingScopeInstance = null;
		}

		internal Dictionary<string, Grouping> GetScopeNames()
		{
			Dictionary<string, Grouping> dictionary = new Dictionary<string, Grouping>();
			int num = (this.m_hierarchyParentGroups != null) ? this.m_hierarchyParentGroups.Count : 0;
			for (int i = 0; i < num; i++)
			{
				dictionary.Add(this.m_hierarchyParentGroups[i].Name, this.m_hierarchyParentGroups[i]);
			}
			if (!this.IsStatic)
			{
				dictionary.Add(this.m_grouping.Name, this.m_grouping);
			}
			return dictionary;
		}

		protected virtual List<ReportItem> ComputeMemberScopedItems()
		{
			List<ReportItem> result = null;
			RuntimeRICollection.MergeDataProcessingItems(this.MemberContentCollection, ref result);
			HierarchyNodeList innerStaticMembersInSameScope = this.InnerStaticMembersInSameScope;
			if (innerStaticMembersInSameScope != null)
			{
				{
					foreach (ReportHierarchyNode item in innerStaticMembersInSameScope)
					{
						RuntimeRICollection.MergeDataProcessingItems(item.MemberContentCollection, ref result);
					}
					return result;
				}
			}
			return result;
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

		internal virtual void TraverseMemberScopes(IRIFScopeVisitor visitor)
		{
		}

		internal virtual bool InnerInitialize(InitializationContext context, bool restrictive)
		{
			bool flag = false;
			if (this.InnerHierarchy != null)
			{
				bool handledCellContents = context.HandledCellContents;
				context.HandledCellContents = false;
				foreach (ReportHierarchyNode item in this.InnerHierarchy)
				{
					flag |= item.Initialize(context, false);
				}
				context.HandledCellContents = handledCellContents;
			}
			else
			{
				context.MemberCellIndex++;
			}
			return flag;
		}

		internal virtual bool Initialize(InitializationContext context)
		{
			return this.Initialize(context, true);
		}

		internal virtual bool Initialize(InitializationContext context, bool restrictive)
		{
			bool suspendErrors = context.ErrorContext.SuspendErrors;
			context.ErrorContext.SuspendErrors |= this.m_isAutoSubtotal;
			bool flag = false;
			this.DataGroupStart(context.ExprHostBuilder);
			bool flag2 = false;
			if (this.m_grouping != null)
			{
				context.SetIndexInCollection(this);
				if (this.m_grouping.Variables != null)
				{
					context.RegisterGroupWithVariables(this);
					context.RegisterVariables(this.m_grouping.Variables);
				}
				this.m_variablesInScope = context.GetCurrentReferencableVariables();
				flag = true;
				if ((context.Location & AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail) != 0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidDetailDataGrouping, Severity.Error, context.ObjectType, context.ObjectName, "Grouping");
				}
				else
				{
					context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InGrouping;
					if (this.m_grouping.IsDetail)
					{
						context.Location |= AspNetCore.ReportingServices.ReportPublishing.LocationFlags.InDetail;
					}
					context.RegisterGroupingScope(this);
					flag2 = true;
					this.m_dataScopeInfo.ValidateScopeRulesForIdc(context, this);
					if (this.m_grouping.DomainScope != null && !context.IsAncestor(this, this.m_grouping.DomainScope))
					{
						if (this.m_grouping.IsClone)
						{
							if (Global.Tracer.TraceVerbose)
							{
								Global.Tracer.Trace(TraceLevel.Verbose, "The grouping '{3}' in the {0} '{1}' has invalid {2} '{4}'. Domain Scope is allowed only if it is an ancestor scope.", this.m_dataRegionDef.ObjectType, this.m_dataRegionDef.Name, "DomainScope", this.m_grouping.Name.MarkAsModelInfo(), this.m_grouping.DomainScope.MarkAsPrivate());
							}
							this.m_grouping.DomainScope = null;
							this.m_grouping.ScopeIDForDomainScope = -1;
						}
						else
						{
							context.ErrorContext.Register(ProcessingErrorCode.rsInvalidGroupingDomainScopeNotAncestor, Severity.Error, this.m_dataRegionDef.ObjectType, this.m_dataRegionDef.Name, "DomainScope", this.m_grouping.Name.MarkAsModelInfo(), this.m_grouping.DomainScope.MarkAsPrivate());
						}
					}
					AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = context.ObjectType;
					string objectName = context.ObjectName;
					context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping;
					context.ObjectName = this.m_grouping.Name;
					context.ValidateScopeRulesForNaturalGroup(this);
					context.ValidateScopeRulesForNaturalSort(this);
					if (this.HasNaturalGroupAndNaturalSort && !ListUtils.IsSubset(this.m_sorting.SortExpressions, this.m_grouping.GroupExpressions, RdlExpressionComparer.Instance))
					{
						context.ErrorContext.Register(ProcessingErrorCode.rsIncompatibleNaturalSortAndNaturalGroup, Severity.Error, this.m_dataRegionDef.ObjectType, this.m_dataRegionDef.Name, "Group", this.m_grouping.Name.MarkAsModelInfo());
					}
				}
				this.m_grouping.Initialize(context);
				if (this.m_sorting != null)
				{
					this.m_sorting.Initialize(context);
				}
				this.InitializeMemberGroupAndSortExpressionFlags();
			}
			if (this.m_dataScopeInfo != null)
			{
				this.m_dataScopeInfo.Initialize(context, this);
			}
			if (this.m_customProperties != null)
			{
				this.m_customProperties.Initialize(null, context);
			}
			this.m_memberCellIndex = context.MemberCellIndex;
			flag |= this.InnerInitialize(context, restrictive);
			if (this.m_grouping != null)
			{
				if (flag2)
				{
					context.UnRegisterGroupingScope(this);
				}
				if (this.m_grouping.Variables != null)
				{
					context.UnregisterVariables(this.m_grouping.Variables);
				}
			}
			this.m_exprHostID = this.DataGroupEnd(context.ExprHostBuilder);
			context.ErrorContext.SuspendErrors = suspendErrors;
			return flag;
		}

		private void InitializeMemberGroupAndSortExpressionFlags()
		{
			int capacity = (this.m_sorting == null) ? this.m_grouping.GroupExpressions.Count : this.m_sorting.SortExpressions.Count;
			this.m_memberGroupAndSortExpressionFlag = new List<ScopeIDType>(capacity);
			if (this.m_sorting != null)
			{
				for (int i = 0; i < this.m_sorting.SortExpressions.Count; i++)
				{
					if (ListUtils.Contains(this.m_grouping.GroupExpressions, this.m_sorting.SortExpressions[i], RdlExpressionComparer.Instance))
					{
						this.m_memberGroupAndSortExpressionFlag.Add(ScopeIDType.SortGroup);
					}
					else
					{
						this.m_memberGroupAndSortExpressionFlag.Add(ScopeIDType.SortValues);
					}
				}
			}
			for (int j = 0; j < this.m_grouping.GroupExpressions.Count; j++)
			{
				if (this.m_sorting == null || !ListUtils.Contains(this.m_sorting.SortExpressions, this.m_grouping.GroupExpressions[j], RdlExpressionComparer.Instance))
				{
					this.m_memberGroupAndSortExpressionFlag.Add(ScopeIDType.GroupValues);
				}
			}
		}

		internal virtual bool PreInitializeDataMember(InitializationContext context)
		{
			return false;
		}

		internal virtual void PostInitializeDataMember(InitializationContext context, bool registeredVisibility)
		{
			if (this.m_grouping != null)
			{
				if (this.m_grouping.IsAtomic(context) || context.EvaluateAtomicityCondition(this.m_dataScopeInfo.HasAggregatesOrRunningValues, this, AtomicityReason.Aggregates) || context.EvaluateAtomicityCondition(this.HasFilters, this, AtomicityReason.Filters) || context.EvaluateAtomicityCondition(this.m_sorting != null && !this.m_sorting.NaturalSort, this, AtomicityReason.NonNaturalSorts))
				{
					context.FoundAtomicScope(this);
				}
				else if (context.EvaluateAtomicityCondition(context.HasMultiplePeerChildScopes(this), this, AtomicityReason.PeerChildScopes))
				{
					this.m_dataScopeInfo.IsDecomposable = true;
					context.FoundAtomicScope(this);
				}
				else
				{
					this.m_dataScopeInfo.IsDecomposable = true;
				}
			}
		}

		internal void CaptureReferencableTextboxes(InitializationContext context)
		{
			this.m_textboxesInScope = context.GetCurrentReferencableTextboxes();
		}

		protected abstract void DataGroupStart(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder);

		protected abstract int DataGroupEnd(AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder builder);

		List<RunningValueInfo> IRunningValueHolder.GetRunningValueList()
		{
			return this.m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_runningValues, "(null != m_runningValues)");
			if (this.m_runningValues.Count == 0)
			{
				this.m_runningValues.Clear();
			}
		}

		internal virtual void InitializeRVDirectionDependentItems(InitializationContext context)
		{
		}

		internal virtual void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			return this.PublishClone(context, null, false);
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			return this.PublishClone(context, newContainingRegion, false);
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion, bool isSubtotal)
		{
			ReportHierarchyNode reportHierarchyNode = (ReportHierarchyNode)base.PublishClone(context);
			reportHierarchyNode.m_dataScopeInfo = this.m_dataScopeInfo.PublishClone(context, reportHierarchyNode.ID);
			context.AddRunningValueHolder(reportHierarchyNode);
			if (isSubtotal)
			{
				reportHierarchyNode.m_grouping = null;
				reportHierarchyNode.m_sorting = null;
			}
			else
			{
				if (this.m_grouping != null)
				{
					reportHierarchyNode.m_grouping = (Grouping)this.m_grouping.PublishClone(context, reportHierarchyNode);
				}
				if (this.m_sorting != null)
				{
					reportHierarchyNode.m_sorting = (Sorting)this.m_sorting.PublishClone(context);
				}
			}
			if (this.m_customProperties != null)
			{
				reportHierarchyNode.m_customProperties = new DataValueList(this.m_customProperties.Count);
				foreach (DataValue customProperty in this.m_customProperties)
				{
					reportHierarchyNode.m_customProperties.Add(customProperty.PublishClone(context));
				}
			}
			if (newContainingRegion != null)
			{
				reportHierarchyNode.m_dataRegionDef = newContainingRegion;
			}
			return reportHierarchyNode;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.IsColumn, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Level, Token.Int32));
			list.Add(new MemberInfo(MemberName.Grouping, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.Sorting, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sorting));
			list.Add(new MemberInfo(MemberName.MemberCellIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
			list.Add(new MemberInfo(MemberName.AutoSubtotal, Token.Boolean));
			list.Add(new MemberInfo(MemberName.RunningValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RunningValueInfo));
			list.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.DataRegionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegion, Token.Reference));
			list.Add(new MemberInfo(MemberName.IndexInCollection, Token.Int32));
			list.Add(new MemberInfo(MemberName.NeedToCacheDataRows, Token.Boolean));
			list.Add(new MemberInfo(MemberName.InScopeEventSources, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IInScopeEventSource));
			list.Add(new MemberInfo(MemberName.HierarchyDynamicIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.HierarchyPathIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.HierarchyParentGroups, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.TextboxesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.VariablesInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveTypedArray, Token.Byte));
			list.Add(new MemberInfo(MemberName.DataScopeInfo, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataScopeInfo));
			list.Add(new MemberInfo(MemberName.OriginalScopeID, Token.Int32));
			list.Add(new MemberInfo(MemberName.InnerDomainScopeCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.MemberGroupAndSortExpressionFlag, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ReportHierarchyNode.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsColumn:
					writer.Write(this.m_isColumn);
					break;
				case MemberName.OriginalScopeID:
					writer.Write(this.m_originalScopeID);
					break;
				case MemberName.Level:
					writer.Write(this.m_level);
					break;
				case MemberName.Grouping:
					writer.Write(this.m_grouping);
					break;
				case MemberName.Sorting:
					writer.Write(this.m_sorting);
					break;
				case MemberName.MemberCellIndex:
					writer.Write(this.m_memberCellIndex);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.RowSpan:
					writer.Write(this.m_rowSpan);
					break;
				case MemberName.ColSpan:
					writer.Write(this.m_colSpan);
					break;
				case MemberName.AutoSubtotal:
					writer.Write(this.m_isAutoSubtotal);
					break;
				case MemberName.RunningValues:
					writer.Write(this.m_runningValues);
					break;
				case MemberName.CustomProperties:
					writer.Write(this.m_customProperties);
					break;
				case MemberName.DataRegionDef:
					writer.WriteReference(this.m_dataRegionDef);
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
				case MemberName.HierarchyDynamicIndex:
					writer.Write(this.m_hierarchyDynamicIndex);
					break;
				case MemberName.HierarchyPathIndex:
					writer.Write(this.m_hierarchyPathIndex);
					break;
				case MemberName.HierarchyParentGroups:
					writer.WriteListOfReferences(this.m_hierarchyParentGroups);
					break;
				case MemberName.TextboxesInScope:
					writer.Write(this.m_textboxesInScope);
					break;
				case MemberName.VariablesInScope:
					writer.Write(this.m_variablesInScope);
					break;
				case MemberName.DataScopeInfo:
					writer.Write(this.m_dataScopeInfo);
					break;
				case MemberName.InnerDomainScopeCount:
					writer.Write(this.InnerDomainScopeCount);
					break;
				case MemberName.MemberGroupAndSortExpressionFlag:
					writer.WriteListOfPrimitives(this.m_memberGroupAndSortExpressionFlag);
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
			reader.RegisterDeclaration(ReportHierarchyNode.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsColumn:
					this.m_isColumn = reader.ReadBoolean();
					break;
				case MemberName.OriginalScopeID:
					this.m_originalScopeID = reader.ReadInt32();
					break;
				case MemberName.Level:
					this.m_level = reader.ReadInt32();
					break;
				case MemberName.Grouping:
					this.Grouping = (Grouping)reader.ReadRIFObject();
					break;
				case MemberName.Sorting:
					this.m_sorting = (Sorting)reader.ReadRIFObject();
					break;
				case MemberName.MemberCellIndex:
					this.m_memberCellIndex = reader.ReadInt32();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.RowSpan:
					this.m_rowSpan = reader.ReadInt32();
					break;
				case MemberName.ColSpan:
					this.m_colSpan = reader.ReadInt32();
					break;
				case MemberName.AutoSubtotal:
					this.m_isAutoSubtotal = reader.ReadBoolean();
					break;
				case MemberName.RunningValues:
					this.m_runningValues = reader.ReadGenericListOfRIFObjects<RunningValueInfo>();
					break;
				case MemberName.CustomProperties:
					this.m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.DataRegionDef:
					this.m_dataRegionDef = reader.ReadReference<DataRegion>(this);
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
				case MemberName.HierarchyDynamicIndex:
					this.m_hierarchyDynamicIndex = reader.ReadInt32();
					break;
				case MemberName.HierarchyPathIndex:
					this.m_hierarchyPathIndex = reader.ReadInt32();
					break;
				case MemberName.HierarchyParentGroups:
					this.m_hierarchyParentGroups = reader.ReadListOfReferences<GroupingList, Grouping>(this);
					break;
				case MemberName.TextboxesInScope:
					this.m_textboxesInScope = reader.ReadByteArray();
					break;
				case MemberName.VariablesInScope:
					this.m_variablesInScope = reader.ReadByteArray();
					break;
				case MemberName.DataScopeInfo:
					this.m_dataScopeInfo = (DataScopeInfo)reader.ReadRIFObject();
					break;
				case MemberName.InnerDomainScopeCount:
					this.m_innerDomainScopeCount = reader.ReadInt32();
					break;
				case MemberName.MemberGroupAndSortExpressionFlag:
					this.m_memberGroupAndSortExpressionFlag = reader.ReadListOfPrimitives<ScopeIDType>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ReportHierarchyNode.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item2 in list)
				{
					switch (item2.MemberName)
					{
					case MemberName.DataRegionDef:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item2.RefID));
						Global.Tracer.Assert(((ReportItem)referenceableItems[item2.RefID]).IsDataRegion);
						Global.Tracer.Assert(this.m_dataRegionDef != (DataRegion)referenceableItems[item2.RefID]);
						this.m_dataRegionDef = (DataRegion)referenceableItems[item2.RefID];
						break;
					case MemberName.InScopeEventSources:
					{
						IReferenceable referenceable = default(IReferenceable);
						referenceableItems.TryGetValue(item2.RefID, out referenceable);
						IInScopeEventSource item = (IInScopeEventSource)referenceable;
						if (this.m_inScopeEventSources == null)
						{
							this.m_inScopeEventSources = new List<IInScopeEventSource>();
						}
						this.m_inScopeEventSources.Add(item);
						break;
					}
					case MemberName.HierarchyParentGroups:
						if (this.m_hierarchyParentGroups == null)
						{
							this.m_hierarchyParentGroups = new GroupingList();
						}
						if (item2.RefID != -2)
						{
							Global.Tracer.Assert(referenceableItems.ContainsKey(item2.RefID));
							Global.Tracer.Assert(referenceableItems[item2.RefID] is Grouping);
							Global.Tracer.Assert(!this.m_hierarchyParentGroups.Contains((Grouping)referenceableItems[item2.RefID]));
							this.m_hierarchyParentGroups.Add((Grouping)referenceableItems[item2.RefID]);
						}
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode;
		}

		internal abstract void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel);

		protected void MemberNodeSetExprHost(IMemberNode exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(null != exprHost, "(null != exprHost)");
			this.m_exprHost = exprHost;
			if (this.m_exprHost.GroupHost != null)
			{
				Global.Tracer.Assert(null != this.m_grouping, "(null != m_grouping)");
				this.m_grouping.SetExprHost(this.m_exprHost.GroupHost, reportObjectModel);
			}
			if (this.m_exprHost.SortHost != null)
			{
				Global.Tracer.Assert(null != this.m_sorting, "(null != m_sorting)");
				this.m_sorting.SetExprHost(this.m_exprHost.SortHost, reportObjectModel);
			}
			if (this.m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(null != this.m_customProperties, "(null != m_customProperties)");
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			if (this.m_dataScopeInfo != null && this.m_dataScopeInfo.JoinInfo != null && this.m_exprHost.JoinConditionExprHostsRemotable != null)
			{
				this.m_dataScopeInfo.JoinInfo.SetJoinConditionExprHost(this.m_exprHost.JoinConditionExprHostsRemotable, reportObjectModel);
			}
		}

		internal abstract void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions);

		private void CalculateDependencies()
		{
			this.m_cellStartIndex = (this.m_cellEndIndex = this.m_memberCellIndex);
			if (this.InnerHierarchy != null)
			{
				ReportHierarchyNode.GetCellIndexes(this.InnerStaticMembersInSameScope, ref this.m_cellStartIndex, ref this.m_cellEndIndex);
			}
		}

		internal List<int> GetCellIndexes()
		{
			if (this.m_cellIndexes == null)
			{
				if (this.InnerStaticMembersInSameScope != null && this.InnerStaticMembersInSameScope.Count != 0 && this.InnerStaticMembersInSameScope.LeafCellIndexes != null)
				{
					this.m_cellIndexes = this.InnerStaticMembersInSameScope.LeafCellIndexes;
				}
				else
				{
					List<int> list = new List<int>(1);
					list.Add(this.m_memberCellIndex);
					this.m_cellIndexes = list;
				}
			}
			return this.m_cellIndexes;
		}

		private static void GetCellIndexes(HierarchyNodeList innerStaticMemberList, ref int cellStartIndex, ref int cellEndIndex)
		{
			if (innerStaticMemberList != null)
			{
				foreach (ReportHierarchyNode innerStaticMember in innerStaticMemberList)
				{
					if (innerStaticMember.InnerHierarchy == null)
					{
						cellStartIndex = Math.Min(cellStartIndex, innerStaticMember.MemberCellIndex);
						cellEndIndex = Math.Max(cellEndIndex, innerStaticMember.MemberCellIndex);
					}
				}
			}
		}

		internal void ResetInstancePathCascade()
		{
			if (this.m_grouping != null)
			{
				base.InstancePathItem.ResetContext();
				HierarchyNodeList innerDynamicMembers = this.InnerDynamicMembers;
				if (innerDynamicMembers != null)
				{
					for (int i = 0; i < innerDynamicMembers.Count; i++)
					{
						innerDynamicMembers[i].InstancePathItem.ResetContext();
					}
				}
			}
		}

		internal virtual void MoveNextForUserSort(OnDemandProcessingContext odpContext)
		{
			if (this.m_grouping != null)
			{
				base.InstancePathItem.MoveNext();
			}
		}

		internal void SetUserSortDetailRowIndex(OnDemandProcessingContext odpContext)
		{
			if (this.m_grouping != null && this.m_grouping.IsDetail)
			{
				int rowIndex = odpContext.ReportObjectModel.FieldsImpl.GetRowIndex();
				if (this.m_isColumn)
				{
					this.m_dataRegionDef.CurrentColDetailIndex = rowIndex;
				}
				else
				{
					this.m_dataRegionDef.CurrentRowDetailIndex = rowIndex;
				}
			}
		}

		internal virtual void SetMemberInstances(IList<DataRegionMemberInstance> memberInstances)
		{
		}

		internal virtual void SetRecursiveParentIndex(int parentInstanceIndex)
		{
		}

		internal virtual void SetInstanceHasRecursiveChildren(bool? hasRecursiveChildren)
		{
		}

		protected override InstancePathItem CreateInstancePathItem()
		{
			if (this.IsStatic)
			{
				return new InstancePathItem();
			}
			if (this.IsColumn)
			{
				return new InstancePathItem(InstancePathItemType.ColumnMemberInstanceIndex, this.IndexInCollection);
			}
			return new InstancePathItem(InstancePathItemType.RowMemberInstanceIndex, this.IndexInCollection);
		}

		void IStaticReferenceable.SetID(int id)
		{
			this.m_staticRefId = id;
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return this.GetObjectType();
		}
	}
}
