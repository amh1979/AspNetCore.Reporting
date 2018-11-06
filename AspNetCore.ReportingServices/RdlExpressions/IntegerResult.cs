using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal struct IntegerResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal int Value;
	}
}
