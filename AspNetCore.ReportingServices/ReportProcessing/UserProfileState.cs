using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum UserProfileState
	{
		None = 0,
		InQuery = 1,
		InReport = 2,
		Both = 3,
		OnDemandExpressions = 8
	}
}
