using System.Diagnostics;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	internal interface IRSTraceInternalWithDynamicLevel : IRSTraceInternal
	{
		void SetTraceLevel(TraceLevel traceLevel);
	}
}
