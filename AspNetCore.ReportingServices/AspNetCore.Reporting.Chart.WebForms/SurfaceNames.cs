using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[Flags]
	internal enum SurfaceNames
	{
		Front = 1,
		Back = 2,
		Left = 4,
		Right = 8,
		Top = 0x10,
		Bottom = 0x20
	}
}
