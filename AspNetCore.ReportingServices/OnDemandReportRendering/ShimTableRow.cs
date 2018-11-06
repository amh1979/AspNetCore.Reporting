using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimTableRow : TablixRow
	{
		private List<ShimTableCell> m_cells;

		private ReportSize m_height;

		private int[] m_rowCellDefinitionMapping;

		public override TablixCell this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_rowCellDefinitionMapping[index] < 0)
					{
						return null;
					}
					return this.m_cells[this.m_rowCellDefinitionMapping[index]];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_rowCellDefinitionMapping.Length;
			}
		}

		public override ReportSize Height
		{
			get
			{
				return this.m_height;
			}
		}

		internal ShimTableRow(Tablix owner, int rowIndex, AspNetCore.ReportingServices.ReportRendering.TableRow renderRow)
			: base(owner, rowIndex)
		{
			this.m_cells = new List<ShimTableCell>();
			this.m_height = new ReportSize(renderRow.Height);
			TableCellCollection tableCellCollection = renderRow.TableCellCollection;
			if (tableCellCollection != null)
			{
				int count = tableCellCollection.Count;
				this.m_rowCellDefinitionMapping = new int[owner.RenderTable.Columns.Count];
				int num = 0;
				for (int i = 0; i < count; i++)
				{
					int colSpan = tableCellCollection[i].ColSpan;
					for (int j = 0; j < colSpan; j++)
					{
						this.m_rowCellDefinitionMapping[num] = ((j == 0) ? i : (-1));
						num++;
					}
					this.m_cells.Add(new ShimTableCell(owner, rowIndex, i, colSpan, tableCellCollection[i].ReportItem));
				}
			}
		}

		internal void UpdateCells(AspNetCore.ReportingServices.ReportRendering.TableRow renderRow)
		{
			int count = this.m_cells.Count;
			TableCellCollection tableCellCollection = (renderRow != null) ? renderRow.TableCellCollection : null;
			for (int i = 0; i < count; i++)
			{
				this.m_cells[i].SetCellContents((tableCellCollection != null) ? tableCellCollection[i] : null);
			}
		}
	}
}
