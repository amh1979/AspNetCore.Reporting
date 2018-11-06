using AspNetCore.ReportingServices.DataExtensions;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSetPublishingResult
	{
		private DataSetDefinition m_dataSetDefinition;

		private DataSourceInfo m_dataSourceInfo;

		private UserLocationFlags m_userReferenceLocation;

		private ProcessingMessageList m_warnings;

		public DataSetDefinition DataSetDefinition
		{
			get
			{
				return this.m_dataSetDefinition;
			}
		}

		public DataSourceInfo DataSourceInfo
		{
			get
			{
				return this.m_dataSourceInfo;
			}
		}

		public bool HasUserProfileQueryDependencies
		{
			get
			{
				if ((this.m_userReferenceLocation & UserLocationFlags.ReportQueries) == (UserLocationFlags)0)
				{
					return false;
				}
				return true;
			}
		}

		public ProcessingMessageList Warnings
		{
			get
			{
				return this.m_warnings;
			}
		}

		public int TimeOut
		{
			get
			{
				if (this.m_dataSetDefinition != null && this.m_dataSetDefinition.DataSetCore != null && this.m_dataSetDefinition.DataSetCore.Query != null)
				{
					return this.m_dataSetDefinition.DataSetCore.Query.TimeOut;
				}
				return 0;
			}
		}

		internal DataSetPublishingResult(DataSetDefinition dataSetDefinition, DataSourceInfo dataSourceInfo, UserLocationFlags userReferenceLocation, ProcessingMessageList warnings)
		{
			this.m_dataSetDefinition = dataSetDefinition;
			this.m_dataSourceInfo = dataSourceInfo;
			this.m_userReferenceLocation = userReferenceLocation;
			this.m_warnings = warnings;
		}
	}
}
