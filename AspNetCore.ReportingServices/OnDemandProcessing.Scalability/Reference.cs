using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal abstract class Reference<T> : BaseReference, IReference<T>, IReference, IStorable, IPersistable, IDisposable where T : IStorable
	{
		internal Reference()
		{
		}

		T IReference<T>.Value()
		{
			return (T)base.InternalValue();
		}
	}
}
