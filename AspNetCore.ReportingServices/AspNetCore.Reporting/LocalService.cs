using AspNetCore.ReportingServices;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Library;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Text;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal abstract class LocalService : ILocalProcessingHost, IDisposable
	{
		internal delegate void LocalModeSecurityValidatorCallback(PreviewItemContext itemContext, PublishingResult publishingResult);

		protected class SubreportCallbackHandler
		{
			private LocalService m_service;

			protected LocalService Service
			{
				get
				{
					return this.m_service;
				}
			}

			public SubreportCallbackHandler(LocalService service)
			{
				this.m_service = service;
			}

			public virtual void OnDemandSubReportCallback(ICatalogItemContext itemContext, string subreportPath, string newChunkName, ReportProcessing.NeedsUpgrade upgradeCheck, ParameterInfoCollection parentQueryParameters, out ICatalogItemContext subreportContext, out string description, out IChunkFactory chunkFactory, out ParameterInfoCollection parameters)
			{
				this.m_service.OnGetSubReportDefinition(itemContext, subreportPath, newChunkName, upgradeCheck, parentQueryParameters, out subreportContext, out description, out chunkFactory, out parameters);
			}
		}

		[NonSerialized]
		private LocalDataRetrieval m_dataRetrieval;

		private ILocalCatalog m_catalog;

		private LocalCatalogTempDB m_catalogTempDB = new LocalCatalogTempDB();

		private bool m_showDetailedSubreportMessages = true;

		[NonSerialized]
		private LocalModeSecurityValidatorCallback m_securityValidator;

		private ReportRuntimeSetupHandler m_reportRuntimeSetupHandler = new ReportRuntimeSetupHandler();

		private PreviewItemContext m_itemContext;

		private LocalExecutionSession m_executionSession = new LocalExecutionSession();

		public PreviewItemContext ItemContext
		{
			[DebuggerStepThrough]
			get
			{
				return this.m_itemContext;
			}
			set
			{
				this.m_itemContext = value;
				this.ResetExecution(true);
			}
		}

		private bool GenerateExpressionHostWithRefusedPermissions
		{
			[DebuggerStepThrough]
			get
			{
				return this.m_reportRuntimeSetupHandler.RequireExpressionHostWithRefusedPermissions;
			}
		}

		public ILocalCatalog Catalog
		{
			[DebuggerStepThrough]
			get
			{
				return this.m_catalog;
			}
		}

		public LocalDataRetrieval DataRetrieval
		{
			[DebuggerStepThrough]
			get
			{
				return this.m_dataRetrieval;
			}
			[DebuggerStepThrough]
			set
			{
				this.m_dataRetrieval = value;
			}
		}

		public bool SupportsQueries
		{
			get
			{
				return this.m_dataRetrieval.SupportsQueries;
			}
		}

		public bool CanSelfCancel
		{
			get
			{
				return false;
			}
		}

		public bool ShowDetailedSubreportMessages
		{
			[DebuggerStepThrough]
			get
			{
				return this.m_showDetailedSubreportMessages;
			}
			set
			{
				if (this.m_showDetailedSubreportMessages != value)
				{
					this.m_showDetailedSubreportMessages = value;
					this.ResetExecution();
				}
			}
		}

		public virtual LocalProcessingHostMapTileServerConfiguration MapTileServerConfiguration
		{
			get
			{
				return null;
			}
		}

		public LocalModeSecurityValidatorCallback SecurityValidator
		{
			[DebuggerStepThrough]
			set
			{
				this.m_securityValidator = value;
			}
		}

		protected virtual bool RecompileOnResetExecution
		{
			[DebuggerStepThrough]
			get
			{
				return false;
			}
		}

		protected LocalExecutionSession ExecutionSession
		{
			[DebuggerStepThrough]
			get
			{
				return this.m_executionSession;
			}
		}

		public LocalExecutionInfo ExecutionInfo
		{
			[DebuggerStepThrough]
			get
			{
				return this.m_executionSession.ExecutionInfo;
			}
		}

		protected LocalService(ILocalCatalog catalog, Evidence sandboxEvidence, PolicyManager policyManager)
		{
			this.m_catalog = catalog;
			ReportRuntimeSetupHandler.InitAppDomainPool(sandboxEvidence, policyManager);
		}

		public void Dispose()
		{
			this.m_reportRuntimeSetupHandler.Dispose();
			this.m_catalogTempDB.Dispose();
		}

		public void CopySecuritySettingsFrom(ILocalProcessingHost sourceProcessingHost)
		{
			LocalService localService = (LocalService)sourceProcessingHost;
			this.m_reportRuntimeSetupHandler = localService.m_reportRuntimeSetupHandler;
		}

		public void SetCancelState(bool shouldCancelRequests)
		{
			throw new NotSupportedException();
		}

		public void SetReportParameters(NameValueCollection userSpecifiedValues)
		{
			ParameterInfoCollection parameterInfoCollection = this.m_executionSession.ExecutionInfo.ReportParameters;
			if (parameterInfoCollection == null)
			{
				ControlSnapshot controlSnapshot = default(ControlSnapshot);
				PublishingResult compiledReport = this.GetCompiledReport(this.m_itemContext, false, out controlSnapshot);
				parameterInfoCollection = compiledReport.Parameters;
			}
			else if (userSpecifiedValues == null)
			{
				return;
			}
			ParameterInfoCollection parameterInfoCollection2;
			if (userSpecifiedValues != null)
			{
				ParameterInfoCollection newParameters = ParameterInfoCollection.DecodeFromNameValueCollectionAndUserCulture(userSpecifiedValues);
				parameterInfoCollection2 = ParameterInfoCollection.Combine(parameterInfoCollection, newParameters, true, false, false, false, Localization.ClientPrimaryCulture);
			}
			else
			{
				parameterInfoCollection2 = parameterInfoCollection;
			}
			ParameterInfoCollection parameterInfoCollection3 = new ParameterInfoCollection();
			parameterInfoCollection2.CopyTo(parameterInfoCollection3);
			this.ProcessAndStoreReportParameters(parameterInfoCollection3);
		}

		public void SetReportDataSourceCredentials(DatasourceCredentials[] credentials)
		{
			DatasourceCredentialsCollection credentials2 = this.m_executionSession.Credentials;
			credentials2.Clear();
			if (credentials != null)
			{
				foreach (DatasourceCredentials datasourceCred in credentials)
				{
					credentials2.Add(datasourceCred);
				}
			}
			this.ProcessAndStoreReportParameters(this.m_executionSession.ExecutionInfo.ReportParameters);
		}

		private void ProcessAndStoreReportParameters(ParameterInfoCollection newParameters)
		{
			ControlSnapshot chunkFactory = default(ControlSnapshot);
			this.GetCompiledReport(this.m_itemContext, false, out chunkFactory);
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext pc = this.CreateProcessingContext(newParameters, null, this.m_executionSession.Credentials, chunkFactory, @object.StreamCallback, this.CreateSubreportCallbackHandler());
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				bool flag = default(bool);
				reportProcessing.ProcessReportParameters(DateTime.Now, pc, false, out flag);
				this.m_executionSession.ExecutionInfo.ReportParameters = newParameters;
			}
		}

		public int PerformSearch(int startPage, int endPage, string searchText)
		{
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = this.CreateProcessingContext(@object.StreamCallback);
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				OnDemandProcessingResult onDemandProcessingResult = default(OnDemandProcessingResult);
				return reportProcessing.ProcessFindStringEvent(startPage, endPage, searchText, this.m_executionSession.EventInfo, processingContext, out onDemandProcessingResult);
			}
		}

		public void PerformToggle(string toggleId)
		{
			if (toggleId != null)
			{
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				EventInformation eventInfo = default(EventInformation);
				bool flag = default(bool);
				reportProcessing.ProcessToggleEvent(toggleId, (IChunkFactory)this.m_executionSession.Snapshot, this.m_executionSession.EventInfo, out eventInfo, out flag);
				if (flag)
				{
					this.m_executionSession.EventInfo = eventInfo;
				}
			}
		}

		public int PerformBookmarkNavigation(string bookmarkId, out string uniqueName)
		{
			uniqueName = null;
			if (bookmarkId == null)
			{
				return 0;
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = this.CreateProcessingContext(@object.StreamCallback);
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				OnDemandProcessingResult onDemandProcessingResult = default(OnDemandProcessingResult);
				return reportProcessing.ProcessBookmarkNavigationEvent(bookmarkId, this.m_executionSession.EventInfo, processingContext, out uniqueName, out onDemandProcessingResult);
			}
		}

		public int PerformDocumentMapNavigation(string documentMapId)
		{
			if (documentMapId == null)
			{
				return 0;
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = this.CreateProcessingContext(@object.StreamCallback);
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				OnDemandProcessingResult onDemandProcessingResult = default(OnDemandProcessingResult);
				return reportProcessing.ProcessDocumentMapNavigationEvent(documentMapId, this.m_executionSession.EventInfo, processingContext, out onDemandProcessingResult);
			}
		}

		public string PerformDrillthrough(string drillthroughId, out NameValueCollection resultParameters)
		{
			resultParameters = null;
			if (drillthroughId == null)
			{
				return null;
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = this.CreateProcessingContext(@object.StreamCallback);
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				OnDemandProcessingResult onDemandProcessingResult = default(OnDemandProcessingResult);
				return reportProcessing.ProcessDrillthroughEvent(drillthroughId, this.m_executionSession.EventInfo, processingContext, out resultParameters, out onDemandProcessingResult);
			}
		}

		public int PerformSort(string paginationMode, string sortId, SortOptions sortDirection, bool clearSort, out string uniqueName)
		{
			this.SetProcessingCulture();
			ControlSnapshot snapshot = this.m_executionSession.Snapshot;
			try
			{
				this.m_executionSession.Snapshot = new ControlSnapshot();
				snapshot.PrepareExecutionSnapshot(this.m_executionSession.Snapshot, null);
				using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
				{
					this.m_itemContext.RSRequestParameters.PaginationModeValue = paginationMode;
					ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
					ProcessingContext pc = this.CreateProcessingContext(@object.StreamCallback);
					AspNetCore.ReportingServices.ReportProcessing.RenderingContext rc = this.CreateRenderingContext();
					int result = default(int);
					OnDemandProcessingResult onDemandProcessingResult = reportProcessing.ProcessUserSortEvent(sortId, sortDirection, clearSort, pc, rc, (IChunkFactory)snapshot, out uniqueName, out result);
					if (onDemandProcessingResult != null && onDemandProcessingResult.SnapshotChanged)
					{
						this.m_executionSession.SaveProcessingResult(onDemandProcessingResult);
					}
					else
					{
						this.m_executionSession.Snapshot = snapshot;
					}
					return result;
				}
			}
			catch
			{
				this.m_executionSession.Snapshot = snapshot;
				throw;
			}
		}

		public IDocumentMap GetDocumentMap()
		{
			if (!this.m_executionSession.ExecutionInfo.HasDocMap)
			{
				return null;
			}
			using (ProcessingStreamHandler @object = new ProcessingStreamHandler())
			{
				ProcessingContext processingContext = this.CreateProcessingContext(@object.StreamCallback);
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				OnDemandProcessingResult result = default(OnDemandProcessingResult);
				IDocumentMap documentMap = reportProcessing.GetDocumentMap(this.m_executionSession.EventInfo, processingContext, out result);
				this.m_executionSession.SaveProcessingResult(result);
				return documentMap;
			}
		}

		public abstract IEnumerable<LocalRenderingExtensionInfo> ListRenderingExtensions();

		protected abstract IRenderingExtension CreateRenderer(string format, bool allowInternal);

		public ProcessingMessageList Render(string format, string deviceInfo, string paginationMode, bool allowInternalRenderers, IEnumerable dataSources, CreateAndRegisterStream createStreamCallback)
		{
			this.SetProcessingCulture();
			this.m_itemContext.RSRequestParameters.FormatParamValue = format;
			this.m_itemContext.RSRequestParameters.SetRenderingParameters(deviceInfo);
			this.m_itemContext.RSRequestParameters.PaginationModeValue = paginationMode;
			ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
			IRenderingExtension renderingExtension = this.CreateRenderer(format, allowInternalRenderers);
            //renderingExtension.Report = report;

            bool flag = false;
			if (format == null || this.m_executionSession.Snapshot == null)
			{
				this.ReinitializeSnapshot(null);
				flag = true;
			}
			SubreportCallbackHandler subreportHandler = this.CreateSubreportCallbackHandler();
			ProcessingContext pc = this.CreateProcessingContext(this.m_executionSession.ExecutionInfo.ReportParameters, dataSources, this.m_executionSession.Credentials, this.m_executionSession.Snapshot, createStreamCallback, subreportHandler);
			AspNetCore.ReportingServices.ReportProcessing.RenderingContext rc = this.CreateRenderingContext();
			OnDemandProcessingResult onDemandProcessingResult = null;
			if (flag)
			{
				try
				{
					if (renderingExtension == null)
					{
						onDemandProcessingResult = reportProcessing.CreateSnapshot(DateTime.Now, pc, null);
					}
					else
					{
						this.m_itemContext.RSRequestParameters.SetReportParameters(this.m_executionSession.ExecutionInfo.ReportParameters.AsNameValueCollectionInUserCulture);
						onDemandProcessingResult = this.CreateSnapshotAndRender(reportProcessing, renderingExtension, pc, rc, subreportHandler, this.m_executionSession.ExecutionInfo.ReportParameters, this.m_executionSession.Credentials);
					}
				}
				catch(Exception ex)
				{
					this.m_executionSession.Snapshot = null;
					throw;
				}
			}
			else if (renderingExtension != null)
			{
				onDemandProcessingResult = reportProcessing.RenderSnapshot(renderingExtension, rc, pc);
			}
			this.m_executionSession.SaveProcessingResult(onDemandProcessingResult);
			return onDemandProcessingResult.Warnings;
		}

		public byte[] RenderStream(string format, string deviceInfo, string streamID, out string mimeType)
		{
			if (this.m_executionSession.Snapshot != null)
			{
				Stream chunk = ((SnapshotBase)this.m_executionSession.Snapshot).GetChunk(streamID, ReportProcessing.ReportChunkTypes.StaticImage, out mimeType);
				if (chunk == null)
				{
					chunk = ((SnapshotBase)this.m_executionSession.Snapshot).GetChunk(streamID, ReportProcessing.ReportChunkTypes.Image, out mimeType);
				}
				if (chunk != null)
				{
					byte[] array = new byte[chunk.Length];
					chunk.Read(array, 0, (int)chunk.Length);
					return array;
				}
			}
			using (StreamCache streamCache = new StreamCache())
			{
				this.m_itemContext.RSRequestParameters.SetRenderingParameters(deviceInfo);
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				IRenderingExtension newRenderer = this.CreateRenderer(format, true);
				AspNetCore.ReportingServices.ReportProcessing.RenderingContext rc = this.CreateRenderingContext();
				ProcessingContext pc = this.CreateProcessingContext(streamCache.StreamCallback);
				OnDemandProcessingResult result = reportProcessing.RenderSnapshotStream(newRenderer, streamID, rc, pc);
				this.m_executionSession.SaveProcessingResult(result);
 
				string text2 = default(string);
				return streamCache.GetMainStream(out Encoding text, out mimeType, out text2);
			}
		}

		protected virtual OnDemandProcessingResult CreateSnapshotAndRender(ReportProcessing repProc, IRenderingExtension renderer, ProcessingContext pc, AspNetCore.ReportingServices.ReportProcessing.RenderingContext rc, SubreportCallbackHandler subreportHandler, ParameterInfoCollection parameters, DatasourceCredentialsCollection credentials)
		{
            return repProc.RenderReport(renderer, DateTime.Now, pc, rc, null);
		}

		private DataSetInfoCollection ResolveSharedDataSets(DataSetInfoCollection reportDataSets)
		{
			DataSetInfoCollection dataSetInfoCollection = new DataSetInfoCollection();
			foreach (DataSetInfo reportDataSet in reportDataSets)
			{
				DataSetInfo dataSetInfo;
				if (reportDataSet.IsValidReference())
				{
					dataSetInfo = reportDataSet;
				}
				else
				{
					dataSetInfo = this.m_catalog.GetDataSet(reportDataSet.AbsolutePath);
					if (dataSetInfo == null)
					{
						throw new ItemNotFoundException(reportDataSet.DataSetName);
					}
					dataSetInfo.ID = reportDataSet.ID;
					dataSetInfo.DataSetName = reportDataSet.DataSetName;
				}
				dataSetInfoCollection.Add(dataSetInfo);
			}
			return dataSetInfoCollection;
		}

		private DataSourceInfoCollection ResolveSharedDataSources(DataSourceInfoCollection reportDataSources)
		{
			DataSourceInfoCollection dataSourceInfoCollection = new DataSourceInfoCollection();
			foreach (DataSourceInfo reportDataSource in reportDataSources)
			{
				if (!reportDataSource.IsReference)
				{
					dataSourceInfoCollection.Add(reportDataSource);
				}
				else
				{
					string dataSourceReference = reportDataSource.DataSourceReference;
					DataSourceInfo sharedDataSource = this.GetSharedDataSource(dataSourceReference);
					if (sharedDataSource == null)
					{
						throw new ItemNotFoundException(dataSourceReference);
					}
					sharedDataSource.ID = reportDataSource.ID;
					sharedDataSource.Name = reportDataSource.Name;
					sharedDataSource.OriginalName = reportDataSource.OriginalName;
					dataSourceInfoCollection.Add(sharedDataSource);
				}
			}
			return dataSourceInfoCollection;
		}

		protected virtual DataSourceInfo GetSharedDataSource(string dataSourcePath)
		{
			return this.m_catalog.GetDataSource(dataSourcePath);
		}

		public string[] GetDataSetNames(PreviewItemContext itemContext)
		{
			if (this.DataRetrieval.SupportsQueries)
			{
				return new string[0];
			}
			ControlSnapshot controlSnapshot = default(ControlSnapshot);
			PublishingResult compiledReport = this.GetCompiledReport(itemContext ?? this.m_itemContext, false, out controlSnapshot);
			return compiledReport.DataSetsName ?? new string[0];
		}

		public DataSourcePromptCollection GetReportDataSourcePrompts(out bool allCredentialsSatisfied)
		{
			RuntimeDataSourceInfoCollection runtimeDataSourceInfoCollection = default(RuntimeDataSourceInfoCollection);
			RuntimeDataSetInfoCollection runtimeDataSetInfoCollection = default(RuntimeDataSetInfoCollection);
			this.GetAllReportDataSourcesAndSharedDataSets(out runtimeDataSourceInfoCollection, out runtimeDataSetInfoCollection);
			DatasourceCredentialsCollection datasourceCredentialsCollection = new DatasourceCredentialsCollection();
			foreach (DatasourceCredentials credential in this.m_executionSession.Credentials)
			{
				DataSourceInfo byOriginalName = runtimeDataSourceInfoCollection.GetByOriginalName(credential.PromptID);
				if (byOriginalName != null && byOriginalName.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Prompt)
				{
					datasourceCredentialsCollection.Add(credential);
				}
			}
			runtimeDataSourceInfoCollection.SetCredentials(datasourceCredentialsCollection, DataProtectionLocal.Instance);
			ServerDataSourceSettings serverDatasourceSettings = new ServerDataSourceSettings(true, true);
			DataSourcePromptCollection promptRepresentatives = runtimeDataSourceInfoCollection.GetPromptRepresentatives(serverDatasourceSettings);
			allCredentialsSatisfied = !runtimeDataSourceInfoCollection.NeedPrompt;
			return promptRepresentatives;
		}

		private void GetAllReportDataSourcesAndSharedDataSets(out RuntimeDataSourceInfoCollection runtimeDataSources, out RuntimeDataSetInfoCollection runtimeDataSets)
		{
			if (!this.DataRetrieval.SupportsQueries)
			{
				runtimeDataSources = new RuntimeDataSourceInfoCollection();
				runtimeDataSets = new RuntimeDataSetInfoCollection();
			}
			else
			{
				ControlSnapshot getCompiledDefinitionFactory = default(ControlSnapshot);
				PublishingResult compiledReport = this.GetCompiledReport(this.m_itemContext, false, out getCompiledDefinitionFactory);
				DataSourceInfoCollection existingDataSources = this.ResolveSharedDataSources(compiledReport.DataSources);
				DataSetInfoCollection dataSetInfoCollection = this.ResolveSharedDataSets(compiledReport.SharedDataSets);
				DataSourceInfoCollection dataSources = this.CompileDataSetsAndCombineDataSources(dataSetInfoCollection, existingDataSources);
				ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
				reportProcessing.GetAllDataSources((ICatalogItemContext)this.m_itemContext, (IChunkFactory)getCompiledDefinitionFactory, (ReportProcessing.OnDemandSubReportDataSourcesCallback)this.OnGetSubReportDataSources, dataSources, dataSetInfoCollection, true, new ServerDataSourceSettings(true, true), out runtimeDataSources, out runtimeDataSets);
			}
		}

		private DataSourceInfoCollection CompileDataSetsAndCombineDataSources(DataSetInfoCollection dataSets, DataSourceInfoCollection existingDataSources)
		{
			DataSourceInfoCollection dataSourceInfoCollection = new DataSourceInfoCollection(existingDataSources);
			foreach (DataSetInfo dataSet in dataSets)
			{
				ICatalogItemContext dataSetContext = this.m_itemContext.GetDataSetContext(dataSet.AbsolutePath);
				DataSetPublishingResult compiledDataSet = this.GetCompiledDataSet(dataSet, dataSetContext);
				compiledDataSet.DataSourceInfo.OriginalName = compiledDataSet.DataSourceInfo.ID.ToString();
				dataSourceInfoCollection.Add(compiledDataSet.DataSourceInfo);
				dataSet.DataSourceId = compiledDataSet.DataSourceInfo.ID;
			}
			return dataSourceInfoCollection;
		}

		private DataSetPublishingResult GetCompiledDataSet(DataSetInfo dataSetInfo, ICatalogItemContext dataSetContext)
		{
			StoredDataSet storedDataSet = this.m_catalogTempDB.GetCompiledDataSet(dataSetInfo);
			if (storedDataSet != null && !storedDataSet.Definition.SequenceEqual(dataSetInfo.Definition))
			{
				storedDataSet = null;
			}
			if (storedDataSet == null)
			{
				DataSetPublishingResult result = default(DataSetPublishingResult);
				try
				{
					using (ControlSnapshot createChunkFactory = new ControlSnapshot())
					{
						ReportProcessing reportProcessing = this.CreateAndConfigureReportProcessing();
						PublishingContext sharedDataSetPublishingContext = new PublishingContext(dataSetContext, dataSetInfo.Definition, createChunkFactory, AppDomain.CurrentDomain, true, this.GetDataSourceForSharedDataSetHandler, reportProcessing.Configuration);
						result = reportProcessing.CreateSharedDataSet(sharedDataSetPublishingContext);
					}
				}
				catch (Exception inner)
				{
					throw new DefinitionInvalidException(dataSetInfo.AbsolutePath, inner);
				}
				storedDataSet = new StoredDataSet(dataSetInfo.Definition, result);
				this.m_catalogTempDB.SetCompiledDataSet(dataSetInfo, storedDataSet);
			}
			return storedDataSet.PublishingResult;
		}

		private DataSourceInfo GetDataSourceForSharedDataSetHandler(string dataSourcePath, out Guid catalogItemId)
		{
			DataSourceInfo dataSource = this.m_catalog.GetDataSource(dataSourcePath);
			if (dataSource == null)
			{
				catalogItemId = Guid.Empty;
			}
			else
			{
				catalogItemId = dataSource.ID;
			}
			return dataSource;
		}

		protected abstract SubreportCallbackHandler CreateSubreportCallbackHandler();

		private void OnGetSubReportDefinition(ICatalogItemContext reportContext, string subreportPath, string newChunkName, ReportProcessing.NeedsUpgrade upgradeCheck, ParameterInfoCollection parentQueryParameters, out ICatalogItemContext subreportContext, out string description, out IChunkFactory chunkFactory, out ParameterInfoCollection parameters)
		{
			if (reportContext == null)
			{
				throw new ArgumentException("OnGetSubReportDefinition: Invalid report context");
			}
			if (upgradeCheck(ReportProcessingFlags.OnDemandEngine))
			{
				throw new Exception("Subreport definition is not compatible with this version of viewer controls");
			}
			subreportContext = reportContext.GetSubreportContext(subreportPath);
			ControlSnapshot controlSnapshot = default(ControlSnapshot);
			PublishingResult compiledReport = this.GetCompiledReport((PreviewItemContext)subreportContext, false, out controlSnapshot);
			controlSnapshot.PrepareExecutionSnapshot(this.m_executionSession.Snapshot, newChunkName);
			description = compiledReport.ReportDescription;
			chunkFactory = controlSnapshot;
			parameters = compiledReport.Parameters;
		}

		private void OnGetSubReportDataSources(ICatalogItemContext itemContext, string subreportPath, ReportProcessing.NeedsUpgrade upgradeCheck, out ICatalogItemContext subreportContext, out IChunkFactory getCompiledDefinition, out DataSourceInfoCollection dataSources, out DataSetInfoCollection dataSets)
		{
			subreportPath = this.NormalizeSubReportPath(subreportPath);
			subreportContext = itemContext.GetSubreportContext(subreportPath);
			RSTrace.ReportPreviewTracer.Trace(TraceLevel.Info, "Getting datasources information for {0}.", subreportContext.ItemPathAsString);
			ControlSnapshot controlSnapshot = default(ControlSnapshot);
			PublishingResult compiledReport = this.GetCompiledReport((PreviewItemContext)subreportContext, false, out controlSnapshot);
			getCompiledDefinition = controlSnapshot;
			dataSources = this.ResolveSharedDataSources(compiledReport.DataSources);
			dataSets = this.ResolveSharedDataSets(compiledReport.SharedDataSets);
		}

		private string NormalizeSubReportPath(string pathFromRdl)
		{
			if (pathFromRdl != null && pathFromRdl.Length > 1 && pathFromRdl[0] != '/')
			{
				return "/" + pathFromRdl;
			}
			return pathFromRdl;
		}

		protected abstract void SetProcessingCulture();

		protected abstract IConfiguration CreateProcessingConfiguration();

		private ReportProcessing CreateAndConfigureReportProcessing()
		{
			ReportProcessing reportProcessing = new ReportProcessing();
			reportProcessing.Configuration = this.CreateProcessingConfiguration();
			return reportProcessing;
		}

		protected ProcessingContext CreateProcessingContext(CreateAndRegisterStream createStreamCallback)
		{
			return this.CreateProcessingContext(this.m_executionSession.ExecutionInfo.ReportParameters, null, null, this.m_executionSession.Snapshot, createStreamCallback, this.CreateSubreportCallbackHandler());
		}

		protected ProcessingContext CreateProcessingContext(ParameterInfoCollection reportParameters, IEnumerable dataSources, DatasourceCredentialsCollection credentials, IChunkFactory chunkFactory, CreateAndRegisterStream createStreamCallback, SubreportCallbackHandler subreportHandler)
		{
			RuntimeDataSourceInfoCollection dataSourceInfoColl = null;
			RuntimeDataSetInfoCollection dataSetInfoColl = null;
			this.GetAllReportDataSourcesAndSharedDataSets(out dataSourceInfoColl, out dataSetInfoColl);
			return this.m_dataRetrieval.CreateProcessingContext(this.m_itemContext, reportParameters, dataSources, dataSourceInfoColl, dataSetInfoColl, this.GetCompiledDataSet, credentials, subreportHandler.OnDemandSubReportCallback, new GetResourceForLocalService(this.Catalog), chunkFactory, this.m_reportRuntimeSetupHandler.ReportRuntimeSetup, createStreamCallback);
		}

		private AspNetCore.ReportingServices.ReportProcessing.RenderingContext CreateRenderingContext()
		{
			LocalExecutionInfo executionInfo = this.m_executionSession.ExecutionInfo;
			int num = executionInfo.TotalPages;
			if (executionInfo.PaginationMode != PaginationMode.TotalPages && num > 0)
			{
				num = -num;
			}
			PaginationMode clientPaginationMode = PaginationMode.Progressive;
			if (string.Compare(this.m_itemContext.RSRequestParameters.PaginationModeValue, "Actual", StringComparison.OrdinalIgnoreCase) == 0)
			{
				clientPaginationMode = PaginationMode.TotalPages;
			}
			return new AspNetCore.ReportingServices.ReportProcessing.RenderingContext(this.m_itemContext, "", this.m_executionSession.EventInfo, this.m_reportRuntimeSetupHandler.ReportRuntimeSetup, null, UserProfileState.Both, clientPaginationMode, num);
		}

		private PublishingResult GetCompiledReport(PreviewItemContext itemContext, bool rebuild, out ControlSnapshot snapshot)
		{
			StoredReport storedReport = null;
			if (!rebuild)
			{
				storedReport = this.m_catalogTempDB.GetCompiledReport(itemContext);
				if (storedReport != null && storedReport.GeneratedExpressionHostWithRefusedPermissions != this.GenerateExpressionHostWithRefusedPermissions)
				{
					storedReport = null;
				}
			}
			if (storedReport == null)
			{
				byte[] reportDefinition = this.m_catalog.GetReportDefinition(itemContext);
				PublishingResult publishingResult = ReportCompiler.CompileReport((ICatalogItemContext)itemContext, reportDefinition, this.GenerateExpressionHostWithRefusedPermissions, out snapshot);
				storedReport = new StoredReport(publishingResult, snapshot, this.GenerateExpressionHostWithRefusedPermissions);
				this.m_catalogTempDB.SetCompiledReport(itemContext, storedReport);
				foreach (DataSourceInfo dataSource in storedReport.PublishingResult.DataSources)
				{
					if (!dataSource.IsReference && dataSource.CredentialsRetrieval != DataSourceInfo.CredentialsRetrievalOption.Integrated)
					{
						string text = null;
						string text2 = null;
						this.m_catalog.GetReportDataSourceCredentials(itemContext, dataSource.Name, out text, out text2);
						bool flag = !string.IsNullOrEmpty(text);
						if (flag)
						{
							dataSource.SetUserName(text, DataProtectionLocal.Instance);
						}
						bool flag2 = text2 != null;
						if (flag2)
						{
							dataSource.SetPassword(text2, DataProtectionLocal.Instance);
						}
						if (flag || flag2)
						{
							dataSource.CredentialsRetrieval = DataSourceInfo.CredentialsRetrievalOption.Store;
						}
						else if (string.IsNullOrEmpty(dataSource.Prompt))
						{
							dataSource.CredentialsRetrieval = DataSourceInfo.CredentialsRetrievalOption.None;
						}
						else
						{
							dataSource.CredentialsRetrieval = DataSourceInfo.CredentialsRetrievalOption.Prompt;
						}
					}
				}
			}
			this.m_securityValidator(itemContext, storedReport.PublishingResult);
			snapshot = storedReport.Snapshot;
			return storedReport.PublishingResult;
		}

		void ILocalProcessingHost.CompileReport()
		{
			this.CompileReport();
		}

		public PublishingResult CompileReport()
		{
			ControlSnapshot compiledReport = default(ControlSnapshot);
			PublishingResult compiledReport2 = this.GetCompiledReport(this.m_itemContext, true, out compiledReport);
			this.m_executionSession.CompiledReport = compiledReport;
			this.m_executionSession.ExecutionInfo.PageProperties = compiledReport2.PageProperties;
			this.m_executionSession.CompiledDataSources = compiledReport2.DataSources;
			return compiledReport2;
		}

		public void ResetExecution()
		{
			this.ResetExecution(this.RecompileOnResetExecution);
		}

		private void ResetExecution(bool forceRecompile)
		{
			if (forceRecompile)
			{
				DatasourceCredentialsCollection datasourceCredentialsCollection = null;
				ParameterInfoCollection reportParameters = null;
				if (this.RecompileOnResetExecution)
				{
					datasourceCredentialsCollection = this.m_executionSession.Credentials;
					reportParameters = this.m_executionSession.ExecutionInfo.ReportParameters;
				}
				this.m_executionSession = new LocalExecutionSession();
				if (datasourceCredentialsCollection != null)
				{
					foreach (DatasourceCredentials item in datasourceCredentialsCollection)
					{
						this.m_executionSession.Credentials.Add(item);
					}
				}
				this.m_executionSession.ExecutionInfo.ReportParameters = reportParameters;
			}
			else
			{
				this.m_executionSession.ResetExecution();
			}
		}

		protected void ReinitializeSnapshot(ProcessingContext pc)
		{
			this.m_executionSession.Snapshot = new ControlSnapshot();
			this.m_executionSession.CompiledReport.PrepareExecutionSnapshot(this.m_executionSession.Snapshot, null);
			if (pc != null)
			{
				pc.ChunkFactory = this.m_executionSession.Snapshot;
			}
		}

		public void ExecuteReportInCurrentAppDomain(Evidence reportEvidence)
		{
			if (reportEvidence == null)
			{
				this.m_reportRuntimeSetupHandler.ExecuteReportInCurrentAppDomain();
			}
			else
			{
				this.m_reportRuntimeSetupHandler.ExecuteReportInCurrentAppDomain(reportEvidence);
			}
		}

		public void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
		{
			this.m_reportRuntimeSetupHandler.AddTrustedCodeModuleInCurrentAppDomain(assemblyName);
		}

		public void ExecuteReportInSandboxAppDomain()
		{
			this.m_reportRuntimeSetupHandler.ExecuteReportInSandboxAppDomain();
		}

		public void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			this.m_reportRuntimeSetupHandler.AddFullTrustModuleInSandboxAppDomain(assemblyName);
		}

		public void SetBasePermissionsForSandboxAppDomain(PermissionSet permissions)
		{
			if (permissions == null)
			{
				throw new ArgumentNullException("permissions");
			}
			this.m_reportRuntimeSetupHandler.SetBasePermissionsForSandboxAppDomain(permissions);
		}

		public void ReleaseSandboxAppDomain()
		{
			this.m_reportRuntimeSetupHandler.ReleaseSandboxAppDomain();
		}
	}
}
