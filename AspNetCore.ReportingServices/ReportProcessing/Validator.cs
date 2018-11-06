using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class Validator
	{
		internal static int DecimalPrecision = 5;

		internal static double NormalMin = 0.0;

		internal static double NegativeMin = 0.0 - Converter.Inches160;

		internal static double NormalMax = Converter.Inches160;

		internal static double BorderWidthMin = Converter.PtPoint25;

		internal static double BorderWidthMax = Converter.Pt20;

		internal static double FontSizeMin = Converter.Pt1;

		internal static double FontSizeMax = Converter.Pt200;

		internal static double PaddingMin = 0.0;

		internal static double PaddingMax = Converter.Pt1000;

		internal static double LineHeightMin = Converter.Pt1;

		internal static double LineHeightMax = Converter.Pt1000;

		private static Regex m_colorRegex = new Regex("^#(\\d|a|b|c|d|e|f){6}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		private static Regex m_colorRegexTransparency = new Regex("^#(\\d|a|b|c|d|e|f){8}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		private Validator()
		{
		}

		internal static bool ValidateColor(string color, out string newColor, bool allowTransparency)
		{
			if (color != null && (color.Length != 7 || color[0] != '#' || !Validator.m_colorRegex.Match(color).Success) && (!allowTransparency || color.Length != 9 || color[0] != '#' || !Validator.m_colorRegexTransparency.Match(color).Success))
			{
				string text = default(string);
				Color color2 = default(Color);
				if (Validator.ValidateReportColor(color, out text, out color2, allowTransparency))
				{
					if (text == null)
					{
						newColor = color;
					}
					else
					{
						newColor = text;
					}
					return true;
				}
				newColor = null;
				return false;
			}
			newColor = color;
			return true;
		}

		internal static bool ValidateColor(string color, out Color c)
		{
			return Validator.ValidateColor(color, out c, false);
		}

		internal static bool ValidateColor(string color, out Color c, bool allowTransparency)
		{
			if (color == null)
			{
				c = Color.Empty;
				return true;
			}
			if (color.Length == 7 && color[0] == '#' && Validator.m_colorRegex.Match(color).Success)
			{
				goto IL_0060;
			}
			if (allowTransparency && color.Length == 9 && color[0] == '#' && Validator.m_colorRegexTransparency.Match(color).Success)
			{
				goto IL_0060;
			}
			string text = default(string);
			if (Validator.ValidateReportColor(color, out text, out c, allowTransparency))
			{
				if (text != null)
				{
					Validator.ColorFromArgb(text, out c, allowTransparency);
				}
				return true;
			}
			c = Color.Empty;
			return false;
			IL_0060:
			Validator.ColorFromArgb(color, out c, allowTransparency);
			return true;
		}

		internal static void ParseColor(string color, out Color c)
		{
			Validator.ParseColor(color, out c, false);
		}

		internal static void ParseColor(string color, out Color c, bool allowTransparency)
		{
			if (color == null)
			{
				c = Color.Empty;
			}
			else
			{
				if (color.Length == 7 && color[0] == '#' && Validator.m_colorRegex.Match(color).Success)
				{
					goto IL_005f;
				}
				if (allowTransparency && color.Length == 9 && color[0] == '#' && Validator.m_colorRegexTransparency.Match(color).Success)
				{
					goto IL_005f;
				}
				c = Color.FromName(color);
			}
			return;
			IL_005f:
			Validator.ColorFromArgb(color, out c, allowTransparency);
		}

		private static void ColorFromArgb(string color, out Color c, bool allowTransparency)
		{
			try
			{
				if (!allowTransparency && color.Length != 7)
				{
					c = Color.FromArgb(0, 0, 0);
				}
				else
				{
					c = Color.FromArgb(Convert.ToInt32(color.Substring(1), 16));
					if (color.Length == 7)
					{
						c = Color.FromArgb(255, c);
					}
				}
			}
			catch
			{
				c = Color.FromArgb(0, 0, 0);
			}
		}

		private static bool ValidateReportColor(string color, out string newColor, out Color c, bool allowTransparency)
		{
			c = Color.FromName(color);
			if (c.A == 0 && c.R == 0 && c.G == 0 && c.B == 0)
			{
				if (string.Compare("LightGrey", color, StringComparison.OrdinalIgnoreCase) == 0)
				{
					newColor = "#d3d3d3";
					return true;
				}
				newColor = null;
				return false;
			}
			switch (c.ToKnownColor())
			{
			case KnownColor.ActiveBorder:
			case KnownColor.ActiveCaption:
			case KnownColor.ActiveCaptionText:
			case KnownColor.AppWorkspace:
			case KnownColor.Control:
			case KnownColor.ControlDark:
			case KnownColor.ControlDarkDark:
			case KnownColor.ControlLight:
			case KnownColor.ControlLightLight:
			case KnownColor.ControlText:
			case KnownColor.Desktop:
			case KnownColor.GrayText:
			case KnownColor.Highlight:
			case KnownColor.HighlightText:
			case KnownColor.HotTrack:
			case KnownColor.InactiveBorder:
			case KnownColor.InactiveCaption:
			case KnownColor.InactiveCaptionText:
			case KnownColor.Info:
			case KnownColor.InfoText:
			case KnownColor.Menu:
			case KnownColor.MenuText:
			case KnownColor.ScrollBar:
			case KnownColor.Window:
			case KnownColor.WindowFrame:
			case KnownColor.WindowText:
				newColor = null;
				return false;
			case KnownColor.Transparent:
				newColor = null;
				return allowTransparency;
			default:
				newColor = null;
				return true;
			}
		}

		internal static bool ValidateSizeString(string sizeString, out RVUnit sizeValue)
		{
			try
			{
				sizeValue = RVUnit.Parse(sizeString, CultureInfo.InvariantCulture);
				if (sizeValue.Type == RVUnitType.Pixel)
				{
					return false;
				}
				return true;
			}
			catch
			{
				sizeValue = RVUnit.Empty;
				return false;
			}
		}

		internal static bool ValidateSizeUnitType(RVUnit sizeValue)
		{
			switch (sizeValue.Type)
			{
			case RVUnitType.Cm:
			case RVUnitType.Inch:
			case RVUnitType.Mm:
			case RVUnitType.Pica:
			case RVUnitType.Point:
				return true;
			default:
				return false;
			}
		}

		internal static bool ValidateSizeIsPositive(RVUnit sizeValue)
		{
			if (sizeValue.Value >= 0.0)
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateSizeValue(double sizeInMM, double minValue, double maxValue)
		{
			if (sizeInMM >= minValue && sizeInMM <= maxValue)
			{
				return true;
			}
			return false;
		}

		internal static void ParseSize(string size, out double sizeInMM)
		{
			RVUnit unit = RVUnit.Parse(size, CultureInfo.InvariantCulture);
			sizeInMM = Converter.ConvertToMM(unit);
		}

		internal static bool ValidateEmbeddedImageName(string embeddedImageName, EmbeddedImageHashtable embeddedImages)
		{
			if (embeddedImageName == null)
			{
				return false;
			}
			if (embeddedImages == null)
			{
				return false;
			}
			return embeddedImages.ContainsKey(embeddedImageName);
		}

		internal static bool ValidateSpecificLanguage(string language, out CultureInfo culture)
		{
			try
			{
				culture = CultureInfo.CreateSpecificCulture(language);
				if (culture.IsNeutralCulture)
				{
					culture = null;
					return false;
				}
				culture = new CultureInfo(culture.Name, false);
				return true;
			}
			catch (ArgumentException)
			{
				culture = null;
				return false;
			}
		}

		internal static bool ValidateLanguage(string language, out CultureInfo culture)
		{
			try
			{
				culture = new CultureInfo(language, false);
				return true;
			}
			catch (ArgumentException)
			{
				culture = null;
				return false;
			}
		}

		internal static bool CreateCalendar(string calendarName, out Calendar calendar)
		{
			calendar = null;
			bool result = false;
			if (Validator.CompareWithInvariantCulture(calendarName, "Gregorian Arabic"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.Arabic);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Gregorian Middle East French"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Gregorian Transliterated English"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Gregorian Transliterated French"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Gregorian US English"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Hebrew"))
			{
				calendar = new HebrewCalendar();
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Hijri"))
			{
				calendar = new HijriCalendar();
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Japanese"))
			{
				calendar = new JapaneseCalendar();
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Korea"))
			{
				calendar = new KoreanCalendar();
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Taiwan"))
			{
				calendar = new TaiwanCalendar();
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Thai Buddhist"))
			{
				calendar = new ThaiBuddhistCalendar();
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Gregorian"))
			{
				calendar = new GregorianCalendar();
			}
			return result;
		}

		internal static bool CreateCalendar(Calendars calendarType, out Calendar calendar)
		{
			calendar = null;
			bool result = false;
			switch (calendarType)
			{
			case Calendars.Default:
			case Calendars.Gregorian:
				calendar = new GregorianCalendar();
				break;
			case Calendars.GregorianArabic:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.Arabic);
				break;
			case Calendars.GregorianMiddleEastFrench:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
				break;
			case Calendars.GregorianTransliteratedEnglish:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
				break;
			case Calendars.GregorianTransliteratedFrench:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
				break;
			case Calendars.GregorianUSEnglish:
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
				break;
			case Calendars.Hebrew:
				calendar = new HebrewCalendar();
				break;
			case Calendars.Hijri:
				calendar = new HijriCalendar();
				break;
			case Calendars.Japanese:
				calendar = new JapaneseCalendar();
				break;
			case Calendars.Julian:
				calendar = new JulianCalendar();
				break;
			case Calendars.Korean:
				calendar = new KoreanCalendar();
				break;
			case Calendars.Taiwan:
				calendar = new TaiwanCalendar();
				break;
			case Calendars.ThaiBuddhist:
				calendar = new ThaiBuddhistCalendar();
				break;
			}
			return result;
		}

		internal static bool ValidateCalendar(CultureInfo langauge, Calendars calendarType)
		{
			if (calendarType == Calendars.Gregorian)
			{
				return true;
			}
			Calendar calendar = default(Calendar);
			bool isGregorianSubType = Validator.CreateCalendar(calendarType, out calendar);
			return Validator.ValidateCalendar(langauge, isGregorianSubType, calendar);
		}

		internal static bool ValidateCalendar(CultureInfo langauge, string calendarName)
		{
			if (Validator.CompareWithInvariantCulture(calendarName, "Gregorian"))
			{
				return true;
			}
			Calendar calendar = default(Calendar);
			bool isGregorianSubType = Validator.CreateCalendar(calendarName, out calendar);
			return Validator.ValidateCalendar(langauge, isGregorianSubType, calendar);
		}

		private static bool ValidateCalendar(CultureInfo langauge, bool isGregorianSubType, Calendar calendar)
		{
			if (calendar == null)
			{
				return false;
			}
			Calendar[] optionalCalendars = langauge.OptionalCalendars;
			if (optionalCalendars != null)
			{
				for (int i = 0; i < optionalCalendars.Length; i++)
				{
					if (optionalCalendars[i].GetType() == calendar.GetType())
					{
						if (!isGregorianSubType)
						{
							return true;
						}
						GregorianCalendarTypes calendarType = ((GregorianCalendar)calendar).CalendarType;
						GregorianCalendarTypes calendarType2 = ((GregorianCalendar)optionalCalendars[i]).CalendarType;
						if (calendarType == calendarType2)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		internal static bool ValidateNumeralVariant(CultureInfo language, int numVariant)
		{
			if (numVariant >= 1 && numVariant <= 7)
			{
				if (numVariant < 3)
				{
					return true;
				}
				string text = language.TwoLetterISOLanguageName;
				if (text == null)
				{
					text = language.ThreeLetterISOLanguageName;
				}
				switch (numVariant)
				{
				case 3:
					if (!Validator.CompareWithInvariantCulture(text, "ar") && !Validator.CompareWithInvariantCulture(text, "ur") && !Validator.CompareWithInvariantCulture(text, "fa") && !Validator.CompareWithInvariantCulture(text, "hi") && !Validator.CompareWithInvariantCulture(text, "kok") && !Validator.CompareWithInvariantCulture(text, "mr") && !Validator.CompareWithInvariantCulture(text, "sa") && !Validator.CompareWithInvariantCulture(text, "bn") && !Validator.CompareWithInvariantCulture(text, "pa") && !Validator.CompareWithInvariantCulture(text, "gu") && !Validator.CompareWithInvariantCulture(text, "or") && !Validator.CompareWithInvariantCulture(text, "ta") && !Validator.CompareWithInvariantCulture(text, "te") && !Validator.CompareWithInvariantCulture(text, "kn") && !Validator.CompareWithInvariantCulture(text, "ms") && !Validator.CompareWithInvariantCulture(text, "th") && !Validator.CompareWithInvariantCulture(text, "lo") && !Validator.CompareWithInvariantCulture(text, "bo"))
					{
						break;
					}
					return true;
				case 7:
					if (!Validator.CompareWithInvariantCulture(text, "ko"))
					{
						break;
					}
					return true;
				default:
					if (!Validator.CompareWithInvariantCulture(text, "ko") && !Validator.CompareWithInvariantCulture(text, "ja"))
					{
						text = language.Name;
						if (!Validator.CompareWithInvariantCulture(text, "zh-CHT") && !Validator.CompareWithInvariantCulture(text, "zh-CHS"))
						{
							break;
						}
						return true;
					}
					return true;
				}
				return false;
			}
			return false;
		}

		internal static bool ValidateColumns(int columns)
		{
			if (columns >= 1 && columns <= 1000)
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateNumeralVariant(int numeralVariant)
		{
			if (numeralVariant >= 1 && numeralVariant <= 7)
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateBorderStyle(string borderStyle, out string borderStyleForLine)
		{
			if (!Validator.CompareWithInvariantCulture(borderStyle, "Dotted") && !Validator.CompareWithInvariantCulture(borderStyle, "Dashed"))
			{
				if (!Validator.CompareWithInvariantCulture(borderStyle, "None") && !Validator.CompareWithInvariantCulture(borderStyle, "Solid") && !Validator.CompareWithInvariantCulture(borderStyle, "Double") && !Validator.CompareWithInvariantCulture(borderStyle, "Groove") && !Validator.CompareWithInvariantCulture(borderStyle, "Ridge") && !Validator.CompareWithInvariantCulture(borderStyle, "Inset") && !Validator.CompareWithInvariantCulture(borderStyle, "WindowInset") && !Validator.CompareWithInvariantCulture(borderStyle, "Outset"))
				{
					borderStyleForLine = null;
					return false;
				}
				borderStyleForLine = "Solid";
				return true;
			}
			borderStyleForLine = borderStyle;
			return true;
		}

		internal static bool ValidateMimeType(string mimeType)
		{
			if (!Validator.CompareWithInvariantCulture(mimeType, "image/bmp") && !Validator.CompareWithInvariantCulture(mimeType, "image/jpeg") && !Validator.CompareWithInvariantCulture(mimeType, "image/gif") && !Validator.CompareWithInvariantCulture(mimeType, "image/png") && !Validator.CompareWithInvariantCulture(mimeType, "image/x-png"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateBackgroundGradientType(string gradientType)
		{
			if (!Validator.CompareWithInvariantCulture(gradientType, "None") && !Validator.CompareWithInvariantCulture(gradientType, "LeftRight") && !Validator.CompareWithInvariantCulture(gradientType, "TopBottom") && !Validator.CompareWithInvariantCulture(gradientType, "Center") && !Validator.CompareWithInvariantCulture(gradientType, "DiagonalLeft") && !Validator.CompareWithInvariantCulture(gradientType, "DiagonalRight") && !Validator.CompareWithInvariantCulture(gradientType, "HorizontalCenter") && !Validator.CompareWithInvariantCulture(gradientType, "VerticalCenter"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateBackgroundRepeat(string repeat)
		{
			if (!Validator.CompareWithInvariantCulture(repeat, "Repeat") && !Validator.CompareWithInvariantCulture(repeat, "NoRepeat") && !Validator.CompareWithInvariantCulture(repeat, "RepeatX") && !Validator.CompareWithInvariantCulture(repeat, "RepeatY"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateFontStyle(string fontStyle)
		{
			if (!Validator.CompareWithInvariantCulture(fontStyle, "Normal") && !Validator.CompareWithInvariantCulture(fontStyle, "Italic"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateFontWeight(string fontWeight)
		{
			if (!Validator.CompareWithInvariantCulture(fontWeight, "Lighter") && !Validator.CompareWithInvariantCulture(fontWeight, "Normal") && !Validator.CompareWithInvariantCulture(fontWeight, "Bold") && !Validator.CompareWithInvariantCulture(fontWeight, "Bolder") && !Validator.CompareWithInvariantCulture(fontWeight, "100") && !Validator.CompareWithInvariantCulture(fontWeight, "200") && !Validator.CompareWithInvariantCulture(fontWeight, "300") && !Validator.CompareWithInvariantCulture(fontWeight, "400") && !Validator.CompareWithInvariantCulture(fontWeight, "500") && !Validator.CompareWithInvariantCulture(fontWeight, "600") && !Validator.CompareWithInvariantCulture(fontWeight, "700") && !Validator.CompareWithInvariantCulture(fontWeight, "800") && !Validator.CompareWithInvariantCulture(fontWeight, "900"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateTextDecoration(string textDecoration)
		{
			if (!Validator.CompareWithInvariantCulture(textDecoration, "None") && !Validator.CompareWithInvariantCulture(textDecoration, "Underline") && !Validator.CompareWithInvariantCulture(textDecoration, "Overline") && !Validator.CompareWithInvariantCulture(textDecoration, "LineThrough"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateTextAlign(string textAlign)
		{
			if (!Validator.CompareWithInvariantCulture(textAlign, "General") && !Validator.CompareWithInvariantCulture(textAlign, "Left") && !Validator.CompareWithInvariantCulture(textAlign, "Center") && !Validator.CompareWithInvariantCulture(textAlign, "Right"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateVerticalAlign(string verticalAlign)
		{
			if (!Validator.CompareWithInvariantCulture(verticalAlign, "Top") && !Validator.CompareWithInvariantCulture(verticalAlign, "Middle") && !Validator.CompareWithInvariantCulture(verticalAlign, "Bottom"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateDirection(string direction)
		{
			if (!Validator.CompareWithInvariantCulture(direction, "LTR") && !Validator.CompareWithInvariantCulture(direction, "RTL"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateWritingMode(string writingMode)
		{
			if (!Validator.CompareWithInvariantCulture(writingMode, "lr-tb") && !Validator.CompareWithInvariantCulture(writingMode, "tb-rl"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateUnicodeBiDi(string unicodeBiDi)
		{
			if (!Validator.CompareWithInvariantCulture(unicodeBiDi, "Normal") && !Validator.CompareWithInvariantCulture(unicodeBiDi, "Embed") && !Validator.CompareWithInvariantCulture(unicodeBiDi, "BiDi-Override"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateCalendar(string calendar)
		{
			if (!Validator.CompareWithInvariantCulture(calendar, "Gregorian") && !Validator.CompareWithInvariantCulture(calendar, "Gregorian Arabic") && !Validator.CompareWithInvariantCulture(calendar, "Gregorian Middle East French") && !Validator.CompareWithInvariantCulture(calendar, "Gregorian Transliterated English") && !Validator.CompareWithInvariantCulture(calendar, "Gregorian Transliterated French") && !Validator.CompareWithInvariantCulture(calendar, "Gregorian US English") && !Validator.CompareWithInvariantCulture(calendar, "Hebrew") && !Validator.CompareWithInvariantCulture(calendar, "Hijri") && !Validator.CompareWithInvariantCulture(calendar, "Japanese") && !Validator.CompareWithInvariantCulture(calendar, "Korea") && !Validator.CompareWithInvariantCulture(calendar, "Taiwan") && !Validator.CompareWithInvariantCulture(calendar, "Thai Buddhist"))
			{
				return false;
			}
			return true;
		}

		internal static bool CompareWithInvariantCulture(string strOne, string strTwo)
		{
			if (ReportProcessing.CompareWithInvariantCulture(strOne, strTwo, false) == 0)
			{
				return true;
			}
			return false;
		}
	}
}
