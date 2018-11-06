using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SubReportInstanceReference : ScopeInstanceReference, IReference<SubReportInstance>, IReference, IStorable, IPersistable
	{
		internal SubReportInstanceReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SubReportInstanceReference;
		}

		public new SubReportInstance Value()
		{
			return (SubReportInstance)base.InternalValue();
		}
	}
}
