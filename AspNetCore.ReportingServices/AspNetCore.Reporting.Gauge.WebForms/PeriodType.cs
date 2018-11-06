using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Serializable]
	internal enum PeriodType
	{
		Milliseconds,
		Seconds,
		Minutes,
		Hours,
		Days
	}
}
