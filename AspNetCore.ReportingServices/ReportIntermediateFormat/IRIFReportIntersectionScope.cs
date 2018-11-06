using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IRIFReportIntersectionScope : IRIFReportDataScope, IRIFReportScope, IInstancePath, IRIFDataScope
	{
		IRIFReportDataScope ParentRowReportScope
		{
			get;
		}

		IRIFReportDataScope ParentColumnReportScope
		{
			get;
		}

		bool IsColumnOuterGrouping
		{
			get;
		}

		void BindToStreamingScopeInstance(IReference<IOnDemandMemberInstance> parentRowScopeInstance, IReference<IOnDemandMemberInstance> parentColumnScopeInstance);
	}
}
