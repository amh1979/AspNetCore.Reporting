namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapImageConverter : CollectionItemTypeConverter
	{
		public MapImageConverter()
		{
			base.simpleType = typeof(MapImage);
		}
	}
}
