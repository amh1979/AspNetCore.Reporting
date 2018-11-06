namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal enum DataFieldStatus
	{
		None,
		Overflow,
		UnSupportedDataType,
		IsMissing = 4,
		IsError = 8
	}
}
