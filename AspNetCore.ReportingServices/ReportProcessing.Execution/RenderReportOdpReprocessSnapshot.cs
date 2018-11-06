using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpReprocessSnapshot : RenderReportOdp
	{
		private readonly IChunkFactory m_originalSnapshotChunks;

		protected override bool IsSnapshotReprocessing
		{
			get
			{
				return true;
			}
		}

		public RenderReportOdpReprocessSnapshot(ProcessingContext pc, RenderingContext rc, IConfiguration configuration, IChunkFactory originalSnapshotChunks)
			: base(pc, rc, configuration)
		{
			this.m_originalSnapshotChunks = originalSnapshotChunks;
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			OnDemandMetadata odpMetadataFromSnapshot = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report report = default(AspNetCore.ReportingServices.ReportIntermediateFormat.Report);
			GlobalIDOwnerCollection globalIDOwnerCollection = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOdpReportSnapshot(base.PublicProcessingContext, this.m_originalSnapshotChunks, errorContext, true, false, base.Configuration, ref odpMetadataFromSnapshot, out report);
			ProcessReportOdpSnapshotReprocessing processReportOdpSnapshotReprocessing = new ProcessReportOdpSnapshotReprocessing(base.Configuration, base.PublicProcessingContext, report, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, odpMetadataFromSnapshot);
			base.m_odpReportSnapshot = ((ProcessReportOdp)processReportOdpSnapshotReprocessing).Execute(out base.m_odpContext);
		}

		protected override void PrepareForExecution()
		{
			base.ValidateReportParameters();
		}
	}
}
