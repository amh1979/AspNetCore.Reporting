namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IIndexedInCollection
	{
		int IndexInCollection
		{
			get;
			set;
		}

		IndexedInCollectionType IndexedInCollectionType
		{
			get;
		}
	}
}
