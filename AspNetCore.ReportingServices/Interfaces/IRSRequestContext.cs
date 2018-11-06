using System.Collections.Generic;
using System.Security.Principal;

namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IRSRequestContext
	{
		IDictionary<string, string> Cookies
		{
			get;
		}

		IDictionary<string, string[]> Headers
		{
			get;
		}

		IIdentity User
		{
			get;
		}
	}
}
