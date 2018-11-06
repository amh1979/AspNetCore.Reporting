namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularScaleConverter : CollectionItemTypeConverter
	{
		public CircularScaleConverter()
		{
			base.simpleType = typeof(CircularScale);
		}
	}
}
