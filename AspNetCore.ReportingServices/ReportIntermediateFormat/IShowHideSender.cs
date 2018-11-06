using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IShowHideSender
	{
		void ProcessSender(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext context, int uniqueName);
	}
}
