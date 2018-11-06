using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface ICatalogConfiguredExtension
	{
		void SetCatalogConfiguration(IDictionary<string, string> configuration);

		IEnumerable<string> EnumerateRequiredProperties();
	}
}
