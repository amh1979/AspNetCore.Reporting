using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Threading;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class AbortHelper : IAbortHelper, IDisposable
	{
		private ProcessingStatus m_overallStatus;

		private Exception m_exception;

		private readonly IJobContext m_jobContext;

		private bool m_enforceSingleAbortException;

		private bool m_hasThrownAbortedException;

		private readonly bool m_requireEventHandler;

		private readonly object m_syncRoot;

		protected ProcessingStatus Status
		{
			get
			{
				return this.m_overallStatus;
			}
			set
			{
				this.m_overallStatus = value;
			}
		}

		internal bool EnforceSingleAbortException
		{
			get
			{
				return this.m_enforceSingleAbortException;
			}
			set
			{
				this.m_enforceSingleAbortException = value;
			}
		}

		public event EventHandler ProcessingAbortEvent;

		internal AbortHelper(IJobContext jobContext, bool enforceSingleAbortException, bool requireEventHandler)
		{
			this.m_enforceSingleAbortException = enforceSingleAbortException;
			this.m_requireEventHandler = requireEventHandler;
			this.m_syncRoot = new object();
			if (jobContext != null)
			{
				this.m_jobContext = jobContext;
				jobContext.AddAbortHelper(this);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.m_jobContext != null)
			{
				this.m_jobContext.RemoveAbortHelper();
			}
		}

		public virtual bool Abort(ProcessingStatus status)
		{
			return this.Abort(status, null);
		}

		protected abstract ProcessingStatus GetStatus(string uniqueName);

		protected abstract void SetStatus(ProcessingStatus newStatus, string uniqueName);

		internal abstract void AddSubreportInstanceOrSharedDataSet(string uniqueName);

		internal bool SetError(Exception e, string uniqueName)
		{
			Global.Tracer.Trace(TraceLevel.Verbose, "An exception has occurred. Trying to abort processing. Details: {0}", (e == null) ? "" : e.ToString());
			if (this.m_exception == null)
			{
				this.m_exception = e;
			}
			else if (e is DataSetExecutionException && e.InnerException == this.m_exception)
			{
				this.m_exception = e;
			}
			if (!this.Abort(ProcessingStatus.AbnormalTermination, uniqueName))
			{
				return false;
			}
			return true;
		}

		private bool Abort(ProcessingStatus status, string uniqueName)
		{
			bool result = !this.m_requireEventHandler;
			if (!Monitor.TryEnter(this.m_syncRoot))
			{
				Global.Tracer.Trace(TraceLevel.Info, "Some other thread is aborting processing.");
				return result;
			}
			try
			{
				if (this.GetStatus(uniqueName) != 0)
				{
					Global.Tracer.Trace(TraceLevel.Info, "Some other thread has already aborted processing.");
					return result;
				}
				this.SetStatus(status, uniqueName);
				if (this.ProcessingAbortEvent != null)
				{
					try
					{
						this.ProcessingAbortEvent(this, new ProcessingAbortEventArgs(uniqueName));
						result = true;
						Global.Tracer.Trace(TraceLevel.Verbose, "Abort callback successful.");
						return result;
					}
					catch (Exception ex)
					{
						Global.Tracer.Trace(TraceLevel.Error, "Exception in abort callback. Details: {0}", ex.ToString());
						return result;
					}
				}
				Global.Tracer.Trace(TraceLevel.Verbose, "No abort callback.");
				return result;
			}
			finally
			{
				Monitor.Exit(this.m_syncRoot);
			}
		}

		internal virtual void ThrowIfAborted(CancelationTrigger cancelationTrigger, string uniqueName)
		{
			ProcessingStatus status = default(ProcessingStatus);
			lock (this.m_syncRoot)
			{
				status = this.GetStatus(uniqueName);
			}
			if (status == ProcessingStatus.Success)
			{
				return;
			}
			if (this.m_hasThrownAbortedException && this.m_enforceSingleAbortException)
			{
				return;
			}
			this.m_hasThrownAbortedException = true;
			if (status == ProcessingStatus.AbnormalTermination)
			{
				throw new ProcessingAbortedException(cancelationTrigger, this.m_exception);
			}
			throw new ProcessingAbortedException(cancelationTrigger);
		}
	}
}
