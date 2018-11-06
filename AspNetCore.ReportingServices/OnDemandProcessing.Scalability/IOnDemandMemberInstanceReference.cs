using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IOnDemandMemberInstanceReference : IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference<IOnDemandMemberOwnerInstance>, IReference<IOnDemandMemberInstance>, IReference, IStorable, IPersistable
	{
	}
}
