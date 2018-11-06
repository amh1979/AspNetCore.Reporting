using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	internal interface IReportItem2005 : IUpgradeable
	{
		Action Action
		{
			get;
			set;
		}
	}
}
