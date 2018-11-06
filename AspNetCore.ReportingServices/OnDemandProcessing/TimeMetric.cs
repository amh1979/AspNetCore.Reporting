using AspNetCore.ReportingServices.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class TimeMetric
	{
		private bool m_isRunning;

		private long m_totalDurationMs;

		private readonly Timer m_timer;

		private readonly long[] m_otherMetricAdjustments;

		private readonly TimeMetricManager m_metricAdjuster;

		private readonly int m_indexInCollection;

		public bool IsRunning
		{
			get
			{
				return this.m_isRunning;
			}
		}

		public long TotalDurationMs
		{
			get
			{
				return this.m_totalDurationMs;
			}
		}

		public long[] OtherMetricAdjustments
		{
			get
			{
				return this.m_otherMetricAdjustments;
			}
		}

		public TimeMetric(int indexInCollection, TimeMetricManager metricAdjuster, int otherMetricCount)
		{
			this.m_indexInCollection = indexInCollection;
			this.m_timer = new Timer();
			this.m_totalDurationMs = 0L;
			this.m_isRunning = false;
			this.m_otherMetricAdjustments = new long[otherMetricCount];
			this.m_metricAdjuster = metricAdjuster;
		}

		public TimeMetric(TimeMetric other)
		{
			this.m_indexInCollection = other.m_indexInCollection;
			this.m_timer = new Timer();
			this.m_totalDurationMs = other.m_totalDurationMs;
			this.m_isRunning = false;
			this.m_otherMetricAdjustments = (long[])other.m_otherMetricAdjustments.Clone();
			this.m_metricAdjuster = other.m_metricAdjuster;
		}

		public void StartTimer()
		{
			this.m_timer.StartTimer();
			this.m_isRunning = true;
		}

		public bool TryStartTimer()
		{
			if (this.m_isRunning)
			{
				return false;
			}
			this.StartTimer();
			return true;
		}

		public void StopTimer()
		{
			this.m_isRunning = false;
			long durationMs = this.m_timer.ElapsedTimeMs();
			this.AddTime(durationMs);
		}

		public void Add(TimeMetric otherMetric)
		{
			this.m_totalDurationMs += otherMetric.TotalDurationMs;
			for (int i = 0; i < this.m_otherMetricAdjustments.Length; i++)
			{
				this.m_otherMetricAdjustments[i] += otherMetric.m_otherMetricAdjustments[i];
			}
		}

		public void AddTime(long durationMs)
		{
			durationMs = ExecutionLogContext.TimerMeasurementAdjusted(durationMs);
			this.m_metricAdjuster.UpdateTimeMetricAdjustments(durationMs, this.m_otherMetricAdjustments);
			this.m_totalDurationMs += durationMs;
		}

		public void Subtract(TimeMetric other)
		{
			this.m_totalDurationMs = ExecutionLogContext.TimerMeasurementAdjusted(this.m_totalDurationMs - other.m_totalDurationMs);
			for (int i = 0; i < this.m_otherMetricAdjustments.Length; i++)
			{
				long durationMs = this.m_otherMetricAdjustments[i] - other.m_otherMetricAdjustments[i];
				this.m_otherMetricAdjustments[i] = ExecutionLogContext.TimerMeasurementAdjusted(durationMs);
			}
		}
	}
}
