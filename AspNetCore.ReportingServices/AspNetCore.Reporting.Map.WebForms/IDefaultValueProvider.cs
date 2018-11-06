namespace AspNetCore.Reporting.Map.WebForms
{
	internal interface IDefaultValueProvider
	{
		object GetDefaultValue(string prop, object currentValue);
	}
}
