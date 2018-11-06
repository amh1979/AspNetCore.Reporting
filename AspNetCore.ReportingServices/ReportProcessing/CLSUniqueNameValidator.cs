namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class CLSUniqueNameValidator : NameValidator
	{
		private ProcessingErrorCode m_errorCodeNotCLS;

		private ProcessingErrorCode m_errorCodeNotUnique;

		internal CLSUniqueNameValidator(ProcessingErrorCode errorCodeNotCLS, ProcessingErrorCode errorCodeNotUnique)
		{
			this.m_errorCodeNotCLS = errorCodeNotCLS;
			this.m_errorCodeNotUnique = errorCodeNotUnique;
		}

		internal bool Validate(ObjectType objectType, string name, ErrorContext errorContext)
		{
			bool result = true;
			if (!NameValidator.IsCLSCompliant(name))
			{
				errorContext.Register(this.m_errorCodeNotCLS, Severity.Error, objectType, name, "Name");
				result = false;
			}
			if (!base.IsUnique(name))
			{
				errorContext.Register(this.m_errorCodeNotUnique, Severity.Error, objectType, name, "Name");
				result = false;
			}
			return result;
		}

		internal bool Validate(string name, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool result = true;
			if (!NameValidator.IsCLSCompliant(name))
			{
				errorContext.Register(this.m_errorCodeNotCLS, Severity.Error, objectType, objectName, "Name", name);
				result = false;
			}
			if (!base.IsUnique(name))
			{
				errorContext.Register(this.m_errorCodeNotUnique, Severity.Error, objectType, objectName, "Name", name);
				result = false;
			}
			return result;
		}

		internal bool Validate(string name, string dataField, string dataSetName, ErrorContext errorContext)
		{
			bool result = true;
			if (!NameValidator.IsCLSCompliant(name))
			{
				errorContext.Register(this.m_errorCodeNotCLS, Severity.Error, ObjectType.Field, dataField, "Name", name, dataSetName);
				result = false;
			}
			if (!base.IsUnique(name))
			{
				errorContext.Register(this.m_errorCodeNotUnique, Severity.Error, ObjectType.Field, dataField, "Name", name, dataSetName);
				result = false;
			}
			return result;
		}
	}
}
