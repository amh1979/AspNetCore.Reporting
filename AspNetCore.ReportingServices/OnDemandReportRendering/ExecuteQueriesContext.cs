using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ExecuteQueriesContext
	{
		private readonly IDbConnection m_connection;

		private readonly IProcessingDataExtensionConnection m_dataExtensionConnection;

		private readonly DataSourceInfo m_dataSourceInfo;

		private readonly CreateAndRegisterStream m_createAndRegisterStream;

		private readonly IJobContext m_jobContext;

		internal IDbConnection Connection
		{
			get
			{
				return this.m_connection;
			}
		}

		internal CreateAndRegisterStream CreateAndRegisterStream
		{
			get
			{
				return this.m_createAndRegisterStream;
			}
		}

		internal IJobContext JobContext
		{
			get
			{
				return this.m_jobContext;
			}
		}

		internal ExecuteQueriesContext(IDbConnection connection, IProcessingDataExtensionConnection dataExtensionConnection, DataSourceInfo dataSourceInfo, CreateAndRegisterStream createAndRegisterStream, IJobContext jobContext)
		{
			this.m_connection = connection;
			this.m_dataExtensionConnection = dataExtensionConnection;
			this.m_dataSourceInfo = dataSourceInfo;
			this.m_createAndRegisterStream = createAndRegisterStream;
			this.m_jobContext = jobContext;
		}

		internal IDbCommand CreateCommandWrapperForCancel(IDbCommand command)
		{
			return new CommandWrappedForCancel(command, this.m_dataExtensionConnection, null, this.m_dataSourceInfo, null, this.m_connection);
		}
	}
}
