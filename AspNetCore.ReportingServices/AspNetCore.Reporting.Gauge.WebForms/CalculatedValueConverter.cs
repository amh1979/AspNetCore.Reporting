namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CalculatedValueConverter : CollectionItemTypeConverter
	{
		public CalculatedValueConverter()
		{
			base.simpleType = typeof(CalculatedValue);
		}
	}
}
