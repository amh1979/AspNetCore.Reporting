using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics;
using System;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportUrlBuilder
	{
		private string m_replacementRoot;

		private CatalogItemUrlBuilder m_catalogItemUrlBuilder;

		private bool m_hasReplacement;

		private bool m_useRepacementRoot = true;

		internal ReportUrlBuilder(RenderingContext reportContext, ICatalogItemContext changedContext, string initialUrl, string replacementRoot)
		{
			ICatalogItemContext currentICatalogItemContext = (changedContext == null) ? reportContext.TopLevelReportContext : changedContext;
			ICatalogItemContext catalogItemContext = default(ICatalogItemContext);
			ReportUrl.BuildPathUri(currentICatalogItemContext, initialUrl, (NameValueCollection)null, out catalogItemContext);
			this.m_catalogItemUrlBuilder = new CatalogItemUrlBuilder(catalogItemContext, catalogItemContext.RSRequestParameters);
			this.m_replacementRoot = replacementRoot;
		}

		internal ReportUrlBuilder(RenderingContext reportContext, string initialUrl, string replacementRoot)
		{
			ICatalogItemContext topLevelReportContext = reportContext.TopLevelReportContext;
			ICatalogItemContext catalogItemContext = default(ICatalogItemContext);
			ReportUrl.BuildPathUri(topLevelReportContext, initialUrl, (NameValueCollection)null, out catalogItemContext);
			this.m_catalogItemUrlBuilder = new CatalogItemUrlBuilder(topLevelReportContext, topLevelReportContext.RSRequestParameters);
			this.m_replacementRoot = replacementRoot;
		}

		internal ReportUrlBuilder(RenderingContext reportContext, string initialUrl, bool useReplacementRoot, bool addReportParameters)
		{
			ICatalogItemContext topLevelReportContext = reportContext.TopLevelReportContext;
			ICatalogItemContext catalogItemContext = default(ICatalogItemContext);
			string pathOrUrl = ReportUrl.BuildPathUri(topLevelReportContext, initialUrl, (NameValueCollection)null, out catalogItemContext);
			this.m_catalogItemUrlBuilder = new CatalogItemUrlBuilder(topLevelReportContext, topLevelReportContext.RSRequestParameters);
			if (addReportParameters)
			{
				this.m_catalogItemUrlBuilder.AppendReportParameters(reportContext.TopLevelReportContext.RSRequestParameters.ReportParameters);
			}
			this.m_useRepacementRoot = useReplacementRoot;
			bool flag = default(bool);
			if (reportContext != null && reportContext.TopLevelReportContext.IsReportServerPathOrUrl(pathOrUrl, true, out flag))
			{
				this.m_replacementRoot = reportContext.ReplacementRoot;
			}
		}

		public override string ToString()
		{
			return this.m_catalogItemUrlBuilder.ToString();
		}

		public Uri ToUri()
		{
			string uriString;
			if (this.m_replacementRoot != null)
			{
				if (this.m_useRepacementRoot)
				{
					this.AddReplacementRoot();
					uriString = this.m_replacementRoot + UrlUtil.UrlEncode(this.m_catalogItemUrlBuilder.ToString());
				}
				else
				{
					uriString = this.m_catalogItemUrlBuilder.ToString();
				}
			}
			else
			{
				uriString = this.m_catalogItemUrlBuilder.ToString();
			}
			return new Uri(uriString);
		}

		public void AddReplacementRoot()
		{
			if (!this.m_hasReplacement)
			{
				this.m_hasReplacement = true;
				if (this.m_replacementRoot != null)
				{
					this.m_catalogItemUrlBuilder.AppendRenderingParameter("ReplacementRoot", this.m_replacementRoot);
				}
			}
		}

		public void AddParameters(NameValueCollection urlParameters, UrlParameterType parameterType)
		{
			switch (parameterType)
			{
			case UrlParameterType.RenderingParameter:
				this.m_catalogItemUrlBuilder.AppendRenderingParameters(urlParameters);
				break;
			case UrlParameterType.ReportParameter:
				this.m_catalogItemUrlBuilder.AppendReportParameters(urlParameters);
				break;
			case UrlParameterType.ServerParameter:
				this.m_catalogItemUrlBuilder.AppendCatalogParameters(urlParameters);
				break;
			}
		}

		public void AddParameter(string name, string val, UrlParameterType parameterType)
		{
			switch (parameterType)
			{
			case UrlParameterType.RenderingParameter:
				this.m_catalogItemUrlBuilder.AppendRenderingParameter(name, val);
				break;
			case UrlParameterType.ReportParameter:
				this.m_catalogItemUrlBuilder.AppendReportParameter(name, val);
				break;
			case UrlParameterType.ServerParameter:
				this.m_catalogItemUrlBuilder.AppendCatalogParameter(name, val);
				break;
			}
		}
	}
}
