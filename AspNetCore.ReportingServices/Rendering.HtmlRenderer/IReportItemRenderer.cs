using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal interface IReportItemRenderer
	{
		void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel);
	}
}
