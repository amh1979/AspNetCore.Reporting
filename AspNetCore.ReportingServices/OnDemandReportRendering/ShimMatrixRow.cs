using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixRow : TablixRow
	{
		private List<ShimMatrixCell> m_cells;

		private ReportSize m_height;

		public override TablixCell this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_cells[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_cells.Count;
			}
		}

		public override ReportSize Height
		{
			get
			{
				if (this.m_height == null)
				{
					int index = base.m_owner.MatrixRowDefinitionMapping[base.m_rowIndex];
					this.m_height = new ReportSize(base.m_owner.RenderMatrix.CellHeights[index]);
				}
				return this.m_height;
			}
		}

		internal ShimMatrixRow(Tablix owner, int rowIndex, ShimMatrixMember rowParentMember, bool inSubtotalRow)
			: base(owner, rowIndex)
		{
			this.m_cells = new List<ShimMatrixCell>();
			this.GenerateMatrixCells(rowParentMember, null, owner.ColumnHierarchy.MemberCollection as ShimMatrixMemberCollection, inSubtotalRow, inSubtotalRow);
		}

		private void GenerateMatrixCells(ShimMatrixMember rowParentMember, ShimMatrixMember colParentMember, ShimMatrixMemberCollection columnMembers, bool inSubtotalRow, bool inSubtotalColumn)
		{
			if (columnMembers == null)
			{
				this.m_cells.Add(new ShimMatrixCell(base.m_owner, base.m_rowIndex, this.m_cells.Count, rowParentMember, colParentMember, inSubtotalRow || inSubtotalColumn));
			}
			else
			{
				int count = columnMembers.Count;
				for (int i = 0; i < count; i++)
				{
					ShimMatrixMember shimMatrixMember = ((ReportElementCollectionBase<TablixMember>)columnMembers)[i] as ShimMatrixMember;
					this.GenerateMatrixCells(rowParentMember, shimMatrixMember, shimMatrixMember.Children as ShimMatrixMemberCollection, inSubtotalRow, inSubtotalColumn || shimMatrixMember.CurrentRenderMatrixMember.IsTotal);
				}
			}
		}
	}
}
