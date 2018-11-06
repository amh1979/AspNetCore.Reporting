namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal interface IPageBreakItem
	{
		bool HasPageBreaks(bool atStart);

		bool IgnorePageBreaks();
	}
}
