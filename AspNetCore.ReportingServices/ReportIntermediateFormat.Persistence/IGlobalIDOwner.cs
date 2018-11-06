namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal interface IGlobalIDOwner
	{
		int GlobalID
		{
			get;
			set;
		}
	}
}
