using AspNetCore.ReportingServices.Interfaces;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal interface IExtensionFactory
	{
		bool IsRegisteredCustomReportItemExtension(string extensionType);

		object GetNewCustomReportItemProcessingInstanceClass(string reportItemName);

		IExtension GetNewRendererExtensionClass(string format);
	}
}
