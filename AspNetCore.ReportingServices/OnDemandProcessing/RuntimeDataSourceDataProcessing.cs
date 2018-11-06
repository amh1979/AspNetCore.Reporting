using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class RuntimeDataSourceDataProcessing : RuntimeAtomicDataSource
	{
		protected readonly DataSet m_dataSet;

		private RuntimeOnDemandDataSet m_runtimeDataSet;

		internal RuntimeOnDemandDataSet RuntimeDataSet
		{
			get
			{
				return this.m_runtimeDataSet;
			}
		}

		internal override bool NoRows
		{
			get
			{
				return base.CheckNoRows(this.m_runtimeDataSet);
			}
		}

		internal RuntimeDataSourceDataProcessing(DataSet dataSet, OnDemandProcessingContext processingContext)
			: base(processingContext.ReportDefinition, dataSet.DataSource, processingContext, false)
		{
			this.m_dataSet = dataSet;
		}

		internal void ProcessSingleOdp()
		{
			ExecutedQuery executedQuery = null;
			try
			{
				ExecutedQueryCache executedQueryCache = base.m_odpContext.StateManager.ExecutedQueryCache;
				if (executedQueryCache != null)
				{
					executedQueryCache.Extract(this.m_dataSet, out executedQuery);
				}
				if (base.InitializeDataSource(executedQuery))
				{
					this.m_runtimeDataSet.InitProcessingParams(base.m_connection, base.m_transaction);
					this.m_runtimeDataSet.ProcessInline(executedQuery);
					base.m_executionMetrics.Add(this.m_runtimeDataSet.DataSetExecutionMetrics);
					if (base.m_totalDurationFromExistingQuery != null)
					{
						base.m_executionMetrics.TotalDuration.Subtract(base.m_totalDurationFromExistingQuery);
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
				if (executedQuery != null)
				{
					executedQuery.Close();
				}
			}
		}

		protected override List<RuntimeDataSet> CreateRuntimeDataSets()
		{
			this.m_runtimeDataSet = this.CreateRuntimeDataSet();
			List<RuntimeDataSet> list = new List<RuntimeDataSet>(1);
			list.Add(this.m_runtimeDataSet);
			return list;
		}

		protected abstract RuntimeOnDemandDataSet CreateRuntimeDataSet();
	}
}
