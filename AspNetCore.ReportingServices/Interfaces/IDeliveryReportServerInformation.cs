namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IDeliveryReportServerInformation
	{
		Extension[] RenderingExtension
		{
			get;
		}

		Setting[] ServerSettings
		{
			get;
		}
	}
}
