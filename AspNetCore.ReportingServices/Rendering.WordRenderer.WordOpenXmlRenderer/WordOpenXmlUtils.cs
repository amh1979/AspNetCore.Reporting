using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal static class WordOpenXmlUtils
	{
		public static string CleanName(string name)
		{
			name = name.Replace('\\', '/');
			if (!name.StartsWith("/", StringComparison.Ordinal) && !name.StartsWith("#", StringComparison.Ordinal))
			{
				return "/" + name;
			}
			return name;
		}

		internal static void CopyStream(Stream src, Stream dest)
		{
			WordOpenXmlUtils.CopyStream(src, dest, src.Length - src.Position);
		}

		internal static void CopyStream(Stream src, Stream dest, long amount)
		{
			byte[] buffer = new byte[1024];
			long num = amount;
			while (num > 0)
			{
				int num2 = src.Read(buffer, 0, (int)Math.Min(num, 1024L));
				num -= num2;
				dest.Write(buffer, 0, num2);
			}
		}

		internal static void FailSerializable()
		{
			RSTrace.ExcelRendererTracer.Assert(false);
		}

		internal static void FailCodingError()
		{
			RSTrace.ExcelRendererTracer.Assert(false);
		}

		public static string ToTwips(float mm, float minTwips, float maxTwips)
		{
			float num = (float)(mm / 25.399999618530273);
			float num2 = (float)(num * 1440.0);
			if (num2 < minTwips)
			{
				num2 = minTwips;
			}
			else if (num2 > maxTwips)
			{
				num2 = maxTwips;
			}
			return Math.Floor((double)num2).ToString(CultureInfo.InvariantCulture);
		}

		public static int ToTwips(float mm)
		{
			float num = (float)(mm / 25.399999618530273);
			float num2 = (float)(num * 1440.0);
			if (num2 < -2147483648.0)
			{
				num2 = -2.14748365E+09f;
			}
			else if (num2 > 2147483648.0)
			{
				num2 = 2.14748365E+09f;
			}
			return (int)Math.Floor((double)num2);
		}

		public static int ToEmus(float mm, int minEmus, int maxEmus)
		{
			int num = (int)Math.Round(mm * 36000.0);
			if (num > maxEmus)
			{
				num = maxEmus;
			}
			else if (num < minEmus)
			{
				num = minEmus;
			}
			return num;
		}

		public static int PixelsToEmus(int pixels, double resolution, int minEmus, int maxEmus)
		{
			int num = (int)((double)pixels / resolution * 914400.0);
			if (num > maxEmus)
			{
				num = maxEmus;
			}
			else if (num < minEmus)
			{
				num = minEmus;
			}
			return num;
		}

		public static int PointsToTwips(double points, double minTwips, double maxTwips)
		{
			double num = points * 20.0;
			if (num < minTwips)
			{
				num = minTwips;
			}
			else if (num > maxTwips)
			{
				num = maxTwips;
			}
			return (int)Math.Floor(num);
		}

		public static int PointsToTwips(double points)
		{
			double d = points * 20.0;
			return (int)Math.Floor(d);
		}

		public static string TwipsToString(long twips, int minTwips, int maxTwips)
		{
			if (twips < minTwips)
			{
				twips = minTwips;
			}
			else if (twips > maxTwips)
			{
				twips = maxTwips;
			}
			return twips.ToString(CultureInfo.InvariantCulture);
		}

		public static string RgbColor(Color color)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
		}

		internal static bool GetTextAlignForType(TypeCode typeCode)
		{
			bool flag = false;
			switch (typeCode)
			{
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				return true;
			default:
				return false;
			}
		}

		internal static Size SizeImage(Size image, Size box, Size.Strategy strategy)
		{
			Size result = default(Size);
			switch (strategy)
			{
			case Size.Strategy.AutoSize:
				result = image;
				return result;
			case Size.Strategy.Fit:
				result = box;
				return result;
			case Size.Strategy.FitProportional:
			{
				float num = (float)box.Width / (float)image.Width;
				float num2 = (float)box.Height / (float)image.Height;
				if (num2 < num)
				{
					num = num2;
				}
				result.Height = (int)((float)image.Height * num);
				result.Width = (int)((float)image.Width * num);
				break;
			}
			case Size.Strategy.Clip:
				result = image;
				return result;
			}
			return result;
		}

		internal static string ThousandthsOfAPercentInverse(double numerator, double denominator)
		{
			return (100000 - (int)(numerator / denominator * 100000.0)).ToString(CultureInfo.InvariantCulture);
		}

		public static string EscapeChar(char c)
		{
			switch (c)
			{
			case '<':
				return "&lt;";
			case '>':
				return "&gt;";
			case '"':
				return "&quot;";
			case '\'':
				return "&apos;";
			case '&':
				return "&amp;";
			case '\0':
				return "";
			default:
				return c.ToString(CultureInfo.InvariantCulture);
			}
		}

		public static string Escape(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			foreach (char c in text)
			{
				stringBuilder.Append(WordOpenXmlUtils.EscapeChar(c));
			}
			return stringBuilder.ToString();
		}
	}
}
