using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[Flags]
	internal enum DataActions
	{
		None = 0,
		RecursiveAggregates = 1,
		PostSortAggregates = 2,
		UserSort = 4,
		AggregatesOfAggregates = 8,
		PostSortAggregatesOfAggregates = 0x10
	}
}
