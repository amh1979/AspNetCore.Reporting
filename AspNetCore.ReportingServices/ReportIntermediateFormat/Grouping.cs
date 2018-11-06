using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Grouping : IAggregateHolder, ISortFilterScope, IReferenceable, IPageBreakOwner, IPersistable
	{
		private string m_name;

		private int m_ID = -1;

		private List<ExpressionInfo> m_groupExpressions;

		private ExpressionInfo m_groupLabel;

		private List<bool> m_sortDirections;

		private PageBreak m_pageBreak;

		private ExpressionInfo m_pageName;

		private List<DataAggregateInfo> m_aggregates;

		private bool m_groupAndSort;

		private List<Filter> m_filters;

		[Reference]
		private List<ReportItem> m_reportItemsWithHideDuplicates;

		private List<ExpressionInfo> m_parent;

		private List<DataAggregateInfo> m_recursiveAggregates;

		private List<DataAggregateInfo> m_postSortAggregates;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		private bool m_saveGroupExprValues;

		private List<ExpressionInfo> m_userSortExpressions;

		private InScopeSortFilterHashtable m_nonDetailSortFiltersInScope;

		private InScopeSortFilterHashtable m_detailSortFiltersInScope;

		private List<Variable> m_variables;

		private string m_domainScope;

		private int m_scopeIDForDomainScope = -1;

		private bool m_naturalGroup;

		[NonSerialized]
		private List<int> m_hideDuplicatesReportItemIDs;

		[NonSerialized]
		private GroupExprHost m_exprHost;

		[NonSerialized]
		private Hashtable m_scopeNames;

		[NonSerialized]
		private int m_recursiveLevel;

		[NonSerialized]
		private int[] m_groupExpressionFieldIndices;

		[NonSerialized]
		private bool m_isClone;

		[NonSerialized]
		private List<object> m_currentGroupExprValues;

		[NonSerialized]
		private object[] m_groupInstanceExprValues;

		[NonSerialized]
		private ReportHierarchyNode m_owner;

		[NonSerialized]
		private List<object>[] m_sortFilterScopeInfo;

		[NonSerialized]
		private int[] m_sortFilterScopeIndex;

		[NonSerialized]
		private bool[] m_needScopeInfoForSortFilterExpression;

		[NonSerialized]
		private bool[] m_sortFilterScopeMatched;

		[NonSerialized]
		private bool[] m_isSortFilterTarget;

		[NonSerialized]
		private bool[] m_isSortFilterExpressionScope;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Grouping.GetDeclaration();

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal ExpressionInfo GroupLabel
		{
			get
			{
				return this.m_groupLabel;
			}
			set
			{
				this.m_groupLabel = value;
			}
		}

		internal List<bool> SortDirections
		{
			get
			{
				return this.m_sortDirections;
			}
			set
			{
				this.m_sortDirections = value;
			}
		}

		internal List<ExpressionInfo> GroupExpressions
		{
			get
			{
				return this.m_groupExpressions;
			}
			set
			{
				this.m_groupExpressions = value;
			}
		}

		internal string DomainScope
		{
			get
			{
				return this.m_domainScope;
			}
			set
			{
				this.m_domainScope = value;
			}
		}

		internal int ScopeIDForDomainScope
		{
			get
			{
				if (this.m_scopeIDForDomainScope == -1)
				{
					return this.Owner.DataScopeInfo.ScopeID;
				}
				return this.m_scopeIDForDomainScope;
			}
			set
			{
				this.m_scopeIDForDomainScope = value;
			}
		}

		internal bool IsDetail
		{
			get
			{
				if (this.m_groupExpressions != null)
				{
					return this.m_groupExpressions.Count == 0;
				}
				return true;
			}
		}

		internal bool IsClone
		{
			get
			{
				return this.m_isClone;
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
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping;
			}
		}

		string IPageBreakOwner.ObjectName
		{
			get
			{
				return this.m_name;
			}
		}

		IInstancePath IPageBreakOwner.InstancePath
		{
			get
			{
				return this.m_owner;
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

		internal bool GroupAndSort
		{
			get
			{
				return this.m_groupAndSort;
			}
			set
			{
				this.m_groupAndSort = value;
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

		internal bool SimpleGroupExpressions
		{
			get
			{
				if (this.m_groupExpressions != null)
				{
					for (int i = 0; i < this.m_groupExpressions.Count; i++)
					{
						ExpressionInfo expressionInfo = this.m_groupExpressions[i];
						Global.Tracer.Assert(null != expressionInfo, "(null != expression)");
						if (expressionInfo.Type != ExpressionInfo.Types.Field && expressionInfo.Type != ExpressionInfo.Types.Constant)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		internal List<ReportItem> ReportItemsWithHideDuplicates
		{
			get
			{
				return this.m_reportItemsWithHideDuplicates;
			}
			set
			{
				this.m_reportItemsWithHideDuplicates = value;
			}
		}

		internal List<ExpressionInfo> Parent
		{
			get
			{
				return this.m_parent;
			}
			set
			{
				this.m_parent = value;
			}
		}

		internal IndexedExprHost ParentExprHost
		{
			get
			{
				if (this.m_exprHost == null)
				{
					return null;
				}
				return this.m_exprHost.ParentExpressionsHost;
			}
		}

		internal IndexedExprHost VariableValueHosts
		{
			get
			{
				if (this.m_exprHost == null)
				{
					return null;
				}
				return this.m_exprHost.VariableValueHosts;
			}
		}

		internal List<DataAggregateInfo> RecursiveAggregates
		{
			get
			{
				return this.m_recursiveAggregates;
			}
			set
			{
				this.m_recursiveAggregates = value;
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

		internal string DataElementName
		{
			get
			{
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
			}
		}

		internal bool SaveGroupExprValues
		{
			get
			{
				return this.m_saveGroupExprValues;
			}
			set
			{
				this.m_saveGroupExprValues = value;
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

		internal InScopeSortFilterHashtable NonDetailSortFiltersInScope
		{
			get
			{
				return this.m_nonDetailSortFiltersInScope;
			}
			set
			{
				this.m_nonDetailSortFiltersInScope = value;
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

		internal List<int> HideDuplicatesReportItemIDs
		{
			get
			{
				return this.m_hideDuplicatesReportItemIDs;
			}
			set
			{
				this.m_hideDuplicatesReportItemIDs = value;
			}
		}

		internal GroupExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
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

		internal int RecursiveLevel
		{
			get
			{
				return this.m_recursiveLevel;
			}
			set
			{
				this.m_recursiveLevel = value;
			}
		}

		internal List<object> CurrentGroupExpressionValues
		{
			get
			{
				return this.m_currentGroupExprValues;
			}
			set
			{
				this.m_currentGroupExprValues = value;
			}
		}

		internal ReportHierarchyNode Owner
		{
			get
			{
				return this.m_owner;
			}
			set
			{
				this.m_owner = value;
			}
		}

		internal List<object>[] SortFilterScopeInfo
		{
			get
			{
				return this.m_sortFilterScopeInfo;
			}
			set
			{
				this.m_sortFilterScopeInfo = value;
			}
		}

		internal int[] SortFilterScopeIndex
		{
			get
			{
				return this.m_sortFilterScopeIndex;
			}
			set
			{
				this.m_sortFilterScopeIndex = value;
			}
		}

		internal bool[] NeedScopeInfoForSortFilterExpression
		{
			get
			{
				return this.m_needScopeInfoForSortFilterExpression;
			}
			set
			{
				this.m_needScopeInfoForSortFilterExpression = value;
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

		internal bool[] SortFilterScopeMatched
		{
			get
			{
				return this.m_sortFilterScopeMatched;
			}
			set
			{
				this.m_sortFilterScopeMatched = value;
			}
		}

		int IReferenceable.ID
		{
			get
			{
				return this.m_ID;
			}
		}

		int ISortFilterScope.ID
		{
			get
			{
				Global.Tracer.Assert(null != this.m_owner);
				return this.m_owner.ID;
			}
		}

		string ISortFilterScope.ScopeName
		{
			get
			{
				return this.m_name;
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
				if (this.m_exprHost == null)
				{
					return null;
				}
				return this.m_exprHost.UserSortExpressionsHost;
			}
		}

		internal List<Variable> Variables
		{
			get
			{
				return this.m_variables;
			}
			set
			{
				this.m_variables = value;
			}
		}

		internal bool NaturalGroup
		{
			get
			{
				return this.m_naturalGroup;
			}
			set
			{
				this.m_naturalGroup = value;
			}
		}

		DataScopeInfo IAggregateHolder.DataScopeInfo
		{
			get
			{
				return this.m_owner.DataScopeInfo;
			}
		}

		internal Grouping(ConstructionPhase phase)
			: this(-1, phase)
		{
		}

		internal Grouping(int id, ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				this.m_groupExpressions = new List<ExpressionInfo>();
				this.m_aggregates = new List<DataAggregateInfo>();
				this.m_postSortAggregates = new List<DataAggregateInfo>();
				this.m_recursiveAggregates = new List<DataAggregateInfo>();
			}
			this.m_ID = id;
		}

		internal bool IsAtomic(InitializationContext context)
		{
			if (!context.EvaluateAtomicityCondition(!this.m_naturalGroup && !this.IsDetail, this.m_owner, AtomicityReason.NonNaturalGroup) && !context.EvaluateAtomicityCondition(this.m_domainScope != null, this.m_owner, AtomicityReason.DomainScope) && !context.EvaluateAtomicityCondition(this.m_parent != null, this.m_owner, AtomicityReason.RecursiveParent))
			{
				return context.EvaluateAtomicityCondition(this.HasAggregatesForAtomicityCheck(), this.m_owner, AtomicityReason.Aggregates);
			}
			return true;
		}

		private bool HasAggregatesForAtomicityCheck()
		{
			if (!DataScopeInfo.HasNonServerAggregates(this.m_aggregates) && !DataScopeInfo.HasAggregates(this.m_postSortAggregates))
			{
				return DataScopeInfo.HasAggregates(this.m_recursiveAggregates);
			}
			return true;
		}

		public void ResetAggregates(AggregatesImpl reportOmAggregates)
		{
			reportOmAggregates.ResetAll(this.m_aggregates);
			reportOmAggregates.ResetAll(this.m_postSortAggregates);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.GroupStart(this.m_name);
			this.m_saveGroupExprValues = context.HasPreviousAggregates;
			this.DataRendererInitialize(context);
			if (this.m_groupExpressions != null)
			{
				for (int i = 0; i < this.m_groupExpressions.Count; i++)
				{
					ExpressionInfo expressionInfo = this.m_groupExpressions[i];
					expressionInfo.GroupExpressionInitialize(context);
					context.ExprHostBuilder.GroupExpression(expressionInfo);
				}
			}
			if (this.m_groupLabel != null)
			{
				this.m_groupLabel.Initialize("Label", context);
				context.ExprHostBuilder.GenericLabel(this.m_groupLabel);
			}
			if (this.m_filters != null)
			{
				for (int j = 0; j < this.m_filters.Count; j++)
				{
					this.m_filters[j].Initialize(context);
				}
			}
			if (this.m_parent != null)
			{
				context.ExprHostBuilder.GroupParentExpressionsStart();
				for (int k = 0; k < this.m_parent.Count; k++)
				{
					ExpressionInfo expressionInfo2 = this.m_parent[k];
					expressionInfo2.GroupExpressionInitialize(context);
					context.ExprHostBuilder.GroupParentExpression(expressionInfo2);
				}
				context.ExprHostBuilder.GroupParentExpressionsEnd();
			}
			if (this.m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				for (int l = 0; l < this.m_userSortExpressions.Count; l++)
				{
					ExpressionInfo expression = this.m_userSortExpressions[l];
					context.ExprHostBuilder.UserSortExpression(expression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			if (this.m_variables != null && this.m_variables.Count != 0)
			{
				context.ExprHostBuilder.VariableValuesStart();
				for (int m = 0; m < this.m_variables.Count; m++)
				{
					Variable variable = this.m_variables[m];
					variable.Initialize(context);
					context.ExprHostBuilder.VariableValueExpression(variable.Value);
				}
				context.ExprHostBuilder.VariableValuesEnd();
			}
			if (this.m_pageBreak != null)
			{
				this.m_pageBreak.Initialize(context);
			}
			if (this.m_pageName != null)
			{
				this.m_pageName.Initialize("PageName", context);
				context.ExprHostBuilder.PageName(this.m_pageName);
			}
			context.ExprHostBuilder.GroupEnd();
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
			Global.Tracer.Assert(null != this.m_recursiveAggregates, "(null != m_recursiveAggregates)");
			if (this.m_recursiveAggregates.Count == 0)
			{
				this.m_recursiveAggregates = null;
			}
		}

		private void DataRendererInitialize(InitializationContext context)
		{
			if (this.m_dataElementOutput == DataElementOutputTypes.Auto || this.m_dataElementOutput == DataElementOutputTypes.ContentsOnly)
			{
				this.m_dataElementOutput = DataElementOutputTypes.Output;
			}
			AspNetCore.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, this.m_name, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal void AddReportItemWithHideDuplicates(ReportItem reportItem)
		{
			if (this.m_reportItemsWithHideDuplicates == null)
			{
				this.m_reportItemsWithHideDuplicates = new List<ReportItem>();
			}
			this.m_reportItemsWithHideDuplicates.Add(reportItem);
		}

		internal void ResetReportItemsWithHideDuplicates()
		{
			if (this.m_reportItemsWithHideDuplicates != null)
			{
				int count = this.m_reportItemsWithHideDuplicates.Count;
				for (int i = 0; i < count; i++)
				{
					TextBox textBox = this.m_reportItemsWithHideDuplicates[i] as TextBox;
					textBox.ResetDuplicates();
				}
			}
		}

		internal bool IsOnPathToSortFilterSource(int index)
		{
			if (this.m_sortFilterScopeInfo != null && this.m_sortFilterScopeIndex != null && -1 != this.m_sortFilterScopeIndex[index])
			{
				return true;
			}
			return false;
		}

		internal int[] GetGroupExpressionFieldIndices()
		{
			if (this.m_groupExpressionFieldIndices == null && this.m_groupExpressions != null && 0 < this.m_groupExpressions.Count)
			{
				Global.Tracer.Assert(this.m_groupExpressions != null && 0 < this.m_groupExpressions.Count, "(null != m_groupExpressions && 0 < m_groupExpressions.Count)");
				this.m_groupExpressionFieldIndices = new int[this.m_groupExpressions.Count];
				for (int i = 0; i < this.m_groupExpressions.Count; i++)
				{
					this.m_groupExpressionFieldIndices[i] = -2;
					ExpressionInfo expressionInfo = this.m_groupExpressions[i];
					if (expressionInfo.Type == ExpressionInfo.Types.Field)
					{
						this.m_groupExpressionFieldIndices[i] = expressionInfo.IntValue;
					}
					else if (expressionInfo.Type == ExpressionInfo.Types.Constant)
					{
						this.m_groupExpressionFieldIndices[i] = -1;
					}
				}
			}
			return this.m_groupExpressionFieldIndices;
		}

		internal Grouping CloneForDomainScope(AutomaticSubtotalContext context, ReportHierarchyNode cloneOwner)
		{
			Grouping grouping = new Grouping(ConstructionPhase.Publishing);
			grouping.m_isClone = true;
			grouping.m_ID = context.GenerateID();
			grouping.m_owner = cloneOwner;
			cloneOwner.OriginalScopeID = this.Owner.DataScopeInfo.ScopeID;
			Global.Tracer.Assert(this.m_name != null, "Group Name cannot be null");
			grouping.m_name = context.CreateAndRegisterUniqueGroupName(this.m_name, this.m_isClone, true);
			this.CloneGroupExpressions(context, grouping);
			return grouping;
		}

		internal object PublishClone(AutomaticSubtotalContext context, ReportHierarchyNode owner)
		{
			Grouping grouping = (Grouping)base.MemberwiseClone();
			grouping.m_isClone = true;
			grouping.m_ID = context.GenerateID();
			grouping.m_owner = owner;
			if (this.DomainScope != null)
			{
				grouping.DomainScope = context.GetNewScopeName(this.DomainScope);
				if (string.CompareOrdinal(this.DomainScope, grouping.DomainScope) != 0)
				{
					context.DomainScopeGroups.Add(grouping);
				}
				else
				{
					grouping.m_scopeIDForDomainScope = this.Owner.DataScopeInfo.ScopeID;
				}
			}
			context.AddAggregateHolder(grouping);
			Global.Tracer.Assert(this.m_name != null);
			grouping.m_name = context.CreateAndRegisterUniqueGroupName(this.m_name, this.m_isClone);
			context.AddSortTarget(grouping.m_name, grouping);
			this.CloneGroupExpressions(context, grouping);
			if (this.m_groupLabel != null)
			{
				grouping.m_groupLabel = (ExpressionInfo)this.m_groupLabel.PublishClone(context);
			}
			if (this.m_sortDirections != null)
			{
				grouping.m_sortDirections = new List<bool>(this.m_sortDirections.Count);
				foreach (bool sortDirection in this.m_sortDirections)
				{
					grouping.m_sortDirections.Add(sortDirection);
				}
			}
			grouping.m_aggregates = new List<DataAggregateInfo>();
			grouping.m_recursiveAggregates = new List<DataAggregateInfo>();
			grouping.m_postSortAggregates = new List<DataAggregateInfo>();
			if (this.m_filters != null)
			{
				grouping.m_filters = new List<Filter>(this.m_filters.Count);
				foreach (Filter filter in this.m_filters)
				{
					grouping.m_filters.Add((Filter)filter.PublishClone(context));
				}
			}
			if (this.m_parent != null)
			{
				grouping.m_parent = new List<ExpressionInfo>(this.m_parent.Count);
				foreach (ExpressionInfo item in this.m_parent)
				{
					grouping.m_parent.Add((ExpressionInfo)item.PublishClone(context));
				}
			}
			if (this.m_dataElementName != null)
			{
				grouping.m_dataElementName = (string)this.m_dataElementName.Clone();
			}
			if (this.m_userSortExpressions != null)
			{
				grouping.m_userSortExpressions = new List<ExpressionInfo>(this.m_userSortExpressions.Count);
				foreach (ExpressionInfo userSortExpression in this.m_userSortExpressions)
				{
					grouping.m_userSortExpressions.Add((ExpressionInfo)userSortExpression.PublishClone(context));
				}
			}
			if (this.m_variables != null)
			{
				grouping.m_variables = new List<Variable>(this.m_variables.Count);
				foreach (Variable variable in this.m_variables)
				{
					grouping.m_variables.Add((Variable)variable.PublishClone(context));
				}
			}
			if (this.m_nonDetailSortFiltersInScope != null)
			{
				grouping.m_nonDetailSortFiltersInScope = new InScopeSortFilterHashtable(this.m_nonDetailSortFiltersInScope.Count);
				IDictionaryEnumerator enumerator6 = this.m_nonDetailSortFiltersInScope.GetEnumerator();
				try
				{
					while (enumerator6.MoveNext())
					{
						object current6 = enumerator6.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)current6;
						List<int> list = (List<int>)dictionaryEntry.Value;
						List<int> list2 = new List<int>(list.Count);
						foreach (int item2 in list)
						{
							list2.Add(item2);
						}
						grouping.m_nonDetailSortFiltersInScope.Add(dictionaryEntry.Key, list2);
					}
				}
				finally
				{
					IDisposable disposable = enumerator6 as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (this.m_detailSortFiltersInScope != null)
			{
				grouping.m_detailSortFiltersInScope = new InScopeSortFilterHashtable(this.m_detailSortFiltersInScope.Count);
				IDictionaryEnumerator enumerator8 = this.m_detailSortFiltersInScope.GetEnumerator();
				try
				{
					while (enumerator8.MoveNext())
					{
						object current8 = enumerator8.Current;
						DictionaryEntry dictionaryEntry2 = (DictionaryEntry)current8;
						List<int> list3 = (List<int>)dictionaryEntry2.Value;
						List<int> list4 = new List<int>(list3.Count);
						foreach (int item3 in list3)
						{
							list4.Add(item3);
						}
						grouping.m_detailSortFiltersInScope.Add(dictionaryEntry2.Key, list4);
					}
				}
				finally
				{
					IDisposable disposable2 = enumerator8 as IDisposable;
					if (disposable2 != null)
					{
						disposable2.Dispose();
					}
				}
			}
			if (this.m_pageBreak != null)
			{
				grouping.m_pageBreak = (PageBreak)this.m_pageBreak.PublishClone(context);
			}
			if (this.m_pageName != null)
			{
				grouping.m_pageName = (ExpressionInfo)this.m_pageName.PublishClone(context);
			}
			return grouping;
		}

		private void CloneGroupExpressions(AutomaticSubtotalContext context, Grouping clone)
		{
			if (this.m_groupExpressions != null)
			{
				clone.m_groupExpressions = new List<ExpressionInfo>(this.m_groupExpressions.Count);
				foreach (ExpressionInfo groupExpression in this.m_groupExpressions)
				{
					clone.m_groupExpressions.Add((ExpressionInfo)groupExpression.PublishClone(context));
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			list.Add(new MemberInfo(MemberName.GroupExpressions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GroupLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SortDirections, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Boolean));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.Aggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.GroupAndSort, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Filters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Filter));
			list.Add(new MemberInfo(MemberName.ReportItemsWithHideDuplicates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.Parent, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RecursiveAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.PostSortAggregates, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateInfo));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.SaveGroupExprValues, Token.Boolean));
			list.Add(new MemberInfo(MemberName.UserSortExpressions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.NonDetailSortFiltersInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Int32PrimitiveListHashtable));
			list.Add(new MemberInfo(MemberName.Variables, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Variable));
			list.Add(new MemberInfo(MemberName.PageBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak));
			list.Add(new MemberInfo(MemberName.PageName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DomainScope, Token.String));
			list.Add(new MemberInfo(MemberName.ScopeIDForDomainScope, Token.Int32));
			list.Add(new MemberInfo(MemberName.NaturalGroup, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Grouping.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.ID:
					writer.Write(this.m_ID);
					break;
				case MemberName.GroupExpressions:
					writer.Write(this.m_groupExpressions);
					break;
				case MemberName.GroupLabel:
					writer.Write(this.m_groupLabel);
					break;
				case MemberName.SortDirections:
					writer.WriteListOfPrimitives(this.m_sortDirections);
					break;
				case MemberName.Aggregates:
					writer.Write(this.m_aggregates);
					break;
				case MemberName.GroupAndSort:
					writer.Write(this.m_groupAndSort);
					break;
				case MemberName.Filters:
					writer.Write(this.m_filters);
					break;
				case MemberName.ReportItemsWithHideDuplicates:
					writer.WriteListOfReferences(this.m_reportItemsWithHideDuplicates);
					break;
				case MemberName.Parent:
					writer.Write(this.m_parent);
					break;
				case MemberName.RecursiveAggregates:
					writer.Write(this.m_recursiveAggregates);
					break;
				case MemberName.PostSortAggregates:
					writer.Write(this.m_postSortAggregates);
					break;
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
					break;
				case MemberName.SaveGroupExprValues:
					writer.Write(this.m_saveGroupExprValues);
					break;
				case MemberName.UserSortExpressions:
					writer.Write(this.m_userSortExpressions);
					break;
				case MemberName.NonDetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(this.m_nonDetailSortFiltersInScope);
					break;
				case MemberName.DetailSortFiltersInScope:
					writer.WriteInt32PrimitiveListHashtable<int>(this.m_detailSortFiltersInScope);
					break;
				case MemberName.Variables:
					writer.Write(this.m_variables);
					break;
				case MemberName.PageBreak:
					writer.Write(this.m_pageBreak);
					break;
				case MemberName.PageName:
					writer.Write(this.m_pageName);
					break;
				case MemberName.DomainScope:
					writer.Write(this.m_domainScope);
					break;
				case MemberName.ScopeIDForDomainScope:
					writer.Write(this.m_scopeIDForDomainScope);
					break;
				case MemberName.NaturalGroup:
					writer.Write(this.m_naturalGroup);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Grouping.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.ID:
					this.m_ID = reader.ReadInt32();
					break;
				case MemberName.GroupExpressions:
					this.m_groupExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.GroupLabel:
					this.m_groupLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SortDirections:
					this.m_sortDirections = reader.ReadListOfPrimitives<bool>();
					break;
				case MemberName.PageBreakLocation:
					this.m_pageBreak = new PageBreak();
					this.m_pageBreak.BreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.Aggregates:
					this.m_aggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.GroupAndSort:
					this.m_groupAndSort = reader.ReadBoolean();
					break;
				case MemberName.Filters:
					this.m_filters = reader.ReadGenericListOfRIFObjects<Filter>();
					break;
				case MemberName.ReportItemsWithHideDuplicates:
					this.m_reportItemsWithHideDuplicates = reader.ReadGenericListOfReferences<ReportItem>(this);
					break;
				case MemberName.Parent:
					this.m_parent = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.RecursiveAggregates:
					this.m_recursiveAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.PostSortAggregates:
					this.m_postSortAggregates = reader.ReadGenericListOfRIFObjects<DataAggregateInfo>();
					break;
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.SaveGroupExprValues:
					this.m_saveGroupExprValues = reader.ReadBoolean();
					break;
				case MemberName.UserSortExpressions:
					this.m_userSortExpressions = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				case MemberName.NonDetailSortFiltersInScope:
					this.m_nonDetailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.DetailSortFiltersInScope:
					this.m_detailSortFiltersInScope = reader.ReadInt32PrimitiveListHashtable<InScopeSortFilterHashtable, int>();
					break;
				case MemberName.Variables:
					this.m_variables = reader.ReadGenericListOfRIFObjects<Variable>();
					break;
				case MemberName.PageBreak:
					this.m_pageBreak = (PageBreak)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					this.m_pageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DomainScope:
					this.m_domainScope = reader.ReadString();
					break;
				case MemberName.ScopeIDForDomainScope:
					this.m_scopeIDForDomainScope = reader.ReadInt32();
					break;
				case MemberName.NaturalGroup:
					this.m_naturalGroup = reader.ReadBoolean();
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
			if (memberReferencesCollection.TryGetValue(Grouping.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.ReportItemsWithHideDuplicates)
					{
						if (this.m_reportItemsWithHideDuplicates == null)
						{
							this.m_reportItemsWithHideDuplicates = new List<ReportItem>();
						}
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						Global.Tracer.Assert(referenceableItems[item.RefID] is ReportItem);
						Global.Tracer.Assert(!this.m_reportItemsWithHideDuplicates.Contains((ReportItem)referenceableItems[item.RefID]));
						this.m_reportItemsWithHideDuplicates.Add((ReportItem)referenceableItems[item.RefID]);
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping;
		}

		internal void SetExprHost(GroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_exprHost.FilterHostsRemotable != null)
			{
				Global.Tracer.Assert(this.m_filters != null, "(m_filters != null)");
				int count = this.m_filters.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_filters[i].SetExprHost(this.m_exprHost.FilterHostsRemotable, reportObjectModel);
				}
			}
			if (this.m_exprHost.ParentExpressionsHost != null)
			{
				this.m_exprHost.ParentExpressionsHost.SetReportObjectModel(reportObjectModel);
			}
			if (this.m_exprHost.VariableValueHosts != null)
			{
				this.m_exprHost.VariableValueHosts.SetReportObjectModel(reportObjectModel);
			}
			if (this.m_exprHost.UserSortExpressionsHost != null)
			{
				this.m_exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
			}
			if (this.m_pageBreak != null && this.m_exprHost.PageBreakExprHost != null)
			{
				this.m_pageBreak.SetExprHost(this.m_exprHost.PageBreakExprHost, reportObjectModel);
			}
		}

		internal string EvaluateGroupingLabelExpression(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_owner, romInstance);
			return context.ReportRuntime.EvaluateGroupingLabelExpression(this, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix, this.m_name);
		}

		internal int GetRecursiveLevel(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_owner, romInstance);
			return this.m_recursiveLevel;
		}

		internal void SetGroupInstanceExpressionValues(object[] exprValues)
		{
			this.m_groupInstanceExprValues = exprValues;
		}

		internal object[] GetGroupInstanceExpressionValues(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_owner, romInstance);
			return this.m_groupInstanceExprValues;
		}

		internal string EvaluatePageName(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.m_owner, romInstance);
			return context.ReportRuntime.EvaluateGroupingPageNameExpression(this, this.m_pageName, this.m_name);
		}
	}
}
