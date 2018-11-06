using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class CLSUniqueNameValidator : NameValidator
	{
		private ProcessingErrorCode m_errorCodeNotCLS;

		private ProcessingErrorCode m_errorCodeNotUnique;

		private ProcessingErrorCode m_errorCodeNameLength;

		private ProcessingErrorCode m_errorCodeCaseInsensitiveDuplicate;

		internal CLSUniqueNameValidator(ProcessingErrorCode errorCodeNotCLS, ProcessingErrorCode errorCodeNotUnique, ProcessingErrorCode errorCodeNameLength)
			: base(false)
		{
			this.m_errorCodeNotCLS = errorCodeNotCLS;
			this.m_errorCodeNotUnique = errorCodeNotUnique;
			this.m_errorCodeNameLength = errorCodeNameLength;
			this.m_errorCodeCaseInsensitiveDuplicate = ProcessingErrorCode.rsNone;
		}

		internal CLSUniqueNameValidator(ProcessingErrorCode errorCodeNotCLS, ProcessingErrorCode errorCodeNotUnique, ProcessingErrorCode errorCodeNameLength, ProcessingErrorCode errorCodeCaseInsensitiveDuplicate)
			: base(true)
		{
			this.m_errorCodeNotCLS = errorCodeNotCLS;
			this.m_errorCodeNotUnique = errorCodeNotUnique;
			this.m_errorCodeNameLength = errorCodeNameLength;
			this.m_errorCodeCaseInsensitiveDuplicate = errorCodeCaseInsensitiveDuplicate;
		}

		internal bool Validate(ObjectType objectType, string name, ErrorContext errorContext)
		{
			bool result = true;
			if (string.IsNullOrEmpty(name) || name.Length > 256)
			{
				errorContext.Register(this.m_errorCodeNameLength, Severity.Error, objectType, name, "Name", "256");
				result = false;
			}
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
			if (base.IsCaseInsensitiveDuplicate(name))
			{
				errorContext.Register(this.m_errorCodeCaseInsensitiveDuplicate, Severity.Warning, objectType, name, "Name");
			}
			return result;
		}

		internal bool Validate(string name, ObjectType objectType, string objectName, ErrorContext errorContext)
		{
			bool result = true;
			if (string.IsNullOrEmpty(name) || name.Length > 256)
			{
				errorContext.Register(this.m_errorCodeNameLength, Severity.Error, objectType, objectName, "Name", "256");
				result = false;
			}
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
			if (string.IsNullOrEmpty(name) || name.Length > 256)
			{
				errorContext.Register(this.m_errorCodeNameLength, Severity.Error, ObjectType.Field, dataField, "Name", name, dataSetName, "256");
				result = false;
			}
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
