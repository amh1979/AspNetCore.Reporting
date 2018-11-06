using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum UserLocationFlags
	{
		None = 1,
		ReportBody = 2,
		ReportPageSection = 4,
		ReportQueries = 8
	}
}
