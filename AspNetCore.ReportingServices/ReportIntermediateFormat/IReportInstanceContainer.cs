using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IReportInstanceContainer
	{
		IReference<ReportInstance> ReportInstance
		{
			get;
		}

		IReference<ReportInstance> SetReportInstance(ReportInstance reportInstance, OnDemandMetadata odpMetadata);
	}
}
