using System.Data;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbConnectionWrapper
	{
		System.Data.IDbConnection Connection
		{
			get;
		}
	}
}
