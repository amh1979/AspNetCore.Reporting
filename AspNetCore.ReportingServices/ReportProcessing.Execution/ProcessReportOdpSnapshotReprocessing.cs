using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpSnapshotReprocessing : ProcessReportOdpSnapshot
	{
		protected override bool ReprocessSnapshot
		{
			get
			{
				return true;
			}
		}

		public ProcessReportOdpSnapshotReprocessing(IConfiguration configuration, ProcessingContext pc, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, OnDemandMetadata odpMetadataFromSnapshot)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, odpMetadataFromSnapshot)
		{
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			OnDemandMetadata onDemandMetadata = new OnDemandMetadata(base.OdpMetadataFromSnapshot, base.ReportDefinition);
			onDemandMetadata.ReportSnapshot = new AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot(base.ReportDefinition, base.PublicProcessingContext.ReportContext.ItemName, base.PublicProcessingContext.Parameters, base.PublicProcessingContext.RequestUserName, base.OdpMetadataFromSnapshot.ReportSnapshot.ExecutionTime, base.PublicProcessingContext.ReportContext.HostRootUri, base.PublicProcessingContext.ReportContext.ParentPath, base.PublicProcessingContext.UserLanguage.Name);
			return onDemandMetadata;
		}

		protected override void SetupReportLanguage(Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpMerge.EvaluateReportLanguage(reportInstance, null);
		}
	}
}
