using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PaginationInfo
	{
		private ArrayList m_pages;

		private int m_totalPageNumber;

		internal int TotalPageNumber
		{
			get
			{
				return this.m_totalPageNumber;
			}
			set
			{
				this.m_totalPageNumber = value;
			}
		}

		internal Page this[int pageNumber]
		{
			get
			{
				return (Page)this.m_pages[pageNumber];
			}
			set
			{
				this.m_pages[pageNumber] = value;
			}
		}

		internal int CurrentPageCount
		{
			get
			{
				return this.m_pages.Count;
			}
		}

		internal PaginationInfo()
		{
			this.m_pages = new ArrayList();
		}

		internal void AddPage(Page page)
		{
			this.m_pages.Add(page);
		}

		internal void Clear()
		{
			this.m_pages.Clear();
		}

		internal void InsertPage(int pageNumber, Page page)
		{
			this.m_pages.Insert(pageNumber, page);
		}

		internal void RemovePage(int pageNumber)
		{
			this.m_pages.RemoveAt(pageNumber);
		}
	}
}
