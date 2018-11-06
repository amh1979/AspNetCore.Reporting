using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCornerCell : IDefinitionPath
	{
		private Tablix m_owner;

		private int m_rowIndex;

		private int m_columnIndex;

		private string m_definitionPath;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell m_cellDef;

		private AspNetCore.ReportingServices.ReportRendering.ReportItem m_cornerReportItem;

		private CellContents m_cellContents;

		public string DefinitionPath
		{
			get
			{
				if (this.m_definitionPath == null)
				{
					this.m_definitionPath = DefinitionPathConstants.GetTablixCellDefinitionPath(this.m_owner, this.m_rowIndex, this.m_columnIndex, false);
				}
				return this.m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath
		{
			get
			{
				return this.m_owner;
			}
		}

		public CellContents CellContents
		{
			get
			{
				if (this.m_cellContents == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						if (this.m_cornerReportItem != null)
						{
							int columns = this.m_owner.Columns;
							int rows = this.m_owner.Rows;
							this.m_cellContents = new CellContents(this, this.m_owner.InSubtotal, this.m_cornerReportItem, columns, rows, this.m_owner.RenderingContext);
						}
					}
					else
					{
						this.m_cellContents = new CellContents(this.m_owner.ReportScope, this, this.m_cellDef.CellContents, this.m_cellDef.RowSpan, this.m_cellDef.ColSpan, this.m_owner.RenderingContext);
					}
				}
				return this.m_cellContents;
			}
		}

		internal TablixCornerCell(Tablix owner, int rowIndex, int colIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell cellDef)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = colIndex;
			this.m_cellDef = cellDef;
		}

		internal TablixCornerCell(Tablix owner, int rowIndex, int colIndex, AspNetCore.ReportingServices.ReportRendering.ReportItem cornerReportItem)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = colIndex;
			this.m_cornerReportItem = cornerReportItem;
		}
	}
}
