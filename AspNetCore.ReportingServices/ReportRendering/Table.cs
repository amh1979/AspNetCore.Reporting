using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Table : DataRegion
	{
		private TableGroupCollection m_tableGroups;

		private TableRowsCollection m_detailRows;

		private TableHeaderFooterRows m_headerRows;

		private TableHeaderFooterRows m_footerRows;

		private TableColumnCollection m_tableColumns;

		private bool m_calculatedFixedColumnHeaders;

		public override bool PageBreakAtEnd
		{
			get
			{
				if (((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).PageBreakAtEnd)
				{
					return true;
				}
				if (((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).FooterRows != null && !((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).FooterRepeatOnNewPage)
				{
					return false;
				}
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).PropagatedPageBreakAtEnd;
			}
		}

		public override bool PageBreakAtStart
		{
			get
			{
				if (((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).PageBreakAtStart)
				{
					return true;
				}
				if (((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).HeaderRows != null && !((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).HeaderRepeatOnNewPage)
				{
					return false;
				}
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).PropagatedPageBreakAtStart;
			}
		}

		public bool GroupBreakAtStart
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).GroupBreakAtStart;
			}
		}

		public bool GroupBreakAtEnd
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).GroupBreakAtEnd;
			}
		}

		public TableColumnCollection Columns
		{
			get
			{
				TableColumnCollection tableColumnCollection = this.m_tableColumns;
				if (this.m_tableColumns == null)
				{
					tableColumnCollection = new TableColumnCollection(this, ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).TableColumns);
					if (base.RenderingContext.CacheState)
					{
						this.m_tableColumns = tableColumnCollection;
					}
				}
				return tableColumnCollection;
			}
		}

		public TableGroupCollection TableGroups
		{
			get
			{
				TableGroupCollection tableGroupCollection = this.m_tableGroups;
				if (this.m_tableGroups == null && ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).TableGroups != null)
				{
					tableGroupCollection = new TableGroupCollection(this, null, ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).TableGroups, (base.ReportItemInstance == null) ? null : ((TableInstance)base.ReportItemInstance).TableGroupInstances);
					if (base.RenderingContext.CacheState)
					{
						this.m_tableGroups = tableGroupCollection;
					}
				}
				return tableGroupCollection;
			}
		}

		public TableHeaderFooterRows TableHeader
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Table table = (AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
				TableHeaderFooterRows tableHeaderFooterRows = this.m_headerRows;
				if (this.m_headerRows == null && table.HeaderRows != null)
				{
					tableHeaderFooterRows = new TableHeaderFooterRows(this, table.HeaderRepeatOnNewPage, table.HeaderRows, (base.ReportItemInstance == null) ? null : ((TableInstance)base.ReportItemInstance).HeaderRowInstances);
					if (base.RenderingContext.CacheState)
					{
						this.m_headerRows = tableHeaderFooterRows;
					}
				}
				return tableHeaderFooterRows;
			}
		}

		public TableHeaderFooterRows TableFooter
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Table table = (AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
				TableHeaderFooterRows tableHeaderFooterRows = this.m_footerRows;
				if (this.m_footerRows == null && table.FooterRows != null)
				{
					tableHeaderFooterRows = new TableHeaderFooterRows(this, table.FooterRepeatOnNewPage, table.FooterRows, (base.ReportItemInstance == null) ? null : ((TableInstance)base.ReportItemInstance).FooterRowInstances);
					if (base.RenderingContext.CacheState)
					{
						this.m_footerRows = tableHeaderFooterRows;
					}
				}
				return tableHeaderFooterRows;
			}
		}

		public TableRowsCollection DetailRows
		{
			get
			{
				AspNetCore.ReportingServices.ReportProcessing.Table table = (AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
				TableRowsCollection tableRowsCollection = this.m_detailRows;
				if (this.m_detailRows == null && table.TableGroups == null && table.TableDetail != null)
				{
					tableRowsCollection = new TableRowsCollection(this, table.TableDetail, (base.ReportItemInstance == null) ? null : ((TableInstance)base.ReportItemInstance).TableDetailInstances);
					if (base.RenderingContext.CacheState)
					{
						this.m_detailRows = tableRowsCollection;
					}
				}
				return tableRowsCollection;
			}
		}

		public string DetailDataElementName
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailDataElementName;
			}
		}

		public string DetailDataCollectionName
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailDataCollectionName;
			}
		}

		public DataElementOutputTypes DetailDataElementOutput
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailDataElementOutput;
			}
		}

		public SharedHiddenState DetailSharedHidden
		{
			get
			{
				if (((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailGroup == null)
				{
					return Visibility.GetSharedHidden(((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).TableDetail.Visibility);
				}
				return Visibility.GetSharedHidden(((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).DetailGroup.Visibility);
			}
		}

		public override bool NoRows
		{
			get
			{
				TableInstance tableInstance = (TableInstance)base.ReportItemInstance;
				if (tableInstance != null && (tableInstance.TableGroupInstances == null || tableInstance.TableGroupInstances.Count != 0) && (tableInstance.TableDetailInstances == null || tableInstance.TableDetailInstances.Count != 0))
				{
					return false;
				}
				return true;
			}
		}

		public bool UseOWC
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).UseOWC;
			}
		}

		public bool ContainsNonSharedStyles
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).OWCNonSharedStyles;
			}
		}

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((TableInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		public bool FixedHeader
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef).FixedHeader;
			}
		}

		internal bool HasFixedColumnHeaders
		{
			get
			{
				if (!this.FixedHeader)
				{
					return false;
				}
				AspNetCore.ReportingServices.ReportProcessing.Table table = (AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
				if (!this.m_calculatedFixedColumnHeaders)
				{
					table.HasFixedColumnHeaders = false;
					if (table.FixedHeader)
					{
						int count = table.TableColumns.Count;
						table.HasFixedColumnHeaders = (table.TableColumns[0].FixedHeader || table.TableColumns[count - 1].FixedHeader);
					}
					this.m_calculatedFixedColumnHeaders = true;
				}
				return table.HasFixedColumnHeaders;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.Table TableDefinition
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportProcessing.Table)base.ReportItemDef;
			}
		}

		internal Table(int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.Table reportItemDef, TableInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch)
			{
				return false;
			}
			if (this.NoRows && base.NoRowMessage != null)
			{
				return false;
			}
			if (searchContext.ItemStartPage == searchContext.ItemEndPage)
			{
				return Table.SearchFullTable(this, searchContext);
			}
			return this.SearchPartialTable(this, searchContext);
		}

		private static int TableWithVisibleColumns(TableColumnCollection columns)
		{
			int num = columns.Count;
			for (int i = 0; i < columns.Count; i++)
			{
				if (columns[i].Hidden)
				{
					num--;
				}
			}
			return num;
		}

		private static bool SearchFullTable(Table table, SearchContext searchContext)
		{
			bool result = false;
			TableColumnCollection columns = table.Columns;
			if (Table.TableWithVisibleColumns(columns) == 0)
			{
				return result;
			}
			result = Table.SearchTableRows(table.TableHeader, columns, searchContext);
			if (!table.NoRows && !result)
			{
				TableGroupCollection tableGroups = table.TableGroups;
				if (tableGroups != null)
				{
					int num = 0;
					while (!result && num < tableGroups.Count)
					{
						result = Table.SearchFullTableGroup(tableGroups[num], columns, searchContext);
						num++;
					}
				}
				else
				{
					TableRowsCollection detailRows = table.DetailRows;
					if (detailRows != null)
					{
						int num2 = 0;
						while (!result && num2 < detailRows.Count)
						{
							result = Table.SearchTableRows(detailRows[num2], columns, searchContext);
							num2++;
						}
					}
				}
			}
			if (!result)
			{
				result = Table.SearchTableRows(table.TableFooter, columns, searchContext);
			}
			return result;
		}

		private bool SearchPartialTable(Table table, SearchContext searchContext)
		{
			TableHeaderFooterRows tableHeaderFooterRows = null;
			bool flag = false;
			SearchContext searchContext2 = new SearchContext(searchContext);
			TableColumnCollection columns = table.Columns;
			tableHeaderFooterRows = table.TableHeader;
			if (tableHeaderFooterRows != null && (tableHeaderFooterRows.RepeatOnNewPage || searchContext.ItemStartPage == searchContext.SearchPage))
			{
				flag = Table.SearchTableRows(tableHeaderFooterRows, columns, searchContext2);
				if (flag)
				{
					return true;
				}
			}
			TableGroupCollection tableGroups = table.TableGroups;
			if (tableGroups != null)
			{
				int num = 0;
				int num2 = 0;
				int num3 = -1;
				int num4 = -1;
				this.GetTableGroupsOnPage(searchContext.SearchPage, out num, out num2);
				if (num >= 0)
				{
					SearchContext searchContext3 = new SearchContext(searchContext);
					this.IsGroupOnThisPage(num, searchContext.SearchPage, out num3, out num4);
					TableGroup tableGroup = tableGroups[num];
					if (num3 != num4)
					{
						searchContext3.ItemStartPage = num3;
						searchContext3.ItemEndPage = num4;
						flag = Table.SearchPartialTableGroup(tableGroup, columns, searchContext3);
					}
					else
					{
						flag = Table.SearchFullTableGroup(tableGroup, columns, searchContext2);
					}
					num++;
					while (!flag && num < num2)
					{
						tableGroup = tableGroups[num];
						flag = Table.SearchFullTableGroup(tableGroup, columns, searchContext2);
						num++;
					}
					if (!flag && num == num2)
					{
						this.IsGroupOnThisPage(num, searchContext.SearchPage, out num3, out num4);
						tableGroup = tableGroups[num];
						if (num3 != num4)
						{
							searchContext3.ItemStartPage = num3;
							searchContext3.ItemEndPage = num4;
							flag = Table.SearchPartialTableGroup(tableGroup, columns, searchContext3);
						}
						else
						{
							flag = Table.SearchFullTableGroup(tableGroup, columns, searchContext2);
						}
					}
				}
			}
			else
			{
				TableRowsCollection detailRows = table.DetailRows;
				if (detailRows != null)
				{
					int num5 = 0;
					int num6 = 0;
					int num7 = 0;
					this.GetDetailsOnThisPage(searchContext.SearchPage - searchContext.ItemStartPage, out num5, out num6);
					if (num5 >= 0)
					{
						num7 = num5 + num6 - 1;
						while (!flag && num5 <= num7)
						{
							flag = Table.SearchTableRows(detailRows[num5], columns, searchContext2);
							num5++;
						}
					}
				}
			}
			if (flag)
			{
				return true;
			}
			tableHeaderFooterRows = table.TableFooter;
			if (tableHeaderFooterRows != null && (tableHeaderFooterRows.RepeatOnNewPage || searchContext.ItemEndPage == searchContext.SearchPage))
			{
				flag = Table.SearchTableRows(tableHeaderFooterRows, columns, searchContext2);
			}
			return flag;
		}

		private static bool SearchFullTableGroup(TableGroup tableGroup, TableColumnCollection columns, SearchContext searchContext)
		{
			bool result = false;
			if (tableGroup != null && !tableGroup.Hidden)
			{
				result = Table.SearchTableRows(tableGroup.GroupHeader, columns, searchContext);
				if (!result)
				{
					TableGroupCollection subGroups = tableGroup.SubGroups;
					if (subGroups != null)
					{
						int num = 0;
						while (!result && num < subGroups.Count)
						{
							result = Table.SearchFullTableGroup(subGroups[num], columns, searchContext);
							num++;
						}
					}
					else
					{
						TableRowsCollection detailRows = tableGroup.DetailRows;
						if (detailRows != null)
						{
							int num2 = 0;
							while (!result && num2 < detailRows.Count)
							{
								result = Table.SearchTableRows(detailRows[num2], columns, searchContext);
								num2++;
							}
						}
					}
				}
				if (!result)
				{
					result = Table.SearchTableRows(tableGroup.GroupFooter, columns, searchContext);
				}
				return result;
			}
			return result;
		}

		private static bool SearchPartialTableGroup(TableGroup group, TableColumnCollection columns, SearchContext searchContext)
		{
			TableHeaderFooterRows tableHeaderFooterRows = null;
			bool flag = false;
			SearchContext searchContext2 = new SearchContext(searchContext);
			tableHeaderFooterRows = group.GroupHeader;
			if (tableHeaderFooterRows != null)
			{
				if (searchContext.SearchPage == searchContext.ItemStartPage || tableHeaderFooterRows.RepeatOnNewPage)
				{
					flag = Table.SearchTableRows(tableHeaderFooterRows, columns, searchContext2);
				}
				if (flag)
				{
					return true;
				}
			}
			TableGroupCollection subGroups = group.SubGroups;
			if (subGroups != null)
			{
				int num = 0;
				int num2 = 0;
				int num3 = -1;
				int num4 = -1;
				group.GetSubGroupsOnPage(searchContext.SearchPage, out num, out num2);
				if (num >= 0)
				{
					SearchContext searchContext3 = new SearchContext(searchContext);
					group.IsGroupOnThisPage(num, searchContext.SearchPage, out num3, out num4);
					TableGroup tableGroup = subGroups[num];
					if (num3 != num4)
					{
						searchContext3.ItemStartPage = num3;
						searchContext3.ItemEndPage = num4;
						flag = Table.SearchPartialTableGroup(tableGroup, columns, searchContext3);
					}
					else
					{
						flag = Table.SearchFullTableGroup(tableGroup, columns, searchContext2);
					}
					num++;
					while (!flag && num < num2)
					{
						tableGroup = subGroups[num];
						flag = Table.SearchFullTableGroup(tableGroup, columns, searchContext2);
						num++;
					}
					if (!flag && num == num2)
					{
						tableGroup = subGroups[num];
						group.IsGroupOnThisPage(num, searchContext.SearchPage, out num3, out num4);
						if (num3 != num4)
						{
							searchContext3.ItemStartPage = num3;
							searchContext3.ItemEndPage = num4;
							flag = Table.SearchPartialTableGroup(tableGroup, columns, searchContext3);
						}
						else
						{
							flag = Table.SearchFullTableGroup(tableGroup, columns, searchContext2);
						}
					}
				}
			}
			else
			{
				TableRowsCollection detailRows = group.DetailRows;
				if (detailRows != null)
				{
					int num5 = 0;
					int num6 = 0;
					int num7 = 0;
					group.GetDetailsOnThisPage(searchContext.SearchPage - searchContext.ItemStartPage, out num5, out num6);
					if (num5 >= 0)
					{
						num7 = num5 + num6 - 1;
						while (!flag && num5 <= num7)
						{
							flag = Table.SearchTableRows(detailRows[num5], columns, searchContext2);
							num5++;
						}
					}
				}
			}
			if (flag)
			{
				return true;
			}
			tableHeaderFooterRows = group.GroupFooter;
			if (tableHeaderFooterRows != null && (tableHeaderFooterRows.RepeatOnNewPage || searchContext.ItemEndPage == searchContext.SearchPage))
			{
				flag = Table.SearchTableRows(tableHeaderFooterRows, columns, searchContext2);
			}
			return flag;
		}

		private static bool SearchTableRows(TableRowCollection tableRows, TableColumnCollection columns, SearchContext searchContext)
		{
			bool flag = false;
			if (tableRows == null)
			{
				return flag;
			}
			TableRow tableRow = null;
			int num = 0;
			while (!flag && num < tableRows.Count)
			{
				tableRow = tableRows[num];
				if (!tableRow.Hidden)
				{
					flag = Table.SearchRowCells(tableRow.TableCellCollection, columns, searchContext);
				}
				num++;
			}
			return flag;
		}

		private static bool SearchRowCells(TableCellCollection rowCells, TableColumnCollection columns, SearchContext searchContext)
		{
			bool flag = false;
			if (rowCells == null)
			{
				return flag;
			}
			int num = 0;
			while (!flag && num < rowCells.Count)
			{
				if (!columns[num].Hidden)
				{
					flag = rowCells[num].ReportItem.Search(searchContext);
				}
				num++;
			}
			return flag;
		}

		public bool IsGroupOnThisPage(int groupIndex, int pageNumber, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			RenderingPagesRangesList childrenStartAndEndPages = ((TableInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
			if (childrenStartAndEndPages == null)
			{
				return true;
			}
			Global.Tracer.Assert(groupIndex >= 0 && groupIndex < childrenStartAndEndPages.Count);
			if (groupIndex >= childrenStartAndEndPages.Count)
			{
				return false;
			}
			RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[groupIndex];
			startPage = renderingPagesRanges.StartPage;
			endPage = renderingPagesRanges.EndPage;
			if (pageNumber >= startPage)
			{
				return pageNumber <= endPage;
			}
			return false;
		}

		public void GetDetailsOnThisPage(int pageIndex, out int start, out int numberOfDetails)
		{
			start = 0;
			numberOfDetails = 0;
			RenderingPagesRangesList childrenStartAndEndPages = ((TableInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
			if (childrenStartAndEndPages != null)
			{
				Global.Tracer.Assert(pageIndex >= 0 && pageIndex < childrenStartAndEndPages.Count);
				RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[pageIndex];
				start = renderingPagesRanges.StartRow;
				numberOfDetails = renderingPagesRanges.NumberOfDetails;
			}
		}

		public void GetTableGroupsOnPage(int page, out int startGroup, out int endGroup)
		{
			startGroup = -1;
			endGroup = -1;
			if (base.ReportItemInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = ((TableInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startGroup, ref endGroup);
				}
			}
		}
	}
}
