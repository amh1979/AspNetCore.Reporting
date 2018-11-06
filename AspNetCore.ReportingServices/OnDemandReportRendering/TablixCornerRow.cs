using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCornerRow : ReportElementCollectionBase<TablixCornerCell>
	{
		private Tablix m_owner;

		private int m_rowIndex;

		private List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell> m_rowDef;

		private AspNetCore.ReportingServices.ReportRendering.ReportItem m_cornerDef;

		private TablixCornerCell[] m_cellROMDefs;

		public override TablixCornerCell this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_cellROMDefs[index] == null)
					{
						if (this.m_owner.IsOldSnapshot)
						{
							if (this.m_rowIndex == 0 && index == 0)
							{
								this.m_cellROMDefs[index] = new TablixCornerCell(this.m_owner, this.m_rowIndex, index, this.m_cornerDef);
							}
						}
						else
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell tablixCornerCell = this.m_rowDef[index];
							if (tablixCornerCell.RowSpan > 0 && tablixCornerCell.ColSpan > 0)
							{
								this.m_cellROMDefs[index] = new TablixCornerCell(this.m_owner, this.m_rowIndex, index, tablixCornerCell);
							}
						}
					}
					return this.m_cellROMDefs[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				if (this.m_owner.IsOldSnapshot)
				{
					if (DataRegion.Type.Matrix == this.m_owner.SnapshotTablixType && this.m_owner.RenderMatrix.Corner != null)
					{
						return this.m_owner.Rows;
					}
					return 0;
				}
				return this.m_rowDef.Count;
			}
		}

		internal TablixCornerRow(Tablix owner, int rowIndex, List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell> rowDef)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_rowDef = rowDef;
			this.m_cellROMDefs = new TablixCornerCell[rowDef.Count];
		}

		internal TablixCornerRow(Tablix owner, int rowIndex, AspNetCore.ReportingServices.ReportRendering.ReportItem cornerDef)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_cornerDef = cornerDef;
			this.m_cellROMDefs = new TablixCornerCell[this.m_owner.Rows];
		}

		internal void SetNewContext()
		{
			if (!this.m_owner.IsOldSnapshot)
			{
				for (int i = 0; i < this.m_cellROMDefs.Length; i++)
				{
					TablixCornerCell tablixCornerCell = this.m_cellROMDefs[i];
					if (tablixCornerCell != null)
					{
						tablixCornerCell.CellContents.SetNewContext();
					}
				}
			}
		}

		internal void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem cornerDef)
		{
			this.m_cornerDef = cornerDef;
		}
	}
}
