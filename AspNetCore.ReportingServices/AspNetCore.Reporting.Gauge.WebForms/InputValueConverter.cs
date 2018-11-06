namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class InputValueConverter : CollectionItemTypeConverter
	{
		public InputValueConverter()
		{
			base.simpleType = typeof(InputValue);
		}
	}
}
