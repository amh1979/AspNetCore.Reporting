using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal struct BinaryResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal byte[] Value;
	}
}
