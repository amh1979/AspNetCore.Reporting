using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class RuntimeSortFilterEventInfo
	{
		private class SortFilterExpressionScopeObj : ReportProcessing.IHierarchyObj
		{
			private RuntimeSortFilterEventInfo m_owner;

			private ReportProcessing.RuntimeDataRegionObjList m_scopeInstances;

			private ArrayList m_scopeValuesList;

			private ReportProcessing.BTreeNode m_sortTree;

			private int m_currentScopeInstanceIndex = -1;

			internal int CurrentScopeInstanceIndex
			{
				get
				{
					return this.m_currentScopeInstanceIndex;
				}
			}

			internal bool SortDirection
			{
				get
				{
					return this.m_owner.SortDirection;
				}
			}

			ReportProcessing.IHierarchyObj ReportProcessing.IHierarchyObj.HierarchyRoot
			{
				get
				{
					return this;
				}
			}

			ReportProcessing.ProcessingContext ReportProcessing.IHierarchyObj.ProcessingContext
			{
				get
				{
					Global.Tracer.Assert(0 < this.m_scopeInstances.Count, "(0 < m_scopeInstances.Count)");
					return this.m_scopeInstances[0].ProcessingContext;
				}
			}

			ReportProcessing.BTreeNode ReportProcessing.IHierarchyObj.SortTree
			{
				get
				{
					return this.m_sortTree;
				}
				set
				{
					this.m_sortTree = value;
				}
			}

			int ReportProcessing.IHierarchyObj.ExpressionIndex
			{
				get
				{
					return 0;
				}
			}

			IntList ReportProcessing.IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
					return null;
				}
			}

			bool ReportProcessing.IHierarchyObj.IsDetail
			{
				get
				{
					return false;
				}
			}

			internal SortFilterExpressionScopeObj(RuntimeSortFilterEventInfo owner)
			{
				this.m_owner = owner;
				this.m_scopeInstances = new ReportProcessing.RuntimeDataRegionObjList();
				this.m_scopeValuesList = new ArrayList();
			}

			ReportProcessing.IHierarchyObj ReportProcessing.IHierarchyObj.CreateHierarchyObj()
			{
				return new SortExpressionScopeInstanceHolder(this);
			}

			ProcessingMessageList ReportProcessing.IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				return ((ReportProcessing.IHierarchyObj)this).ProcessingContext.RegisterComparisonErrorForSortFilterEvent(propertyName);
			}

			void ReportProcessing.IHierarchyObj.NextRow()
			{
				Global.Tracer.Assert(false);
			}

			void ReportProcessing.IHierarchyObj.Traverse(ReportProcessing.ProcessingStages operation)
			{
				if (this.m_sortTree != null)
				{
					this.m_sortTree.Traverse(operation, this.m_owner.SortDirection);
				}
			}

			void ReportProcessing.IHierarchyObj.ReadRow()
			{
				Global.Tracer.Assert(false);
			}

			void ReportProcessing.IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(false);
			}

			void ReportProcessing.IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(false);
			}

			void ReportProcessing.IHierarchyObj.AddSortInfoIndex(int sortFilterInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				Global.Tracer.Assert(false);
			}

			internal void RegisterScopeInstance(ReportProcessing.RuntimeDataRegionObj scopeObj, VariantList[] scopeValues)
			{
				this.m_scopeInstances.Add(scopeObj);
				this.m_scopeValuesList.Add(scopeValues);
			}

			internal void SortSEScopes(ReportProcessing.ProcessingContext processingContext, TextBox eventSource)
			{
				this.m_sortTree = new ReportProcessing.BTreeNode(this);
				for (int i = 0; i < this.m_scopeInstances.Count; i++)
				{
					ReportProcessing.RuntimeDataRegionObj runtimeDataRegionObj = this.m_scopeInstances[i];
					this.m_currentScopeInstanceIndex = i;
					runtimeDataRegionObj.SetupEnvironment();
					this.m_sortTree.NextRow(processingContext.ReportRuntime.EvaluateUserSortExpression(eventSource));
				}
			}

			internal void AddSortOrder(int scopeInstanceIndex, bool incrementCounter)
			{
				this.m_owner.AddSortOrder((VariantList[])this.m_scopeValuesList[scopeInstanceIndex], incrementCounter);
			}
		}

		private class SortExpressionScopeInstanceHolder : ReportProcessing.IHierarchyObj
		{
			private SortFilterExpressionScopeObj m_owner;

			private IntList m_scopeInstanceIndices;

			ReportProcessing.IHierarchyObj ReportProcessing.IHierarchyObj.HierarchyRoot
			{
				get
				{
					return this;
				}
			}

			ReportProcessing.ProcessingContext ReportProcessing.IHierarchyObj.ProcessingContext
			{
				get
				{
					return ((ReportProcessing.IHierarchyObj)this.m_owner).ProcessingContext;
				}
			}

			ReportProcessing.BTreeNode ReportProcessing.IHierarchyObj.SortTree
			{
				get
				{
					return null;
				}
				set
				{
					Global.Tracer.Assert(false);
				}
			}

			int ReportProcessing.IHierarchyObj.ExpressionIndex
			{
				get
				{
					return -1;
				}
			}

			IntList ReportProcessing.IHierarchyObj.SortFilterInfoIndices
			{
				get
				{
					return null;
				}
			}

			bool ReportProcessing.IHierarchyObj.IsDetail
			{
				get
				{
					return false;
				}
			}

			internal SortExpressionScopeInstanceHolder(SortFilterExpressionScopeObj owner)
			{
				this.m_owner = owner;
				this.m_scopeInstanceIndices = new IntList();
			}

			ReportProcessing.IHierarchyObj ReportProcessing.IHierarchyObj.CreateHierarchyObj()
			{
				Global.Tracer.Assert(false);
				return null;
			}

			ProcessingMessageList ReportProcessing.IHierarchyObj.RegisterComparisonError(string propertyName)
			{
				Global.Tracer.Assert(false);
				return null;
			}

			void ReportProcessing.IHierarchyObj.NextRow()
			{
				this.m_scopeInstanceIndices.Add(this.m_owner.CurrentScopeInstanceIndex);
			}

			void ReportProcessing.IHierarchyObj.Traverse(ReportProcessing.ProcessingStages operation)
			{
				if (this.m_owner.SortDirection)
				{
					for (int i = 0; i < this.m_scopeInstanceIndices.Count; i++)
					{
						this.m_owner.AddSortOrder(this.m_scopeInstanceIndices[i], i == this.m_scopeInstanceIndices.Count - 1);
					}
				}
				else
				{
					for (int num = this.m_scopeInstanceIndices.Count - 1; num >= 0; num--)
					{
						this.m_owner.AddSortOrder(this.m_scopeInstanceIndices[num], num == 0);
					}
				}
			}

			void ReportProcessing.IHierarchyObj.ReadRow()
			{
				Global.Tracer.Assert(false);
			}

			void ReportProcessing.IHierarchyObj.ProcessUserSort()
			{
				Global.Tracer.Assert(false);
			}

			void ReportProcessing.IHierarchyObj.MarkSortInfoProcessed(RuntimeSortFilterEventInfoList runtimeSortFilterInfo)
			{
				Global.Tracer.Assert(false);
			}

			void ReportProcessing.IHierarchyObj.AddSortInfoIndex(int sortFilterInfoIndex, RuntimeSortFilterEventInfo sortInfo)
			{
				Global.Tracer.Assert(false);
			}
		}

		private TextBox m_eventSource;

		private int m_oldUniqueName;

		private VariantList[] m_sortSourceScopeInfo;

		private bool m_sortDirection;

		private ReportProcessing.IScope m_eventSourceScope;

		private int m_eventSourceDetailIndex = -1;

		private ReportProcessing.RuntimeDataRegionObjList m_detailScopes;

		private IntList m_detailScopeIndices;

		private ReportProcessing.IHierarchyObj m_eventTarget;

		private bool m_targetSortFilterInfoAdded;

		private ReportProcessing.RuntimeExpressionInfoList m_groupExpressionsInSortTarget;

		private ArrayList m_sortFilterExpressionScopeObjects;

		private int m_currentSortIndex = 1;

		private int m_currentInstanceIndex;

		private ScopeLookupTable m_sortOrders;

		private bool m_processed;

		private int m_nullScopeCount;

		private int m_newUniqueName = -1;

		private int m_page;

		private Hashtable m_peerSortFilters;

		internal TextBox EventSource
		{
			get
			{
				return this.m_eventSource;
			}
		}

		internal ReportProcessing.IScope EventSourceScope
		{
			get
			{
				return this.m_eventSourceScope;
			}
			set
			{
				this.m_eventSourceScope = value;
			}
		}

		internal int EventSourceDetailIndex
		{
			get
			{
				return this.m_eventSourceDetailIndex;
			}
			set
			{
				this.m_eventSourceDetailIndex = value;
			}
		}

		internal ReportProcessing.RuntimeDataRegionObjList DetailScopes
		{
			get
			{
				return this.m_detailScopes;
			}
			set
			{
				this.m_detailScopes = value;
			}
		}

		internal IntList DetailScopeIndices
		{
			get
			{
				return this.m_detailScopeIndices;
			}
			set
			{
				this.m_detailScopeIndices = value;
			}
		}

		internal bool SortDirection
		{
			get
			{
				return this.m_sortDirection;
			}
			set
			{
				this.m_sortDirection = value;
			}
		}

		internal VariantList[] SortSourceScopeInfo
		{
			get
			{
				return this.m_sortSourceScopeInfo;
			}
		}

		internal ReportProcessing.IHierarchyObj EventTarget
		{
			get
			{
				return this.m_eventTarget;
			}
			set
			{
				this.m_eventTarget = value;
			}
		}

		internal bool TargetSortFilterInfoAdded
		{
			get
			{
				return this.m_targetSortFilterInfoAdded;
			}
			set
			{
				this.m_targetSortFilterInfoAdded = value;
			}
		}

		internal bool Processed
		{
			get
			{
				return this.m_processed;
			}
			set
			{
				this.m_processed = value;
			}
		}

		internal int OldUniqueName
		{
			get
			{
				return this.m_oldUniqueName;
			}
			set
			{
				this.m_oldUniqueName = value;
			}
		}

		internal int NewUniqueName
		{
			get
			{
				return this.m_newUniqueName;
			}
			set
			{
				this.m_newUniqueName = value;
			}
		}

		internal int Page
		{
			get
			{
				return this.m_page;
			}
			set
			{
				this.m_page = value;
			}
		}

		internal Hashtable PeerSortFilters
		{
			get
			{
				return this.m_peerSortFilters;
			}
			set
			{
				this.m_peerSortFilters = value;
			}
		}

		internal RuntimeSortFilterEventInfo(TextBox eventSource, int oldUniqueName, bool sortDirection, VariantList[] sortSourceScopeInfo)
		{
			Global.Tracer.Assert(eventSource != null && null != eventSource.UserSort, "(null != eventSource && null != eventSource.UserSort)");
			this.m_eventSource = eventSource;
			this.m_oldUniqueName = oldUniqueName;
			this.m_sortDirection = sortDirection;
			this.m_sortSourceScopeInfo = sortSourceScopeInfo;
		}

		internal void RegisterSortFilterExpressionScope(ref int containerSortFilterExprScopeIndex, ReportProcessing.RuntimeDataRegionObj scopeObj, VariantList[] scopeValues, int sortFilterInfoIndex)
		{
			if (this.m_eventTarget != null && !this.m_targetSortFilterInfoAdded)
			{
				this.m_eventTarget.AddSortInfoIndex(sortFilterInfoIndex, this);
			}
			SortFilterExpressionScopeObj sortFilterExpressionScopeObj;
			if (-1 != containerSortFilterExprScopeIndex)
			{
				sortFilterExpressionScopeObj = (SortFilterExpressionScopeObj)this.m_sortFilterExpressionScopeObjects[containerSortFilterExprScopeIndex];
			}
			else
			{
				if (this.m_sortFilterExpressionScopeObjects == null)
				{
					this.m_sortFilterExpressionScopeObjects = new ArrayList();
				}
				containerSortFilterExprScopeIndex = this.m_sortFilterExpressionScopeObjects.Count;
				sortFilterExpressionScopeObj = new SortFilterExpressionScopeObj(this);
				this.m_sortFilterExpressionScopeObjects.Add(sortFilterExpressionScopeObj);
			}
			sortFilterExpressionScopeObj.RegisterScopeInstance(scopeObj, scopeValues);
		}

		internal void PrepareForSorting(ReportProcessing.ProcessingContext processingContext)
		{
			Global.Tracer.Assert(!this.m_processed, "(!m_processed)");
			if (this.m_eventTarget != null && this.m_sortFilterExpressionScopeObjects != null)
			{
				processingContext.UserSortFilterContext.CurrentSortFilterEventSource = this.m_eventSource;
				for (int i = 0; i < this.m_sortFilterExpressionScopeObjects.Count; i++)
				{
					SortFilterExpressionScopeObj sortFilterExpressionScopeObj = (SortFilterExpressionScopeObj)this.m_sortFilterExpressionScopeObjects[i];
					sortFilterExpressionScopeObj.SortSEScopes(processingContext, this.m_eventSource);
				}
				GroupingList groupsInSortTarget = this.m_eventSource.UserSort.GroupsInSortTarget;
				if (groupsInSortTarget != null && 0 < groupsInSortTarget.Count)
				{
					this.m_groupExpressionsInSortTarget = new ReportProcessing.RuntimeExpressionInfoList();
					for (int j = 0; j < groupsInSortTarget.Count; j++)
					{
						Grouping grouping = groupsInSortTarget[j];
						for (int k = 0; k < grouping.GroupExpressions.Count; k++)
						{
							this.m_groupExpressionsInSortTarget.Add(new ReportProcessing.RuntimeExpressionInfo(grouping.GroupExpressions, grouping.ExprHost, null, k));
						}
					}
				}
				this.CollectSortOrders();
			}
		}

		private void CollectSortOrders()
		{
			this.m_currentSortIndex = 1;
			for (int i = 0; i < this.m_sortFilterExpressionScopeObjects.Count; i++)
			{
				((ReportProcessing.IHierarchyObj)this.m_sortFilterExpressionScopeObjects[i]).Traverse(ReportProcessing.ProcessingStages.UserSortFilter);
			}
			this.m_sortFilterExpressionScopeObjects = null;
		}

		internal bool ProcessSorting(ReportProcessing.ProcessingContext processingContext)
		{
			Global.Tracer.Assert(!this.m_processed, "(!m_processed)");
			if (this.m_eventTarget == null)
			{
				return false;
			}
			this.m_eventTarget.ProcessUserSort();
			this.m_sortOrders = null;
			return true;
		}

		private void AddSortOrder(VariantList[] scopeValues, bool incrementCounter)
		{
			if (this.m_sortOrders == null)
			{
				this.m_sortOrders = new ScopeLookupTable();
			}
			if (scopeValues == null || scopeValues.Length == 0)
			{
				this.m_sortOrders.Add(this.m_eventSource.UserSort.GroupsInSortTarget, scopeValues, this.m_currentSortIndex);
			}
			else
			{
				int num = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					if (scopeValues[i] == null)
					{
						num++;
					}
				}
				if (num >= this.m_nullScopeCount)
				{
					if (num > this.m_nullScopeCount)
					{
						this.m_sortOrders.Clear();
						this.m_nullScopeCount = num;
					}
					this.m_sortOrders.Add(this.m_eventSource.UserSort.GroupsInSortTarget, scopeValues, this.m_currentSortIndex);
				}
			}
			if (incrementCounter)
			{
				this.m_currentSortIndex++;
			}
		}

		internal object GetSortOrder(ReportRuntime runtime)
		{
			object obj = null;
			if (this.m_eventSource.UserSort.SortExpressionScope == null)
			{
				obj = runtime.EvaluateUserSortExpression(this.m_eventSource);
			}
			else if (this.m_sortOrders == null)
			{
				obj = null;
			}
			else
			{
				GroupingList groupsInSortTarget = this.m_eventSource.UserSort.GroupsInSortTarget;
				if (groupsInSortTarget == null || groupsInSortTarget.Count == 0)
				{
					obj = this.m_sortOrders.LookupTable;
				}
				else
				{
					bool flag = true;
					bool flag2 = false;
					int num = 0;
					Hashtable hashtable = (Hashtable)this.m_sortOrders.LookupTable;
					int num2 = 0;
					int num3 = 0;
					while (num3 < groupsInSortTarget.Count)
					{
						IEnumerator enumerator = hashtable.Keys.GetEnumerator();
						enumerator.MoveNext();
						num2 = (int)enumerator.Current;
						for (int i = 0; i < num2; i++)
						{
							num += groupsInSortTarget[num3++].GroupExpressions.Count;
						}
						hashtable = (Hashtable)hashtable[num2];
						if (num3 < groupsInSortTarget.Count)
						{
							Grouping grouping = groupsInSortTarget[num3];
							for (int j = 0; j < grouping.GroupExpressions.Count; j++)
							{
								object key = runtime.EvaluateRuntimeExpression(this.m_groupExpressionsInSortTarget[num], ObjectType.Grouping, grouping.Name, "GroupExpression");
								num++;
								obj = hashtable[key];
								if (obj == null)
								{
									flag = false;
									break;
								}
								if (num < this.m_groupExpressionsInSortTarget.Count)
								{
									hashtable = (Hashtable)obj;
								}
							}
							num3++;
							if (!flag)
							{
								break;
							}
							continue;
						}
						flag2 = true;
						break;
					}
					if (flag && flag2)
					{
						obj = hashtable[1];
						if (obj == null)
						{
							flag = false;
						}
					}
					if (flag)
					{
						this.m_currentInstanceIndex = this.m_currentSortIndex + 1;
					}
					else
					{
						obj = this.m_currentInstanceIndex;
					}
				}
			}
			if (obj == null)
			{
				obj = DBNull.Value;
			}
			return obj;
		}

		internal void MatchEventSource(TextBox textBox, TextBoxInstance textBoxInstance, ReportProcessing.IScope containingScope, ReportProcessing.ProcessingContext processingContext)
		{
			bool flag = false;
			if (!(containingScope is ReportProcessing.RuntimePivotCell))
			{
				while (containingScope != null && !(containingScope is ReportProcessing.RuntimeGroupLeafObj) && !(containingScope is ReportProcessing.RuntimeDetailObj) && !(containingScope is ReportProcessing.RuntimeOnePassDetailObj))
				{
					containingScope = containingScope.GetOuterScope(true);
				}
			}
			if (containingScope == null)
			{
				if (this.m_eventSource.ContainingScopes == null || this.m_eventSource.ContainingScopes.Count == 0)
				{
					flag = true;
				}
			}
			else if (this.m_eventSourceScope == containingScope)
			{
				flag = true;
				DataRegion dataRegion = null;
				if (containingScope is ReportProcessing.RuntimeDetailObj)
				{
					dataRegion = ((ReportProcessing.RuntimeDetailObj)containingScope).DataRegionDef;
				}
				else if (containingScope is ReportProcessing.RuntimeOnePassDetailObj)
				{
					dataRegion = ((ReportProcessing.RuntimeOnePassDetailObj)containingScope).DataRegionDef;
				}
				if (dataRegion != null && dataRegion.CurrentDetailRowIndex != this.m_eventSourceDetailIndex)
				{
					flag = false;
				}
			}
			if (flag)
			{
				if (textBox == this.m_eventSource)
				{
					this.m_newUniqueName = textBoxInstance.UniqueName;
					this.m_page = processingContext.Pagination.GetTextBoxStartPage(textBox);
				}
				else if (this.m_peerSortFilters != null && this.m_peerSortFilters.Contains(textBox.ID))
				{
					this.m_peerSortFilters[textBox.ID] = textBoxInstance.UniqueName;
				}
			}
		}
	}
}
