namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class SearchContext
	{
		private int m_searchPage = -1;

		private int m_itemStartPage = -1;

		private int m_itemEndPage = -1;

		private string m_findValue;

		internal int SearchPage
		{
			get
			{
				return this.m_searchPage;
			}
		}

		internal string FindValue
		{
			get
			{
				return this.m_findValue;
			}
		}

		internal int ItemStartPage
		{
			get
			{
				return this.m_itemStartPage;
			}
			set
			{
				this.m_itemStartPage = value;
			}
		}

		internal int ItemEndPage
		{
			get
			{
				return this.m_itemEndPage;
			}
			set
			{
				this.m_itemEndPage = value;
			}
		}

		internal bool IsItemOnSearchPage
		{
			get
			{
				if (this.m_itemStartPage <= this.m_searchPage && this.m_searchPage <= this.m_itemEndPage)
				{
					return true;
				}
				return false;
			}
		}

		internal SearchContext(int searchPage, string findValue, int itemStartPage, int itemEndPage)
		{
			this.m_searchPage = searchPage;
			this.m_findValue = findValue;
			this.m_itemStartPage = itemStartPage;
			this.m_itemEndPage = itemEndPage;
		}

		internal SearchContext(SearchContext copy)
		{
			this.m_searchPage = copy.SearchPage;
			this.m_findValue = copy.FindValue;
		}
	}
}
