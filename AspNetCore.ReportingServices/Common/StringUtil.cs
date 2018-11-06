using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.Common
{
	internal static class StringUtil
	{
		private sealed class StringComparerWithOptions : EqualityComparer<string>
		{
			private readonly CompareInfo m_compareInfo;

			private readonly CompareOptions m_options;

			internal StringComparerWithOptions(CultureInfo culture, CompareOptions compareOptions)
			{
				this.m_compareInfo = culture.CompareInfo;
				this.m_options = compareOptions;
			}

			public override bool Equals(string x, string y)
			{
				return this.m_compareInfo.Compare(x, y, this.m_options) == 0;
			}

			public override int GetHashCode(string obj)
			{
				return this.m_compareInfo.GetSortKey(obj, this.m_options).GetHashCode();
			}
		}

		private const string SuffixGroup = "suffix";

		public static readonly string ClsCompliantIdentifierPattern = "[\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}][\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]*";

		private static readonly Regex m_digitSuffixRegex = new Regex("(?<suffix>\\d{1,9})$");

		public static int? GetDigitSuffix(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				Match match = StringUtil.m_digitSuffixRegex.Match(value);
				if (match.Success)
				{
					return int.Parse(match.Groups["suffix"].Value, CultureInfo.InvariantCulture);
				}
			}
			return null;
		}

		public static string SetDigitSuffix(string value, int suffix)
		{
			int? digitSuffix = StringUtil.GetDigitSuffix(value);
			string text = suffix.ToString(CultureInfo.InvariantCulture);
			if (!digitSuffix.HasValue)
			{
				return value + text;
			}
			return StringUtil.m_digitSuffixRegex.Replace(value, text);
		}

		public static string IncrementDigitSuffix(string value, int defaultSuffix)
		{
			int? digitSuffix = StringUtil.GetDigitSuffix(value);
			if (!digitSuffix.HasValue)
			{
				return StringUtil.SetDigitSuffix(value, defaultSuffix);
			}
			return StringUtil.SetDigitSuffix(value, digitSuffix.Value + 1);
		}

		public static string IncrementDigitSuffix(string value)
		{
			return StringUtil.IncrementDigitSuffix(value, 2);
		}

		public static string TrimToMaxLength(string value, int maxLength)
		{
			value = (value ?? string.Empty);
			if (value.Length <= maxLength)
			{
				return value;
			}
			Match match = StringUtil.m_digitSuffixRegex.Match(value);
			if (match.Success)
			{
				int num = maxLength - match.Groups["suffix"].Length;
				if (num < 0)
				{
					throw new ArgumentOutOfRangeException("maxLength");
				}
				return value.Substring(0, num) + match.Groups["suffix"].Value;
			}
			return value.Substring(0, maxLength);
		}

		public static string RemoveAmpersandEllipsis(string text)
		{
			return StringUtil.RemoveEllipsis(StringUtil.RemoveAmpersand(text));
		}

		public static string RemoveAmpersand(string text)
		{
			if (text != null)
			{
				int num = text.IndexOf('&');
				if (num >= 0)
				{
					return text.Remove(num, 1);
				}
			}
			return text;
		}

		public static string RemoveEllipsis(string text)
		{
			if (text != null)
			{
				int num = text.Length - 3;
				if (num > 0 && text[num] == '.' && text[num + 1] == '.' && text[num + 2] == '.')
				{
					return text.Remove(num);
				}
			}
			return text;
		}

		public static string RemoveLastSubstring(string text, char separator)
		{
			int num = text.LastIndexOf(separator);
			if (num < 0)
			{
				return text;
			}
			return text.Substring(0, num);
		}

		public static string GetLastSubstring(string text, char separator)
		{
			int num = text.LastIndexOf(separator);
			if (num < 0)
			{
				return text;
			}
			return text.Substring(num + 1);
		}

		public static string FormatInvariant(string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}

		public static string Join(string separator, IList<string> strings)
		{
			return StringUtil.Join(separator, strings, 0, strings.Count);
		}

		public static string Join(string separator, IList<string> strings, int startIndex, int count)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count >= 0 && startIndex + count <= strings.Count)
			{
				if (count == 0)
				{
					return string.Empty;
				}
				int num = 0;
				int length = separator.Length;
				for (int i = 0; i < count; i++)
				{
					num += strings[i + startIndex].Length + length;
				}
				num -= length;
				if (num == 0)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder(num, num);
				stringBuilder.Append(strings[startIndex]);
				for (int j = 1; j < count; j++)
				{
					stringBuilder.Append(separator);
					stringBuilder.Append(strings[j + startIndex]);
				}
				return stringBuilder.ToString();
			}
			throw new ArgumentOutOfRangeException("count");
		}

		public static string NormalizeLineBreaks(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			StringReader stringReader = new StringReader(s);
			StringWriter stringWriter = new StringWriter(CultureInfo.CurrentCulture);
			bool flag = true;
			string value;
			while ((value = stringReader.ReadLine()) != null)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringWriter.WriteLine();
				}
				stringWriter.Write(value);
			}
			return stringWriter.ToString();
		}

		public static bool EqualsTrimmedWithNormalizedLineBreaks(string s1, string s2)
		{
			if (s1 != null)
			{
				s1 = s1.Trim();
			}
			if (s2 != null)
			{
				s2 = s2.Trim();
			}
			return StringUtil.NormalizeLineBreaks(s1) == StringUtil.NormalizeLineBreaks(s2);
		}

		public static string BuildErrorMessage(Exception e)
		{
			if (e.InnerException != null)
			{
				return StringUtil.BuildErrorMessage(e.InnerException) + "\r\n----------------------------\r\n" + e.Message;
			}
			return e.Message;
		}

		public static string MakeCLSNameFromFilename(string path)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			return StringUtil.GetClsCompliantIdentifier(fileNameWithoutExtension, null);
		}

		public static StringComparer GetClsCompliantComparer()
		{
			return StringComparer.Ordinal;
		}

		public static int CompareClsCompliantIdentifiers(string s1, string s2)
		{
			return StringUtil.CompareClsCompliantIdentifiers(s1, s2, false);
		}

		public static int CompareClsCompliantIdentifiers(string s1, string s2, bool ignoreCase)
		{
			StringComparison comparisonType = (StringComparison)(ignoreCase ? 5 : 4);
			return string.Compare(s1, s2, comparisonType);
		}

		public static string GetClsCompliantIdentifier(string candidate, string defaultIdentifier)
		{
			StringBuilder stringBuilder = new StringBuilder();
			candidate = (candidate ?? string.Empty);
			if (candidate.Length == 0 || !StringUtil.IsClsCompliantIdentifierChar(candidate[0], true))
			{
				stringBuilder.Append(defaultIdentifier);
			}
			for (int i = 0; i < candidate.Length; i++)
			{
				if (StringUtil.IsClsCompliantIdentifierChar(candidate[i], stringBuilder.Length == 0))
				{
					stringBuilder.Append(candidate[i]);
				}
			}
			return stringBuilder.ToString();
		}

		public static bool IsClsCompliantIdentifier(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			if (!StringUtil.IsClsCompliantIdentifierChar(s[0], true))
			{
				return false;
			}
			for (int i = 1; i < s.Length; i++)
			{
				if (!StringUtil.IsClsCompliantIdentifierChar(s[i], false))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsClsCompliantIdentifierChar(char c, bool firstChar)
		{
			switch (char.GetUnicodeCategory(c))
			{
			case UnicodeCategory.UppercaseLetter:
			case UnicodeCategory.LowercaseLetter:
			case UnicodeCategory.TitlecaseLetter:
			case UnicodeCategory.ModifierLetter:
			case UnicodeCategory.OtherLetter:
			case UnicodeCategory.LetterNumber:
				return true;
			case UnicodeCategory.NonSpacingMark:
			case UnicodeCategory.SpacingCombiningMark:
			case UnicodeCategory.DecimalDigitNumber:
			case UnicodeCategory.Format:
			case UnicodeCategory.ConnectorPunctuation:
				return !firstChar;
			default:
				return false;
			}
		}

		public static string GenerateUniqueName(string candidate, string baseName, Predicate<string> nameExists)
		{
			if (baseName == null)
			{
				throw new ArgumentNullException("baseName");
			}
			if (!string.IsNullOrEmpty(candidate))
			{
				if (!nameExists(candidate))
				{
					return candidate;
				}
				baseName = candidate;
			}
			int num = 1;
			while (true)
			{
				candidate = baseName + num;
				if (nameExists(candidate))
				{
					num++;
					continue;
				}
				break;
			}
			return candidate;
		}

		public static string SplitName(string name)
		{
			return Regex.Replace(name, "(\\p{Ll})(\\p{Lu})|_+", "$1 $2");
		}

		public static IEqualityComparer<string> CreateComparer(CultureInfo culture, CompareOptions compareOptions)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (compareOptions != 0 && compareOptions != CompareOptions.IgnoreCase)
			{
				return new StringComparerWithOptions(culture, compareOptions);
			}
			return StringComparer.Create(culture, compareOptions == CompareOptions.IgnoreCase);
		}
	}
}
