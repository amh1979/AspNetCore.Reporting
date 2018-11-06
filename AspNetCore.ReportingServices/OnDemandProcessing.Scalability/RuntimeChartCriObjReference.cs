using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeChartCriObjReference : RuntimeDataTablixObjReference, IReference<RuntimeChartCriObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeChartCriObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeChartCriObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeChartCriObj Value()
		{
			return (RuntimeChartCriObj)base.InternalValue();
		}
	}
}
