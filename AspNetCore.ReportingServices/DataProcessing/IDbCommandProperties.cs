namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbCommandProperties
	{
		bool SetRequestMemoryLimit(int limit);

		bool SetRequestIDAndCurrentActivityID();
	}
}
