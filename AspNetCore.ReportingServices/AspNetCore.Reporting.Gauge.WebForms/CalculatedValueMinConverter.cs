namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CalculatedValueMinConverter : CollectionItemTypeConverter
	{
		public CalculatedValueMinConverter()
		{
			base.simpleType = typeof(CalculatedValueMin);
		}
	}
}
