using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.Reporting
{
	internal sealed class GetResourceForLocalService : IGetResource
	{
		private ILocalCatalog m_catalog;

		public GetResourceForLocalService(ILocalCatalog catalog)
		{
			this.m_catalog = catalog;
		}

		public void GetResource(ICatalogItemContext reportContext, string imageUrl, out byte[] resource, out string mimeType, out bool registerWarning, out bool registerInvalidSizeWarning)
		{
			registerInvalidSizeWarning = false;
			ICatalogItemContext catalogItemContext = null;
			if (reportContext != null)
			{
				catalogItemContext = reportContext.Combine(imageUrl);
			}
			if (catalogItemContext != null)
			{
				resource = this.m_catalog.GetResource(catalogItemContext.ItemPathAsString, out mimeType);
				registerWarning = (resource == null);
			}
			else
			{
				try
				{
					registerWarning = false;
					resource = ExternalResourceLoader.GetExternalResource(imageUrl, true, (string)null, (string)null, (string)null, 600, ExternalResourceLoader.MaxResourceSizeUnlimited, (ExternalResourceAbortHelper)null, out mimeType, out registerInvalidSizeWarning);
				}
				finally
				{
					registerWarning = true;
				}
			}
		}
	}
}
