namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal interface IDataRegion
	{
		bool HasDataCells
		{
			get;
		}

		IDataRegionRowCollection RowCollection
		{
			get;
		}
	}
}
