using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDetailObjReference : RuntimeHierarchyObjReference, IReference<RuntimeDetailObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDetailObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDetailObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDetailObj Value()
		{
			return (RuntimeDetailObj)base.InternalValue();
		}
	}
}
