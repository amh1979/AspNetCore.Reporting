namespace AspNetCore.Reporting.Map.WebForms
{
	internal class PathDataBindingRuleConverter : CollectionItemTypeConverter
	{
		public PathDataBindingRuleConverter()
		{
			base.simpleType = typeof(PathDataBindingRule);
		}
	}
}
