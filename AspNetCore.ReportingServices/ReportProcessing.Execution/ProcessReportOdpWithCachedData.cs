using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal sealed class ProcessReportOdpWithCachedData : ProcessReportOdpInitial
	{
		private readonly OnDemandMetadata m_odpMetadataFromDataCache;

		protected override bool ProcessWithCachedData
		{
			get
			{
				return true;
			}
		}

		public ProcessReportOdpWithCachedData(IConfiguration configuration, ProcessingContext pc, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime, OnDemandMetadata odpMetadataFromDataCache)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime)
		{
			Global.Tracer.Assert(odpMetadataFromDataCache != null, "Must provide existing metadata to process with cached data");
			this.m_odpMetadataFromDataCache = odpMetadataFromDataCache;
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			OnDemandMetadata onDemandMetadata = base.PrepareMetadata();
			onDemandMetadata.PrepareForCachedDataProcessing(this.m_odpMetadataFromDataCache);
			return onDemandMetadata;
		}
	}
}
