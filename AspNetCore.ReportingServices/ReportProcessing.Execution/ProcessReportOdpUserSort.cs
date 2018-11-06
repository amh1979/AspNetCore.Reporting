using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpUserSort : ProcessReportOdpSnapshotReprocessing
	{
		private readonly SortFilterEventInfoMap m_oldUserSortInformation;

		private readonly EventInformation m_newUserSortInformation;

		private readonly string m_oldUserSortEventSourceUniqueName;

		public ProcessReportOdpUserSort(IConfiguration configuration, ProcessingContext pc, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, OnDemandMetadata odpMetadataFromSnapshot, SortFilterEventInfoMap oldUserSortInformation, EventInformation newUserSortInformation, string oldUserSortEventSourceUniqueName)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, odpMetadataFromSnapshot)
		{
			this.m_oldUserSortInformation = oldUserSortInformation;
			this.m_newUserSortInformation = newUserSortInformation;
			this.m_oldUserSortEventSourceUniqueName = oldUserSortEventSourceUniqueName;
		}

		protected override void CompleteOdpContext(OnDemandProcessingContext odpContext)
		{
			odpContext.OldSortFilterEventInfo = this.m_oldUserSortInformation;
			odpContext.UserSortFilterInfo = this.m_newUserSortInformation;
			odpContext.UserSortFilterEventSourceUniqueName = this.m_oldUserSortEventSourceUniqueName;
		}

		protected override void PreProcessTablices(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			Merge.PreProcessTablixes(base.ReportDefinition, odpContext, false);
			reportSnapshot.SortFilterEventInfo = odpContext.NewSortFilterEventInfo;
		}
	}
}
