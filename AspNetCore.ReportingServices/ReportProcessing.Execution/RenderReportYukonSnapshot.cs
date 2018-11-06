using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportYukonSnapshot : RenderReportYukon
	{
		protected virtual bool IsRenderStream
		{
			get
			{
				return false;
			}
		}

		public RenderReportYukonSnapshot(ProcessingContext pc, RenderingContext rc, ReportProcessing processing)
			: base(pc, rc, processing)
		{
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			ChunkFactoryAdapter @object = new ChunkFactoryAdapter(base.PublicProcessingContext.ChunkFactory);
			Hashtable instanceObjects = default(Hashtable);
			Hashtable definitionObjects = default(Hashtable);
			IntermediateFormatReader.State declarationsRead = default(IntermediateFormatReader.State);
			bool flag = default(bool);
			base.m_reportSnapshot = ReportProcessing.DeserializeReportSnapshot((ReportProcessing.GetReportChunk)@object.GetReportChunk, (ReportProcessing.CreateReportChunk)@object.CreateReportChunk, base.PublicProcessingContext.GetResourceCallback, base.PublicRenderingContext, base.PublicProcessingContext.DataProtection, out instanceObjects, out definitionObjects, out declarationsRead, out flag);
			Global.Tracer.Assert(null != base.m_reportSnapshot, "(null != reportSnapshot)");
			Global.Tracer.Assert(null != base.m_reportSnapshot.Report, "(null != reportSnapshot.Report)");
			Global.Tracer.Assert(null != base.m_reportSnapshot.ReportInstance, "(null != reportSnapshot.ReportInstance)");
			base.m_chunkManager = new ChunkManager.RenderingChunkManager(@object.GetReportChunk, instanceObjects, definitionObjects, declarationsRead, base.m_reportSnapshot.Report.IntermediateFormatVersion);
			bool flag2 = default(bool);
			EventInformation eventInfo = default(EventInformation);
			base.Processing.ProcessShowHideToggle(base.PublicRenderingContext.ShowHideToggle, base.m_reportSnapshot, base.PublicRenderingContext.EventInfo, base.m_chunkManager, out flag2, out eventInfo);
			if (flag2)
			{
				base.PublicRenderingContext.EventInfo = eventInfo;
			}
			bool retrieveRenderingInfo = this.IsRenderStream || !flag;
			base.m_renderingContext = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(base.m_reportSnapshot, base.PublicRenderingContext.Format, base.m_reportSnapshot.ExecutionTime, base.m_reportSnapshot.Report.EmbeddedImages, base.m_reportSnapshot.Report.ImageStreamNames, base.PublicRenderingContext.EventInfo, base.PublicRenderingContext.ReportContext, base.PublicRenderingContext.ReportUri, base.RenderingParameters, @object.GetReportChunk, base.m_chunkManager, base.PublicProcessingContext.GetResourceCallback, @object.GetChunkMimeType, base.PublicRenderingContext.StoreServerParametersCallback, retrieveRenderingInfo, base.PublicRenderingContext.AllowUserProfileState, base.PublicRenderingContext.ReportRuntimeSetup, base.PublicProcessingContext.JobContext, base.PublicProcessingContext.DataProtection);
		}

		protected override void PrepareForExecution()
		{
		}

		protected override void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
			if (!this.IsRenderStream)
			{
				errorContext.Combine(base.m_reportSnapshot.Warnings);
			}
		}

		protected override OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext)
		{
			ReportInstanceInfo reportInstanceInfo = (ReportInstanceInfo)base.m_reportSnapshot.ReportInstance.GetInstanceInfo(base.m_renderingContext.ChunkManager);
			return new YukonProcessingResult(renderingInfoChanged, base.PublicProcessingContext.ChunkFactory, base.m_reportSnapshot.HasShowHide, base.m_renderingContext.RenderingInfoManager, eventInfoChanged, base.PublicRenderingContext.EventInfo, reportInstanceInfo.Parameters, errorContext.Messages, base.m_reportSnapshot.Report.AutoRefresh, base.GetNumberOfPages(renderProperties), base.GetUpdatedPaginationMode(renderProperties, base.PublicRenderingContext.ClientPaginationMode), base.PublicProcessingContext.ChunkFactory.ReportProcessingFlags, base.m_renderingContext.UsedUserProfileState, executionLogContext);
		}
	}
}
