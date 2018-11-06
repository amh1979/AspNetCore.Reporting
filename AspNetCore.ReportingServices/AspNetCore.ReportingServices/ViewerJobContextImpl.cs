using AspNetCore.ReportingServices.DataProcessing;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Internal;
using System;
using System.Threading;

namespace AspNetCore.ReportingServices
{
	internal class ViewerJobContextImpl : IJobContext
	{
		private readonly object m_sync = new object();

		private readonly AdditionalInfo m_additionalInfo = new AdditionalInfo();

		public object SyncRoot
		{
			get
			{
				return this.m_sync;
			}
		}

		public ExecutionLogLevel ExecutionLogLevel
		{
			get
			{
				return ExecutionLogLevel.Normal;
			}
		}

		public TimeSpan TimeDataRetrieval
		{
            get; set;
        }

		public TimeSpan TimeProcessing
		{
            get; set;
        }

		public TimeSpan TimeRendering
		{
            get;set;
		}

		public long RowCount
		{
            get;set;
		}

        public AdditionalInfo AdditionalInfo => m_additionalInfo;


        public string ExecutionId
		{
			get
			{
				return string.Empty;
			}
		}

		public void AddAbortHelper(IAbortHelper abortHelper)
		{
		}

		public IAbortHelper GetAbortHelper()
		{
			return null;
		}

		public void RemoveAbortHelper()
		{
		}

		public void AddCommand(IDbCommand cmd)
		{
		}

		public void RemoveCommand(IDbCommand cmd)
		{
		}

		public bool ApplyCommandMemoryLimit(IDbCommand cmd)
		{
			return false;
		}

		public bool SetAdditionalCorrelation(IDbCommand cmd)
		{
			return false;
		}

		public void TryQueueWorkItem(WaitCallback callback, object state)
		{
			if (callback != null)
			{
				callback(state);
			}
		}

		public void QueueWorkItem(WaitCallback callback, object state)
		{
			throw new NotImplementedException();
		}
	}
}
