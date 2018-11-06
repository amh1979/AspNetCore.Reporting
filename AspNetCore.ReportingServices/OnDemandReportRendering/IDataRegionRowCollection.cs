namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal interface IDataRegionRowCollection
	{
		int Count
		{
			get;
		}

		IDataRegionRow GetIfExists(int index);
	}
}
