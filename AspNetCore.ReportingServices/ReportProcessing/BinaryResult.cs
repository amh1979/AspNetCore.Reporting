namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal struct BinaryResult
	{
		internal bool ErrorOccurred;

		internal DataFieldStatus FieldStatus;

		internal byte[] Value;
	}
}
