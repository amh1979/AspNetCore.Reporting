using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpStreaming : ProcessReportOdpInitial
	{
		private readonly IAbortHelper m_abortHelper;

		protected override OnDemandProcessingContext.Mode OnDemandProcessingMode
		{
			get
			{
				return OnDemandProcessingContext.Mode.Streaming;
			}
		}

		public ProcessReportOdpStreaming(IConfiguration configuration, ProcessingContext pc, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime, IAbortHelper abortHelper)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime)
		{
			this.m_abortHelper = abortHelper;
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			base.SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
		}

		protected override IAbortHelper GetAbortHelper()
		{
			return this.m_abortHelper ?? base.GetAbortHelper();
		}

		protected override void CleanupAbortHandler(OnDemandProcessingContext odpContext)
		{
		}
	}
}
