using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class UserSortFilterContext
	{
		private TextBox m_currentSortFilterEventSource;

		private RuntimeSortFilterEventInfoList m_runtimeSortFilterInfo;

		private ReportProcessing.IScope m_currentContainingScope;

		private GroupingList m_containingScopes;

		private int m_dataSetID = -1;

		private SubReportList m_detailScopeSubReports;

		private int[] m_inProcessUserSortPhase;

		internal TextBox CurrentSortFilterEventSource
		{
			get
			{
				return this.m_currentSortFilterEventSource;
			}
			set
			{
				this.m_currentSortFilterEventSource = value;
			}
		}

		internal RuntimeSortFilterEventInfoList RuntimeSortFilterInfo
		{
			get
			{
				return this.m_runtimeSortFilterInfo;
			}
			set
			{
				this.m_runtimeSortFilterInfo = value;
			}
		}

		internal int DataSetID
		{
			get
			{
				return this.m_dataSetID;
			}
			set
			{
				this.m_dataSetID = value;
			}
		}

		internal ReportProcessing.IScope CurrentContainingScope
		{
			get
			{
				return this.m_currentContainingScope;
			}
			set
			{
				this.m_currentContainingScope = value;
			}
		}

		internal GroupingList ContainingScopes
		{
			get
			{
				return this.m_containingScopes;
			}
			set
			{
				this.m_containingScopes = value;
			}
		}

		internal SubReportList DetailScopeSubReports
		{
			get
			{
				return this.m_detailScopeSubReports;
			}
			set
			{
				this.m_detailScopeSubReports = value;
			}
		}

		internal UserSortFilterContext()
		{
		}

		internal UserSortFilterContext(UserSortFilterContext copy)
		{
			this.m_runtimeSortFilterInfo = copy.RuntimeSortFilterInfo;
			this.m_currentContainingScope = copy.CurrentContainingScope;
			this.m_containingScopes = copy.ContainingScopes;
			this.m_dataSetID = copy.DataSetID;
			this.m_detailScopeSubReports = copy.DetailScopeSubReports;
			this.m_inProcessUserSortPhase = copy.m_inProcessUserSortPhase;
		}

		internal UserSortFilterContext(UserSortFilterContext parentContext, SubReport subReport)
		{
			this.m_runtimeSortFilterInfo = parentContext.RuntimeSortFilterInfo;
			this.m_dataSetID = parentContext.DataSetID;
			this.m_containingScopes = subReport.ContainingScopes;
			this.m_detailScopeSubReports = subReport.DetailScopeSubReports;
			this.m_inProcessUserSortPhase = parentContext.m_inProcessUserSortPhase;
		}

		internal bool PopulateRuntimeSortFilterEventInfo(ReportProcessing.ProcessingContext pc, DataSet myDataSet)
		{
			if (pc.UserSortFilterInfo != null && pc.UserSortFilterInfo.SortInfo != null && pc.OldSortFilterEventInfo != null)
			{
				if (this.m_dataSetID != -1)
				{
					return false;
				}
				this.m_runtimeSortFilterInfo = null;
				EventInformation.SortEventInfo sortInfo = pc.UserSortFilterInfo.SortInfo;
				for (int i = 0; i < sortInfo.Count; i++)
				{
					int uniqueNameAt = sortInfo.GetUniqueNameAt(i);
					SortFilterEventInfo sortFilterEventInfo = pc.OldSortFilterEventInfo[uniqueNameAt];
					if (sortFilterEventInfo != null && sortFilterEventInfo.EventSource.UserSort != null && sortFilterEventInfo.EventSource.UserSort.DataSetID == myDataSet.ID)
					{
						if (this.m_runtimeSortFilterInfo == null)
						{
							this.m_runtimeSortFilterInfo = new RuntimeSortFilterEventInfoList();
						}
						this.m_runtimeSortFilterInfo.Add(new RuntimeSortFilterEventInfo(sortFilterEventInfo.EventSource, uniqueNameAt, sortInfo.GetSortDirectionAt(i), sortFilterEventInfo.EventSourceScopeInfo));
					}
				}
				if (this.m_runtimeSortFilterInfo != null)
				{
					int count = this.m_runtimeSortFilterInfo.Count;
					for (int j = 0; j < count; j++)
					{
						TextBox eventSource = this.m_runtimeSortFilterInfo[j].EventSource;
						ISortFilterScope sortExpressionScope = eventSource.UserSort.SortExpressionScope;
						if (sortExpressionScope != null)
						{
							sortExpressionScope.IsSortFilterExpressionScope = this.SetSortFilterInfo(sortExpressionScope.IsSortFilterExpressionScope, count, j);
						}
						ISortFilterScope sortTarget = eventSource.UserSort.SortTarget;
						if (sortTarget != null)
						{
							sortTarget.IsSortFilterTarget = this.SetSortFilterInfo(sortTarget.IsSortFilterTarget, count, j);
						}
						if (eventSource.ContainingScopes != null && 0 < eventSource.ContainingScopes.Count)
						{
							int num = 0;
							for (int k = 0; k < eventSource.ContainingScopes.Count; k++)
							{
								Grouping grouping = eventSource.ContainingScopes[k];
								VariantList variantList = this.m_runtimeSortFilterInfo[j].SortSourceScopeInfo[k];
								if (grouping != null)
								{
									if (grouping.SortFilterScopeInfo == null)
									{
										grouping.SortFilterScopeInfo = new VariantList[count];
										for (int l = 0; l < count; l++)
										{
											grouping.SortFilterScopeInfo[l] = null;
										}
										grouping.SortFilterScopeIndex = new int[count];
										for (int m = 0; m < count; m++)
										{
											grouping.SortFilterScopeIndex[m] = -1;
										}
									}
									grouping.SortFilterScopeInfo[j] = variantList;
									grouping.SortFilterScopeIndex[j] = k;
								}
								else
								{
									SubReportList detailScopeSubReports = eventSource.UserSort.DetailScopeSubReports;
									ReportItem parent;
									if (detailScopeSubReports != null && num < detailScopeSubReports.Count)
									{
										parent = detailScopeSubReports[num++].Parent;
									}
									else
									{
										Global.Tracer.Assert(k == eventSource.ContainingScopes.Count - 1, "(j == eventSource.ContainingScopes.Count - 1)");
										parent = eventSource.Parent;
									}
									while (parent != null && !(parent is DataRegion))
									{
										parent = parent.Parent;
									}
									Global.Tracer.Assert(parent is DataRegion, "(parent is DataRegion)");
									DataRegion dataRegion = (DataRegion)parent;
									if (dataRegion.SortFilterSourceDetailScopeInfo == null)
									{
										dataRegion.SortFilterSourceDetailScopeInfo = new int[count];
										for (int n = 0; n < count; n++)
										{
											dataRegion.SortFilterSourceDetailScopeInfo[n] = -1;
										}
									}
									Global.Tracer.Assert(variantList != null && 1 == variantList.Count, "(null != scopeValues && 1 == scopeValues.Count)");
									dataRegion.SortFilterSourceDetailScopeInfo[j] = (int)((ArrayList)variantList)[0];
								}
							}
						}
						GroupingList groupsInSortTarget = eventSource.UserSort.GroupsInSortTarget;
						if (groupsInSortTarget != null)
						{
							for (int num3 = 0; num3 < groupsInSortTarget.Count; num3++)
							{
								groupsInSortTarget[num3].NeedScopeInfoForSortFilterExpression = this.SetSortFilterInfo(groupsInSortTarget[num3].NeedScopeInfoForSortFilterExpression, count, j);
							}
						}
						IntList peerSortFilters = eventSource.GetPeerSortFilters(false);
						if (peerSortFilters != null)
						{
							if (this.m_runtimeSortFilterInfo[j].PeerSortFilters == null)
							{
								this.m_runtimeSortFilterInfo[j].PeerSortFilters = new Hashtable();
							}
							for (int num4 = 0; num4 < peerSortFilters.Count; num4++)
							{
								if (eventSource.ID != peerSortFilters[num4])
								{
									this.m_runtimeSortFilterInfo[j].PeerSortFilters.Add(peerSortFilters[num4], null);
								}
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		private bool[] SetSortFilterInfo(bool[] source, int count, int index)
		{
			bool[] array = source;
			if (array == null)
			{
				array = new bool[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = false;
				}
			}
			array[index] = true;
			return array;
		}

		internal bool IsSortFilterTarget(bool[] isSortFilterTarget, ReportProcessing.IScope outerScope, ReportProcessing.IHierarchyObj target, ref RuntimeUserSortTargetInfo userSortTargetInfo)
		{
			bool result = false;
			if (this.m_runtimeSortFilterInfo != null && isSortFilterTarget != null && (outerScope == null || !outerScope.TargetForNonDetailSort))
			{
				for (int i = 0; i < this.m_runtimeSortFilterInfo.Count; i++)
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = this.m_runtimeSortFilterInfo[i];
					if (runtimeSortFilterEventInfo.EventTarget == null && isSortFilterTarget[i] && (outerScope == null || outerScope.TargetScopeMatched(i, false)))
					{
						runtimeSortFilterEventInfo.EventTarget = target;
						if (userSortTargetInfo == null)
						{
							userSortTargetInfo = new RuntimeUserSortTargetInfo(target, i, runtimeSortFilterEventInfo);
						}
						else
						{
							userSortTargetInfo.AddSortInfo(target, i, runtimeSortFilterEventInfo);
						}
						result = true;
					}
				}
			}
			return result;
		}

		internal void RegisterSortFilterExpressionScope(ReportProcessing.IScope container, ReportProcessing.RuntimeDataRegionObj scopeObj, bool[] isSortFilterExpressionScope)
		{
			RuntimeSortFilterEventInfoList runtimeSortFilterInfo = this.m_runtimeSortFilterInfo;
			if (runtimeSortFilterInfo != null && isSortFilterExpressionScope != null && scopeObj != null)
			{
				VariantList[] array = null;
				for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
				{
					if (isSortFilterExpressionScope[i] && scopeObj.IsTargetForSort(i, false) && scopeObj.TargetScopeMatched(i, false))
					{
						if (array == null && runtimeSortFilterInfo[i].EventSource.UserSort.GroupsInSortTarget != null)
						{
							int num = 0;
							array = new VariantList[runtimeSortFilterInfo[i].EventSource.UserSort.GroupsInSortTarget.Count];
							scopeObj.GetScopeValues(runtimeSortFilterInfo[i].EventTarget, array, ref num);
						}
						runtimeSortFilterInfo[i].RegisterSortFilterExpressionScope(ref container.SortFilterExpressionScopeInfoIndices[i], scopeObj, array, i);
					}
				}
			}
		}

		internal bool ProcessUserSort(ReportProcessing.ProcessingContext processingContext)
		{
			bool result = false;
			RuntimeSortFilterEventInfoList runtimeSortFilterInfo = this.m_runtimeSortFilterInfo;
			if (runtimeSortFilterInfo != null)
			{
				bool flag;
				bool flag2;
				do
				{
					flag = false;
					flag2 = true;
					this.ProcessUserSort(processingContext, ref flag, ref flag2, ref result);
				}
				while (flag && !flag2);
			}
			return result;
		}

		private void ProcessUserSort(ReportProcessing.ProcessingContext processingContext, ref bool processed, ref bool canStop, ref bool processedAny)
		{
			RuntimeSortFilterEventInfoList runtimeSortFilterInfo = this.m_runtimeSortFilterInfo;
			for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
			{
				if (!runtimeSortFilterInfo[i].Processed)
				{
					runtimeSortFilterInfo[i].PrepareForSorting(processingContext);
				}
			}
			for (int j = 0; j < runtimeSortFilterInfo.Count; j++)
			{
				if (!runtimeSortFilterInfo[j].Processed)
				{
					if (runtimeSortFilterInfo[j].ProcessSorting(processingContext))
					{
						processedAny = true;
						processed = true;
					}
					else
					{
						canStop = false;
					}
				}
			}
		}

		internal void ProcessUserSortForTarget(ObjectModelImpl reportObjectModel, ReportRuntime reportRuntime, ReportProcessing.IHierarchyObj target, ref ReportProcessing.DataRowList dataRows, bool targetForNonDetailSort)
		{
			if (targetForNonDetailSort && dataRows != null && 0 < dataRows.Count)
			{
				RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = null;
				IntList sortFilterInfoIndices = target.SortFilterInfoIndices;
				Global.Tracer.Assert(null != target.SortTree, "(null != target.SortTree)");
				if (sortFilterInfoIndices != null)
				{
					runtimeSortFilterEventInfo = this.m_runtimeSortFilterInfo[sortFilterInfoIndices[0]];
				}
				for (int i = 0; i < dataRows.Count; i++)
				{
					reportObjectModel.FieldsImpl.SetFields(dataRows[i]);
					object keyValue = DBNull.Value;
					if (runtimeSortFilterEventInfo != null)
					{
						keyValue = runtimeSortFilterEventInfo.GetSortOrder(reportRuntime);
					}
					target.SortTree.NextRow(keyValue);
				}
				dataRows = null;
			}
			target.MarkSortInfoProcessed(this.m_runtimeSortFilterInfo);
		}

		internal void EnterProcessUserSortPhase(int index)
		{
			if (this.m_inProcessUserSortPhase == null)
			{
				if (this.m_runtimeSortFilterInfo == null || this.m_runtimeSortFilterInfo.Count == 0)
				{
					return;
				}
				this.m_inProcessUserSortPhase = new int[this.m_runtimeSortFilterInfo.Count];
				for (int i = 0; i < this.m_runtimeSortFilterInfo.Count; i++)
				{
					this.m_inProcessUserSortPhase[i] = 0;
				}
			}
			this.m_inProcessUserSortPhase[index]++;
		}

		internal void LeaveProcessUserSortPhase(int index)
		{
			if (this.m_inProcessUserSortPhase != null)
			{
				this.m_inProcessUserSortPhase[index]--;
				Global.Tracer.Assert(0 <= this.m_inProcessUserSortPhase[index], "(0 <= m_inProcessUserSortPhase[index])");
			}
		}

		internal bool InProcessUserSortPhase(int index)
		{
			if (this.m_inProcessUserSortPhase == null)
			{
				return false;
			}
			return this.m_inProcessUserSortPhase[index] > 0;
		}

		internal void UpdateContextFromDataSet(UserSortFilterContext dataSetContext)
		{
			if (-1 == this.m_dataSetID)
			{
				this.m_dataSetID = dataSetContext.DataSetID;
				this.m_runtimeSortFilterInfo = dataSetContext.RuntimeSortFilterInfo;
			}
		}
	}
}
