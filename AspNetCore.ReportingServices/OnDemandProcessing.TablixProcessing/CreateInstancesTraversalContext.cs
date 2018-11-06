using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class CreateInstancesTraversalContext : ITraversalContext
	{
		private ScopeInstance m_parentInstance;

		private IReference<RuntimeMemberObj>[] m_innerMembers;

		private IReference<RuntimeDataTablixGroupLeafObj> m_innerGroupLeafRef;

		internal ScopeInstance ParentInstance
		{
			get
			{
				return this.m_parentInstance;
			}
		}

		internal IReference<RuntimeMemberObj>[] InnerMembers
		{
			get
			{
				return this.m_innerMembers;
			}
		}

		internal IReference<RuntimeDataTablixGroupLeafObj> InnerGroupLeafRef
		{
			get
			{
				return this.m_innerGroupLeafRef;
			}
		}

		internal CreateInstancesTraversalContext(ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef)
		{
			this.m_parentInstance = parentInstance;
			this.m_innerMembers = innerMembers;
			this.m_innerGroupLeafRef = innerGroupLeafRef;
		}
	}
}
