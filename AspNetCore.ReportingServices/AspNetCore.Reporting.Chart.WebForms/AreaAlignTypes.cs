using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[Flags]
	internal enum AreaAlignTypes
	{
		None = 0,
		Position = 1,
		PlotPosition = 2,
		Cursor = 4,
		AxesView = 8,
		All = 0xF
	}
}
