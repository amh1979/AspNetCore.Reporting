using System;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal static class Localization
	{
		public static string ClientBrowserCultureName
		{
			get
			{
				return Localization.ClientPrimaryCulture.Name;
			}
		}

		public static string ClientCurrentCultureName
		{
			get
			{
				return Localization.ClientPrimaryCulture.Name;
			}
		}

		public static CultureInfo ClientPrimaryCulture
		{
			get
			{
				return Thread.CurrentThread.CurrentCulture;
			}
		}

		public static CultureInfo SqlCulture
		{
			get
			{
				return Localization.ClientPrimaryCulture;
			}
		}

		public static CultureInfo DefaultReportServerSpecificCulture
		{
			get
			{
				CultureInfo installedUICulture = CultureInfo.InstalledUICulture;
				if (installedUICulture.IsNeutralCulture)
				{
					CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(installedUICulture.Name);
					return new CultureInfo(cultureInfo.LCID, false);
				}
				return installedUICulture;
			}
		}

		public static CultureInfo CatalogCulture
		{
			get
			{
				return CultureInfo.InvariantCulture;
			}
		}

		public static int CatalogCultureCompare(string a, string b)
		{
			return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
