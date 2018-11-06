using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	internal interface IPageBreakLocation2005 : IUpgradeable
	{
		bool PageBreakAtStart
		{
			get;
			set;
		}

		bool PageBreakAtEnd
		{
			get;
			set;
		}

		PageBreak PageBreak
		{
			get;
			set;
		}
	}
}
