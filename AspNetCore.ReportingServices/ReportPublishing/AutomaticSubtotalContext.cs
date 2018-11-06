using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal struct AutomaticSubtotalContext
	{
		private string m_objectName;

		private ObjectType m_objectType;

		private LocationFlags m_location;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion m_currentDataRegion;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion m_currentDataRegionClone;

		private Map m_currentMapClone;

		private MapVectorLayer m_currentMapVectorLayerClone;

		private string m_currentScope;

		private string m_currentScopeBeingCloned;

		private List<ICreateSubtotals> m_createSubtotals;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> m_domainScopeGroups;

		private Holder<int> m_startIndex;

		private Holder<int> m_currentIndex;

		private List<CellList> m_cellLists;

		private List<TablixColumn> m_tablixColumns;

		private RowList m_rows;

		private ScopeTree m_scopeTree;

		private Dictionary<string, string> m_scopeNameMap;

		private Dictionary<string, string> m_reportItemNameMap;

		private Dictionary<string, string> m_aggregateMap;

		private Dictionary<string, string> m_lookupMap;

		private Dictionary<string, string> m_variableNameMap;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> m_expressionsWithReportItemReferences;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility> m_visibilitiesWithToggleToUpdate;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem> m_reportItemsWithRepeatWithToUpdate;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.EndUserSort> m_endUserSortWithTarget;

		private Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope> m_reportScopes;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection> m_reportItemCollections;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.IAggregateHolder> m_aggregateHolders;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.IRunningValueHolder> m_runningValueHolders;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo m_outerAggregate;

		private Dictionary<string, IRIFDataScope> m_scopeNamesToClone;

		private IRIFDataScope m_currentDataScope;

		private NameValidator m_reportItemNameValidator;

		private NameValidator m_scopeNameValidator;

		private NameValidator m_variableNameValidator;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

		private bool m_dynamicWithStaticPeerEncountered;

		private int m_headerLevel;

		private int m_originalColumnCount;

		private int m_originalRowCount;

		private bool[] m_headerLevelHasStaticArray;

		private Holder<int> m_variableSequenceIdCounter;

		private Holder<int> m_textboxSequenceIdCounter;

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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion CurrentDataRegion
		{
			get
			{
				return this.m_currentDataRegion;
			}
			set
			{
				this.m_currentDataRegion = value;
			}
		}

		internal IRIFDataScope CurrentDataScope
		{
			get
			{
				return this.m_currentDataScope;
			}
			set
			{
				this.m_currentDataScope = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion CurrentDataRegionClone
		{
			get
			{
				return this.m_currentDataRegionClone;
			}
			set
			{
				this.m_currentDataRegionClone = value;
			}
		}

		internal Map CurrentMapClone
		{
			get
			{
				return this.m_currentMapClone;
			}
			set
			{
				this.m_currentMapClone = value;
			}
		}

		internal MapVectorLayer CurrentMapVectorLayerClone
		{
			get
			{
				return this.m_currentMapVectorLayerClone;
			}
			set
			{
				this.m_currentMapVectorLayerClone = value;
			}
		}

		internal string CurrentScope
		{
			get
			{
				return this.m_currentScope;
			}
			set
			{
				this.m_currentScope = value;
			}
		}

		internal string CurrentScopeBeingCloned
		{
			get
			{
				return this.m_currentScopeBeingCloned;
			}
			set
			{
				this.m_currentScopeBeingCloned = value;
			}
		}

		internal bool[] HeaderLevelHasStaticArray
		{
			get
			{
				return this.m_headerLevelHasStaticArray;
			}
			set
			{
				this.m_headerLevelHasStaticArray = value;
			}
		}

		internal List<ICreateSubtotals> CreateSubtotalsDefinitions
		{
			get
			{
				return this.m_createSubtotals;
			}
		}

		internal List<AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> DomainScopeGroups
		{
			get
			{
				return this.m_domainScopeGroups;
			}
		}

		internal int StartIndex
		{
			get
			{
				return this.m_startIndex.Value;
			}
			set
			{
				this.m_startIndex.Value = value;
			}
		}

		internal int CurrentIndex
		{
			get
			{
				return this.m_currentIndex.Value;
			}
			set
			{
				this.m_currentIndex.Value = value;
			}
		}

		internal List<CellList> CellLists
		{
			get
			{
				return this.m_cellLists;
			}
			set
			{
				this.m_cellLists = value;
			}
		}

		internal List<TablixColumn> TablixColumns
		{
			get
			{
				return this.m_tablixColumns;
			}
			set
			{
				this.m_tablixColumns = value;
			}
		}

		internal RowList Rows
		{
			get
			{
				return this.m_rows;
			}
			set
			{
				this.m_rows = value;
			}
		}

		internal bool DynamicWithStaticPeerEncountered
		{
			get
			{
				return this.m_dynamicWithStaticPeerEncountered;
			}
			set
			{
				this.m_dynamicWithStaticPeerEncountered = value;
			}
		}

		internal int HeaderLevel
		{
			get
			{
				return this.m_headerLevel;
			}
			set
			{
				this.m_headerLevel = value;
			}
		}

		internal int OriginalColumnCount
		{
			get
			{
				return this.m_originalColumnCount;
			}
			set
			{
				this.m_originalColumnCount = value;
			}
		}

		internal int OriginalRowCount
		{
			get
			{
				return this.m_originalRowCount;
			}
			set
			{
				this.m_originalRowCount = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo OuterAggregate
		{
			get
			{
				return this.m_outerAggregate;
			}
			set
			{
				this.m_outerAggregate = value;
			}
		}

		internal Dictionary<string, IRIFDataScope> ScopeNamesToClone
		{
			get
			{
				return this.m_scopeNamesToClone;
			}
		}

		internal AutomaticSubtotalContext(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, List<ICreateSubtotals> createSubtotals, List<AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping> domainScopeGroups, NameValidator reportItemNameValidator, NameValidator scopeNameValidator, NameValidator variableNameValidator, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope> reportScopes, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection> reportItemCollections, List<AspNetCore.ReportingServices.ReportIntermediateFormat.IAggregateHolder> aggregateHolders, List<AspNetCore.ReportingServices.ReportIntermediateFormat.IRunningValueHolder> runningValueHolders, Holder<int> variableSequenceIdCounter, Holder<int> textboxSequenceIdCounter, ScopeTree scopeTree)
		{
			this.m_createSubtotals = createSubtotals;
			this.m_domainScopeGroups = domainScopeGroups;
			this.m_reportItemNameValidator = reportItemNameValidator;
			this.m_scopeNameValidator = scopeNameValidator;
			this.m_variableNameValidator = variableNameValidator;
			this.m_report = report;
			this.m_variableSequenceIdCounter = variableSequenceIdCounter;
			this.m_textboxSequenceIdCounter = textboxSequenceIdCounter;
			this.m_dynamicWithStaticPeerEncountered = false;
			this.m_location = LocationFlags.None;
			this.m_objectName = null;
			this.m_objectType = ObjectType.Tablix;
			this.m_currentDataRegion = null;
			this.m_cellLists = null;
			this.m_tablixColumns = null;
			this.m_rows = null;
			this.m_scopeNameMap = new Dictionary<string, string>(StringComparer.Ordinal);
			this.m_reportItemNameMap = new Dictionary<string, string>(StringComparer.Ordinal);
			this.m_aggregateMap = new Dictionary<string, string>(StringComparer.Ordinal);
			this.m_lookupMap = new Dictionary<string, string>(StringComparer.Ordinal);
			this.m_variableNameMap = new Dictionary<string, string>(StringComparer.Ordinal);
			this.m_currentScope = null;
			this.m_currentScopeBeingCloned = null;
			this.m_startIndex = new Holder<int>();
			this.m_currentIndex = new Holder<int>();
			this.m_headerLevel = 0;
			this.m_originalColumnCount = 0;
			this.m_originalRowCount = 0;
			this.m_reportScopes = reportScopes;
			this.m_reportItemCollections = reportItemCollections;
			this.m_aggregateHolders = aggregateHolders;
			this.m_runningValueHolders = runningValueHolders;
			this.m_expressionsWithReportItemReferences = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo>();
			this.m_visibilitiesWithToggleToUpdate = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility>();
			this.m_reportItemsWithRepeatWithToUpdate = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem>();
			this.m_endUserSortWithTarget = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.EndUserSort>();
			this.m_scopeNamesToClone = new Dictionary<string, IRIFDataScope>(StringComparer.Ordinal);
			this.m_headerLevelHasStaticArray = null;
			this.m_currentDataRegionClone = null;
			this.m_currentMapClone = null;
			this.m_outerAggregate = null;
			this.m_scopeTree = scopeTree;
			this.m_currentDataScope = null;
			this.m_currentMapVectorLayerClone = null;
		}

		internal int GenerateVariableSequenceID()
		{
			return this.m_variableSequenceIdCounter.Value++;
		}

		internal int GenerateTextboxSequenceID()
		{
			return this.m_textboxSequenceIdCounter.Value++;
		}

		internal bool HasStaticPeerWithHeader(TablixMember member, out int spanDifference)
		{
			spanDifference = 0;
			if (member.HeaderLevel == -1)
			{
				return false;
			}
			int num = member.HeaderLevel + (member.IsColumn ? member.RowSpan : member.ColSpan);
			Global.Tracer.Assert(num <= this.m_headerLevelHasStaticArray.Length, "(count <= m_headerLevelHasStaticArray.Length)");
			for (int i = member.HeaderLevel; i < num; i++)
			{
				if (this.m_headerLevelHasStaticArray[i])
				{
					if (member.Grouping != null)
					{
						this.m_dynamicWithStaticPeerEncountered = true;
						spanDifference = i - member.HeaderLevel;
					}
					return true;
				}
			}
			return false;
		}

		internal void AddReportItemCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection collection)
		{
			this.m_reportItemCollections.Add(collection);
		}

		internal void AddAggregateHolder(AspNetCore.ReportingServices.ReportIntermediateFormat.IAggregateHolder aggregateHolder)
		{
			this.m_aggregateHolders.Add(aggregateHolder);
		}

		internal void AddRunningValueHolder(AspNetCore.ReportingServices.ReportIntermediateFormat.IRunningValueHolder runningValueHolder)
		{
			this.m_runningValueHolders.Add(runningValueHolder);
		}

		internal string CreateUniqueReportItemName(string oldName, bool isClone)
		{
			return this.CreateUniqueReportItemName(oldName, false, isClone);
		}

		internal string CreateUniqueReportItemName(string oldName, bool emptyRectangle, bool isClone)
		{
			string text = null;
			if (!emptyRectangle && this.m_reportItemNameMap.TryGetValue(oldName, out text))
			{
				return text;
			}
			StringBuilder stringBuilder = null;
			int num = 1;
			if (isClone)
			{
				num = 2;
			}
			do
			{
				stringBuilder = new StringBuilder();
				stringBuilder.Append(oldName);
				if (emptyRectangle && !isClone)
				{
					stringBuilder.Append("_AsRectangle");
				}
				else if (isClone)
				{
					stringBuilder.Append("_");
				}
				else
				{
					stringBuilder.Append("_InAutoSubtotal");
				}
				stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture.NumberFormat));
				text = stringBuilder.ToString();
				num++;
			}
			while (!this.m_reportItemNameValidator.Validate(text));
			if (!emptyRectangle)
			{
				if (this.m_reportItemNameMap.ContainsKey(oldName))
				{
					this.m_reportItemNameMap[oldName] = text;
				}
				else
				{
					this.m_reportItemNameMap.Add(oldName, text);
				}
			}
			return text;
		}

		internal string GetNewReportItemName(string oldName)
		{
			string result = default(string);
			if (this.m_reportItemNameMap.TryGetValue(oldName, out result))
			{
				return result;
			}
			return oldName;
		}

		internal string CreateAndRegisterUniqueGroupName(string oldName, bool isClone)
		{
			return this.CreateAndRegisterUniqueGroupName(oldName, isClone, false);
		}

		internal string CreateAndRegisterUniqueGroupName(string oldName, bool isClone, bool isDomainScope)
		{
			string text = null;
			if (this.m_scopeNameMap.TryGetValue(oldName, out text))
			{
				return text;
			}
			StringBuilder stringBuilder = null;
			int num = 1;
			if (isClone)
			{
				num = 2;
			}
			do
			{
				stringBuilder = ((!isDomainScope) ? ((!isClone) ? new StringBuilder(oldName.Length + 19) : new StringBuilder(oldName.Length + 4)) : new StringBuilder(oldName.Length + 16));
				stringBuilder.Append(oldName);
				if (isDomainScope)
				{
					stringBuilder.Append("_DomainScope");
				}
				else if (isClone)
				{
					stringBuilder.Append("_");
				}
				else
				{
					stringBuilder.Append("_InAutoSubtotal");
				}
				stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture.NumberFormat));
				text = stringBuilder.ToString();
				num++;
			}
			while (!this.m_scopeNameValidator.Validate(text));
			this.RegisterClonedScopeName(oldName, text);
			return text;
		}

		internal string CreateUniqueVariableName(string oldName, bool isClone)
		{
			StringBuilder stringBuilder = null;
			string text = null;
			int num = 1;
			if (isClone)
			{
				num = 2;
			}
			do
			{
				stringBuilder = ((!isClone) ? new StringBuilder(oldName.Length + 19) : new StringBuilder(oldName.Length + 4));
				stringBuilder.Append(oldName);
				if (isClone)
				{
					stringBuilder.Append("_");
				}
				else
				{
					stringBuilder.Append("_InAutoSubtotal");
				}
				stringBuilder.Append(num.ToString(CultureInfo.InvariantCulture.NumberFormat));
				text = stringBuilder.ToString();
				num++;
			}
			while (!this.m_variableNameValidator.Validate(text));
			Global.Tracer.Assert(!this.m_variableNameMap.ContainsKey(oldName), "(!m_variableNameMap.ContainsKey(oldName))");
			this.m_variableNameMap.Add(oldName, text);
			return text;
		}

		internal string GetNewVariableName(string oldVariableName)
		{
			string result = default(string);
			if (oldVariableName != null && oldVariableName.Length > 0 && this.m_variableNameMap.TryGetValue(oldVariableName, out result))
			{
				return result;
			}
			return oldVariableName;
		}

		internal string GetNewScopeName(string oldScopeName)
		{
			string result = default(string);
			if (oldScopeName != null && oldScopeName.Length > 0 && this.m_scopeNameMap.TryGetValue(oldScopeName, out result))
			{
				return result;
			}
			return oldScopeName;
		}

		internal string GetNewScopeNameForInnerOrOuterAggregate(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo originalAggregate)
		{
			string scope = originalAggregate.PublishingInfo.Scope;
			IRIFDataScope iRIFDataScope = default(IRIFDataScope);
			if (this.m_scopeNamesToClone.TryGetValue(scope, out iRIFDataScope))
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = iRIFDataScope as AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion;
				if (dataRegion != null)
				{
					return this.CreateUniqueReportItemName(scope, dataRegion.IsClone);
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = iRIFDataScope as AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode;
				if (reportHierarchyNode != null)
				{
					return this.CreateAndRegisterUniqueGroupName(scope, reportHierarchyNode.IsClone);
				}
				Global.Tracer.Assert(false, "Unknown object type in GetNewScopeNameForNestedAggregate: {0}", iRIFDataScope);
				return scope;
			}
			IRIFDataScope scopeByName = this.m_scopeTree.GetScopeByName(this.m_currentScope);
			int num = (scopeByName == null || !this.NeedsSubtotalScopeLift(originalAggregate, scopeByName)) ? (-1) : this.m_scopeTree.MeasureScopeDistance(this.m_currentScopeBeingCloned, this.m_currentScope);
			if (num <= 0)
			{
				return scope;
			}
			string text = this.m_scopeTree.FindAncestorScopeName(scope, num);
			if (text == null)
			{
				return scope;
			}
			if (this.m_outerAggregate != null && !string.IsNullOrEmpty(this.m_outerAggregate.PublishingInfo.Scope))
			{
				IRIFDataScope scopeByName2 = this.m_scopeTree.GetScopeByName(this.m_outerAggregate.PublishingInfo.Scope);
				IRIFDataScope scopeByName3 = this.m_scopeTree.GetScopeByName(text);
				if (scopeByName2 != null && scopeByName3 != null && this.m_scopeTree.IsParentScope(scopeByName3, scopeByName2))
				{
					text = this.m_outerAggregate.PublishingInfo.Scope;
				}
			}
			return text;
		}

		private bool NeedsSubtotalScopeLift(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo aggregate, IRIFDataScope displayScope)
		{
			if (aggregate.PublishingInfo.HasScope)
			{
				IRIFDataScope scopeByName = this.m_scopeTree.GetScopeByName(aggregate.PublishingInfo.Scope);
				if (scopeByName == null)
				{
					return false;
				}
				if (this.m_scopeTree.IsParentScope(displayScope, scopeByName))
				{
					return true;
				}
				return false;
			}
			return true;
		}

		internal void RegisterScopeName(string name)
		{
			Global.Tracer.Assert(!this.m_scopeNameMap.ContainsKey(name), "(!m_scopeNameMap.ContainsKey(name))");
			this.m_scopeNameMap.Add(name, this.m_currentScope);
			this.m_currentScopeBeingCloned = name;
		}

		internal void RegisterClonedScopeName(string oldName, string newName)
		{
			Global.Tracer.Assert(!this.m_scopeNameMap.ContainsKey(oldName), "(!m_scopeNameMap.ContainsKey(oldName))");
			this.m_scopeNameMap.Add(oldName, newName);
		}

		internal string CreateAggregateID(string oldID)
		{
			this.m_report.LastAggregateID++;
			string text = "Aggregate" + this.m_report.LastAggregateID;
			Global.Tracer.Assert(!this.m_aggregateMap.ContainsKey(oldID), "(!m_aggregateMap.ContainsKey(oldID))");
			this.m_aggregateMap.Add(oldID, text);
			return text;
		}

		internal string CreateLookupID(string oldID)
		{
			this.m_report.LastLookupID++;
			string text = "Lookup" + this.m_report.LastLookupID;
			Global.Tracer.Assert(!this.m_lookupMap.ContainsKey(oldID), "(!m_lookupMap.ContainsKey(oldID))");
			this.m_lookupMap.Add(oldID, text);
			return text;
		}

		internal string GetNewAggregateID(string oldID)
		{
			string result = default(string);
			if (this.m_aggregateMap.TryGetValue(oldID, out result))
			{
				return result;
			}
			return oldID;
		}

		internal string GetNewLookupID(string oldID)
		{
			string result = default(string);
			if (this.m_lookupMap.TryGetValue(oldID, out result))
			{
				return result;
			}
			return oldID;
		}

		internal int GenerateID()
		{
			this.m_report.LastID++;
			return this.m_report.LastID;
		}

		internal void AdjustReferences()
		{
			if (this.m_expressionsWithReportItemReferences.Count > 0)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionsWithReportItemReference in this.m_expressionsWithReportItemReferences)
				{
					expressionsWithReportItemReference.UpdateReportItemReferences(this);
				}
			}
			if (this.m_visibilitiesWithToggleToUpdate.Count > 0)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility item in this.m_visibilitiesWithToggleToUpdate)
				{
					item.UpdateToggleItemReference(this);
				}
			}
			if (this.m_reportItemsWithRepeatWithToUpdate.Count > 0)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem item2 in this.m_reportItemsWithRepeatWithToUpdate)
				{
					item2.UpdateRepeatWithReference(this);
				}
			}
			if (this.m_endUserSortWithTarget.Count > 0)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.EndUserSort item3 in this.m_endUserSortWithTarget)
				{
					item3.UpdateSortScopeAndTargetReference(this);
				}
			}
			this.m_lookupMap.Clear();
			this.m_aggregateMap.Clear();
			this.m_reportItemNameMap.Clear();
			this.m_variableNameMap.Clear();
			this.m_visibilitiesWithToggleToUpdate.Clear();
			this.m_reportItemsWithRepeatWithToUpdate.Clear();
			this.m_expressionsWithReportItemReferences.Clear();
			this.m_endUserSortWithTarget.Clear();
			this.m_scopeNameMap.Clear();
			this.m_scopeNamesToClone.Clear();
		}

		internal void AddSortTarget(string scopeName, AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope target)
		{
			Global.Tracer.Assert(!this.m_reportScopes.ContainsKey(scopeName), "(!m_reportScopes.ContainsKey(scopeName))");
			this.m_reportScopes.Add(scopeName, target);
		}

		internal bool TryGetNewSortTarget(string scopeName, out AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope target)
		{
			target = null;
			return this.m_reportScopes.TryGetValue(scopeName, out target);
		}

		internal void AddExpressionThatReferencesReportItems(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
		{
			this.m_expressionsWithReportItemReferences.Add(expression);
		}

		internal void AddVisibilityWithToggleToUpdate(AspNetCore.ReportingServices.ReportIntermediateFormat.Visibility visibility)
		{
			this.m_visibilitiesWithToggleToUpdate.Add(visibility);
		}

		internal void AddReportItemWithRepeatWithToUpdate(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem)
		{
			this.m_reportItemsWithRepeatWithToUpdate.Add(reportItem);
		}

		internal void AddEndUserSort(AspNetCore.ReportingServices.ReportIntermediateFormat.EndUserSort endUserSort)
		{
			this.m_endUserSortWithTarget.Add(endUserSort);
		}

		internal void AddSubReport(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			this.m_report.SubReports.Add(subReport);
		}
	}
}
