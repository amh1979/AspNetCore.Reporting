using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[Flags]
	internal enum LabelAlignmentTypes
	{
		Top = 1,
		Bottom = 2,
		Right = 4,
		Left = 8,
		TopLeft = 0x10,
		TopRight = 0x20,
		BottomLeft = 0x40,
		BottomRight = 0x80,
		Center = 0x100
	}
}
