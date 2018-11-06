using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeCriObjReference : RuntimeChartCriObjReference, IReference<RuntimeCriObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeCriObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeCriObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeCriObj Value()
		{
			return (RuntimeCriObj)base.InternalValue();
		}
	}
}
