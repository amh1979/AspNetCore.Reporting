using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Persistence
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	internal sealed class HashtableOfReferencesAttribute : Attribute
	{
	}
}
