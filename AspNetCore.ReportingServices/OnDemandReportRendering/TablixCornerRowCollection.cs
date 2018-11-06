using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCornerRowCollection : ReportElementCollectionBase<TablixCornerRow>
	{
		private Tablix m_owner;

		private TablixCornerRow[] m_cornerRows;

		public override TablixCornerRow this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					if (this.m_cornerRows == null)
					{
						this.m_cornerRows = new TablixCornerRow[this.Count];
					}
					TablixCornerRow tablixCornerRow = this.m_cornerRows[index];
					if (tablixCornerRow == null)
					{
						tablixCornerRow = ((!this.m_owner.IsOldSnapshot) ? (this.m_cornerRows[index] = new TablixCornerRow(this.m_owner, index, this.m_owner.TablixDef.Corner[index])) : (this.m_cornerRows[index] = new TablixCornerRow(this.m_owner, index, this.m_owner.RenderMatrix.Corner)));
					}
					return tablixCornerRow;
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
						return this.m_owner.Columns;
					}
					return 0;
				}
				if (this.m_owner.TablixDef.Corner != null)
				{
					return this.m_owner.TablixDef.Corner.Count;
				}
				return 0;
			}
		}

		internal TablixCornerRowCollection(Tablix owner)
		{
			this.m_owner = owner;
		}

		internal void ResetContext()
		{
			if (this.m_owner.IsOldSnapshot && 0 < this.Count && this.m_cornerRows != null && this.m_cornerRows[0] != null)
			{
				this.m_cornerRows[0].UpdateRenderReportItem(this.m_owner.RenderMatrix.Corner);
			}
		}

		internal void SetNewContext()
		{
			if (!this.m_owner.IsOldSnapshot && this.m_cornerRows != null)
			{
				for (int i = 0; i < this.m_cornerRows.Length; i++)
				{
					TablixCornerRow tablixCornerRow = this.m_cornerRows[i];
					if (tablixCornerRow != null)
					{
						tablixCornerRow.SetNewContext();
					}
				}
			}
		}
	}
}
