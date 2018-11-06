using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	internal interface IReportObjectModelProxyForCustomCode
	{
		Parameters Parameters
		{
			get;
		}

		Globals Globals
		{
			get;
		}

		User User
		{
			get;
		}

		Variables Variables
		{
			get;
		}
	}
}
