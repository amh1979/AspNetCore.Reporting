using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class UserSortFilterContext
	{
		private IInScopeEventSource m_currentSortFilterEventSource;

		private List<IReference<RuntimeSortFilterEventInfo>> m_runtimeSortFilterInfo;

		private IReference<IScope> m_currentContainingScope;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GroupingList m_containingScopes;

		private int m_dataSetGlobalID = -1;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport> m_detailScopeSubReports;

		private int[] m_inProcessUserSortPhase;

		internal IInScopeEventSource CurrentSortFilterEventSource
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

		internal List<IReference<RuntimeSortFilterEventInfo>> RuntimeSortFilterInfo
		{
			get
			{
				return this.m_runtimeSortFilterInfo;
			}
		}

		internal int DataSetGlobalId
		{
			get
			{
				return this.m_dataSetGlobalID;
			}
			set
			{
				this.m_dataSetGlobalID = value;
			}
		}

		internal IReference<IScope> CurrentContainingScope
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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GroupingList ContainingScopes
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

		internal List<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport> DetailScopeSubReports
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

		internal UserSortFilterContext(UserSortFilterContext parentContext, AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			this.m_runtimeSortFilterInfo = parentContext.RuntimeSortFilterInfo;
			this.m_dataSetGlobalID = parentContext.DataSetGlobalId;
			this.m_inProcessUserSortPhase = parentContext.m_inProcessUserSortPhase;
			subReport.UpdateSubReportScopes(parentContext);
			this.m_containingScopes = subReport.ContainingScopes;
			this.m_detailScopeSubReports = subReport.DetailScopeSubReports;
		}

		internal List<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport> CloneDetailScopeSubReports()
		{
			if (this.m_detailScopeSubReports == null)
			{
				return null;
			}
			int count = this.m_detailScopeSubReports.Count;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport>(count);
			for (int i = 0; i < count; i++)
			{
				list.Add(this.m_detailScopeSubReports[i]);
			}
			return list;
		}

		internal void ResetContextForTopLevelDataSet()
		{
			this.m_dataSetGlobalID = -1;
			this.m_currentSortFilterEventSource = null;
			this.m_runtimeSortFilterInfo = null;
			this.m_currentContainingScope = null;
			this.m_containingScopes = null;
			this.m_inProcessUserSortPhase = null;
		}

		internal void UpdateContextForFirstSubreportInstance(UserSortFilterContext parentContext)
		{
			if (-1 == this.m_dataSetGlobalID)
			{
				this.m_dataSetGlobalID = parentContext.DataSetGlobalId;
				this.m_runtimeSortFilterInfo = parentContext.RuntimeSortFilterInfo;
				this.m_inProcessUserSortPhase = parentContext.m_inProcessUserSortPhase;
			}
		}

		internal static void ProcessEventSources(OnDemandProcessingContext odpContext, IScope containingScope, List<IInScopeEventSource> inScopeEventSources)
		{
			if (inScopeEventSources != null && inScopeEventSources.Count != 0)
			{
				foreach (IInScopeEventSource inScopeEventSource in inScopeEventSources)
				{
					if (inScopeEventSource.UserSort != null)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.SortFilterEventInfo sortFilterEventInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.SortFilterEventInfo(inScopeEventSource);
						sortFilterEventInfo.EventSourceScopeInfo = odpContext.GetScopeValues(inScopeEventSource.ContainingScopes, containingScope);
						if (odpContext.TopLevelContext.NewSortFilterEventInfo == null)
						{
							odpContext.TopLevelContext.NewSortFilterEventInfo = new SortFilterEventInfoMap();
						}
						string text = InstancePathItem.GenerateUniqueNameString(inScopeEventSource.ID, inScopeEventSource.InstancePath);
						odpContext.TopLevelContext.NewSortFilterEventInfo.Add(text, sortFilterEventInfo);
						List<IReference<RuntimeSortFilterEventInfo>> list = odpContext.RuntimeSortFilterInfo;
						if (list == null && odpContext.SubReportUniqueName == null)
						{
							list = odpContext.TopLevelContext.ReportRuntimeUserSortFilterInfo;
						}
						if (list != null)
						{
							for (int i = 0; i < list.Count; i++)
							{
								IReference<RuntimeSortFilterEventInfo> reference = list[i];
								using (reference.PinValue())
								{
									RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
									runtimeSortFilterEventInfo.MatchEventSource(inScopeEventSource, text, containingScope, odpContext);
								}
							}
						}
					}
				}
			}
		}

		internal bool PopulateRuntimeSortFilterEventInfo(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet myDataSet)
		{
			if (odpContext.TopLevelContext.UserSortFilterInfo != null && odpContext.TopLevelContext.UserSortFilterInfo.OdpSortInfo != null && odpContext.TopLevelContext.OldSortFilterEventInfo != null)
			{
				if (-1 != this.m_dataSetGlobalID)
				{
					return false;
				}
				this.m_runtimeSortFilterInfo = null;
				EventInformation.OdpSortEventInfo odpSortInfo = odpContext.TopLevelContext.UserSortFilterInfo.OdpSortInfo;
				for (int i = 0; i < odpSortInfo.Count; i++)
				{
					string uniqueNameAt = odpSortInfo.GetUniqueNameAt(i);
					AspNetCore.ReportingServices.ReportIntermediateFormat.SortFilterEventInfo sortFilterEventInfo = odpContext.TopLevelContext.OldSortFilterEventInfo[uniqueNameAt];
					if (sortFilterEventInfo != null && sortFilterEventInfo.EventSource.UserSort != null)
					{
						int num = sortFilterEventInfo.EventSource.UserSort.SubReportDataSetGlobalId;
						if (-1 == num)
						{
							num = sortFilterEventInfo.EventSource.UserSort.DataSet.GlobalID;
						}
						if (num == myDataSet.GlobalID)
						{
							if (this.m_runtimeSortFilterInfo == null)
							{
								this.m_runtimeSortFilterInfo = new List<IReference<RuntimeSortFilterEventInfo>>();
							}
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = new RuntimeSortFilterEventInfo(sortFilterEventInfo.EventSource, uniqueNameAt, odpSortInfo.GetSortDirectionAt(i), sortFilterEventInfo.EventSourceScopeInfo, odpContext, (this.m_currentContainingScope == null) ? 1 : this.m_currentContainingScope.Value().Depth);
							runtimeSortFilterEventInfo.SelfReference.UnPinValue();
							this.m_runtimeSortFilterInfo.Add(runtimeSortFilterEventInfo.SelfReference);
						}
					}
				}
				if (this.m_runtimeSortFilterInfo != null)
				{
					int count = this.m_runtimeSortFilterInfo.Count;
					for (int j = 0; j < count; j++)
					{
						IReference<RuntimeSortFilterEventInfo> reference = this.m_runtimeSortFilterInfo[j];
						using (reference.PinValue())
						{
							RuntimeSortFilterEventInfo runtimeSortFilterEventInfo2 = reference.Value();
							IInScopeEventSource eventSource = runtimeSortFilterEventInfo2.EventSource;
							AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortExpressionScope = eventSource.UserSort.SortExpressionScope;
							if (sortExpressionScope != null)
							{
								sortExpressionScope.IsSortFilterExpressionScope = this.SetSortFilterInfo(sortExpressionScope.IsSortFilterExpressionScope, count, j);
							}
							AspNetCore.ReportingServices.ReportIntermediateFormat.ISortFilterScope sortTarget = eventSource.UserSort.SortTarget;
							if (sortTarget != null)
							{
								sortTarget.IsSortFilterTarget = this.SetSortFilterInfo(sortTarget.IsSortFilterTarget, count, j);
							}
							if (eventSource.ContainingScopes != null && 0 < eventSource.ContainingScopes.Count)
							{
								int num2 = 0;
								int num3 = 0;
								for (int k = 0; k < eventSource.ContainingScopes.Count; k++)
								{
									AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = eventSource.ContainingScopes[k];
									if (grouping == null || !grouping.IsDetail)
									{
										List<object> list = runtimeSortFilterEventInfo2.SortSourceScopeInfo[num2];
										if (grouping != null)
										{
											if (grouping.SortFilterScopeInfo == null)
											{
												grouping.SortFilterScopeInfo = new List<object>[count];
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
											grouping.SortFilterScopeInfo[j] = list;
											grouping.SortFilterScopeIndex[j] = num2;
										}
										else
										{
											List<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport> detailScopeSubReports = eventSource.UserSort.DetailScopeSubReports;
											AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem = (detailScopeSubReports == null || num3 >= detailScopeSubReports.Count) ? eventSource.Parent : detailScopeSubReports[num3++].Parent;
											while (reportItem != null && !reportItem.IsDataRegion)
											{
												reportItem = reportItem.Parent;
											}
											Global.Tracer.Assert(reportItem.IsDataRegion, "(parent.IsDataRegion)");
											AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)reportItem;
											if (dataRegion.SortFilterSourceDetailScopeInfo == null)
											{
												dataRegion.SortFilterSourceDetailScopeInfo = new int[count];
												for (int n = 0; n < count; n++)
												{
													dataRegion.SortFilterSourceDetailScopeInfo[n] = -1;
												}
											}
											Global.Tracer.Assert(list != null && 1 == list.Count, "(null != scopeValues && 1 == scopeValues.Count)");
											dataRegion.SortFilterSourceDetailScopeInfo[j] = (int)list[0];
										}
										num2++;
									}
								}
							}
							AspNetCore.ReportingServices.ReportIntermediateFormat.GroupingList groupsInSortTarget = eventSource.UserSort.GroupsInSortTarget;
							if (groupsInSortTarget != null)
							{
								for (int num5 = 0; num5 < groupsInSortTarget.Count; num5++)
								{
									groupsInSortTarget[num5].NeedScopeInfoForSortFilterExpression = this.SetSortFilterInfo(groupsInSortTarget[num5].NeedScopeInfoForSortFilterExpression, count, j);
								}
							}
							List<int> peerSortFilters = eventSource.GetPeerSortFilters(false);
							if (peerSortFilters != null && peerSortFilters.Count != 0)
							{
								if (runtimeSortFilterEventInfo2.PeerSortFilters == null)
								{
									runtimeSortFilterEventInfo2.PeerSortFilters = new Hashtable();
								}
								for (int num6 = 0; num6 < peerSortFilters.Count; num6++)
								{
									if (eventSource.ID != peerSortFilters[num6])
									{
										runtimeSortFilterEventInfo2.PeerSortFilters.Add(peerSortFilters[num6], null);
									}
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

		internal bool IsSortFilterTarget(bool[] isSortFilterTarget, IReference<IScope> outerScope, IReference<IHierarchyObj> target, ref RuntimeUserSortTargetInfo userSortTargetInfo)
		{
			bool result = false;
			if (this.m_runtimeSortFilterInfo != null && isSortFilterTarget != null && (outerScope == null || !outerScope.Value().TargetForNonDetailSort))
			{
				for (int i = 0; i < this.m_runtimeSortFilterInfo.Count; i++)
				{
					IReference<RuntimeSortFilterEventInfo> reference = this.m_runtimeSortFilterInfo[i];
					using (reference.PinValue())
					{
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
						if (isSortFilterTarget[i] && (outerScope == null || outerScope.Value().TargetScopeMatched(i, false)))
						{
							runtimeSortFilterEventInfo.EventTarget = target;
							runtimeSortFilterEventInfo.Processed = false;
							if (userSortTargetInfo == null)
							{
								userSortTargetInfo = new RuntimeUserSortTargetInfo(target, i, reference);
							}
							else
							{
								userSortTargetInfo.AddSortInfo(target, i, reference);
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		internal void RegisterSortFilterExpressionScope(IReference<IScope> containerRef, IReference<RuntimeDataRegionObj> scopeRef, bool[] isSortFilterExpressionScope)
		{
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = this.m_runtimeSortFilterInfo;
			if (runtimeSortFilterInfo != null && isSortFilterExpressionScope != null && scopeRef != null)
			{
				List<object>[] array = null;
				using (scopeRef.PinValue())
				{
					RuntimeDataRegionObj runtimeDataRegionObj = scopeRef.Value();
					using (containerRef.PinValue())
					{
						IScope scope = containerRef.Value();
						for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
						{
							if (isSortFilterExpressionScope[i] && runtimeDataRegionObj.IsTargetForSort(i, false) && runtimeDataRegionObj.TargetScopeMatched(i, false))
							{
								IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[i];
								using (reference.PinValue())
								{
									RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
									if (array == null && runtimeSortFilterEventInfo.EventSource.UserSort.GroupsInSortTarget != null)
									{
										int num = 0;
										array = new List<object>[runtimeSortFilterEventInfo.EventSource.UserSort.GroupsInSortTarget.Count];
										runtimeDataRegionObj.GetScopeValues(runtimeSortFilterEventInfo.EventTarget, array, ref num);
									}
									runtimeSortFilterEventInfo.RegisterSortFilterExpressionScope(ref scope.SortFilterExpressionScopeInfoIndices[i], scopeRef, array, i);
								}
							}
						}
					}
				}
			}
		}

		internal bool ProcessUserSort(OnDemandProcessingContext odpContext)
		{
			bool result = false;
			bool flag = false;
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = this.m_runtimeSortFilterInfo;
			if (runtimeSortFilterInfo != null)
			{
				while (true)
				{
					bool flag2 = flag;
					flag = false;
					bool flag3 = true;
					this.ProcessUserSort(odpContext, ref flag, ref flag3, ref result);
					if (!flag)
					{
						if (flag3)
						{
							break;
						}
						if (!flag2)
						{
							break;
						}
					}
				}
			}
			return result;
		}

		private void ProcessUserSort(OnDemandProcessingContext odpContext, ref bool processed, ref bool canStop, ref bool processedAny)
		{
			List<IReference<RuntimeSortFilterEventInfo>> runtimeSortFilterInfo = this.m_runtimeSortFilterInfo;
			for (int i = 0; i < runtimeSortFilterInfo.Count; i++)
			{
				IReference<RuntimeSortFilterEventInfo> reference = runtimeSortFilterInfo[i];
				using (reference.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
					if (!runtimeSortFilterEventInfo.Processed)
					{
						runtimeSortFilterEventInfo.PrepareForSorting(odpContext);
					}
				}
			}
			for (int j = 0; j < runtimeSortFilterInfo.Count; j++)
			{
				IReference<RuntimeSortFilterEventInfo> reference2 = runtimeSortFilterInfo[j];
				using (reference2.PinValue())
				{
					RuntimeSortFilterEventInfo runtimeSortFilterEventInfo2 = reference2.Value();
					if (!runtimeSortFilterEventInfo2.Processed)
					{
						if (runtimeSortFilterEventInfo2.ProcessSorting(odpContext))
						{
							processedAny = true;
							processed = true;
							odpContext.FirstPassPostProcess();
							return;
						}
						canStop = false;
					}
				}
			}
		}

		internal void ProcessUserSortForTarget(ObjectModelImpl reportObjectModel, AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRuntime, IReference<IHierarchyObj> target, ref ScalableList<DataFieldRow> dataRows, bool targetForNonDetailSort)
		{
			using (target.PinValue())
			{
				IHierarchyObj hierarchyObj = target.Value();
				if (targetForNonDetailSort && dataRows != null && 0 < dataRows.Count)
				{
					IReference<RuntimeSortFilterEventInfo> reference = null;
					try
					{
						List<int> sortFilterInfoIndices = hierarchyObj.SortFilterInfoIndices;
						Global.Tracer.Assert(null != hierarchyObj.SortTree, "(null != targetObj.SortTree)");
						if (sortFilterInfoIndices != null)
						{
							reference = this.m_runtimeSortFilterInfo[sortFilterInfoIndices[0]];
						}
						RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = null;
						if (reference != null)
						{
							reference.PinValue();
							runtimeSortFilterEventInfo = reference.Value();
						}
						for (int i = 0; i < dataRows.Count; i++)
						{
							dataRows[i].SetFields(reportObjectModel.FieldsImpl);
							object keyValue = DBNull.Value;
							if (runtimeSortFilterEventInfo != null)
							{
								keyValue = runtimeSortFilterEventInfo.GetSortOrder(reportRuntime);
							}
							hierarchyObj.SortTree.NextRow(keyValue, hierarchyObj);
						}
					}
					finally
					{
						if (reference != null)
						{
							reference.UnPinValue();
						}
					}
					if (dataRows != null)
					{
						dataRows.Dispose();
					}
					dataRows = null;
				}
				hierarchyObj.MarkSortInfoProcessed(this.m_runtimeSortFilterInfo);
			}
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
	}
}
