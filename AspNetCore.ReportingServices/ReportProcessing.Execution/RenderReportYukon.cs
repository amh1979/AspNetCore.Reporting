using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal abstract class RenderReportYukon : RenderReport
	{
		protected ChunkManager.RenderingChunkManager m_chunkManager;

		protected ReportProcessing.ProcessingContext m_context;

		protected AspNetCore.ReportingServices.ReportRendering.RenderingContext m_renderingContext;

		protected ReportSnapshot m_reportSnapshot;

		private readonly ReportProcessing m_processing;

		protected override ProcessingEngine RunningProcessingEngine
		{
			get
			{
				return ProcessingEngine.YukonEngine;
			}
		}

		protected ReportProcessing Processing
		{
			get
			{
				return this.m_processing;
			}
		}

		protected Uri ReportUri
		{
			get
			{
				return base.PublicRenderingContext.ReportUri;
			}
		}

		public RenderReportYukon(ProcessingContext pc, RenderingContext rc, ReportProcessing processing)
			: base(pc, rc)
		{
			this.m_processing = processing;
		}

		protected override AspNetCore.ReportingServices.OnDemandReportRendering.Report PrepareROM(out AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext)
		{
			odpRenderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(base.PublicRenderingContext.Format, this.m_reportSnapshot, base.PublicProcessingContext.ChunkFactory, base.PublicRenderingContext.EventInfo);
			return new AspNetCore.ReportingServices.OnDemandReportRendering.Report(this.m_reportSnapshot.Report, this.m_reportSnapshot.ReportInstance, this.m_renderingContext, odpRenderingContext, base.ReportName, base.PublicRenderingContext.ReportDescription);
		}

		protected override void FinalCleanup()
		{
			if (this.m_chunkManager != null)
			{
				this.m_chunkManager.Close();
			}
		}

		protected override OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext)
		{
			return new YukonProcessingResult(this.m_reportSnapshot, this.m_context.ChunkManager, base.PublicProcessingContext.ChunkFactory, base.PublicProcessingContext.Parameters, this.m_reportSnapshot.Report.AutoRefresh, base.GetNumberOfPages(renderProperties), errorContext.Messages, true, this.m_renderingContext.RenderingInfoManager, eventInfoChanged, base.PublicRenderingContext.EventInfo, base.GetUpdatedPaginationMode(renderProperties, base.PublicRenderingContext.ClientPaginationMode), base.PublicProcessingContext.ChunkFactory.ReportProcessingFlags, userProfileState | this.m_renderingContext.UsedUserProfileState, executionLogContext);
		}
	}
}
