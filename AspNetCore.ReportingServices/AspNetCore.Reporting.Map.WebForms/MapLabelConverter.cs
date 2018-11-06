namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapLabelConverter : CollectionItemTypeConverter
	{
		public MapLabelConverter()
		{
			base.simpleType = typeof(MapLabel);
		}
	}
}
