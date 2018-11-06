using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class PublishingValidator
	{
		private static readonly string m_invalidCharacters = ";?:@&=+$,\\*<>|\"";

		private PublishingValidator()
		{
		}

		private static bool ValidateColor(ExpressionInfo color, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != color);
			if (ExpressionInfo.Types.Constant == color.Type)
			{
				string value = default(string);
				if (!Validator.ValidateColor(color.Value, out value, objectType == ObjectType.Chart))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Error, objectType, objectName, propertyName, color.Value);
					return false;
				}
				color.Value = value;
			}
			return true;
		}

		internal static bool ValidateSize(string size, ObjectType objectType, string objectName, string propertyName, bool restrictMaxValue, ErrorContext errorContext, out double sizeInMM, out string roundSize)
		{
			bool flag = ObjectType.Line == objectType;
			double minValue = flag ? Validator.NegativeMin : Validator.NormalMin;
			double maxValue = restrictMaxValue ? Validator.NormalMax : 1.7976931348623157E+308;
			return PublishingValidator.ValidateSize(size, flag, minValue, maxValue, objectType, objectName, propertyName, errorContext, out sizeInMM, out roundSize);
		}

		private static bool ValidateSize(ExpressionInfo size, double minValue, double maxValue, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != size);
			if (ExpressionInfo.Types.Constant == size.Type)
			{
				bool allowNegative = false;
				double num = default(double);
				string text = default(string);
				return PublishingValidator.ValidateSize(size.Value, allowNegative, minValue, maxValue, objectType, objectName, propertyName, errorContext, out num, out text);
			}
			return true;
		}

		private static bool ValidateSize(string size, bool allowNegative, double minValue, double maxValue, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, out double validSizeInMM, out string newSize)
		{
			validSizeInMM = minValue;
			newSize = minValue + "mm";
			RVUnit sizeValue = default(RVUnit);
			if (!Validator.ValidateSizeString(size, out sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, objectType, objectName, propertyName, size);
				return false;
			}
			if (!Validator.ValidateSizeUnitType(sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidMeasurementUnit, Severity.Error, objectType, objectName, propertyName, sizeValue.Type.ToString());
				return false;
			}
			if (!allowNegative && !Validator.ValidateSizeIsPositive(sizeValue))
			{
				errorContext.Register(ProcessingErrorCode.rsNegativeSize, Severity.Error, objectType, objectName, propertyName);
				return false;
			}
			RVUnit unit = new RVUnit(Math.Round(sizeValue.Value, Validator.DecimalPrecision), sizeValue.Type);
			double num = Converter.ConvertToMM(unit);
			if (!Validator.ValidateSizeValue(num, minValue, maxValue))
			{
				errorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Error, objectType, objectName, propertyName, size, Converter.ConvertSize(minValue), Converter.ConvertSize(maxValue));
				return false;
			}
			validSizeInMM = Math.Round(num, Validator.DecimalPrecision);
			newSize = unit.ToString(CultureInfo.InvariantCulture);
			return true;
		}

		internal static bool ValidateEmbeddedImageName(ExpressionInfo embeddedImageName, EmbeddedImageHashtable embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != embeddedImageName);
			if (ExpressionInfo.Types.Constant == embeddedImageName.Type)
			{
				return PublishingValidator.ValidateEmbeddedImageName(embeddedImageName.Value, embeddedImages, objectType, objectName, propertyName, errorContext);
			}
			return true;
		}

		internal static bool ValidateEmbeddedImageName(AttributeInfo embeddedImageName, EmbeddedImageHashtable embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != embeddedImageName);
			if (!embeddedImageName.IsExpression)
			{
				return PublishingValidator.ValidateEmbeddedImageName(embeddedImageName.Value, embeddedImages, objectType, objectName, propertyName, errorContext);
			}
			return true;
		}

		private static bool ValidateEmbeddedImageName(string embeddedImageName, EmbeddedImageHashtable embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateEmbeddedImageName(embeddedImageName, embeddedImages))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEmbeddedImageProperty, Severity.Error, objectType, objectName, propertyName, embeddedImageName);
				return false;
			}
			return true;
		}

		internal static bool ValidateLanguage(string languageValue, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, out CultureInfo culture)
		{
			culture = null;
			Global.Tracer.Assert(null != languageValue);
			if (!Validator.ValidateLanguage(languageValue, out culture))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, objectType, objectName, propertyName, languageValue);
				return false;
			}
			return true;
		}

		internal static bool ValidateLanguage(ExpressionInfo language, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != language);
			if (ExpressionInfo.Types.Constant == language.Type)
			{
				CultureInfo cultureInfo = null;
				if (!Validator.ValidateLanguage(language.Value, out cultureInfo))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, objectType, objectName, propertyName, language.Value);
					return false;
				}
			}
			return true;
		}

		internal static bool ValidateSpecificLanguage(ExpressionInfo language, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, out CultureInfo culture)
		{
			culture = null;
			Global.Tracer.Assert(null != language);
			if (ExpressionInfo.Types.Constant == language.Type && !Validator.ValidateSpecificLanguage(language.Value, out culture))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, objectType, objectName, propertyName, language.Value);
				return false;
			}
			return true;
		}

		internal static bool ValidateColumns(int columns, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateColumns(columns))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidColumnsInBody, Severity.Error, objectType, objectName, propertyName);
				return false;
			}
			return true;
		}

		private static bool ValidateNumeralVariant(ExpressionInfo numeralVariant, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != numeralVariant);
			if (ExpressionInfo.Types.Constant == numeralVariant.Type && !Validator.ValidateNumeralVariant(numeralVariant.IntValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariant, Severity.Error, objectType, objectName, propertyName, numeralVariant.IntValue.ToString(CultureInfo.InvariantCulture));
				return false;
			}
			return true;
		}

		internal static bool ValidateMimeType(ExpressionInfo mimeType, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (mimeType == null)
			{
				errorContext.Register(ProcessingErrorCode.rsMissingMIMEType, Severity.Error, objectType, objectName, propertyName);
				return false;
			}
			if (ExpressionInfo.Types.Constant == mimeType.Type)
			{
				return PublishingValidator.ValidateMimeType(mimeType.Value, objectType, objectName, propertyName, errorContext);
			}
			return true;
		}

		internal static bool ValidateMimeType(string mimeType, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateMimeType(mimeType))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidMIMEType, Severity.Error, objectType, objectName, propertyName, mimeType);
				return false;
			}
			return true;
		}

		private static bool ValidateBorderStyle(ExpressionInfo borderStyle, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != borderStyle);
			if (ExpressionInfo.Types.Constant == borderStyle.Type)
			{
				string value = default(string);
				if (!Validator.ValidateBorderStyle(borderStyle.Value, out value))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidBorderStyle, Severity.Error, objectType, objectName, propertyName, borderStyle.Value);
					return false;
				}
				if (ObjectType.Line == objectType)
				{
					borderStyle.Value = value;
				}
			}
			return true;
		}

		private static bool ValidateBackgroundGradientType(ExpressionInfo repeat, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != repeat);
			if (ExpressionInfo.Types.Constant == repeat.Type && !Validator.ValidateBackgroundGradientType(repeat.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundGradientType, Severity.Error, objectType, objectName, propertyName, repeat.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateBackgroundRepeat(ExpressionInfo repeat, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != repeat);
			if (ExpressionInfo.Types.Constant == repeat.Type && !Validator.ValidateBackgroundRepeat(repeat.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundRepeat, Severity.Error, objectType, objectName, propertyName, repeat.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateFontStyle(ExpressionInfo fontStyle, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != fontStyle);
			if (ExpressionInfo.Types.Constant == fontStyle.Type && !Validator.ValidateFontStyle(fontStyle.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidFontStyle, Severity.Error, objectType, objectName, propertyName, fontStyle.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateFontWeight(ExpressionInfo fontWeight, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != fontWeight);
			if (ExpressionInfo.Types.Constant == fontWeight.Type && !Validator.ValidateFontWeight(fontWeight.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidFontWeight, Severity.Error, objectType, objectName, propertyName, fontWeight.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateTextDecoration(ExpressionInfo textDecoration, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != textDecoration);
			if (ExpressionInfo.Types.Constant == textDecoration.Type && !Validator.ValidateTextDecoration(textDecoration.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidTextDecoration, Severity.Error, objectType, objectName, propertyName, textDecoration.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateTextAlign(ExpressionInfo textAlign, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != textAlign);
			if (ExpressionInfo.Types.Constant == textAlign.Type && !Validator.ValidateTextAlign(textAlign.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidTextAlign, Severity.Error, objectType, objectName, propertyName, textAlign.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateVerticalAlign(ExpressionInfo verticalAlign, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != verticalAlign);
			if (ExpressionInfo.Types.Constant == verticalAlign.Type && !Validator.ValidateVerticalAlign(verticalAlign.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidVerticalAlign, Severity.Error, objectType, objectName, propertyName, verticalAlign.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateDirection(ExpressionInfo direction, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != direction);
			if (ExpressionInfo.Types.Constant == direction.Type && !Validator.ValidateDirection(direction.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidDirection, Severity.Error, objectType, objectName, propertyName, direction.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateWritingMode(ExpressionInfo writingMode, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != writingMode);
			if (ExpressionInfo.Types.Constant == writingMode.Type && !Validator.ValidateWritingMode(writingMode.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidWritingMode, Severity.Error, objectType, objectName, propertyName, writingMode.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateUnicodeBiDi(ExpressionInfo unicodeBiDi, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != unicodeBiDi);
			if (ExpressionInfo.Types.Constant == unicodeBiDi.Type && !Validator.ValidateUnicodeBiDi(unicodeBiDi.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidUnicodeBiDi, Severity.Error, objectType, objectName, propertyName, unicodeBiDi.Value);
				return false;
			}
			return true;
		}

		private static bool ValidateCalendar(ExpressionInfo calendar, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != calendar);
			if (ExpressionInfo.Types.Constant == calendar.Type && !Validator.ValidateCalendar(calendar.Value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCalendar, Severity.Error, objectType, objectName, propertyName, calendar.Value);
				return false;
			}
			return true;
		}

		private static void ValidateBackgroundImage(ExpressionInfo backgroundImageSource, ExpressionInfo backgroundImageValue, ExpressionInfo backgroundImageMIMEType, Style style, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			if (backgroundImageSource != null)
			{
				bool flag = true;
				Global.Tracer.Assert(ExpressionInfo.Types.Constant == backgroundImageSource.Type);
				Image.SourceType intValue = (Image.SourceType)backgroundImageSource.IntValue;
				Global.Tracer.Assert(null != backgroundImageValue);
				if (Image.SourceType.Database == intValue && ExpressionInfo.Types.Constant == backgroundImageValue.Type)
				{
					errorContext.Register(ProcessingErrorCode.rsBinaryConstant, Severity.Error, objectType, objectName, "BackgroundImageValue");
					flag = false;
				}
				if (Image.SourceType.Database == intValue && !PublishingValidator.ValidateMimeType(backgroundImageMIMEType, objectType, objectName, "BackgroundImageMIMEType", errorContext))
				{
					flag = false;
				}
				if (flag)
				{
					style.AddAttribute("BackgroundImageSource", backgroundImageSource);
					style.AddAttribute("BackgroundImageValue", backgroundImageValue);
					if (Image.SourceType.Database == intValue)
					{
						style.AddAttribute("BackgroundImageMIMEType", backgroundImageMIMEType);
					}
				}
			}
		}

		internal static Style ValidateAndCreateStyle(StringList names, ExpressionInfoList values, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			Style style = new Style(ConstructionPhase.Publishing);
			Global.Tracer.Assert(null != names);
			Global.Tracer.Assert(null != values);
			Global.Tracer.Assert(names.Count == values.Count);
			ExpressionInfo backgroundImageSource = null;
			ExpressionInfo backgroundImageValue = null;
			ExpressionInfo backgroundImageMIMEType = null;
			for (int i = 0; i < names.Count; i++)
			{
				switch (names[i])
				{
				case "BackgroundImageSource":
					backgroundImageSource = values[i];
					break;
				case "BackgroundImageValue":
					backgroundImageValue = values[i];
					break;
				case "BackgroundImageMIMEType":
					backgroundImageMIMEType = values[i];
					break;
				case "BackgroundRepeat":
					if (PublishingValidator.ValidateBackgroundRepeat(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "BorderColor":
				case "BorderColorLeft":
				case "BorderColorRight":
				case "BorderColorTop":
				case "BorderColorBottom":
					if (PublishingValidator.ValidateColor(values[i], objectType, objectName, "BorderColor", errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "BorderStyle":
				case "BorderStyleLeft":
				case "BorderStyleRight":
				case "BorderStyleTop":
				case "BorderStyleBottom":
					if (PublishingValidator.ValidateBorderStyle(values[i], objectType, objectName, "BorderStyle", errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "BorderWidth":
				case "BorderWidthLeft":
				case "BorderWidthRight":
				case "BorderWidthTop":
				case "BorderWidthBottom":
					if (PublishingValidator.ValidateSize(values[i], Validator.BorderWidthMin, Validator.BorderWidthMax, objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "BackgroundColor":
				case "BackgroundGradientEndColor":
					if (PublishingValidator.ValidateColor(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "BackgroundGradientType":
					if (PublishingValidator.ValidateBackgroundGradientType(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "FontStyle":
					if (PublishingValidator.ValidateFontStyle(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "FontFamily":
					style.AddAttribute(names[i], values[i]);
					break;
				case "FontSize":
					if (PublishingValidator.ValidateSize(values[i], Validator.FontSizeMin, Validator.FontSizeMax, objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "FontWeight":
					if (PublishingValidator.ValidateFontWeight(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "Format":
					style.AddAttribute(names[i], values[i]);
					break;
				case "TextDecoration":
					if (PublishingValidator.ValidateTextDecoration(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "TextAlign":
					if (PublishingValidator.ValidateTextAlign(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "VerticalAlign":
					if (PublishingValidator.ValidateVerticalAlign(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "Color":
					if (PublishingValidator.ValidateColor(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "PaddingLeft":
				case "PaddingRight":
				case "PaddingTop":
				case "PaddingBottom":
					if (PublishingValidator.ValidateSize(values[i], Validator.PaddingMin, Validator.PaddingMax, objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "LineHeight":
					if (PublishingValidator.ValidateSize(values[i], Validator.LineHeightMin, Validator.LineHeightMax, objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "Direction":
					if (PublishingValidator.ValidateDirection(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "WritingMode":
					if (PublishingValidator.ValidateWritingMode(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "Language":
				{
					CultureInfo cultureInfo = default(CultureInfo);
					if (PublishingValidator.ValidateSpecificLanguage(values[i], objectType, objectName, names[i], errorContext, out cultureInfo))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				}
				case "UnicodeBiDi":
					if (PublishingValidator.ValidateUnicodeBiDi(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "Calendar":
					if (PublishingValidator.ValidateCalendar(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "NumeralLanguage":
					if (PublishingValidator.ValidateLanguage(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				case "NumeralVariant":
					if (PublishingValidator.ValidateNumeralVariant(values[i], objectType, objectName, names[i], errorContext))
					{
						style.AddAttribute(names[i], values[i]);
					}
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			PublishingValidator.ValidateBackgroundImage(backgroundImageSource, backgroundImageValue, backgroundImageMIMEType, style, objectType, objectName, errorContext);
			if (0 < style.StyleAttributes.Count)
			{
				return style;
			}
			return null;
		}

		internal static void ValidateCalendar(CultureInfo language, string calendar, ObjectType objectType, string ObjectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateCalendar(language, calendar))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCalendarForLanguage, Severity.Error, objectType, ObjectName, propertyName, calendar, language.Name);
			}
		}

		internal static void ValidateNumeralVariant(CultureInfo language, int numVariant, ObjectType objectType, string ObjectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateNumeralVariant(language, numVariant))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariantForLanguage, Severity.Error, objectType, ObjectName, propertyName, numVariant.ToString(CultureInfo.InvariantCulture), language.Name);
			}
		}

		internal static string ValidateReportName(ICatalogItemContext reportContext, string reportName, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != reportName);
			if (reportName.StartsWith(Uri.UriSchemeHttp + Uri.SchemeDelimiter, StringComparison.OrdinalIgnoreCase) || reportName.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter, StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					new Uri(reportName);
				}
				catch (UriFormatException)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidReportUri, Severity.Error, objectType, objectName, propertyName);
					return reportName;
				}
			}
			else if (reportName.Length > 0 && -1 != reportName.IndexOfAny(PublishingValidator.m_invalidCharacters.ToCharArray()))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidReportNameCharacters, Severity.Error, objectType, objectName, propertyName, PublishingValidator.m_invalidCharacters);
				return reportName;
			}
			string text;
			try
			{
				text = reportContext.AdjustSubreportOrDrillthroughReportPath(reportName.Trim());
			}
			catch (RSException)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidReportUri, Severity.Error, objectType, objectName, propertyName);
				return reportName;
			}
			if (text != null && reportName.Length != 0)
			{
				return text;
			}
			errorContext.Register((ProcessingErrorCode)((reportName.Length == 0) ? 151 : 153), Severity.Error, objectType, objectName, propertyName);
			return reportName;
		}
	}
}
