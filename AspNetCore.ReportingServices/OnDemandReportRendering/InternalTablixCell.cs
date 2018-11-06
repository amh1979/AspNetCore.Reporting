using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalTablixCell : TablixCell
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCell m_cellDef;

		public override string ID
		{
			get
			{
				return this.m_cellDef.RenderingModelID;
			}
		}

		public override string DataElementName
		{
			get
			{
				return this.m_cellDef.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_cellDef.DataElementOutput;
			}
		}

		public override CellContents CellContents
		{
			get
			{
				if (base.m_cellContents == null)
				{
					base.m_cellContents = new CellContents(this, this, this.m_cellDef.CellContents, this.m_cellDef.RowSpan, this.m_cellDef.ColSpan, base.m_owner.RenderingContext);
				}
				return base.m_cellContents;
			}
		}

		internal InternalTablixCell(Tablix owner, int rowIndex, int colIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCell cellDef)
			: base(cellDef, owner, rowIndex, colIndex)
		{
			this.m_cellDef = cellDef;
		}
	}
}
