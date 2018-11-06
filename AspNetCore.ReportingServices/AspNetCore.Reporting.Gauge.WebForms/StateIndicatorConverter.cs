namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class StateIndicatorConverter : CollectionItemTypeConverter
	{
		public StateIndicatorConverter()
		{
			base.simpleType = typeof(StateIndicator);
		}
	}
}
