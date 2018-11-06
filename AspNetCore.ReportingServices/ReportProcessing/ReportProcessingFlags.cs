using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum ReportProcessingFlags
	{
		NotSet = 0,
		OnDemandEngine = 1,
		YukonEngine = 0x10,
		UpgradedYukonSnapshot = 2,
		YukonSnapshot = 0x20
	}
}
