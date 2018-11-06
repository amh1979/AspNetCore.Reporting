using System;

namespace AspNetCore.Reporting
{
	internal static class SPUrlUtility
	{
		private static readonly string[] m_rgstrAllowedProtocols = new string[12]
		{
			"http://",
			"https://",
			"file://",
			"file:\\\\",
			"ftp://",
			"mailto:",
			"msn:",
			"news:",
			"nntp:",
			"pnm://",
			"mms://",
			"outlook:"
		};

		public static string[] AllowedProtocols
		{
			get
			{
				return SPUrlUtility.m_rgstrAllowedProtocols;
			}
		}

		public static bool IsProtocolAllowed(string fullOrRelativeUrl)
		{
			return SPUrlUtility.IsProtocolAllowed(fullOrRelativeUrl, true);
		}

		public static bool IsProtocolAllowed(string fullOrRelativeUrl, bool allowRelativeUrl)
		{
			if (fullOrRelativeUrl != null && fullOrRelativeUrl.Length > 0)
			{
				fullOrRelativeUrl = fullOrRelativeUrl.Split('?')[0];
				int num = fullOrRelativeUrl.IndexOf(':');
				if (num == -1)
				{
					if (allowRelativeUrl)
					{
						return true;
					}
					return false;
				}
				if (SPUrlUtility.m_rgstrAllowedProtocols == null)
				{
					return false;
				}
				fullOrRelativeUrl = fullOrRelativeUrl.TrimStart();
				string[] rgstrAllowedProtocols = SPUrlUtility.m_rgstrAllowedProtocols;
				foreach (string value in rgstrAllowedProtocols)
				{
					if (fullOrRelativeUrl.StartsWith(value, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
			if (allowRelativeUrl)
			{
				return true;
			}
			return false;
		}
	}
}
