using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandStateManagerStreaming : OnDemandStateManager
	{
		private class DataPipelineThrottle
		{
			private bool m_stoppingScopeInstanceCreated;

			private bool m_anyScopeInstanceCreated;

			private PipelineAdvanceMode m_pipelineMode;

			private IRIFReportDataScope m_targetScopeForDataProcessing;

			private bool m_inUse;

			private bool m_metStoppingCondition;

			public bool InUse
			{
				get
				{
					return this.m_inUse;
				}
			}

			internal bool ShouldStopPipelineAdvance(bool rowAccepted)
			{
				switch (this.m_pipelineMode)
				{
				case PipelineAdvanceMode.ByOneRow:
					this.m_metStoppingCondition = rowAccepted;
					break;
				case PipelineAdvanceMode.ToStoppingScopeInstance:
					this.m_metStoppingCondition = (rowAccepted && this.m_stoppingScopeInstanceCreated);
					break;
				case PipelineAdvanceMode.ToFulfillServerAggregate:
					this.m_metStoppingCondition = (this.m_stoppingScopeInstanceCreated || !OnDemandStateManagerStreaming.NeedsDataForServerAggregate(this.m_targetScopeForDataProcessing));
					break;
				default:
					Global.Tracer.Assert(false, "Unknown pipeline mode: {0}", this.m_pipelineMode);
					throw new InvalidOperationException();
				}
				return this.m_metStoppingCondition;
			}

			internal void CreatedScopeInstance(IRIFReportDataScope scope)
			{
				this.m_anyScopeInstanceCreated = true;
				if (OnDemandStateManagerStreaming.CanBindOrProcessIndividually(scope) && this.IsTargetScopeForDataProcessing(scope))
				{
					this.m_stoppingScopeInstanceCreated = true;
				}
			}

			private bool IsTargetScopeForDataProcessing(IRIFReportDataScope candidateScope)
			{
				return this.m_targetScopeForDataProcessing.IsSameOrChildScopeOf(candidateScope);
			}

			public void StartUsingContext(PipelineAdvanceMode mode, IRIFReportDataScope targetScope)
			{
				this.m_inUse = true;
				this.m_metStoppingCondition = false;
				this.m_anyScopeInstanceCreated = false;
				this.m_stoppingScopeInstanceCreated = false;
				this.m_pipelineMode = mode;
				this.m_targetScopeForDataProcessing = targetScope;
			}

			public bool StopUsingContext()
			{
				this.m_inUse = false;
				if (this.m_pipelineMode == PipelineAdvanceMode.ToStoppingScopeInstance)
				{
					return this.m_anyScopeInstanceCreated;
				}
				return this.m_metStoppingCondition;
			}
		}

		private enum PipelineAdvanceMode
		{
			ToStoppingScopeInstance,
			ByOneRow,
			ToFulfillServerAggregate
		}

		private IReportScopeInstance m_lastROMInstance;

		private IReference<IOnDemandScopeInstance> m_lastOnDemandScopeInstance;

		private IRIFReportDataScope m_lastRIFObject;

		private DataPipelineManager m_pipelineManager;

		private QueryRestartInfo m_queryRestartInfo;

		private ExecutedQueryCache m_executedQueryCache;

		private EventHandler m_abortProcessor;

		private DataPipelineThrottle m_pipelineThrottle;

		private DataPipelineThrottle m_pipelineThrottle2;

		internal override IReportScopeInstance LastROMInstance
		{
			get
			{
				return this.m_lastROMInstance;
			}
		}

		internal override IRIFReportScope LastTablixProcessingReportScope
		{
			get
			{
				return this.m_lastRIFObject;
			}
			set
			{
				Global.Tracer.Assert(false, "Set LastTablixProcessingReportScope not supported in this execution mode");
				throw new NotImplementedException();
			}
		}

		internal override IInstancePath LastRIFObject
		{
			get
			{
				return this.m_lastRIFObject;
			}
			set
			{
				Global.Tracer.Assert(false, "Set LastRIFObject not supported in this execution mode");
				throw new NotImplementedException();
			}
		}

		internal override QueryRestartInfo QueryRestartInfo
		{
			get
			{
				return this.m_queryRestartInfo;
			}
		}

		internal override ExecutedQueryCache ExecutedQueryCache
		{
			get
			{
				return this.m_executedQueryCache;
			}
		}

		public OnDemandStateManagerStreaming(OnDemandProcessingContext odpContext)
			: base(odpContext)
		{
			this.m_queryRestartInfo = new QueryRestartInfo();
			if (base.m_odpContext.AbortInfo != null)
			{
				this.m_abortProcessor = this.AbortHandler;
				base.m_odpContext.AbortInfo.ProcessingAbortEvent += this.m_abortProcessor;
			}
		}

		internal override ExecutedQueryCache SetupExecutedQueryCache()
		{
			Global.Tracer.Assert(this.m_executedQueryCache == null, "Cannot SetupExecutedQueryCache twice");
			this.m_executedQueryCache = new ExecutedQueryCache();
			return this.ExecutedQueryCache;
		}

		internal override void ResetOnDemandState()
		{
		}

		internal override int RecursiveLevel(string scopeName)
		{
			Global.Tracer.Assert(false, "The Level function is not supported in this execution mode.");
			throw new NotImplementedException();
		}

		internal override bool InScope(string scopeName)
		{
			Global.Tracer.Assert(false, "The InScope function is not supported in this execution mode.");
			throw new NotImplementedException();
		}

		internal override Dictionary<string, object> GetCurrentSpecialGroupingValues()
		{
			Global.Tracer.Assert(false, "The CreateDrillthroughContext function is not supported in this execution mode.");
			throw new NotImplementedException();
		}

		internal override bool CalculateAggregate(string aggregateName)
		{
			Global.Tracer.Assert(!base.m_odpContext.IsPageHeaderFooter, "Not supported for page header/footer in streaming mode");
			OnDemandProcessingContext odpWorkerContextForTablixProcessing = base.GetOdpWorkerContextForTablixProcessing();
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo);
			odpWorkerContextForTablixProcessing.ReportAggregates.TryGetValue(aggregateName, out dataAggregateInfo);
			if (dataAggregateInfo == null)
			{
				return false;
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = base.m_odpContext.ReportDefinition.MappingDataSetIndexToDataSet[dataAggregateInfo.DataSetIndexInCollection];
			if (!odpWorkerContextForTablixProcessing.IsTablixProcessingComplete(dataSet.IndexInCollection))
			{
				if (odpWorkerContextForTablixProcessing.IsTablixProcessingMode)
				{
					return false;
				}
				DataSetAggregateDataPipelineManager pipeline = new DataSetAggregateDataPipelineManager(odpWorkerContextForTablixProcessing, dataSet);
				odpWorkerContextForTablixProcessing.OnDemandProcessDataPipelineWithRestore(pipeline);
			}
			return true;
		}

		internal override bool CalculateLookup(LookupInfo lookup)
		{
			Global.Tracer.Assert(false, "Lookup functions are not supported in this execution mode.");
			throw new NotImplementedException();
		}

		internal override bool PrepareFieldsCollectionForDirectFields()
		{
			Global.Tracer.Assert(false, "The fields collection should already be setup for Streaming ODP Mode");
			throw new NotImplementedException();
		}

		internal override void EvaluateScopedFieldReference(string scopeName, int fieldIndex, ref AspNetCore.ReportingServices.RdlExpressions.VariantResult result)
		{
			Global.Tracer.Assert(this.m_lastRIFObject != null, "The RIF object for the current scope should be present.");
			try
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet = default(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet);
				if (!base.m_odpContext.ReportDefinition.MappingNameToDataSet.TryGetValue(scopeName, out targetDataSet))
				{
					throw new ReportProcessingException_NonExistingScopeReference(scopeName);
				}
				NonStructuralIdcDataManager nonStructuralIdcDataManager = default(NonStructuralIdcDataManager);
				if (!base.TryGetNonStructuralIdcDataManager(targetDataSet, out nonStructuralIdcDataManager))
				{
					nonStructuralIdcDataManager = this.CreateNonStructuralIdcDataManager(scopeName, targetDataSet);
				}
				if (nonStructuralIdcDataManager.SourceDataScope.CurrentStreamingScopeInstance != nonStructuralIdcDataManager.LastParentScopeInstance)
				{
					nonStructuralIdcDataManager.RegisterActiveParent(nonStructuralIdcDataManager.SourceDataScope.CurrentStreamingScopeInstance);
					nonStructuralIdcDataManager.Advance();
				}
				else
				{
					nonStructuralIdcDataManager.SetupEnvironment();
				}
				base.m_odpContext.ReportRuntime.EvaluateSimpleFieldReference(fieldIndex, ref result);
			}
			finally
			{
				this.SetupEnvironment(this.m_lastRIFObject, this.m_lastOnDemandScopeInstance.Value(), this.m_lastOnDemandScopeInstance);
			}
		}

		private NonStructuralIdcDataManager CreateNonStructuralIdcDataManager(string scopeName, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet)
		{
			IRIFReportDataScope sourceDataScope = default(IRIFReportDataScope);
			if (!DataScopeInfo.TryGetInnermostParentScopeRelatedToTargetDataSet(targetDataSet, this.m_lastRIFObject, out sourceDataScope))
			{
				throw new ReportProcessingException_InvalidScopeReference(scopeName);
			}
			NonStructuralIdcDataManager nonStructuralIdcDataManager = new NonStructuralIdcDataManager(base.m_odpContext, targetDataSet, sourceDataScope);
			base.RegisterDisposableDataReaderOrIdcDataManager(nonStructuralIdcDataManager);
			base.AddNonStructuralIdcDataManager(targetDataSet, nonStructuralIdcDataManager);
			return nonStructuralIdcDataManager;
		}

		internal override void RestoreContext(IInstancePath originalObject)
		{
			if (originalObject != null && base.m_odpContext.ReportRuntime.ContextUpdated && this.m_lastRIFObject != originalObject)
			{
				this.SetupObjectModels((IRIFReportDataScope)originalObject, -1);
			}
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance)
		{
			this.SetupContext(rifObject, romInstance, -1);
		}

		internal override void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			this.m_lastROMInstance = romInstance;
			IRIFReportDataScope iRIFReportDataScope = romInstance.ReportScope.RIFReportScope as IRIFReportDataScope;
			if (iRIFReportDataScope != null)
			{
				if (this.m_lastOnDemandScopeInstance != null && iRIFReportDataScope.CurrentStreamingScopeInstance == this.m_lastOnDemandScopeInstance)
				{
					return;
				}
				this.SetupObjectModels(iRIFReportDataScope, moveNextInstanceIndex);
			}
		}

		private void SetupObjectModels(IRIFReportDataScope reportDataScope, int moveNextInstanceIndex)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDefinition = base.m_odpContext.ReportDefinition;
			base.m_odpContext.EnsureCultureIsSetOnCurrentThread();
			this.EnsureScopeIsBound(reportDataScope);
			if (this.m_lastOnDemandScopeInstance != reportDataScope.CurrentStreamingScopeInstance)
			{
				this.SetupEnvironment(reportDataScope, reportDataScope.CurrentStreamingScopeInstance.Value(), reportDataScope.CurrentStreamingScopeInstance);
			}
		}

		private void EnsureScopeIsBound(IRIFReportDataScope reportDataScope)
		{
			this.BindScopeToInstance(reportDataScope);
			if (!reportDataScope.IsBoundToStreamingScopeInstance && OnDemandStateManagerStreaming.CanBindOrProcessIndividually(reportDataScope) && this.TryProcessToNextScopeInstance(reportDataScope))
			{
				this.BindScopeToInstance(reportDataScope);
			}
			if (!reportDataScope.IsBoundToStreamingScopeInstance)
			{
				reportDataScope.BindToNoRowsScopeInstance(base.m_odpContext);
			}
		}

		private void SetupEnvironment(IRIFReportDataScope reportDataScope, IOnDemandScopeInstance scopeInst, IReference<IOnDemandScopeInstance> scopeInstRef)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = reportDataScope.DataScopeInfo.DataSet;
			if (base.m_odpContext.CurrentDataSetIndex != dataSet.IndexInCollection)
			{
				base.m_odpContext.SetupFieldsForNewDataSet(dataSet, base.m_odpContext.GetDataSetInstance(dataSet), true, false);
			}
			scopeInst.SetupEnvironment();
			this.m_lastOnDemandScopeInstance = scopeInstRef;
			this.m_lastRIFObject = reportDataScope;
		}

		private void BindScopeToInstance(IRIFReportDataScope reportDataScope)
		{
			if (!reportDataScope.IsBoundToStreamingScopeInstance)
			{
				if (!reportDataScope.IsScope)
				{
					IRIFReportDataScope parentReportScope = reportDataScope.ParentReportScope;
					this.EnsureScopeIsBound(parentReportScope);
					reportDataScope.BindToStreamingScopeInstance(parentReportScope.CurrentStreamingScopeInstance);
				}
				else
				{
					switch (reportDataScope.InstancePathItem.Type)
					{
					case InstancePathItemType.Cell:
						if (reportDataScope.IsDataIntersectionScope)
						{
							IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)reportDataScope;
							IRIFReportDataScope parentRowReportScope = iRIFReportIntersectionScope.ParentRowReportScope;
							IReference<IOnDemandMemberInstance> reference3 = default(IReference<IOnDemandMemberInstance>);
							if (this.TryBindParentScope<IOnDemandMemberInstance>(reportDataScope, parentRowReportScope, out reference3))
							{
								IRIFReportDataScope parentColumnReportScope = iRIFReportIntersectionScope.ParentColumnReportScope;
								IReference<IOnDemandMemberInstance> reference4 = default(IReference<IOnDemandMemberInstance>);
								if (this.TryBindParentScope<IOnDemandMemberInstance>(reportDataScope, parentColumnReportScope, out reference4))
								{
									IReference<IOnDemandMemberInstance> reference5;
									IReference<IOnDemandMemberInstance> reference6;
									if (!iRIFReportIntersectionScope.IsColumnOuterGrouping)
									{
										reference5 = reference3;
										reference6 = reference4;
									}
									else
									{
										reference5 = reference4;
										reference6 = reference3;
									}
									this.CheckForPrematureScopeInstance(reportDataScope);
									IReference<IOnDemandScopeInstance> reference7 = default(IReference<IOnDemandScopeInstance>);
									IOnDemandScopeInstance cellInstance = SyntheticTriangulatedCellReference.GetCellInstance(reference5, reference6, out reference7);
									if (cellInstance == null && iRIFReportIntersectionScope.DataScopeInfo.NeedsIDC && this.TryProcessToCreateCell(iRIFReportIntersectionScope, (RuntimeDataTablixGroupLeafObjReference)reference6, (RuntimeDataTablixGroupLeafObjReference)reference5))
									{
										cellInstance = SyntheticTriangulatedCellReference.GetCellInstance(reference5, reference6, out reference7);
									}
									if (cellInstance != null)
									{
										if (reference7 == null)
										{
											iRIFReportIntersectionScope.BindToStreamingScopeInstance(reference5, reference6);
											this.SetupEnvironment(reportDataScope, cellInstance, iRIFReportIntersectionScope.CurrentStreamingScopeInstance);
										}
										else
										{
											reportDataScope.BindToStreamingScopeInstance(reference7);
										}
									}
								}
							}
						}
						else
						{
							Global.Tracer.Assert(false, "Non-intersection cell scopes are not yet supported by streaming ODP.");
						}
						break;
					case InstancePathItemType.ColumnMemberInstanceIndexTopMost:
					case InstancePathItemType.ColumnMemberInstanceIndex:
					case InstancePathItemType.RowMemberInstanceIndex:
					{
						IRIFReportDataScope parentReportScope3 = reportDataScope.ParentReportScope;
						IReference<IOnDemandMemberOwnerInstance> reference2 = default(IReference<IOnDemandMemberOwnerInstance>);
						if (this.TryBindParentScope<IOnDemandMemberOwnerInstance>(reportDataScope, parentReportScope3, out reference2))
						{
							this.CheckForPrematureScopeInstance(reportDataScope);
							using (reference2.PinValue())
							{
								IOnDemandMemberOwnerInstance onDemandMemberOwnerInstance = reference2.Value();
								AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode rifMember = (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)reportDataScope;
								IOnDemandMemberInstanceReference firstMemberInstance = onDemandMemberOwnerInstance.GetFirstMemberInstance(rifMember);
								if (this.RequiresIdcProcessing(reportDataScope, firstMemberInstance, (IReference<IOnDemandScopeInstance>)reference2))
								{
									firstMemberInstance = onDemandMemberOwnerInstance.GetFirstMemberInstance(rifMember);
								}
								reportDataScope.BindToStreamingScopeInstance(firstMemberInstance);
							}
						}
						break;
					}
					case InstancePathItemType.DataRegion:
					{
						IRIFReportDataScope parentReportScope2 = reportDataScope.ParentReportScope;
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)reportDataScope;
						IReference<IOnDemandScopeInstance> reference = default(IReference<IOnDemandScopeInstance>);
						if (parentReportScope2 == null)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = dataRegion.DataScopeInfo.DataSet;
							DataPipelineManager orCreatePipelineManager = this.GetOrCreatePipelineManager(dataSet, dataRegion);
							reportDataScope.BindToStreamingScopeInstance(orCreatePipelineManager.GroupTreeRoot.GetDataRegionInstance(dataRegion));
						}
						else if (this.TryBindParentScope<IOnDemandScopeInstance>(reportDataScope, parentReportScope2, out reference))
						{
							this.CheckForPrematureScopeInstance(reportDataScope);
							using (reference.PinValue())
							{
								IOnDemandScopeInstance onDemandScopeInstance = reference.Value();
								IReference<IOnDemandScopeInstance> dataRegionInstance = onDemandScopeInstance.GetDataRegionInstance(dataRegion);
								if (this.RequiresIdcProcessing(reportDataScope, dataRegionInstance, reference))
								{
									dataRegionInstance = onDemandScopeInstance.GetDataRegionInstance(dataRegion);
								}
								reportDataScope.BindToStreamingScopeInstance(dataRegionInstance);
							}
						}
						break;
					}
					default:
						Global.Tracer.Assert(false, "SetupObjectModels cannot handle IRIFReportDataScope of type: {0}", Enum.GetName(typeof(InstancePathItemType), reportDataScope.InstancePathItem.Type));
						break;
					}
				}
			}
		}

		private bool RequiresIdcProcessing(IRIFReportDataScope reportDataScope, IReference<IOnDemandScopeInstance> scopeInstanceRef, IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			if (reportDataScope.DataScopeInfo.NeedsIDC)
			{
				if (scopeInstanceRef == null)
				{
					this.RegisterParentForIdc(reportDataScope, parentScopeInstanceRef);
					return this.TryProcessToNextScopeInstance(reportDataScope);
				}
				IOnDemandScopeInstance onDemandScopeInstance = scopeInstanceRef.Value();
				if (onDemandScopeInstance.IsNoRows && onDemandScopeInstance.IsMostRecentlyCreatedScopeInstance)
				{
					this.RegisterParentForIdc(reportDataScope, parentScopeInstanceRef);
					return this.ProcessOneRow(reportDataScope);
				}
			}
			return false;
		}

		private void RegisterParentForIdc(IRIFReportDataScope reportDataScope, IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			IdcDataManager idcDataManager = (IdcDataManager)base.GetOrCreateIdcDataManager(reportDataScope);
			idcDataManager.RegisterActiveParent(parentScopeInstanceRef);
		}

		internal override bool CheckForPrematureServerAggregate(string aggregateName)
		{
			IRIFReportDataScope iRIFReportDataScope = this.m_lastRIFObject;
			while (iRIFReportDataScope != null && !iRIFReportDataScope.IsScope)
			{
				iRIFReportDataScope = iRIFReportDataScope.ParentReportScope;
			}
			if (iRIFReportDataScope != null && iRIFReportDataScope.IsBoundToStreamingScopeInstance)
			{
				if (OnDemandStateManagerStreaming.NeedsDataForServerAggregate(iRIFReportDataScope))
				{
					this.AdvanceDataPipeline(iRIFReportDataScope, PipelineAdvanceMode.ToFulfillServerAggregate);
					this.SetupEnvironment(iRIFReportDataScope, iRIFReportDataScope.CurrentStreamingScopeInstance.Value(), iRIFReportDataScope.CurrentStreamingScopeInstance);
					return true;
				}
				return false;
			}
			return false;
		}

		internal static bool NeedsDataForServerAggregate(IRIFReportDataScope reportDataScope)
		{
			IOnDemandScopeInstance onDemandScopeInstance = reportDataScope.CurrentStreamingScopeInstance.Value();
			if (!onDemandScopeInstance.IsNoRows && onDemandScopeInstance.IsMostRecentlyCreatedScopeInstance)
			{
				return onDemandScopeInstance.HasUnProcessedServerAggregate;
			}
			return false;
		}

		private void CheckForPrematureScopeInstance(IRIFReportDataScope reportDataScope)
		{
			if (!OnDemandStateManagerStreaming.CanBindOrProcessIndividually(reportDataScope) && !reportDataScope.DataScopeInfo.NeedsIDC && !reportDataScope.IsDataIntersectionScope && !DataScopeInfo.HasDecomposableAncestorWithNonLatestInstanceBinding(reportDataScope))
			{
				this.TryProcessToNextScopeInstance(reportDataScope);
			}
		}

		private bool TryBindParentScope<T>(IRIFReportDataScope reportDataScope, IRIFReportDataScope parentReportDataScope, out IReference<T> parentScopeInst) where T : IOnDemandScopeInstance
		{
			this.EnsureScopeIsBound(parentReportDataScope);
			parentScopeInst = (IReference<T>)parentReportDataScope.CurrentStreamingScopeInstance;
			if (parentScopeInst.Value().IsNoRows)
			{
				reportDataScope.BindToNoRowsScopeInstance(base.m_odpContext);
				return false;
			}
			return true;
		}

		private DataPipelineManager GetOrCreatePipelineManager(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, IRIFReportDataScope targetScope)
		{
			if (this.m_pipelineManager != null)
			{
				if (this.m_pipelineManager.DataSetIndex == dataSet.IndexInCollection)
				{
					return this.m_pipelineManager;
				}
				if (base.m_odpContext.IsTablixProcessingComplete(dataSet.IndexInCollection))
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Performance: While rendering the report: '{0}' the data set {1} was processed multiple times due to rendering traversal order.", base.m_odpContext.ReportContext.ItemPathAsString.MarkAsPrivate(), dataSet.Name.MarkAsPrivate());
				}
				this.CleanupPipelineManager();
				base.ShutdownSequentialReadersAndIdcDataManagers();
			}
			if (dataSet.AllowIncrementalProcessing)
			{
				this.m_pipelineManager = new IncrementalDataPipelineManager(base.m_odpContext, dataSet);
			}
			else
			{
				this.m_pipelineManager = new StreamingAtomicDataPipelineManager(base.m_odpContext, dataSet);
			}
			this.m_pipelineThrottle = new DataPipelineThrottle();
			this.m_pipelineThrottle.StartUsingContext(PipelineAdvanceMode.ToStoppingScopeInstance, targetScope);
			this.m_pipelineManager.StartProcessing();
			this.m_pipelineThrottle.StopUsingContext();
			this.TryProcessToNextScopeInstance(targetScope);
			return this.m_pipelineManager;
		}

		private void CleanupPipelineManager()
		{
			if (this.m_pipelineManager != null)
			{
				this.m_pipelineManager.StopProcessing();
				this.m_pipelineManager = null;
			}
		}

		internal override IRecordRowReader CreateSequentialDataReader(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance)
		{
			LiveRecordRowReader liveRecordRowReader = new LiveRecordRowReader(dataSet, base.m_odpContext);
			dataSetInstance = liveRecordRowReader.DataSetInstance;
			base.RegisterDisposableDataReaderOrIdcDataManager(liveRecordRowReader);
			return liveRecordRowReader;
		}

		private bool TryProcessToCreateCell(IRIFReportDataScope reportDataScope, RuntimeDataTablixGroupLeafObjReference columnGroupLeafRef, RuntimeDataTablixGroupLeafObjReference rowGroupLeafRef)
		{
			CellIdcDataManager cellIdcDataManager = (CellIdcDataManager)base.GetOrCreateIdcDataManager(reportDataScope);
			cellIdcDataManager.RegisterActiveIntersection(columnGroupLeafRef, rowGroupLeafRef);
			return this.TryProcessToNextScopeInstance(reportDataScope);
		}

		private bool TryProcessToNextScopeInstance(IRIFReportDataScope reportDataScope)
		{
			return this.AdvanceDataPipeline(reportDataScope, PipelineAdvanceMode.ToStoppingScopeInstance);
		}

		private bool AdvanceDataPipeline(IRIFReportDataScope reportDataScope, PipelineAdvanceMode pipelineMode)
		{
			this.m_lastOnDemandScopeInstance = null;
			DataPipelineThrottle pipelineThrottle = this.SetupUsablePipelineContextWithBackup();
			this.m_pipelineThrottle.StartUsingContext(pipelineMode, reportDataScope);
			bool isTablixProcessingMode = base.m_odpContext.IsTablixProcessingMode;
			bool flag = base.m_odpContext.ExecutionLogContext.TryStartTablixProcessingTimer();
			base.m_odpContext.IsTablixProcessingMode = true;
			if (reportDataScope.DataScopeInfo.DataPipelineID != this.m_pipelineManager.DataSetIndex)
			{
				BaseIdcDataManager idcDataManager = base.m_odpContext.StateManager.GetIdcDataManager(reportDataScope);
				idcDataManager.Advance();
			}
			else
			{
				this.m_pipelineManager.Advance();
			}
			base.m_odpContext.IsTablixProcessingMode = isTablixProcessingMode;
			if (flag)
			{
				base.m_odpContext.ExecutionLogContext.StopTablixProcessingTimer();
			}
			bool result = this.m_pipelineThrottle.StopUsingContext();
			this.m_pipelineThrottle = pipelineThrottle;
			return result;
		}

		private DataPipelineThrottle SetupUsablePipelineContextWithBackup()
		{
			if (this.m_pipelineThrottle == null)
			{
				this.m_pipelineThrottle = new DataPipelineThrottle();
			}
			DataPipelineThrottle pipelineThrottle = this.m_pipelineThrottle;
			if (this.m_pipelineThrottle.InUse)
			{
				if (this.m_pipelineThrottle2 == null)
				{
					this.m_pipelineThrottle2 = new DataPipelineThrottle();
				}
				if (this.m_pipelineThrottle2.InUse)
				{
					this.m_pipelineThrottle = new DataPipelineThrottle();
				}
				else
				{
					this.m_pipelineThrottle = this.m_pipelineThrottle2;
				}
			}
			return pipelineThrottle;
		}

		internal override bool ProcessOneRow(IRIFReportDataScope scope)
		{
			return this.AdvanceDataPipeline(scope, PipelineAdvanceMode.ByOneRow);
		}

		private void AbortHandler(object sender, EventArgs e)
		{
			if (this.m_pipelineManager != null)
			{
				this.m_pipelineManager.Abort();
			}
		}

		internal override void FreeResources()
		{
			if (this.m_abortProcessor != null)
			{
				base.m_odpContext.AbortInfo.ProcessingAbortEvent -= this.m_abortProcessor;
				this.m_abortProcessor = null;
			}
			base.FreeResources();
			this.CleanupPipelineManager();
			this.CleanupQueryCache();
		}

		private void CleanupQueryCache()
		{
			if (this.m_executedQueryCache != null)
			{
				this.m_executedQueryCache.Close();
			}
		}

		internal override void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			IRIFReportDataScope iRIFReportDataScope = romInstance.ReportScope.RIFReportScope as IRIFReportDataScope;
			IReference<IOnDemandMemberInstance> reference = iRIFReportDataScope.CurrentStreamingScopeInstance as IReference<IOnDemandMemberInstance>;
			if (!reference.Value().IsNoRows)
			{
				IDisposable disposable = reference.PinValue();
				IOnDemandMemberInstance onDemandMemberInstance = reference.Value();
				iRIFReportDataScope.BindToStreamingScopeInstance(onDemandMemberInstance.GetNextMemberInstance());
				if (!iRIFReportDataScope.IsBoundToStreamingScopeInstance && OnDemandStateManagerStreaming.CanBindOrProcessIndividually(iRIFReportDataScope) && onDemandMemberInstance.IsMostRecentlyCreatedScopeInstance)
				{
					IdcDataManager idcDataManager = null;
					if (iRIFReportDataScope.DataScopeInfo.NeedsIDC)
					{
						idcDataManager = (IdcDataManager)base.GetIdcDataManager(iRIFReportDataScope);
						List<object> groupExprValues = onDemandMemberInstance.GroupExprValues;
						AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode)iRIFReportDataScope;
						List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> groupExpressions = reportHierarchyNode.Grouping.GroupExpressions;
						idcDataManager.SetSkippingFilter(groupExpressions, groupExprValues);
					}
					if (this.TryProcessToNextScopeInstance(iRIFReportDataScope))
					{
						iRIFReportDataScope.BindToStreamingScopeInstance(onDemandMemberInstance.GetNextMemberInstance());
					}
					if (idcDataManager != null)
					{
						idcDataManager.ClearSkippingFilter();
					}
				}
				if (!iRIFReportDataScope.IsBoundToStreamingScopeInstance)
				{
					iRIFReportDataScope.BindToNoRowsScopeInstance(base.m_odpContext);
				}
				this.SetupEnvironment(iRIFReportDataScope, iRIFReportDataScope.CurrentStreamingScopeInstance.Value(), iRIFReportDataScope.CurrentStreamingScopeInstance);
				disposable.Dispose();
			}
		}

		internal override bool ShouldStopPipelineAdvance(bool rowAccepted)
		{
			return this.m_pipelineThrottle.ShouldStopPipelineAdvance(rowAccepted);
		}

		internal override void CreatedScopeInstance(IRIFReportDataScope scope)
		{
			this.m_pipelineThrottle.CreatedScopeInstance(scope);
		}

		internal static bool CanBindOrProcessIndividually(IRIFReportDataScope scope)
		{
			return scope.DataScopeInfo.IsDecomposable;
		}
	}
}
