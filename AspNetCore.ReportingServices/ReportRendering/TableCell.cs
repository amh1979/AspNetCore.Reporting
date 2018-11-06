using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableCell
	{
		private AspNetCore.ReportingServices.ReportProcessing.Table m_tableDef;

		private int m_index;

		private TableCellCollection m_cells;

		public ReportItem ReportItem
		{
			get
			{
				ReportItemCollection reportItems = this.m_cells.ReportItems;
				if (reportItems == null)
				{
					return null;
				}
				return reportItems[this.m_index];
			}
		}

		public int ColSpan
		{
			get
			{
				return this.m_cells.ColSpans[this.m_index];
			}
		}

		public string ID
		{
			get
			{
				string[] array = this.m_cells.RowDef.RenderingModelIDs;
				if (array == null)
				{
					array = new string[this.m_cells.RowDef.IDs.Count];
					this.m_cells.RowDef.RenderingModelIDs = array;
				}
				if (array[this.m_index] == null)
				{
					array[this.m_index] = this.m_cells.RowDef.IDs[this.m_index].ToString(CultureInfo.InvariantCulture);
				}
				return array[this.m_index];
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				int num = this.m_cells.RowDef.IDs[this.m_index];
				return this.m_cells.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num];
			}
			set
			{
				int num = this.m_cells.RowDef.IDs[this.m_index];
				this.m_cells.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num] = value;
			}
		}

		internal TableCell(AspNetCore.ReportingServices.ReportProcessing.Table tableDef, int index, TableCellCollection cells)
		{
			this.m_tableDef = tableDef;
			this.m_index = index;
			this.m_cells = cells;
		}
	}
}
