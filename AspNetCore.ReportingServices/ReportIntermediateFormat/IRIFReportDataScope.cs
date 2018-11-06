using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IRIFReportDataScope : IRIFReportScope, IInstancePath, IRIFDataScope
	{
		bool IsDataIntersectionScope
		{
			get;
		}

		bool IsScope
		{
			get;
		}

		IRIFReportDataScope ParentReportScope
		{
			get;
		}

		IReference<IOnDemandScopeInstance> CurrentStreamingScopeInstance
		{
			get;
		}

		bool IsGroup
		{
			get;
		}

		bool IsBoundToStreamingScopeInstance
		{
			get;
		}

		void BindToStreamingScopeInstance(IReference<IOnDemandScopeInstance> scopeInstance);

		void BindToNoRowsScopeInstance(OnDemandProcessingContext odpContext);

		void ClearStreamingScopeInstanceBinding();

		void ResetAggregates(AggregatesImpl reportOmAggregates);

		bool HasServerAggregate(string aggregateName);

		bool IsSameOrChildScopeOf(IRIFReportDataScope candidateScope);

		bool IsChildScopeOf(IRIFReportDataScope candidateScope);
	}
}
