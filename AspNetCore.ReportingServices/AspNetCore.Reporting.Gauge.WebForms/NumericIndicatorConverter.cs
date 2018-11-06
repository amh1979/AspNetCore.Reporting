namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class NumericIndicatorConverter : CollectionItemTypeConverter
	{
		public NumericIndicatorConverter()
		{
			base.simpleType = typeof(NumericIndicator);
		}
	}
}
