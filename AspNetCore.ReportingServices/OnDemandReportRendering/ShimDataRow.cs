using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataRow : DataRow
	{
		private List<ShimDataCell> m_cells;

		public override DataCell this[int index]
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

		internal ShimDataRow(CustomReportItem owner, int rowIndex, ShimDataMember parentDataMember)
			: base(owner, rowIndex)
		{
			this.m_cells = new List<ShimDataCell>();
			this.GenerateDataCells(parentDataMember, null, owner.CustomData.DataColumnHierarchy.MemberCollection as ShimDataMemberCollection);
		}

		private void GenerateDataCells(ShimDataMember rowParentMember, ShimDataMember columnParentMember, ShimDataMemberCollection columnMembers)
		{
			if (columnMembers == null)
			{
				this.m_cells.Add(new ShimDataCell(base.m_owner, base.m_rowIndex, this.m_cells.Count, rowParentMember, columnParentMember));
			}
			else
			{
				int count = columnMembers.Count;
				for (int i = 0; i < count; i++)
				{
					ShimDataMember shimDataMember = ((ReportElementCollectionBase<DataMember>)columnMembers)[i] as ShimDataMember;
					this.GenerateDataCells(rowParentMember, shimDataMember, shimDataMember.Children as ShimDataMemberCollection);
				}
			}
		}
	}
}
