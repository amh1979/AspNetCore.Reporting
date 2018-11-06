using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal sealed class ProcessDataShape : ProcessReportOdpStreaming
	{
		private readonly bool m_useParallelQueryExecution;

		public ProcessDataShape(IConfiguration configuration, ProcessingContext pc, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime, IAbortHelper abortHelper, bool useParallelQueryExecution)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime, abortHelper)
		{
			this.m_useParallelQueryExecution = useParallelQueryExecution;
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			this.ParallelPreloadQueries(odpContext);
			base.SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
		}

		private void ParallelPreloadQueries(OnDemandProcessingContext odpContext)
		{
			if (this.m_useParallelQueryExecution)
			{
				IJobContext jobContext = odpContext.JobContext;
			}
		}
	}
}
