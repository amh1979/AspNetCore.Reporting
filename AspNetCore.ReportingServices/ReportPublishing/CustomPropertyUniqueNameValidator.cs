using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class CustomPropertyUniqueNameValidator : DynamicImageOrCustomUniqueNameValidator
	{
		internal CustomPropertyUniqueNameValidator()
		{
		}

		internal override bool Validate(Severity severity, ObjectType objectType, string objectName, string propertyNameValue, ErrorContext errorContext)
		{
			Global.Tracer.Assert(false);
			return this.Validate(severity, "", objectType, objectName, propertyNameValue, errorContext);
		}

		internal override bool Validate(Severity severity, string propertyName, ObjectType objectType, string objectName, string propertyNameValue, ErrorContext errorContext)
		{
			bool result = true;
			if (propertyNameValue == null || !base.IsUnique(propertyNameValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidCustomPropertyName, severity, objectType, objectName, propertyNameValue);
				result = false;
			}
			return result;
		}
	}
}
