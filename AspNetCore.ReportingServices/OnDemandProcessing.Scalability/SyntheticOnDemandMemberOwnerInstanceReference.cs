using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SyntheticOnDemandMemberOwnerInstanceReference : SyntheticOnDemandScopeInstanceReference, IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference<IOnDemandMemberOwnerInstance>, IReference, IStorable, IPersistable
	{
		public SyntheticOnDemandMemberOwnerInstanceReference(IOnDemandMemberOwnerInstance memberOwner)
			: base(memberOwner)
		{
		}

		IOnDemandMemberOwnerInstance IReference<IOnDemandMemberOwnerInstance>.Value()
		{
			return (IOnDemandMemberOwnerInstance)this.Value();
		}
	}
}
