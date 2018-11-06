using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataRowCollection : DataRowCollection
	{
		private List<ShimDataRow> m_dataRows;

		public override DataRow this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_dataRows[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_dataRows.Count;
			}
		}

		internal ShimDataRowCollection(CustomReportItem owner)
			: base(owner)
		{
			this.m_dataRows = new List<ShimDataRow>();
			this.AppendDataRows(null, owner.CustomData.DataRowHierarchy.MemberCollection as ShimDataMemberCollection);
		}

		private void AppendDataRows(ShimDataMember rowParentMember, ShimDataMemberCollection rowMembers)
		{
			if (rowMembers == null)
			{
				this.m_dataRows.Add(new ShimDataRow(base.m_owner, this.m_dataRows.Count, rowParentMember));
			}
			else
			{
				int count = rowMembers.Count;
				for (int i = 0; i < count; i++)
				{
					ShimDataMember shimDataMember = ((ReportElementCollectionBase<DataMember>)rowMembers)[i] as ShimDataMember;
					this.AppendDataRows(shimDataMember, shimDataMember.Children as ShimDataMemberCollection);
				}
			}
		}

		internal void UpdateCells(ShimDataMember innermostMember)
		{
			if (innermostMember != null && innermostMember.Children == null)
			{
				if (!innermostMember.IsColumn)
				{
					int memberCellIndex = innermostMember.MemberCellIndex;
					int count = this.m_dataRows[memberCellIndex].Count;
					for (int i = 0; i < count; i++)
					{
						((ShimDataCell)((ReportElementCollectionBase<DataCell>)this.m_dataRows[memberCellIndex])[i]).SetNewContext();
					}
				}
				else
				{
					int memberCellIndex2 = innermostMember.MemberCellIndex;
					int count2 = this.m_dataRows.Count;
					for (int j = 0; j < count2; j++)
					{
						((ShimDataCell)((ReportElementCollectionBase<DataCell>)this.m_dataRows[j])[memberCellIndex2]).SetNewContext();
					}
				}
			}
		}
	}
}
