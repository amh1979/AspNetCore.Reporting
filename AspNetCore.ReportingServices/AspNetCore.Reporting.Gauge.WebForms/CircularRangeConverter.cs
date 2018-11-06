namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularRangeConverter : CollectionItemTypeConverter
	{
		public CircularRangeConverter()
		{
			base.simpleType = typeof(CircularRange);
		}
	}
}
