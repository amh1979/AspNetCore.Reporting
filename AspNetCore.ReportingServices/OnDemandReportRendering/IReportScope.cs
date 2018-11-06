using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal interface IReportScope
	{
		IReportScopeInstance ReportScopeInstance
		{
			get;
		}

		IRIFReportScope RIFReportScope
		{
			get;
		}
	}
}
