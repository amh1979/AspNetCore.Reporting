namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class StateConverter : CollectionItemTypeConverter
	{
		public StateConverter()
		{
			base.simpleType = typeof(State);
		}
	}
}
