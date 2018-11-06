namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal interface ISelectable
	{
		void DrawSelection(GaugeGraphics g, bool designTimeSelection);
	}
}
