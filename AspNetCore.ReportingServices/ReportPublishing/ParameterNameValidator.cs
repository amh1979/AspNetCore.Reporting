using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class ParameterNameValidator : NameValidator
	{
		internal ParameterNameValidator()
			: base(false)
		{
		}

		internal bool Validate(string parameterName, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool result = true;
			if (string.IsNullOrEmpty(parameterName) || parameterName.Length > 256)
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidParameterNameLength, Severity.Error, objectType, objectName, "Name", parameterName, "256");
				result = false;
			}
			if (!NameValidator.IsCLSCompliant(parameterName))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidParameterNameNotCLSCompliant, Severity.Error, objectType, objectName, "Name", parameterName);
				result = false;
			}
			return result;
		}
	}
}
