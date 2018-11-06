using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal interface IReportParameterLookup
	{
		string GetReportParamsInstanceId(NameValueCollection reportParameters);
	}
}
