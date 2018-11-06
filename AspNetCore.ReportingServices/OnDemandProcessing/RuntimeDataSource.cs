using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeDataSource
	{
		protected readonly OnDemandProcessingContext m_odpContext;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

		protected readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource m_dataSource;

		protected List<RuntimeDataSet> m_runtimeDataSets;

		private bool m_canAbort;

		protected TimeMetric m_totalDurationFromExistingQuery;

		protected DataProcessingMetrics m_executionMetrics;

		private readonly bool m_mergeTran;

		protected IDbConnection m_connection;

		protected AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.TransactionInfo m_transaction;

		private bool m_needToCloseConnection;

		private bool m_isGlobalConnection;

		private bool m_isTransactionOwner;

		private bool m_isGlobalTransaction;

		protected bool m_useConcurrentDataSetProcessing;

		internal DataProcessingMetrics ExecutionMetrics
		{
			get
			{
				return this.m_executionMetrics;
			}
		}

		internal abstract bool NoRows
		{
			get;
		}

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.Report ReportDefinition
		{
			get
			{
				return this.m_report;
			}
		}

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource DataSourceDefinition
		{
			get
			{
				return this.m_dataSource;
			}
		}

		protected OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_odpContext;
			}
		}

		protected virtual bool AllowConcurrentProcessing
		{
			get
			{
				return false;
			}
		}

		protected virtual bool NeedsExecutionLogging
		{
			get
			{
				return true;
			}
		}

		protected virtual bool CreatesDataChunks
		{
			get
			{
				return false;
			}
		}

		protected RuntimeDataSource(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext processingContext, bool mergeTransactions)
		{
			this.m_report = report;
			this.m_dataSource = dataSource;
			this.m_odpContext = processingContext;
			this.m_runtimeDataSets = null;
			this.m_mergeTran = mergeTransactions;
			this.m_executionMetrics = new DataProcessingMetrics(this.m_odpContext.JobContext, this.m_odpContext.ExecutionLogContext);
			Global.Tracer.Assert(this.m_dataSource.Name != null, "The name of a data source cannot be null.");
		}

		internal virtual void Abort()
		{
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Abort handler called. CanAbort = {1}.", this.m_dataSource.Name, this.m_canAbort);
			}
			if (this.m_canAbort && this.m_runtimeDataSets != null)
			{
				int count = this.m_runtimeDataSets.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_runtimeDataSets[i].Abort();
				}
			}
		}

		internal void EraseDataChunk()
		{
			Global.Tracer.Assert(this.CreatesDataChunks, "EraseDataChunk is invalid for the current RuntimeDataSource implementation.");
			if (this.m_runtimeDataSets != null)
			{
				foreach (RuntimeDataSet runtimeDataSet in this.m_runtimeDataSets)
				{
					runtimeDataSet.EraseDataChunk();
				}
			}
		}

		protected bool InitializeDataSource(ExecutedQuery existingQuery)
		{
			if (this.m_dataSource.DataSets != null && 0 < this.m_dataSource.DataSets.Count)
			{
				this.m_connection = null;
				this.m_transaction = null;
				this.m_needToCloseConnection = false;
				this.m_isGlobalConnection = false;
				this.m_isTransactionOwner = false;
				this.m_isGlobalTransaction = false;
				this.m_runtimeDataSets = this.CreateRuntimeDataSets();
				if (0 >= this.m_runtimeDataSets.Count)
				{
					return false;
				}
				this.m_canAbort = true;
				this.m_odpContext.CheckAndThrowIfAborted();
				this.m_useConcurrentDataSetProcessing = (this.m_runtimeDataSets.Count > 1 && this.AllowConcurrentProcessing);
				if (!this.m_dataSource.IsArtificialForSharedDataSets)
				{
					if (existingQuery != null)
					{
						this.InitializeFromExistingQuery(existingQuery);
					}
					else
					{
						this.OpenInitialConnectionAndTransaction();
					}
				}
				return true;
			}
			return false;
		}

		protected void TeardownDataSource()
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Processing of all data sets completed.", this.m_dataSource.Name.MarkAsModelInfo());
			this.m_odpContext.CheckAndThrowIfAborted();
			this.ComputeAndUpdateRowCounts();
			this.CommitTransaction();
		}

		protected void HandleException(Exception e)
		{
			if (!(e is ProcessingAbortedException))
			{
				Global.Tracer.Trace(TraceLevel.Error, "Data source '{0}': An error has occurred. Details: {1}", this.m_dataSource.Name.MarkAsModelInfo(), e.ToString());
			}
			this.RollbackTransaction();
		}

		protected virtual void FinalCleanup()
		{
			this.CloseConnection();
		}

		private void CloseConnection()
		{
			if (this.m_needToCloseConnection)
			{
				RuntimeDataSource.CloseConnection(this.m_connection, this.m_dataSource, this.m_odpContext, this.m_executionMetrics);
				if (this.NeedsExecutionLogging && this.m_odpContext.ExecutionLogContext != null)
				{
					int num = (this.m_runtimeDataSets != null) ? this.m_runtimeDataSets.Count : 0;
					List<DataProcessingMetrics> list = new List<DataProcessingMetrics>();
					for (int i = 0; i < num; i++)
					{
						if (this.m_runtimeDataSets[i].IsConnectionOwner)
						{
							this.m_odpContext.ExecutionLogContext.AddDataSourceParallelExecutionMetrics(this.m_dataSource.Name, this.m_dataSource.DataSourceReference, this.m_dataSource.Type, this.m_runtimeDataSets[i].DataSetExecutionMetrics);
						}
						else
						{
							list.Add(this.m_runtimeDataSets[i].DataSetExecutionMetrics);
						}
					}
					this.m_odpContext.ExecutionLogContext.AddDataSourceMetrics(this.m_dataSource.Name, this.m_dataSource.DataSourceReference, this.m_dataSource.Type, this.m_executionMetrics, list.ToArray());
				}
			}
			this.m_connection = null;
		}

		internal void RecordTimeDataRetrieval()
		{
			this.m_odpContext.ExecutionLogContext.AddDataProcessingTime(this.m_executionMetrics.TotalDuration);
		}

		internal static DataSourceInfo GetDataSourceInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext processingContext)
		{
			if (processingContext.CreateAndSetupDataExtensionFunction.MustResolveSharedDataSources)
			{
				return dataSource.GetDataSourceInfo(processingContext);
			}
			return null;
		}

		private void RollbackTransaction()
		{
			if (this.m_transaction != null)
			{
				this.m_transaction.RollbackRequired = true;
				if (this.m_isGlobalTransaction)
				{
					this.m_odpContext.GlobalDataSourceInfo.Remove(this.m_dataSource.Name);
				}
				if (this.m_isTransactionOwner)
				{
					Global.Tracer.Trace(TraceLevel.Error, "Data source '{0}': Rolling the transaction back.", this.m_dataSource.Name.MarkAsModelInfo());
					try
					{
						this.m_transaction.Transaction.Rollback();
					}
					catch (Exception innerException)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorRollbackTransaction, innerException, this.m_dataSource.Name.MarkAsModelInfo());
					}
				}
				this.m_transaction = null;
			}
		}

		private void CommitTransaction()
		{
			if (this.m_isTransactionOwner)
			{
				if (this.m_isGlobalTransaction)
				{
					if (this.m_isGlobalConnection)
					{
						this.m_needToCloseConnection = false;
					}
				}
				else
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Committing transaction.", this.m_dataSource.Name.MarkAsModelInfo());
					try
					{
						this.m_transaction.Transaction.Commit();
					}
					catch (Exception innerException)
					{
						throw new ReportProcessingException(ErrorCode.rsErrorCommitTransaction, innerException, this.m_dataSource.Name.MarkAsModelInfo());
					}
				}
				this.m_isTransactionOwner = false;
			}
			this.m_transaction = null;
		}

		private void ComputeAndUpdateRowCounts()
		{
			for (int i = 0; i < this.m_runtimeDataSets.Count; i++)
			{
				this.m_executionMetrics.AddRowCount(this.m_runtimeDataSets[i].NumRowsRead);
			}
			IJobContext jobContext = this.m_odpContext.JobContext;
			if (this.NeedsExecutionLogging && jobContext != null)
			{
				lock (jobContext.SyncRoot)
				{
					jobContext.RowCount += this.m_executionMetrics.TotalRowsRead;
				}
			}
		}

		private void InitializeFromExistingQuery(ExecutedQuery query)
		{
			query.ReleaseOwnership(ref this.m_connection);
			this.m_needToCloseConnection = true;
			this.MergeAutoCollationSettings(this.m_connection);
			this.m_executionMetrics.Add(DataProcessingMetrics.MetricType.OpenConnection, query.ExecutionMetrics.OpenConnectionDurationMs);
			this.m_executionMetrics.ConnectionFromPool = query.ExecutionMetrics.ConnectionFromPool;
			this.m_totalDurationFromExistingQuery = new TimeMetric(query.ExecutionMetrics.TotalDuration);
		}

		protected virtual void OpenInitialConnectionAndTransaction()
		{
			if (this.m_dataSource.Transaction && this.m_mergeTran)
			{
				AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfo dataSourceInfo = this.m_odpContext.GlobalDataSourceInfo[this.m_dataSource.Name];
				if (dataSourceInfo != null)
				{
					this.m_connection = dataSourceInfo.Connection;
					this.m_transaction = dataSourceInfo.TransactionInfo;
				}
			}
			Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Transaction = {1}, MergeTran = {2}, NumDataSets = {3}", this.m_dataSource.Name.MarkAsModelInfo(), this.m_dataSource.Transaction, this.m_mergeTran, this.m_runtimeDataSets.Count);
			if (this.m_connection == null)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = this.m_runtimeDataSets[0].DataSet;
				this.m_connection = RuntimeDataSource.OpenConnection(this.m_dataSource, dataSet, this.m_odpContext, this.m_executionMetrics);
				this.m_needToCloseConnection = true;
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Created a connection.", this.m_dataSource.Name.MarkAsModelInfo());
			}
			bool flag = false;
			if (this.m_dataSource.Transaction)
			{
				if (this.m_transaction == null)
				{
					IDbTransaction transaction = this.m_connection.BeginTransaction();
					Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Begun a transaction.", this.m_dataSource.Name.MarkAsModelInfo());
					this.m_transaction = new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.TransactionInfo(transaction);
					this.m_isTransactionOwner = true;
				}
				IDbTransactionExtension dbTransactionExtension = this.m_transaction.Transaction as IDbTransactionExtension;
				flag = (dbTransactionExtension != null && dbTransactionExtension.AllowMultiConnection);
				this.m_useConcurrentDataSetProcessing &= flag;
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': TransactionCanSpanConnections = {1}, ConcurrentDataSets = {2}", this.m_dataSource.Name.MarkAsModelInfo(), flag, this.m_useConcurrentDataSetProcessing);
			}
			this.MergeAutoCollationSettings(this.m_connection);
			if (this.m_isTransactionOwner && this.m_report.SubReportMergeTransactions && !this.m_odpContext.ProcessReportParameters)
			{
				IDbConnection connection;
				if (flag)
				{
					connection = null;
					this.m_isGlobalConnection = false;
				}
				else
				{
					connection = this.m_connection;
					this.m_isGlobalConnection = true;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Storing trans+conn into GlobalDataSourceInfo. CloseConnection = {1}.", this.m_dataSource.Name.MarkAsModelInfo(), this.m_needToCloseConnection);
				DataSourceInfo dataSourceInfo2 = RuntimeDataSource.GetDataSourceInfo(this.m_dataSource, this.m_odpContext);
				this.m_odpContext.GlobalDataSourceInfo.Add(this.m_dataSource, connection, this.m_transaction, dataSourceInfo2);
				this.m_isGlobalTransaction = true;
			}
		}

		private void MergeAutoCollationSettings(IDbConnection connection)
		{
			if (connection is IDbCollationProperties && this.m_dataSource.AnyActiveDataSetNeedsAutoDetectCollation())
			{
				try
				{
					string cultureName = default(string);
					bool caseSensitive = default(bool);
					bool accentSensitive = default(bool);
					bool kanatypeSensitive = default(bool);
					bool widthSensitive = default(bool);
					if (((IDbCollationProperties)connection).GetCollationProperties(out cultureName, out caseSensitive, out accentSensitive, out kanatypeSensitive, out widthSensitive))
					{
						this.m_dataSource.MergeCollationSettingsForAllDataSets(this.m_odpContext.ErrorContext, cultureName, caseSensitive, accentSensitive, kanatypeSensitive, widthSensitive);
					}
				}
				catch (Exception ex)
				{
					this.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsCollationDetectionFailed, Severity.Warning, ObjectType.DataSource, this.m_dataSource.Name, "Collation", ex.ToString());
				}
			}
		}

		protected abstract List<RuntimeDataSet> CreateRuntimeDataSets();

        internal static IDbConnection OpenConnection(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSourceObj, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSetObj, OnDemandProcessingContext pc, DataProcessingMetrics metrics)
        {
            IDbConnection dbConnection = null;
            try
            {
                metrics.StartTimer(DataProcessingMetrics.MetricType.OpenConnection);
                DataSourceInfo dataSourceInfo = null;
                string text = dataSourceObj.ResolveConnectionString(pc, out dataSourceInfo);
                if (pc.UseVerboseExecutionLogging)
                {
                    metrics.ResolvedConnectionString = text;
                }
                //if (pc.CreateAndSetupDataExtensionFunction.MustResolveSharedDataSources)
                //{

                //}

                return pc.CreateAndSetupDataExtensionFunction.OpenDataSourceExtensionConnection(dataSourceObj, text, dataSourceInfo, dataSetObj.Name);
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
                throw new ReportProcessingException(ErrorCode.rsErrorOpeningConnection, ex2, dataSourceObj.Name);
            }
            finally
            {
                long num = metrics.RecordTimerMeasurementWithUpdatedTotal(DataProcessingMetrics.MetricType.OpenConnection);
                Global.Tracer.Trace(TraceLevel.Verbose, "Opening a connection for DataSource: {0} took {1} ms.", dataSourceObj.Name.MarkAsModelInfo(), num);
            }
        }

		internal static void CloseConnection(IDbConnection connection, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext odpContext, DataProcessingMetrics executionMetrics)
		{
			try
			{
				DataSourceInfo dataSourceInfo = RuntimeDataSource.GetDataSourceInfo(dataSource, odpContext);
				odpContext.CreateAndSetupDataExtensionFunction.CloseConnection(connection, dataSource, dataSourceInfo);
			}
			catch (Exception innerException)
			{
				throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException, dataSource.Name);
			}
		}

		protected bool CheckNoRows(RuntimeDataSet runtimeDataSet)
		{
			if (runtimeDataSet == null)
			{
				return false;
			}
			return runtimeDataSet.NoRows;
		}
	}
}
