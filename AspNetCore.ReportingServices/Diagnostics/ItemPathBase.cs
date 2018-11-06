using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal abstract class ItemPathBase
	{
		protected readonly string m_value;

		protected string m_editSessionID;

		public string Value
		{
			get
			{
				return this.m_value;
			}
		}

		public string EditSessionID
		{
			get
			{
				return this.m_editSessionID;
			}
			set
			{
				this.m_editSessionID = value;
			}
		}

		public bool IsEditSession
		{
			get
			{
				return !string.IsNullOrEmpty(this.m_editSessionID);
			}
		}

		public virtual string FullEditSessionIdentifier
		{
			get
			{
				if (this.IsEditSession)
				{
					if (!this.Value.Contains("|") && !this.EditSessionID.Contains("|"))
					{
						return string.Format(CultureInfo.InvariantCulture, "|{0}|@|{1}|", this.EditSessionID, this.Value);
					}
					throw new InternalCatalogException("Unexpected character in ItemPath or EditSessionID");
				}
				return this.Value;
			}
		}

		protected ItemPathBase(string itemPath)
		{
			if (!ItemPathBase.ParseInternalItemPathParts(itemPath, out this.m_editSessionID, out this.m_value))
			{
				this.m_value = ((itemPath != null) ? itemPath.Trim() : null);
				this.m_editSessionID = null;
			}
		}

		protected ItemPathBase(string itemPath, string editSessionID)
		{
			this.m_value = itemPath;
			this.m_editSessionID = editSessionID;
		}

		public static string SafeValue(ItemPathBase path)
		{
			if (path == null)
			{
				return null;
			}
			return path.Value;
		}

		public static string SafeEditSessionID(ItemPathBase path)
		{
			if (path == null)
			{
				return null;
			}
			return path.EditSessionID;
		}

		public static bool IsNullOrEmpty(ItemPathBase path)
		{
			if (path != null)
			{
				return string.IsNullOrEmpty(path.Value);
			}
			return true;
		}

		public override string ToString()
		{
			return this.m_value;
		}

		public static string GetLocalPath(string path)
		{
			return path;
		}

		public static int CatalogCompare(ItemPathBase a, string b)
		{
			int num = Localization.CatalogCultureCompare(ItemPathBase.SafeValue(a), b);
			if (num == 0 && a != null && a.IsEditSession)
			{
				return 1;
			}
			return num;
		}

		public static int CatalogCompare(ItemPathBase a, ItemPathBase b)
		{
			int num = Localization.CatalogCultureCompare(ItemPathBase.SafeValue(a), ItemPathBase.SafeValue(b));
			if (num == 0)
			{
				return string.CompareOrdinal(ItemPathBase.SafeEditSessionID(a), ItemPathBase.SafeEditSessionID(b));
			}
			return num;
		}

		public static string GetParentPathForSharePoint(string path)
		{
			return path;
		}

		public static string GetEditSessionID(string path)
		{
			string result = null;
			string text = null;
			ItemPathBase.ParseInternalItemPathParts(path, out result, out text);
			return result;
		}

		public static bool ParseInternalItemPathParts(string path, out string editSessionID, out string itemPath)
		{
			if (!string.IsNullOrEmpty(path))
			{
				Match match = Regex.Match(path, "  ^\\|\r\n        ([a-z0-9]{24})\r\n    \\|\r\n    @\r\n    \\|\r\n        ([^\\|]+)\r\n    \\|$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
				if (match.Success)
				{
					editSessionID = match.Groups[1].Value.Trim();
					itemPath = match.Groups[2].Value.Trim();
					return true;
				}
			}
			editSessionID = null;
			itemPath = null;
			return false;
		}
	}
}
