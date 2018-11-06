using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableGroup : Group, IDocumentMapEntry
	{
		private AspNetCore.ReportingServices.ReportProcessing.TableGroup m_groupDef;

		private TableGroupInstance m_groupInstance;

		private TableGroupInstanceInfo m_groupInstanceInfo;

		private TableGroupCollection m_subGroups;

		private TableRowsCollection m_detailRows;

		private TableHeaderFooterRows m_headerRows;

		private TableHeaderFooterRows m_footerRows;

		private TableGroup m_parent;

		public override string ID
		{
			get
			{
				if (this.m_groupDef.RenderingModelID == null)
				{
					this.m_groupDef.RenderingModelID = this.m_groupDef.ID.ToString(CultureInfo.InvariantCulture);
				}
				return this.m_groupDef.RenderingModelID;
			}
		}

		public override string Label
		{
			get
			{
				string result = null;
				if (base.m_groupingDef != null && base.m_groupingDef.GroupLabel != null)
				{
					result = ((base.m_groupingDef.GroupLabel.Type != ExpressionInfo.Types.Constant) ? ((this.m_groupInstance != null) ? this.InstanceInfo.Label : null) : base.m_groupingDef.GroupLabel.Value);
				}
				return result;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				if (this.m_groupInstance != null && base.m_groupingDef != null)
				{
					return null != base.m_groupingDef.GroupLabel;
				}
				return false;
			}
		}

		public TableHeaderFooterRows GroupHeader
		{
			get
			{
				TableHeaderFooterRows tableHeaderFooterRows = this.m_headerRows;
				if (this.m_headerRows == null && this.m_groupDef.HeaderRows != null)
				{
					tableHeaderFooterRows = new TableHeaderFooterRows((Table)base.OwnerDataRegion, this.m_groupDef.HeaderRepeatOnNewPage, this.m_groupDef.HeaderRows, (this.m_groupInstance == null) ? null : this.m_groupInstance.HeaderRowInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						this.m_headerRows = tableHeaderFooterRows;
					}
				}
				return tableHeaderFooterRows;
			}
		}

		public TableHeaderFooterRows GroupFooter
		{
			get
			{
				TableHeaderFooterRows tableHeaderFooterRows = this.m_footerRows;
				if (this.m_footerRows == null && this.m_groupDef.FooterRows != null)
				{
					tableHeaderFooterRows = new TableHeaderFooterRows((Table)base.OwnerDataRegion, this.m_groupDef.FooterRepeatOnNewPage, this.m_groupDef.FooterRows, (this.m_groupInstance == null) ? null : this.m_groupInstance.FooterRowInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						this.m_footerRows = tableHeaderFooterRows;
					}
				}
				return tableHeaderFooterRows;
			}
		}

		public override bool PageBreakAtStart
		{
			get
			{
				if (this.m_groupDef.Grouping.PageBreakAtStart)
				{
					return true;
				}
				if (this.m_groupDef.HeaderRows != null && !this.m_groupDef.HeaderRepeatOnNewPage)
				{
					return false;
				}
				return this.m_groupDef.PropagatedPageBreakAtStart;
			}
		}

		public override bool PageBreakAtEnd
		{
			get
			{
				if (this.m_groupDef.Grouping.PageBreakAtEnd)
				{
					return true;
				}
				if (this.m_groupDef.FooterRows != null && !this.m_groupDef.FooterRepeatOnNewPage)
				{
					return false;
				}
				return this.m_groupDef.PropagatedPageBreakAtEnd;
			}
		}

		public TableGroupCollection SubGroups
		{
			get
			{
				TableGroupCollection tableGroupCollection = this.m_subGroups;
				if (this.m_subGroups == null && this.m_groupDef.SubGroup != null)
				{
					tableGroupCollection = new TableGroupCollection((Table)base.OwnerDataRegion, this, this.m_groupDef.SubGroup, (this.m_groupInstance == null) ? null : this.m_groupInstance.SubGroupInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						this.m_subGroups = tableGroupCollection;
					}
				}
				return tableGroupCollection;
			}
		}

		public TableRowsCollection DetailRows
		{
			get
			{
				TableRowsCollection tableRowsCollection = this.m_detailRows;
				AspNetCore.ReportingServices.ReportProcessing.Table table = (AspNetCore.ReportingServices.ReportProcessing.Table)base.OwnerDataRegion.ReportItemDef;
				if (this.m_detailRows == null && this.m_groupDef.SubGroup == null && table.TableDetail != null)
				{
					tableRowsCollection = new TableRowsCollection((Table)base.OwnerDataRegion, table.TableDetail, (this.m_groupInstance == null) ? null : this.m_groupInstance.TableDetailInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						this.m_detailRows = tableRowsCollection;
					}
				}
				return tableRowsCollection;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (this.m_groupInstance == null)
				{
					return true;
				}
				if (this.m_groupDef.Visibility == null)
				{
					return false;
				}
				if (this.m_groupDef.Visibility.Toggle != null)
				{
					return base.OwnerDataRegion.RenderingContext.IsItemHidden(this.m_groupInstance.UniqueName, false);
				}
				return this.InstanceInfo.StartHidden;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = base.m_customProperties;
				if (base.m_customProperties == null)
				{
					if (this.m_groupDef.Grouping == null || this.m_groupDef.Grouping.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((this.m_groupInstance != null) ? new CustomPropertyCollection(this.m_groupDef.Grouping.CustomProperties, this.InstanceInfo.CustomPropertyInstances) : new CustomPropertyCollection(this.m_groupDef.Grouping.CustomProperties, null));
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						base.m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public TableGroup Parent
		{
			get
			{
				return this.m_parent;
			}
		}

		internal TableGroupInstanceInfo InstanceInfo
		{
			get
			{
				if (this.m_groupInstance == null)
				{
					return null;
				}
				if (this.m_groupInstanceInfo == null)
				{
					this.m_groupInstanceInfo = this.m_groupInstance.GetInstanceInfo(base.OwnerDataRegion.RenderingContext.ChunkManager);
				}
				return this.m_groupInstanceInfo;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.TableGroup GroupDefinition
		{
			get
			{
				return this.m_groupDef;
			}
		}

		internal TableGroup(Table owner, TableGroup parent, AspNetCore.ReportingServices.ReportProcessing.TableGroup groupDef, TableGroupInstance groupInstance)
			: base(owner, groupDef.Grouping, groupDef.Visibility)
		{
			this.m_parent = parent;
			this.m_groupDef = groupDef;
			this.m_groupInstance = groupInstance;
			if (this.m_groupInstance != null)
			{
				base.m_uniqueName = this.m_groupInstance.UniqueName;
			}
		}

		public void GetDetailsOnThisPage(int pageIndex, out int start, out int numberOfDetails)
		{
			start = 0;
			numberOfDetails = 0;
			if (this.m_groupInstance.ChildrenStartAndEndPages != null)
			{
				Global.Tracer.Assert(pageIndex >= 0 && pageIndex < this.m_groupInstance.ChildrenStartAndEndPages.Count);
				RenderingPagesRanges renderingPagesRanges = this.m_groupInstance.ChildrenStartAndEndPages[pageIndex];
				start = renderingPagesRanges.StartRow;
				numberOfDetails = renderingPagesRanges.NumberOfDetails;
			}
		}

		public bool IsGroupOnThisPage(int groupIndex, int pageNumber, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			if (this.m_groupInstance.ChildrenStartAndEndPages != null && groupIndex < this.m_groupInstance.ChildrenStartAndEndPages.Count)
			{
				RenderingPagesRanges renderingPagesRanges = this.m_groupInstance.ChildrenStartAndEndPages[groupIndex];
				startPage = renderingPagesRanges.StartPage;
				endPage = renderingPagesRanges.EndPage;
				if (pageNumber >= startPage)
				{
					return pageNumber <= endPage;
				}
				return false;
			}
			return false;
		}

		public void GetSubGroupsOnPage(int page, out int startGroup, out int endGroup)
		{
			startGroup = -1;
			endGroup = -1;
			if (this.m_groupInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = this.m_groupInstance.ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startGroup, ref endGroup);
				}
			}
		}
	}
}
