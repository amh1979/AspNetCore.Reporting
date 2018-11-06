namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularGaugeConverter : CollectionItemTypeConverter
	{
		public CircularGaugeConverter()
		{
			base.simpleType = typeof(CircularGauge);
		}
	}
}
