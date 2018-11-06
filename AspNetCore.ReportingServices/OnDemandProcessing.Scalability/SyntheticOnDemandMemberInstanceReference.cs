using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SyntheticOnDemandMemberInstanceReference : SyntheticOnDemandScopeInstanceReference, IOnDemandMemberInstanceReference, IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference<IOnDemandMemberOwnerInstance>, IReference<IOnDemandMemberInstance>, IReference, IStorable, IPersistable
	{
		public SyntheticOnDemandMemberInstanceReference(IOnDemandMemberInstance member)
			: base(member)
		{
		}

		IOnDemandMemberOwnerInstance IReference<IOnDemandMemberOwnerInstance>.Value()
		{
			return (IOnDemandMemberOwnerInstance)this.Value();
		}

		IOnDemandMemberInstance IReference<IOnDemandMemberInstance>.Value()
		{
			return (IOnDemandMemberInstance)this.Value();
		}
	}
}
