using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class DataCell
	{
		private CustomReportItem m_owner;

		private int m_rowIndex;

		private int m_columnIndex;

		private CustomReportItemCellInstance m_cellInstance;

		private DataValueCollection m_dataValueCollection;

		public DataValueCollection DataValues
		{
			get
			{
				if (this.m_cellInstance == null)
				{
					return null;
				}
				DataValueCollection dataValueCollection = this.m_dataValueCollection;
				if (this.m_dataValueCollection == null)
				{
					dataValueCollection = new DataValueCollection(this.GetCellDefinition(), this.m_cellInstance.DataValueInstances);
					if (this.m_owner.UseCache)
					{
						this.m_dataValueCollection = dataValueCollection;
					}
				}
				return dataValueCollection;
			}
		}

		internal int ColumnIndex
		{
			get
			{
				if (this.m_cellInstance == null)
				{
					return -1;
				}
				return this.m_cellInstance.ColumnIndex;
			}
		}

		internal int RowIndex
		{
			get
			{
				if (this.m_cellInstance == null)
				{
					return -1;
				}
				return this.m_cellInstance.RowIndex;
			}
		}

		internal DataCell(CustomReportItem owner, int rowIndex, int columnIndex)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = columnIndex;
			if (!owner.CustomData.NoRows)
			{
				CustomReportItemCellInstancesList cells = this.m_owner.CriInstance.Cells;
				this.m_cellInstance = cells[rowIndex][columnIndex];
			}
		}

		private DataValueCRIList GetCellDefinition()
		{
			Global.Tracer.Assert(!this.m_owner.CustomData.NoRows && this.m_owner.CriDefinition.DataRowCells != null && this.m_cellInstance.RowIndex < this.m_owner.CriDefinition.DataRowCells.Count && this.m_cellInstance.ColumnIndex < this.m_owner.CriDefinition.DataRowCells[this.m_cellInstance.RowIndex].Count && 0 < this.m_owner.CriDefinition.DataRowCells[this.m_cellInstance.RowIndex][this.m_cellInstance.ColumnIndex].Count);
			return this.m_owner.CriDefinition.DataRowCells[this.m_cellInstance.RowIndex][this.m_cellInstance.ColumnIndex];
		}
	}
}
