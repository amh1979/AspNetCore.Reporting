using System;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal static class CatalogItemNameUtility
	{
		public const char PathSeparatorChar = '/';

		public static int MaxItemPathLength
		{
			get
			{
				return 260;
			}
		}

		public static void SplitPath(string itemPath, out string itemName, out string parentPath)
		{
			if (itemPath == null)
			{
				parentPath = null;
				itemName = null;
			}
			else
			{
				int num = itemPath.LastIndexOf('/');
				int num2 = 0;
				if (itemPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
				{
					num2 = "http://".Length;
				}
				else if (itemPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
				{
					num2 = "https://".Length;
				}
				if (num < num2)
				{
					parentPath = null;
					itemName = string.Empty;
				}
				else
				{
					parentPath = itemPath.Substring(0, num);
					itemName = itemPath.Substring(num + 1);
				}
			}
		}

		public static bool IsWebUrl(string path)
		{
			if (!path.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
			{
				return path.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}
	}
}
