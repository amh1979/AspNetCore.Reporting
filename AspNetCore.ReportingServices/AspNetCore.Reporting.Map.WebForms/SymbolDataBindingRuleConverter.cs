namespace AspNetCore.Reporting.Map.WebForms
{
	internal class SymbolDataBindingRuleConverter : CollectionItemTypeConverter
	{
		public SymbolDataBindingRuleConverter()
		{
			base.simpleType = typeof(SymbolDataBindingRule);
		}
	}
}
