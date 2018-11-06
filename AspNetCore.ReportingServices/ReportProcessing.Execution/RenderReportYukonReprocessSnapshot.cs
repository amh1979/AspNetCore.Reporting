using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportYukonReprocessSnapshot : RenderReportYukon
	{
		private readonly IChunkFactory m_originalSnapshotChunks;

		protected override bool IsSnapshotReprocessing
		{
			get
			{
				return true;
			}
		}

		public RenderReportYukonReprocessSnapshot(ProcessingContext pc, RenderingContext rc, ReportProcessing processing, IChunkFactory originalSnapshotChunks)
			: base(pc, rc, processing)
		{
			this.m_originalSnapshotChunks = originalSnapshotChunks;
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(base.PublicProcessingContext.ChunkFactory);
			ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(this.m_originalSnapshotChunks);
			Hashtable definitionObjects = null;
			DateTime executionTime = default(DateTime);
			Report report = ReportProcessing.DeserializeReportFromSnapshot((ReportProcessing.GetReportChunk)object2.GetReportChunk, out executionTime, out definitionObjects);
			base.m_reportSnapshot = base.Processing.ProcessReport(report, base.PublicProcessingContext, true, false, (ReportProcessing.GetReportChunk)object2.GetReportChunk, (ErrorContext)errorContext, executionTime, (ReportProcessing.CreateReportChunk)null, out base.m_context, out userProfileState);
			Global.Tracer.Assert(null != base.m_context, "(null != context)");
			base.m_chunkManager = new ChunkManager.RenderingChunkManager(@object.GetReportChunk, null, definitionObjects, null, report.IntermediateFormatVersion);
			base.m_renderingContext = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(base.m_reportSnapshot, base.PublicRenderingContext.Format, executionTime, report.EmbeddedImages, report.ImageStreamNames, base.PublicRenderingContext.EventInfo, base.PublicProcessingContext.ReportContext, base.ReportUri, null, @object.GetReportChunk, base.m_chunkManager, base.PublicProcessingContext.GetResourceCallback, @object.GetChunkMimeType, base.PublicRenderingContext.StoreServerParametersCallback, false, base.PublicProcessingContext.AllowUserProfileState, base.PublicRenderingContext.ReportRuntimeSetup, base.PublicProcessingContext.JobContext, base.PublicProcessingContext.DataProtection);
		}

		protected override void PrepareForExecution()
		{
			base.ValidateReportParameters();
		}
	}
}
