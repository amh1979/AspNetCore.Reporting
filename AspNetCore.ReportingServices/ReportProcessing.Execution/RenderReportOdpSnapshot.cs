using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpSnapshot : RenderReportOdp
	{
		protected virtual bool IsRenderStream
		{
			get
			{
				return false;
			}
		}

		public RenderReportOdpSnapshot(ProcessingContext pc, RenderingContext rc, IConfiguration configuration)
			: base(pc, rc, configuration)
		{
		}

		protected override void PrepareForExecution()
		{
			EventInformation eventInfo = default(EventInformation);
			bool flag = default(bool);
			ReportProcessing.ProcessOdpToggleEvent(base.PublicRenderingContext.ShowHideToggle, base.PublicProcessingContext.ChunkFactory, base.PublicRenderingContext.EventInfo, out eventInfo, out flag);
			if (flag)
			{
				base.PublicRenderingContext.EventInfo = eventInfo;
			}
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			OnDemandMetadata onDemandMetadata = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report report = default(AspNetCore.ReportingServices.ReportIntermediateFormat.Report);
			GlobalIDOwnerCollection globalIDOwnerCollection = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.DeserializeOdpReportSnapshot(base.PublicProcessingContext, (IChunkFactory)null, errorContext, true, true, base.Configuration, ref onDemandMetadata, out report);
			base.m_odpReportSnapshot = onDemandMetadata.ReportSnapshot;
			ProcessReportOdpSnapshot processReportOdpSnapshot = new ProcessReportOdpSnapshot(base.Configuration, base.PublicProcessingContext, report, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, onDemandMetadata);
			((ProcessReportOdp)processReportOdpSnapshot).Execute(out base.m_odpContext);
		}

		protected override void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
			if (!this.IsRenderStream)
			{
				errorContext.Combine(base.m_odpReportSnapshot.Warnings);
			}
			base.CleanupSuccessfulProcessing(errorContext);
		}
	}
}
