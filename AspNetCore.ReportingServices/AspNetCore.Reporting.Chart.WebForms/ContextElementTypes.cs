using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[Flags]
	internal enum ContextElementTypes
	{
		None = 0,
		ChartArea = 1,
		Series = 2,
		Axis = 8,
		Title = 0x10,
		Annotation = 0x20,
		Legend = 0x40,
		AxisLabel = 0x80,
		Any = 0xFB
	}
}
