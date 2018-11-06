namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal interface IDataCorrelation
	{
		bool NextCorrelatedRow();
	}
}
