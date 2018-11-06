using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimMatrixRowCollection : TablixRowCollection
	{
		private List<ShimMatrixRow> m_rows;

		public override TablixRow this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_rows[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_rows.Count;
			}
		}

		internal ShimMatrixRowCollection(Tablix owner)
			: base(owner)
		{
			this.m_rows = new List<ShimMatrixRow>();
			this.AppendMatrixRows(null, owner.RowHierarchy.MemberCollection as ShimMatrixMemberCollection, owner.InSubtotal);
		}

		private void AppendMatrixRows(ShimMatrixMember rowParentMember, ShimMatrixMemberCollection rowMembers, bool inSubtotalRow)
		{
			if (rowMembers == null)
			{
				this.m_rows.Add(new ShimMatrixRow(base.m_owner, this.m_rows.Count, rowParentMember, inSubtotalRow));
			}
			else
			{
				int count = rowMembers.Count;
				for (int i = 0; i < count; i++)
				{
					ShimMatrixMember shimMatrixMember = ((ReportElementCollectionBase<TablixMember>)rowMembers)[i] as ShimMatrixMember;
					this.AppendMatrixRows(shimMatrixMember, shimMatrixMember.Children as ShimMatrixMemberCollection, inSubtotalRow || shimMatrixMember.CurrentRenderMatrixMember.IsTotal);
				}
			}
		}

		internal void UpdateCells(ShimMatrixMember innermostMember)
		{
			if (innermostMember != null && innermostMember.Children == null)
			{
				if (!innermostMember.IsColumn)
				{
					int memberCellIndex = innermostMember.MemberCellIndex;
					int count = this.m_rows[memberCellIndex].Count;
					for (int i = 0; i < count; i++)
					{
						((ShimMatrixCell)((ReportElementCollectionBase<TablixCell>)this.m_rows[memberCellIndex])[i]).ResetCellContents();
					}
				}
				else
				{
					int memberCellIndex2 = innermostMember.MemberCellIndex;
					int count2 = this.m_rows.Count;
					for (int j = 0; j < count2; j++)
					{
						((ShimMatrixCell)((ReportElementCollectionBase<TablixCell>)this.m_rows[j])[memberCellIndex2]).ResetCellContents();
					}
				}
			}
		}
	}
}
