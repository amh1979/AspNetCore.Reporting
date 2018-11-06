using System;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Flags]
	internal enum RenderingArea
	{
		All = 0,
		PageCreation = 1,
		KeepTogether = 2,
		RepeatOnNewPage = 4,
		RichText = 8
	}
}
