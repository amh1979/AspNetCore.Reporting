using System.Collections.Generic;
using System.Net;

namespace AspNetCore.Reporting
{
	public interface IReportServerConnection2 : IReportServerConnection, IReportServerCredentials
	{
		IEnumerable<Cookie> Cookies
		{
			get;
		}

		IEnumerable<string> Headers
		{
			get;
		}
	}
}
