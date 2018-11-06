using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class DataRegionInstanceReference : ScopeInstanceReference, IReference<DataRegionInstance>, IReference, IStorable, IPersistable
	{
		internal DataRegionInstanceReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.DataRegionInstanceReference;
		}

		public new DataRegionInstance Value()
		{
			return (DataRegionInstance)base.InternalValue();
		}
	}
}
