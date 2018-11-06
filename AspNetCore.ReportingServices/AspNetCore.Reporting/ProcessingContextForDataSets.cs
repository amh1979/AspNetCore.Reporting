using AspNetCore.Reporting;
using AspNetCore.ReportingServices;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

namespace AspNetCore.Reporting
{
	internal class ProcessingContextForDataSets : ProcessingContext
	{
		private IEnumerable m_dataSources;

		private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subReportInfoCallback;

		internal override bool EnableDataBackedParameters
		{
			get
			{
				return false;
			}
		}

		internal override IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction
		{
			get
			{
				return new DataSetExtensionConnection(this.m_subReportInfoCallback, this.m_dataSources);
			}
		}

		internal override RuntimeDataSourceInfoCollection DataSources
		{
			get
			{
				return null;
			}
		}

		internal override RuntimeDataSetInfoCollection SharedDataSetReferences
		{
			get
			{
				return null;
			}
		}

		internal override bool CanShareDataSets
		{
			get
			{
				return false;
			}
		}

		public ProcessingContextForDataSets(PreviewItemContext reportContext, ParameterInfoCollection parameters, IEnumerable dataSources, ReportProcessing.OnDemandSubReportCallback subReportCallback, LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subReportInfoCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup reportRuntimeSetup, AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream createStreamCallback)
			: base(reportContext, WindowsIdentity.GetCurrent().Name, parameters, subReportCallback, getResourceFunction, chunkFactory, ReportProcessing.ExecutionType.Live, Thread.CurrentThread.CurrentCulture, UserProfileState.Both, UserProfileState.None, reportRuntimeSetup, createStreamCallback, false, new ViewerJobContextImpl(), new ViewerExtensionFactory(), DataProtectionLocal.Instance)
		{
			this.m_dataSources = dataSources;
			this.m_subReportInfoCallback = subReportInfoCallback;
		}

		internal override ReportProcessing.ProcessingContext CreateInternalProcessingContext(string chartName, AspNetCore.ReportingServices.ReportProcessing.Report report, ErrorContext errorContext, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, ReportProcessing.GetReportChunk getChunkCallback, ReportProcessing.CreateReportChunk cacheDataCallback)
		{
			AspNetCore.ReportingServices.ReportProcessing.Global.Tracer.Assert(false, "CreateInternalProcessingContext is not used for ODP Engine Controls");
			return null;
		}

		internal override ReportProcessing.ProcessingContext ParametersInternalProcessingContext(ErrorContext errorContext, DateTime executionTimeStamp, bool isSnapshot)
		{
			AspNetCore.ReportingServices.ReportProcessing.Global.Tracer.Assert(false, "ParametersInternalProcessingContext is not used for ODP Engine Controls");
			return null;
		}
	}
}
