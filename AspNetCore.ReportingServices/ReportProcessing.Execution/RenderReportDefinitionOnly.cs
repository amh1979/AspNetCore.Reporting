using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportDefinitionOnly : RenderReportOdpInitial
	{
		public RenderReportDefinitionOnly(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, IConfiguration configuration)
			: base(pc, rc, executionTimeStamp, configuration)
		{
		}

		protected override void CleanupForException()
		{
		}

		protected override void FinalCleanup()
		{
		}

		protected override void PrepareForExecution()
		{
			base.ValidateReportParameters();
			ReportProcessing.CheckReportCredentialsAndConnectionUserDependency(base.PublicProcessingContext);
		}

		protected override void UpdateEventInfoInSnapshot()
		{
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			GlobalIDOwnerCollection globalIDOwnerCollection = default(GlobalIDOwnerCollection);
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDefinition = base.GetReportDefinition(out globalIDOwnerCollection);
			ProcessReportDefinitionOnly processReportDefinitionOnly = new ProcessReportDefinitionOnly(base.Configuration, base.PublicProcessingContext, reportDefinition, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, base.ExecutionTimeStamp);
			base.m_odpReportSnapshot = ((ProcessReportOdp)processReportDefinitionOnly).Execute(out base.m_odpContext);
		}

		protected override OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext)
		{
			return new DefinitionOnlyOnDemandProcessingResult(base.m_odpReportSnapshot, base.m_odpContext.OdpMetadata.OdpChunkManager, base.m_odpContext.OdpMetadata.SnapshotHasChanged, base.PublicProcessingContext.ChunkFactory, base.PublicProcessingContext.Parameters, 0, base.GetNumberOfPages(renderProperties), errorContext.Messages, eventInfoChanged, base.PublicRenderingContext.EventInfo, base.GetUpdatedPaginationMode(renderProperties, base.PublicRenderingContext.ClientPaginationMode), base.PublicProcessingContext.ChunkFactory.ReportProcessingFlags, base.m_odpContext.HasUserProfileState, executionLogContext);
		}

		protected override AspNetCore.ReportingServices.OnDemandReportRendering.Report PrepareROM(out AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext)
		{
			odpRenderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(base.PublicRenderingContext.Format, base.m_odpReportSnapshot, base.PublicRenderingContext.EventInfo, base.m_odpContext);
			odpRenderingContext.InstanceAccessDisallowed = true;
			return new AspNetCore.ReportingServices.OnDemandReportRendering.Report(base.m_odpReportSnapshot.Report, odpRenderingContext, base.ReportName, base.PublicRenderingContext.ReportDescription);
		}
	}
}
