using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using System;

namespace AspNetCore.ReportingServices
{
	internal class ViewerExtensionFactory : IExtensionFactory
	{
		public bool IsRegisteredCustomReportItemExtension(string extensionType)
		{
			throw new NotImplementedException();
		}

		public object GetNewCustomReportItemProcessingInstanceClass(string reportItemName)
		{
			throw new NotImplementedException();
		}

		public IExtension GetNewRendererExtensionClass(string format)
		{
			throw new NotImplementedException();
		}
	}
}
