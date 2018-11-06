using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class TableHeaderFooterRows : TableRowCollection
	{
		private bool m_repeatOnNewPage;

		public bool RepeatOnNewPage
		{
			get
			{
				return this.m_repeatOnNewPage;
			}
		}

		internal TableHeaderFooterRows(Table owner, bool repeatOnNewPage, TableRowList rowDefs, TableRowInstance[] rowInstances)
			: base(owner, rowDefs, rowInstances)
		{
			this.m_repeatOnNewPage = repeatOnNewPage;
		}
	}
}
