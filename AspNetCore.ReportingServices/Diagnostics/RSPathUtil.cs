using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Specialized;
using System.Text;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class RSPathUtil : IPathManager
	{
		public static readonly RSPathUtil Instance = new RSPathUtil();

		private static readonly Uri m_absoluteUri = new Uri("http://q");

		private RSPathUtil()
		{
		}

		string IPathManager.RelativePathToAbsolutePath(string path, string reportPath)
		{
			Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);
			if (uri.IsAbsoluteUri)
			{
				return path;
			}
			string text = null;
			text = ((reportPath.Length != 0) ? reportPath : "/");
			Uri baseUri = new Uri("c:" + text);
			Uri uri2 = new Uri(baseUri, path);
			string components = uri2.GetComponents(UriComponents.Path, UriFormat.Unescaped);
			return components.Substring(2);
		}

		public bool IsSupportedUrl(string path, bool checkProtocol)
		{
			bool flag = default(bool);
			return ((IPathManager)this).IsSupportedUrl(path, checkProtocol, out flag);
		}

		bool IPathManager.IsSupportedUrl(string path, bool checkProtocol, out bool isInternal)
		{
			isInternal = false;
			if (!path.StartsWith("HTTP:", StringComparison.OrdinalIgnoreCase) && !path.StartsWith("HTTPS:", StringComparison.OrdinalIgnoreCase) && !path.StartsWith("FTP:", StringComparison.OrdinalIgnoreCase) && !path.StartsWith("MAILTO:", StringComparison.OrdinalIgnoreCase) && !path.StartsWith("NEWS:", StringComparison.OrdinalIgnoreCase) && !path.StartsWith("FILE:", StringComparison.OrdinalIgnoreCase))
			{
				if (RSPathUtil.ContainsOtherProtocol(path))
				{
					if (!checkProtocol)
					{
						return true;
					}
					return false;
				}
				isInternal = true;
				return true;
			}
			return true;
		}

		string IPathManager.EnsureReportNamePath(string reportNamePath)
		{
			return reportNamePath;
		}

		StringBuilder IPathManager.ConstructUrlBuilder(IPathTranslator pathTranslator, string serverVirtualFolderUrl, string itemPath, bool alreadyEscaped, bool addItemPathAsQuery, bool forceAddItemPathAsQuery)
		{
			if (!alreadyEscaped)
			{
				if (string.IsNullOrEmpty(serverVirtualFolderUrl))
				{
					serverVirtualFolderUrl = "http://reportserver";
				}
				else
				{
					Uri uri = new Uri(serverVirtualFolderUrl);
					serverVirtualFolderUrl = uri.AbsoluteUri;
				}
			}
			string value = UrlUtil.UrlEncode(itemPath);
			StringBuilder stringBuilder = new StringBuilder(serverVirtualFolderUrl);
			if (addItemPathAsQuery)
			{
				stringBuilder.Append("?");
			}
			stringBuilder.Append(value);
			return stringBuilder;
		}

		void IPathManager.ExtractFromUrl(string url, out string path, out NameValueCollection queryParameters)
		{
			RSTrace.CatalogTrace.Assert(false, "RSPathUtil.ExtractFromUrl cannot be used in local mode due to client profile restrictions");
			throw new NotImplementedException("RSPathUtil.ExtractFromUrl cannot be used in local mode due to client profile restrictions");
		}

		private static bool ContainsOtherProtocol(string path)
		{
			Uri absoluteUri = RSPathUtil.m_absoluteUri;
			Uri uri = new Uri(absoluteUri, path);
			if (uri.Scheme != absoluteUri.Scheme)
			{
				return true;
			}
			return false;
		}
	}
}
