using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableCell : ShimCell
	{
		private int m_colSpan;

		private TableCell m_renderCellContents;

		private AspNetCore.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		public override CellContents CellContents
		{
			get
			{
				if (base.m_cellContents == null)
				{
					base.m_cellContents = new CellContents(this, base.m_inSubtotal, this.m_renderReportItem, 1, this.m_colSpan, base.m_owner.RenderingContext);
				}
				return base.m_cellContents;
			}
		}

		internal ShimTableCell(Tablix owner, int rowIndex, int colIndex, int colSpan, AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
			: base(owner, rowIndex, colIndex, owner.InSubtotal)
		{
			this.m_colSpan = colSpan;
			this.m_renderReportItem = renderReportItem;
		}

		internal void SetCellContents(TableCell renderCellContents)
		{
			if (renderCellContents != null)
			{
				this.m_renderCellContents = renderCellContents;
				if (renderCellContents.ReportItem != null)
				{
					this.m_renderReportItem = renderCellContents.ReportItem;
				}
			}
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (base.m_cellContents != null)
			{
				base.m_cellContents.UpdateRenderReportItem((renderCellContents != null) ? renderCellContents.ReportItem : null);
			}
		}
	}
}
