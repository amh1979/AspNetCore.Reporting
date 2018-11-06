using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class DataShapeAbortHelperBase : AbortHelper
	{
		public DataShapeAbortHelperBase(IJobContext jobContext, bool enforceSingleAbortException)
			: base(jobContext, enforceSingleAbortException, false)
		{
		}

		protected override ProcessingStatus GetStatus(string uniqueName)
		{
			Global.Tracer.Assert(uniqueName == null, "Data shape processing does not support sub-units.");
			return base.Status;
		}

		protected override void SetStatus(ProcessingStatus newStatus, string uniqueName)
		{
			Global.Tracer.Assert(uniqueName == null, "Data shape processing does not support sub-units.");
			base.Status = newStatus;
		}

		internal override void AddSubreportInstanceOrSharedDataSet(string uniqueName)
		{
			Global.Tracer.Assert(false, "Data shape processing does nto support sub-units.");
		}
	}
}
