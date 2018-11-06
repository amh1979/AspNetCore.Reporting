using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeTablixObjReference : RuntimeDataTablixObjReference, IReference<RuntimeTablixObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeTablixObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeTablixObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeTablixObj Value()
		{
			return (RuntimeTablixObj)base.InternalValue();
		}
	}
}
