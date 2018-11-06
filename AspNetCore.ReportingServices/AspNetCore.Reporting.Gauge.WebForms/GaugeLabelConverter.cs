namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class GaugeLabelConverter : CollectionItemTypeConverter
	{
		public GaugeLabelConverter()
		{
			base.simpleType = typeof(GaugeLabel);
		}
	}
}
