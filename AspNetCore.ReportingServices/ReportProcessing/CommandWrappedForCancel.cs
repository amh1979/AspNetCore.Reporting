using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class CommandWrappedForCancel : IDbCommand, IDisposable
	{
		private readonly IDbCommand m_command;

		private readonly IProcessingDataExtensionConnection m_dataExtensionConnection;

		private readonly IProcessingDataSource m_dataSourceObj;

		private readonly DataSourceInfo m_dataSourceInfo;

		private readonly string m_datasetName;

		private readonly IDbConnection m_connection;

		public string CommandText
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int CommandTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public CommandType CommandType
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IDataParameterCollection Parameters
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IDbTransaction Transaction
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal CommandWrappedForCancel(IDbCommand command, IProcessingDataExtensionConnection dataExtensionConnection, IProcessingDataSource dataSourceObj, DataSourceInfo dataSourceInfo, string datasetName, IDbConnection connection)
		{
			this.m_command = command;
			this.m_dataExtensionConnection = dataExtensionConnection;
			this.m_dataSourceObj = dataSourceObj;
			this.m_dataSourceInfo = dataSourceInfo;
			this.m_datasetName = datasetName;
			this.m_connection = connection;
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw new NotImplementedException();
		}

		public IDataParameter CreateParameter()
		{
			throw new NotImplementedException();
		}

		public void Cancel()
		{
			if (this.m_command is IDbImpersonationNeededForCommandCancel)
			{
				this.m_dataExtensionConnection.HandleImpersonation(this.m_dataSourceObj, this.m_dataSourceInfo, this.m_datasetName, this.m_connection, delegate
				{
					this.m_command.Cancel();
				});
			}
			else
			{
				this.m_command.Cancel();
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
