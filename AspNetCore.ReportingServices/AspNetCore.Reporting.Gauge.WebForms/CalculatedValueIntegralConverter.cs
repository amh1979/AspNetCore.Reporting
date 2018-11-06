namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CalculatedValueIntegralConverter : CollectionItemTypeConverter
	{
		public CalculatedValueIntegralConverter()
		{
			base.simpleType = typeof(CalculatedValueIntegral);
		}
	}
}
