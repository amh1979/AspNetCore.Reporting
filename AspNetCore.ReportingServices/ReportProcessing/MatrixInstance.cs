using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixInstance : ReportItemInstance, IShowHideContainer, IPageItem
	{
		private ReportItemInstance m_cornerContent;

		private MatrixHeadingInstanceList m_columnInstances;

		private MatrixHeadingInstanceList m_rowInstances;

		private MatrixCellInstancesList m_cells;

		private int m_instanceCountOfInnerRowWithPageBreak;

		private RenderingPagesRangesList m_renderingPages;

		[NonSerialized]
		private int m_currentCellOuterIndex;

		[NonSerialized]
		private int m_currentCellInnerIndex;

		[NonSerialized]
		private int m_currentOuterStaticIndex;

		[NonSerialized]
		private int m_currentInnerStaticIndex;

		[NonSerialized]
		private MatrixHeadingInstanceList m_innerHeadingInstanceList;

		[NonSerialized]
		private bool m_inFirstPage;

		[NonSerialized]
		private int m_extraPagesFilled;

		[NonSerialized]
		private int m_numberOfChildrenOnThisPage;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ReportItemInstance CornerContent
		{
			get
			{
				return this.m_cornerContent;
			}
			set
			{
				this.m_cornerContent = value;
			}
		}

		internal MatrixHeadingInstanceList ColumnInstances
		{
			get
			{
				return this.m_columnInstances;
			}
			set
			{
				this.m_columnInstances = value;
			}
		}

		internal MatrixHeadingInstanceList RowInstances
		{
			get
			{
				return this.m_rowInstances;
			}
			set
			{
				this.m_rowInstances = value;
			}
		}

		internal MatrixCellInstancesList Cells
		{
			get
			{
				return this.m_cells;
			}
			set
			{
				this.m_cells = value;
			}
		}

		internal int CellColumnCount
		{
			get
			{
				if (0 < this.m_cells.Count)
				{
					return this.m_cells[0].Count;
				}
				return 0;
			}
		}

		internal int CellRowCount
		{
			get
			{
				return this.m_cells.Count;
			}
		}

		internal int InstanceCountOfInnerRowWithPageBreak
		{
			get
			{
				return this.m_instanceCountOfInnerRowWithPageBreak;
			}
			set
			{
				this.m_instanceCountOfInnerRowWithPageBreak = value;
			}
		}

		internal RenderingPagesRangesList ChildrenStartAndEndPages
		{
			get
			{
				return this.m_renderingPages;
			}
			set
			{
				this.m_renderingPages = value;
			}
		}

		internal int CurrentCellOuterIndex
		{
			get
			{
				return this.m_currentCellOuterIndex;
			}
		}

		internal int CurrentCellInnerIndex
		{
			get
			{
				return this.m_currentCellInnerIndex;
			}
		}

		internal int CurrentOuterStaticIndex
		{
			set
			{
				this.m_currentOuterStaticIndex = value;
			}
		}

		internal int CurrentInnerStaticIndex
		{
			set
			{
				this.m_currentInnerStaticIndex = value;
			}
		}

		internal MatrixHeadingInstanceList InnerHeadingInstanceList
		{
			get
			{
				return this.m_innerHeadingInstanceList;
			}
			set
			{
				this.m_innerHeadingInstanceList = value;
			}
		}

		internal bool InFirstPage
		{
			get
			{
				return this.m_inFirstPage;
			}
			set
			{
				this.m_inFirstPage = value;
			}
		}

		internal int ExtraPagesFilled
		{
			get
			{
				if (this.m_extraPagesFilled < 1)
				{
					return 0;
				}
				if (this.m_numberOfChildrenOnThisPage > 1)
				{
					return this.m_extraPagesFilled;
				}
				return this.m_extraPagesFilled - 1;
			}
			set
			{
				this.m_extraPagesFilled = value;
			}
		}

		internal int NumberOfChildrenOnThisPage
		{
			get
			{
				return this.m_numberOfChildrenOnThisPage;
			}
			set
			{
				this.m_numberOfChildrenOnThisPage = value;
			}
		}

		int IPageItem.StartPage
		{
			get
			{
				return this.m_startPage;
			}
			set
			{
				this.m_startPage = value;
			}
		}

		int IPageItem.EndPage
		{
			get
			{
				return this.m_endPage;
			}
			set
			{
				this.m_endPage = value;
			}
		}

		internal MatrixInstanceInfo InstanceInfo
		{
			get
			{
				if (base.m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(false, string.Empty);
					return null;
				}
				return (MatrixInstanceInfo)base.m_instanceInfo;
			}
		}

		internal Matrix MatrixDef
		{
			get
			{
				return base.m_reportItemDef as Matrix;
			}
		}

		internal MatrixInstance(ReportProcessing.ProcessingContext pc, Matrix reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new MatrixInstanceInfo(pc, reportItemDef, this);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			this.m_columnInstances = new MatrixHeadingInstanceList();
			this.m_rowInstances = new MatrixHeadingInstanceList();
			this.m_cells = new MatrixCellInstancesList();
			this.m_renderingPages = new RenderingPagesRangesList();
			reportItemDef.CurrentPage = reportItemDef.StartPage;
			this.m_startPage = reportItemDef.StartPage;
			if (reportItemDef.FirstCellInstances == null)
			{
				int count = reportItemDef.CellReportItems.Count;
				reportItemDef.FirstCellInstances = new BoolList(count);
				for (int i = 0; i < count; i++)
				{
					reportItemDef.FirstCellInstances.Add(true);
				}
			}
			this.m_inFirstPage = pc.ChunkManager.InFirstPage;
		}

		internal MatrixInstance()
		{
		}

		protected override object SearchChildren(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			object obj = null;
			Matrix matrix = (Matrix)base.ReportItemDef;
			if (matrix.CornerReportItems.Count > 0)
			{
				if (this.m_cornerContent != null)
				{
					obj = ((ISearchByUniqueName)this.m_cornerContent).Find(targetUniqueName, ref nonCompNames, chunkManager);
					if (obj != null)
					{
						return obj;
					}
				}
				else
				{
					NonComputedUniqueNames cornerNonComputedNames = ((MatrixInstanceInfo)base.GetInstanceInfo(chunkManager, false)).CornerNonComputedNames;
					obj = ((ISearchByUniqueName)matrix.CornerReportItems[0]).Find(targetUniqueName, ref cornerNonComputedNames, chunkManager);
					if (obj != null)
					{
						nonCompNames = cornerNonComputedNames;
						return obj;
					}
				}
			}
			obj = ((ISearchByUniqueName)this.m_columnInstances).Find(targetUniqueName, ref nonCompNames, chunkManager);
			if (obj != null)
			{
				return obj;
			}
			obj = ((ISearchByUniqueName)this.m_rowInstances).Find(targetUniqueName, ref nonCompNames, chunkManager);
			if (obj != null)
			{
				return obj;
			}
			int count = this.m_cells.Count;
			for (int i = 0; i < count; i++)
			{
				MatrixCellInstanceList matrixCellInstanceList = this.m_cells[i];
				int count2 = matrixCellInstanceList.Count;
				for (int j = 0; j < count2; j++)
				{
					MatrixCellInstance matrixCellInstance = matrixCellInstanceList[j];
					MatrixCellInstanceInfo instanceInfo = matrixCellInstance.GetInstanceInfo(chunkManager);
					int index = instanceInfo.RowIndex * matrix.MatrixColumns.Count + instanceInfo.ColumnIndex;
					if (matrix.CellReportItems.IsReportItemComputed(index))
					{
						if (matrixCellInstance.Content != null)
						{
							obj = ((ISearchByUniqueName)matrixCellInstance.Content).Find(targetUniqueName, ref nonCompNames, chunkManager);
							if (obj != null)
							{
								return obj;
							}
						}
					}
					else
					{
						NonComputedUniqueNames contentUniqueNames = instanceInfo.ContentUniqueNames;
						obj = ((ISearchByUniqueName)matrix.CellReportItems[index]).Find(targetUniqueName, ref contentUniqueNames, chunkManager);
						if (obj != null)
						{
							nonCompNames = contentUniqueNames;
							return obj;
						}
					}
				}
			}
			return null;
		}

		void IShowHideContainer.BeginProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.BeginProcessContainer(base.m_uniqueName, base.m_reportItemDef.Visibility);
		}

		void IShowHideContainer.EndProcessContainer(ReportProcessing.ProcessingContext context)
		{
			context.EndProcessContainer(base.m_uniqueName, base.m_reportItemDef.Visibility);
		}

		internal ReportItem GetCellReportItemDef(int cellRIIndex, out bool computed)
		{
			if (-1 == cellRIIndex)
			{
				cellRIIndex = this.GetCurrentCellRIIndex();
			}
			computed = false;
			int num = default(int);
			ReportItem result = default(ReportItem);
			((Matrix)base.m_reportItemDef).CellReportItems.GetReportItem(cellRIIndex, out computed, out num, out result);
			return result;
		}

		internal MatrixCellInstance AddCell(ReportProcessing.ProcessingContext pc, out NonComputedUniqueNames cellNonComputedUniqueNames)
		{
			Matrix matrix = (Matrix)base.m_reportItemDef;
			int currentCellRIIndex = this.GetCurrentCellRIIndex();
			bool flag = matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column;
			int num;
			int colIndex;
			if (flag)
			{
				num = this.m_currentOuterStaticIndex;
				colIndex = this.m_currentInnerStaticIndex;
			}
			else
			{
				colIndex = this.m_currentOuterStaticIndex;
				num = this.m_currentInnerStaticIndex;
			}
			MatrixCellInstance matrixCellInstance = null;
			matrixCellInstance = ((pc.HeadingInstance == null || pc.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass == null) ? new MatrixCellInstance(num, colIndex, matrix, currentCellRIIndex, pc, out cellNonComputedUniqueNames) : new MatrixSubtotalCellInstance(num, colIndex, matrix, currentCellRIIndex, pc, out cellNonComputedUniqueNames));
			if (!flag && this.m_currentCellOuterIndex == 0)
			{
				goto IL_009c;
			}
			if (flag && this.m_currentCellInnerIndex == 0)
			{
				goto IL_009c;
			}
			goto IL_0146;
			IL_0146:
			if (matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				this.m_cells[this.m_currentCellOuterIndex].Add(matrixCellInstance);
			}
			else
			{
				if (this.m_currentCellOuterIndex == 0)
				{
					Global.Tracer.Assert(this.m_cells.Count == this.m_currentCellInnerIndex);
					MatrixCellInstanceList value = new MatrixCellInstanceList();
					this.m_cells.Add(value);
				}
				this.m_cells[this.m_currentCellInnerIndex].Add(matrixCellInstance);
			}
			this.m_currentCellInnerIndex++;
			return matrixCellInstance;
			IL_009c:
			if (!pc.Pagination.IgnoreHeight)
			{
				pc.Pagination.AddToCurrentPageHeight(matrix, matrix.MatrixRows[num].HeightValue);
			}
			if (!pc.Pagination.IgnorePageBreak && pc.Pagination.CurrentPageHeight >= pc.Pagination.PageHeight && this.m_rowInstances.Count > 1)
			{
				pc.Pagination.SetCurrentPageHeight(matrix, 0.0);
				this.m_extraPagesFilled++;
				matrix.CurrentPage = this.m_startPage + this.m_extraPagesFilled;
				this.m_numberOfChildrenOnThisPage = 0;
			}
			else
			{
				this.m_numberOfChildrenOnThisPage++;
			}
			goto IL_0146;
		}

		internal void NewOuterCells()
		{
			if (0 >= this.m_currentCellInnerIndex && this.m_cells.Count != 0)
			{
				return;
			}
			Matrix matrix = (Matrix)base.m_reportItemDef;
			if (matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				MatrixCellInstanceList value = new MatrixCellInstanceList();
				this.m_cells.Add(value);
			}
			if (0 < this.m_currentCellInnerIndex)
			{
				this.m_currentCellOuterIndex++;
				this.m_currentCellInnerIndex = 0;
			}
		}

		internal int GetCurrentCellRIIndex()
		{
			Matrix matrix = (Matrix)base.m_reportItemDef;
			int count = matrix.MatrixColumns.Count;
			if (matrix.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				return this.m_currentOuterStaticIndex * count + this.m_currentInnerStaticIndex;
			}
			return this.m_currentInnerStaticIndex * count + this.m_currentOuterStaticIndex;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.CornerContent, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.RowInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.Cells, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.MatrixCellInstancesList));
			memberInfoList.Add(new MemberInfo(MemberName.InstanceCountOfInnerRowWithPageBreak, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenStartAndEndPages, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RenderingPagesRangesList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			return reader.ReadMatrixInstanceInfo((Matrix)base.m_reportItemDef);
		}
	}
}
