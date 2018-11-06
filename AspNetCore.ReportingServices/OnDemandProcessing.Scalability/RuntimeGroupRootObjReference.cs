using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGroupRootObjReference : RuntimeGroupObjReference, IReference<RuntimeGroupRootObj>, IReference<IDataCorrelation>, IReference, IStorable, IPersistable
	{
		internal RuntimeGroupRootObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeGroupRootObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGroupRootObj Value()
		{
			return (RuntimeGroupRootObj)base.InternalValue();
		}

		[DebuggerStepThrough]
		IDataCorrelation IReference<IDataCorrelation>.Value()
		{
			return (IDataCorrelation)base.InternalValue();
		}
	}
}
