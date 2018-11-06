using System;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbTransaction : IDisposable
	{
		void Commit();

		void Rollback();
	}
}
