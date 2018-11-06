using System;

namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbCommand : IDisposable
	{
		string CommandText
		{
			get;
			set;
		}

		int CommandTimeout
		{
			get;
			set;
		}

		CommandType CommandType
		{
			get;
			set;
		}

		IDataParameterCollection Parameters
		{
			get;
		}

		IDbTransaction Transaction
		{
			get;
			set;
		}

		IDataReader ExecuteReader(CommandBehavior behavior);

		IDataParameter CreateParameter();

		void Cancel();
	}
}
