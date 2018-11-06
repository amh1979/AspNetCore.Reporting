using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class RetrievalManager
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

		private DataSetDefinition m_dataSetDefinition;

		private bool m_noRows;

		private OnDemandProcessingContext m_odpContext;

		private List<RuntimeAtomicDataSource> m_runtimeDataSources = new List<RuntimeAtomicDataSource>();

		internal bool NoRows
		{
			get
			{
				return this.m_noRows;
			}
		}

		internal RetrievalManager(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext context)
		{
			this.m_report = report;
			this.m_odpContext = context;
		}

		internal RetrievalManager(DataSetDefinition dataSetDefinition, OnDemandProcessingContext context)
		{
			this.m_dataSetDefinition = dataSetDefinition;
			this.m_odpContext = context;
		}

		internal void FetchParameterData(ReportParameterDataSetCache aCache, int aDataSourceIndex, int aDataSetIndex)
		{
			RuntimeDataSourceParameters item = new RuntimeDataSourceParameters(this.m_report, this.m_report.DataSources[aDataSourceIndex], this.m_odpContext, aDataSetIndex, aCache);
			this.m_runtimeDataSources.Add(item);
			this.FetchData();
		}

		internal bool FetchSharedDataSet(ParameterInfoCollection parameters)
		{
			if (parameters != null && parameters.Count != 0)
			{
				this.m_odpContext.ReportObjectModel.ParametersImpl.Clear();
				this.m_odpContext.ReportObjectModel.Initialize(parameters);
			}
			if (this.m_odpContext.ExternalDataSetContext.CachedDataChunkName == null)
			{
				return this.FetchSharedDataSetLive();
			}
			return this.FetchSharedDataSetCached();
		}

		private bool FetchSharedDataSetCached()
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet(this.m_dataSetDefinition.DataSetCore);
			DataSetInstance dataSetInstance = new DataSetInstance(dataSet);
			ProcessingDataReader processingDataReader = new ProcessingDataReader(dataSetInstance, dataSet, this.m_odpContext, true);
			IRowConsumer consumerRequest = this.m_odpContext.ExternalDataSetContext.ConsumerRequest;
			consumerRequest.SetProcessingDataReader(processingDataReader);
			long num = 0L;
			try
			{
				while (processingDataReader.GetNextRow())
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow underlyingRecordRowObject = processingDataReader.GetUnderlyingRecordRowObject();
					consumerRequest.NextRow(underlyingRecordRowObject);
					num++;
				}
			}
			finally
			{
				if (this.m_odpContext.JobContext != null)
				{
					lock (this.m_odpContext.JobContext.SyncRoot)
					{
						this.m_odpContext.JobContext.RowCount += num;
					}
				}
			}
			return true;
		}

		private bool FetchSharedDataSetLive()
		{
			this.m_runtimeDataSources.Add(new RuntimeDataSourceSharedDataSet(this.m_dataSetDefinition, this.m_odpContext));
			try
			{
				return this.FetchData();
			}
			catch
			{
				this.m_runtimeDataSources[0].EraseDataChunk();
				throw;
			}
			finally
			{
				this.FinallyBlockForDataSetExecution();
			}
		}

		internal bool PrefetchData(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, ParameterInfoCollection parameters, bool mergeTran)
		{
			if (this.m_report.DataSourceCount == 0)
			{
				return true;
			}
			try
			{
				bool flag = true;
				for (int i = 0; i < this.m_report.DataSourceCount; i++)
				{
					this.m_runtimeDataSources.Add(new RuntimeDataSourcePrefetch(this.m_report, reportInstance, this.m_report.DataSources[i], this.m_odpContext, mergeTran));
				}
				flag &= this.FetchData();
				if (this.m_report.ParametersNotUsedInQuery && this.m_odpContext.ErrorSavingSnapshotData)
				{
					for (int j = 0; j < parameters.Count; j++)
					{
						parameters[j].UsedInQuery = true;
					}
					return false;
				}
				return flag;
			}
			catch
			{
				foreach (RuntimeAtomicDataSource runtimeDataSource in this.m_runtimeDataSources)
				{
					runtimeDataSource.EraseDataChunk();
				}
				throw;
			}
			finally
			{
				this.FinallyBlockForDataSetExecution();
			}
		}

		private void FinallyBlockForDataSetExecution()
		{
			this.m_noRows = true;
			DataProcessingMetrics dataProcessingMetrics = null;
			foreach (RuntimeAtomicDataSource runtimeDataSource in this.m_runtimeDataSources)
			{
				if (dataProcessingMetrics == null || runtimeDataSource.ExecutionMetrics.TotalDurationMs > dataProcessingMetrics.TotalDurationMs)
				{
					dataProcessingMetrics = runtimeDataSource.ExecutionMetrics;
				}
				if (!runtimeDataSource.NoRows)
				{
					this.m_noRows = false;
				}
			}
			if (dataProcessingMetrics != null)
			{
				this.m_odpContext.ExecutionLogContext.AddDataProcessingTime(dataProcessingMetrics.TotalDuration);
			}
			this.m_runtimeDataSources.Clear();
		}

		private bool FetchData()
		{
			EventHandler eventHandler = null;
			int count = this.m_runtimeDataSources.Count;
			ThreadSet threadSet = null;
			try
			{
				if (this.m_odpContext.AbortInfo != null)
				{
					eventHandler = this.AbortHandler;
					this.m_odpContext.AbortInfo.ProcessingAbortEvent += eventHandler;
				}
				if (count != 0)
				{
					RuntimeAtomicDataSource @object;
					if (count > 1)
					{
						threadSet = new ThreadSet(count - 1);
						try
						{
							for (int i = 1; i < count; i++)
							{
								@object = this.m_runtimeDataSources[i];
								threadSet.TryQueueWorkItem(this.m_odpContext, @object.ProcessConcurrent);
							}
						}
						catch (Exception e)
						{
							if (this.m_odpContext.AbortInfo != null)
							{
								this.m_odpContext.AbortInfo.SetError(e, this.m_odpContext.ProcessingAbortItemUniqueIdentifier);
							}
							throw;
						}
					}
					@object = this.m_runtimeDataSources[0];
					@object.ProcessConcurrent(null);
				}
			}
			finally
			{
				if (threadSet != null && count > 1)
				{
					threadSet.WaitForCompletion();
					threadSet.Dispose();
				}
				if (eventHandler != null)
				{
					this.m_odpContext.AbortInfo.ProcessingAbortEvent -= eventHandler;
				}
			}
			this.m_odpContext.CheckAndThrowIfAborted();
			return true;
		}

		private void AbortHandler(object sender, EventArgs e)
		{
			if (e is ProcessingAbortEventArgs && ((ProcessingAbortEventArgs)e).UniqueName == this.m_odpContext.ProcessingAbortItemUniqueIdentifier)
			{
				if (Global.Tracer.TraceInfo)
				{
					Global.Tracer.Trace(TraceLevel.Info, "DataPrefetch abort handler called for Report with ID={0}. Aborting data sources ...", this.m_odpContext.ProcessingAbortItemUniqueIdentifier);
				}
				int count = this.m_runtimeDataSources.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_runtimeDataSources[i].Abort();
				}
			}
		}
	}
}
