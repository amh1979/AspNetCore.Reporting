using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataRow : DataRow
	{
		private CustomDataRow m_rowDef;

		public override DataCell this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (base.m_cachedDataCells == null)
					{
						base.m_cachedDataCells = new DataCell[this.Count];
					}
					if (base.m_cachedDataCells[index] == null)
					{
						base.m_cachedDataCells[index] = new InternalDataCell(base.m_owner, base.m_rowIndex, index, this.m_rowDef.DataCells[index]);
					}
					return base.m_cachedDataCells[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_rowDef.Cells.Count;
			}
		}

		internal InternalDataRow(CustomReportItem owner, int rowIndex, CustomDataRow rowDef)
			: base(owner, rowIndex)
		{
			this.m_rowDef = rowDef;
		}
	}
}
