using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ReportProcessingContext : ProcessingContext
	{
		private readonly RuntimeDataSourceInfoCollection m_dataSources;

		private readonly RuntimeDataSetInfoCollection m_sharedDataSetReferences;

		private readonly IProcessingDataExtensionConnection m_createDataExtensionInstanceFunction;

		private ISharedDataSet m_dataSetExecute;

		internal override bool EnableDataBackedParameters
		{
			get
			{
				return true;
			}
		}

		internal override RuntimeDataSourceInfoCollection DataSources
		{
			get
			{
				return this.m_dataSources;
			}
		}

		internal override RuntimeDataSetInfoCollection SharedDataSetReferences
		{
			get
			{
				return this.m_sharedDataSetReferences;
			}
		}

		internal IProcessingDataExtensionConnection CreateDataExtensionInstanceFunction
		{
			get
			{
				return this.m_createDataExtensionInstanceFunction;
			}
		}

		internal override bool CanShareDataSets
		{
			get
			{
				return true;
			}
		}

		internal ISharedDataSet DataSetExecute
		{
			get
			{
				return this.m_dataSetExecute;
			}
			set
			{
				this.m_dataSetExecute = value;
			}
		}

		internal override IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction
		{
			get
			{
				return this.m_createDataExtensionInstanceFunction;
			}
		}

		internal ReportProcessingContext(ICatalogItemContext reportContext, string requestUserName, ParameterInfoCollection parameters, RuntimeDataSourceInfoCollection dataSources, RuntimeDataSetInfoCollection sharedDataSetReferences, ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceFunction, IChunkFactory createChunkFactory, ReportProcessing.ExecutionType interactiveExecution, CultureInfo culture, UserProfileState allowUserProfileState, UserProfileState initialUserProfileState, IProcessingDataExtensionConnection createDataExtensionInstanceFunction, ReportRuntimeSetup reportRuntimeSetup, CreateAndRegisterStream createStreamCallback, bool isHistorySnapshot, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection, ISharedDataSet dataSetExecute)
			: base(reportContext, requestUserName, parameters, subReportCallback, getResourceFunction, createChunkFactory, interactiveExecution, culture, allowUserProfileState, initialUserProfileState, reportRuntimeSetup, createStreamCallback, isHistorySnapshot, jobContext, extFactory, dataProtection)
		{
			this.m_dataSources = dataSources;
			this.m_sharedDataSetReferences = sharedDataSetReferences;
			this.m_createDataExtensionInstanceFunction = createDataExtensionInstanceFunction;
			this.m_dataSetExecute = dataSetExecute;
		}

		internal override ReportProcessing.ProcessingContext CreateInternalProcessingContext(string chartName, Report report, ErrorContext errorContext, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, ReportProcessing.GetReportChunk getChunkCallback, ReportProcessing.CreateReportChunk cacheDataCallback)
		{
			SubreportCallbackAdapter @object = new SubreportCallbackAdapter(base.OnDemandSubReportCallback, errorContext);
			return new ReportProcessing.ReportProcessingContext(chartName, this.DataSources, base.RequestUserName, base.UserLanguage, @object.SubReportCallback, base.ReportContext, report, errorContext, base.CreateReportChunkCallback, base.GetResourceCallback, base.InteractiveExecution, executionTime, allowUserProfileState, isHistorySnapshot, snapshotProcessing, processWithCachedData, getChunkCallback, cacheDataCallback, this.CreateDataExtensionInstanceFunction, base.ReportRuntimeSetup, base.JobContext, base.ExtFactory, base.DataProtection);
		}

		internal override ReportProcessing.ProcessingContext ParametersInternalProcessingContext(ErrorContext errorContext, DateTime executionTimeStamp, bool isSnapshot)
		{
			return new ReportProcessing.ReportProcessingContext(null, this.DataSources, base.RequestUserName, base.UserLanguage, base.ReportContext, errorContext, base.InteractiveExecution, executionTimeStamp, base.AllowUserProfileState, isSnapshot, this.CreateDataExtensionInstanceFunction, base.ReportRuntimeSetup, base.JobContext, base.ExtFactory, base.DataProtection);
		}
	}
}
