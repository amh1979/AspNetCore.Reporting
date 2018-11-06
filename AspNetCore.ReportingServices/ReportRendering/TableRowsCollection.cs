using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableRowsCollection
	{
		private Table m_owner;

		private TableDetail m_detailDef;

		private TableDetailInstanceList m_detailInstances;

		private TableDetailRowCollection[] m_rows;

		private TableDetailRowCollection m_firstRows;

		public TableDetailRowCollection this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					TableDetailRowCollection tableDetailRowCollection = null;
					if (index == 0)
					{
						tableDetailRowCollection = this.m_firstRows;
					}
					else if (this.m_rows != null)
					{
						tableDetailRowCollection = this.m_rows[index - 1];
					}
					if (tableDetailRowCollection == null)
					{
						TableRowInstance[] rowInstances = null;
						TableDetailInstance tableDetailInstance = null;
						if (this.m_detailInstances != null && index < this.m_detailInstances.Count)
						{
							tableDetailInstance = this.m_detailInstances[index];
							rowInstances = tableDetailInstance.DetailRowInstances;
						}
						else
						{
							Global.Tracer.Assert(this.m_detailInstances == null || 0 == this.m_detailInstances.Count);
						}
						tableDetailRowCollection = new TableDetailRowCollection(this.m_owner, this.m_detailDef.DetailRows, rowInstances, tableDetailInstance);
						if (this.m_owner.RenderingContext.CacheState)
						{
							if (index == 0)
							{
								this.m_firstRows = tableDetailRowCollection;
							}
							else
							{
								if (this.m_rows == null)
								{
									this.m_rows = new TableDetailRowCollection[this.m_detailInstances.Count - 1];
								}
								this.m_rows[index - 1] = tableDetailRowCollection;
							}
						}
					}
					return tableDetailRowCollection;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				if (this.m_detailInstances != null && this.m_detailInstances.Count != 0)
				{
					return this.m_detailInstances.Count;
				}
				return 1;
			}
		}

		internal TableDetail DetailDefinition
		{
			get
			{
				return this.m_detailDef;
			}
		}

		internal TableRowsCollection(Table owner, TableDetail detailDef, TableDetailInstanceList detailInstances)
		{
			this.m_owner = owner;
			this.m_detailInstances = detailInstances;
			this.m_detailDef = detailDef;
		}
	}
}
