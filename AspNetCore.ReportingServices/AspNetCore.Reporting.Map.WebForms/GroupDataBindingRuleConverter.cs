namespace AspNetCore.Reporting.Map.WebForms
{
	internal class GroupDataBindingRuleConverter : CollectionItemTypeConverter
	{
		public GroupDataBindingRuleConverter()
		{
			base.simpleType = typeof(GroupDataBindingRule);
		}
	}
}
