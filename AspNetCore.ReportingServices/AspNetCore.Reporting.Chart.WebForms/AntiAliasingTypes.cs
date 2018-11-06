using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[Flags]
	internal enum AntiAliasingTypes
	{
		None = 0,
		Text = 1,
		Graphics = 2,
		All = 3
	}
}
