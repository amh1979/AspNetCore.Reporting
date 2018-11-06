using System;

namespace AspNetCore.Reporting
{
	public interface IReportServerConnection : IReportServerCredentials
	{
		Uri ReportServerUrl
		{
			get;
		}

		int Timeout
		{
			get;
		}
	}
}
