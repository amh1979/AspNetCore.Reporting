using System.Diagnostics;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	internal sealed class RenderingDiagnostics
	{
		public static bool Enabled
		{
			get
			{
				return RSTrace.RenderingTracer.TraceVerbose;
			}
		}

		public static void Trace(RenderingArea renderingArea, TraceLevel traceLevel, string message)
		{
			if (RenderingDiagnostics.Enabled)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, message);
			}
		}

		public static void Trace(RenderingArea renderingArea, TraceLevel traceLevel, string format, params object[] arg)
		{
			if (RenderingDiagnostics.Enabled)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, format, arg);
			}
		}
	}
}
