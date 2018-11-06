using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	internal sealed class PersistedWithinRequestOnlyAttribute : Attribute
	{
	}
}
