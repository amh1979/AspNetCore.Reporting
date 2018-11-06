using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpLiveAndCacheData : RenderReportOdpInitial
	{
		private IChunkFactory m_metaDataChunkFactory;

		public RenderReportOdpLiveAndCacheData(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, IConfiguration configuration, IChunkFactory metaDataChunkFactory)
			: base(pc, rc, executionTimeStamp, configuration)
		{
			Global.Tracer.Assert(metaDataChunkFactory != null, "Must supply a IChunkFactory to store the cached data");
			this.m_metaDataChunkFactory = metaDataChunkFactory;
		}

		protected override void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.SerializeMetadata(this.m_metaDataChunkFactory, base.OdpContext.OdpMetadata, base.OdpContext.GetActiveCompatibilityVersion(), base.OdpContext.ProhibitSerializableValues);
			base.CleanupSuccessfulProcessing(errorContext);
		}
	}
}
