namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class GaugeImageConverter : CollectionItemTypeConverter
	{
		public GaugeImageConverter()
		{
			base.simpleType = typeof(GaugeImage);
		}
	}
}
