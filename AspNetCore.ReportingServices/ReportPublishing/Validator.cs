using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class Validator
	{
		internal class DoubleComparer : IComparer<double>
		{
			private static DoubleComparer m_instance;

			public static DoubleComparer Instance
			{
				get
				{
					if (DoubleComparer.m_instance == null)
					{
						DoubleComparer.m_instance = new DoubleComparer();
					}
					return DoubleComparer.m_instance;
				}
			}

			private DoubleComparer()
			{
			}

			public int Compare(double x, double y)
			{
				return Validator.CompareDoubles(x, y);
			}
		}

		internal const int DecimalPrecision = 10;

		internal const int RoundingPrecision = 4;

		internal const int TruncatePrecision = 3;

		internal const int ParagraphListLevelMin = 0;

		internal const int ParagraphListLevelMax = 9;

		internal static double NormalMin = 0.0;

		internal static double NegativeMin = -11557.0;

		internal static double NormalMax = 11557.0;

		internal static double BorderWidthMin = 0.08814;

		internal static double BorderWidthMax = 7.0555555555555554;

		internal static double FontSizeMin = 0.35277777777777775;

		internal static double FontSizeMax = 70.555555555555543;

		internal static double PaddingMin = 0.0;

		internal static double PaddingMax = 352.77777777777777;

		internal static double LineHeightMin = 0.35277777777777775;

		internal static double LineHeightMax = 352.77777777777777;

		private static Regex m_colorRegex = new Regex("^#(\\d|a|b|c|d|e|f){6}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		private static Regex m_colorRegexTransparency = new Regex("^#(\\d|a|b|c|d|e|f){8}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		internal static bool ValidateGaugeAntiAliasings(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "All") && !Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Text") && !Validator.CompareWithInvariantCulture(val, "Graphics"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeBarStarts(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "ScaleStart") && !Validator.CompareWithInvariantCulture(val, "Zero"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeCapStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "RoundedDark") && !Validator.CompareWithInvariantCulture(val, "Rounded") && !Validator.CompareWithInvariantCulture(val, "RoundedLight") && !Validator.CompareWithInvariantCulture(val, "RoundedWithAdditionalTop") && !Validator.CompareWithInvariantCulture(val, "RoundedWithWideIndentation") && !Validator.CompareWithInvariantCulture(val, "FlattenedWithIndentation") && !Validator.CompareWithInvariantCulture(val, "FlattenedWithWideIndentation") && !Validator.CompareWithInvariantCulture(val, "RoundedGlossyWithIndentation") && !Validator.CompareWithInvariantCulture(val, "RoundedWithIndentation"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeFrameShapes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Default") && !Validator.CompareWithInvariantCulture(val, "Circular") && !Validator.CompareWithInvariantCulture(val, "Rectangular") && !Validator.CompareWithInvariantCulture(val, "RoundedRectangular") && !Validator.CompareWithInvariantCulture(val, "AutoShape") && !Validator.CompareWithInvariantCulture(val, "CustomCircular1") && !Validator.CompareWithInvariantCulture(val, "CustomCircular2") && !Validator.CompareWithInvariantCulture(val, "CustomCircular3") && !Validator.CompareWithInvariantCulture(val, "CustomCircular4") && !Validator.CompareWithInvariantCulture(val, "CustomCircular5") && !Validator.CompareWithInvariantCulture(val, "CustomCircular6") && !Validator.CompareWithInvariantCulture(val, "CustomCircular7") && !Validator.CompareWithInvariantCulture(val, "CustomCircular8") && !Validator.CompareWithInvariantCulture(val, "CustomCircular9") && !Validator.CompareWithInvariantCulture(val, "CustomCircular10") && !Validator.CompareWithInvariantCulture(val, "CustomCircular11") && !Validator.CompareWithInvariantCulture(val, "CustomCircular12") && !Validator.CompareWithInvariantCulture(val, "CustomCircular13") && !Validator.CompareWithInvariantCulture(val, "CustomCircular14") && !Validator.CompareWithInvariantCulture(val, "CustomCircular15") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN1") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN2") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN3") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN4") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS1") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS2") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS3") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS4") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE1") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE2") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE3") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE4") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW1") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW2") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW3") && !Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW4") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE1") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE2") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE3") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE4") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW1") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW2") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW3") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW4") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE1") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE2") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE3") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE4") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW1") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW2") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW3") && !Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW4"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeFrameStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Simple") && !Validator.CompareWithInvariantCulture(val, "Edged"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeGlassEffects(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Simple"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeInputValueFormulas(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Average") && !Validator.CompareWithInvariantCulture(val, "Linear") && !Validator.CompareWithInvariantCulture(val, "Max") && !Validator.CompareWithInvariantCulture(val, "Min") && !Validator.CompareWithInvariantCulture(val, "Median") && !Validator.CompareWithInvariantCulture(val, "OpenClose") && !Validator.CompareWithInvariantCulture(val, "Percentile") && !Validator.CompareWithInvariantCulture(val, "Variance") && !Validator.CompareWithInvariantCulture(val, "RateOfChange") && !Validator.CompareWithInvariantCulture(val, "Integral"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeLabelPlacements(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Inside") && !Validator.CompareWithInvariantCulture(val, "Outside") && !Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeMarkerStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Triangle") && !Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Rectangle") && !Validator.CompareWithInvariantCulture(val, "Circle") && !Validator.CompareWithInvariantCulture(val, "Diamond") && !Validator.CompareWithInvariantCulture(val, "Trapezoid") && !Validator.CompareWithInvariantCulture(val, "Star") && !Validator.CompareWithInvariantCulture(val, "Wedge") && !Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeOrientations(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Auto") && !Validator.CompareWithInvariantCulture(val, "Horizontal") && !Validator.CompareWithInvariantCulture(val, "Vertical"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugePointerPlacements(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Cross") && !Validator.CompareWithInvariantCulture(val, "Outside") && !Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeThermometerStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Standard") && !Validator.CompareWithInvariantCulture(val, "Flask"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeTickMarkShapes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Rectangle") && !Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Triangle") && !Validator.CompareWithInvariantCulture(val, "Circle") && !Validator.CompareWithInvariantCulture(val, "Diamond") && !Validator.CompareWithInvariantCulture(val, "Trapezoid") && !Validator.CompareWithInvariantCulture(val, "Star") && !Validator.CompareWithInvariantCulture(val, "Wedge") && !Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateLinearPointerTypes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Marker") && !Validator.CompareWithInvariantCulture(val, "Bar") && !Validator.CompareWithInvariantCulture(val, "Thermometer"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateRadialPointerNeedleStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Triangular") && !Validator.CompareWithInvariantCulture(val, "Rectangular") && !Validator.CompareWithInvariantCulture(val, "TaperedWithTail") && !Validator.CompareWithInvariantCulture(val, "Tapered") && !Validator.CompareWithInvariantCulture(val, "ArrowWithTail") && !Validator.CompareWithInvariantCulture(val, "Arrow") && !Validator.CompareWithInvariantCulture(val, "StealthArrowWithTail") && !Validator.CompareWithInvariantCulture(val, "StealthArrow") && !Validator.CompareWithInvariantCulture(val, "TaperedWithStealthArrow") && !Validator.CompareWithInvariantCulture(val, "StealthArrowWithWideTail") && !Validator.CompareWithInvariantCulture(val, "TaperedWithRoundedPoint"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateRadialPointerTypes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Needle") && !Validator.CompareWithInvariantCulture(val, "Marker") && !Validator.CompareWithInvariantCulture(val, "Bar"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateScaleRangePlacements(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Inside") && !Validator.CompareWithInvariantCulture(val, "Outside") && !Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateBackgroundGradientTypes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "StartToEnd") && !Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "LeftRight") && !Validator.CompareWithInvariantCulture(val, "TopBottom") && !Validator.CompareWithInvariantCulture(val, "Center") && !Validator.CompareWithInvariantCulture(val, "DiagonalLeft") && !Validator.CompareWithInvariantCulture(val, "DiagonalRight") && !Validator.CompareWithInvariantCulture(val, "HorizontalCenter") && !Validator.CompareWithInvariantCulture(val, "VerticalCenter"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateTextAntiAliasingQualities(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "High") && !Validator.CompareWithInvariantCulture(val, "Normal") && !Validator.CompareWithInvariantCulture(val, "SystemDefault"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeResizeModes(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "AutoFit") && !Validator.CompareWithInvariantCulture(val, "None"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateImageSourceType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (Validator.ValidateImageSourceType(val))
			{
				return true;
			}
			Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateMimeType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (Validator.ValidateMimeType(val))
			{
				return true;
			}
			Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
			return false;
		}

		internal static bool ValidateGaugeIndicatorStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Mechanical") && !Validator.CompareWithInvariantCulture(val, "Digital7Segment") && !Validator.CompareWithInvariantCulture(val, "Digital14Segment"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeShowSigns(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "NegativeOnly") && !Validator.CompareWithInvariantCulture(val, "Both") && !Validator.CompareWithInvariantCulture(val, "None"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeStateIndicatorStyles(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Circle") && !Validator.CompareWithInvariantCulture(val, "Flag") && !Validator.CompareWithInvariantCulture(val, "ArrowDown") && !Validator.CompareWithInvariantCulture(val, "ArrowDownIncline") && !Validator.CompareWithInvariantCulture(val, "ArrowSide") && !Validator.CompareWithInvariantCulture(val, "ArrowUp") && !Validator.CompareWithInvariantCulture(val, "ArrowUpIncline") && !Validator.CompareWithInvariantCulture(val, "BoxesAllFilled") && !Validator.CompareWithInvariantCulture(val, "BoxesNoneFilled") && !Validator.CompareWithInvariantCulture(val, "BoxesOneFilled") && !Validator.CompareWithInvariantCulture(val, "BoxesTwoFilled") && !Validator.CompareWithInvariantCulture(val, "BoxesThreeFilled") && !Validator.CompareWithInvariantCulture(val, "LightArrowDown") && !Validator.CompareWithInvariantCulture(val, "LightArrowDownIncline") && !Validator.CompareWithInvariantCulture(val, "LightArrowSide") && !Validator.CompareWithInvariantCulture(val, "LightArrowUp") && !Validator.CompareWithInvariantCulture(val, "LightArrowUpIncline") && !Validator.CompareWithInvariantCulture(val, "QuartersAllFilled") && !Validator.CompareWithInvariantCulture(val, "QuartersNoneFilled") && !Validator.CompareWithInvariantCulture(val, "QuartersOneFilled") && !Validator.CompareWithInvariantCulture(val, "QuartersTwoFilled") && !Validator.CompareWithInvariantCulture(val, "QuartersThreeFilled") && !Validator.CompareWithInvariantCulture(val, "SignalMeterFourFilled") && !Validator.CompareWithInvariantCulture(val, "SignalMeterNoneFilled") && !Validator.CompareWithInvariantCulture(val, "SignalMeterOneFilled") && !Validator.CompareWithInvariantCulture(val, "SignalMeterThreeFilled") && !Validator.CompareWithInvariantCulture(val, "SignalMeterTwoFilled") && !Validator.CompareWithInvariantCulture(val, "StarQuartersAllFilled") && !Validator.CompareWithInvariantCulture(val, "StarQuartersNoneFilled") && !Validator.CompareWithInvariantCulture(val, "StarQuartersOneFilled") && !Validator.CompareWithInvariantCulture(val, "StarQuartersTwoFilled") && !Validator.CompareWithInvariantCulture(val, "StarQuartersThreeFilled") && !Validator.CompareWithInvariantCulture(val, "ThreeSignsCircle") && !Validator.CompareWithInvariantCulture(val, "ThreeSignsDiamond") && !Validator.CompareWithInvariantCulture(val, "ThreeSignsTriangle") && !Validator.CompareWithInvariantCulture(val, "ThreeSymbolCheck") && !Validator.CompareWithInvariantCulture(val, "ThreeSymbolCross") && !Validator.CompareWithInvariantCulture(val, "ThreeSymbolExclamation") && !Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCheck") && !Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCross") && !Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledExclamation") && !Validator.CompareWithInvariantCulture(val, "TrafficLight") && !Validator.CompareWithInvariantCulture(val, "TrafficLightUnrimmed") && !Validator.CompareWithInvariantCulture(val, "TriangleDash") && !Validator.CompareWithInvariantCulture(val, "TriangleDown") && !Validator.CompareWithInvariantCulture(val, "TriangleUp") && !Validator.CompareWithInvariantCulture(val, "ButtonStop") && !Validator.CompareWithInvariantCulture(val, "ButtonPlay") && !Validator.CompareWithInvariantCulture(val, "ButtonPause") && !Validator.CompareWithInvariantCulture(val, "FaceSmile") && !Validator.CompareWithInvariantCulture(val, "FaceNeutral") && !Validator.CompareWithInvariantCulture(val, "FaceFrown") && !Validator.CompareWithInvariantCulture(val, "Image") && !Validator.CompareWithInvariantCulture(val, "None"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateGaugeTransformationType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.IsStateIndicatorTransformationTypePercent(val) && !Validator.CompareWithInvariantCulture(val, "None"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool IsStateIndicatorTransformationTypePercent(string val)
		{
			return Validator.CompareWithInvariantCulture(val, "Percentage");
		}

		internal static bool ValidateMapLegendTitleSeparator(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Line") && !Validator.CompareWithInvariantCulture(val, "ThickLine") && !Validator.CompareWithInvariantCulture(val, "DoubleLine") && !Validator.CompareWithInvariantCulture(val, "DashLine") && !Validator.CompareWithInvariantCulture(val, "DotLine") && !Validator.CompareWithInvariantCulture(val, "GradientLine") && !Validator.CompareWithInvariantCulture(val, "ThickGradientLine"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapLegendLayout(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "AutoTable") && !Validator.CompareWithInvariantCulture(val, "Column") && !Validator.CompareWithInvariantCulture(val, "Row") && !Validator.CompareWithInvariantCulture(val, "WideTable") && !Validator.CompareWithInvariantCulture(val, "TallTable"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapAutoBool(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Auto") && !Validator.CompareWithInvariantCulture(val, "True") && !Validator.CompareWithInvariantCulture(val, "False"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateLabelPlacement(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Alternate") && !Validator.CompareWithInvariantCulture(val, "Top") && !Validator.CompareWithInvariantCulture(val, "Bottom"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateLabelBehavior(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Auto") && !Validator.CompareWithInvariantCulture(val, "ShowMiddleValue") && !Validator.CompareWithInvariantCulture(val, "ShowBorderValue"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateUnit(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Percentage") && !Validator.CompareWithInvariantCulture(val, "Inch") && !Validator.CompareWithInvariantCulture(val, "Point") && !Validator.CompareWithInvariantCulture(val, "Centimeter") && !Validator.CompareWithInvariantCulture(val, "Millimeter") && !Validator.CompareWithInvariantCulture(val, "Pica"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateLabelPosition(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Near") && !Validator.CompareWithInvariantCulture(val, "OneQuarter") && !Validator.CompareWithInvariantCulture(val, "Center") && !Validator.CompareWithInvariantCulture(val, "ThreeQuarters") && !Validator.CompareWithInvariantCulture(val, "Far"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapPosition(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "TopCenter") && !Validator.CompareWithInvariantCulture(val, "TopLeft") && !Validator.CompareWithInvariantCulture(val, "TopRight") && !Validator.CompareWithInvariantCulture(val, "LeftTop") && !Validator.CompareWithInvariantCulture(val, "LeftCenter") && !Validator.CompareWithInvariantCulture(val, "LeftBottom") && !Validator.CompareWithInvariantCulture(val, "RightTop") && !Validator.CompareWithInvariantCulture(val, "RightCenter") && !Validator.CompareWithInvariantCulture(val, "RightBottom") && !Validator.CompareWithInvariantCulture(val, "BottomRight") && !Validator.CompareWithInvariantCulture(val, "BottomCenter") && !Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapCoordinateSystem(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Planar") && !Validator.CompareWithInvariantCulture(val, "Geographic"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapProjection(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Equirectangular") && !Validator.CompareWithInvariantCulture(val, "Mercator") && !Validator.CompareWithInvariantCulture(val, "Robinson") && !Validator.CompareWithInvariantCulture(val, "Fahey") && !Validator.CompareWithInvariantCulture(val, "Eckert1") && !Validator.CompareWithInvariantCulture(val, "Eckert3") && !Validator.CompareWithInvariantCulture(val, "HammerAitoff") && !Validator.CompareWithInvariantCulture(val, "Wagner3") && !Validator.CompareWithInvariantCulture(val, "Bonne"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapPalette(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Random") && !Validator.CompareWithInvariantCulture(val, "Light") && !Validator.CompareWithInvariantCulture(val, "SemiTransparent") && !Validator.CompareWithInvariantCulture(val, "BrightPastel") && !Validator.CompareWithInvariantCulture(val, "Pacific"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapRuleDistributionType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Optimal") && !Validator.CompareWithInvariantCulture(val, "EqualInterval") && !Validator.CompareWithInvariantCulture(val, "EqualDistribution") && !Validator.CompareWithInvariantCulture(val, "Custom"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapResizeMode(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "AutoFit") && !Validator.CompareWithInvariantCulture(val, "None"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapMarkerStyle(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Rectangle") && !Validator.CompareWithInvariantCulture(val, "Circle") && !Validator.CompareWithInvariantCulture(val, "Diamond") && !Validator.CompareWithInvariantCulture(val, "Triangle") && !Validator.CompareWithInvariantCulture(val, "Trapezoid") && !Validator.CompareWithInvariantCulture(val, "Star") && !Validator.CompareWithInvariantCulture(val, "Wedge") && !Validator.CompareWithInvariantCulture(val, "Pentagon") && !Validator.CompareWithInvariantCulture(val, "PushPin") && !Validator.CompareWithInvariantCulture(val, "Image"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapLineLabelPlacement(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Above") && !Validator.CompareWithInvariantCulture(val, "Center") && !Validator.CompareWithInvariantCulture(val, "Below"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapPolygonLabelPlacement(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "MiddleCenter") && !Validator.CompareWithInvariantCulture(val, "MiddleLeft") && !Validator.CompareWithInvariantCulture(val, "MiddleRight") && !Validator.CompareWithInvariantCulture(val, "TopCenter") && !Validator.CompareWithInvariantCulture(val, "TopLeft") && !Validator.CompareWithInvariantCulture(val, "TopRight") && !Validator.CompareWithInvariantCulture(val, "BottomCenter") && !Validator.CompareWithInvariantCulture(val, "BottomLeft") && !Validator.CompareWithInvariantCulture(val, "BottomRight"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapPointLabelPlacement(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Bottom") && !Validator.CompareWithInvariantCulture(val, "Top") && !Validator.CompareWithInvariantCulture(val, "Left") && !Validator.CompareWithInvariantCulture(val, "Right") && !Validator.CompareWithInvariantCulture(val, "Center"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapTileStyle(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Road") && !Validator.CompareWithInvariantCulture(val, "Aerial") && !Validator.CompareWithInvariantCulture(val, "Hybrid"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapVisibilityMode(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Visible") && !Validator.CompareWithInvariantCulture(val, "Hidden") && !Validator.CompareWithInvariantCulture(val, "ZoomBased"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateDataType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Boolean") && !Validator.CompareWithInvariantCulture(val, "DateTime") && !Validator.CompareWithInvariantCulture(val, "Integer") && !Validator.CompareWithInvariantCulture(val, "Float") && !Validator.CompareWithInvariantCulture(val, "String"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapBorderSkinType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Emboss") && !Validator.CompareWithInvariantCulture(val, "Raised") && !Validator.CompareWithInvariantCulture(val, "Sunken") && !Validator.CompareWithInvariantCulture(val, "FrameThin1") && !Validator.CompareWithInvariantCulture(val, "FrameThin2") && !Validator.CompareWithInvariantCulture(val, "FrameThin3") && !Validator.CompareWithInvariantCulture(val, "FrameThin4") && !Validator.CompareWithInvariantCulture(val, "FrameThin5") && !Validator.CompareWithInvariantCulture(val, "FrameThin6") && !Validator.CompareWithInvariantCulture(val, "FrameTitle1") && !Validator.CompareWithInvariantCulture(val, "FrameTitle2") && !Validator.CompareWithInvariantCulture(val, "FrameTitle3") && !Validator.CompareWithInvariantCulture(val, "FrameTitle4") && !Validator.CompareWithInvariantCulture(val, "FrameTitle5") && !Validator.CompareWithInvariantCulture(val, "FrameTitle6") && !Validator.CompareWithInvariantCulture(val, "FrameTitle7") && !Validator.CompareWithInvariantCulture(val, "FrameTitle8"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapAntiAliasing(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "All") && !Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Text") && !Validator.CompareWithInvariantCulture(val, "Graphics"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateMapTextAntiAliasingQuality(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "High") && !Validator.CompareWithInvariantCulture(val, "Normal") && !Validator.CompareWithInvariantCulture(val, "SystemDefault"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

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
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
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

		internal static bool ValidateSize(string size, bool allowNegative, out double sizeInMM)
		{
			return Validator.ValidateSize(size, allowNegative ? Validator.NegativeMin : Validator.NormalMin, Validator.NormalMax, out sizeInMM);
		}

		private static bool ValidateSize(string size, double minValue, double maxValue, out double sizeInMM)
		{
			RVUnit rVUnit = default(RVUnit);
			if (Validator.ValidateSizeString(size, out rVUnit) && Validator.ValidateSizeUnitType(rVUnit))
			{
				sizeInMM = Converter.ConvertToMM(rVUnit);
				return Validator.ValidateSizeValue(sizeInMM, minValue, maxValue);
			}
			sizeInMM = 0.0;
			return false;
		}

		internal static bool ValidateSizeString(string sizeString, out RVUnit sizeValue)
		{
			try
			{
				sizeValue = new RVUnit(sizeString, CultureInfo.InvariantCulture, RVUnitType.Pixel);
				if (sizeValue.Type == RVUnitType.Pixel)
				{
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
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

		internal static int CompareDoubles(double first, double second)
		{
			double num = Math.Round(first, 4);
			double num2 = Math.Round(second, 4);
			long num3 = (long)(num * Math.Pow(10.0, 3.0));
			long num4 = (long)(num2 * Math.Pow(10.0, 3.0));
			if (num3 < num4)
			{
				return -1;
			}
			if (num3 > num4)
			{
				return 1;
			}
			return 0;
		}

		internal static bool ValidateEmbeddedImageName(string embeddedImageName, Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> embeddedImages)
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
			if (language == null)
			{
				culture = null;
				return true;
			}
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
			if (language == null)
			{
				culture = null;
				return true;
			}
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

		private static bool CreateCalendar(string calendarName, out Calendar calendar)
		{
			calendar = null;
			bool result = false;
			if (Validator.CompareWithInvariantCulture(calendarName, "GregorianUSEnglish"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "GregorianArabic"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.Arabic);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "GregorianMiddleEastFrench"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.MiddleEastFrench);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "GregorianTransliteratedEnglish"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedEnglish);
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "GregorianTransliteratedFrench"))
			{
				result = true;
				calendar = new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
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
			else if (Validator.CompareWithInvariantCulture(calendarName, "Korean"))
			{
				calendar = new KoreanCalendar();
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "Taiwan"))
			{
				calendar = new TaiwanCalendar();
			}
			else if (Validator.CompareWithInvariantCulture(calendarName, "ThaiBuddhist"))
			{
				calendar = new ThaiBuddhistCalendar();
			}
			return result;
		}

		internal static bool ValidateCalendar(CultureInfo langauge, string calendarName)
		{
			if (!Validator.CompareWithInvariantCulture(calendarName, "Default") && !Validator.CompareWithInvariantCulture(calendarName, "Gregorian"))
			{
				Calendar calendar = default(Calendar);
				bool flag = Validator.CreateCalendar(calendarName, out calendar);
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
							if (!flag)
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
			return true;
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

		internal static bool ValidatePalette(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Default") && !Validator.CompareWithInvariantCulture(val, "EarthTones") && !Validator.CompareWithInvariantCulture(val, "Excel") && !Validator.CompareWithInvariantCulture(val, "GrayScale") && !Validator.CompareWithInvariantCulture(val, "Light") && !Validator.CompareWithInvariantCulture(val, "Pastel") && !Validator.CompareWithInvariantCulture(val, "SemiTransparent") && !Validator.CompareWithInvariantCulture(val, "Berry") && !Validator.CompareWithInvariantCulture(val, "Chocolate") && !Validator.CompareWithInvariantCulture(val, "Fire") && !Validator.CompareWithInvariantCulture(val, "SeaGreen") && !Validator.CompareWithInvariantCulture(val, "BrightPastel") && !Validator.CompareWithInvariantCulture(val, "Pacific") && !Validator.CompareWithInvariantCulture(val, "PacificLight") && !Validator.CompareWithInvariantCulture(val, "PacificSemiTransparent") && !Validator.CompareWithInvariantCulture(val, "Custom"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidatePaletteHatchBehavior(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Default") && !Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Always"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartIntervalType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Default") && !Validator.CompareWithInvariantCulture(val, "Auto") && !Validator.CompareWithInvariantCulture(val, "Number") && !Validator.CompareWithInvariantCulture(val, "Years") && !Validator.CompareWithInvariantCulture(val, "Months") && !Validator.CompareWithInvariantCulture(val, "Weeks") && !Validator.CompareWithInvariantCulture(val, "Days") && !Validator.CompareWithInvariantCulture(val, "Hours") && !Validator.CompareWithInvariantCulture(val, "Minutes") && !Validator.CompareWithInvariantCulture(val, "Seconds") && !Validator.CompareWithInvariantCulture(val, "Milliseconds"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartTickMarksType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Inside") && !Validator.CompareWithInvariantCulture(val, "Outside") && !Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartColumnType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Text") && !Validator.CompareWithInvariantCulture(val, "SeriesSymbol"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartCellType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Text") && !Validator.CompareWithInvariantCulture(val, "SeriesSymbol") && !Validator.CompareWithInvariantCulture(val, "Image"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartCellAlignment(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Center") && !Validator.CompareWithInvariantCulture(val, "Top") && !Validator.CompareWithInvariantCulture(val, "TopLeft") && !Validator.CompareWithInvariantCulture(val, "TopRight") && !Validator.CompareWithInvariantCulture(val, "Left") && !Validator.CompareWithInvariantCulture(val, "Right") && !Validator.CompareWithInvariantCulture(val, "BottomRight") && !Validator.CompareWithInvariantCulture(val, "Bottom") && !Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartAllowOutsideChartArea(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Partial") && !Validator.CompareWithInvariantCulture(val, "True") && !Validator.CompareWithInvariantCulture(val, "False"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartCalloutLineAnchor(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Arrow") && !Validator.CompareWithInvariantCulture(val, "Diamond") && !Validator.CompareWithInvariantCulture(val, "Square") && !Validator.CompareWithInvariantCulture(val, "Round") && !Validator.CompareWithInvariantCulture(val, "None"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartCalloutLineStyle(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Solid") && !Validator.CompareWithInvariantCulture(val, "Dotted") && !Validator.CompareWithInvariantCulture(val, "Dashed") && !Validator.CompareWithInvariantCulture(val, "Double") && !Validator.CompareWithInvariantCulture(val, "DashDot") && !Validator.CompareWithInvariantCulture(val, "DashDotDot") && !Validator.CompareWithInvariantCulture(val, "None"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartCalloutStyle(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Underline") && !Validator.CompareWithInvariantCulture(val, "Box") && !Validator.CompareWithInvariantCulture(val, "None"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartCustomItemSeparator(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Line") && !Validator.CompareWithInvariantCulture(val, "ThickLine") && !Validator.CompareWithInvariantCulture(val, "DoubleLine") && !Validator.CompareWithInvariantCulture(val, "DashLine") && !Validator.CompareWithInvariantCulture(val, "DotLine") && !Validator.CompareWithInvariantCulture(val, "GradientLine") && !Validator.CompareWithInvariantCulture(val, "ThickGradientLine"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartSeriesFormula(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "BollingerBands") && !Validator.CompareWithInvariantCulture(val, "MovingAverage") && !Validator.CompareWithInvariantCulture(val, "ExponentialMovingAverage") && !Validator.CompareWithInvariantCulture(val, "TriangularMovingAverage") && !Validator.CompareWithInvariantCulture(val, "WeightedMovingAverage") && !Validator.CompareWithInvariantCulture(val, "MACD") && !Validator.CompareWithInvariantCulture(val, "DetrendedPriceOscillator") && !Validator.CompareWithInvariantCulture(val, "Envelopes") && !Validator.CompareWithInvariantCulture(val, "Performance") && !Validator.CompareWithInvariantCulture(val, "RateOfChange") && !Validator.CompareWithInvariantCulture(val, "RelativeStrengthIndex") && !Validator.CompareWithInvariantCulture(val, "StandardDeviation") && !Validator.CompareWithInvariantCulture(val, "TRIX") && !Validator.CompareWithInvariantCulture(val, "Mean") && !Validator.CompareWithInvariantCulture(val, "Median") && !Validator.CompareWithInvariantCulture(val, "Error"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartSeriesType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Column") && !Validator.CompareWithInvariantCulture(val, "Bar") && !Validator.CompareWithInvariantCulture(val, "Line") && !Validator.CompareWithInvariantCulture(val, "Shape") && !Validator.CompareWithInvariantCulture(val, "Scatter") && !Validator.CompareWithInvariantCulture(val, "Area") && !Validator.CompareWithInvariantCulture(val, "Range") && !Validator.CompareWithInvariantCulture(val, "Polar"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartSeriesSubtype(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName, string elementNamespace)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Plain") && !Validator.CompareWithInvariantCulture(val, "Stacked") && !Validator.CompareWithInvariantCulture(val, "PercentStacked") && !Validator.CompareWithInvariantCulture(val, "Smooth") && !Validator.CompareWithInvariantCulture(val, "Stepped") && !Validator.CompareWithInvariantCulture(val, "Pie") && !Validator.CompareWithInvariantCulture(val, "ExplodedPie") && !Validator.CompareWithInvariantCulture(val, "Doughnut") && !Validator.CompareWithInvariantCulture(val, "ExplodedDoughnut") && !Validator.CompareWithInvariantCulture(val, "Funnel") && !Validator.CompareWithInvariantCulture(val, "Pyramid") && !Validator.CompareWithInvariantCulture(val, "Bubble") && !Validator.CompareWithInvariantCulture(val, "Candlestick") && !Validator.CompareWithInvariantCulture(val, "Stock") && !Validator.CompareWithInvariantCulture(val, "Bar") && !Validator.CompareWithInvariantCulture(val, "Column") && !Validator.CompareWithInvariantCulture(val, "BoxPlot") && !Validator.CompareWithInvariantCulture(val, "ErrorBar") && !Validator.CompareWithInvariantCulture(val, "Radar"))
			{
				if (RdlNamespaceComparer.Instance.Compare(elementNamespace, "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition") >= 0 && Validator.CompareWithInvariantCulture(val, "Map"))
				{
					return true;
				}
				if (RdlNamespaceComparer.Instance.Compare(elementNamespace, "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition") >= 0 && (Validator.CompareWithInvariantCulture(val, "TreeMap") || Validator.CompareWithInvariantCulture(val, "Sunburst")))
				{
					return true;
				}
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool IsValidChartSeriesSubType(string type, string subType)
		{
			if (Validator.CompareWithInvariantCulture(subType, "Plain"))
			{
				return true;
			}
			if (Validator.CompareWithInvariantCulture(type, "Area") && (Validator.CompareWithInvariantCulture(subType, "Smooth") || Validator.CompareWithInvariantCulture(subType, "Stacked") || Validator.CompareWithInvariantCulture(subType, "PercentStacked")))
			{
				return true;
			}
			if (!Validator.CompareWithInvariantCulture(type, "Bar") && !Validator.CompareWithInvariantCulture(type, "Column"))
			{
				goto IL_007b;
			}
			if (!Validator.CompareWithInvariantCulture(subType, "Stacked") && !Validator.CompareWithInvariantCulture(subType, "PercentStacked"))
			{
				goto IL_007b;
			}
			return true;
			IL_007b:
			if (Validator.CompareWithInvariantCulture(type, "Line") && (Validator.CompareWithInvariantCulture(subType, "Smooth") || Validator.CompareWithInvariantCulture(subType, "Stepped")))
			{
				return true;
			}
			if (Validator.CompareWithInvariantCulture(type, "Polar") && Validator.CompareWithInvariantCulture(subType, "Radar"))
			{
				return true;
			}
			if (Validator.CompareWithInvariantCulture(type, "Range") && (Validator.CompareWithInvariantCulture(subType, "Candlestick") || Validator.CompareWithInvariantCulture(subType, "Stock") || Validator.CompareWithInvariantCulture(subType, "Smooth") || Validator.CompareWithInvariantCulture(subType, "Bar") || Validator.CompareWithInvariantCulture(subType, "Column") || Validator.CompareWithInvariantCulture(subType, "BoxPlot") || Validator.CompareWithInvariantCulture(subType, "ErrorBar")))
			{
				return true;
			}
			if (Validator.CompareWithInvariantCulture(type, "Scatter") && Validator.CompareWithInvariantCulture(subType, "Bubble"))
			{
				return true;
			}
			if (Validator.CompareWithInvariantCulture(type, "Shape") && (Validator.CompareWithInvariantCulture(subType, "Pie") || Validator.CompareWithInvariantCulture(subType, "ExplodedPie") || Validator.CompareWithInvariantCulture(subType, "Doughnut") || Validator.CompareWithInvariantCulture(subType, "ExplodedDoughnut") || Validator.CompareWithInvariantCulture(subType, "Funnel") || Validator.CompareWithInvariantCulture(subType, "Pyramid") || Validator.CompareWithInvariantCulture(subType, "TreeMap") || Validator.CompareWithInvariantCulture(subType, "Sunburst")))
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateChartAxisLocation(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Default") && !Validator.CompareWithInvariantCulture(val, "Opposite"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartAxisArrow(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Triangle") && !Validator.CompareWithInvariantCulture(val, "SharpTriangle") && !Validator.CompareWithInvariantCulture(val, "Lines"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartBorderSkinType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Emboss") && !Validator.CompareWithInvariantCulture(val, "Raised") && !Validator.CompareWithInvariantCulture(val, "Sunken") && !Validator.CompareWithInvariantCulture(val, "FrameThin1") && !Validator.CompareWithInvariantCulture(val, "FrameThin2") && !Validator.CompareWithInvariantCulture(val, "FrameThin3") && !Validator.CompareWithInvariantCulture(val, "FrameThin4") && !Validator.CompareWithInvariantCulture(val, "FrameThin5") && !Validator.CompareWithInvariantCulture(val, "FrameThin6") && !Validator.CompareWithInvariantCulture(val, "FrameTitle1") && !Validator.CompareWithInvariantCulture(val, "FrameTitle2") && !Validator.CompareWithInvariantCulture(val, "FrameTitle3") && !Validator.CompareWithInvariantCulture(val, "FrameTitle4") && !Validator.CompareWithInvariantCulture(val, "FrameTitle5") && !Validator.CompareWithInvariantCulture(val, "FrameTitle6") && !Validator.CompareWithInvariantCulture(val, "FrameTitle7") && !Validator.CompareWithInvariantCulture(val, "FrameTitle8"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartTitlePositions(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "TopCenter") && !Validator.CompareWithInvariantCulture(val, "TopLeft") && !Validator.CompareWithInvariantCulture(val, "TopRight") && !Validator.CompareWithInvariantCulture(val, "LeftTop") && !Validator.CompareWithInvariantCulture(val, "LeftCenter") && !Validator.CompareWithInvariantCulture(val, "LeftBottom") && !Validator.CompareWithInvariantCulture(val, "RightTop") && !Validator.CompareWithInvariantCulture(val, "RightCenter") && !Validator.CompareWithInvariantCulture(val, "RightBottom") && !Validator.CompareWithInvariantCulture(val, "BottomRight") && !Validator.CompareWithInvariantCulture(val, "BottomCenter") && !Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartAxisTitlePositions(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Center") && !Validator.CompareWithInvariantCulture(val, "Near") && !Validator.CompareWithInvariantCulture(val, "Far"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartTitleDockings(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Top") && !Validator.CompareWithInvariantCulture(val, "Left") && !Validator.CompareWithInvariantCulture(val, "Right") && !Validator.CompareWithInvariantCulture(val, "Bottom"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartAxisLabelRotation(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Rotate15") && !Validator.CompareWithInvariantCulture(val, "Rotate30") && !Validator.CompareWithInvariantCulture(val, "Rotate45") && !Validator.CompareWithInvariantCulture(val, "Rotate90"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartBreakLineType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Ragged") && !Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Straight") && !Validator.CompareWithInvariantCulture(val, "Wave"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartAutoBool(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Auto") && !Validator.CompareWithInvariantCulture(val, "True") && !Validator.CompareWithInvariantCulture(val, "False"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartDataLabelPosition(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Auto") && !Validator.CompareWithInvariantCulture(val, "Top") && !Validator.CompareWithInvariantCulture(val, "TopLeft") && !Validator.CompareWithInvariantCulture(val, "TopRight") && !Validator.CompareWithInvariantCulture(val, "Left") && !Validator.CompareWithInvariantCulture(val, "Center") && !Validator.CompareWithInvariantCulture(val, "Right") && !Validator.CompareWithInvariantCulture(val, "BottomRight") && !Validator.CompareWithInvariantCulture(val, "BottomLeft") && !Validator.CompareWithInvariantCulture(val, "Bottom") && !Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartMarkerType(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Square") && !Validator.CompareWithInvariantCulture(val, "Circle") && !Validator.CompareWithInvariantCulture(val, "Diamond") && !Validator.CompareWithInvariantCulture(val, "Triangle") && !Validator.CompareWithInvariantCulture(val, "Cross") && !Validator.CompareWithInvariantCulture(val, "Star4") && !Validator.CompareWithInvariantCulture(val, "Star5") && !Validator.CompareWithInvariantCulture(val, "Star6") && !Validator.CompareWithInvariantCulture(val, "Star10") && !Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartThreeDProjectionMode(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Oblique") && !Validator.CompareWithInvariantCulture(val, "Perspective"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateChartThreeDShading(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "None") && !Validator.CompareWithInvariantCulture(val, "Real") && !Validator.CompareWithInvariantCulture(val, "Simple"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		internal static bool ValidateBackgroundHatchType(string backgroundHatchType)
		{
			if (!Validator.CompareWithInvariantCulture(backgroundHatchType, "Default") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "None") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "BackwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Cross") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DarkDownwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DarkHorizontal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DarkUpwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DarkVertical") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DashedDownwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DashedHorizontal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DashedUpwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DashedVertical") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DiagonalBrick") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DiagonalCross") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Divot") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DottedDiamond") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "DottedGrid") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "ForwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Horizontal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "HorizontalBrick") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "LargeCheckerBoard") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "LargeConfetti") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "LargeGrid") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "LightDownwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "LightHorizontal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "LightUpwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "LightVertical") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "NarrowHorizontal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "NarrowVertical") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "OutlinedDiamond") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent05") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent10") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent20") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent25") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent30") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent40") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent50") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent60") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent70") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent75") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent80") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Percent90") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Plaid") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Shingle") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "SmallCheckerBoard") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "SmallConfetti") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "SmallGrid") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "SolidDiamond") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Sphere") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Trellis") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Vertical") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Wave") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "Weave") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "WideDownwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "WideUpwardDiagonal") && !Validator.CompareWithInvariantCulture(backgroundHatchType, "ZigZag"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidatePosition(string position)
		{
			if (!Validator.CompareWithInvariantCulture(position, "Default") && !Validator.CompareWithInvariantCulture(position, "Top") && !Validator.CompareWithInvariantCulture(position, "TopLeft") && !Validator.CompareWithInvariantCulture(position, "TopRight") && !Validator.CompareWithInvariantCulture(position, "Left") && !Validator.CompareWithInvariantCulture(position, "Center") && !Validator.CompareWithInvariantCulture(position, "Right") && !Validator.CompareWithInvariantCulture(position, "BottomRight") && !Validator.CompareWithInvariantCulture(position, "Bottom") && !Validator.CompareWithInvariantCulture(position, "BottomLeft"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateTextEffect(string textEffect)
		{
			if (!Validator.CompareWithInvariantCulture(textEffect, "Default") && !Validator.CompareWithInvariantCulture(textEffect, "None") && !Validator.CompareWithInvariantCulture(textEffect, "Shadow") && !Validator.CompareWithInvariantCulture(textEffect, "Emboss") && !Validator.CompareWithInvariantCulture(textEffect, "Embed") && !Validator.CompareWithInvariantCulture(textEffect, "Frame"))
			{
				return false;
			}
			return true;
		}

		internal static bool IsDynamicImageReportItem(ObjectType objectType)
		{
			if (objectType != ObjectType.Chart && objectType != ObjectType.GaugePanel)
			{
				return objectType == ObjectType.Map;
			}
			return true;
		}

		internal static bool IsDynamicImageSubElement(IStyleContainer styleContainer)
		{
			if (Validator.IsDynamicImageReportItem(styleContainer.ObjectType))
			{
				if (!(styleContainer is AspNetCore.ReportingServices.ReportIntermediateFormat.Chart) && !(styleContainer is GaugePanel))
				{
					return !(styleContainer is Map);
				}
				return false;
			}
			return false;
		}

		internal static bool ValidateBorderStyle(string borderStyle, bool isDefaultBorder, ObjectType objectType, bool isDynamicImageSubElement, out string validatedStyle)
		{
			bool flag = Validator.IsDynamicImageReportItem(objectType);
			bool flag2 = objectType == ObjectType.Line;
			if (!Validator.CompareWithInvariantCulture(borderStyle, "Dotted") && !Validator.CompareWithInvariantCulture(borderStyle, "Dashed"))
			{
				if (!Validator.CompareWithInvariantCulture(borderStyle, "None") && !Validator.CompareWithInvariantCulture(borderStyle, "Solid") && !Validator.CompareWithInvariantCulture(borderStyle, "Double"))
				{
					if (!Validator.CompareWithInvariantCulture(borderStyle, "DashDot") && !Validator.CompareWithInvariantCulture(borderStyle, "DashDotDot"))
					{
						if (Validator.CompareWithInvariantCulture(borderStyle, "Default"))
						{
							if (isDefaultBorder)
							{
								if (flag2)
								{
									validatedStyle = "Solid";
								}
								else
								{
									validatedStyle = "None";
								}
							}
							else
							{
								validatedStyle = borderStyle;
							}
							return true;
						}
						validatedStyle = null;
						return false;
					}
					if (flag)
					{
						if (isDynamicImageSubElement)
						{
							validatedStyle = borderStyle;
						}
						else
						{
							validatedStyle = "Dashed";
						}
					}
					else
					{
						validatedStyle = null;
					}
					return flag;
				}
				if (flag2)
				{
					validatedStyle = "Solid";
				}
				else
				{
					validatedStyle = borderStyle;
				}
				return true;
			}
			validatedStyle = borderStyle;
			return true;
		}

		internal static bool ValidateImageSourceType(string val)
		{
			if (!Validator.CompareWithInvariantCulture(val, "External") && !Validator.CompareWithInvariantCulture(val, "Embedded") && !Validator.CompareWithInvariantCulture(val, "Database"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateMimeType(string mimeType)
		{
			if (mimeType == null)
			{
				return false;
			}
			if (!Validator.CompareWithInvariantCultureIgnoreCase(mimeType, "image/bmp") && !Validator.CompareWithInvariantCultureIgnoreCase(mimeType, "image/jpeg") && !Validator.CompareWithInvariantCultureIgnoreCase(mimeType, "image/gif") && !Validator.CompareWithInvariantCultureIgnoreCase(mimeType, "image/png") && !Validator.CompareWithInvariantCultureIgnoreCase(mimeType, "image/x-png"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateBackgroundGradientType(string gradientType)
		{
			if (!Validator.CompareWithInvariantCulture(gradientType, "Default") && !Validator.CompareWithInvariantCulture(gradientType, "None") && !Validator.CompareWithInvariantCulture(gradientType, "LeftRight") && !Validator.CompareWithInvariantCulture(gradientType, "TopBottom") && !Validator.CompareWithInvariantCulture(gradientType, "Center") && !Validator.CompareWithInvariantCulture(gradientType, "DiagonalLeft") && !Validator.CompareWithInvariantCulture(gradientType, "DiagonalRight") && !Validator.CompareWithInvariantCulture(gradientType, "HorizontalCenter") && !Validator.CompareWithInvariantCulture(gradientType, "VerticalCenter"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateBackgroundRepeatForNamespace(string repeat, string rdlNamespace)
		{
			if (Validator.CompareWithInvariantCulture(repeat, "FitProportional"))
			{
				return RdlNamespaceComparer.Instance.Compare(rdlNamespace, "http://schemas.microsoft.com/sqlserver/reporting/2012/01/reportdefinition") >= 0;
			}
			return true;
		}

		internal static bool ValidateBackgroundRepeat(string repeat, ObjectType objectType)
		{
			bool flag = objectType == ObjectType.Chart;
			bool result = objectType == ObjectType.ReportSection;
			if (!Validator.CompareWithInvariantCulture(repeat, "Default") && !Validator.CompareWithInvariantCulture(repeat, "Repeat") && !Validator.CompareWithInvariantCulture(repeat, "Clip"))
			{
				if (!Validator.CompareWithInvariantCulture(repeat, "RepeatX") && !Validator.CompareWithInvariantCulture(repeat, "RepeatY"))
				{
					if (Validator.CompareWithInvariantCulture(repeat, "Fit"))
					{
						if (!flag)
						{
							return result;
						}
						return true;
					}
					if (Validator.CompareWithInvariantCulture(repeat, "FitProportional"))
					{
						return result;
					}
					return false;
				}
				return !flag;
			}
			return true;
		}

		internal static bool ValidateFontStyle(string fontStyle)
		{
			if (!Validator.CompareWithInvariantCulture(fontStyle, "Default") && !Validator.CompareWithInvariantCulture(fontStyle, "Normal") && !Validator.CompareWithInvariantCulture(fontStyle, "Italic"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateFontWeight(string fontWeight)
		{
			if (!Validator.CompareWithInvariantCulture(fontWeight, "Thin") && !Validator.CompareWithInvariantCulture(fontWeight, "ExtraLight") && !Validator.CompareWithInvariantCulture(fontWeight, "Light") && !Validator.CompareWithInvariantCulture(fontWeight, "Normal") && !Validator.CompareWithInvariantCulture(fontWeight, "Default") && !Validator.CompareWithInvariantCulture(fontWeight, "Medium") && !Validator.CompareWithInvariantCulture(fontWeight, "SemiBold") && !Validator.CompareWithInvariantCulture(fontWeight, "Bold") && !Validator.CompareWithInvariantCulture(fontWeight, "ExtraBold") && !Validator.CompareWithInvariantCulture(fontWeight, "Heavy"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateTextDecoration(string textDecoration)
		{
			if (!Validator.CompareWithInvariantCulture(textDecoration, "Default") && !Validator.CompareWithInvariantCulture(textDecoration, "None") && !Validator.CompareWithInvariantCulture(textDecoration, "Underline") && !Validator.CompareWithInvariantCulture(textDecoration, "Overline") && !Validator.CompareWithInvariantCulture(textDecoration, "LineThrough"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateTextAlign(string textAlign)
		{
			if (!Validator.CompareWithInvariantCulture(textAlign, "Default") && !Validator.CompareWithInvariantCulture(textAlign, "General") && !Validator.CompareWithInvariantCulture(textAlign, "Left") && !Validator.CompareWithInvariantCulture(textAlign, "Center") && !Validator.CompareWithInvariantCulture(textAlign, "Right"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateVerticalAlign(string verticalAlign)
		{
			if (!Validator.CompareWithInvariantCulture(verticalAlign, "Default") && !Validator.CompareWithInvariantCulture(verticalAlign, "Top") && !Validator.CompareWithInvariantCulture(verticalAlign, "Middle") && !Validator.CompareWithInvariantCulture(verticalAlign, "Bottom"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateDirection(string direction)
		{
			if (!Validator.CompareWithInvariantCulture(direction, "Default") && !Validator.CompareWithInvariantCulture(direction, "LTR") && !Validator.CompareWithInvariantCulture(direction, "RTL"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateWritingMode(string writingMode)
		{
			if (!Validator.CompareWithInvariantCulture(writingMode, "Default") && !Validator.CompareWithInvariantCulture(writingMode, "Horizontal") && !Validator.CompareWithInvariantCulture(writingMode, "Vertical") && !Validator.CompareWithInvariantCulture(writingMode, "Rotate270"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateUnicodeBiDi(string unicodeBiDi)
		{
			if (!Validator.CompareWithInvariantCulture(unicodeBiDi, "Normal") && !Validator.CompareWithInvariantCulture(unicodeBiDi, "Embed") && !Validator.CompareWithInvariantCulture(unicodeBiDi, "BiDiOverride"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateCalendar(string calendar)
		{
			if (!Validator.CompareWithInvariantCulture(calendar, "Default") && !Validator.CompareWithInvariantCulture(calendar, "Gregorian") && !Validator.CompareWithInvariantCulture(calendar, "GregorianUSEnglish") && !Validator.CompareWithInvariantCulture(calendar, "GregorianArabic") && !Validator.CompareWithInvariantCulture(calendar, "GregorianMiddleEastFrench") && !Validator.CompareWithInvariantCulture(calendar, "GregorianTransliteratedEnglish") && !Validator.CompareWithInvariantCulture(calendar, "GregorianTransliteratedFrench") && !Validator.CompareWithInvariantCulture(calendar, "Hebrew") && !Validator.CompareWithInvariantCulture(calendar, "Hijri") && !Validator.CompareWithInvariantCulture(calendar, "Japanese") && !Validator.CompareWithInvariantCulture(calendar, "Korean") && !Validator.CompareWithInvariantCulture(calendar, "Taiwan") && !Validator.CompareWithInvariantCulture(calendar, "ThaiBuddhist"))
			{
				return false;
			}
			return true;
		}

		internal static bool CompareWithInvariantCulture(string strOne, string strTwo)
		{
			if (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(strOne, strTwo, false) == 0)
			{
				return true;
			}
			return false;
		}

		internal static bool CompareWithInvariantCultureIgnoreCase(string strOne, string strTwo)
		{
			if (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(strOne, strTwo, true) == 0)
			{
				return true;
			}
			return false;
		}

		internal static bool ValidateTextOrientations(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (!Validator.CompareWithInvariantCulture(val, "Auto") && !Validator.CompareWithInvariantCulture(val, "Horizontal") && !Validator.CompareWithInvariantCulture(val, "Rotated90") && !Validator.CompareWithInvariantCulture(val, "Rotated270") && !Validator.CompareWithInvariantCulture(val, "Stacked"))
			{
				Validator.RegisterInvalidEnumValueError(val, errorContext, context, propertyName);
				return false;
			}
			return true;
		}

		private static void RegisterInvalidEnumValueError(string val, ErrorContext errorContext, PublishingContextStruct context, string propertyName)
		{
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Error, context.ObjectType, context.ObjectName, propertyName, val);
			}
		}

		internal static bool ValidateTextRunMarkupType(string value)
		{
			if (!Validator.CompareWithInvariantCulture(value, "None") && !Validator.CompareWithInvariantCulture(value, "HTML"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateParagraphListStyle(string value)
		{
			if (!Validator.CompareWithInvariantCulture(value, "None") && !Validator.CompareWithInvariantCulture(value, "Numbered") && !Validator.CompareWithInvariantCulture(value, "Bulleted"))
			{
				return false;
			}
			return true;
		}

		internal static bool ValidateParagraphListLevel(int value, out int? adjustedValue)
		{
			if (value < 0)
			{
				adjustedValue = null;
				return false;
			}
			if (value > 9)
			{
				adjustedValue = 9;
				return false;
			}
			adjustedValue = value;
			return true;
		}
	}
}
