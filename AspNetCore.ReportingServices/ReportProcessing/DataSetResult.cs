namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class DataSetResult
	{
		private ParameterInfoCollection m_parameters;

		private ProcessingMessageList m_warnings;

		private UserProfileState m_usedUserProfileState;

		private bool m_successfulCompletion;

		public ParameterInfoCollection Parameters
		{
			get
			{
				return this.m_parameters;
			}
		}

		public ProcessingMessageList Warnings
		{
			get
			{
				return this.m_warnings;
			}
		}

		public UserProfileState UsedUserProfileState
		{
			get
			{
				return this.m_usedUserProfileState;
			}
		}

		public bool SuccessfulCompletion
		{
			get
			{
				return this.m_successfulCompletion;
			}
		}

		public DataSetResult(ParameterInfoCollection finalParameters, ProcessingMessageList warnings, UserProfileState usedUserProfileState, bool successfulCompletion)
		{
			this.m_parameters = finalParameters;
			this.m_warnings = warnings;
			this.m_usedUserProfileState = usedUserProfileState;
			this.m_successfulCompletion = successfulCompletion;
		}
	}
}
