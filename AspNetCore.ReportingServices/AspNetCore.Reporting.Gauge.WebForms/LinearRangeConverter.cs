namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LinearRangeConverter : CollectionItemTypeConverter
	{
		public LinearRangeConverter()
		{
			base.simpleType = typeof(LinearRange);
		}
	}
}
