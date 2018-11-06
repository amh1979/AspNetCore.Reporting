namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CalculatedValueLinearConverter : CollectionItemTypeConverter
	{
		public CalculatedValueLinearConverter()
		{
			base.simpleType = typeof(CalculatedValueLinear);
		}
	}
}
