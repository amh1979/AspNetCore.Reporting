namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ShapeDataBindingRuleConverter : CollectionItemTypeConverter
	{
		public ShapeDataBindingRuleConverter()
		{
			base.simpleType = typeof(ShapeDataBindingRule);
		}
	}
}
