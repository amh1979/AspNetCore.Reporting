using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class ScopeInstanceReference : Reference<ScopeInstance>
	{
		internal ScopeInstanceReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.ScopeInstanceReference;
		}

		public ScopeInstance Value()
		{
			return (ScopeInstance)base.InternalValue();
		}
	}
}
