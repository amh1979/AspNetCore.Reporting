using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class StaticReferenceAttribute : Attribute
	{
	}
}
