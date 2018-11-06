using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixCell : ShimCell
	{
		private AspNetCore.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		private ShimMatrixMember m_rowParentMember;

		private ShimMatrixMember m_colParentMember;

		public override CellContents CellContents
		{
			get
			{
				if (base.m_cellContents == null)
				{
					base.m_cellContents = new CellContents(this, base.m_inSubtotal, this.CachedRenderReportItem, 1, 1, base.m_owner.RenderingContext);
				}
				else if (this.m_renderReportItem == null)
				{
					base.m_cellContents.UpdateRenderReportItem(this.CachedRenderReportItem);
				}
				return base.m_cellContents;
			}
		}

		private AspNetCore.ReportingServices.ReportRendering.ReportItem CachedRenderReportItem
		{
			get
			{
				if (this.m_renderReportItem == null)
				{
					int cachedMemberCellIndex = this.m_rowParentMember.CurrentRenderMatrixMember.CachedMemberCellIndex;
					int cellIndex = this.m_colParentMember.CurrentMatrixMemberCellIndexes.GetCellIndex(this.m_colParentMember);
					MatrixCell matrixCell = base.m_owner.RenderMatrix.CellCollection[cachedMemberCellIndex, cellIndex];
					if (matrixCell != null)
					{
						this.m_renderReportItem = matrixCell.ReportItem;
					}
				}
				return this.m_renderReportItem;
			}
		}

		internal ShimMatrixCell(Tablix owner, int rowIndex, int colIndex, ShimMatrixMember rowParentMember, ShimMatrixMember colParentMember, bool inSubtotal)
			: base(owner, rowIndex, colIndex, inSubtotal)
		{
			this.m_rowParentMember = rowParentMember;
			this.m_colParentMember = colParentMember;
		}

		internal void ResetCellContents()
		{
			this.m_renderReportItem = null;
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
