using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableColumnCollection
	{
		private Table m_owner;

		private TableColumnList m_columnDefs;

		private TableColumn[] m_columns;

		public TableColumn this[int index]
		{
			get
			{
				if (index >= 0 && index < this.m_columnDefs.Count)
				{
					TableColumn tableColumn = null;
					if (this.m_columns == null || this.m_columns[index] == null)
					{
						tableColumn = new TableColumn(this.m_owner, this.m_columnDefs[index], index);
						if (this.m_owner.RenderingContext.CacheState)
						{
							if (this.m_columns == null)
							{
								this.m_columns = new TableColumn[this.m_columnDefs.Count];
							}
							this.m_columns[index] = tableColumn;
						}
					}
					else
					{
						tableColumn = this.m_columns[index];
					}
					return tableColumn;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.m_columnDefs.Count;
			}
		}

		internal TableColumnCollection(Table owner, TableColumnList columnDefs)
		{
			this.m_owner = owner;
			this.m_columnDefs = columnDefs;
		}
	}
}
