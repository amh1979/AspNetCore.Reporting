namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CalculatedValueAverageConverter : CollectionItemTypeConverter
	{
		public CalculatedValueAverageConverter()
		{
			base.simpleType = typeof(CalculatedValueAverage);
		}
	}
}
