using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeAtomicDataSource : RuntimeDataSource
	{
		protected RuntimeAtomicDataSource(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, OnDemandProcessingContext processingContext, bool mergeTransactions)
			: base(report, dataSource, processingContext, mergeTransactions)
		{
		}

		internal void ProcessConcurrent(object threadSet)
		{
			try
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Thread has started processing data source '{0}'", base.DataSourceDefinition.Name.MarkAsModelInfo());
				}
				this.Process(false);
			}
			catch (ProcessingAbortedException)
			{
				if (Global.Tracer.TraceWarning)
				{
					Global.Tracer.Trace(TraceLevel.Warning, "Data source '{0}': Report processing has been aborted.", base.DataSourceDefinition.Name.MarkAsModelInfo());
				}
				if (!base.m_odpContext.StreamingMode)
				{
					goto end_IL_0043;
				}
				throw;
				end_IL_0043:;
			}
			catch (Exception ex2)
			{
				if (Global.Tracer.TraceError)
				{
					Global.Tracer.Trace(TraceLevel.Error, "An exception has occurred in data source '{0}'. Details: {1}", base.DataSourceDefinition.Name.MarkAsModelInfo(), ex2.ToString());
				}
				if (base.OdpContext.AbortInfo != null)
				{
					base.OdpContext.AbortInfo.SetError(ex2, base.OdpContext.ProcessingAbortItemUniqueIdentifier);
					goto end_IL_008c;
				}
				throw;
				end_IL_008c:;
			}
			finally
			{
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Processing of data source '{0}' completed.", base.DataSourceDefinition.Name.MarkAsModelInfo());
				}
				ThreadSet threadSet2 = threadSet as ThreadSet;
				if (threadSet2 != null)
				{
					threadSet2.ThreadCompleted();
				}
			}
		}

		private void Process(bool fromOdp)
		{
			try
			{
				if (base.InitializeDataSource(null))
				{
					if (base.m_useConcurrentDataSetProcessing)
					{
						this.ExecuteParallelDataSets();
					}
					else
					{
						this.ExecuteSequentialDataSets();
					}
					base.TeardownDataSource();
				}
			}
			catch (Exception e)
			{
				base.HandleException(e);
				throw;
			}
			finally
			{
				this.FinalCleanup();
			}
		}

		private void ExecuteSequentialDataSets()
		{
			for (int i = 0; i < base.m_runtimeDataSets.Count; i++)
			{
				base.m_odpContext.CheckAndThrowIfAborted();
				RuntimeAtomicDataSet runtimeAtomicDataSet = (RuntimeAtomicDataSet)base.m_runtimeDataSets[i];
				runtimeAtomicDataSet.InitProcessingParams(base.m_connection, base.m_transaction);
				runtimeAtomicDataSet.ProcessConcurrent(null);
				base.m_executionMetrics.Add(runtimeAtomicDataSet.DataSetExecutionMetrics);
			}
		}

		private void ExecuteParallelDataSets()
		{
			ThreadSet threadSet = new ThreadSet(base.m_runtimeDataSets.Count - 1);
			try
			{
				for (int i = 1; i < base.m_runtimeDataSets.Count; i++)
				{
					RuntimeAtomicDataSet runtimeAtomicDataSet = (RuntimeAtomicDataSet)base.m_runtimeDataSets[i];
					runtimeAtomicDataSet.InitProcessingParams(null, base.m_transaction);
					threadSet.TryQueueWorkItem(base.m_odpContext, runtimeAtomicDataSet.ProcessConcurrent);
				}
				RuntimeAtomicDataSet runtimeAtomicDataSet2 = (RuntimeAtomicDataSet)base.m_runtimeDataSets[0];
				runtimeAtomicDataSet2.InitProcessingParams(base.m_connection, base.m_transaction);
				runtimeAtomicDataSet2.ProcessConcurrent(null);
			}
			catch (Exception e)
			{
				if (base.m_odpContext.AbortInfo != null)
				{
					base.m_odpContext.AbortInfo.SetError(e, base.m_odpContext.ProcessingAbortItemUniqueIdentifier);
				}
				throw;
			}
			finally
			{
				threadSet.WaitForCompletion();
				threadSet.Dispose();
			}
			if (this.NeedsExecutionLogging && base.m_odpContext.JobContext != null)
			{
				DataProcessingMetrics dataProcessingMetrics = null;
				for (int j = 0; j < base.m_runtimeDataSets.Count; j++)
				{
					RuntimeDataSet runtimeDataSet = base.m_runtimeDataSets[j];
					if (dataProcessingMetrics == null || runtimeDataSet.DataSetExecutionMetrics.TotalDurationMs > dataProcessingMetrics.TotalDurationMs)
					{
						dataProcessingMetrics = runtimeDataSet.DataSetExecutionMetrics;
					}
				}
				base.m_executionMetrics.Add(dataProcessingMetrics);
			}
		}
	}
}
