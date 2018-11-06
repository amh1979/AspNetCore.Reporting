using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Serializable]
	internal enum DurationType
	{
		Infinite,
		Milliseconds,
		Seconds,
		Minutes,
		Hours,
		Days,
		Count
	}
}
