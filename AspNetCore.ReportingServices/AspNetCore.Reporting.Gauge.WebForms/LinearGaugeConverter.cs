namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LinearGaugeConverter : CollectionItemTypeConverter
	{
		public LinearGaugeConverter()
		{
			base.simpleType = typeof(LinearGauge);
		}
	}
}
