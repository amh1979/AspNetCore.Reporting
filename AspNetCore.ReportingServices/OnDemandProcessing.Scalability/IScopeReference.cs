using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class IScopeReference : Reference<IScope>
	{
		internal IScopeReference()
		{
		}

		[DebuggerStepThrough]
		public IScope Value()
		{
			return (IScope)base.InternalValue();
		}
	}
}
