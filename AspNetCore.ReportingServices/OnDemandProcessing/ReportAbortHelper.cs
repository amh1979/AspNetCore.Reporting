using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class ReportAbortHelper : AbortHelper, IDisposable
	{
		private Hashtable m_reportStatus;

		internal ReportAbortHelper(IJobContext jobContext, bool enforceSingleAbortException)
			: base(jobContext, enforceSingleAbortException, true)
		{
			this.m_reportStatus = new Hashtable();
		}

		protected override ProcessingStatus GetStatus(string uniqueName)
		{
			ProcessingStatus status = base.Status;
			if (uniqueName == null)
			{
				return status;
			}
			if (status != 0)
			{
				return status;
			}
			Global.Tracer.Assert(this.m_reportStatus.ContainsKey(uniqueName), "(m_reportStatus.ContainsKey(uniqueName))");
			return (ProcessingStatus)this.m_reportStatus[uniqueName];
		}

		protected override void SetStatus(ProcessingStatus newStatus, string uniqueName)
		{
			if (uniqueName == null)
			{
				base.Status = newStatus;
			}
			else
			{
				Hashtable hashtable = Hashtable.Synchronized(this.m_reportStatus);
				Global.Tracer.Assert(hashtable.ContainsKey(uniqueName), "(reportStatus.ContainsKey(uniqueName))");
				hashtable[uniqueName] = newStatus;
			}
		}

		internal override void AddSubreportInstanceOrSharedDataSet(string uniqueName)
		{
			Hashtable hashtable = Hashtable.Synchronized(this.m_reportStatus);
			if (!hashtable.ContainsKey(uniqueName))
			{
				hashtable.Add(uniqueName, ProcessingStatus.Success);
			}
		}
	}
}
