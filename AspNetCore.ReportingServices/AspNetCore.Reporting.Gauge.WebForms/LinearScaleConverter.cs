namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class LinearScaleConverter : CollectionItemTypeConverter
	{
		public LinearScaleConverter()
		{
			base.simpleType = typeof(LinearScale);
		}
	}
}
