using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportYukonInitial : RenderReportYukon
	{
		private readonly DateTime m_executionTimeStamp;

		private readonly IChunkFactory m_yukonCompiledDefinition;

		public RenderReportYukonInitial(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, ReportProcessing processing, IChunkFactory yukonCompiledDefinition)
			: base(pc, rc, processing)
		{
			this.m_executionTimeStamp = executionTimeStamp;
			this.m_yukonCompiledDefinition = yukonCompiledDefinition;
		}

		protected override void PrepareForExecution()
		{
			base.ValidateReportParameters();
			ReportProcessing.CheckReportCredentialsAndConnectionUserDependency(base.PublicProcessingContext);
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(this.m_yukonCompiledDefinition);
			ChunkFactoryAdapter object2 = new ChunkFactoryAdapter(base.PublicProcessingContext.ChunkFactory);
			Hashtable definitionObjects = null;
			Report report = ReportProcessing.DeserializeReport((ReportProcessing.GetReportChunk)@object.GetReportChunk, out definitionObjects);
			base.m_reportSnapshot = base.Processing.ProcessReport(report, base.PublicProcessingContext, false, false, (ReportProcessing.GetReportChunk)object2.GetReportChunk, (ErrorContext)errorContext, this.m_executionTimeStamp, (ReportProcessing.CreateReportChunk)null, out base.m_context, out userProfileState);
			Global.Tracer.Assert(null != base.m_context, "(null != m_context)");
			executionLogContext.AddLegacyDataProcessingTime(base.m_context.DataProcessingDurationMs);
			base.m_chunkManager = new ChunkManager.RenderingChunkManager(object2.GetReportChunk, null, definitionObjects, null, report.IntermediateFormatVersion);
			base.m_renderingContext = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(base.m_reportSnapshot, base.PublicRenderingContext.Format, this.m_executionTimeStamp, report.EmbeddedImages, report.ImageStreamNames, base.PublicRenderingContext.EventInfo, base.PublicProcessingContext.ReportContext, base.ReportUri, base.RenderingParameters, object2.GetReportChunk, base.m_chunkManager, base.PublicProcessingContext.GetResourceCallback, object2.GetChunkMimeType, base.PublicRenderingContext.StoreServerParametersCallback, false, base.PublicProcessingContext.AllowUserProfileState, base.PublicRenderingContext.ReportRuntimeSetup, base.PublicProcessingContext.JobContext, base.PublicProcessingContext.DataProtection);
		}
	}
}
