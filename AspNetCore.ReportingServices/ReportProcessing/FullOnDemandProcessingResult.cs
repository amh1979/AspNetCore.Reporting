using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class FullOnDemandProcessingResult : OnDemandProcessingResult
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager m_chunkManager;

		private readonly bool m_snapshotChanged;

		public override bool SnapshotChanged
		{
			get
			{
				return this.m_snapshotChanged;
			}
		}

		internal FullOnDemandProcessingResult(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot newOdpSnapshot, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager chunkManager, bool newOdpSnapshotChanged, IChunkFactory createChunkFactory, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, newOdpSnapshot.DefinitionTreeHasDocumentMap, newOdpSnapshot.HasShowHide || newOdpSnapshot.HasUserSortFilter, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
			this.m_chunkManager = chunkManager;
			this.m_snapshotChanged = newOdpSnapshotChanged;
		}

		public override void Save()
		{
			lock (this)
			{
				if (this.m_chunkManager != null)
				{
					this.m_chunkManager.SerializeSnapshot();
					this.m_chunkManager = null;
				}
			}
		}
	}
}
