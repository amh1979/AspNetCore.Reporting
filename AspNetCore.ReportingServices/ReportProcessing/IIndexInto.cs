namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal interface IIndexInto
	{
		object GetChildAt(int index, out NonComputedUniqueNames nonCompNames);
	}
}
