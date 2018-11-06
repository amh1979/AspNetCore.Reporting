using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal struct FloatResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal double Value;
	}
}
