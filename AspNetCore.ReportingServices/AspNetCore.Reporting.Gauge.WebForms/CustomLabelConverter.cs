namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CustomLabelConverter : CollectionItemTypeConverter
	{
		public CustomLabelConverter()
		{
			base.simpleType = typeof(CustomLabel);
		}
	}
}
