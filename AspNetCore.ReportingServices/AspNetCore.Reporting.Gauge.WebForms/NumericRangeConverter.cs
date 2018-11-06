namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class NumericRangeConverter : CollectionItemTypeConverter
	{
		public NumericRangeConverter()
		{
			base.simpleType = typeof(NumericRange);
		}
	}
}
