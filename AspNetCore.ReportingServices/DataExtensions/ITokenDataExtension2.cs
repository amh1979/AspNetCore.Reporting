using AspNetCore.ReportingServices.DataProcessing;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal interface ITokenDataExtension2 : ITokenDataExtension
	{
		bool UseTokenAuthentication
		{
			get;
		}
	}
}
