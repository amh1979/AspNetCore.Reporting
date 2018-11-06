using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixColumn
	{
		private Tablix m_owner;

		private int m_columnIndex;

		private ReportSize m_width;

		public ReportSize Width
		{
			get
			{
				if (this.m_width == null)
				{
					if (this.m_owner.IsOldSnapshot)
					{
						switch (this.m_owner.SnapshotTablixType)
						{
						case DataRegion.Type.List:
							this.m_width = new ReportSize(this.m_owner.RenderList.Width);
							break;
						case DataRegion.Type.Table:
							this.m_width = new ReportSize(this.m_owner.RenderTable.Columns[this.m_columnIndex].Width);
							break;
						case DataRegion.Type.Matrix:
						{
							int index = this.m_owner.MatrixColDefinitionMapping[this.m_columnIndex];
							this.m_width = new ReportSize(this.m_owner.RenderMatrix.CellWidths[index]);
							break;
						}
						}
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.TablixColumn tablixColumn = this.m_owner.TablixDef.TablixColumns[this.m_columnIndex];
						this.m_width = new ReportSize(tablixColumn.Width, tablixColumn.WidthValue);
					}
				}
				return this.m_width;
			}
		}

		internal TablixColumn(Tablix owner, int columnIndex)
		{
			this.m_owner = owner;
			this.m_columnIndex = columnIndex;
		}
	}
}
