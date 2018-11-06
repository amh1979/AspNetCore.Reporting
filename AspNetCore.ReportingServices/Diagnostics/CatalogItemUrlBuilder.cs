using AspNetCore.ReportingServices.Common;
using System.Collections.Specialized;
using System.Text;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class CatalogItemUrlBuilder
	{
		private StringBuilder m_urlString;

		private IReportParameterLookup m_paramLookup;

		private IPathTranslator m_pathTranslator;

		private IPathManager m_pathManager;

		private static readonly string EncodedParameterNullSuffix = UrlUtil.UrlEncode(":isnull");

		private static readonly string EncodedCatalogParameterPrefix = UrlUtil.UrlEncode("rs:");

		private static readonly string EncodedRenderingParameterPrefix = UrlUtil.UrlEncode("rc:");

		private static readonly string EncodedReportParameterPrefix = UrlUtil.UrlEncode("");

		private static readonly string EncodedUserNameParameterPrefix = UrlUtil.UrlEncode("dsu:");

		public static string NameValueCollectionToQueryString(NameValueCollection parameters)
		{
			if (parameters == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				string[] values = parameters.GetValues(i);
				if (values == null || values.Length == 0)
				{
					if (!flag)
					{
						stringBuilder.Append("&");
					}
					flag = false;
					stringBuilder.Append(UrlUtil.UrlEncode(key));
				}
				else
				{
					for (int j = 0; j < values.Length; j++)
					{
						if (!flag)
						{
							stringBuilder.Append("&");
						}
						flag = false;
						stringBuilder.Append(UrlUtil.UrlEncode(key));
						stringBuilder.Append("=");
						stringBuilder.Append(UrlUtil.UrlEncode(values[j]));
					}
				}
			}
			return stringBuilder.ToString();
		}

		private CatalogItemUrlBuilder(IPathManager pathManager)
		{
			this.m_pathManager = pathManager;
		}

		public CatalogItemUrlBuilder(string urlString)
		{
			this.m_urlString = new StringBuilder(urlString);
		}

		public CatalogItemUrlBuilder(ICatalogItemContext ctx)
			: this(ctx, false)
		{
		}

		public CatalogItemUrlBuilder(ICatalogItemContext ctx, IReportParameterLookup paramLookup)
			: this(ctx, false)
		{
			this.m_paramLookup = paramLookup;
		}

		public CatalogItemUrlBuilder(ICatalogItemContext ctx, bool isFolder)
		{
			this.m_pathTranslator = ctx.PathTranslator;
			this.m_pathManager = ctx.PathManager;
			this.Construct(ctx.HostRootUri, ctx.HostSpecificItemPath, false, true, isFolder);
		}

		public static CatalogItemUrlBuilder CreateNonServerBuilder(string serverVirtualFolderUrl, string itemPath, bool alreadyEscaped, bool addItemPathAsQuery)
		{
			CatalogItemUrlBuilder catalogItemUrlBuilder = new CatalogItemUrlBuilder(RSPathUtil.Instance);
			catalogItemUrlBuilder.Construct(serverVirtualFolderUrl, itemPath, alreadyEscaped, addItemPathAsQuery, false);
			return catalogItemUrlBuilder;
		}

		private void Construct(string serverVirtualFolderUrl, string itemPath, bool alreadyEscaped, bool addItemPathAsQuery, bool isFolder)
		{
			this.m_urlString = this.m_pathManager.ConstructUrlBuilder(this.m_pathTranslator, serverVirtualFolderUrl, itemPath, alreadyEscaped, addItemPathAsQuery, isFolder);
		}

		public override string ToString()
		{
			return this.m_urlString.ToString();
		}

		public void AppendUnparsedParameters(NameValueCollection parameters)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				string key = parameters.GetKey(i);
				string[] values = parameters.GetValues(i);
				if (key != null)
				{
					if (values != null)
					{
						for (int j = 0; j < values.Length; j++)
						{
							this.AppendOneParameter(string.Empty, key, values[j], false);
						}
					}
					else
					{
						this.AppendOneParameter(string.Empty, key, null, false);
					}
				}
			}
		}

		public void AppendReportParameter(string name, string val)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add(name, val);
			if (!this.ReplaceParametersWithExecParameterId(nameValueCollection))
			{
				this.AppendOneParameter(CatalogItemUrlBuilder.EncodedReportParameterPrefix, name, val);
			}
		}

		private void InternalAppendReportParameters(NameValueCollection parameters)
		{
			this.AppendParameterCollection(CatalogItemUrlBuilder.EncodedReportParameterPrefix, parameters);
		}

		public void AppendReportParameters(NameValueCollection parameters)
		{
			if (!this.ReplaceParametersWithExecParameterId(parameters))
			{
				this.InternalAppendReportParameters(parameters);
			}
		}

		private bool ReplaceParametersWithExecParameterId(NameValueCollection parameters)
		{
			string text = null;
			if (this.m_paramLookup != null && parameters != null)
			{
				text = this.m_paramLookup.GetReportParamsInstanceId(parameters);
			}
			if (text != null)
			{
				this.AppendCatalogParameter("StoredParametersID", text);
				return true;
			}
			return false;
		}

		public void AppendRenderingParameter(string name, string val)
		{
			this.AppendOneParameter(CatalogItemUrlBuilder.EncodedRenderingParameterPrefix, name, val);
		}

		public void AppendRenderingParameters(NameValueCollection parameters)
		{
			this.AppendParameterCollection(CatalogItemUrlBuilder.EncodedRenderingParameterPrefix, parameters);
		}

		public void AppendCatalogParameter(string name, string val)
		{
			this.AppendOneParameter(CatalogItemUrlBuilder.EncodedCatalogParameterPrefix, name, val);
		}

		public void AppendCatalogParameters(NameValueCollection parameters)
		{
			this.AppendParameterCollection(CatalogItemUrlBuilder.EncodedCatalogParameterPrefix, parameters);
		}

		private void AppendParameterCollection(string encodedPrefix, NameValueCollection parameters)
		{
			if (parameters != null)
			{
				for (int i = 0; i < parameters.Count; i++)
				{
					string key = parameters.GetKey(i);
					string[] values = parameters.GetValues(i);
					if (values == null)
					{
						this.AppendOneParameter(encodedPrefix, key, null);
					}
					else
					{
						for (int j = 0; j < values.Length; j++)
						{
							this.AppendOneParameter(encodedPrefix, key, values[j]);
						}
					}
				}
			}
		}

		private void AppendOneParameter(string encodedPrefix, string name, string val)
		{
			this.AppendOneParameter(encodedPrefix, name, val, true);
		}

		private void AppendOneParameter(string encodedPrefix, string name, string val, bool addNullSuffix)
		{
			this.m_urlString.Append('&');
			if (val != null)
			{
				this.m_urlString.Append(encodedPrefix);
				this.m_urlString.Append(CatalogItemUrlBuilder.EncodeUrlParameter(name));
				this.m_urlString.Append("=");
				this.m_urlString.Append(CatalogItemUrlBuilder.EncodeUrlParameter(val));
			}
			else
			{
				this.m_urlString.Append(encodedPrefix);
				this.m_urlString.Append(CatalogItemUrlBuilder.EncodeUrlParameter(name));
				if (addNullSuffix)
				{
					this.m_urlString.Append(CatalogItemUrlBuilder.EncodedParameterNullSuffix);
				}
				this.m_urlString.Append("=");
				this.m_urlString.Append(bool.TrueString);
			}
		}

		private static string EncodeUrlParameter(string param)
		{
			string text = UrlUtil.UrlEncode(param);
			return text.Replace("'", "%27");
		}
	}
}
