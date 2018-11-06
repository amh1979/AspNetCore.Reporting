using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportUrl
	{
		private Uri m_pathUri;

		private string m_replacementRoot;

		private RenderingContext m_reportContext;

		private ICatalogItemContext m_newICatalogItemContext;

		internal ReportUrl(ICatalogItemContext catContext, string initialUrl)
		{
			this.m_pathUri = new Uri(ReportUrl.BuildPathUri(catContext, initialUrl, (NameValueCollection)null, out this.m_newICatalogItemContext));
		}

		internal ReportUrl(RenderingContext reportContext, string initialUrl)
		{
			this.m_reportContext = reportContext;
			this.m_pathUri = new Uri(ReportUrl.BuildPathUri(reportContext.TopLevelReportContext, initialUrl, (NameValueCollection)null, out this.m_newICatalogItemContext));
		}

		internal ReportUrl(RenderingContext reportContext, string initialUrl, bool checkProtocol, NameValueCollection unparsedParameters, bool useReplacementRoot)
		{
			this.m_reportContext = reportContext;
			this.m_pathUri = new Uri(ReportUrl.BuildPathUri(reportContext.TopLevelReportContext, checkProtocol, initialUrl, unparsedParameters, out this.m_newICatalogItemContext));
			bool flag = default(bool);
			if (useReplacementRoot && reportContext.TopLevelReportContext.IsReportServerPathOrUrl(this.m_pathUri.AbsoluteUri, checkProtocol, out flag))
			{
				this.m_replacementRoot = reportContext.ReplacementRoot;
			}
		}

		internal static string BuildPathUri(ICatalogItemContext currentICatalogItemContext, string initialUrl, NameValueCollection unparsedParameters, out ICatalogItemContext newContext)
		{
			return ReportUrl.BuildPathUri(currentICatalogItemContext, true, initialUrl, unparsedParameters, out newContext);
		}

		internal static string BuildPathUri(ICatalogItemContext currentCatalogItemContext, bool checkProtocol, string initialUrl, NameValueCollection unparsedParameters, out ICatalogItemContext newContext)
		{
			newContext = null;
			if (currentCatalogItemContext == null)
			{
				return initialUrl;
			}
			string text = null;
			try
			{
				text = currentCatalogItemContext.CombineUrl(initialUrl, checkProtocol, unparsedParameters, out newContext);
			}
			catch (UriFormatException)
			{
				throw new RenderingObjectModelException(ErrorCode.rsMalformattedURL);
			}
			if (!currentCatalogItemContext.IsSupportedProtocol(text, checkProtocol))
			{
				throw new RenderingObjectModelException(ErrorCode.rsUnsupportedURLProtocol, text);
			}
			return text;
		}

		internal static ReportUrl BuildHyperLinkURL(string hyperLinkUrlValue, RenderingContext renderingContext)
		{
			ReportUrl result = null;
			try
			{
				if (hyperLinkUrlValue != null)
				{
					bool flag = default(bool);
					if (renderingContext.TopLevelReportContext.IsReportServerPathOrUrl(hyperLinkUrlValue, false, out flag) && flag)
					{
						NameValueCollection unparsedParameters = default(NameValueCollection);
						renderingContext.TopLevelReportContext.PathManager.ExtractFromUrl(hyperLinkUrlValue, out hyperLinkUrlValue, out unparsedParameters);
						if (hyperLinkUrlValue != null && hyperLinkUrlValue.Length != 0)
						{
							return new ReportUrl(renderingContext, hyperLinkUrlValue, false, unparsedParameters, true);
						}
						return null;
					}
					return new ReportUrl(renderingContext, hyperLinkUrlValue, false, null, true);
				}
				return result;
			}
			catch
			{
				return null;
			}
		}

		public override string ToString()
		{
			return this.m_pathUri.AbsoluteUri;
		}

		public Uri ToUri()
		{
			Uri result = this.m_pathUri;
			if (this.m_replacementRoot != null)
			{
				ReportUrlBuilder reportUrlBuilder = new ReportUrlBuilder(this.m_reportContext, this.m_pathUri.AbsoluteUri, this.m_replacementRoot);
				result = reportUrlBuilder.ToUri();
			}
			return result;
		}

		public ReportUrlBuilder GetUrlBuilder(string initialUrl, bool useReplacementRoot)
		{
			return new ReportUrlBuilder(this.m_reportContext, this.m_newICatalogItemContext, initialUrl, useReplacementRoot ? this.m_replacementRoot : null);
		}
	}
}
