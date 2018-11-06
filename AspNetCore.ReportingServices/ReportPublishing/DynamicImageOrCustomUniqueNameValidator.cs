using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal abstract class DynamicImageOrCustomUniqueNameValidator : UniqueNameValidator
	{
		internal abstract bool Validate(Severity severity, string propertyName, ObjectType objectType, string objectName, string propertyNameValue, ErrorContext errorContext);
	}
}
