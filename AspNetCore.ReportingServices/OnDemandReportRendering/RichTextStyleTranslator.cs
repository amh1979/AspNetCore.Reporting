using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RichTextStyleTranslator
	{
		internal class StyleEnumConstants
		{
			internal const string Default = "Default";

			internal const string Normal = "Normal";

			internal const string General = "General";

			internal const string Center = "Center";

			internal const string Left = "Left";

			internal const string Right = "Right";

			internal const string Thin = "Thin";

			internal const string ExtraLight = "ExtraLight";

			internal const string Light = "Light";

			internal const string Lighter = "Lighter";

			internal const string Medium = "Medium";

			internal const string SemiBold = "SemiBold";

			internal const string Bold = "Bold";

			internal const string Bolder = "Bolder";

			internal const string ExtraBold = "ExtraBold";

			internal const string Heavy = "Heavy";

			internal const string FontWeight100 = "100";

			internal const string FontWeight200 = "200";

			internal const string FontWeight300 = "300";

			internal const string FontWeight400 = "400";

			internal const string FontWeight500 = "500";

			internal const string FontWeight600 = "600";

			internal const string FontWeight700 = "700";

			internal const string FontWeight800 = "800";

			internal const string FontWeight900 = "900";
		}

		internal static bool CompareWithInvariantCulture(string str1, string str2)
		{
			return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal static bool TranslateHtmlFontSize(string value, out string translatedSize)
		{
			int num = default(int);
			if (int.TryParse(value, out num))
			{
				if (num <= 0)
				{
					translatedSize = "7.5pt";
				}
				else
				{
					switch (num)
					{
					case 1:
						translatedSize = "7.5pt";
						break;
					case 2:
						translatedSize = "10pt";
						break;
					case 3:
						translatedSize = "11pt";
						break;
					case 4:
						translatedSize = "13.5pt";
						break;
					case 5:
						translatedSize = "18pt";
						break;
					case 6:
						translatedSize = "24pt";
						break;
					default:
						translatedSize = "36pt";
						break;
					}
				}
				return true;
			}
			translatedSize = null;
			return false;
		}

		internal static string TranslateHtmlColor(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (value[0] == '#')
				{
					return value;
				}
				if (char.IsDigit(value[0]))
				{
					return "#" + value;
				}
			}
			return value;
		}

		internal static bool TranslateFontWeight(string styleString, out FontWeights fontWieght)
		{
			fontWieght = FontWeights.Normal;
			if (!string.IsNullOrEmpty(styleString))
			{
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Normal", styleString))
				{
					fontWieght = FontWeights.Normal;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Bold", styleString))
				{
					fontWieght = FontWeights.Bold;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Bolder", styleString))
				{
					fontWieght = FontWeights.Bold;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("100", styleString))
				{
					fontWieght = FontWeights.Thin;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("200", styleString))
				{
					fontWieght = FontWeights.ExtraLight;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("300", styleString))
				{
					fontWieght = FontWeights.Light;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("400", styleString))
				{
					fontWieght = FontWeights.Normal;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("500", styleString))
				{
					fontWieght = FontWeights.Medium;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("600", styleString))
				{
					fontWieght = FontWeights.SemiBold;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("700", styleString))
				{
					fontWieght = FontWeights.Bold;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("800", styleString))
				{
					fontWieght = FontWeights.ExtraBold;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("900", styleString))
				{
					fontWieght = FontWeights.Heavy;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Thin", styleString))
				{
					fontWieght = FontWeights.Thin;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("ExtraLight", styleString))
				{
					fontWieght = FontWeights.ExtraLight;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Light", styleString))
				{
					fontWieght = FontWeights.Light;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Lighter", styleString))
				{
					fontWieght = FontWeights.Light;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Medium", styleString))
				{
					fontWieght = FontWeights.Medium;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("SemiBold", styleString))
				{
					fontWieght = FontWeights.SemiBold;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("ExtraBold", styleString))
				{
					fontWieght = FontWeights.ExtraBold;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Heavy", styleString))
				{
					fontWieght = FontWeights.Heavy;
					goto IL_01b6;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Default", styleString))
				{
					fontWieght = FontWeights.Normal;
					goto IL_01b6;
				}
				return false;
			}
			return false;
			IL_01b6:
			return true;
		}

		internal static bool TranslateTextAlign(string styleString, out TextAlignments textAlignment)
		{
			textAlignment = TextAlignments.General;
			if (!string.IsNullOrEmpty(styleString))
			{
				if (RichTextStyleTranslator.CompareWithInvariantCulture("General", styleString))
				{
					textAlignment = TextAlignments.General;
					goto IL_0067;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Left", styleString))
				{
					textAlignment = TextAlignments.Left;
					goto IL_0067;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Center", styleString))
				{
					textAlignment = TextAlignments.Center;
					goto IL_0067;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Right", styleString))
				{
					textAlignment = TextAlignments.Right;
					goto IL_0067;
				}
				if (RichTextStyleTranslator.CompareWithInvariantCulture("Default", styleString))
				{
					textAlignment = TextAlignments.General;
					goto IL_0067;
				}
				return false;
			}
			return false;
			IL_0067:
			return true;
		}
	}
}
