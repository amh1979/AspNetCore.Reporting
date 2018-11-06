using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class CLSNameValidator : NameValidator
	{
		internal CLSNameValidator()
			: base(false)
		{
		}

		internal static bool ValidateDataElementName(ref string elementName, string defaultName, ObjectType objectType, string objectName, string propertyName, ErrorContext errorContext)
		{
			Global.Tracer.Assert(null != defaultName);
			if (elementName == null)
			{
				elementName = defaultName;
			}
			else if (!NameValidator.IsCLSCompliant(elementName))
			{
				errorContext.Register(ProcessingErrorCode.rsInvalidDataElementNameNotCLSCompliant, Severity.Error, objectType, objectName, null, propertyName, elementName);
				return false;
			}
			return true;
		}
	}
}
