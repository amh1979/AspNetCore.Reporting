using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportUrl
	{
		private AspNetCore.ReportingServices.ReportRendering.ReportUrl m_renderUrl;

		private Uri m_pathUri;

		private bool IsOldSnapshot
		{
			get
			{
				return null != this.m_renderUrl;
			}
		}

		internal ReportUrl(AspNetCore.ReportingServices.ReportRendering.ReportUrl renderUrl)
		{
			this.m_renderUrl = renderUrl;
		}

		internal ReportUrl(ICatalogItemContext itemContext, string initialUrl)
		{
			this.m_pathUri = new Uri(ReportUrl.BuildPathUri(itemContext, initialUrl, null));
		}

		internal ReportUrl(ICatalogItemContext itemContext, string initialUrl, bool checkProtocol, NameValueCollection unparsedParameters)
		{
			ICatalogItemContext catalogItemContext = default(ICatalogItemContext);
			this.m_pathUri = new Uri(ReportUrl.BuildPathUri(itemContext, checkProtocol, initialUrl, unparsedParameters, out catalogItemContext));
			if (this.m_pathUri != (Uri)null && string.CompareOrdinal(this.m_pathUri.Scheme, "mailto") == 0)
			{
				string absoluteUri = this.m_pathUri.AbsoluteUri;
			}
		}

		internal static string BuildPathUri(ICatalogItemContext currentICatalogItemContext, string initialUrl, NameValueCollection unparsedParameters)
		{
			ICatalogItemContext catalogItemContext = default(ICatalogItemContext);
			return ReportUrl.BuildPathUri(currentICatalogItemContext, true, initialUrl, unparsedParameters, out catalogItemContext);
		}

		internal static string BuildPathUri(ICatalogItemContext currentICatalogItemContext, bool checkProtocol, string initialUrl, NameValueCollection unparsedParameters, out ICatalogItemContext newContext)
		{
			newContext = null;
			if (currentICatalogItemContext == null)
			{
				return initialUrl;
			}
			string text = null;
			try
			{
				text = currentICatalogItemContext.CombineUrl(initialUrl, checkProtocol, unparsedParameters, out newContext);
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception)
			{
				throw new RenderingObjectModelException(ErrorCode.rsMalformattedURL);
			}
			if (!currentICatalogItemContext.IsSupportedProtocol(text, checkProtocol))
			{
				throw new RenderingObjectModelException(ErrorCode.rsUnsupportedURLProtocol, text.MarkAsPrivate());
			}
			return text;
		}

		internal static string BuildDrillthroughUrl(ICatalogItemContext currentCatalogItemContext, string initialUrl, NameValueCollection parameters)
		{
			ICatalogItemContext catalogItemContext = default(ICatalogItemContext);
			string text = ReportUrl.BuildPathUri(currentCatalogItemContext, true, initialUrl, parameters, out catalogItemContext);
			if (text != null && catalogItemContext != null)
			{
				CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(catalogItemContext, catalogItemContext.RSRequestParameters);
				catalogItemUrlBuilder.AppendReportParameters(parameters);
				Uri uri = new Uri(catalogItemUrlBuilder.ToString());
				return uri.AbsoluteUri;
			}
			return null;
		}

		internal static ReportUrl BuildHyperlinkUrl(RenderingContext renderingContext, ObjectType objectType, string objectName, string propertyName, ICatalogItemContext itemContext, string initialUrl)
		{
			ReportUrl result = null;
			if (initialUrl == null)
			{
				return null;
			}
			bool flag = false;
			try
			{
				string text = initialUrl;
				bool flag2 = default(bool);
				bool flag3 = itemContext.IsReportServerPathOrUrl(text, false, out flag2);
				NameValueCollection unparsedParameters = null;
				if (flag3 && flag2)
				{
					itemContext.PathManager.ExtractFromUrl(text, out text, out unparsedParameters);
					if (text == null || text.Length == 0)
					{
						flag = true;
						text = null;
						result = null;
					}
				}
				if (text != null)
				{
					result = new ReportUrl(itemContext, text, false, unparsedParameters);
				}
			}
			catch (ItemNotFoundException)
			{
				flag = true;
			}
			catch (RenderingObjectModelException)
			{
				flag = true;
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception)
			{
				flag = true;
			}
			if (flag)
			{
				renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidURLProtocol, Severity.Warning, objectType, objectName, propertyName, initialUrl.MarkAsPrivate());
			}
			return result;
		}

		public Uri ToUri()
		{
			if (this.IsOldSnapshot)
			{
				return this.m_renderUrl.ToUri();
			}
			return this.m_pathUri;
		}

		public override string ToString()
		{
			if (this.IsOldSnapshot)
			{
				return this.m_renderUrl.ToString();
			}
			return this.m_pathUri.AbsoluteUri;
		}
	}
}
