using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class DataCellCollection
	{
		private CustomReportItem m_owner;

		private int m_columnsCount;

		private int m_rowsCount;

		private DataCell m_firstCell;

		private DataRowCells m_firstColumnCells;

		private DataRowCells m_firstRowCells;

		private DataRowCells[] m_cells;

		public DataCell this[int row, int column]
		{
			get
			{
				if (row >= 0 && row < this.m_rowsCount)
				{
					if (column >= 0 && column < this.m_columnsCount)
					{
						DataCell dataCell = null;
						if (row == 0 && column == 0)
						{
							dataCell = this.m_firstCell;
						}
						else if (row == 0)
						{
							if (this.m_firstRowCells != null)
							{
								dataCell = this.m_firstRowCells[column - 1];
							}
						}
						else if (column == 0)
						{
							if (this.m_firstColumnCells != null)
							{
								dataCell = this.m_firstColumnCells[row - 1];
							}
						}
						else if (this.m_cells != null && this.m_cells[row - 1] != null)
						{
							dataCell = this.m_cells[row - 1][column - 1];
						}
						if (dataCell == null)
						{
							dataCell = new DataCell(this.m_owner, row, column);
							if (this.m_owner.UseCache)
							{
								if (row == 0 && column == 0)
								{
									this.m_firstCell = dataCell;
								}
								else if (row == 0)
								{
									if (this.m_firstRowCells == null)
									{
										this.m_firstRowCells = new DataRowCells(this.m_columnsCount - 1);
									}
									this.m_firstRowCells[column - 1] = dataCell;
								}
								else if (column == 0)
								{
									if (this.m_firstColumnCells == null)
									{
										this.m_firstColumnCells = new DataRowCells(this.m_rowsCount - 1);
									}
									this.m_firstColumnCells[row - 1] = dataCell;
								}
								else
								{
									if (this.m_cells == null)
									{
										this.m_cells = new DataRowCells[this.m_rowsCount - 1];
									}
									if (this.m_cells[row - 1] == null)
									{
										this.m_cells[row - 1] = new DataRowCells(this.m_columnsCount - 1);
									}
									this.m_cells[row - 1][column - 1] = dataCell;
								}
							}
						}
						return dataCell;
					}
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, column, 0, this.m_columnsCount);
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, row, 0, this.m_rowsCount);
			}
		}

		public int Count
		{
			get
			{
				return this.m_rowsCount * this.m_columnsCount;
			}
		}

		public int RowCount
		{
			get
			{
				return this.m_rowsCount;
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.m_columnsCount;
			}
		}

		internal DataCellCollection(CustomReportItem owner, int rowsCount, int columnsCount)
		{
			this.m_owner = owner;
			this.m_rowsCount = rowsCount;
			this.m_columnsCount = columnsCount;
		}
	}
}
