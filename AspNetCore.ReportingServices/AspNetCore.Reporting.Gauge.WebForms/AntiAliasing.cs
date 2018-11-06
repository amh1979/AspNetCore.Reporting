using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Flags]
	internal enum AntiAliasing
	{
		None = 0,
		Text = 1,
		Graphics = 2,
		All = 3
	}
}
