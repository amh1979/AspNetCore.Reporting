using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Persistence
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class ReferenceAttribute : Attribute
	{
	}
}
