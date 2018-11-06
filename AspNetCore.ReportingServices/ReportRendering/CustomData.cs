namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class CustomData
	{
		private CustomReportItem m_owner;

		private DataCellCollection m_datacells;

		private DataGroupingCollection m_columns;

		private DataGroupingCollection m_rows;

		public bool NoRows
		{
			get
			{
				if (this.m_owner.CriInstance != null && this.m_owner.CriInstance.Cells != null && this.m_owner.CriInstance.Cells.Count != 0 && this.m_owner.CriInstance.Cells[0].Count != 0)
				{
					return false;
				}
				return true;
			}
		}

		public DataCellCollection DataCells
		{
			get
			{
				DataCellCollection dataCellCollection = this.m_datacells;
				if (this.m_datacells == null)
				{
					if (!this.NoRows)
					{
						dataCellCollection = new DataCellCollection(this.m_owner, this.m_owner.CriInstance.CellRowCount, this.m_owner.CriInstance.CellColumnCount);
					}
					if (this.m_owner.UseCache)
					{
						this.m_datacells = dataCellCollection;
					}
				}
				return dataCellCollection;
			}
		}

		public DataGroupingCollection DataColumnGroupings
		{
			get
			{
				DataGroupingCollection dataGroupingCollection = this.m_columns;
				if (this.m_columns == null && this.m_owner.CriDefinition.Columns != null)
				{
					dataGroupingCollection = new DataGroupingCollection(this.m_owner, null, this.m_owner.CriDefinition.Columns, (this.m_owner.CriInstance == null) ? null : this.m_owner.CriInstance.ColumnInstances);
					if (this.m_owner.UseCache)
					{
						this.m_columns = dataGroupingCollection;
					}
				}
				return dataGroupingCollection;
			}
		}

		public DataGroupingCollection DataRowGroupings
		{
			get
			{
				DataGroupingCollection dataGroupingCollection = this.m_rows;
				if (this.m_rows == null && this.m_owner.CriDefinition.Rows != null)
				{
					dataGroupingCollection = new DataGroupingCollection(this.m_owner, null, this.m_owner.CriDefinition.Rows, (this.m_owner.CriInstance == null) ? null : this.m_owner.CriInstance.RowInstances);
					if (this.m_owner.UseCache)
					{
						this.m_rows = dataGroupingCollection;
					}
				}
				return dataGroupingCollection;
			}
		}

		internal CustomData(CustomReportItem owner)
		{
			this.m_owner = owner;
		}
	}
}
