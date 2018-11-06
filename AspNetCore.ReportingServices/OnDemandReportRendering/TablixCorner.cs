namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixCorner
	{
		private Tablix m_owner;

		private TablixCornerRowCollection m_rowCollection;

		public TablixCornerRowCollection RowCollection
		{
			get
			{
				if (this.m_rowCollection == null)
				{
					this.m_rowCollection = new TablixCornerRowCollection(this.m_owner);
				}
				return this.m_rowCollection;
			}
		}

		internal TablixCorner(Tablix owner)
		{
			this.m_owner = owner;
		}

		internal void ResetContext()
		{
			if (this.m_rowCollection != null)
			{
				this.m_rowCollection.ResetContext();
			}
		}

		internal void SetNewContext()
		{
			if (this.m_rowCollection != null)
			{
				this.m_rowCollection.SetNewContext();
			}
		}
	}
}
