using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Threading;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class ExecutedQuery
	{
		private readonly DataSource m_dataSource;

		private readonly DataSet m_dataSet;

		private readonly OnDemandProcessingContext m_odpContext;

		private readonly DataProcessingMetrics m_executionMetrics;

		private readonly string m_commandText;

		private readonly DateTime m_queryExecutionTimestamp;

		private readonly DataSourceErrorInspector m_errorInspector;

		private IDbConnection m_connection;

		private IDbCommand m_command;

		private IDbCommand m_commandWrappedForCancel;

		private IDataReader m_dataReader;

		internal DataSet DataSet
		{
			get
			{
				return this.m_dataSet;
			}
		}

		internal DateTime QueryExecutionTimestamp
		{
			get
			{
				return this.m_queryExecutionTimestamp;
			}
		}

		internal string CommandText
		{
			get
			{
				return this.m_commandText;
			}
		}

		internal DataSourceErrorInspector ErrorInspector
		{
			get
			{
				return this.m_errorInspector;
			}
		}

		internal DataProcessingMetrics ExecutionMetrics
		{
			get
			{
				return this.m_executionMetrics;
			}
		}

		internal ExecutedQuery(DataSource dataSource, DataSet dataSet, OnDemandProcessingContext odpContext, DataProcessingMetrics executionMetrics, string commandText, DateTime queryExecutionTimestamp, DataSourceErrorInspector errorInspector)
		{
			this.m_dataSource = dataSource;
			this.m_dataSet = dataSet;
			this.m_odpContext = odpContext;
			this.m_executionMetrics = executionMetrics;
			this.m_commandText = commandText;
			this.m_queryExecutionTimestamp = queryExecutionTimestamp;
			this.m_errorInspector = errorInspector;
		}

		internal void AssumeOwnership(ref IDbConnection connection, ref IDbCommand command, ref IDbCommand commandWrappedForCancel, ref IDataReader dataReader)
		{
			ExecutedQuery.AssignAndClear<IDbConnection>(ref this.m_connection, ref connection);
			ExecutedQuery.AssignAndClear<IDbCommand>(ref this.m_command, ref command);
			ExecutedQuery.AssignAndClear<IDbCommand>(ref this.m_commandWrappedForCancel, ref commandWrappedForCancel);
			ExecutedQuery.AssignAndClear<IDataReader>(ref this.m_dataReader, ref dataReader);
		}

		internal void ReleaseOwnership(ref IDbConnection connection)
		{
			ExecutedQuery.AssignAndClear<IDbConnection>(ref connection, ref this.m_connection);
		}

		internal void ReleaseOwnership(ref IDbCommand command, ref IDbCommand commandWrappedForCancel, ref IDataReader dataReader)
		{
			ExecutedQuery.AssignAndClear<IDataReader>(ref dataReader, ref this.m_dataReader);
			ExecutedQuery.AssignAndClear<IDbCommand>(ref commandWrappedForCancel, ref this.m_commandWrappedForCancel);
			ExecutedQuery.AssignAndClear<IDbCommand>(ref command, ref this.m_command);
		}

		private static void AssignAndClear<T>(ref T target, ref T source) where T : class
		{
			Interlocked.Exchange<T>(ref target, source);
			Interlocked.Exchange<T>(ref source, (T)null);
		}

		internal void Close()
		{
			IDataReader dataReader = Interlocked.Exchange<IDataReader>(ref this.m_dataReader, (IDataReader)null);
			if (dataReader != null)
			{
				QueryExecutionUtils.DisposeDataExtensionObject(ref dataReader, "data reader", this.m_dataSet.Name, this.m_executionMetrics, DataProcessingMetrics.MetricType.DisposeDataReader);
			}
			this.m_commandWrappedForCancel = null;
			IDbCommand dbCommand = Interlocked.Exchange<IDbCommand>(ref this.m_command, (IDbCommand)null);
			if (dbCommand != null)
			{
				QueryExecutionUtils.DisposeDataExtensionObject(ref dbCommand, "command", this.m_dataSet.Name);
			}
			IDbConnection dbConnection = Interlocked.Exchange<IDbConnection>(ref this.m_connection, (IDbConnection)null);
			if (dbConnection != null)
			{
				RuntimeDataSource.CloseConnection(dbConnection, this.m_dataSource, this.m_odpContext, this.m_executionMetrics);
			}
		}
	}
}
