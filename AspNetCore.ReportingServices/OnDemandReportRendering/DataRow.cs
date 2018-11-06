namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRow : ReportElementCollectionBase<DataCell>, IDataRegionRow
	{
		protected CustomReportItem m_owner;

		protected int m_rowIndex;

		protected DataCell[] m_cachedDataCells;

		internal DataRow(CustomReportItem owner, int rowIndex)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
		}

		IDataRegionCell IDataRegionRow.GetIfExists(int cellIndex)
		{
			if (this.m_cachedDataCells != null && cellIndex >= 0 && cellIndex < this.Count)
			{
				return this.m_cachedDataCells[cellIndex];
			}
			return null;
		}
	}
}
