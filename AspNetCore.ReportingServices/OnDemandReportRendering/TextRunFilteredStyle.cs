using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TextRunFilteredStyle : Style
	{
		internal TextRunFilteredStyle(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext, bool useRenderStyle)
			: base(renderReportItem, renderingContext, useRenderStyle)
		{
		}

		protected override bool IsAvailableStyle(StyleAttributeNames styleName)
		{
			switch (styleName)
			{
			case StyleAttributeNames.FontStyle:
			case StyleAttributeNames.FontFamily:
			case StyleAttributeNames.FontSize:
			case StyleAttributeNames.FontWeight:
			case StyleAttributeNames.Format:
			case StyleAttributeNames.TextDecoration:
			case StyleAttributeNames.Color:
			case StyleAttributeNames.Language:
			case StyleAttributeNames.Calendar:
			case StyleAttributeNames.NumeralLanguage:
			case StyleAttributeNames.NumeralVariant:
			case StyleAttributeNames.CurrencyLanguage:
				return true;
			default:
				return false;
			}
		}
	}
}
