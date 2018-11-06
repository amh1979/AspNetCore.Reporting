namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLHeaderFooterPropsDef : RPLItemPropsDef
	{
		private bool m_printOnFirstPage;

		private bool m_printBetweenSections;

		public bool PrintOnFirstPage
		{
			get
			{
				return this.m_printOnFirstPage;
			}
			set
			{
				this.m_printOnFirstPage = value;
			}
		}

		public bool PrintBetweenSections
		{
			get
			{
				return this.m_printBetweenSections;
			}
			set
			{
				this.m_printBetweenSections = value;
			}
		}

		internal RPLHeaderFooterPropsDef()
		{
		}
	}
}
