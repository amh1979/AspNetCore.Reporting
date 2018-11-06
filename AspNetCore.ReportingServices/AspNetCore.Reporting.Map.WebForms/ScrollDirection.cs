using System;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Flags]
	internal enum ScrollDirection
	{
		None = 0,
		North = 1,
		South = 2,
		West = 4,
		East = 8
	}
}
