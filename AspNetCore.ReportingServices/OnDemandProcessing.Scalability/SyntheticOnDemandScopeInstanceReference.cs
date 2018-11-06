using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SyntheticOnDemandScopeInstanceReference : SyntheticReferenceBase<IOnDemandScopeInstance>
	{
		private readonly IOnDemandScopeInstance m_value;

		public SyntheticOnDemandScopeInstanceReference(IOnDemandScopeInstance scopeInstance)
		{
			this.m_value = scopeInstance;
		}

		public override IOnDemandScopeInstance Value()
		{
			return this.m_value;
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SyntheticOnDemandScopeInstanceReference;
		}
	}
}
