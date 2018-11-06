using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class PageCollection
	{
		private PaginationInfo m_paginationDef;

		private Report m_report;

		public int TotalCount
		{
			get
			{
				return this.m_paginationDef.TotalPageNumber;
			}
			set
			{
				this.m_paginationDef.TotalPageNumber = value;
			}
		}

		public Page this[int pageNumber]
		{
			get
			{
				if (0 <= pageNumber && pageNumber < this.Count)
				{
					Page page = this.m_paginationDef[pageNumber];
					if (page != null && this.m_report != null)
					{
						if (page.PageSectionHeader == null)
						{
							page.PageSectionHeader = this.GetHeader(page.HeaderInstance);
						}
						if (page.PageSectionFooter == null)
						{
							page.PageSectionFooter = this.GetFooter(page.FooterInstance);
						}
					}
					return page;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, pageNumber, 0, this.Count);
			}
			set
			{
				if (0 <= pageNumber && pageNumber < this.Count)
				{
					this.m_paginationDef[pageNumber] = value;
					return;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, pageNumber, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.m_paginationDef.CurrentPageCount;
			}
		}

		internal PageCollection(PaginationInfo paginationDef, Report report)
		{
			this.m_paginationDef = paginationDef;
			this.m_report = report;
		}

		public void Add(Page page)
		{
			this.m_paginationDef.AddPage(page);
		}

		public void Clear()
		{
			this.m_paginationDef.Clear();
		}

		public void Insert(int index, Page page)
		{
			this.m_paginationDef.InsertPage(index, page);
		}

		public void RemoveAt(int index)
		{
			this.m_paginationDef.RemovePage(index);
		}

		internal PageSection GetHeader(PageSectionInstance headerInstance)
		{
			PageSection result = null;
			AspNetCore.ReportingServices.ReportProcessing.Report reportDef = this.m_report.ReportDef;
			if (reportDef != null)
			{
				if (!reportDef.PageHeaderEvaluation)
				{
					result = this.m_report.PageHeader;
				}
				else if (reportDef.PageHeader != null && headerInstance != null)
				{
					string text = headerInstance.PageNumber + "ph";
					RenderingContext renderingContext = new RenderingContext(this.m_report.RenderingContext, text);
					result = new PageSection(text, reportDef.PageHeader, headerInstance, this.m_report, renderingContext, false);
				}
			}
			return result;
		}

		internal PageSection GetFooter(PageSectionInstance footerInstance)
		{
			PageSection result = null;
			AspNetCore.ReportingServices.ReportProcessing.Report reportDef = this.m_report.ReportDef;
			if (reportDef != null)
			{
				if (!reportDef.PageFooterEvaluation)
				{
					result = this.m_report.PageFooter;
				}
				else if (reportDef.PageFooter != null && footerInstance != null)
				{
					string text = footerInstance.PageNumber + "pf";
					RenderingContext renderingContext = new RenderingContext(this.m_report.RenderingContext, text);
					result = new PageSection(text, reportDef.PageFooter, footerInstance, this.m_report, renderingContext, false);
				}
			}
			return result;
		}
	}
}
