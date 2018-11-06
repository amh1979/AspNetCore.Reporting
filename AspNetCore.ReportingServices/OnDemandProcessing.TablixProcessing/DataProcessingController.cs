using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class DataProcessingController
	{
		private readonly OnDemandProcessingContext m_odpContext;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

		private readonly DataSetInstance m_dataSetInstance;

		private RuntimeOnDemandDataSetObj m_dataSetObj;

		private bool m_hasSortFilterInfo;

		public IOnDemandScopeInstance GroupTreeRoot
		{
			get
			{
				return this.m_dataSetObj;
			}
		}

		public DataProcessingController(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance)
		{
			this.m_odpContext = odpContext;
			this.m_dataSet = dataSet;
			this.m_dataSetInstance = dataSetInstance;
			this.m_report = odpContext.ReportDefinition;
			this.m_odpContext.EnsureScalabilitySetup();
			UserSortFilterContext userSortFilterContext = this.m_odpContext.UserSortFilterContext;
			if (!this.m_odpContext.InSubreportInDataRegion)
			{
				userSortFilterContext.ResetContextForTopLevelDataSet();
			}
			this.m_hasSortFilterInfo = this.m_odpContext.PopulateRuntimeSortFilterEventInfo(this.m_dataSet);
			if (-1 == userSortFilterContext.DataSetGlobalId)
			{
				userSortFilterContext.DataSetGlobalId = this.m_dataSet.GlobalID;
			}
			Global.Tracer.Assert(this.m_odpContext.ReportObjectModel != null && this.m_odpContext.ReportRuntime != null);
			this.m_odpContext.SetupFieldsForNewDataSet(this.m_dataSet, this.m_dataSetInstance, true, true);
			this.m_dataSet.SetFilterExprHost(this.m_odpContext.ReportObjectModel);
			this.m_dataSetObj = new RuntimeOnDemandDataSetObj(this.m_odpContext, this.m_dataSet, this.m_dataSetInstance);
		}

		public void InitializeDataProcessing()
		{
			this.m_dataSetObj.Initialize();
			Global.Tracer.Assert(this.m_odpContext.CurrentDataSetIndex == this.m_dataSet.IndexInCollection, "(m_odpContext.CurrentDataSetIndex == m_dataSet.IndexInCollection)");
		}

		public void TeardownDataProcessing()
		{
			this.m_dataSetObj.Teardown();
			this.m_odpContext.EnsureScalabilityCleanup();
		}

		public void NextRow(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow row, int rowNumber, bool useRowOffset, bool readerExtensionsSupported)
		{
			FieldsImpl fieldsImplForUpdate = this.m_odpContext.ReportObjectModel.GetFieldsImplForUpdate(this.m_dataSet);
			if (useRowOffset)
			{
				fieldsImplForUpdate.NewRow();
			}
			else
			{
				fieldsImplForUpdate.NewRow(row.StreamPosition);
			}
			if (fieldsImplForUpdate.AddRowIndex)
			{
				fieldsImplForUpdate.SetRowIndex(rowNumber);
			}
			this.m_odpContext.ReportObjectModel.UpdateFieldValues(false, row, this.m_dataSetInstance, readerExtensionsSupported);
			this.m_dataSetObj.NextRow();
		}

		public void AllRowsRead()
		{
			this.m_dataSetObj.FinishReadingRows();
			this.m_odpContext.FirstPassPostProcess();
			Global.Tracer.Trace(TraceLevel.Verbose, "TablixProcessing: FirstPass Complete");
			if (this.m_report.HasAggregatesOfAggregatesInUserSort && this.m_odpContext.RuntimeSortFilterInfo != null && this.m_odpContext.RuntimeSortFilterInfo.Count > 0)
			{
				this.PreComputeAggregatesOfAggregates();
			}
			this.m_odpContext.ApplyUserSorts();
			Global.Tracer.Trace(TraceLevel.Verbose, "TablixProcessing: ApplyUserSorts Complete");
			this.SortAndFilter();
			Global.Tracer.Trace(TraceLevel.Verbose, "TablixProcessing: SortAndFilter Complete");
			this.m_odpContext.CheckAndThrowIfAborted();
			this.PostSortOperations();
			Global.Tracer.Trace(TraceLevel.Verbose, "TablixProcessing: PostSortOperations Complete");
			this.StoreDataSetLevelAggregates();
		}

		private void PreComputeAggregatesOfAggregates()
		{
			if (this.m_report.NeedPostGroupProcessing)
			{
				this.m_odpContext.SecondPassOperation = SecondPassOperations.FilteringOrAggregatesOrDomainScope;
				AggregateUpdateContext aggContext = new AggregateUpdateContext(this.m_odpContext, AggregateMode.Aggregates);
				this.m_odpContext.DomainScopeContext = new DomainScopeContext();
				this.m_dataSetObj.SortAndFilter(aggContext);
				this.m_odpContext.DomainScopeContext = null;
			}
		}

		private void SortAndFilter()
		{
			if (this.m_report.NeedPostGroupProcessing)
			{
				this.m_odpContext.SecondPassOperation = (SecondPassOperations)(this.m_report.HasVariables ? 1 : 0);
				if (this.m_report.HasSpecialRecursiveAggregates)
				{
					this.m_odpContext.SecondPassOperation |= SecondPassOperations.FilteringOrAggregatesOrDomainScope;
				}
				else
				{
					this.m_odpContext.SecondPassOperation |= (SecondPassOperations.Sorting | SecondPassOperations.FilteringOrAggregatesOrDomainScope);
				}
				AggregateUpdateContext aggContext = new AggregateUpdateContext(this.m_odpContext, AggregateMode.Aggregates);
				this.m_dataSetObj.EnterProcessUserSortPhase();
				this.m_odpContext.DomainScopeContext = new DomainScopeContext();
				this.m_dataSetObj.SortAndFilter(aggContext);
				this.m_odpContext.DomainScopeContext = null;
				if (this.m_report.HasSpecialRecursiveAggregates)
				{
					this.m_odpContext.SecondPassOperation = SecondPassOperations.Sorting;
					this.m_dataSetObj.SortAndFilter(aggContext);
				}
				this.m_dataSetObj.LeaveProcessUserSortPhase();
			}
		}

		private void PostSortOperations()
		{
			if (this.m_report.HasPostSortAggregates)
			{
				Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection = new Dictionary<string, IReference<RuntimeGroupRootObj>>();
				AggregateUpdateContext aggContext = new AggregateUpdateContext(this.m_odpContext, AggregateMode.PostSortAggregates);
				this.m_dataSetObj.CalculateRunningValues(groupCollection, aggContext);
			}
		}

		private void StoreDataSetLevelAggregates()
		{
			if (this.m_dataSet.Aggregates == null && this.m_dataSet.PostSortAggregates == null)
			{
				return;
			}
			DataSetInstance dataSetInstance = this.m_odpContext.CurrentReportInstance.GetDataSetInstance(this.m_dataSet.IndexInCollection, this.m_odpContext);
			if (this.m_dataSet.Aggregates != null)
			{
				dataSetInstance.StoreAggregates(this.m_odpContext, this.m_dataSet.Aggregates);
			}
			if (this.m_dataSet.PostSortAggregates != null)
			{
				dataSetInstance.StoreAggregates(this.m_odpContext, this.m_dataSet.PostSortAggregates);
			}
		}

		public void GenerateGroupTree()
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "Data processing complete.  Beginning group tree creation");
			OnDemandMetadata odpMetadata = this.m_odpContext.OdpMetadata;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.EnsureGroupTreeStorageSetup(odpMetadata, this.m_odpContext.ChunkFactory, odpMetadata.GlobalIDOwnerCollection, false, this.m_odpContext.GetActiveCompatibilityVersion(), this.m_odpContext.ProhibitSerializableValues);
			this.CreateInstances();
			if (!this.m_odpContext.InSubreportInDataRegion)
			{
				this.m_odpContext.TopLevelContext.MergeNewUserSortFilterInformation();
				odpMetadata.ResetUserSortFilterContexts();
			}
			this.m_dataSetObj.CompleteLookupProcessing();
		}

		private void CreateInstances()
		{
			this.m_odpContext.ReportRuntime.CurrentScope = this.m_dataSetObj;
			if (this.m_hasSortFilterInfo && this.m_odpContext.RuntimeSortFilterInfo != null)
			{
				if (this.m_odpContext.TopLevelContext.ReportRuntimeUserSortFilterInfo == null)
				{
					this.m_odpContext.TopLevelContext.ReportRuntimeUserSortFilterInfo = new List<IReference<RuntimeSortFilterEventInfo>>();
				}
				this.m_odpContext.TopLevelContext.ReportRuntimeUserSortFilterInfo.AddRange(this.m_odpContext.RuntimeSortFilterInfo);
			}
			this.m_dataSetObj.CreateInstances();
			if (this.m_odpContext.ReportDefinition.InScopeEventSources != null)
			{
				int count = this.m_odpContext.ReportDefinition.InScopeEventSources.Count;
				List<IInScopeEventSource> list = null;
				for (int i = 0; i < count; i++)
				{
					IInScopeEventSource inScopeEventSource = this.m_odpContext.ReportDefinition.InScopeEventSources[i];
					if (inScopeEventSource.UserSort.DataSet == this.m_dataSet)
					{
						if (list == null)
						{
							list = new List<IInScopeEventSource>(count - i);
						}
						list.Add(inScopeEventSource);
					}
				}
				if (list != null)
				{
					UserSortFilterContext.ProcessEventSources(this.m_odpContext, this.m_dataSetObj, list);
				}
			}
			this.m_odpContext.ReportRuntime.CurrentScope = null;
		}
	}
}
