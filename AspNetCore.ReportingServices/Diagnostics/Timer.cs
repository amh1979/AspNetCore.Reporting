using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class Timer
	{
		private static int m_valuesLessThanZero;

		private bool m_start;

		private long m_LastValue;

		private long Value
		{
			get
			{
				long result = 0L;
				Timer.QueryPerformanceCounter(ref result);
				return result;
			}
		}

		private long Frequency
		{
			get
			{
				long result = 0L;
				Timer.QueryPerformanceFrequency(ref result);
				return result;
			}
		}

		public void StartTimer()
		{
			this.m_start = true;
			this.m_LastValue = this.Value;
		}

		public long ElapsedTimeMs()
		{
			if (!this.m_start)
			{
				return 0L;
			}
			this.m_start = false;
			long lastValue = this.m_LastValue;
			long value = this.Value;
			long num = value - lastValue;
			float num2 = (float)(1000.0 * (float)num / (float)this.Frequency);
			long num3 = (long)num2;
			if (num3 < 0 && RSTrace.RunningJobsTrace.TraceWarning && Interlocked.Increment(ref Timer.m_valuesLessThanZero) == 1)
			{
				RSTrace.RunningJobsTrace.Trace(TraceLevel.Warning, "Timestamp values retrieved from current CPU are not synchronized with other CPUs");
			}
			return num3;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		private static extern bool QueryPerformanceCounter([In] [Out] ref long lpPerformanceCount);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		private static extern bool QueryPerformanceFrequency([In] [Out] ref long lpFrequency);
	}
}
