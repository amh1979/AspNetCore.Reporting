using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IOnDemandMemberOwnerInstanceReference : IReference<IOnDemandScopeInstance>, IReference<IOnDemandMemberOwnerInstance>, IReference, IStorable, IPersistable
	{
	}
}
