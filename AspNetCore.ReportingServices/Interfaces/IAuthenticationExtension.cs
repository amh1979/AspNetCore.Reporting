using System;
using System.Security.Principal;

namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IAuthenticationExtension : IExtension
	{
		void GetUserInfo(out IIdentity userIdentity, out IntPtr userId);

		bool LogonUser(string userName, string password, string authority);

		bool IsValidPrincipalName(string principalName);
	}
}
