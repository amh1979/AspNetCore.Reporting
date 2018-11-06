using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal abstract class TableRowCollection
	{
		internal Table m_owner;

		internal TableRowList m_rowDefs;

		internal TableRowInstance[] m_rowInstances;

		internal TableRow[] m_rows;

		public virtual TableRow this[int index]
		{
			get
			{
				if (index >= 0 && index < this.m_rowDefs.Count)
				{
					TableRow tableRow = null;
					if (this.m_rows != null)
					{
						tableRow = this.m_rows[index];
					}
					if (tableRow == null)
					{
						TableRowInstance rowInstance = null;
						if (this.m_rowInstances != null && index < this.m_rowInstances.Length)
						{
							rowInstance = this.m_rowInstances[index];
						}
						else
						{
							Global.Tracer.Assert(null == this.m_rowInstances);
						}
						tableRow = new TableRow(this.m_owner, this.m_rowDefs[index], rowInstance);
						if (this.m_owner.RenderingContext.CacheState)
						{
							if (this.m_rows == null)
							{
								this.m_rows = new TableRow[this.m_rowDefs.Count];
							}
							this.m_rows[index] = tableRow;
						}
					}
					return tableRow;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.m_rowDefs.Count;
			}
		}

		internal TableRowList DetailRowDefinitions
		{
			get
			{
				return this.m_rowDefs;
			}
		}

		internal TableRowCollection(Table owner, TableRowList rowDefs, TableRowInstance[] rowInstances)
		{
			this.m_owner = owner;
			this.m_rowInstances = rowInstances;
			this.m_rowDefs = rowDefs;
		}
	}
}
