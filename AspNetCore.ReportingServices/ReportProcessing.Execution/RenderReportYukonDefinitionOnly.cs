using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal sealed class RenderReportYukonDefinitionOnly : RenderReportYukonInitial
	{
		public RenderReportYukonDefinitionOnly(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, ReportProcessing processing, IChunkFactory yukonCompiledDefinition)
			: base(pc, rc, executionTimeStamp, processing, yukonCompiledDefinition)
		{
		}

		protected override AspNetCore.ReportingServices.OnDemandReportRendering.Report PrepareROM(out AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext)
		{
			odpRenderingContext = new AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext(base.PublicRenderingContext.Format, base.m_reportSnapshot, base.PublicProcessingContext.ChunkFactory, base.PublicRenderingContext.EventInfo);
			odpRenderingContext.InstanceAccessDisallowed = true;
			return new AspNetCore.ReportingServices.OnDemandReportRendering.Report(base.m_reportSnapshot.Report, base.m_reportSnapshot.ReportInstance, base.m_renderingContext, odpRenderingContext, base.ReportName, base.PublicRenderingContext.ReportDescription);
		}
	}
}
