using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal struct StringResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal string Value;
	}
}
