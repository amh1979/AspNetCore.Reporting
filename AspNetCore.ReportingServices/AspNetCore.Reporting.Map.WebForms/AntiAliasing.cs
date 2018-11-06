using System;

namespace AspNetCore.Reporting.Map.WebForms
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
