using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixRow : TablixRow
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixRow m_rowDef;

		private ReportSize m_height;

		private TablixCell[] m_cellROMDefs;

		public override TablixCell this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCell tablixCell = this.m_rowDef.TablixCells[index];
					if (tablixCell.ColSpan > 0 && this.m_cellROMDefs[index] == null)
					{
						this.m_cellROMDefs[index] = new InternalTablixCell(base.m_owner, base.m_rowIndex, index, tablixCell);
					}
					return this.m_cellROMDefs[index];
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

		public override ReportSize Height
		{
			get
			{
				if (this.m_height == null)
				{
					this.m_height = new ReportSize(this.m_rowDef.Height, this.m_rowDef.HeightValue);
				}
				return this.m_height;
			}
		}

		internal InternalTablixRow(Tablix owner, int rowIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.TablixRow rowDef)
			: base(owner, rowIndex)
		{
			this.m_rowDef = rowDef;
			this.m_cellROMDefs = new TablixCell[rowDef.Cells.Count];
		}

		internal override IDataRegionCell GetIfExists(int index)
		{
			if (this.m_cellROMDefs != null && index >= 0 && index < this.Count)
			{
				return this.m_cellROMDefs[index];
			}
			return null;
		}
	}
}
