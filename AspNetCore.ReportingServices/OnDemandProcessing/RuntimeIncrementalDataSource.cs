using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeIncrementalDataSource : RuntimeDataSource
	{
		protected readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		protected abstract RuntimeIncrementalDataSet RuntimeDataSet
		{
			get;
		}

		internal override bool NoRows
		{
			get
			{
				return base.CheckNoRows(this.RuntimeDataSet);
			}
		}

		protected RuntimeIncrementalDataSource(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, OnDemandProcessingContext odpContext)
			: base(report, dataSet.DataSource, odpContext, false)
		{
			this.m_dataSet = dataSet;
		}

		internal void Initialize()
		{
			ExecutedQuery executedQuery = null;
			try
			{
				ExecutedQueryCache executedQueryCache = base.m_odpContext.StateManager.ExecutedQueryCache;
				if (executedQueryCache != null)
				{
					executedQueryCache.Extract(this.m_dataSet, out executedQuery);
				}
				base.InitializeDataSource(executedQuery);
				this.InitializeDataSet(executedQuery);
			}
			catch (Exception e)
			{
				base.HandleException(e);
				this.FinalCleanup();
				if (executedQuery != null)
				{
					executedQuery.Close();
				}
				throw;
			}
		}

		internal override void Abort()
		{
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Abort handler called.", base.m_dataSource.Name.MarkAsModelInfo());
			}
			if (this.RuntimeDataSet != null)
			{
				this.RuntimeDataSet.Abort();
			}
		}

		protected void InitializeDataSet(ExecutedQuery existingQuery)
		{
			this.RuntimeDataSet.InitProcessingParams(base.m_connection, base.m_transaction);
			this.RuntimeDataSet.Initialize(existingQuery);
		}

		internal void Teardown()
		{
			try
			{
				this.TeardownDataSet();
				base.TeardownDataSource();
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

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			if (this.RuntimeDataSet != null)
			{
				TimeMetric timeMetric = this.RuntimeDataSet.DataSetExecutionMetrics.TotalDuration;
				if (base.m_totalDurationFromExistingQuery != null)
				{
					timeMetric = new TimeMetric(timeMetric);
					timeMetric.Subtract(base.m_totalDurationFromExistingQuery);
				}
				base.m_odpContext.ExecutionLogContext.AddDataProcessingTime(timeMetric);
				base.m_executionMetrics.Add(this.RuntimeDataSet.DataSetExecutionMetrics);
			}
		}

		protected virtual void TeardownDataSet()
		{
			this.RuntimeDataSet.Teardown();
		}

		internal void RecordSkippedRowCount(long rowCount)
		{
			this.RuntimeDataSet.RecordSkippedRowCount(rowCount);
		}
	}
}
