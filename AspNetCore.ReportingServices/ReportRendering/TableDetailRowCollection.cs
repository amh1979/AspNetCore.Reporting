using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableDetailRowCollection : TableRowCollection
	{
		private TableDetailInstance m_detailInstance;

		private TableDetailInstanceInfo m_detailInstanceInfo;

		public override TableRow this[int index]
		{
			get
			{
				if (index >= 0 && index < base.m_rowDefs.Count)
				{
					TableDetailRow tableDetailRow = null;
					if (base.m_rows != null)
					{
						tableDetailRow = (TableDetailRow)base.m_rows[index];
					}
					if (tableDetailRow == null)
					{
						TableRowInstance rowInstance = null;
						if (base.m_rowInstances != null && index < base.m_rowInstances.Length)
						{
							rowInstance = base.m_rowInstances[index];
						}
						else
						{
							Global.Tracer.Assert(null == base.m_rowInstances);
						}
						tableDetailRow = new TableDetailRow(base.m_owner, base.m_rowDefs[index], rowInstance, this);
						if (base.m_owner.RenderingContext.CacheState)
						{
							if (base.m_rows == null)
							{
								base.m_rows = new TableDetailRow[base.m_rowDefs.Count];
							}
							base.m_rows[index] = tableDetailRow;
						}
					}
					return tableDetailRow;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, base.Count);
			}
		}

		internal TableDetailInstance DetailInstance
		{
			get
			{
				return this.m_detailInstance;
			}
		}

		internal TableDetailInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_detailInstance == null)
				{
					return null;
				}
				if (this.m_detailInstanceInfo == null)
				{
					this.m_detailInstanceInfo = this.m_detailInstance.GetInstanceInfo(base.m_owner.RenderingContext.ChunkManager);
				}
				return this.m_detailInstanceInfo;
			}
		}

		internal bool Hidden
		{
			get
			{
				bool result = false;
				TableDetail tableDetail = ((AspNetCore.ReportingServices.ReportProcessing.Table)base.m_owner.ReportItemDef).TableDetail;
				if (this.DetailInstance == null)
				{
					result = RenderingContext.GetDefinitionHidden(tableDetail.Visibility);
				}
				else if (tableDetail.Visibility != null)
				{
					result = ((tableDetail.Visibility.Toggle == null) ? this.InstanceInfo.StartHidden : base.m_owner.RenderingContext.IsItemHidden(this.DetailInstance.UniqueName, false));
				}
				return result;
			}
		}

		internal TableDetailRowCollection(Table owner, TableRowList rowDefs, TableRowInstance[] rowInstances, TableDetailInstance detailInstance)
			: base(owner, rowDefs, rowInstances)
		{
			this.m_detailInstance = detailInstance;
		}
	}
}
