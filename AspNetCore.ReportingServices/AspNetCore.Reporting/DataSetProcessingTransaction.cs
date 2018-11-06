using AspNetCore.ReportingServices.DataProcessing;
using System;

namespace AspNetCore.Reporting
{
	internal class DataSetProcessingTransaction : IDbTransaction, IDisposable
	{
		public void Commit()
		{
		}

		public void Rollback()
		{
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
