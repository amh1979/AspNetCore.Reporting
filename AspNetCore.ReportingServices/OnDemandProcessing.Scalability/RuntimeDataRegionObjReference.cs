using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataRegionObjReference : IScopeReference, IReference<RuntimeDataRegionObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataRegionObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataRegionObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDataRegionObj Value()
		{
			return (RuntimeDataRegionObj)base.InternalValue();
		}
	}
}
