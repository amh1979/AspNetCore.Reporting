using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public interface IReportObjectModelProxyForCustomCode
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
	}
}
