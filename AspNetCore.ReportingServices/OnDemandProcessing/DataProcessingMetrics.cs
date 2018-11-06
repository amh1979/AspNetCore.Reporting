using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Internal;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class DataProcessingMetrics
	{
		internal enum MetricType
		{
			ExecuteReader,
			DataReaderMapping,
			Query,
			OpenConnection,
			DisposeDataReader,
			CancelCommand
		}

		private const long DurationNotMeasured = -2L;

		private IJobContext m_jobContext;

		private Timer[] m_timers;

		private TimeMetric m_totalTimeMetric;

		private long m_totalRowsRead;

		private long m_totalRowsSkipped;

		private long m_queryDurationMs;

		private long m_dataReaderMappingDurationMs;

		private long m_executeReaderDurationMs;

		private long m_openConnectionDurationMs;

		private long m_disposeDataReaderDurationMs;

		private long m_cancelCommandDurationMs = -2L;

		private string m_commandText;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_specificDataSet;

		private IDataParameter[] m_startAtParameters;

		private IDataParameterCollection m_queryParameters;

		private string m_resolvedConnectionString;

		private bool? m_connectionFromPool = null;

		internal long TotalRowsRead
		{
			get
			{
				return this.m_totalRowsRead;
			}
		}

		internal long TotalRowsSkipped
		{
			get
			{
				return this.m_totalRowsSkipped;
			}
		}

		internal long TotalDurationMs
		{
			get
			{
				if (this.m_totalTimeMetric == null)
				{
					return 0L;
				}
				return this.m_totalTimeMetric.TotalDurationMs;
			}
		}

		internal TimeMetric TotalDuration
		{
			get
			{
				return this.m_totalTimeMetric;
			}
		}

		internal long QueryDurationMs
		{
			get
			{
				return this.m_queryDurationMs;
			}
		}

		internal long ExecuteReaderDurationMs
		{
			get
			{
				return this.m_executeReaderDurationMs;
			}
		}

		internal long DataReaderMappingDurationMs
		{
			get
			{
				return this.m_dataReaderMappingDurationMs;
			}
		}

		internal long OpenConnectionDurationMs
		{
			get
			{
				return this.m_openConnectionDurationMs;
			}
		}

		internal long DisposeDataReaderDurationMs
		{
			get
			{
				return this.m_disposeDataReaderDurationMs;
			}
		}

		internal long CancelCommandDurationMs
		{
			get
			{
				return this.m_cancelCommandDurationMs;
			}
		}

		public string CommandText
		{
			get
			{
				return this.m_commandText;
			}
			set
			{
				this.m_commandText = value;
			}
		}

		internal string ResolvedConnectionString
		{
			get
			{
				return this.m_resolvedConnectionString;
			}
			set
			{
				this.m_resolvedConnectionString = value;
			}
		}

		internal bool? ConnectionFromPool
		{
			get
			{
				return this.m_connectionFromPool;
			}
			set
			{
				this.m_connectionFromPool = value;
			}
		}

		internal DataProcessingMetrics(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, IJobContext jobContext, ExecutionLogContext executionLogContext)
			: this(jobContext, executionLogContext)
		{
			this.m_specificDataSet = dataSet;
		}

		internal DataProcessingMetrics(IJobContext jobContext, ExecutionLogContext executionLogContext)
		{
			this.m_jobContext = jobContext;
			if (jobContext != null)
			{
				this.m_timers = new Timer[6];
				this.m_totalTimeMetric = executionLogContext.CreateDataRetrievalWorkerTimer();
			}
			else
			{
				this.m_timers = null;
			}
		}

		internal void Add(DataProcessingMetrics metrics)
		{
			if (metrics != null)
			{
				if (this.m_totalTimeMetric != null)
				{
					this.m_totalTimeMetric.Add(metrics.m_totalTimeMetric);
				}
				this.Add(MetricType.ExecuteReader, metrics.m_executeReaderDurationMs);
				this.Add(MetricType.DataReaderMapping, metrics.m_dataReaderMappingDurationMs);
				this.Add(MetricType.Query, metrics.m_queryDurationMs);
				this.Add(MetricType.OpenConnection, metrics.m_openConnectionDurationMs);
				this.Add(MetricType.DisposeDataReader, metrics.m_disposeDataReaderDurationMs);
				this.Add(MetricType.CancelCommand, metrics.m_cancelCommandDurationMs);
			}
		}

		internal long Add(MetricType type, Timer timer)
		{
			if (timer == null)
			{
				return -1L;
			}
			long num = timer.ElapsedTimeMs();
			this.Add(type, num);
			return num;
		}

		internal void Add(MetricType type, long elapsedTimeMs)
		{
			switch (type)
			{
			case MetricType.ExecuteReader:
				this.Add(ref this.m_executeReaderDurationMs, elapsedTimeMs);
				break;
			case MetricType.DataReaderMapping:
				this.Add(ref this.m_dataReaderMappingDurationMs, elapsedTimeMs);
				break;
			case MetricType.Query:
				this.Add(ref this.m_queryDurationMs, elapsedTimeMs);
				break;
			case MetricType.OpenConnection:
				this.Add(ref this.m_openConnectionDurationMs, elapsedTimeMs);
				break;
			case MetricType.DisposeDataReader:
				this.Add(ref this.m_disposeDataReaderDurationMs, elapsedTimeMs);
				break;
			case MetricType.CancelCommand:
				this.Add(ref this.m_cancelCommandDurationMs, elapsedTimeMs);
				break;
			}
		}

		private void Add(ref long currentDurationMs, long elapsedTimeMs)
		{
			if (currentDurationMs == -2)
			{
				currentDurationMs = 0L;
			}
			currentDurationMs += ExecutionLogContext.TimerMeasurementAdjusted(elapsedTimeMs);
		}

		internal void AddRowCount(long rowCount)
		{
			this.m_totalRowsRead += rowCount;
		}

		internal void AddSkippedRowCount(long rowCount)
		{
			this.m_totalRowsSkipped += rowCount;
		}

		internal void StartTimer(MetricType type)
		{
			if (this.m_jobContext != null)
			{
				(this.m_timers[(int)type] = new Timer()).StartTimer();
			}
		}

		internal long RecordTimerMeasurement(MetricType type)
		{
			if (this.m_jobContext == null)
			{
				return 0L;
			}
			if (this.m_timers[(int)type] == null)
			{
				return 0L;
			}
			long result = this.Add(type, this.m_timers[(int)type]);
			this.m_timers[(int)type] = null;
			return result;
		}

		internal long RecordTimerMeasurementWithUpdatedTotal(MetricType type)
		{
			long num = this.RecordTimerMeasurement(type);
			if (this.m_totalTimeMetric != null && !this.m_totalTimeMetric.IsRunning)
			{
				this.m_totalTimeMetric.AddTime(num);
			}
			return num;
		}

		public void StartTotalTimer()
		{
			if (this.m_totalTimeMetric != null)
			{
				this.m_totalTimeMetric.StartTimer();
			}
		}

		public void RecordTotalTimerMeasurement()
		{
			if (this.m_totalTimeMetric != null)
			{
				this.m_totalTimeMetric.StopTimer();
			}
		}

		internal void SetStartAtParameters(IDataParameter[] startAtParameters)
		{
			this.m_startAtParameters = startAtParameters;
		}

		internal void SetQueryParameters(IDataParameterCollection queryParameters)
		{
			this.m_queryParameters = queryParameters;
		}

		internal AspNetCore.ReportingServices.Diagnostics.Internal.DataSet ToAdditionalInfoDataSet(IJobContext jobContext)
		{
			if (jobContext != null && this.m_specificDataSet != null)
			{
				AspNetCore.ReportingServices.Diagnostics.Internal.DataSet dataSet = new AspNetCore.ReportingServices.Diagnostics.Internal.DataSet();
				dataSet.Name = this.m_specificDataSet.Name;
				if (jobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
				{
					dataSet.CommandText = this.m_commandText;
					if (this.m_startAtParameters != null || this.m_queryParameters != null)
					{
						List<QueryParameter> list = new List<QueryParameter>();
						this.AddStartAtParameters(list);
						this.AddQueryParameters(list);
						if (list.Count > 0)
						{
							dataSet.QueryParameters = list;
						}
					}
				}
				dataSet.RowsRead = this.m_totalRowsRead;
				dataSet.TotalTimeDataRetrieval = this.m_totalTimeMetric.TotalDurationMs;
				if (jobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
				{
					dataSet.QueryPrepareAndExecutionTime = this.m_queryDurationMs;
				}
				dataSet.ExecuteReaderTime = this.m_executeReaderDurationMs;
				if (jobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
				{
					dataSet.DataReaderMappingTime = this.m_dataReaderMappingDurationMs;
					dataSet.DisposeDataReaderTime = this.m_disposeDataReaderDurationMs;
				}
				if (this.m_cancelCommandDurationMs != -2)
				{
					dataSet.CancelCommandTime = this.m_cancelCommandDurationMs.ToString(CultureInfo.InvariantCulture);
				}
				return dataSet;
			}
			return null;
		}

		private void AddStartAtParameters(List<QueryParameter> queryParams)
		{
			if (this.m_startAtParameters != null && this.m_startAtParameters.Length != 0)
			{
				IDataParameter[] startAtParameters = this.m_startAtParameters;
				foreach (IDataParameter dataParameter in startAtParameters)
				{
					if (dataParameter != null)
					{
						queryParams.Add(DataProcessingMetrics.CreateAdditionalInfoQueryParameter(dataParameter.ParameterName, dataParameter.Value));
					}
				}
			}
		}

		private void AddQueryParameters(List<QueryParameter> queryParams)
		{
			if (this.m_queryParameters != null)
			{
				foreach (IDataParameter queryParameter in this.m_queryParameters)
				{
					if (queryParameter != null)
					{
						IDataMultiValueParameter dataMultiValueParameter = queryParameter as IDataMultiValueParameter;
						if (dataMultiValueParameter != null)
						{
							DataProcessingMetrics.AddMultiValueQueryParameter(queryParams, dataMultiValueParameter);
						}
						else
						{
							queryParams.Add(DataProcessingMetrics.CreateAdditionalInfoQueryParameter(queryParameter.ParameterName, queryParameter.Value));
						}
					}
				}
			}
		}

		private static void AddMultiValueQueryParameter(List<QueryParameter> queryParams, IDataMultiValueParameter parameter)
		{
			if (parameter.Values != null)
			{
				object[] values = parameter.Values;
				foreach (object parameterValue in values)
				{
					queryParams.Add(DataProcessingMetrics.CreateAdditionalInfoQueryParameter(parameter.ParameterName, parameterValue));
				}
			}
		}

		private static QueryParameter CreateAdditionalInfoQueryParameter(string parameterName, object parameterValue)
		{
			QueryParameter queryParameter = new QueryParameter();
			queryParameter.Name = parameterName;
			if (parameterValue != null)
			{
				queryParameter.Value = Convert.ToString(parameterValue, CultureInfo.InvariantCulture);
				queryParameter.TypeName = parameterValue.GetType().ToString();
			}
			return queryParameter;
		}
	}
}
