namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class KnobConverter : CollectionItemTypeConverter
	{
		public KnobConverter()
		{
			base.simpleType = typeof(Knob);
		}
	}
}
