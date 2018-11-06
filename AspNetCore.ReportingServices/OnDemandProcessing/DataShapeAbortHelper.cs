using AspNetCore.ReportingServices.Diagnostics;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class DataShapeAbortHelper : DataShapeAbortHelperBase, IDataShapeAbortHelper, IAbortHelper, IDisposable
	{
		private readonly DataShapeQueryAbortHelper m_parentAbortHelper;

		public DataShapeAbortHelper(DataShapeQueryAbortHelper parentAbortHelper)
			: base(null, true)
		{
			this.m_parentAbortHelper = parentAbortHelper;
		}

		public void ThrowIfAborted(CancelationTrigger cancelationTrigger)
		{
			this.ThrowIfAborted(cancelationTrigger, null);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.m_parentAbortHelper.RemoveDataShapeAbortHelper(this);
			}
			base.Dispose(disposing);
		}
	}
}
