using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	[PersistedWithinRequestOnly]
	[SkipStaticValidation]
	internal class StreamingNoRowsMemberInstance : StreamingNoRowsScopeInstanceBase, IOnDemandMemberInstance, IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		public List<object> GroupExprValues
		{
			get
			{
				return null;
			}
		}

		public StreamingNoRowsMemberInstance(OnDemandProcessingContext odpContext, IRIFReportDataScope member)
			: base(odpContext, member)
		{
		}

		public IOnDemandMemberInstanceReference GetNextMemberInstance()
		{
			return null;
		}

		public IOnDemandScopeInstance GetCellInstance(IOnDemandMemberInstanceReference outerGroupInstanceRef, out IReference<IOnDemandScopeInstance> cellRef)
		{
			cellRef = null;
			return null;
		}

		public IOnDemandMemberInstanceReference GetFirstMemberInstance(ReportHierarchyNode rifMember)
		{
			return null;
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.StreamingNoRowsMemberInstance;
		}
	}
}
