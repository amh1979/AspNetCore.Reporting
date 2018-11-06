using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	[SkipStaticValidation]
	[PersistedWithinRequestOnly]
	internal class StreamingNoRowsDataRegionInstance : StreamingNoRowsScopeInstanceBase, IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		public StreamingNoRowsDataRegionInstance(OnDemandProcessingContext odpContext, IRIFReportDataScope dataRegion)
			: base(odpContext, dataRegion)
		{
		}

		public IOnDemandMemberInstanceReference GetFirstMemberInstance(ReportHierarchyNode rifMember)
		{
			return null;
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.StreamingNoRowsDataRegionInstance;
		}
	}
}
