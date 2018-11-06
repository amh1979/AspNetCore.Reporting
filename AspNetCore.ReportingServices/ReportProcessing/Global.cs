using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Reflection;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class Global
	{
		internal static readonly string ReportProcessingNamespace = "AspNetCore.ReportingServices.ReportProcessing";

		internal static RSTrace Tracer = RSTrace.ProcessingTracer;

		internal static RSTrace RenderingTracer = RSTrace.RenderingTracer;

		internal static string ReportProcessingLocation = Assembly.GetExecutingAssembly().Location;

		private Global()
		{
		}
	}
}
