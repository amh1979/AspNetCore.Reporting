namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class DataRowSortOwnerTraversalContext : ITraversalContext
	{
		private IDataRowSortOwner m_sortOwner;

		internal IDataRowSortOwner SortOwner
		{
			get
			{
				return this.m_sortOwner;
			}
		}

		internal DataRowSortOwnerTraversalContext(IDataRowSortOwner sortOwner)
		{
			this.m_sortOwner = sortOwner;
		}
	}
}
