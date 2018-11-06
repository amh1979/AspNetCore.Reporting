namespace AspNetCore.Reporting.Map.WebForms
{
	internal class NamedImageConverter : CollectionItemTypeConverter
	{
		public NamedImageConverter()
		{
			base.simpleType = typeof(NamedImage);
		}
	}
}
