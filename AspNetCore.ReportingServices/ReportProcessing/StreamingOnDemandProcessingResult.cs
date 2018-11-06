using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class StreamingOnDemandProcessingResult : OnDemandProcessingResult
	{
		public override bool SnapshotChanged
		{
			get
			{
				return false;
			}
		}

		internal StreamingOnDemandProcessingResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot newOdpSnapshot, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager chunkManager, bool newOdpSnapshotChanged, IChunkFactory createChunkFactory, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, newOdpSnapshot.DefinitionTreeHasDocumentMap, newOdpSnapshot.HasShowHide || newOdpSnapshot.HasUserSortFilter, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
		}

		public override void Save()
		{
		}
	}
}
