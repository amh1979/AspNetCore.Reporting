namespace AspNetCore.ReportingServices.DataExtensions
{
	internal interface IServiceEndpoint
	{
		string Host
		{
			get;
		}

		int Port
		{
			get;
		}
	}
}
