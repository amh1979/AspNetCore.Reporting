using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class EnumTranslator
	{
		internal static GaugeFrameShapes TranslateGaugeFrameShapes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeFrameShapes.Default;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return GaugeFrameShapes.Default;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circular"))
			{
				return GaugeFrameShapes.Circular;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangular"))
			{
				return GaugeFrameShapes.Rectangular;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedRectangular"))
			{
				return GaugeFrameShapes.RoundedRectangular;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoShape"))
			{
				return GaugeFrameShapes.AutoShape;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular1"))
			{
				return GaugeFrameShapes.CustomCircular1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular2"))
			{
				return GaugeFrameShapes.CustomCircular2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular3"))
			{
				return GaugeFrameShapes.CustomCircular3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular4"))
			{
				return GaugeFrameShapes.CustomCircular4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular5"))
			{
				return GaugeFrameShapes.CustomCircular5;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular6"))
			{
				return GaugeFrameShapes.CustomCircular6;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular7"))
			{
				return GaugeFrameShapes.CustomCircular7;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular8"))
			{
				return GaugeFrameShapes.CustomCircular8;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular9"))
			{
				return GaugeFrameShapes.CustomCircular9;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular10"))
			{
				return GaugeFrameShapes.CustomCircular10;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular11"))
			{
				return GaugeFrameShapes.CustomCircular11;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular12"))
			{
				return GaugeFrameShapes.CustomCircular12;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular13"))
			{
				return GaugeFrameShapes.CustomCircular13;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular14"))
			{
				return GaugeFrameShapes.CustomCircular14;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomCircular15"))
			{
				return GaugeFrameShapes.CustomCircular15;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN1"))
			{
				return GaugeFrameShapes.CustomSemiCircularN1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN2"))
			{
				return GaugeFrameShapes.CustomSemiCircularN2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN3"))
			{
				return GaugeFrameShapes.CustomSemiCircularN3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularN4"))
			{
				return GaugeFrameShapes.CustomSemiCircularN4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS1"))
			{
				return GaugeFrameShapes.CustomSemiCircularS1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS2"))
			{
				return GaugeFrameShapes.CustomSemiCircularS2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS3"))
			{
				return GaugeFrameShapes.CustomSemiCircularS3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularS4"))
			{
				return GaugeFrameShapes.CustomSemiCircularS4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE1"))
			{
				return GaugeFrameShapes.CustomSemiCircularE1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE2"))
			{
				return GaugeFrameShapes.CustomSemiCircularE2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE3"))
			{
				return GaugeFrameShapes.CustomSemiCircularE3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularE4"))
			{
				return GaugeFrameShapes.CustomSemiCircularE4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW1"))
			{
				return GaugeFrameShapes.CustomSemiCircularW1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW2"))
			{
				return GaugeFrameShapes.CustomSemiCircularW2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW3"))
			{
				return GaugeFrameShapes.CustomSemiCircularW3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomSemiCircularW4"))
			{
				return GaugeFrameShapes.CustomSemiCircularW4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNE4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNE4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularNW4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularNW4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSE4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSE4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW1"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW2"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW3"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "CustomQuarterCircularSW4"))
			{
				return GaugeFrameShapes.CustomQuarterCircularSW4;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeFrameShapes.Default;
		}

		internal static GaugeFrameStyles TranslateGaugeFrameStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeFrameStyles.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeFrameStyles.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
			{
				return GaugeFrameStyles.Simple;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Edged"))
			{
				return GaugeFrameStyles.Edged;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeFrameStyles.None;
		}

		internal static GaugeAntiAliasings TranslateGaugeAntiAliasings(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeAntiAliasings.All;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
			{
				return GaugeAntiAliasings.All;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeAntiAliasings.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
			{
				return GaugeAntiAliasings.Text;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Graphics"))
			{
				return GaugeAntiAliasings.Graphics;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeAntiAliasings.All;
		}

		internal static GaugeGlassEffects TranslateGaugeGlassEffects(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeGlassEffects.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeGlassEffects.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
			{
				return GaugeGlassEffects.Simple;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeGlassEffects.None;
		}

		internal static GaugeBarStarts TranslateGaugeBarStarts(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeBarStarts.ScaleStart;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ScaleStart"))
			{
				return GaugeBarStarts.ScaleStart;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Zero"))
			{
				return GaugeBarStarts.Zero;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeBarStarts.ScaleStart;
		}

		internal static GaugeCapStyles TranslateGaugeCapStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeCapStyles.RoundedDark;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedDark"))
			{
				return GaugeCapStyles.RoundedDark;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rounded"))
			{
				return GaugeCapStyles.Rounded;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedLight"))
			{
				return GaugeCapStyles.RoundedLight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithAdditionalTop"))
			{
				return GaugeCapStyles.RoundedWithAdditionalTop;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithWideIndentation"))
			{
				return GaugeCapStyles.RoundedWithWideIndentation;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FlattenedWithIndentation"))
			{
				return GaugeCapStyles.FlattenedWithIndentation;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FlattenedWithWideIndentation"))
			{
				return GaugeCapStyles.FlattenedWithWideIndentation;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedGlossyWithIndentation"))
			{
				return GaugeCapStyles.RoundedGlossyWithIndentation;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RoundedWithIndentation"))
			{
				return GaugeCapStyles.RoundedWithIndentation;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeCapStyles.RoundedDark;
		}

		internal static GaugeInputValueFormulas TranslateGaugeInputValueFormulas(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeInputValueFormulas.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeInputValueFormulas.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Average"))
			{
				return GaugeInputValueFormulas.Average;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Linear"))
			{
				return GaugeInputValueFormulas.Linear;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Max"))
			{
				return GaugeInputValueFormulas.Max;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Min"))
			{
				return GaugeInputValueFormulas.Min;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Median"))
			{
				return GaugeInputValueFormulas.Median;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "OpenClose"))
			{
				return GaugeInputValueFormulas.OpenClose;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentile"))
			{
				return GaugeInputValueFormulas.Percentile;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Variance"))
			{
				return GaugeInputValueFormulas.Variance;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RateOfChange"))
			{
				return GaugeInputValueFormulas.RateOfChange;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Integral"))
			{
				return GaugeInputValueFormulas.Integral;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeInputValueFormulas.None;
		}

		internal static GaugeLabelPlacements TranslateGaugeLabelPlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeLabelPlacements.Inside;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return GaugeLabelPlacements.Inside;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return GaugeLabelPlacements.Outside;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return GaugeLabelPlacements.Cross;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeLabelPlacements.Inside;
		}

		internal static GaugeMarkerStyles TranslateGaugeMarkerStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeMarkerStyles.Triangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return GaugeMarkerStyles.Triangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeMarkerStyles.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return GaugeMarkerStyles.Rectangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeMarkerStyles.Circle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return GaugeMarkerStyles.Diamond;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return GaugeMarkerStyles.Trapezoid;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return GaugeMarkerStyles.Star;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return GaugeMarkerStyles.Wedge;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return GaugeMarkerStyles.Pentagon;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeMarkerStyles.Triangle;
		}

		internal static GaugeOrientations TranslateGaugeOrientations(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeOrientations.Auto;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return GaugeOrientations.Auto;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
			{
				return GaugeOrientations.Horizontal;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Vertical"))
			{
				return GaugeOrientations.Vertical;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeOrientations.Auto;
		}

		internal static GaugePointerPlacements TranslateGaugePointerPlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugePointerPlacements.Cross;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return GaugePointerPlacements.Cross;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return GaugePointerPlacements.Outside;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return GaugePointerPlacements.Inside;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugePointerPlacements.Cross;
		}

		internal static GaugeThermometerStyles TranslateGaugeThermometerStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeThermometerStyles.Standard;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Standard"))
			{
				return GaugeThermometerStyles.Standard;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Flask"))
			{
				return GaugeThermometerStyles.Flask;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeThermometerStyles.Standard;
		}

		internal static GaugeTickMarkShapes TranslateGaugeTickMarkShapes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeTickMarkShapes.Rectangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return GaugeTickMarkShapes.Rectangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeTickMarkShapes.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return GaugeTickMarkShapes.Triangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeTickMarkShapes.Circle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return GaugeTickMarkShapes.Diamond;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return GaugeTickMarkShapes.Trapezoid;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return GaugeTickMarkShapes.Star;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return GaugeTickMarkShapes.Wedge;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return GaugeTickMarkShapes.Pentagon;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeTickMarkShapes.Rectangle;
		}

		internal static LinearPointerTypes TranslateLinearPointerTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return LinearPointerTypes.Marker;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Marker"))
			{
				return LinearPointerTypes.Marker;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
			{
				return LinearPointerTypes.Bar;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Thermometer"))
			{
				return LinearPointerTypes.Thermometer;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return LinearPointerTypes.Marker;
		}

		internal static RadialPointerNeedleStyles TranslateRadialPointerNeedleStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return RadialPointerNeedleStyles.Triangular;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangular"))
			{
				return RadialPointerNeedleStyles.Triangular;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangular"))
			{
				return RadialPointerNeedleStyles.Rectangular;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithTail"))
			{
				return RadialPointerNeedleStyles.TaperedWithTail;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Tapered"))
			{
				return RadialPointerNeedleStyles.Tapered;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowWithTail"))
			{
				return RadialPointerNeedleStyles.ArrowWithTail;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Arrow"))
			{
				return RadialPointerNeedleStyles.Arrow;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrowWithTail"))
			{
				return RadialPointerNeedleStyles.StealthArrowWithTail;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrow"))
			{
				return RadialPointerNeedleStyles.StealthArrow;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithStealthArrow"))
			{
				return RadialPointerNeedleStyles.TaperedWithStealthArrow;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StealthArrowWithWideTail"))
			{
				return RadialPointerNeedleStyles.StealthArrowWithWideTail;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TaperedWithRoundedPoint"))
			{
				return RadialPointerNeedleStyles.TaperedWithRoundedPoint;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return RadialPointerNeedleStyles.Triangular;
		}

		internal static RadialPointerTypes TranslateRadialPointerTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return RadialPointerTypes.Needle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Needle"))
			{
				return RadialPointerTypes.Needle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Marker"))
			{
				return RadialPointerTypes.Marker;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
			{
				return RadialPointerTypes.Bar;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return RadialPointerTypes.Needle;
		}

		internal static ScaleRangePlacements TranslateScaleRangePlacements(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return ScaleRangePlacements.Inside;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
			{
				return ScaleRangePlacements.Inside;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
			{
				return ScaleRangePlacements.Outside;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
			{
				return ScaleRangePlacements.Cross;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ScaleRangePlacements.Inside;
		}

		internal static BackgroundGradientTypes TranslateBackgroundGradientTypes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return BackgroundGradientTypes.StartToEnd;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StartToEnd"))
			{
				return BackgroundGradientTypes.StartToEnd;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return BackgroundGradientTypes.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftRight"))
			{
				return BackgroundGradientTypes.LeftRight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopBottom"))
			{
				return BackgroundGradientTypes.TopBottom;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return BackgroundGradientTypes.Center;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DiagonalLeft"))
			{
				return BackgroundGradientTypes.DiagonalLeft;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DiagonalRight"))
			{
				return BackgroundGradientTypes.DiagonalRight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "HorizontalCenter"))
			{
				return BackgroundGradientTypes.HorizontalCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "VerticalCenter"))
			{
				return BackgroundGradientTypes.VerticalCenter;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return BackgroundGradientTypes.StartToEnd;
		}

		internal static TextAntiAliasingQualities TranslateTextAntiAliasingQualities(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return TextAntiAliasingQualities.High;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "High"))
			{
				return TextAntiAliasingQualities.High;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Normal"))
			{
				return TextAntiAliasingQualities.Normal;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SystemDefault"))
			{
				return TextAntiAliasingQualities.SystemDefault;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return TextAntiAliasingQualities.High;
		}

		internal static GaugeResizeModes TranslateGaugeResizeModes(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeResizeModes.AutoFit;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoFit"))
			{
				return GaugeResizeModes.AutoFit;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeResizeModes.None;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeResizeModes.AutoFit;
		}

		internal static GaugeIndicatorStyles TranslateGaugeIndicatorStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeIndicatorStyles.Mechanical;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mechanical"))
			{
				return GaugeIndicatorStyles.Mechanical;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Digital7Segment"))
			{
				return GaugeIndicatorStyles.Digital7Segment;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Digital14Segment"))
			{
				return GaugeIndicatorStyles.Digital14Segment;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeIndicatorStyles.Mechanical;
		}

		internal static GaugeShowSigns TranslateGaugeShowSigns(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeShowSigns.NegativeOnly;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "NegativeOnly"))
			{
				return GaugeShowSigns.NegativeOnly;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Both"))
			{
				return GaugeShowSigns.Both;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeShowSigns.None;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeShowSigns.NegativeOnly;
		}

		internal static GaugeStateIndicatorStyles TranslateGaugeStateIndicatorStyles(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeStateIndicatorStyles.Circle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return GaugeStateIndicatorStyles.Circle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Flag"))
			{
				return GaugeStateIndicatorStyles.Flag;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowDown"))
			{
				return GaugeStateIndicatorStyles.ArrowDown;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowDownIncline"))
			{
				return GaugeStateIndicatorStyles.ArrowDownIncline;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowSide"))
			{
				return GaugeStateIndicatorStyles.ArrowSide;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowUp"))
			{
				return GaugeStateIndicatorStyles.ArrowUp;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ArrowUpIncline"))
			{
				return GaugeStateIndicatorStyles.ArrowUpIncline;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesAllFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesAllFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesNoneFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesNoneFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesOneFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesOneFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesTwoFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesTwoFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxesThreeFilled"))
			{
				return GaugeStateIndicatorStyles.BoxesThreeFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowDown"))
			{
				return GaugeStateIndicatorStyles.LightArrowDown;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowDownIncline"))
			{
				return GaugeStateIndicatorStyles.LightArrowDownIncline;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowSide"))
			{
				return GaugeStateIndicatorStyles.LightArrowSide;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowUp"))
			{
				return GaugeStateIndicatorStyles.LightArrowUp;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LightArrowUpIncline"))
			{
				return GaugeStateIndicatorStyles.LightArrowUpIncline;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersAllFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersAllFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersNoneFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersNoneFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersOneFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersOneFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersTwoFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersTwoFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "QuartersThreeFilled"))
			{
				return GaugeStateIndicatorStyles.QuartersThreeFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterFourFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterFourFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterNoneFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterNoneFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterOneFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterOneFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterThreeFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterThreeFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SignalMeterTwoFilled"))
			{
				return GaugeStateIndicatorStyles.SignalMeterTwoFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersAllFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersAllFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersNoneFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersNoneFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersOneFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersOneFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersTwoFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersTwoFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StarQuartersThreeFilled"))
			{
				return GaugeStateIndicatorStyles.StarQuartersThreeFilled;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsCircle"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsCircle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsDiamond"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsDiamond;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSignsTriangle"))
			{
				return GaugeStateIndicatorStyles.ThreeSignsTriangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolCheck"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolCheck;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolCross"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolCross;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolExclamation"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolExclamation;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCheck"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledCheck;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledCross"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledCross;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeSymbolUnCircledExclamation"))
			{
				return GaugeStateIndicatorStyles.ThreeSymbolUnCircledExclamation;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TrafficLight"))
			{
				return GaugeStateIndicatorStyles.TrafficLight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TrafficLightUnrimmed"))
			{
				return GaugeStateIndicatorStyles.TrafficLightUnrimmed;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleDash"))
			{
				return GaugeStateIndicatorStyles.TriangleDash;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleDown"))
			{
				return GaugeStateIndicatorStyles.TriangleDown;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangleUp"))
			{
				return GaugeStateIndicatorStyles.TriangleUp;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonStop"))
			{
				return GaugeStateIndicatorStyles.ButtonStop;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonPlay"))
			{
				return GaugeStateIndicatorStyles.ButtonPlay;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ButtonPause"))
			{
				return GaugeStateIndicatorStyles.ButtonPause;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceSmile"))
			{
				return GaugeStateIndicatorStyles.FaceSmile;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceNeutral"))
			{
				return GaugeStateIndicatorStyles.FaceNeutral;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FaceFrown"))
			{
				return GaugeStateIndicatorStyles.FaceFrown;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
			{
				return GaugeStateIndicatorStyles.Image;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeStateIndicatorStyles.None;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeStateIndicatorStyles.Circle;
		}

		internal static GaugeTransformationType TranslateGaugeTransformationType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return GaugeTransformationType.Percentage;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentage"))
			{
				return GaugeTransformationType.Percentage;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return GaugeTransformationType.None;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return GaugeTransformationType.Percentage;
		}

		internal static MapLegendTitleSeparator TranslateMapLegendTitleSeparator(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLegendTitleSeparator.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapLegendTitleSeparator.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
			{
				return MapLegendTitleSeparator.Line;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickLine"))
			{
				return MapLegendTitleSeparator.ThickLine;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DoubleLine"))
			{
				return MapLegendTitleSeparator.DoubleLine;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashLine"))
			{
				return MapLegendTitleSeparator.DashLine;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DotLine"))
			{
				return MapLegendTitleSeparator.DotLine;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GradientLine"))
			{
				return MapLegendTitleSeparator.GradientLine;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickGradientLine"))
			{
				return MapLegendTitleSeparator.ThickGradientLine;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapLegendTitleSeparator.None;
		}

		internal static MapLegendLayout TranslateMapLegendLayout(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLegendLayout.AutoTable;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoTable"))
			{
				return MapLegendLayout.AutoTable;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
			{
				return MapLegendLayout.Column;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Row"))
			{
				return MapLegendLayout.Row;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WideTable"))
			{
				return MapLegendLayout.WideTable;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TallTable"))
			{
				return MapLegendLayout.TallTable;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapLegendLayout.AutoTable;
		}

		internal static MapAutoBool TranslateMapAutoBool(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapAutoBool.Auto;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return MapAutoBool.Auto;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
			{
				return MapAutoBool.True;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
			{
				return MapAutoBool.False;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapAutoBool.Auto;
		}

		internal static MapLabelPlacement TranslateLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelPlacement.Alternate;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Alternate"))
			{
				return MapLabelPlacement.Alternate;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
			{
				return MapLabelPlacement.Top;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
			{
				return MapLabelPlacement.Bottom;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapLabelPlacement.Alternate;
		}

		internal static MapLabelBehavior TranslateLabelBehavior(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelBehavior.Auto;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return MapLabelBehavior.Auto;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ShowMiddleValue"))
			{
				return MapLabelBehavior.ShowMiddleValue;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ShowBorderValue"))
			{
				return MapLabelBehavior.ShowBorderValue;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapLabelBehavior.Auto;
		}

		internal static Unit TranslateUnit(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return Unit.Percentage;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Percentage"))
			{
				return Unit.Percentage;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inch"))
			{
				return Unit.Inch;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Point"))
			{
				return Unit.Point;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Centimeter"))
			{
				return Unit.Centimeter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Millimeter"))
			{
				return Unit.Millimeter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pica"))
			{
				return Unit.Pica;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return Unit.Percentage;
		}

		internal static MapLabelPosition TranslateLabelPosition(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLabelPosition.Near;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Near"))
			{
				return MapLabelPosition.Near;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "OneQuarter"))
			{
				return MapLabelPosition.OneQuarter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapLabelPosition.Center;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThreeQuarters"))
			{
				return MapLabelPosition.ThreeQuarters;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Far"))
			{
				return MapLabelPosition.Far;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapLabelPosition.Near;
		}

		internal static MapPosition TranslateMapPosition(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPosition.TopCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
			{
				return MapPosition.TopCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
			{
				return MapPosition.TopLeft;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
			{
				return MapPosition.TopRight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
			{
				return MapPosition.LeftTop;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
			{
				return MapPosition.LeftCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
			{
				return MapPosition.LeftBottom;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
			{
				return MapPosition.RightTop;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
			{
				return MapPosition.RightCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
			{
				return MapPosition.RightBottom;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
			{
				return MapPosition.BottomRight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
			{
				return MapPosition.BottomCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return MapPosition.BottomLeft;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapPosition.TopCenter;
		}

		internal static MapCoordinateSystem TranslateMapCoordinateSystem(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapCoordinateSystem.Planar;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Planar"))
			{
				return MapCoordinateSystem.Planar;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Geographic"))
			{
				return MapCoordinateSystem.Geographic;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapCoordinateSystem.Planar;
		}

		internal static MapProjection TranslateMapProjection(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapProjection.Equirectangular;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Equirectangular"))
			{
				return MapProjection.Equirectangular;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mercator"))
			{
				return MapProjection.Mercator;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Robinson"))
			{
				return MapProjection.Robinson;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Fahey"))
			{
				return MapProjection.Fahey;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Eckert1"))
			{
				return MapProjection.Eckert1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Eckert3"))
			{
				return MapProjection.Eckert3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "HammerAitoff"))
			{
				return MapProjection.HammerAitoff;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wagner3"))
			{
				return MapProjection.Wagner3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bonne"))
			{
				return MapProjection.Bonne;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapProjection.Equirectangular;
		}

		internal static MapRuleDistributionType TranslateMapRuleDistributionType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapRuleDistributionType.Optimal;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Optimal"))
			{
				return MapRuleDistributionType.Optimal;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EqualInterval"))
			{
				return MapRuleDistributionType.EqualInterval;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EqualDistribution"))
			{
				return MapRuleDistributionType.EqualDistribution;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Custom"))
			{
				return MapRuleDistributionType.Custom;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapRuleDistributionType.Optimal;
		}

		internal static MapMarkerStyle TranslateMapMarkerStyle(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapMarkerStyle.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapMarkerStyle.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rectangle"))
			{
				return MapMarkerStyle.Rectangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
			{
				return MapMarkerStyle.Circle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
			{
				return MapMarkerStyle.Diamond;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
			{
				return MapMarkerStyle.Triangle;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Trapezoid"))
			{
				return MapMarkerStyle.Trapezoid;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star"))
			{
				return MapMarkerStyle.Star;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wedge"))
			{
				return MapMarkerStyle.Wedge;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pentagon"))
			{
				return MapMarkerStyle.Pentagon;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PushPin"))
			{
				return MapMarkerStyle.PushPin;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
			{
				return MapMarkerStyle.Image;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapMarkerStyle.None;
		}

		internal static MapResizeMode TranslateMapResizeMode(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapResizeMode.AutoFit;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoFit"))
			{
				return MapResizeMode.AutoFit;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapResizeMode.None;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapResizeMode.AutoFit;
		}

		internal static MapPalette TranslateMapPalette(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPalette.Random;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Random"))
			{
				return MapPalette.Random;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Light"))
			{
				return MapPalette.Light;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SemiTransparent"))
			{
				return MapPalette.SemiTransparent;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BrightPastel"))
			{
				return MapPalette.BrightPastel;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pacific"))
			{
				return MapPalette.Pacific;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapPalette.Random;
		}

		internal static MapLineLabelPlacement TranslateMapLineLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapLineLabelPlacement.Above;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Above"))
			{
				return MapLineLabelPlacement.Above;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapLineLabelPlacement.Center;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Below"))
			{
				return MapLineLabelPlacement.Below;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapLineLabelPlacement.Above;
		}

		internal static MapPolygonLabelPlacement TranslateMapPolygonLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPolygonLabelPlacement.MiddleCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleCenter"))
			{
				return MapPolygonLabelPlacement.MiddleCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleLeft"))
			{
				return MapPolygonLabelPlacement.MiddleLeft;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MiddleRight"))
			{
				return MapPolygonLabelPlacement.MiddleRight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
			{
				return MapPolygonLabelPlacement.TopCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
			{
				return MapPolygonLabelPlacement.TopLeft;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
			{
				return MapPolygonLabelPlacement.TopRight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
			{
				return MapPolygonLabelPlacement.BottomCenter;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
			{
				return MapPolygonLabelPlacement.BottomLeft;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
			{
				return MapPolygonLabelPlacement.BottomRight;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapPolygonLabelPlacement.MiddleCenter;
		}

		internal static MapPointLabelPlacement TranslateMapPointLabelPlacement(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapPointLabelPlacement.Bottom;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
			{
				return MapPointLabelPlacement.Bottom;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
			{
				return MapPointLabelPlacement.Top;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
			{
				return MapPointLabelPlacement.Left;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
			{
				return MapPointLabelPlacement.Right;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
			{
				return MapPointLabelPlacement.Center;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapPointLabelPlacement.Bottom;
		}

		internal static MapTileStyle TranslateMapTileStyle(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapTileStyle.Road;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Road"))
			{
				return MapTileStyle.Road;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Aerial"))
			{
				return MapTileStyle.Aerial;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hybrid"))
			{
				return MapTileStyle.Hybrid;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapTileStyle.Road;
		}

		internal static MapVisibilityMode TranslateMapVisibilityMode(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapVisibilityMode.Visible;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Visible"))
			{
				return MapVisibilityMode.Visible;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hidden"))
			{
				return MapVisibilityMode.Hidden;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ZoomBased"))
			{
				return MapVisibilityMode.ZoomBased;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapVisibilityMode.Visible;
		}

		internal static MapDataType TranslateDataType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapDataType.String;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Boolean"))
			{
				return MapDataType.Boolean;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DateTime"))
			{
				return MapDataType.DateTime;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Integer"))
			{
				return MapDataType.Integer;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Float"))
			{
				return MapDataType.Float;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "String"))
			{
				return MapDataType.String;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Decimal"))
			{
				return MapDataType.Decimal;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapDataType.String;
		}

		internal static MapAntiAliasing TranslateMapAntiAliasing(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapAntiAliasing.All;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
			{
				return MapAntiAliasing.All;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapAntiAliasing.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
			{
				return MapAntiAliasing.Text;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Graphics"))
			{
				return MapAntiAliasing.Graphics;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapAntiAliasing.All;
		}

		internal static MapTextAntiAliasingQuality TranslateMapTextAntiAliasingQuality(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapTextAntiAliasingQuality.High;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "High"))
			{
				return MapTextAntiAliasingQuality.High;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Normal"))
			{
				return MapTextAntiAliasingQuality.Normal;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SystemDefault"))
			{
				return MapTextAntiAliasingQuality.SystemDefault;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapTextAntiAliasingQuality.High;
		}

		internal static MapBorderSkinType TranslateMapBorderSkinType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return MapBorderSkinType.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return MapBorderSkinType.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Emboss"))
			{
				return MapBorderSkinType.Emboss;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Raised"))
			{
				return MapBorderSkinType.Raised;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunken"))
			{
				return MapBorderSkinType.Sunken;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin1"))
			{
				return MapBorderSkinType.FrameThin1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin2"))
			{
				return MapBorderSkinType.FrameThin2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin3"))
			{
				return MapBorderSkinType.FrameThin3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin4"))
			{
				return MapBorderSkinType.FrameThin4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin5"))
			{
				return MapBorderSkinType.FrameThin5;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin6"))
			{
				return MapBorderSkinType.FrameThin6;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle1"))
			{
				return MapBorderSkinType.FrameTitle1;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle2"))
			{
				return MapBorderSkinType.FrameTitle2;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle3"))
			{
				return MapBorderSkinType.FrameTitle3;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle4"))
			{
				return MapBorderSkinType.FrameTitle4;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle5"))
			{
				return MapBorderSkinType.FrameTitle5;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle6"))
			{
				return MapBorderSkinType.FrameTitle6;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle7"))
			{
				return MapBorderSkinType.FrameTitle7;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle8"))
			{
				return MapBorderSkinType.FrameTitle8;
			}
			if (errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return MapBorderSkinType.None;
		}

		internal static ChartBreakLineType TranslateChartBreakLineType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Ragged"))
				{
					return ChartBreakLineType.Ragged;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartBreakLineType.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Straight"))
				{
					return ChartBreakLineType.Straight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Wave"))
				{
					return ChartBreakLineType.Wave;
				}
				if (errorContext != null && errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartBreakLineType.Ragged;
		}

		internal static ChartIntervalType TranslateChartIntervalType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
				{
					return ChartIntervalType.Default;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartIntervalType.Auto;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Number"))
				{
					return ChartIntervalType.Number;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Years"))
				{
					return ChartIntervalType.Years;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Months"))
				{
					return ChartIntervalType.Months;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Weeks"))
				{
					return ChartIntervalType.Weeks;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Days"))
				{
					return ChartIntervalType.Days;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Hours"))
				{
					return ChartIntervalType.Hours;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Minutes"))
				{
					return ChartIntervalType.Minutes;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Seconds"))
				{
					return ChartIntervalType.Seconds;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Milliseconds"))
				{
					return ChartIntervalType.Milliseconds;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartIntervalType.Default;
		}

		internal static ChartAutoBool TranslateChartAutoBool(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartAutoBool.Auto;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
				{
					return ChartAutoBool.True;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
				{
					return ChartAutoBool.False;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartAutoBool.Auto;
		}

		internal static ChartAxisLabelRotation TranslateChartAxisLabelRotation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAxisLabelRotation.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate30"))
				{
					return ChartAxisLabelRotation.Rotate30;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate45"))
				{
					return ChartAxisLabelRotation.Rotate45;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotate90"))
				{
					return ChartAxisLabelRotation.Rotate90;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartAxisLabelRotation.None;
		}

		internal static ChartAxisLocation TranslateChartAxisLocation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
				{
					return ChartAxisLocation.Default;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Opposite"))
				{
					return ChartAxisLocation.Opposite;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartAxisLocation.Default;
		}

		internal static ChartAxisArrow TranslateChartAxisArrow(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAxisArrow.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
				{
					return ChartAxisArrow.Triangle;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SharpTriangle"))
				{
					return ChartAxisArrow.SharpTriangle;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Lines"))
				{
					return ChartAxisArrow.Lines;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartAxisArrow.None;
		}

		internal static ChartTickMarksType TranslateChartTickMarksType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartTickMarksType.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Inside"))
				{
					return ChartTickMarksType.Inside;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
				{
					return ChartTickMarksType.Outside;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
				{
					return ChartTickMarksType.Cross;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartTickMarksType.None;
		}

		internal static ChartColumnType TranslateChartColumnType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
				{
					return ChartColumnType.Text;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeriesSymbol"))
				{
					return ChartColumnType.SeriesSymbol;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartColumnType.Text;
		}

		internal static ChartCellType TranslateChartCellType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Text"))
				{
					return ChartCellType.Text;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeriesSymbol"))
				{
					return ChartCellType.SeriesSymbol;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Image"))
				{
					return ChartCellType.Image;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartCellType.Text;
		}

		internal static ChartCellAlignment TranslateChartCellAlignment(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartCellAlignment.Center;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartCellAlignment.Top;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartCellAlignment.TopLeft;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartCellAlignment.TopRight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartCellAlignment.Left;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartCellAlignment.Right;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartCellAlignment.BottomRight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartCellAlignment.Bottom;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartCellAlignment.BottomLeft;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartCellAlignment.Center;
		}

		internal static ChartAllowOutsideChartArea TranslateChartAllowOutsideChartArea(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Partial"))
				{
					return ChartAllowOutsideChartArea.Partial;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "True"))
				{
					return ChartAllowOutsideChartArea.True;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "False"))
				{
					return ChartAllowOutsideChartArea.False;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartAllowOutsideChartArea.Partial;
		}

		internal static ChartCalloutLineAnchor TranslateChartCalloutLineAnchor(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Arrow"))
				{
					return ChartCalloutLineAnchor.Arrow;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
				{
					return ChartCalloutLineAnchor.Diamond;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Square"))
				{
					return ChartCalloutLineAnchor.Square;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Round"))
				{
					return ChartCalloutLineAnchor.Round;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutLineAnchor.None;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartCalloutLineAnchor.Arrow;
		}

		internal static ChartCalloutLineStyle TranslateChartCalloutLineStyle(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Solid"))
				{
					return ChartCalloutLineStyle.Solid;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Dotted"))
				{
					return ChartCalloutLineStyle.Dotted;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Dashed"))
				{
					return ChartCalloutLineStyle.Dashed;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Double"))
				{
					return ChartCalloutLineStyle.Double;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashDot"))
				{
					return ChartCalloutLineStyle.DashDot;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashDotDot"))
				{
					return ChartCalloutLineStyle.DashDotDot;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutLineStyle.None;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartCalloutLineStyle.Solid;
		}

		internal static ChartCalloutStyle TranslateChartCalloutStyle(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Underline"))
				{
					return ChartCalloutStyle.Underline;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Box"))
				{
					return ChartCalloutStyle.Box;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartCalloutStyle.None;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartCalloutStyle.Underline;
		}

		internal static ChartSeriesFormula TranslateChartSeriesFormula(string val)
		{
			if (val == null)
			{
				return ChartSeriesFormula.BollingerBands;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BollingerBands"))
			{
				return ChartSeriesFormula.BollingerBands;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MovingAverage"))
			{
				return ChartSeriesFormula.MovingAverage;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExponentialMovingAverage"))
			{
				return ChartSeriesFormula.ExponentialMovingAverage;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TriangularMovingAverage"))
			{
				return ChartSeriesFormula.TriangularMovingAverage;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WeightedMovingAverage"))
			{
				return ChartSeriesFormula.WeightedMovingAverage;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "MACD"))
			{
				return ChartSeriesFormula.MACD;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DetrendedPriceOscillator"))
			{
				return ChartSeriesFormula.DetrendedPriceOscillator;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Envelopes"))
			{
				return ChartSeriesFormula.Envelopes;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Performance"))
			{
				return ChartSeriesFormula.Performance;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RateOfChange"))
			{
				return ChartSeriesFormula.RateOfChange;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RelativeStrengthIndex"))
			{
				return ChartSeriesFormula.RelativeStrengthIndex;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "StandardDeviation"))
			{
				return ChartSeriesFormula.StandardDeviation;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TRIX"))
			{
				return ChartSeriesFormula.TRIX;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Mean"))
			{
				return ChartSeriesFormula.Mean;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Median"))
			{
				return ChartSeriesFormula.Median;
			}
			return ChartSeriesFormula.BollingerBands;
		}

		internal static ChartTitlePositions TranslateChartTitlePosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
				{
					return ChartTitlePositions.TopCenter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartTitlePositions.TopLeft;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartTitlePositions.TopRight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
				{
					return ChartTitlePositions.LeftTop;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
				{
					return ChartTitlePositions.LeftCenter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
				{
					return ChartTitlePositions.LeftBottom;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
				{
					return ChartTitlePositions.RightTop;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
				{
					return ChartTitlePositions.RightCenter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
				{
					return ChartTitlePositions.RightBottom;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartTitlePositions.BottomRight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
				{
					return ChartTitlePositions.BottomCenter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartTitlePositions.BottomLeft;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartTitlePositions.TopCenter;
		}

		internal static ChartTitleDockings TranslateChartTitleDocking(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartTitleDockings.Top;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartTitleDockings.Left;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartTitleDockings.Right;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartTitleDockings.Bottom;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartTitleDockings.Top;
		}

		internal static ChartAxisTitlePositions TranslateChartAxisTitlePosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartAxisTitlePositions.Center;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Near"))
				{
					return ChartAxisTitlePositions.Near;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Far"))
				{
					return ChartAxisTitlePositions.Far;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartAxisTitlePositions.Center;
		}

		internal static ChartSeparators TranslateChartSeparator(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartSeparators.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
				{
					return ChartSeparators.Line;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickLine"))
				{
					return ChartSeparators.ThickLine;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DoubleLine"))
				{
					return ChartSeparators.DoubleLine;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DashLine"))
				{
					return ChartSeparators.DashLine;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "DotLine"))
				{
					return ChartSeparators.DotLine;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GradientLine"))
				{
					return ChartSeparators.GradientLine;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ThickGradientLine"))
				{
					return ChartSeparators.ThickGradientLine;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartSeparators.None;
		}

		internal static ChartLegendLayouts TranslateChartLegendLayout(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartLegendLayouts.Column;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Row"))
				{
					return ChartLegendLayouts.Row;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "AutoTable"))
				{
					return ChartLegendLayouts.AutoTable;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "WideTable"))
				{
					return ChartLegendLayouts.WideTable;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TallTable"))
				{
					return ChartLegendLayouts.TallTable;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartLegendLayouts.AutoTable;
		}

		internal static ChartLegendPositions TranslateChartLegendPositions(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopCenter"))
				{
					return ChartLegendPositions.TopCenter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartLegendPositions.TopLeft;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartLegendPositions.TopRight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftTop"))
				{
					return ChartLegendPositions.LeftTop;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftCenter"))
				{
					return ChartLegendPositions.LeftCenter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "LeftBottom"))
				{
					return ChartLegendPositions.LeftBottom;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightTop"))
				{
					return ChartLegendPositions.RightTop;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightCenter"))
				{
					return ChartLegendPositions.RightCenter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "RightBottom"))
				{
					return ChartLegendPositions.RightBottom;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartLegendPositions.BottomRight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomCenter"))
				{
					return ChartLegendPositions.BottomCenter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartLegendPositions.BottomLeft;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartLegendPositions.RightTop;
		}

		internal static ChartAreaAlignOrientations TranslateChartAreaAlignOrientation(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartAreaAlignOrientations.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Vertical"))
				{
					return ChartAreaAlignOrientations.Vertical;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
				{
					return ChartAreaAlignOrientations.Horizontal;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "All"))
				{
					return ChartAreaAlignOrientations.All;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartAreaAlignOrientations.None;
		}

		internal static ChartThreeDProjectionModes TranslateChartThreeDProjectionMode(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Oblique"))
				{
					return ChartThreeDProjectionModes.Oblique;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Perspective"))
				{
					return ChartThreeDProjectionModes.Perspective;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartThreeDProjectionModes.Oblique;
		}

		internal static ChartThreeDShadingTypes TranslateChartThreeDShading(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartThreeDShadingTypes.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Real"))
				{
					return ChartThreeDShadingTypes.Real;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Simple"))
				{
					return ChartThreeDShadingTypes.Simple;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartThreeDShadingTypes.Real;
		}

		internal static ChartBorderSkinType TranslateChartBorderSkinType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartBorderSkinType.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Emboss"))
				{
					return ChartBorderSkinType.Emboss;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Raised"))
				{
					return ChartBorderSkinType.Raised;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunken"))
				{
					return ChartBorderSkinType.Sunken;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin1"))
				{
					return ChartBorderSkinType.FrameThin1;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin2"))
				{
					return ChartBorderSkinType.FrameThin2;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin3"))
				{
					return ChartBorderSkinType.FrameThin3;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin4"))
				{
					return ChartBorderSkinType.FrameThin4;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin5"))
				{
					return ChartBorderSkinType.FrameThin5;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameThin6"))
				{
					return ChartBorderSkinType.FrameThin6;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle1"))
				{
					return ChartBorderSkinType.FrameTitle1;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle2"))
				{
					return ChartBorderSkinType.FrameTitle2;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle3"))
				{
					return ChartBorderSkinType.FrameTitle3;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle4"))
				{
					return ChartBorderSkinType.FrameTitle4;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle5"))
				{
					return ChartBorderSkinType.FrameTitle5;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle6"))
				{
					return ChartBorderSkinType.FrameTitle6;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle7"))
				{
					return ChartBorderSkinType.FrameTitle7;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "FrameTitle8"))
				{
					return ChartBorderSkinType.FrameTitle8;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartBorderSkinType.None;
		}

		internal static ChartSeriesType TranslateChartSeriesType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartSeriesType.Column;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
				{
					return ChartSeriesType.Bar;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Line"))
				{
					return ChartSeriesType.Line;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Shape"))
				{
					return ChartSeriesType.Shape;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Scatter"))
				{
					return ChartSeriesType.Scatter;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Area"))
				{
					return ChartSeriesType.Area;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Range"))
				{
					return ChartSeriesType.Range;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Polar"))
				{
					return ChartSeriesType.Polar;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartSeriesType.Column;
		}

		internal static ChartSeriesSubtype TranslateChartSeriesSubtype(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Plain"))
				{
					return ChartSeriesSubtype.Plain;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stacked"))
				{
					return ChartSeriesSubtype.Stacked;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PercentStacked"))
				{
					return ChartSeriesSubtype.PercentStacked;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Smooth"))
				{
					return ChartSeriesSubtype.Smooth;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stepped"))
				{
					return ChartSeriesSubtype.Stepped;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pie"))
				{
					return ChartSeriesSubtype.Pie;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExplodedPie"))
				{
					return ChartSeriesSubtype.ExplodedPie;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Doughnut"))
				{
					return ChartSeriesSubtype.Doughnut;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ExplodedDoughnut"))
				{
					return ChartSeriesSubtype.ExplodedDoughnut;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Funnel"))
				{
					return ChartSeriesSubtype.Funnel;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pyramid"))
				{
					return ChartSeriesSubtype.Pyramid;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TreeMap"))
				{
					return ChartSeriesSubtype.TreeMap;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Sunburst"))
				{
					return ChartSeriesSubtype.Sunburst;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bubble"))
				{
					return ChartSeriesSubtype.Bubble;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Candlestick"))
				{
					return ChartSeriesSubtype.Candlestick;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stock"))
				{
					return ChartSeriesSubtype.Stock;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bar"))
				{
					return ChartSeriesSubtype.Bar;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Column"))
				{
					return ChartSeriesSubtype.Column;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BoxPlot"))
				{
					return ChartSeriesSubtype.BoxPlot;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "ErrorBar"))
				{
					return ChartSeriesSubtype.ErrorBar;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Radar"))
				{
					return ChartSeriesSubtype.Radar;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartSeriesSubtype.Plain;
		}

		internal static ChartDataLabelPositions TranslateChartDataLabelPosition(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartDataLabelPositions.Auto;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Top"))
				{
					return ChartDataLabelPositions.Top;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopLeft"))
				{
					return ChartDataLabelPositions.TopLeft;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "TopRight"))
				{
					return ChartDataLabelPositions.TopRight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Left"))
				{
					return ChartDataLabelPositions.Left;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Center"))
				{
					return ChartDataLabelPositions.Center;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Right"))
				{
					return ChartDataLabelPositions.Right;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomRight"))
				{
					return ChartDataLabelPositions.BottomRight;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BottomLeft"))
				{
					return ChartDataLabelPositions.BottomLeft;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Bottom"))
				{
					return ChartDataLabelPositions.Bottom;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Outside"))
				{
					return ChartDataLabelPositions.Outside;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartDataLabelPositions.Auto;
		}

		internal static ChartMarkerTypes TranslateChartMarkerType(string val, IErrorContext errorContext)
		{
			if (val != null)
			{
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
				{
					return ChartMarkerTypes.None;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Square"))
				{
					return ChartMarkerTypes.Square;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Circle"))
				{
					return ChartMarkerTypes.Circle;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Diamond"))
				{
					return ChartMarkerTypes.Diamond;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Triangle"))
				{
					return ChartMarkerTypes.Triangle;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Cross"))
				{
					return ChartMarkerTypes.Cross;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star4"))
				{
					return ChartMarkerTypes.Star4;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star5"))
				{
					return ChartMarkerTypes.Star5;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star6"))
				{
					return ChartMarkerTypes.Star6;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Star10"))
				{
					return ChartMarkerTypes.Star10;
				}
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
				{
					return ChartMarkerTypes.Auto;
				}
				if (errorContext != null)
				{
					errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
				}
			}
			return ChartMarkerTypes.None;
		}

		internal static ChartPalette TranslateChartPalette(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return ChartPalette.Default;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return ChartPalette.Default;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "EarthTones"))
			{
				return ChartPalette.EarthTones;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Excel"))
			{
				return ChartPalette.Excel;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "GrayScale"))
			{
				return ChartPalette.GrayScale;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Light"))
			{
				return ChartPalette.Light;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pastel"))
			{
				return ChartPalette.Pastel;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SemiTransparent"))
			{
				return ChartPalette.SemiTransparent;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Berry"))
			{
				return ChartPalette.Berry;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Chocolate"))
			{
				return ChartPalette.Chocolate;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Fire"))
			{
				return ChartPalette.Fire;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "SeaGreen"))
			{
				return ChartPalette.SeaGreen;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "BrightPastel"))
			{
				return ChartPalette.BrightPastel;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Pacific"))
			{
				return ChartPalette.Pacific;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PacificLight"))
			{
				return ChartPalette.PacificLight;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "PacificSemiTransparent"))
			{
				return ChartPalette.PacificSemiTransparent;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Custom"))
			{
				return ChartPalette.Custom;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return ChartPalette.Default;
		}

		internal static PaletteHatchBehavior TranslatePaletteHatchBehavior(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return PaletteHatchBehavior.Default;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Default"))
			{
				return PaletteHatchBehavior.Default;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "None"))
			{
				return PaletteHatchBehavior.None;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Always"))
			{
				return PaletteHatchBehavior.Always;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return PaletteHatchBehavior.Default;
		}

		internal static Image.SourceType TranslateImageSourceType(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return Image.SourceType.External;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "External"))
			{
				return Image.SourceType.External;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Embedded"))
			{
				return Image.SourceType.Embedded;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Database"))
			{
				return Image.SourceType.Database;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return Image.SourceType.External;
		}

		internal static TextOrientations TranslateTextOrientations(string val, IErrorContext errorContext)
		{
			if (val == null)
			{
				return TextOrientations.Auto;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Auto"))
			{
				return TextOrientations.Auto;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Horizontal"))
			{
				return TextOrientations.Horizontal;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotated90"))
			{
				return TextOrientations.Rotated90;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Rotated270"))
			{
				return TextOrientations.Rotated270;
			}
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.CompareWithInvariantCulture(val, "Stacked"))
			{
				return TextOrientations.Stacked;
			}
			if (errorContext != null && errorContext != null)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidEnumValue, Severity.Warning, val);
			}
			return TextOrientations.Auto;
		}
	}
}
