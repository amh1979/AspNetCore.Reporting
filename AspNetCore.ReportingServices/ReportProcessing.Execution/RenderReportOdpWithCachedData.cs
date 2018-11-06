using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpWithCachedData : RenderReportOdp
	{
		private readonly DateTime m_executionTimeStamp;

		private readonly IChunkFactory m_dataCacheChunks;

		public RenderReportOdpWithCachedData(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, IConfiguration configuration, IChunkFactory dataCacheChunks)
			: base(pc, rc, configuration)
		{
			this.m_executionTimeStamp = executionTimeStamp;
			this.m_dataCacheChunks = dataCacheChunks;
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			OnDemandMetadata onDemandMetadata = null;
			GlobalIDOwnerCollection globalIDOwnerCollection = new GlobalIDOwnerCollection();
			onDemandMetadata = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOnDemandMetadata(this.m_dataCacheChunks, globalIDOwnerCollection);
			globalIDOwnerCollection = new GlobalIDOwnerCollection();
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report report = ReportProcessing.DeserializeKatmaiReport(base.PublicProcessingContext.ChunkFactory, false, globalIDOwnerCollection);
			ProcessReportOdpWithCachedData processReportOdpWithCachedData = new ProcessReportOdpWithCachedData(base.Configuration, base.PublicProcessingContext, report, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, this.m_executionTimeStamp, onDemandMetadata);
			base.m_odpReportSnapshot = ((ProcessReportOdp)processReportOdpWithCachedData).Execute(out base.m_odpContext);
		}

		protected override void PrepareForExecution()
		{
			base.ValidateReportParameters();
		}
	}
}
