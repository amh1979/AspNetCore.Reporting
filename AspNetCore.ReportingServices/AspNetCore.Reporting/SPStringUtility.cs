using System.Text.RegularExpressions;

namespace AspNetCore.Reporting
{
	internal static class SPStringUtility
	{
		public static string RemoveNonAlphaNumericChars(string value)
		{
			if (value != null && value.Length != 0)
			{
				return Regex.Replace(value, "[^a-zA-Z_0-9]+", string.Empty, RegexOptions.Compiled);
			}
			return value;
		}
	}
}
