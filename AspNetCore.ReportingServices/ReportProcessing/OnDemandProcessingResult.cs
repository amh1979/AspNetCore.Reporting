using AspNetCore.ReportingServices.OnDemandProcessing;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal abstract class OnDemandProcessingResult
	{
		private readonly bool m_hasInteractivity;

		private readonly ParameterInfoCollection m_parameters;

		private readonly int m_autoRefresh;

		private readonly ProcessingMessageList m_warnings;

		private readonly int m_numberOfPages;

		private readonly bool m_hasDocumentMap;

		protected readonly IChunkFactory m_createChunkFactory;

		private readonly bool m_eventInfoChanged;

		private readonly EventInformation m_newEventInfo;

		private readonly PaginationMode m_updatedPaginationMode;

		private readonly ReportProcessingFlags m_updatedReportProcessingFlags;

		private readonly UserProfileState m_usedUserProfileState;

		private readonly ExecutionLogContext m_executionLogContext;

		public abstract bool SnapshotChanged
		{
			get;
		}

		public bool HasInteractivity
		{
			get
			{
				return this.m_hasInteractivity;
			}
		}

		public bool HasDocumentMap
		{
			get
			{
				return this.m_hasDocumentMap;
			}
		}

		public ParameterInfoCollection Parameters
		{
			get
			{
				return this.m_parameters;
			}
		}

		public int AutoRefresh
		{
			get
			{
				return this.m_autoRefresh;
			}
		}

		public ProcessingMessageList Warnings
		{
			get
			{
				return this.m_warnings;
			}
		}

		public int NumberOfPages
		{
			get
			{
				return this.m_numberOfPages;
			}
		}

		public bool EventInfoChanged
		{
			get
			{
				return this.m_eventInfoChanged;
			}
		}

		public EventInformation NewEventInfo
		{
			get
			{
				return this.m_newEventInfo;
			}
		}

		public PaginationMode UpdatedPaginationMode
		{
			get
			{
				return this.m_updatedPaginationMode;
			}
		}

		public ReportProcessingFlags UpdatedReportProcessingFlags
		{
			get
			{
				return this.m_updatedReportProcessingFlags;
			}
		}

		public UserProfileState UsedUserProfileState
		{
			get
			{
				return this.m_usedUserProfileState;
			}
		}

		public ExecutionLogContext ExecutionLogContext
		{
			get
			{
				return this.m_executionLogContext;
			}
		}

		protected OnDemandProcessingResult(IChunkFactory createChunkFactory, bool hasDocumentMap, bool hasInteractivity, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
		{
			this.m_createChunkFactory = createChunkFactory;
			this.m_hasDocumentMap = hasDocumentMap;
			this.m_numberOfPages = numberOfPages;
			this.m_hasInteractivity = hasInteractivity;
			this.m_parameters = parameters;
			this.m_autoRefresh = autoRefresh;
			this.m_warnings = warnings;
			this.m_eventInfoChanged = eventInfoChanged;
			this.m_newEventInfo = newEventInfo;
			this.m_parameters = parameters;
			this.m_updatedPaginationMode = updatedPaginationMode;
			this.m_updatedReportProcessingFlags = updatedProcessingFlags;
			this.m_usedUserProfileState = usedUserProfileState;
			this.m_executionLogContext = executionLogContext;
		}

		public abstract void Save();
	}
}
