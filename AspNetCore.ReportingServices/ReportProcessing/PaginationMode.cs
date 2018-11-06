using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum PaginationMode
	{
		Progressive = 0,
		TotalPages = 1,
		Estimate = 2
	}
}
