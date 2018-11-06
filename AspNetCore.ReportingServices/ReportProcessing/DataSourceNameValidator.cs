namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class DataSourceNameValidator : NameValidator
	{
		internal bool Validate(ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool result = true;
			if (!base.IsUnique(objectName))
			{
				errorContext.Register(ProcessingErrorCode.rsDuplicateDataSourceName, Severity.Error, objectType, objectName, "Name");
				result = false;
			}
			return result;
		}
	}
}
