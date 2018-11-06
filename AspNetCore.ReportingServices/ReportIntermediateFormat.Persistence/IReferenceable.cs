namespace AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal interface IReferenceable
	{
		int ID
		{
			get;
		}

		ObjectType GetObjectType();
	}
}
