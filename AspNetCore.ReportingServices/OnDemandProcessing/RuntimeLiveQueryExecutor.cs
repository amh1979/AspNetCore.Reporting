using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeLiveQueryExecutor
	{
		protected readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource m_dataSource;

		protected readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		protected readonly OnDemandProcessingContext m_odpContext;

		protected DataProcessingMetrics m_executionMetrics;

		protected IDbConnection m_dataSourceConnection;

		protected AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.TransactionInfo m_transInfo;

		protected bool m_isConnectionOwner;

		protected IDbCommand m_command;

		protected IDbCommand m_commandWrappedForCancel;

		internal DataProcessingMetrics DataSetExecutionMetrics
		{
			get
			{
				return this.m_executionMetrics;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet DataSet
		{
			get
			{
				return this.m_dataSet;
			}
		}

		internal bool IsConnectionOwner
		{
			get
			{
				return this.m_isConnectionOwner;
			}
		}

		internal RuntimeLiveQueryExecutor(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, OnDemandProcessingContext odpContext)
		{
			this.m_dataSource = dataSource;
			this.m_dataSet = dataSet;
			this.m_odpContext = odpContext;
			this.m_executionMetrics = new DataProcessingMetrics(dataSet, this.m_odpContext.JobContext, this.m_odpContext.ExecutionLogContext);
		}

		internal void Abort()
		{
			IDbCommand command = this.m_command;
			IDbCommand commandWrappedForCancel = this.m_commandWrappedForCancel;
			if (command != null)
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Data set '{0}': Cancelling command.", this.m_dataSet.Name.MarkAsPrivate());
				}
				if (commandWrappedForCancel != null)
				{
					commandWrappedForCancel.Cancel();
				}
				else
				{
					command.Cancel();
				}
			}
		}

		protected void CloseConnection()
		{
			if (this.m_isConnectionOwner && this.m_dataSourceConnection != null)
			{
				RuntimeDataSource.CloseConnection(this.m_dataSourceConnection, this.m_dataSource, this.m_odpContext, this.m_executionMetrics);
				this.m_dataSourceConnection = null;
			}
		}

		protected IDataReader RunLiveQuery(List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> queryParams, object[] paramValues)
		{
			IDataReader dataReader = null;
			IDbCommand dbCommand = null;
            ViewerJobContextImpl jobContext = (ViewerJobContextImpl)this.m_odpContext.JobContext;
			if (this.m_dataSourceConnection == null)
			{
				this.m_dataSourceConnection = RuntimeDataSource.OpenConnection(this.m_dataSource, this.m_dataSet, this.m_odpContext, this.m_executionMetrics);
			}
            if (string.IsNullOrEmpty(this.m_dataSourceConnection.ConnectionString)&&!string.IsNullOrEmpty(this.m_dataSource.ConnectStringExpression.OriginalText))
            {
                this.m_dataSourceConnection.ConnectionString = this.m_dataSource.ConnectStringExpression.OriginalText;
            }
			try
			{
				this.m_executionMetrics.StartTimer(DataProcessingMetrics.MetricType.Query);
				dbCommand = this.CreateCommand();
				this.SetCommandParameters(dbCommand, queryParams, paramValues);
				string commandText = this.SetCommandText(dbCommand);
				this.StoreCommandText(commandText);
				this.SetCommandType(dbCommand);
				this.SetTransaction(dbCommand);
				this.m_odpContext.CheckAndThrowIfAborted();
                //todo : delete;
                //var ss=System.AppDomain.CurrentDomain.GetAssemblies().Select(t => t.FullName).OrderBy(t => t).ToList();

                this.SetCommandTimeout(dbCommand);
				this.ExtractRewrittenCommandText(dbCommand);
				this.SetRestartPosition(dbCommand);
				DataSourceInfo dataSourceInfo = null;
				if (dbCommand is IDbImpersonationNeededForCommandCancel)
				{
					dataSourceInfo = this.m_dataSource.GetDataSourceInfo(this.m_odpContext);
				}
				this.m_command = dbCommand;
				this.m_commandWrappedForCancel = new CommandWrappedForCancel(this.m_command, this.m_odpContext.CreateAndSetupDataExtensionFunction, this.m_dataSource, dataSourceInfo, this.m_dataSet.Name, this.m_dataSourceConnection);
				if (jobContext != null)
				{
					jobContext.SetAdditionalCorrelation(this.m_command);
					jobContext.ApplyCommandMemoryLimit(this.m_command);
				}
				DataSourceErrorInspector errorInspector = this.CreateErrorInspector();
				dataReader = this.ExecuteReader(jobContext, errorInspector, commandText);
				this.StoreDataReader(dataReader, errorInspector);
				return dataReader;
			}
			catch (RSException ex)
			{
				this.EagerInlineCommandAndReaderCleanup(ref dataReader, ref dbCommand);
				throw;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				this.EagerInlineCommandAndReaderCleanup(ref dataReader, ref dbCommand);
				throw;
			}
			finally
			{
				this.m_executionMetrics.RecordTimerMeasurement(DataProcessingMetrics.MetricType.Query);
			}
		}

		protected abstract void StoreDataReader(IDataReader dataReader, DataSourceErrorInspector errorInspector);

		protected abstract void ExtractRewrittenCommandText(IDbCommand command);

		private IDbCommand CreateCommand()
		{
			try
			{
				return this.m_dataSourceConnection.CreateCommand();
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorCreatingCommand, innerException, this.m_dataSource.Name.MarkAsModelInfo());
			}
		}

		private IDataReader ExecuteReader(IJobContext jobContext, DataSourceErrorInspector errorInspector, string commandText)
		{
			IDataReader dataReader = null;
			try
			{
				if (jobContext != null)
				{
					jobContext.AddCommand(this.m_commandWrappedForCancel);
				}
				this.m_executionMetrics.StartTimer(DataProcessingMetrics.MetricType.ExecuteReader);
				try
				{
					dataReader = this.m_command.ExecuteReader(CommandBehavior.SingleResult);
                    if (dataReader == null)
                    {
                        string connStr = this.m_dataSourceConnection.ConnectionString;
                        if (string.IsNullOrEmpty(connStr))
                        {
                            connStr = this.m_dataSourceConnection.ConnectionString= this.m_dataSource.ConnectStringExpression.OriginalText;
                        }
                        using (SqlConnection conn = new SqlConnection(connStr))
                        {
                            conn.Open();
                            using (var comm = conn.CreateCommand())
                            {
                                comm.CommandText = this.m_command.CommandText;
                                comm.CommandType = (System.Data.CommandType)this.m_command.CommandType;
                                foreach (IDataParameter p in this.m_command.Parameters)
                                {
                                    var p1 = comm.CreateParameter();
                                    p1.ParameterName = p.ParameterName;
                                    p1.Value = p.Value;
                                    comm.Parameters.Add(p1);
                                }
                                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(comm);
                                var table = new System.Data.DataTable();
                                sqlDataAdapter.Fill(table);
                                dataReader = new Reporting.DataTableReader(table);
                            }
                        }
                    }
				}
				catch (Exception ex)
				{
					if (this.m_odpContext.ContextMode == OnDemandProcessingContext.Mode.Streaming)
					{
						ErrorCode errorCode = ErrorCode.rsSuccess;
						bool flag = errorInspector != null && errorInspector.TryInterpretProviderErrorCode(ex, out errorCode);
						this.TraceExecuteReaderFailed(ex, commandText, flag ? new ErrorCode?(errorCode) : null);
						if (flag)
						{
							string text = string.Format(CultureInfo.CurrentCulture, RPRes.Keys.GetString(ErrorCode.rsErrorExecutingCommand.ToString()), this.m_dataSet.Name.MarkAsPrivate());
							throw new ReportProcessingQueryException(errorCode, ex, text);
						}
						if (errorInspector != null && errorInspector.IsOnPremiseServiceException(ex))
						{
							throw new ReportProcessingQueryOnPremiseServiceException(ErrorCode.rsErrorExecutingCommand, ex, this.m_dataSet.Name.MarkAsPrivate());
						}
					}
					throw new ReportProcessingException(ErrorCode.rsErrorExecutingCommand, ex, this.m_dataSet.Name.MarkAsPrivate());
				}
				finally
				{
					this.m_executionMetrics.RecordTimerMeasurement(DataProcessingMetrics.MetricType.ExecuteReader);
				}
			}
			finally
			{
				if (jobContext != null)
				{
					jobContext.RemoveCommand(this.m_commandWrappedForCancel);
				}
			}
			if (dataReader == null)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "The source data reader is null. Cannot read results.");
				}
				throw new ReportProcessingException(ErrorCode.rsErrorCreatingDataReader, this.m_dataSet.Name.MarkAsPrivate());
			}
			return dataReader;
		}

		private void TraceExecuteReaderFailed(Exception e, string commandText, ErrorCode? specificErrorCode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("An error occured running the query for DataSet: \"");
			stringBuilder.Append(this.m_dataSet.Name.MarkAsPrivate());
			stringBuilder.Append("\"");
			if (specificErrorCode.HasValue)
			{
				stringBuilder.Append(" ErrorCode: \"").Append(specificErrorCode.Value).Append("\"");
			}
			if (this.m_dataSet.Query != null && this.m_dataSet.Query.TimeOut > 0)
			{
				stringBuilder.Append(" Timeout: \"").Append(this.m_dataSet.Query.TimeOut).Append("\"");
			}
			if (!string.IsNullOrEmpty(this.m_dataSource.ConnectionCategory))
			{
				stringBuilder.Append(" ConnectionCategory: \"").Append(this.m_dataSource.ConnectionCategory).Append("\"");
			}
			stringBuilder.Append("Error: \"").Append(e.Message).Append("\"");
			if (!string.IsNullOrEmpty(commandText))
			{
				stringBuilder.Append("Query: ");
				if (commandText.Length > 2048)
				{
					stringBuilder.Append(commandText.Substring(0, 2048).MarkAsPrivate());
					stringBuilder.Append(" ...");
				}
				else
				{
					stringBuilder.Append(commandText.MarkAsPrivate());
				}
			}
			Global.Tracer.Trace(TraceLevel.Error, stringBuilder.ToString());
		}

		protected abstract void SetRestartPosition(IDbCommand command);

		private void SetCommandTimeout(IDbCommand command)
		{
			try
			{
				if (this.m_dataSet.Query.TimeOut == 0 && command is CommandWrapper && ((CommandWrapper)command).UnderlyingCommand.GetType().Name=="SqlCommand")
				{
					command.CommandTimeout = 2147483646;
				}
				else
				{
					command.CommandTimeout = this.m_dataSet.Query.TimeOut;
				}
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorSettingQueryTimeout, innerException, this.m_dataSet.Name.MarkAsPrivate());
			}
		}

		private void SetTransaction(IDbCommand command)
		{
			if (this.m_transInfo != null)
			{
				try
				{
					command.Transaction = this.m_transInfo.Transaction;
				}
				catch (Exception innerException)
				{
					throw new ReportProcessingException(ErrorCode.rsErrorSettingTransaction, innerException, this.m_dataSet.Name.MarkAsPrivate());
				}
			}
		}

		private void SetCommandType(IDbCommand command)
		{
			try
			{
				command.CommandType = (CommandType)this.m_dataSet.Query.CommandType;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorSettingCommandType, innerException, this.m_dataSet.Name.MarkAsPrivate());
			}
		}

		private string SetCommandText(IDbCommand command)
		{
			try
			{
				if (this.m_dataSet.Query.CommandText != null)
				{
					AspNetCore.ReportingServices.RdlExpressions.StringResult stringResult = this.m_odpContext.ReportRuntime.EvaluateCommandText(this.m_dataSet);
					if (stringResult.ErrorOccurred)
					{
						throw new ReportProcessingException(ErrorCode.rsQueryCommandTextProcessingError, this.m_dataSet.Name.MarkAsPrivate());
					}
					command.CommandText = stringResult.Value;
					if (this.m_odpContext.UseVerboseExecutionLogging)
					{
						this.m_executionMetrics.CommandText = stringResult.Value;
					}
					return stringResult.Value;
				}
				return null;
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorSettingCommandText, innerException, this.m_dataSet.Name.MarkAsPrivate());
			}
		}

		protected abstract void StoreCommandText(string commandText);

		private void SetCommandParameters(IDbCommand command, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue> queryParams, object[] paramValues)
		{
			if (queryParams != null)
			{
				int num = 0;
				IDataParameter dataParameter;
				while (true)
				{
					if (num < paramValues.Length)
					{
						if (!this.m_odpContext.IsSharedDataSetExecutionOnly || !((DataSetParameterValue)queryParams[num]).OmitFromQuery)
						{
							try
							{
								dataParameter = command.CreateParameter();
							}
							catch (Exception innerException)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorCreatingQueryParameter, innerException, this.m_dataSet.Name.MarkAsPrivate());
							}
							dataParameter.ParameterName = queryParams[num].Name;
							bool flag = dataParameter is IDataMultiValueParameter && paramValues[num] is ICollection;
							object obj = paramValues[num];
							if (obj == null)
							{
								obj = DBNull.Value;
							}
							if (!(dataParameter is IDataMultiValueParameter) && paramValues[num] is ICollection)
							{
								break;
							}
							if (flag)
							{
								int count = ((ICollection)obj).Count;
								if (1 == count)
								{
									try
									{
										Global.Tracer.Assert(obj is object[], "(paramValue is object[])");
										dataParameter.Value = (obj as object[])[0];
									}
									catch (Exception innerException2)
									{
										throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException2, this.m_dataSource.Name.MarkAsModelInfo());
									}
								}
								else
								{
									object[] array = new object[count];
									((ICollection)obj).CopyTo(array, 0);
									((IDataMultiValueParameter)dataParameter).Values = array;
								}
							}
							else
							{
								try
								{
									dataParameter.Value = obj;
								}
								catch (Exception innerException3)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException3, this.m_dataSource.Name.MarkAsModelInfo());
								}
							}
							try
							{
								command.Parameters.Add(dataParameter);
							}
							catch (Exception innerException4)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorAddingQueryParameter, innerException4, this.m_dataSource.Name.MarkAsModelInfo());
							}
							if (this.m_odpContext.UseVerboseExecutionLogging)
							{
								this.m_executionMetrics.SetQueryParameters(command.Parameters);
							}
						}
						num++;
						continue;
					}
					return;
				}
				throw new ReportProcessingException(ErrorCode.rsErrorAddingMultiValueQueryParameter, null, this.m_dataSet.Name.MarkAsPrivate(), dataParameter.ParameterName);
			}
		}

		protected void EagerInlineCommandAndReaderCleanup(ref IDataReader reader, ref IDbCommand command)
		{
			this.EagerInlineReaderCleanup(ref reader);
			this.EagerInlineCommandCleanup(ref command);
		}

		protected abstract void EagerInlineReaderCleanup(ref IDataReader reader);

		private void EagerInlineCommandCleanup(ref IDbCommand command)
		{
			if (this.m_command != null)
			{
				command = null;
				this.DisposeCommand();
			}
			else
			{
				this.DisposeDataExtensionObject<IDbCommand>(ref command, "command");
			}
		}

		protected void DisposeCommand()
		{
			this.m_commandWrappedForCancel = null;
			this.DisposeDataExtensionObject<IDbCommand>(ref this.m_command, "command");
		}

		protected void CancelCommand()
		{
			if (this.m_commandWrappedForCancel != null)
			{
				try
				{
					this.m_executionMetrics.StartTimer(DataProcessingMetrics.MetricType.CancelCommand);
					this.m_commandWrappedForCancel.Cancel();
					this.m_executionMetrics.RecordTimerMeasurementWithUpdatedTotal(DataProcessingMetrics.MetricType.CancelCommand);
				}
				catch (RSException)
				{
					throw;
				}
				catch (Exception ex2)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex2))
					{
						throw;
					}
					Global.Tracer.Trace(TraceLevel.Warning, "Error occurred while canceling the command for DataSet '" + this.m_dataSet.Name.MarkAsPrivate() + "'. Details: " + ex2.ToString());
				}
			}
		}

		protected void DisposeDataExtensionObject<T>(ref T obj, string objectType) where T : class, IDisposable
		{
			QueryExecutionUtils.DisposeDataExtensionObject<T>(ref obj, objectType, this.m_dataSet.Name.MarkAsPrivate());
		}

		protected void DisposeDataExtensionObject<T>(ref T obj, string objectType, DataProcessingMetrics.MetricType? metricType) where T : class, IDisposable
		{
			QueryExecutionUtils.DisposeDataExtensionObject<T>(ref obj, objectType, this.m_dataSet.Name.MarkAsPrivate(), this.m_executionMetrics, metricType);
		}

		private DataSourceErrorInspector CreateErrorInspector()
		{
			return null;
		}
	}
}
