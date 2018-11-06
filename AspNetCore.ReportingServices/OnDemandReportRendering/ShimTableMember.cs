using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableMember : ShimTablixMember
	{
		private bool m_isDetailGroup;

		private bool m_isFixedHeader;

		private KeepWithGroup m_keepWithGroup;

		private int m_rowDefinitionStartIndex = -1;

		private int m_rowDefinitionEndIndex = -1;

		private TableColumn m_column;

		private TableRowsCollection m_renderDetails;

		private TableRow m_innerStaticRow;

		public override KeepWithGroup KeepWithGroup
		{
			get
			{
				return this.m_keepWithGroup;
			}
		}

		public override bool RepeatOnNewPage
		{
			get
			{
				return this.m_keepWithGroup != KeepWithGroup.None;
			}
		}

		public override string DataElementName
		{
			get
			{
				if (this.m_isDetailGroup)
				{
					return base.OwnerTablix.RenderTable.DetailDataCollectionName;
				}
				return base.DataElementName;
			}
		}

		public override TablixMemberCollection Children
		{
			get
			{
				if (this.IsColumn)
				{
					return null;
				}
				return base.m_children;
			}
		}

		public override bool FixedData
		{
			get
			{
				return this.m_isFixedHeader;
			}
		}

		public override bool IsStatic
		{
			get
			{
				if (this.IsColumn)
				{
					return true;
				}
				if (!this.m_isDetailGroup && (base.m_group == null || base.m_group.RenderGroups == null))
				{
					return true;
				}
				return false;
			}
		}

		public override bool IsColumn
		{
			get
			{
				return null != this.m_column;
			}
		}

		internal override int RowSpan
		{
			get
			{
				if (this.IsColumn)
				{
					return 0;
				}
				if (this.IsStatic)
				{
					return 1;
				}
				return this.m_rowDefinitionEndIndex - this.m_rowDefinitionStartIndex;
			}
		}

		internal override int ColSpan
		{
			get
			{
				if (this.IsColumn)
				{
					return 1;
				}
				return 0;
			}
		}

		public override int MemberCellIndex
		{
			get
			{
				return this.m_rowDefinitionStartIndex;
			}
		}

		public override TablixHeader TablixHeader
		{
			get
			{
				return null;
			}
		}

		public override bool IsTotal
		{
			get
			{
				return false;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (base.m_visibility == null)
				{
					if (this.IsColumn && this.m_column.ColumnDefinition.Visibility != null)
					{
						base.m_visibility = new ShimTableMemberVisibility(this, ShimTableMemberVisibility.Mode.StaticColumn);
					}
					else if (!this.IsColumn && base.m_group != null)
					{
						if (this.m_isDetailGroup && this.m_renderDetails.DetailDefinition.Visibility != null)
						{
							base.m_visibility = new ShimTableMemberVisibility(this, ShimTableMemberVisibility.Mode.TableDetails);
						}
						else if (!this.m_isDetailGroup && base.m_group.CurrentShimRenderGroup.m_visibilityDef != null)
						{
							base.m_visibility = new ShimTableMemberVisibility(this, ShimTableMemberVisibility.Mode.TableGroup);
						}
					}
					else if (!this.IsColumn && this.m_innerStaticRow != null && this.m_innerStaticRow.m_rowDef.Visibility != null)
					{
						base.m_visibility = new ShimTableMemberVisibility(this, ShimTableMemberVisibility.Mode.StaticRow);
					}
				}
				return base.m_visibility;
			}
		}

		internal override PageBreakLocation PropagatedGroupBreak
		{
			get
			{
				if (this.IsStatic)
				{
					return PageBreakLocation.None;
				}
				return base.m_propagatedPageBreak;
			}
		}

		public override bool HideIfNoRows
		{
			get
			{
				if (this.IsStatic && base.m_parent == null)
				{
					return false;
				}
				return true;
			}
		}

		public override TablixMemberInstance Instance
		{
			get
			{
				if (base.OwnerTablix.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					if (this.IsStatic)
					{
						base.m_instance = new TablixMemberInstance(base.OwnerTablix, this);
					}
					else
					{
						TablixDynamicMemberInstance instance = new TablixDynamicMemberInstance(base.OwnerTablix, this, new InternalShimDynamicMemberLogic(this));
						base.m_owner.RenderingContext.AddDynamicInstance(instance);
						base.m_instance = instance;
					}
				}
				return base.m_instance;
			}
		}

		internal int RowDefinitionEndIndex
		{
			get
			{
				return this.m_rowDefinitionEndIndex;
			}
		}

		internal string DetailInstanceUniqueName
		{
			get
			{
				if (!this.m_isDetailGroup)
				{
					return null;
				}
				if (base.m_group.CurrentRenderGroupIndex < 0)
				{
					return null;
				}
				string uniqueName = ((TableRowCollection)this.m_renderDetails[base.m_group.CurrentRenderGroupIndex])[0].UniqueName;
				return base.m_owner.RenderingContext.GenerateShimUniqueName(uniqueName);
			}
		}

		internal TableRowsCollection RenderTableDetails
		{
			get
			{
				if (!this.IsColumn)
				{
					return this.m_renderDetails;
				}
				return null;
			}
		}

		internal TableGroup RenderTableGroup
		{
			get
			{
				if (!this.IsColumn && !this.m_isDetailGroup && base.m_group != null)
				{
					return (TableGroup)base.m_group.CurrentShimRenderGroup;
				}
				return null;
			}
		}

		internal TableRow RenderTableRow
		{
			get
			{
				if (!this.IsColumn)
				{
					return this.m_innerStaticRow;
				}
				return null;
			}
		}

		internal TableColumn RenderTableColumn
		{
			get
			{
				if (this.IsColumn)
				{
					return this.m_column;
				}
				return null;
			}
		}

		internal ShimTableMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, int parentCollectionIndex, TableRow staticRow, KeepWithGroup keepWithGroup, bool isFixedTableHeader)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, false)
		{
			this.m_innerStaticRow = staticRow;
			this.m_rowDefinitionStartIndex = owner.GetAndIncrementMemberCellDefinitionIndex();
			this.m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
			this.m_keepWithGroup = keepWithGroup;
			this.m_isFixedHeader = isFixedTableHeader;
		}

		internal ShimTableMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, int parentCollectionIndex, ShimRenderGroups renderGroups)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, false)
		{
			this.m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			if (renderGroups != null)
			{
				base.m_children = new ShimTableMemberCollection(this, (Tablix)base.m_owner, this, (TableGroup)renderGroups[0]);
			}
			base.m_group = new Group(owner, renderGroups, this);
			this.m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimTableMember(IDefinitionPath parentDefinitionPath, Tablix owner, ShimTableMember parent, int parentCollectionIndex, TableRowsCollection renderRows)
			: base(parentDefinitionPath, owner, parent, parentCollectionIndex, false)
		{
			this.m_rowDefinitionStartIndex = owner.GetCurrentMemberCellDefinitionIndex();
			this.m_isDetailGroup = true;
			this.m_renderDetails = renderRows;
			base.m_children = new ShimTableMemberCollection(this, (Tablix)base.m_owner, this, renderRows[0]);
			base.m_group = new Group(owner, this);
			this.m_rowDefinitionEndIndex = owner.GetCurrentMemberCellDefinitionIndex();
		}

		internal ShimTableMember(IDefinitionPath parentDefinitionPath, Tablix owner, int columnIndex, TableColumnCollection columns)
			: base(parentDefinitionPath, owner, null, columnIndex, true)
		{
			this.m_column = columns[columnIndex];
			this.m_isFixedHeader = this.m_column.FixedHeader;
			this.m_rowDefinitionStartIndex = (this.m_rowDefinitionEndIndex = columnIndex);
		}

		internal override bool SetNewContext(int index)
		{
			base.ResetContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_isDetailGroup)
			{
				if (base.OwnerTablix.RenderTable.NoRows)
				{
					return false;
				}
				if (this.m_renderDetails != null && index >= 0 && index < this.m_renderDetails.Count)
				{
					bool flag = base.m_group.CurrentRenderGroupIndex == -1 && 0 == index;
					base.m_group.CurrentRenderGroupIndex = index;
					if (!flag)
					{
						((ShimTableMemberCollection)base.m_children).UpdateDetails(this.m_renderDetails[index]);
					}
					return true;
				}
				return false;
			}
			if (base.m_group != null && base.m_group.RenderGroups != null)
			{
				if (base.OwnerTablix.RenderTable.NoRows)
				{
					return false;
				}
				if (index >= 0 && index < base.m_group.RenderGroups.Count)
				{
					TableGroup tableGroup = base.m_group.RenderGroups[index] as TableGroup;
					if (tableGroup.InstanceInfo == null)
					{
						return false;
					}
					base.m_group.CurrentRenderGroupIndex = index;
					((ShimTableMemberCollection)base.m_children).UpdateHeaderFooter(tableGroup.GroupHeader, tableGroup.GroupFooter);
					((ShimTableMemberCollection)base.m_children).ResetContext(tableGroup);
					return true;
				}
				return false;
			}
			return index <= 1;
		}

		internal void UpdateRow(TableRow newTableRow)
		{
			this.m_innerStaticRow = newTableRow;
			((ShimTableRow)((ReportElementCollectionBase<TablixRow>)(ShimTableRowCollection)base.OwnerTablix.Body.RowCollection)[this.m_rowDefinitionStartIndex]).UpdateCells(newTableRow);
		}

		internal override void ResetContext()
		{
			base.ResetContext();
			if (base.m_group.CurrentRenderGroupIndex >= 0)
			{
				this.ResetContext(null, null);
			}
		}

		internal void ResetContext(TableGroupCollection newRenderSubGroups, TableRowsCollection newRenderDetails)
		{
			if (this.m_isDetailGroup)
			{
				base.m_group.CurrentRenderGroupIndex = -1;
				if (newRenderDetails != null)
				{
					this.m_renderDetails = newRenderDetails;
				}
				((ShimTableMemberCollection)base.m_children).UpdateDetails(this.m_renderDetails[0]);
			}
			else if (base.m_group != null && base.m_group.RenderGroups != null)
			{
				base.m_group.CurrentRenderGroupIndex = -1;
				if (newRenderSubGroups != null)
				{
					base.m_group.RenderGroups = new ShimRenderGroups(newRenderSubGroups);
				}
				if (base.m_children != null)
				{
					TableGroup tableGroup = base.m_group.CurrentShimRenderGroup as TableGroup;
					((ShimTableMemberCollection)base.m_children).UpdateHeaderFooter(tableGroup.GroupHeader, tableGroup.GroupFooter);
					((ShimTableMemberCollection)base.m_children).ResetContext(null);
				}
			}
			this.SetNewContext(true);
		}
	}
}
