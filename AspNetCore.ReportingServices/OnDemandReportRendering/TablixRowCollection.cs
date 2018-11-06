namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class TablixRowCollection : ReportElementCollectionBase<TablixRow>, IDataRegionRowCollection
	{
		protected Tablix m_owner;

		internal TablixRowCollection(Tablix owner)
		{
			this.m_owner = owner;
		}

		IDataRegionRow IDataRegionRowCollection.GetIfExists(int index)
		{
			if (index >= 0 && index < this.Count)
			{
				return ((ReportElementCollectionBase<TablixRow>)this)[index];
			}
			return null;
		}
	}
}
