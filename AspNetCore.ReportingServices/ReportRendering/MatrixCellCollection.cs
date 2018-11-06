using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class MatrixCellCollection
	{
		private Matrix m_owner;

		private int m_columnsCount;

		private int m_rowsCount;

		private MatrixCell m_firstCell;

		private MatrixRowCells m_firstMatrixColumnCells;

		private MatrixRowCells m_firstMatrixRowCells;

		private MatrixRowCells[] m_cells;

		public MatrixCell this[int row, int column]
		{
			get
			{
				if (row >= 0 && row < this.m_rowsCount)
				{
					if (column >= 0 && column < this.m_columnsCount)
					{
						MatrixCell matrixCell = null;
						if (row == 0 && column == 0)
						{
							matrixCell = this.m_firstCell;
						}
						else if (row == 0)
						{
							if (this.m_firstMatrixRowCells != null)
							{
								matrixCell = this.m_firstMatrixRowCells[column - 1];
							}
						}
						else if (column == 0)
						{
							if (this.m_firstMatrixColumnCells != null)
							{
								matrixCell = this.m_firstMatrixColumnCells[row - 1];
							}
						}
						else if (this.m_cells != null && this.m_cells[row - 1] != null)
						{
							matrixCell = this.m_cells[row - 1][column - 1];
						}
						if (matrixCell == null)
						{
							matrixCell = new MatrixCell(this.m_owner, row, column);
							if (this.m_owner.RenderingContext.CacheState)
							{
								if (row == 0 && column == 0)
								{
									this.m_firstCell = matrixCell;
								}
								else if (row == 0)
								{
									if (this.m_firstMatrixRowCells == null)
									{
										this.m_firstMatrixRowCells = new MatrixRowCells(this.m_columnsCount - 1);
									}
									this.m_firstMatrixRowCells[column - 1] = matrixCell;
								}
								else if (column == 0)
								{
									if (this.m_firstMatrixColumnCells == null)
									{
										this.m_firstMatrixColumnCells = new MatrixRowCells(this.m_rowsCount - 1);
									}
									this.m_firstMatrixColumnCells[row - 1] = matrixCell;
								}
								else
								{
									if (this.m_cells == null)
									{
										this.m_cells = new MatrixRowCells[this.m_rowsCount - 1];
									}
									if (this.m_cells[row - 1] == null)
									{
										this.m_cells[row - 1] = new MatrixRowCells(this.m_columnsCount - 1);
									}
									this.m_cells[row - 1][column - 1] = matrixCell;
								}
							}
						}
						return matrixCell;
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

		internal MatrixCellCollection(Matrix owner, int rowsCount, int columnsCount)
		{
			this.m_owner = owner;
			this.m_rowsCount = rowsCount;
			this.m_columnsCount = columnsCount;
		}
	}
}
