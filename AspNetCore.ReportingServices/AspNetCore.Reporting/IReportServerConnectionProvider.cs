namespace AspNetCore.Reporting
{
	internal interface IReportServerConnectionProvider
	{
		IReportServerConnection Create();
	}
}
