using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Threading;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class ThreadSet : IDisposable
	{
		private int m_runningThreadCount;

		private ManualResetEvent m_allThreadsDone;

		private bool m_disposed;

		private bool m_waitCalled;

		private object m_counterLock = new object();

		internal ThreadSet(int expectedThreadCount)
		{
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "ThreadSet object created. {0} threads remaining.", expectedThreadCount);
			}
			this.m_allThreadsDone = new ManualResetEvent(true);
		}

		internal void TryQueueWorkItem(OnDemandProcessingContext processingContext, WaitCallback workItemCallback)
		{
			try
			{
				Interlocked.Increment(ref this.m_runningThreadCount);
				processingContext.JobContext.TryQueueWorkItem(workItemCallback, this);
			}
			catch (Exception)
			{
				Interlocked.Decrement(ref this.m_runningThreadCount);
				throw;
			}
		}

		internal void QueueWorkItem(OnDemandProcessingContext processingContext, WaitCallback workItemCallback)
		{
			try
			{
				Interlocked.Increment(ref this.m_runningThreadCount);
				processingContext.JobContext.QueueWorkItem(workItemCallback, this);
			}
			catch (Exception)
			{
				Interlocked.Decrement(ref this.m_runningThreadCount);
				throw;
			}
		}

		internal void ThreadCompleted()
		{
			int num = default(int);
			lock (this.m_counterLock)
			{
				num = Interlocked.Decrement(ref this.m_runningThreadCount);
				if (num <= 0)
				{
					this.m_allThreadsDone.Set();
				}
			}
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "Thread completed. {0} thread remaining.", num);
			}
		}

		internal void WaitForCompletion()
		{
			this.m_waitCalled = true;
			lock (this.m_counterLock)
			{
				if (Thread.VolatileRead(ref this.m_runningThreadCount) > 0)
				{
					this.m_allThreadsDone.Reset();
				}
			}
			this.m_allThreadsDone.WaitOne();
			if (Global.Tracer.TraceVerbose)
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "All the processing threads have completed.");
			}
		}

		public void Dispose()
		{
			if (!this.m_disposed)
			{
				this.m_disposed = true;
				this.m_allThreadsDone.Close();
			}
		}
	}
}
