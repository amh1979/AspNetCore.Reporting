using System;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	[Flags]
	internal enum TextRunState : byte
	{
		Clear = 0,
		HasEastAsianChars = 1,
		FallbackFont = 2
	}
}
