using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Matrix : DataRegion
	{
		private ReportItem m_corner;

		private MatrixCellCollection m_cells;

		private MatrixMemberCollection m_columns;

		private MatrixMemberCollection m_rows;

		private int m_groupsBeforeRowHeaders = -1;

		private int m_cellsBeforeRowHeaders = -1;

		private SizeCollection m_cellWidths;

		private SizeCollection m_cellHeights;

		private bool m_noRows;

		private List<int> m_rowMemberMapping;

		private List<int> m_colMemberMapping;

		public MatrixLayoutDirection LayoutDirection
		{
			get
			{
				if (((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).LayoutDirection)
				{
					return MatrixLayoutDirection.RTL;
				}
				return MatrixLayoutDirection.LTR;
			}
		}

		public override bool PageBreakAtEnd
		{
			get
			{
				if (!((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PageBreakAtEnd)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PropagatedPageBreakAtEnd;
				}
				return true;
			}
		}

		public override bool PageBreakAtStart
		{
			get
			{
				if (!((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PageBreakAtStart)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PropagatedPageBreakAtStart;
				}
				return true;
			}
		}

		public bool GroupBreakAtStart
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PropagatedPageBreakAtStart;
			}
		}

		public bool GroupBreakAtEnd
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).PropagatedPageBreakAtEnd;
			}
		}

		public ReportItem Corner
		{
			get
			{
				ReportItem reportItem = this.m_corner;
				if (this.m_corner == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.Matrix matrix = (AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef;
					if (matrix.CornerReportItems != null && 0 < matrix.CornerReportItems.Count)
					{
						AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef = matrix.CornerReportItems[0];
						ReportItemInstance reportItemInstance = null;
						NonComputedUniqueNames nonComputedUniqueNames = null;
						if (base.ReportItemInstance != null)
						{
							MatrixInstance matrixInstance = (MatrixInstance)base.ReportItemInstance;
							reportItemInstance = matrixInstance.CornerContent;
							nonComputedUniqueNames = ((MatrixInstanceInfo)base.InstanceInfo).CornerNonComputedNames;
						}
						reportItem = ReportItem.CreateItem(0, reportItemDef, reportItemInstance, base.RenderingContext, nonComputedUniqueNames);
						if (base.RenderingContext.CacheState)
						{
							this.m_corner = reportItem;
						}
					}
				}
				return reportItem;
			}
		}

		public MatrixCellCollection CellCollection
		{
			get
			{
				MatrixCellCollection matrixCellCollection = this.m_cells;
				if (this.m_cells == null)
				{
					int num = 0;
					int num2 = 0;
					if (!this.m_noRows && base.ReportItemInstance != null && 0 < ((MatrixInstance)base.ReportItemInstance).Cells.Count)
					{
						num = ((MatrixInstance)base.ReportItemInstance).CellRowCount;
						num2 = ((MatrixInstance)base.ReportItemInstance).CellColumnCount;
					}
					else
					{
						num = ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).MatrixRows.Count;
						num2 = ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).MatrixColumns.Count;
					}
					matrixCellCollection = new MatrixCellCollection(this, num, num2);
					if (base.RenderingContext.CacheState)
					{
						this.m_cells = matrixCellCollection;
					}
				}
				return matrixCellCollection;
			}
		}

		public MatrixMemberCollection ColumnMemberCollection
		{
			get
			{
				MatrixMemberCollection matrixMemberCollection = this.m_columns;
				if (this.m_columns == null)
				{
					matrixMemberCollection = new MatrixMemberCollection(this, null, ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).Columns, (base.ReportItemInstance == null) ? null : ((MatrixInstance)base.ReportItemInstance).ColumnInstances, this.m_colMemberMapping, false);
					if (base.RenderingContext.CacheState)
					{
						this.m_columns = matrixMemberCollection;
					}
				}
				return matrixMemberCollection;
			}
		}

		public MatrixMemberCollection RowMemberCollection
		{
			get
			{
				MatrixMemberCollection matrixMemberCollection = this.m_rows;
				if (this.m_rows == null)
				{
					matrixMemberCollection = new MatrixMemberCollection(this, null, ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).Rows, (base.ReportItemInstance == null) ? null : ((MatrixInstance)base.ReportItemInstance).RowInstances, this.m_rowMemberMapping, false);
					if (base.RenderingContext.CacheState)
					{
						this.m_rows = matrixMemberCollection;
					}
				}
				return matrixMemberCollection;
			}
		}

		public int CellColumns
		{
			get
			{
				if (!this.m_noRows && base.ReportItemInstance != null)
				{
					return ((MatrixInstance)base.ReportItemInstance).CellColumnCount;
				}
				return 0;
			}
		}

		public int CellRows
		{
			get
			{
				if (!this.m_noRows && base.ReportItemInstance != null)
				{
					return ((MatrixInstance)base.ReportItemInstance).CellRowCount;
				}
				return 0;
			}
		}

		public int MatrixPages
		{
			get
			{
				if (!this.m_noRows && base.ReportItemInstance != null)
				{
					return ((MatrixInstance)base.ReportItemInstance).InstanceCountOfInnerRowWithPageBreak;
				}
				return 0;
			}
		}

		public int PageBreakRow
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).InnerRowLevelWithPageBreak;
			}
		}

		public int Columns
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).ColumnCount;
			}
		}

		public int Rows
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).RowCount;
			}
		}

		public override bool NoRows
		{
			get
			{
				return this.m_noRows;
			}
		}

		public int GroupsBeforeRowHeaders
		{
			get
			{
				if (this.m_groupsBeforeRowHeaders < 0)
				{
					this.CalculateGroupsCellsBeforeRowHeaders();
				}
				return this.m_groupsBeforeRowHeaders;
			}
		}

		public int CellsBeforeRowHeaders
		{
			get
			{
				if (this.m_cellsBeforeRowHeaders < 0)
				{
					this.CalculateGroupsCellsBeforeRowHeaders();
				}
				return this.m_cellsBeforeRowHeaders;
			}
		}

		public SizeCollection CellWidths
		{
			get
			{
				SizeCollection sizeCollection = this.m_cellWidths;
				if (this.m_cellWidths == null)
				{
					sizeCollection = new SizeCollection(this, true);
					if (base.RenderingContext.CacheState)
					{
						this.m_cellWidths = sizeCollection;
					}
				}
				return sizeCollection;
			}
		}

		public SizeCollection CellHeights
		{
			get
			{
				SizeCollection sizeCollection = this.m_cellHeights;
				if (this.m_cellHeights == null)
				{
					sizeCollection = new SizeCollection(this, false);
					if (base.RenderingContext.CacheState)
					{
						this.m_cellHeights = sizeCollection;
					}
				}
				return sizeCollection;
			}
		}

		public bool UseOWC
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).UseOWC;
			}
		}

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((MatrixInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		public bool RowGroupingFixedHeader
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).RowGroupingFixedHeader;
			}
		}

		public bool ColumnGroupingFixedHeader
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).ColumnGroupingFixedHeader;
			}
		}

		internal Matrix(int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.Matrix reportItemDef, MatrixInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			if (reportItemInstance != null && reportItemInstance.Cells.Count != 0 && reportItemInstance.Cells[0].Count != 0)
			{
				this.m_rowMemberMapping = Matrix.CalculateMapping(reportItemDef.Rows, reportItemInstance.RowInstances, false);
				this.m_colMemberMapping = Matrix.CalculateMapping(reportItemDef.Columns, reportItemInstance.ColumnInstances, false);
				this.m_noRows = (this.m_rowMemberMapping.Count == 0 || 0 == this.m_colMemberMapping.Count);
			}
			else
			{
				this.m_noRows = true;
			}
		}

		public bool IsRowMemberOnThisPage(int groupIndex, int pageNumber, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			RenderingPagesRangesList childrenStartAndEndPages = ((MatrixInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
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

		public void GetRowMembersOnPage(int page, out int startMember, out int endMember)
		{
			startMember = -1;
			endMember = -1;
			if (base.ReportItemInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = ((MatrixInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startMember, ref endMember);
				}
			}
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (!base.SkipSearch && !this.NoRows)
			{
				IntList hiddenColumns = null;
				bool flag = Matrix.SearchMatrixColumns(this.ColumnMemberCollection, ref hiddenColumns, searchContext);
				if (!flag)
				{
					flag = Matrix.SearchMatrixRowsContent(this, null, searchContext, hiddenColumns);
				}
				return flag;
			}
			return false;
		}

		private static void BuildHiddenColumns(MatrixMember member, ref IntList hiddenColumns)
		{
			if (hiddenColumns == null)
			{
				hiddenColumns = new IntList();
			}
			for (int i = 0; i < member.ColumnSpan; i++)
			{
				hiddenColumns.Add(member.MemberCellIndex + i);
			}
		}

		private static bool HiddenColumn(IntList hiddenColumns, ref int columnIndex, int cellIndex)
		{
			bool result = false;
			if (hiddenColumns != null && columnIndex < hiddenColumns.Count)
			{
				while (columnIndex < hiddenColumns.Count && cellIndex > hiddenColumns[columnIndex])
				{
					columnIndex++;
				}
				if (cellIndex == hiddenColumns[columnIndex])
				{
					columnIndex++;
					result = true;
				}
			}
			return result;
		}

		private static bool SearchMatrixRowsContent(Matrix matrix, MatrixMember member, SearchContext searchContext, IntList hiddenColumns)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			MatrixMember matrixMember = null;
			MatrixMemberCollection matrixMemberCollection = null;
			MatrixMemberCollection matrixMemberCollection2 = null;
			SearchContext searchContext2 = new SearchContext(searchContext);
			bool flag2 = false;
			matrixMemberCollection2 = ((member != null) ? member.Children : matrix.RowMemberCollection);
			if (searchContext.ItemStartPage != searchContext.ItemEndPage)
			{
				if (member == null)
				{
					matrix.GetRowMembersOnPage(searchContext.SearchPage, out num, out num2);
				}
				else
				{
					member.GetChildRowMembersOnPage(searchContext.SearchPage, out num, out num2);
				}
				flag2 = true;
			}
			else
			{
				num = 0;
				num2 = matrixMemberCollection2.Count - 1;
			}
			num3 = num2 - num + 1;
			int num4 = num;
			while (!flag && num4 <= num2)
			{
				matrixMember = matrixMemberCollection2[num4];
				if (matrixMember.Hidden)
				{
					num3--;
				}
				else
				{
					matrixMemberCollection = matrixMember.Children;
					if (matrixMemberCollection != null)
					{
						flag = matrixMember.ReportItem.Search(searchContext2);
						if (!flag)
						{
							if (flag2 && (num4 == num || num4 == num2))
							{
								int itemStartPage = 0;
								int itemEndPage = 0;
								SearchContext searchContext3 = new SearchContext(searchContext);
								if (member == null)
								{
									matrix.IsRowMemberOnThisPage(num4, searchContext.SearchPage, out itemStartPage, out itemEndPage);
								}
								else
								{
									member.IsRowMemberOnThisPage(num4, searchContext.SearchPage, out itemStartPage, out itemEndPage);
								}
								searchContext3.ItemStartPage = itemStartPage;
								searchContext3.ItemEndPage = itemEndPage;
								flag = Matrix.SearchMatrixRowsContent(matrix, matrixMember, searchContext3, hiddenColumns);
							}
							else
							{
								flag = Matrix.SearchMatrixRowsContent(matrix, matrixMember, searchContext2, hiddenColumns);
							}
						}
					}
					else
					{
						flag = matrixMember.ReportItem.Search(searchContext2);
						if (!flag)
						{
							flag = Matrix.SearchRangeCells(matrix, matrixMember.MemberCellIndex, hiddenColumns, searchContext2);
						}
					}
				}
				num4++;
			}
			if (!flag && num3 == 0)
			{
				if (!matrixMember.IsTotal)
				{
					matrixMember = matrixMemberCollection2[0];
				}
				matrixMemberCollection = matrixMember.Children;
				flag = ((matrixMemberCollection == null) ? Matrix.SearchRangeCells(matrix, matrixMember.MemberCellIndex, hiddenColumns, searchContext2) : Matrix.SearchRowTotal(matrix, matrixMemberCollection, hiddenColumns, searchContext2));
			}
			return flag;
		}

		private static bool SearchMatrixColumns(MatrixMemberCollection columns, ref IntList hiddenColumns, SearchContext searchContext)
		{
			if (columns == null)
			{
				return false;
			}
			bool flag = false;
			int num = 0;
			int num2 = columns.Count - 1;
			int num3 = 0;
			MatrixMember matrixMember = null;
			MatrixMemberCollection matrixMemberCollection = null;
			SearchContext searchContext2 = new SearchContext(searchContext);
			int index = 0;
			int count = 0;
			num3 = num2 - num + 1;
			int num4 = num;
			while (!flag && num4 <= num2)
			{
				matrixMember = columns[num4];
				if (matrixMember.Hidden)
				{
					if (matrixMember.IsTotal)
					{
						if (hiddenColumns != null)
						{
							index = hiddenColumns.Count;
						}
						count = matrixMember.ColumnSpan;
					}
					Matrix.BuildHiddenColumns(matrixMember, ref hiddenColumns);
					num3--;
				}
				else
				{
					flag = matrixMember.ReportItem.Search(searchContext2);
					if (!flag)
					{
						matrixMemberCollection = matrixMember.Children;
						flag = Matrix.SearchMatrixColumns(matrixMemberCollection, ref hiddenColumns, searchContext2);
					}
				}
				num4++;
			}
			if (num3 == 0)
			{
				hiddenColumns.RemoveRange(index, count);
				if (!flag)
				{
					if (!matrixMember.IsTotal)
					{
						matrixMember = columns[0];
					}
					matrixMemberCollection = matrixMember.Children;
					if (matrixMemberCollection != null)
					{
						int num5 = 0;
						while (!flag && num5 < matrixMemberCollection.Count)
						{
							matrixMember = matrixMemberCollection[num5];
							flag = matrixMember.ReportItem.Search(searchContext2);
							num5++;
						}
					}
				}
			}
			return flag;
		}

		private static bool SearchRangeCells(Matrix matrix, int indexRow, IntList hiddenColumns, SearchContext searchContext)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			MatrixCellCollection cellCollection = matrix.CellCollection;
			num = 0;
			while (!flag && num < matrix.CellColumns)
			{
				if (!Matrix.HiddenColumn(hiddenColumns, ref num2, num))
				{
					flag = cellCollection[indexRow, num].ReportItem.Search(searchContext);
				}
				num++;
			}
			return flag;
		}

		private static bool SearchRowTotal(Matrix matrix, MatrixMemberCollection rowTotalChildren, IntList hiddenColumns, SearchContext searchContext)
		{
			bool flag = false;
			MatrixMember matrixMember = null;
			int num = 0;
			while (!flag && num < rowTotalChildren.Count)
			{
				matrixMember = rowTotalChildren[num];
				flag = matrixMember.ReportItem.Search(searchContext);
				if (!flag)
				{
					flag = Matrix.SearchRangeCells(matrix, matrixMember.MemberCellIndex, hiddenColumns, searchContext);
				}
				num++;
			}
			return flag;
		}

		internal static List<int> CalculateMapping(MatrixHeading headingDef, MatrixHeadingInstanceList headingInstances, bool inParentSubtotal)
		{
			List<int> list = new List<int>();
			if (headingInstances == null)
			{
				return list;
			}
			bool flag = true;
			for (int i = 0; i < headingInstances.Count; i++)
			{
				if (inParentSubtotal || headingInstances[i].IsSubtotal || !Matrix.IsEmpty(headingDef, headingInstances[i]))
				{
					if (!headingInstances[i].IsSubtotal)
					{
						flag = false;
					}
					list.Add(i);
				}
			}
			if (flag && list.Count <= 1)
			{
				list.Clear();
			}
			return list;
		}

		private static bool IsEmpty(MatrixHeading headingDef, MatrixHeadingInstance headingInstance)
		{
			if (headingDef != null && headingDef.SubHeading != null)
			{
				if (headingInstance.SubHeadingInstances != null && headingInstance.SubHeadingInstances.Count != 0)
				{
					int count = headingInstance.SubHeadingInstances.Count;
					bool flag = true;
					for (int i = 0; i < count; i++)
					{
						if (!flag)
						{
							break;
						}
						flag = Matrix.IsEmpty(headingDef.SubHeading, headingInstance.SubHeadingInstances[i]);
					}
					return flag;
				}
				return true;
			}
			return false;
		}

		private void CalculateGroupsCellsBeforeRowHeaders()
		{
			this.m_groupsBeforeRowHeaders = ((AspNetCore.ReportingServices.ReportProcessing.Matrix)base.ReportItemDef).GroupsBeforeRowHeaders;
			if (this.m_groupsBeforeRowHeaders > 0 && base.ReportItemInstance != null)
			{
				MatrixHeadingInstanceList columnInstances = ((MatrixInstance)base.ReportItemInstance).ColumnInstances;
				MatrixMemberCollection matrixMemberCollection = null;
				int num = -1;
				if (columnInstances != null && 0 < columnInstances.Count)
				{
					num = columnInstances.Count - 1;
					if (columnInstances[0].IsSubtotal || (this.m_groupsBeforeRowHeaders == num && columnInstances[num].IsSubtotal))
					{
						this.m_groupsBeforeRowHeaders++;
					}
				}
				this.m_cellsBeforeRowHeaders = 0;
				if (this.m_groupsBeforeRowHeaders > num + 1)
				{
					this.m_groupsBeforeRowHeaders = 0;
				}
				else
				{
					matrixMemberCollection = this.ColumnMemberCollection;
				}
				for (int i = 0; i < this.m_groupsBeforeRowHeaders; i++)
				{
					this.m_cellsBeforeRowHeaders += matrixMemberCollection[i].ColumnSpan;
				}
			}
			else
			{
				this.m_groupsBeforeRowHeaders = 0;
				this.m_cellsBeforeRowHeaders = 0;
			}
		}
	}
}
