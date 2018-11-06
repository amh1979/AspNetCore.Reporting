namespace AspNetCore.ReportingServices.DataProcessing
{
	internal interface IDbCommandAnalysis
	{
		IDataParameterCollection GetParameters();
	}
}
