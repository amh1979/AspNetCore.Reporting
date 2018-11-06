using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[Flags]
	internal enum ProcessMode
	{
		Paint = 1,
		HotRegions = 2,
		ImageMaps = 4
	}
}
