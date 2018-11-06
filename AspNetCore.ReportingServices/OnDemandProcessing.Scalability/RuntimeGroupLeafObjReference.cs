using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGroupLeafObjReference : RuntimeGroupObjReference, IReference<RuntimeGroupLeafObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGroupLeafObj Value()
		{
			return (RuntimeGroupLeafObj)base.InternalValue();
		}
	}
}
