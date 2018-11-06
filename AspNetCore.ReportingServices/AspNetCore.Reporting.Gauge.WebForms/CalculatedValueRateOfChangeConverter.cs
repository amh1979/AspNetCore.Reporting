namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CalculatedValueRateOfChangeConverter : CollectionItemTypeConverter
	{
		public CalculatedValueRateOfChangeConverter()
		{
			base.simpleType = typeof(CalculatedValueRateOfChange);
		}
	}
}
