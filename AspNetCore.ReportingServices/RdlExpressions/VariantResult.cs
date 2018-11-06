using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal struct VariantResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal string ExceptionMessage;

		internal object Value;

		internal TypeCode TypeCode;

		internal VariantResult(bool errorOccurred, object v)
		{
			this.ErrorOccurred = errorOccurred;
			this.Value = v;
			this.FieldStatus = DataFieldStatus.None;
			this.ExceptionMessage = null;
			this.TypeCode = TypeCode.Empty;
		}
	}
}
