namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal interface IShowHideContainer
	{
		void BeginProcessContainer(ReportProcessing.ProcessingContext context);

		void EndProcessContainer(ReportProcessing.ProcessingContext context);
	}
}
