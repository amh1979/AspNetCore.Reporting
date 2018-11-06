using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeMapDataRegionObjReference : RuntimeChartCriObjReference, IReference<RuntimeMapDataRegionObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeMapDataRegionObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeMapDataRegionObjReference;
		}

		public new RuntimeMapDataRegionObj Value()
		{
			return (RuntimeMapDataRegionObj)base.InternalValue();
		}
	}
}
