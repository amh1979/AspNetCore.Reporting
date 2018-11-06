namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CalculatedValueMaxConverter : CollectionItemTypeConverter
	{
		public CalculatedValueMaxConverter()
		{
			base.simpleType = typeof(CalculatedValueMax);
		}
	}
}
