using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class DynamicImageObjectUniqueNameValidator : DynamicImageOrCustomUniqueNameValidator
	{
		internal DynamicImageObjectUniqueNameValidator()
		{
		}

		internal void Clear()
		{
			base.m_dictionary.Clear();
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
				errorContext.Register(ProcessingErrorCode.rsInvalidObjectNameNotUnique, severity, objectType, objectName, propertyName, propertyNameValue);
				result = false;
			}
			if (propertyNameValue != null && !NameValidator.IsCLSCompliant(propertyNameValue))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidObjectNameNotCLSCompliant, severity, objectType, objectName, propertyName, propertyNameValue);
				result = false;
			}
			return result;
		}
	}
}
