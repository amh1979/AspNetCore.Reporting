using AspNetCore.ReportingServices.Interfaces;
using System;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbPoolableConnection : IDbConnection, IDisposable, IExtension
	{
		bool IsAlive
		{
			get;
		}

		bool IsFromPool
		{
			get;
			set;
		}

		string GetConnectionStringForPooling();
	}
}
