using AspNetCore.ReportingServices.OnDemandReportRendering;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class SelectiveRenderingCannotFindReportItemException : ReportRenderingException
	{
		internal SelectiveRenderingCannotFindReportItemException(string name)
			: base(HPBRes.ReportItemCannotBeFound(name), false)
		{
		}
	}
}
