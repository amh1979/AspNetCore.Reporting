using System;
using System.Security.Principal;

namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IAuthenticationExtension2 : IExtension
	{
		void GetUserInfo(out IIdentity userIdentity, out IntPtr userId);

		void GetUserInfo(IRSRequestContext requestContext, out IIdentity userIdentity, out IntPtr userId);

		bool LogonUser(string userName, string password, string authority);

		bool IsValidPrincipalName(string principalName);
	}
}
