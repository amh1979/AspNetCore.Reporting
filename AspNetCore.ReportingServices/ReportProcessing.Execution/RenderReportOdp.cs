using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal abstract class RenderReportOdp : RenderReport
	{
		protected OnDemandProcessingContext m_odpContext;

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot m_odpReportSnapshot;

		private readonly IConfiguration m_configuration;

		protected OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_odpContext;
			}
		}

		protected override ProcessingEngine RunningProcessingEngine
		{
			get
			{
				return ProcessingEngine.OnDemandEngine;
			}
		}

		protected IConfiguration Configuration
		{
			get
			{
				return this.m_configuration;
			}
		}

		public RenderReportOdp(ProcessingContext pc, RenderingContext rc, IConfiguration configuration)
			: base(pc, rc)
		{
			this.m_configuration = configuration;
		}

		protected override AspNetCore.ReportingServices.OnDemandReportRendering.Report PrepareROM(out AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext)
		{
			odpRenderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(base.PublicRenderingContext.Format, this.m_odpReportSnapshot, base.PublicRenderingContext.EventInfo, this.m_odpContext);
			return new AspNetCore.ReportingServices.OnDemandReportRendering.Report(this.m_odpReportSnapshot.Report, this.m_odpReportSnapshot.ReportInstance, odpRenderingContext, base.ReportName, base.PublicRenderingContext.ReportDescription);
		}

		protected override void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
			ReportProcessing.CleanupOnDemandProcessing(this.m_odpContext, true);
		}

		protected override OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext)
		{
			return new FullOnDemandProcessingResult(this.m_odpReportSnapshot, this.m_odpContext.OdpMetadata.OdpChunkManager, this.m_odpContext.OdpMetadata.SnapshotHasChanged, base.PublicProcessingContext.ChunkFactory, base.PublicProcessingContext.Parameters, this.m_odpReportSnapshot.Report.EvaluateAutoRefresh(null, this.m_odpContext), base.GetNumberOfPages(renderProperties), errorContext.Messages, eventInfoChanged, base.PublicRenderingContext.EventInfo, base.GetUpdatedPaginationMode(renderProperties, base.PublicRenderingContext.ClientPaginationMode), base.PublicProcessingContext.ChunkFactory.ReportProcessingFlags, this.m_odpContext.HasUserProfileState, executionLogContext);
		}

		protected override void FinalCleanup()
		{
			if (this.m_odpContext != null)
			{
				this.m_odpContext.FreeAllResources();
			}
		}

		protected override void CleanupForException()
		{
			ReportProcessing.RequestErrorGroupTreeCleanup(this.m_odpContext);
		}

		protected override void UpdateEventInfoInSnapshot()
		{
			Global.Tracer.Assert(this.m_odpReportSnapshot != null, "Snapshot must exist for ODP Engine");
			if (this.m_odpContext.NewSortFilterEventInfo != null && this.m_odpContext.NewSortFilterEventInfo.Count > 0)
			{
				this.m_odpReportSnapshot.SortFilterEventInfo = this.m_odpContext.NewSortFilterEventInfo;
			}
			else
			{
				this.m_odpReportSnapshot.SortFilterEventInfo = null;
			}
		}
	}
}
