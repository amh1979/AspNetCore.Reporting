namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class NamedImageConverter : CollectionItemTypeConverter
	{
		public NamedImageConverter()
		{
			base.simpleType = typeof(NamedImage);
		}
	}
}
