using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class PublishingValidator
	{
		private static readonly string m_invalidCharacters = ";?:@&=+$,\\*<>|\"";

		private PublishingValidator()
		{
		}

		internal static bool ValidateColor(StyleInformation.StyleInformationAttribute colorAttribute, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (colorAttribute.ValueType == AspNetCore.ReportingServices.ReportIntermediateFormat.ValueType.ThemeReference)
			{
				string stringValue = colorAttribute.Value.StringValue;
				if (string.IsNullOrEmpty(stringValue))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Error, objectType, objectName, propertyName, stringValue);
					return false;
				}
				return true;
			}
			return PublishingValidator.ValidateColor(colorAttribute.Value, objectType, objectName, propertyName, errorContext);
		}

		internal static bool ValidateColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo color, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != color);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == color.Type)
			{
				string stringValue = default(string);
				if (!Validator.ValidateColor(color.StringValue, out stringValue, Validator.IsDynamicImageReportItem(objectType)))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Error, objectType, objectName, propertyName, color.StringValue);
					return false;
				}
				color.StringValue = stringValue;
			}
			return true;
		}

		internal static void ValidateBorderColorNotTransparent(ObjectType objectType, string objectName, AspNetCore.ReportingServices.ReportIntermediateFormat.Style styleClass, string styleName, ErrorContext errorContext)
		{
			ReportColor reportColor = default(ReportColor);
			AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo);
			if (styleClass.GetAttributeInfo(styleName, out attributeInfo) && !attributeInfo.IsExpression && ReportColor.TryParse(attributeInfo.Value, true, out reportColor) && reportColor.ToColor().A != 255)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Error, objectType, objectName, styleName, attributeInfo.Value);
			}
		}

		internal static bool ValidateSize(string size, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			double num = default(double);
			string text = default(string);
			return PublishingValidator.ValidateSize(size, false, Validator.NegativeMin, Validator.NormalMax, objectType, objectName, propertyName, errorContext, out num, out text);
		}

		internal static bool ValidateSize(string size, ObjectType objectType, string objectName, string propertyName, bool restrictMaxValue, ErrorContext errorContext, out double sizeInMM, out string roundSize)
		{
			bool allowNegative = ObjectType.Line == objectType;
			return PublishingValidator.ValidateSize(size, objectType, objectName, propertyName, restrictMaxValue, allowNegative, errorContext, out sizeInMM, out roundSize);
		}

		internal static bool ValidateSize(string size, ObjectType objectType, string objectName, string propertyName, bool restrictMaxValue, bool allowNegative, ErrorContext errorContext, out double sizeInMM, out string roundSize)
		{
			double minValue = allowNegative ? Validator.NegativeMin : Validator.NormalMin;
			double maxValue = restrictMaxValue ? Validator.NormalMax : 1.7976931348623157E+308;
			return PublishingValidator.ValidateSize(size, allowNegative, minValue, maxValue, objectType, objectName, propertyName, errorContext, out sizeInMM, out roundSize);
		}

		internal static bool ValidateSize(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo size, double minValue, double maxValue, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != size);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == size.Type)
			{
				bool allowNegative = false;
				double num = default(double);
				string text = default(string);
				return PublishingValidator.ValidateSize(size.StringValue, allowNegative, minValue, maxValue, objectType, objectName, propertyName, errorContext, out num, out text);
			}
			return true;
		}

		internal static bool ValidateSize(string size, bool allowNegative, double minValue, double maxValue, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, out double validSizeInMM, out string newSize)
		{
			validSizeInMM = minValue;
			newSize = minValue + "mm";
			RVUnit rVUnit = default(RVUnit);
			if (!Validator.ValidateSizeString(size, out rVUnit))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidSize, Severity.Error, objectType, objectName, propertyName, size);
				return false;
			}
			if (!Validator.ValidateSizeUnitType(rVUnit))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidMeasurementUnit, Severity.Error, objectType, objectName, propertyName, rVUnit.Type.ToString());
				return false;
			}
			if (!allowNegative && !Validator.ValidateSizeIsPositive(rVUnit))
			{
				errorContext.Register(ProcessingErrorCode.rsNegativeSize, Severity.Error, objectType, objectName, propertyName);
				return false;
			}
			double num = Converter.ConvertToMM(rVUnit);
			if (!Validator.ValidateSizeValue(num, minValue, maxValue))
			{
				errorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Error, objectType, objectName, propertyName, size, Converter.ConvertSizeFromMM(allowNegative ? minValue : Math.Max(0.0, minValue), rVUnit.Type), Converter.ConvertSizeFromMM(maxValue, rVUnit.Type));
				return false;
			}
			validSizeInMM = num;
			newSize = rVUnit.ToString(CultureInfo.InvariantCulture);
			return true;
		}

		internal static bool ValidateEmbeddedImageName(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo embeddedImageName, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != embeddedImageName);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == embeddedImageName.Type)
			{
				return PublishingValidator.ValidateEmbeddedImageName(embeddedImageName.StringValue, embeddedImages, objectType, objectName, propertyName, errorContext);
			}
			return true;
		}

		internal static bool ValidateEmbeddedImageName(AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo embeddedImageName, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != embeddedImageName);
			if (!embeddedImageName.IsExpression)
			{
				return PublishingValidator.ValidateEmbeddedImageName(embeddedImageName.Value, embeddedImages, objectType, objectName, propertyName, errorContext);
			}
			return true;
		}

		private static bool ValidateEmbeddedImageName(string embeddedImageName, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
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

		internal static bool ValidateLanguage(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo language, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != language);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == language.Type)
			{
				CultureInfo cultureInfo = null;
				if (!Validator.ValidateLanguage(language.StringValue, out cultureInfo))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, objectType, objectName, propertyName, language.StringValue);
					return false;
				}
			}
			return true;
		}

		internal static bool ValidateSpecificLanguage(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo language, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, out CultureInfo culture)
		{
			culture = null;
			Global.Tracer.Assert(null != language);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == language.Type && !Validator.ValidateSpecificLanguage(language.StringValue, out culture))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidLanguage, Severity.Error, objectType, objectName, propertyName, language.StringValue);
				return false;
			}
			return true;
		}

		internal static bool ValidateColumns(int columns, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext, int sectionNumber)
		{
			if (!Validator.ValidateColumns(columns))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidColumnsInReportSection, Severity.Error, objectType, objectName, propertyName, sectionNumber.ToString(CultureInfo.InvariantCulture));
				return false;
			}
			return true;
		}

		private static bool ValidateNumeralVariant(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo numeralVariant, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != numeralVariant);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == numeralVariant.Type && !Validator.ValidateNumeralVariant(numeralVariant.IntValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidNumeralVariant, Severity.Error, objectType, objectName, propertyName, numeralVariant.IntValue.ToString(CultureInfo.InvariantCulture));
				return false;
			}
			return true;
		}

		internal static bool ValidateMimeType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo mimeType, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (mimeType == null)
			{
				errorContext.Register(ProcessingErrorCode.rsMissingMIMEType, Severity.Error, objectType, objectName, propertyName);
				return false;
			}
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == mimeType.Type)
			{
				return PublishingValidator.ValidateMimeType(mimeType.StringValue, objectType, objectName, propertyName, errorContext);
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

		private static bool ValidateTextEffect(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo textEffect, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != textEffect);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == textEffect.Type && !Validator.ValidateTextEffect(textEffect.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidTextEffect, Severity.Error, objectType, objectName, propertyName, textEffect.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateBackgroundHatchType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundHatchType, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != backgroundHatchType);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == backgroundHatchType.Type && !Validator.ValidateBackgroundHatchType(backgroundHatchType.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundHatchType, Severity.Error, objectType, objectName, propertyName, backgroundHatchType.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidatePosition(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo position, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != position);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == position.Type && !Validator.ValidatePosition(position.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundImagePosition, Severity.Error, objectType, objectName, propertyName, position.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateBorderStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo borderStyle, ObjectType objectType, string objectName, bool isDynamicElementSubElement, string propertyName, bool isDefaultBorder, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != borderStyle);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == borderStyle.Type)
			{
				string stringValue = default(string);
				if (!Validator.ValidateBorderStyle(borderStyle.StringValue, isDefaultBorder, objectType, isDynamicElementSubElement, out stringValue))
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidBorderStyle, Severity.Error, objectType, objectName, propertyName, borderStyle.StringValue);
					return false;
				}
				borderStyle.StringValue = stringValue;
			}
			return true;
		}

		private static bool ValidateBackgroundGradientType(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo repeat, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != repeat);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == repeat.Type && !Validator.ValidateBackgroundGradientType(repeat.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundGradientType, Severity.Error, objectType, objectName, propertyName, repeat.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateBackgroundRepeat(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo repeat, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != repeat);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == repeat.Type && !Validator.ValidateBackgroundRepeat(repeat.StringValue, objectType))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidBackgroundRepeat, Severity.Error, objectType, objectName, propertyName, repeat.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateTransparency(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo transparency, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != transparency);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == transparency.Type)
			{
				double floatValue = transparency.FloatValue;
				if (!(floatValue < 0.0) && !(floatValue > 100.0))
				{
					goto IL_006d;
				}
				errorContext.Register(ProcessingErrorCode.rsOutOfRangeSize, Severity.Error, objectType, objectName, propertyName, transparency.OriginalText, "0", "100");
				return false;
			}
			goto IL_006d;
			IL_006d:
			return true;
		}

		private static bool ValidateFontStyle(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo fontStyle, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != fontStyle);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == fontStyle.Type && !Validator.ValidateFontStyle(fontStyle.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidFontStyle, Severity.Error, objectType, objectName, propertyName, fontStyle.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateFontWeight(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo fontWeight, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != fontWeight);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == fontWeight.Type && !Validator.ValidateFontWeight(fontWeight.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidFontWeight, Severity.Error, objectType, objectName, propertyName, fontWeight.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateTextDecoration(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo textDecoration, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != textDecoration);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == textDecoration.Type && !Validator.ValidateTextDecoration(textDecoration.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidTextDecoration, Severity.Error, objectType, objectName, propertyName, textDecoration.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateTextAlign(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo textAlign, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != textAlign);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == textAlign.Type && !Validator.ValidateTextAlign(textAlign.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidTextAlign, Severity.Error, objectType, objectName, propertyName, textAlign.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateVerticalAlign(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo verticalAlign, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != verticalAlign);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == verticalAlign.Type && !Validator.ValidateVerticalAlign(verticalAlign.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidVerticalAlign, Severity.Error, objectType, objectName, propertyName, verticalAlign.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateDirection(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo direction, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != direction);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == direction.Type && !Validator.ValidateDirection(direction.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidDirection, Severity.Error, objectType, objectName, propertyName, direction.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateWritingMode(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo writingMode, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != writingMode);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == writingMode.Type && !Validator.ValidateWritingMode(writingMode.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidWritingMode, Severity.Error, objectType, objectName, propertyName, writingMode.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateUnicodeBiDi(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo unicodeBiDi, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != unicodeBiDi);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == unicodeBiDi.Type && !Validator.ValidateUnicodeBiDi(unicodeBiDi.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidUnicodeBiDi, Severity.Error, objectType, objectName, propertyName, unicodeBiDi.StringValue);
				return false;
			}
			return true;
		}

		private static bool ValidateCalendar(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo calendar, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != calendar);
			if (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == calendar.Type && !Validator.ValidateCalendar(calendar.StringValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCalendar, Severity.Error, objectType, objectName, propertyName, calendar.StringValue);
				return false;
			}
			return true;
		}

		private static void ValidateBackgroundImage(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageSource, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageValue, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageMIMEType, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundEmbeddingMode, AspNetCore.ReportingServices.ReportIntermediateFormat.Style style, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			if (backgroundImageSource != null)
			{
				bool flag = true;
				Global.Tracer.Assert(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == backgroundImageSource.Type);
				AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType intValue = (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType)backgroundImageSource.IntValue;
				Global.Tracer.Assert(null != backgroundImageValue);
				if (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Database == intValue && AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Constant == backgroundImageValue.Type)
				{
					errorContext.Register(ProcessingErrorCode.rsBinaryConstant, Severity.Error, objectType, objectName, "BackgroundImageValue");
					flag = false;
				}
				if (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Database == intValue && !PublishingValidator.ValidateMimeType(backgroundImageMIMEType, objectType, objectName, "BackgroundImageMIMEType", errorContext))
				{
					flag = false;
				}
				if (flag)
				{
					style.AddAttribute("BackgroundImageSource", backgroundImageSource);
					style.AddAttribute("BackgroundImageValue", backgroundImageValue);
					if (backgroundEmbeddingMode != null)
					{
						style.AddAttribute("EmbeddingMode", backgroundEmbeddingMode);
					}
					if (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Database == intValue)
					{
						style.AddAttribute("BackgroundImageMIMEType", backgroundImageMIMEType);
					}
				}
			}
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.Style ValidateAndCreateStyle(List<StyleInformation.StyleInformationAttribute> attributes, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool flag = default(bool);
			return PublishingValidator.ValidateAndCreateStyle(attributes, objectType, objectName, true, errorContext, false, out flag);
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.Style ValidateAndCreateStyle(List<StyleInformation.StyleInformationAttribute> attributes, ObjectType objectType, string objectName, bool isDynamicImageSubElement, ErrorContext errorContext)
		{
			bool flag = default(bool);
			return PublishingValidator.ValidateAndCreateStyle(attributes, objectType, objectName, isDynamicImageSubElement, errorContext, false, out flag);
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.Style ValidateAndCreateStyle(List<StyleInformation.StyleInformationAttribute> attributes, ObjectType objectType, string objectName, ErrorContext errorContext, bool checkForMeDotValue, out bool meDotValueReferenced)
		{
			return PublishingValidator.ValidateAndCreateStyle(attributes, objectType, objectName, false, errorContext, checkForMeDotValue, out meDotValueReferenced);
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.Style ValidateAndCreateStyle(List<StyleInformation.StyleInformationAttribute> attributes, ObjectType objectType, string objectName, bool isDynamicImageSubElement, ErrorContext errorContext, bool checkForMeDotValue, out bool meDotValueReferenced)
		{
			meDotValueReferenced = false;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Style style = new AspNetCore.ReportingServices.ReportIntermediateFormat.Style(AspNetCore.ReportingServices.ReportIntermediateFormat.ConstructionPhase.Publishing);
			Global.Tracer.Assert(null != attributes);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageSource = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageValue = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundImageMIMEType = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo backgroundEmbeddingMode = null;
			for (int i = 0; i < attributes.Count; i++)
			{
				StyleInformation.StyleInformationAttribute styleInformationAttribute = attributes[i];
				if (checkForMeDotValue && styleInformationAttribute.ValueType == AspNetCore.ReportingServices.ReportIntermediateFormat.ValueType.Constant && styleInformationAttribute.Value.MeDotValueDetected)
				{
					meDotValueReferenced = true;
				}
				switch (attributes[i].Name)
				{
				case "BackgroundImageSource":
					backgroundImageSource = styleInformationAttribute.Value;
					break;
				case "BackgroundImageValue":
					backgroundImageValue = styleInformationAttribute.Value;
					break;
				case "BackgroundImageMIMEType":
					backgroundImageMIMEType = styleInformationAttribute.Value;
					break;
				case "BackgroundRepeat":
					if (PublishingValidator.ValidateBackgroundRepeat(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "EmbeddingMode":
					backgroundEmbeddingMode = styleInformationAttribute.Value;
					break;
				case "Transparency":
					if (PublishingValidator.ValidateTransparency(styleInformationAttribute.Value, objectType, objectName, "Transparency", errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "TransparentColor":
					if (PublishingValidator.ValidateColor(styleInformationAttribute.Value, objectType, objectName, "TransparentColor", errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BorderColor":
				case "BorderColorLeft":
				case "BorderColorRight":
				case "BorderColorTop":
				case "BorderColorBottom":
					if (PublishingValidator.ValidateColor(styleInformationAttribute.Value, objectType, objectName, "BorderColor", errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BorderStyle":
					if (PublishingValidator.ValidateBorderStyle(styleInformationAttribute.Value, objectType, objectName, isDynamicImageSubElement, "BorderStyle", true, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BorderStyleLeft":
				case "BorderStyleRight":
				case "BorderStyleTop":
				case "BorderStyleBottom":
					if (PublishingValidator.ValidateBorderStyle(styleInformationAttribute.Value, objectType, objectName, isDynamicImageSubElement, "BorderStyle", false, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BorderWidth":
				case "BorderWidthLeft":
				case "BorderWidthRight":
				case "BorderWidthTop":
				case "BorderWidthBottom":
					if (PublishingValidator.ValidateSize(styleInformationAttribute.Value, Validator.BorderWidthMin, Validator.BorderWidthMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BackgroundGradientEndColor":
					if (PublishingValidator.ValidateColor(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BackgroundGradientType":
					if (PublishingValidator.ValidateBackgroundGradientType(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "FontStyle":
					if (PublishingValidator.ValidateFontStyle(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "FontFamily":
					style.AddAttribute(styleInformationAttribute);
					break;
				case "FontSize":
					if (PublishingValidator.ValidateSize(styleInformationAttribute.Value, Validator.FontSizeMin, Validator.FontSizeMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "FontWeight":
					if (PublishingValidator.ValidateFontWeight(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Format":
					style.AddAttribute(styleInformationAttribute);
					break;
				case "TextDecoration":
					if (PublishingValidator.ValidateTextDecoration(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "TextAlign":
					if (PublishingValidator.ValidateTextAlign(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "VerticalAlign":
					if (PublishingValidator.ValidateVerticalAlign(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Color":
				case "BackgroundColor":
					if (PublishingValidator.ValidateColor(styleInformationAttribute, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "PaddingLeft":
				case "PaddingRight":
				case "PaddingTop":
				case "PaddingBottom":
					if (PublishingValidator.ValidateSize(styleInformationAttribute.Value, Validator.PaddingMin, Validator.PaddingMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "LineHeight":
					if (PublishingValidator.ValidateSize(styleInformationAttribute.Value, Validator.LineHeightMin, Validator.LineHeightMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Direction":
					if (PublishingValidator.ValidateDirection(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "WritingMode":
					if (PublishingValidator.ValidateWritingMode(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Language":
				{
					CultureInfo cultureInfo = default(CultureInfo);
					if (PublishingValidator.ValidateSpecificLanguage(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext, out cultureInfo))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				}
				case "UnicodeBiDi":
					if (PublishingValidator.ValidateUnicodeBiDi(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Calendar":
					if (PublishingValidator.ValidateCalendar(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "CurrencyLanguage":
					if (PublishingValidator.ValidateLanguage(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "NumeralLanguage":
					if (PublishingValidator.ValidateLanguage(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "NumeralVariant":
					if (PublishingValidator.ValidateNumeralVariant(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "ShadowColor":
					if (PublishingValidator.ValidateColor(styleInformationAttribute.Value, objectType, objectName, "ShadowColor", errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "ShadowOffset":
					if (PublishingValidator.ValidateSize(styleInformationAttribute.Value, Validator.NormalMin, Validator.NormalMax, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "BackgroundHatchType":
					if (PublishingValidator.ValidateBackgroundHatchType(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "TextEffect":
					if (PublishingValidator.ValidateTextEffect(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				case "Position":
					if (PublishingValidator.ValidatePosition(styleInformationAttribute.Value, objectType, objectName, styleInformationAttribute.Name, errorContext))
					{
						style.AddAttribute(styleInformationAttribute);
					}
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			PublishingValidator.ValidateBackgroundImage(backgroundImageSource, backgroundImageValue, backgroundImageMIMEType, backgroundEmbeddingMode, style, objectType, objectName, errorContext);
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

		internal static void ValidateTextRunMarkupType(string value, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateTextRunMarkupType(value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidMarkupType, Severity.Error, objectType, objectName, propertyName, value);
			}
		}

		internal static void ValidateParagraphListStyle(string value, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			if (!Validator.ValidateParagraphListStyle(value))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidListStyle, Severity.Error, objectType, objectName, propertyName, value);
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
