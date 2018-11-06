using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal struct ParameterValueResult
	{
		internal bool ErrorOccurred;

		internal object Value;

		internal DataType Type;
	}
}
