using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Internal;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class ExecutionLogContext
	{
		private enum TimeMetricType
		{
			Processing,
			Rendering,
			TablixProcessing,
			DataRetrieval
		}

		private sealed class ScaleCacheInfo
		{
			private long m_childPeakMemoryUsageKB;

			private readonly int m_reportGlobalId;

			public long ChildPeakMemoryUsageKB
			{
				get
				{
					return this.m_childPeakMemoryUsageKB;
				}
				set
				{
					this.m_childPeakMemoryUsageKB = value;
				}
			}

			public int ReportGlobalId
			{
				get
				{
					return this.m_reportGlobalId;
				}
			}

			public ScaleCacheInfo(int reportGlobalId)
			{
				this.m_reportGlobalId = reportGlobalId;
				this.m_childPeakMemoryUsageKB = 0L;
			}
		}

		private const int RootScaleCacheInfoId = -2147483648;

		private long m_processingScalabilityDurationMs;

		private long m_peakGroupTreeMemoryUsageKB;

		private long m_externalImageDurationMs;

		private long m_externalImageCount;

		private long m_externalImageBytes;

		private List<Pair<string, DataProcessingMetrics>> m_dataSetMetrics = new List<Pair<string, DataProcessingMetrics>>();

		private List<DataSourceMetrics> m_dataSourceConnectionMetrics = new List<DataSourceMetrics>();

		private Timer m_externalImageTimer;

		private TimeMetricManager m_metricManager;

		private static readonly int TimeMetricCount = Enum.GetValues(typeof(TimeMetricType)).Length;

		private readonly IJobContext m_jobContext;

		private readonly Stack<ScaleCacheInfo> m_activeScaleCaches = new Stack<ScaleCacheInfo>();

		internal bool IsProcessingTimerRunning
		{
			get
			{
				if (this.m_metricManager != null)
				{
					return this.m_metricManager[0].IsRunning;
				}
				return false;
			}
		}

		internal bool IsRenderingTimerRunning
		{
			get
			{
				if (this.m_metricManager != null)
				{
					return this.m_metricManager[1].IsRunning;
				}
				return false;
			}
		}

		internal long PeakTablixProcessingMemoryUsageKB
		{
			get
			{
				Global.Tracer.Assert(this.m_activeScaleCaches.Count > 0, "Missing root of active cache tree");
				return this.m_activeScaleCaches.Peek().ChildPeakMemoryUsageKB;
			}
		}

		internal long PeakGroupTreeMemoryUsageKB
		{
			get
			{
				return this.m_peakGroupTreeMemoryUsageKB;
			}
		}

		internal long PeakProcesssingMemoryUsage
		{
			get
			{
				return this.PeakTablixProcessingMemoryUsageKB + this.PeakGroupTreeMemoryUsageKB;
			}
		}

		internal long DataProcessingDurationMsNormalized
		{
			get
			{
				return this.GetNormalizedAdjustedMetric(TimeMetricType.DataRetrieval);
			}
		}

		internal long ProcessingScalabilityDurationMsNormalized
		{
			get
			{
				return ExecutionLogContext.NormalizeCalculatedDuration(this.m_processingScalabilityDurationMs);
			}
		}

		internal long ReportRenderingDurationMsNormalized
		{
			get
			{
				return this.GetNormalizedAdjustedMetric(TimeMetricType.Rendering);
			}
		}

		internal long ReportProcessingDurationMsNormalized
		{
			get
			{
				long normalizedAdjustedMetric = this.GetNormalizedAdjustedMetric(TimeMetricType.Processing);
				long normalizedAdjustedMetric2 = this.GetNormalizedAdjustedMetric(TimeMetricType.TablixProcessing);
				return ExecutionLogContext.NormalizeCalculatedDuration(normalizedAdjustedMetric + normalizedAdjustedMetric2);
			}
		}

		internal long ExternalImageDurationMs
		{
			get
			{
				return this.m_externalImageDurationMs;
			}
		}

		internal long ExternalImageCount
		{
			get
			{
				return this.m_externalImageCount;
			}
			set
			{
				this.m_externalImageCount = value;
			}
		}

		internal long ExternalImageBytes
		{
			get
			{
				return this.m_externalImageBytes;
			}
			set
			{
				this.m_externalImageBytes = value;
			}
		}

		internal List<Pair<string, DataProcessingMetrics>> DataSetMetrics
		{
			get
			{
				return this.m_dataSetMetrics;
			}
		}

		internal List<DataSourceMetrics> DataSourceConnectionMetrics
		{
			get
			{
				return this.m_dataSourceConnectionMetrics;
			}
		}

		internal ExecutionLogContext(IJobContext jobContext)
		{
			this.m_activeScaleCaches.Push(new ScaleCacheInfo(-2147483648));
			this.m_jobContext = jobContext;
			if (this.m_jobContext != null)
			{
				this.m_metricManager = new TimeMetricManager(ExecutionLogContext.TimeMetricCount);
			}
		}

		public static long TimerMeasurementAdjusted(long durationMs)
		{
			return Math.Max(0L, durationMs);
		}

		public static long NormalizeCalculatedDuration(long durationMs)
		{
			return Math.Max(-1L, durationMs);
		}

		internal List<Connection> GetConnectionMetrics()
		{
			if (this.DataSourceConnectionMetrics != null)
			{
				List<Connection> list = new List<Connection>();
				{
					foreach (DataSourceMetrics dataSourceConnectionMetric in this.DataSourceConnectionMetrics)
					{
						Connection connection = dataSourceConnectionMetric.ToAdditionalInfoConnection(this.m_jobContext);
						if (connection != null)
						{
							list.Add(connection);
						}
					}
					return list;
				}
			}
			return null;
		}

		public void StopAllRunningTimers()
		{
			if (this.m_metricManager != null)
			{
				this.m_metricManager.StopAllRunningTimers();
			}
		}

		public void UpdateForTreeScaleCache(long scaleTimeDurationMs, long peakGroupTreeMemoryUsageKB)
		{
			this.m_processingScalabilityDurationMs += scaleTimeDurationMs;
			this.m_peakGroupTreeMemoryUsageKB += peakGroupTreeMemoryUsageKB;
		}

		internal void AddLegacyDataProcessingTime(long durationMs)
		{
			if (this.m_metricManager != null)
			{
				TimeMetric timeMetric = this.m_metricManager[3];
				timeMetric.AddTime(durationMs);
			}
		}

		internal void AddDataProcessingTime(TimeMetric childMetric)
		{
			if (this.m_metricManager != null && childMetric != null)
			{
				TimeMetric timeMetric = this.m_metricManager[3];
				timeMetric.Add(childMetric);
			}
		}

		internal void AddDataSetMetrics(string dataSetName, DataProcessingMetrics metrics)
		{
			lock (this.m_dataSetMetrics)
			{
				this.m_dataSetMetrics.Add(new Pair<string, DataProcessingMetrics>(dataSetName, metrics));
			}
		}

		internal void AddDataSourceParallelExecutionMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, DataProcessingMetrics parallelDataSetMetrics)
		{
			lock (this.m_dataSourceConnectionMetrics)
			{
				this.m_dataSourceConnectionMetrics.Add(new DataSourceMetrics(dataSourceName, dataSourceReference, dataSourceType, parallelDataSetMetrics));
			}
		}

		internal void AddDataSourceMetrics(string dataSourceName, string dataSourceReference, string dataSourceType, DataProcessingMetrics aggregatedMetrics, DataProcessingMetrics[] dataSetsMetrics)
		{
			lock (this.m_dataSourceConnectionMetrics)
			{
				this.m_dataSourceConnectionMetrics.Add(new DataSourceMetrics(dataSourceName, dataSourceReference, dataSourceType, aggregatedMetrics, dataSetsMetrics));
			}
		}

		internal void StartProcessingTimer()
		{
			this.StartTimer(TimeMetricType.Processing);
		}

		internal void StopProcessingTimer()
		{
			this.StopTimer(TimeMetricType.Processing);
		}

		internal void StartRenderingTimer()
		{
			this.StartTimer(TimeMetricType.Rendering);
		}

		internal void StopRenderingTimer()
		{
			this.StopTimer(TimeMetricType.Rendering);
		}

		internal void StartTablixProcessingTimer()
		{
			this.StartTimer(TimeMetricType.TablixProcessing);
		}

		internal bool TryStartTablixProcessingTimer()
		{
			return this.TryStartTimer(TimeMetricType.TablixProcessing);
		}

		internal void StopTablixProcessingTimer()
		{
			this.StopTimer(TimeMetricType.TablixProcessing);
		}

		internal void StartExternalImageTimer()
		{
			if (this.m_jobContext != null)
			{
				this.m_externalImageTimer = new Timer();
				this.m_externalImageTimer.StartTimer();
			}
		}

		internal void StopExternalImageTimer()
		{
			if (this.m_externalImageTimer != null)
			{
				this.m_externalImageDurationMs += this.m_externalImageTimer.ElapsedTimeMs();
				this.m_externalImageTimer = null;
			}
		}

		public TimeMetric CreateDataRetrievalWorkerTimer()
		{
			return this.m_metricManager.CreateTimeMetric(3);
		}

		private void StartTimer(TimeMetricType metricType)
		{
			if (this.m_metricManager != null)
			{
				this.m_metricManager[(int)metricType].StartTimer();
			}
		}

		private bool TryStartTimer(TimeMetricType metricType)
		{
			if (this.m_metricManager != null)
			{
				return this.m_metricManager[(int)metricType].TryStartTimer();
			}
			return false;
		}

		private void StopTimer(TimeMetricType metricType)
		{
			if (this.m_metricManager != null)
			{
				this.m_metricManager[(int)metricType].StopTimer();
			}
		}

		private long GetNormalizedAdjustedMetric(TimeMetricType metricType)
		{
			if (this.m_metricManager == null)
			{
				return 0L;
			}
			return this.m_metricManager.GetNormalizedAdjustedMetric((int)metricType);
		}

		internal void RegisterTablixProcessingScaleCache(int reportId)
		{
			this.m_activeScaleCaches.Push(new ScaleCacheInfo(reportId));
		}

		internal void UnRegisterTablixProcessingScaleCache(int reportId, IScalabilityCache tablixProcessingCache)
		{
			this.m_processingScalabilityDurationMs += tablixProcessingCache.ScalabilityDurationMs;
			long num = tablixProcessingCache.PeakMemoryUsageKBytes;
			ScaleCacheInfo scaleCacheInfo = this.m_activeScaleCaches.Peek();
			if (scaleCacheInfo.ReportGlobalId == reportId && reportId != -2147483648)
			{
				num += scaleCacheInfo.ChildPeakMemoryUsageKB;
				this.m_activeScaleCaches.Pop();
				scaleCacheInfo = this.m_activeScaleCaches.Peek();
			}
			scaleCacheInfo.ChildPeakMemoryUsageKB = Math.Max(scaleCacheInfo.ChildPeakMemoryUsageKB, num);
		}
	}
}
