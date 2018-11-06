using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportSection : IDefinitionPath, IReportScope
	{
		private AspNetCore.ReportingServices.ReportRendering.Report m_renderReport;

		private Report m_reportDef;

		private int m_sectionIndex;

		private ReportSectionInstance m_instance;

		private Page m_page;

		private Body m_body;

		private string m_definitionPath;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection m_sectionDef;

		private ReportItemsImpl m_bodyItemsForHeadFoot;

		private ReportItemsImpl m_pageSectionItemsForHeadFoot;

		private Dictionary<string, AggregatesImpl> m_pageAggregatesOverReportItems = new Dictionary<string, AggregatesImpl>();

		public string Name
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return "ReportSection0";
				}
				return this.m_sectionDef.Name;
			}
		}

		public Body Body
		{
			get
			{
				if (this.m_body == null)
				{
					if (this.IsOldSnapshot)
					{
						this.m_body = new Body(this, this.m_reportDef.SubreportInSubtotal, this.m_renderReport, this.m_reportDef.RenderingContext);
					}
					else
					{
						this.m_body = new Body(this, this, this.m_sectionDef, this.m_reportDef.RenderingContext);
					}
				}
				return this.m_body;
			}
		}

		public ReportSize Width
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.Width);
				}
				if (this.m_sectionDef.WidthForRendering == null)
				{
					this.m_sectionDef.WidthForRendering = new ReportSize(this.m_sectionDef.Width, this.m_sectionDef.WidthValue);
				}
				return this.m_sectionDef.WidthForRendering;
			}
		}

		public Page Page
		{
			get
			{
				if (this.m_page == null)
				{
					if (this.m_reportDef.IsOldSnapshot)
					{
						this.m_page = new Page(this, this.m_reportDef.RenderReport, this.m_reportDef.RenderingContext, this);
					}
					else
					{
						this.m_page = new Page(this, this.m_reportDef.RenderingContext, this);
					}
				}
				return this.m_page;
			}
		}

		public string DataElementName
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return string.Empty;
				}
				return this.m_sectionDef.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return DataElementOutputTypes.ContentsOnly;
				}
				return this.m_sectionDef.DataElementOutput;
			}
		}

		public ReportSectionInstance Instance
		{
			get
			{
				if (this.m_reportDef.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ReportSectionInstance(this);
				}
				return this.m_instance;
			}
		}

		public string ID
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderReport.Body.ID + "xE";
				}
				return this.m_sectionDef.RenderingModelID;
			}
		}

		internal bool IsOldSnapshot
		{
			get
			{
				return this.m_reportDef.IsOldSnapshot;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection SectionDef
		{
			get
			{
				return this.m_sectionDef;
			}
		}

		internal ReportItemsImpl BodyItemsForHeadFoot
		{
			get
			{
				return this.m_bodyItemsForHeadFoot;
			}
			set
			{
				this.m_bodyItemsForHeadFoot = value;
			}
		}

		internal ReportItemsImpl PageSectionItemsForHeadFoot
		{
			get
			{
				return this.m_pageSectionItemsForHeadFoot;
			}
			set
			{
				this.m_pageSectionItemsForHeadFoot = value;
			}
		}

		internal Dictionary<string, AggregatesImpl> PageAggregatesOverReportItems
		{
			get
			{
				return this.m_pageAggregatesOverReportItems;
			}
			set
			{
				this.m_pageAggregatesOverReportItems = value;
			}
		}

		internal Report Report
		{
			get
			{
				return this.m_reportDef;
			}
		}

		internal int SectionIndex
		{
			get
			{
				return this.m_sectionIndex;
			}
		}

		public bool NeedsTotalPages
		{
			get
			{
				if (!this.NeedsOverallTotalPages)
				{
					return this.NeedsPageBreakTotalPages;
				}
				return true;
			}
		}

		public bool NeedsOverallTotalPages
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderReport.NeedsHeaderFooterEvaluation;
				}
				return this.m_sectionDef.NeedsOverallTotalPages;
			}
		}

		public bool NeedsPageBreakTotalPages
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return false;
				}
				return this.m_sectionDef.NeedsPageBreakTotalPages;
			}
		}

		public bool NeedsReportItemsOnPage
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderReport.NeedsHeaderFooterEvaluation;
				}
				return this.m_sectionDef.NeedsReportItemsOnPage;
			}
		}

		public string DefinitionPath
		{
			get
			{
				return this.m_definitionPath;
			}
		}

		public IDefinitionPath ParentDefinitionPath
		{
			get
			{
				return this.m_reportDef;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance
		{
			get
			{
				return this.Instance;
			}
		}

		IRIFReportScope IReportScope.RIFReportScope
		{
			get
			{
				return this.m_sectionDef;
			}
		}

		internal ReportSection(Report reportDef, AspNetCore.ReportingServices.ReportRendering.Report renderReport, int indexInCollection)
			: this(reportDef, indexInCollection)
		{
			this.m_renderReport = renderReport;
		}

		internal ReportSection(Report reportDef, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef, int indexInCollection)
			: this(reportDef, indexInCollection)
		{
			this.m_sectionDef = sectionDef;
		}

		private ReportSection(Report reportDef, int indexInCollection)
		{
			this.m_reportDef = reportDef;
			this.m_sectionIndex = indexInCollection;
			this.m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(reportDef, indexInCollection);
		}

		internal void UpdateSubReportContents(AspNetCore.ReportingServices.ReportRendering.Report newRenderSubreport)
		{
			this.m_renderReport = newRenderSubreport;
			if (this.m_body != null)
			{
				this.m_body.UpdateSubReportContents(this.m_renderReport);
			}
			if (this.m_page != null)
			{
				this.m_page.UpdateSubReportContents(this.m_renderReport);
			}
		}

		internal void SetNewContext()
		{
			if (this.m_body != null)
			{
				this.m_body.SetNewContext();
			}
			if (this.m_page != null)
			{
				this.m_page.SetNewContext();
			}
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}

		public void GetPageSections()
		{
			PageEvaluation pageEvaluation = this.m_reportDef.PageEvaluation;
			if (pageEvaluation != null)
			{
				pageEvaluation.UpdatePageSections(this);
			}
		}

		public void SetPage(int pageNumber, int totalPages, int overallPageNumber, int overallTotalPages)
		{
			PageEvaluation pageEvaluation = this.m_reportDef.PageEvaluation;
			if (pageEvaluation != null)
			{
				pageEvaluation.Reset(this, pageNumber, totalPages, overallPageNumber, overallTotalPages);
			}
		}

		public void SetPage(int pageNumber, int totalPages)
		{
			this.SetPage(pageNumber, totalPages, pageNumber, totalPages);
		}

		public void SetPageName(string pageName)
		{
			PageEvaluation pageEvaluation = this.m_reportDef.PageEvaluation;
			if (pageEvaluation != null)
			{
				pageEvaluation.SetPageName(pageName);
			}
		}
	}
}
