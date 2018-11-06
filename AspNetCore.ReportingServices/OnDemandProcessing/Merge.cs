using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class Merge
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

		private OnDemandProcessingContext m_odpContext;

		private RetrievalManager m_retrievalManager;

		private string m_reportLanguage;

		private bool m_initialized;

		private ParameterInfoCollection m_parameters;

		internal Merge(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext odpContext)
		{
			this.m_report = report;
			this.m_odpContext = odpContext;
			this.m_retrievalManager = new RetrievalManager(report, odpContext);
		}

		internal void Init(bool includeParameters, bool parametersOnly)
		{
			if (this.m_odpContext.ReportRuntime == null)
			{
				this.m_odpContext.ReportObjectModel.Initialize(this.m_report, this.m_odpContext.CurrentReportInstance);
				this.m_odpContext.ReportRuntime = new AspNetCore.ReportingServices.RdlExpressions.ReportRuntime(this.m_report.ObjectType, this.m_odpContext.ReportObjectModel, this.m_odpContext.ErrorContext);
			}
			if (!this.m_initialized && !this.m_odpContext.InitializedRuntime)
			{
				this.m_initialized = true;
				this.m_odpContext.ReportRuntime.LoadCompiledCode(this.m_report, includeParameters, parametersOnly, this.m_odpContext.ReportObjectModel, this.m_odpContext.ReportRuntimeSetup);
				if (this.m_odpContext.ReportRuntime.ReportExprHost != null)
				{
					this.m_report.SetExprHost(this.m_odpContext.ReportRuntime.ReportExprHost, this.m_odpContext.ReportObjectModel);
				}
			}
		}

		internal void Init(ParameterInfoCollection parameters)
		{
			if (this.m_odpContext.ReportRuntime == null)
			{
				this.Init(false, false);
			}
			this.m_odpContext.ReportObjectModel.Initialize(parameters);
			this.m_odpContext.ReportRuntime.CustomCodeOnInit(this.m_odpContext.ReportDefinition);
		}

		internal void EvaluateReportLanguage(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, string snapshotLanguage)
		{
			CultureInfo cultureInfo = null;
			if (snapshotLanguage != null)
			{
				this.m_reportLanguage = snapshotLanguage;
				cultureInfo = Merge.GetSpecificCultureInfoFromLanguage(snapshotLanguage, this.m_odpContext.ErrorContext);
			}
			else if (this.m_report.Language != null)
			{
				if (this.m_report.Language.Type != AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant)
				{
					this.m_odpContext.LanguageInstanceId += 1u;
					this.m_reportLanguage = this.m_odpContext.ReportRuntime.EvaluateReportLanguageExpression(this.m_report, out cultureInfo);
				}
				else
				{
					cultureInfo = Merge.GetSpecificCultureInfoFromLanguage(this.m_report.Language.StringValue, this.m_odpContext.ErrorContext);
				}
			}
			if (cultureInfo == null && !this.m_odpContext.InSubreport)
			{
				cultureInfo = Localization.DefaultReportServerSpecificCulture;
			}
			if (cultureInfo != null)
			{
				Thread.CurrentThread.CurrentCulture = cultureInfo;
				reportInstance.Language = cultureInfo.ToString();
				this.m_odpContext.ThreadCulture = cultureInfo;
			}
		}

		private static CultureInfo GetSpecificCultureInfoFromLanguage(string language, ErrorContext errorContext)
		{
			CultureInfo cultureInfo = null;
			try
			{
				cultureInfo = new CultureInfo(language, false);
				if (cultureInfo.IsNeutralCulture)
				{
					cultureInfo = CultureInfo.CreateSpecificCulture(language);
					cultureInfo = new CultureInfo(cultureInfo.Name, false);
					return cultureInfo;
				}
				return cultureInfo;
			}
			catch (Exception ex)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Warning, ObjectType.Report, null, "Language", ex.Message);
				return cultureInfo;
			}
		}

		internal void FetchData(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, bool mergeTransaction)
		{
			if (this.m_odpContext.ProcessWithCachedData)
			{
				if (!reportInstance.IsMissingExpectedDataChunk(this.m_odpContext))
				{
					return;
				}
				throw new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataCacheUnavailableException();
			}
			if (!this.m_odpContext.SnapshotProcessing)
			{
				if (this.m_odpContext.InSubreport)
				{
					this.m_odpContext.CreateAndSetupDataExtensionFunction.DataSetRetrieveForReportInstance(this.m_odpContext.ReportContext, this.m_parameters);
				}
				if (!this.m_retrievalManager.PrefetchData(reportInstance, this.m_parameters, mergeTransaction))
				{
					throw new ProcessingAbortedException();
				}
				reportInstance.NoRows = this.m_retrievalManager.NoRows;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance PrepareReportInstance(IReportInstanceContainer reportInstanceContainer)
		{
			IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance> reference;
			if (reportInstanceContainer.ReportInstance == null || reportInstanceContainer.ReportInstance.Value() == null)
			{
				reference = AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance.CreateInstance(reportInstanceContainer, this.m_odpContext, this.m_report, this.m_parameters);
			}
			else
			{
				reference = reportInstanceContainer.ReportInstance;
				reference.Value().InitializeFromSnapshot(this.m_odpContext);
			}
			return reference.Value();
		}

		internal void SetupReport(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			this.m_odpContext.CurrentReportInstance = reportInstance;
			if (!this.m_odpContext.InitializedRuntime)
			{
				this.m_odpContext.InitializedRuntime = true;
				List<ReportSection> reportSections = this.m_report.ReportSections;
				if (reportSections != null)
				{
					foreach (ReportSection item in reportSections)
					{
						this.m_odpContext.RuntimeInitializeReportItemObjs(item.ReportItems, true);
						this.m_odpContext.RuntimeInitializeTextboxObjs(item.ReportItems, true);
					}
				}
				if (this.m_report.HasVariables)
				{
					this.m_odpContext.RuntimeInitializeVariables(this.m_report);
				}
				if (this.m_report.HasLookups)
				{
					this.m_odpContext.RuntimeInitializeLookups(this.m_report);
				}
				this.m_report.RegisterDataSetScopedAggregates(this.m_odpContext);
			}
		}

		internal bool InitAndSetupSubReport(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance> currentSubReportInstance = subReport.CurrentSubReportInstance;
			bool flag = this.InitSubReport(subReport, currentSubReportInstance);
			if (flag)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = currentSubReportInstance.Value().ReportInstance.Value();
				this.m_odpContext.SetupEnvironment(reportInstance);
				this.m_odpContext.ReportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.OnDemandExpressions);
				this.m_odpContext.IsUnrestrictedRenderFormatReferenceMode = true;
			}
			currentSubReportInstance.Value().Initialized = true;
			return flag;
		}

		private bool InitSubReport(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport, IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance> subReportInstanceRef)
		{
			bool flag = true;
			AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance = subReportInstanceRef.Value();
			if (this.m_odpContext.SnapshotProcessing && subReportInstance.ProcessedWithError)
			{
				return false;
			}
			try
			{
				if (!this.m_odpContext.SnapshotProcessing)
				{
					subReportInstance.RetrievalStatus = subReport.RetrievalStatus;
				}
				if (subReportInstance.RetrievalStatus == AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed)
				{
					subReportInstance.ProcessedWithError = true;
					return false;
				}
				if (this.m_odpContext.CurrentReportInstance == null && subReport.Report != null)
				{
					if (!this.m_odpContext.SnapshotProcessing)
					{
						subReport.OdpSubReportInfo.UserSortDataSetGlobalId = this.m_odpContext.ParentContext.UserSortFilterContext.DataSetGlobalId;
					}
					this.m_odpContext.UserSortFilterContext.UpdateContextForFirstSubreportInstance(this.m_odpContext.ParentContext.UserSortFilterContext);
				}
				if (this.m_odpContext.SnapshotProcessing && !this.m_odpContext.ReprocessSnapshot)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = subReportInstance.ReportInstance.Value();
					this.m_odpContext.CurrentReportInstance = reportInstance;
					this.m_odpContext.LoadExistingSubReportDataChunkNameModifier(subReportInstance);
					reportInstance.InitializeFromSnapshot(this.m_odpContext);
					this.Init(true, false);
					this.m_odpContext.ThreadCulture = subReportInstance.ThreadCulture;
					this.SetupReport(reportInstance);
					this.m_odpContext.SetSubReportContext(subReportInstance, true);
					this.m_odpContext.ReportRuntime.CustomCodeOnInit(this.m_odpContext.ReportDefinition);
					this.m_odpContext.OdpMetadata.SetUpdatedVariableValues(this.m_odpContext, reportInstance);
					return flag;
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance2 = this.PrepareReportInstance(subReportInstance);
				this.m_odpContext.CurrentReportInstance = reportInstance2;
				this.Init(true, false);
				subReportInstance.InstanceUniqueName = this.m_odpContext.CreateUniqueID().ToString(CultureInfo.InvariantCulture);
				this.m_odpContext.SetSubReportContext(subReportInstance, false);
				this.SetupReport(reportInstance2);
				this.m_parameters = OnDemandProcessingContext.EvaluateSubReportParameters(this.m_odpContext.ParentContext, subReport);
				bool flag2 = default(bool);
				if (!this.m_odpContext.SnapshotProcessing && !this.m_odpContext.ProcessWithCachedData)
				{
					try
					{
						this.m_odpContext.ProcessReportParameters = true;
						this.m_odpContext.ReportObjectModel.ParametersImpl.Clear();
						AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ProcessReportParameters(subReport.Report, this.m_odpContext, this.m_parameters);
					}
					finally
					{
						this.m_odpContext.ProcessReportParameters = false;
					}
					if (!this.m_parameters.ValuesAreValid())
					{
						subReportInstance.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.ParametersNotSpecified;
						throw new ReportProcessingException(ErrorCode.rsParametersNotSpecified);
					}
					this.m_odpContext.ReportParameters = this.m_parameters;
				}
				else if (!this.m_parameters.ValuesAreValid(out flag2) && flag2)
				{
					subReportInstance.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.ParametersNotSpecified;
					throw new ReportProcessingException(ErrorCode.rsParametersNotSpecified);
				}
				this.Init(this.m_parameters);
				subReportInstance.Parameters = new ParametersImpl(this.m_odpContext.ReportObjectModel.ParametersImpl);
				this.m_odpContext.SetSubReportNameModifierAndParameters(subReportInstance, !this.m_odpContext.SnapshotProcessing);
				if (this.m_odpContext.ReprocessSnapshot)
				{
					reportInstance2.InitializeFromSnapshot(this.m_odpContext);
				}
				this.EvaluateReportLanguage(reportInstance2, null);
				subReportInstance.ThreadCulture = this.m_odpContext.ThreadCulture;
				if (!this.m_odpContext.SnapshotProcessing || this.m_odpContext.FoundExistingSubReportInstance)
				{
					flag = this.FetchSubReportData(subReport, subReportInstance);
					if (flag)
					{
						subReportInstance.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataRetrieved;
					}
					else
					{
						subReportInstance.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataRetrieveFailed;
						subReportInstance.ProcessedWithError = true;
					}
				}
				else
				{
					subReportInstance.RetrievalStatus = AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataNotRetrieved;
					subReportInstance.ProcessedWithError = true;
				}
				this.m_odpContext.ReportParameters = null;
				return flag;
			}
			catch (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataCacheUnavailableException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				flag = false;
				subReportInstance.ProcessedWithError = true;
				if (subReportInstance.ReportInstance != null)
				{
					subReportInstance.ReportInstance.Value().NoRows = false;
				}
				if (ex2 is RSException)
				{
					this.m_odpContext.ErrorContext.Register((RSException)ex2, subReport.ObjectType);
					return flag;
				}
				return flag;
			}
		}

		internal bool FetchSubReportData(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport, AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = subReportInstance.ReportInstance.Value();
			reportInstance.ResetReportVariables(subReport.OdpContext);
			bool flag;
			try
			{
				this.FetchData(reportInstance, subReport.MergeTransactions);
				if (subReport.OdpContext.ReprocessSnapshot && reportInstance.IsMissingExpectedDataChunk(subReport.OdpContext))
				{
					flag = false;
				}
				else
				{
					if (subReport.OdpContext.ReprocessSnapshot && !subReport.InDataRegion)
					{
						Merge.PreProcessTablixes(subReport.Report, subReport.OdpContext, false);
					}
					flag = true;
				}
			}
			catch (ProcessingAbortedException)
			{
				flag = false;
			}
			if (flag)
			{
				reportInstance.CalculateAndStoreReportVariables(subReport.OdpContext);
			}
			return flag;
		}

		internal static void TablixDataProcessing(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet specificDataSetOnly)
		{
			bool flag = false;
			while (!flag)
			{
				int indexInCollection = specificDataSetOnly.IndexInCollection;
				int unprocessedDataSetCount = default(int);
				bool[] exclusionList = odpContext.GenerateDataSetExclusionList(out unprocessedDataSetCount);
				indexInCollection = odpContext.ReportDefinition.CalculateDatasetRootIndex(indexInCollection, exclusionList, unprocessedDataSetCount);
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = odpContext.ReportDefinition.MappingDataSetIndexToDataSet[indexInCollection];
				FullAtomicDataPipelineManager fullAtomicDataPipelineManager = new FullAtomicDataPipelineManager(odpContext, dataSet);
				fullAtomicDataPipelineManager.StartProcessing();
				fullAtomicDataPipelineManager.StopProcessing();
				flag = (indexInCollection == specificDataSetOnly.IndexInCollection);
			}
		}

		internal static bool PreProcessTablixes(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext odpContext, bool onlyWithSubReports)
		{
			bool result = false;
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource in report.DataSources)
			{
				if (dataSource.DataSets != null)
				{
					foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet in dataSource.DataSets)
					{
						DataSetInstance dataSetInstance = odpContext.CurrentReportInstance.GetDataSetInstance(dataSet, odpContext);
						if (dataSetInstance != null && dataSet.DataRegions.Count != 0 && !odpContext.IsTablixProcessingComplete(dataSet.IndexInCollection) && (!onlyWithSubReports || dataSet.HasSubReports || (odpContext.InSubreport && odpContext.HasUserSortFilter)))
						{
							result = true;
							Merge.TablixDataProcessing(odpContext, dataSet);
						}
					}
				}
			}
			return result;
		}
	}
}
