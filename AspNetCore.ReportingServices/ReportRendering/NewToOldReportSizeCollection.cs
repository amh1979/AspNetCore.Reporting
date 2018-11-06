namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class NewToOldReportSizeCollection : ReportSizeCollection
	{
		private ReportSizeCollection m_col;

		public override ReportSize this[int index]
		{
			get
			{
				return new ReportSize(this.m_col[index]);
			}
			set
			{
				this.m_col[index] = new ReportSize(value);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_col.Count;
			}
		}

		internal NewToOldReportSizeCollection(ReportSizeCollection col)
		{
			this.m_col = col;
		}
	}
}
