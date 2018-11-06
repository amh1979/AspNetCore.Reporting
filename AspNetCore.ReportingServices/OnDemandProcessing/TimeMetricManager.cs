using AspNetCore.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class TimeMetricManager
	{
		private readonly TimeMetric[] m_timeMetrics;

		public TimeMetric this[int index]
		{
			get
			{
				return this.m_timeMetrics[index];
			}
		}

		public TimeMetricManager(int metricCount)
		{
			this.m_timeMetrics = new TimeMetric[metricCount];
			for (int i = 0; i < this.m_timeMetrics.Length; i++)
			{
				this.m_timeMetrics[i] = this.CreateTimeMetric(i);
			}
		}

		public TimeMetric CreateTimeMetric(int index)
		{
			return new TimeMetric(index, this, this.m_timeMetrics.Length);
		}

		public long GetNormalizedAdjustedMetric(int targetIndex)
		{
			TimeMetric timeMetric = this.m_timeMetrics[targetIndex];
			long num = timeMetric.TotalDurationMs;
			for (int i = 0; i < this.m_timeMetrics.Length; i++)
			{
				if (i != targetIndex)
				{
					TimeMetric timeMetric2 = this.m_timeMetrics[i];
					num -= timeMetric2.OtherMetricAdjustments[targetIndex];
				}
			}
			return ExecutionLogContext.NormalizeCalculatedDuration(num);
		}

		public void UpdateTimeMetricAdjustments(long lastDurationMs, long[] metricAdjustments)
		{
			if (lastDurationMs > 0)
			{
				int num = this.m_timeMetrics.Length - 1;
				while (true)
				{
					if (num >= 0)
					{
						if (!this.m_timeMetrics[num].IsRunning)
						{
							num--;
							continue;
						}
						break;
					}
					return;
				}
				metricAdjustments[num] += lastDurationMs;
			}
		}

		public void StopAllRunningTimers()
		{
			for (int num = this.m_timeMetrics.Length - 1; num >= 0; num--)
			{
				TimeMetric timeMetric = this.m_timeMetrics[num];
				if (timeMetric.IsRunning)
				{
					timeMetric.StopTimer();
				}
			}
		}

		[Conditional("DEBUG")]
		public void VerifyStartOrder(int index)
		{
			for (int i = index; i < this.m_timeMetrics.Length; i++)
			{
				Global.Tracer.Assert(!this.m_timeMetrics[i].IsRunning, "Later metric must not be running when starting an earlier metric or adjustments will not work.");
			}
		}
	}
}
