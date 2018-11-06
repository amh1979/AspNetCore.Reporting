using AspNetCore.ReportingServices.Interfaces;
using System;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbConnectionExtension : IDbConnection, IDisposable, IExtension
	{
		string Impersonate
		{
			set;
		}

		string UserName
		{
			set;
		}

		string Password
		{
			set;
		}

		bool IntegratedSecurity
		{
			get;
			set;
		}
	}
}
