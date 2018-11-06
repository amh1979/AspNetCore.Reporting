using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemInstance : ReportItemInstance, IPageItem
	{
		private ReportItemColInstance m_altReportItemColInstance;

		private CustomReportItemHeadingInstanceList m_columnInstances;

		private CustomReportItemHeadingInstanceList m_rowInstances;

		private CustomReportItemCellInstancesList m_cells;

		[NonSerialized]
		private int m_currentCellOuterIndex;

		[NonSerialized]
		private int m_currentCellInnerIndex;

		[NonSerialized]
		private int m_currentOuterStaticIndex;

		[NonSerialized]
		private int m_currentInnerStaticIndex;

		[NonSerialized]
		private CustomReportItemHeadingInstanceList m_innerHeadingInstanceList;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal ReportItemColInstance AltReportItemColInstance
		{
			get
			{
				return this.m_altReportItemColInstance;
			}
			set
			{
				this.m_altReportItemColInstance = value;
			}
		}

		internal CustomReportItemHeadingInstanceList ColumnInstances
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

		internal CustomReportItemHeadingInstanceList RowInstances
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

		internal CustomReportItemCellInstancesList Cells
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

		internal CustomReportItemHeadingInstanceList InnerHeadingInstanceList
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

		internal CustomReportItemInstance()
		{
		}

		internal CustomReportItemInstance(ReportProcessing.ProcessingContext pc, CustomReportItem reportItemDef)
			: base(pc.CreateUniqueName(), reportItemDef)
		{
			base.m_instanceInfo = new CustomReportItemInstanceInfo(pc, reportItemDef, this);
			pc.Pagination.EnterIgnoreHeight(reportItemDef.StartHidden);
			if (reportItemDef.DataSetName != null)
			{
				this.m_columnInstances = new CustomReportItemHeadingInstanceList();
				this.m_rowInstances = new CustomReportItemHeadingInstanceList();
				this.m_cells = new CustomReportItemCellInstancesList();
			}
		}

		internal CustomReportItemCellInstance AddCell(ReportProcessing.ProcessingContext pc)
		{
			CustomReportItem customReportItem = (CustomReportItem)base.m_reportItemDef;
			bool flag = customReportItem.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column;
			int rowIndex;
			int colIndex;
			if (flag)
			{
				rowIndex = this.m_currentOuterStaticIndex;
				colIndex = this.m_currentInnerStaticIndex;
			}
			else
			{
				colIndex = this.m_currentOuterStaticIndex;
				rowIndex = this.m_currentInnerStaticIndex;
			}
			CustomReportItemCellInstance customReportItemCellInstance = new CustomReportItemCellInstance(rowIndex, colIndex, customReportItem, pc);
			if (flag)
			{
				this.m_cells[this.m_currentCellOuterIndex].Add(customReportItemCellInstance);
			}
			else
			{
				if (this.m_currentCellOuterIndex == 0)
				{
					Global.Tracer.Assert(this.m_cells.Count == this.m_currentCellInnerIndex);
					CustomReportItemCellInstanceList value = new CustomReportItemCellInstanceList();
					this.m_cells.Add(value);
				}
				this.m_cells[this.m_currentCellInnerIndex].Add(customReportItemCellInstance);
			}
			this.m_currentCellInnerIndex++;
			return customReportItemCellInstance;
		}

		internal void NewOuterCells()
		{
			if (0 >= this.m_currentCellInnerIndex && this.m_cells.Count != 0)
			{
				return;
			}
			CustomReportItem customReportItem = (CustomReportItem)base.m_reportItemDef;
			if (customReportItem.ProcessingInnerGrouping == Pivot.ProcessingInnerGroupings.Column)
			{
				CustomReportItemCellInstanceList value = new CustomReportItemCellInstanceList();
				this.m_cells.Add(value);
			}
			if (0 < this.m_currentCellInnerIndex)
			{
				this.m_currentCellOuterIndex++;
				this.m_currentCellInnerIndex = 0;
			}
		}

		internal override ReportItemInstanceInfo ReadInstanceInfo(IntermediateFormatReader reader)
		{
			Global.Tracer.Assert(base.m_instanceInfo is OffsetInfo);
			return reader.ReadCustomReportItemInstanceInfo((CustomReportItem)base.m_reportItemDef);
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.AltReportItemColInstance, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemColInstance));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.RowInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.Cells, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.CustomReportItemCellInstancesList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstance, memberInfoList);
		}
	}
}
