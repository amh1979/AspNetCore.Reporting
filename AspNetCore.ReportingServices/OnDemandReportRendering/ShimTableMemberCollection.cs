using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableMemberCollection : ShimMemberCollection
	{
		private int m_rowDefinitionStartIndex = -1;

		private int m_rowDefinitionEndIndex = -1;

		private int m_dynamicSubgroupChildIndex = -1;

		public override TablixMember this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return (TablixMember)base.m_children[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return base.m_children.Length;
			}
		}

		internal PageBreakLocation PropagatedGroupBreakLocation
		{
			get
			{
				if (this.m_dynamicSubgroupChildIndex < 0)
				{
					return PageBreakLocation.None;
				}
				return ((TablixMember)base.m_children[this.m_dynamicSubgroupChildIndex]).PropagatedGroupBreak;
			}
		}

		internal ShimTableMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, bool isColumnGroup)
			: base(parentDefinitionPath, owner, isColumnGroup)
		{
			if (base.m_isColumnGroup)
			{
				int count = owner.RenderTable.Columns.Count;
				base.m_children = new ShimTableMember[count];
				for (int i = 0; i < count; i++)
				{
					base.m_children[i] = new ShimTableMember(this, owner, i, owner.RenderTable.Columns);
				}
			}
			else
			{
				this.m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
				base.m_children = this.CreateInnerHierarchy(owner, (ShimTableMember)null, owner.RenderTable.TableHeader, owner.RenderTable.TableFooter, owner.RenderTable.TableGroups, owner.RenderTable.DetailRows, ref this.m_dynamicSubgroupChildIndex);
				this.m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
			}
		}

		internal ShimTableMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, AspNetCore.ReportingServices.ReportRendering.TableGroup tableGroup)
			: base(parentDefinitionPath, owner, false)
		{
			this.m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			base.m_children = this.CreateInnerHierarchy(owner, parent, tableGroup.GroupHeader, tableGroup.GroupFooter, tableGroup.SubGroups, tableGroup.DetailRows, ref this.m_dynamicSubgroupChildIndex);
			this.m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimTableMemberCollection(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, TableDetailRowCollection detailRows)
			: base(parentDefinitionPath, owner, false)
		{
			this.m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			int count = detailRows.Count;
			base.m_children = new ShimTableMember[count];
			for (int i = 0; i < count; i++)
			{
				base.m_children[i] = new ShimTableMember(this, owner, parent, i, ((TableRowCollection)detailRows)[i], KeepWithGroup.None, false);
			}
			this.m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		private TablixMember[] CreateInnerHierarchy(Tablix owner, ShimTableMember parent, TableHeaderFooterRows headerRows, TableHeaderFooterRows footerRows, TableGroupCollection subGroups, TableRowsCollection detailRows, ref int dynamicSubgroupChildIndex)
		{
			List<ShimTableMember> list = new List<ShimTableMember>();
			bool noKeepWith = subGroups == null && null == detailRows;
			this.CreateHeaderFooter(list, headerRows, ShimTableMemberCollection.DetermineKeepWithGroup(true, headerRows, noKeepWith), owner, parent, parent == null && owner.RenderTable.FixedHeader);
			if (subGroups != null)
			{
				dynamicSubgroupChildIndex = list.Count;
				this.CreateInnerDynamicGroups(list, subGroups, owner, parent);
			}
			else if (detailRows != null)
			{
				dynamicSubgroupChildIndex = list.Count;
				list.Add(new ShimTableMember(this, owner, parent, dynamicSubgroupChildIndex, detailRows));
			}
			this.CreateHeaderFooter(list, footerRows, ShimTableMemberCollection.DetermineKeepWithGroup(false, footerRows, noKeepWith), owner, parent, false);
			return list.ToArray();
		}

		private static KeepWithGroup DetermineKeepWithGroup(bool isHeader, TableHeaderFooterRows rows, bool noKeepWith)
		{
			if (!noKeepWith && rows != null && rows.RepeatOnNewPage)
			{
				if (isHeader)
				{
					return KeepWithGroup.After;
				}
				return KeepWithGroup.Before;
			}
			return KeepWithGroup.None;
		}

		private void CreateHeaderFooter(List<ShimTableMember> rowGroups, TableHeaderFooterRows headerFooterRows, KeepWithGroup keepWithGroup, Tablix owner, ShimTableMember parent, bool isFixedTableHeader)
		{
			if (headerFooterRows != null)
			{
				int count = headerFooterRows.Count;
				int num = rowGroups.Count;
				for (int i = 0; i < count; i++)
				{
					rowGroups.Add(new ShimTableMember(this, owner, parent, num, ((TableRowCollection)headerFooterRows)[i], keepWithGroup, isFixedTableHeader));
					num++;
				}
			}
		}

		private void CreateInnerDynamicGroups(List<ShimTableMember> rowGroups, TableGroupCollection renderGroupCollection, Tablix owner, ShimTableMember parent)
		{
			if (renderGroupCollection != null)
			{
				ShimTableMember item = new ShimTableMember(this, owner, parent, rowGroups.Count, new ShimRenderGroups(renderGroupCollection));
				rowGroups.Add(item);
			}
		}

		internal void UpdateContext()
		{
			if (base.m_children != null)
			{
				this.UpdateHeaderFooter(base.OwnerTablix.RenderTable.TableHeader, base.OwnerTablix.RenderTable.TableFooter);
				if (this.m_dynamicSubgroupChildIndex >= 0)
				{
					((ShimTableMember)base.m_children[this.m_dynamicSubgroupChildIndex]).ResetContext(base.OwnerTablix.RenderTable.TableGroups, base.OwnerTablix.RenderTable.DetailRows);
				}
			}
		}

		internal void UpdateHeaderFooter(TableHeaderFooterRows headerRows, TableHeaderFooterRows footerRows)
		{
			if (base.m_children != null)
			{
				if (headerRows == null && footerRows == null)
				{
					return;
				}
				int num = (headerRows != null) ? headerRows.Count : 0;
				int num2 = (footerRows != null) ? footerRows.Count : 0;
				int num3 = base.m_children.Length;
				for (int i = 0; i < num; i++)
				{
					((ShimTableMember)base.m_children[i]).UpdateRow(((TableRowCollection)headerRows)[i]);
				}
				for (int num4 = num2; num4 > 0; num4--)
				{
					((ShimTableMember)base.m_children[num3 - num4]).UpdateRow(((TableRowCollection)footerRows)[num2 - num4]);
				}
			}
		}

		internal void UpdateDetails(TableDetailRowCollection newRenderDetails)
		{
			if (base.m_children != null && newRenderDetails != null)
			{
				for (int i = 0; i < base.m_children.Length; i++)
				{
					((ShimTableMember)base.m_children[i]).UpdateRow(((TableRowCollection)newRenderDetails)[i]);
				}
				return;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
		}

		internal void ResetContext(AspNetCore.ReportingServices.ReportRendering.TableGroup newRenderGroup)
		{
			if (base.m_children != null)
			{
				for (int i = 0; i < base.m_children.Length; i++)
				{
					((ShimTableMember)base.m_children[i]).ResetContext((newRenderGroup != null) ? newRenderGroup.SubGroups : null, (newRenderGroup != null) ? newRenderGroup.DetailRows : null);
				}
			}
		}
	}
}
