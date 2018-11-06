namespace AspNetCore.ReportingServices.Diagnostics
{
	internal enum ProcessingStatus
	{
		Success,
		CanceledByUser,
		AbnormalTermination,
		TimeoutExpired
	}
}
