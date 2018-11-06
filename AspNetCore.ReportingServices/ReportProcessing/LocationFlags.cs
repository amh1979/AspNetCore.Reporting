using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Flags]
	internal enum LocationFlags
	{
		None = 1,
		InDataSet = 2,
		InDataRegion = 4,
		InGrouping = 8,
		InDetail = 0x10,
		InMatrixCell = 0x20,
		InPageSection = 0x40,
		InMatrixSubtotal = 0x80,
		InMatrixCellTopLevelItem = 0x100,
		InMatrixOrTable = 0x200,
		InMatrixGroupHeader = 0x400
	}
}
