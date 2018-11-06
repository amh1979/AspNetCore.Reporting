using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpSnapshot : ProcessReportOdp
	{
		private readonly OnDemandMetadata m_odpMetadataFromSnapshot;

		protected OnDemandMetadata OdpMetadataFromSnapshot
		{
			get
			{
				return this.m_odpMetadataFromSnapshot;
			}
		}

		protected override bool SnapshotProcessing
		{
			get
			{
				return true;
			}
		}

		protected override bool ReprocessSnapshot
		{
			get
			{
				return false;
			}
		}

		protected override bool ProcessWithCachedData
		{
			get
			{
				return false;
			}
		}

		protected override OnDemandProcessingContext.Mode OnDemandProcessingMode
		{
			get
			{
				return OnDemandProcessingContext.Mode.Full;
			}
		}

		public ProcessReportOdpSnapshot(IConfiguration configuration, ProcessingContext pc, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, OnDemandMetadata odpMetadataFromSnapshot)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext)
		{
			Global.Tracer.Assert(odpMetadataFromSnapshot != null, "Must provide existing metadata when processing an existing snapshot");
			Global.Tracer.Assert(odpMetadataFromSnapshot.OdpChunkManager != null && null != odpMetadataFromSnapshot.ReportSnapshot, "Must provide chunk manager and ReportSnapshot when processing an existing snapshot");
			this.m_odpMetadataFromSnapshot = odpMetadataFromSnapshot;
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			Global.Tracer.Assert(null != this.m_odpMetadataFromSnapshot.ReportInstance, "Processing an existing snapshot with no ReportInstance");
			return this.m_odpMetadataFromSnapshot;
		}

		protected override void SetupReportLanguage(Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpMerge.EvaluateReportLanguage(reportInstance, reportInstance.Language);
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			base.SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
			if (base.ReportDefinition.HasSubReports)
			{
				SubReportInitializer.InitializeSubReportOdpContext(base.ReportDefinition, odpContext);
				SubReportInitializer.InitializeSubReports(base.ReportDefinition, reportInstance, odpContext, false, false);
			}
			this.PreProcessTablices(odpContext, reportSnapshot);
			reportInstance.CalculateAndStoreReportVariables(odpContext);
			odpContext.OdpMetadata.SetUpdatedVariableValues(odpContext, reportInstance);
		}

		protected virtual void PreProcessTablices(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
		}
	}
}
