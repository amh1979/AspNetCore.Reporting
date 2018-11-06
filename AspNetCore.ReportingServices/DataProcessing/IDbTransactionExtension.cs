using System;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbTransactionExtension : IDbTransaction, IDisposable
	{
		bool AllowMultiConnection
		{
			get;
		}
	}
}
