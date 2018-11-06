using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class YukonProcessingResult : OnDemandProcessingResult
	{
		private ReportSnapshot m_newSnapshot;

		private ChunkManager.ProcessingChunkManager m_legacyChunkManager;

		private bool m_renderingInfoChanged;

		private RenderingInfoManager m_renderingInfoManager;

		private readonly bool m_snapshotChanged;

		public override bool SnapshotChanged
		{
			get
			{
				return this.m_snapshotChanged;
			}
		}

		internal YukonProcessingResult(ReportSnapshot newSnapshot, ChunkManager.ProcessingChunkManager chunkManager, IChunkFactory createChunkFactory, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool renderingInfoChanged, RenderingInfoManager renderingInfoManager, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, newSnapshot.HasDocumentMap, newSnapshot.HasShowHide, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
			this.m_snapshotChanged = this.Initialize(newSnapshot, chunkManager, renderingInfoChanged, renderingInfoManager);
		}

		internal YukonProcessingResult(bool renderingInfoChanged, IChunkFactory createChunkFactory, bool hasInteractivity, RenderingInfoManager renderingInfoManager, bool eventInfoChanged, EventInformation newEventInfo, ParameterInfoCollection parameters, ProcessingMessageList warnings, int autoRefresh, int numberOfPages, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, false, hasInteractivity, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
			this.m_snapshotChanged = this.Initialize(null, null, renderingInfoChanged, renderingInfoManager);
		}

		internal YukonProcessingResult(ReportSnapshot newSnapshot, ChunkManager.ProcessingChunkManager chunkManager, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(null, newSnapshot.HasDocumentMap, newSnapshot.HasShowHide, parameters, autoRefresh, numberOfPages, warnings, false, null, PaginationMode.Progressive, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
			this.m_snapshotChanged = this.Initialize(newSnapshot, chunkManager, false, null);
		}

		private bool Initialize(ReportSnapshot newSnapshot, ChunkManager.ProcessingChunkManager chunkManager, bool renderingInfoChanged, RenderingInfoManager renderingInfoManager)
		{
			this.m_newSnapshot = newSnapshot;
			this.m_legacyChunkManager = chunkManager;
			this.m_renderingInfoChanged = renderingInfoChanged;
			this.m_renderingInfoManager = renderingInfoManager;
			if (!this.m_renderingInfoChanged)
			{
				return null != this.m_newSnapshot;
			}
			return true;
		}

		public override void Save()
		{
			lock (this)
			{
				if (this.m_newSnapshot != null && this.m_legacyChunkManager != null)
				{
					this.m_legacyChunkManager.SaveFirstPage();
					this.m_legacyChunkManager.SaveReportSnapshot(this.m_newSnapshot);
					this.m_newSnapshot = null;
				}
				if (this.m_renderingInfoChanged && this.m_renderingInfoManager != null)
				{
					ChunkFactoryAdapter @object = new ChunkFactoryAdapter(base.m_createChunkFactory);
					this.m_renderingInfoManager.Save(@object.CreateReportChunk);
					this.m_renderingInfoManager = null;
				}
			}
		}
	}
}
