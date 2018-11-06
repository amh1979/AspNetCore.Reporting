using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageSection : ReportElement
	{
		internal const string PageHeaderUniqueNamePrefix = "ph";

		internal const string PageFooterUniqueNamePrefix = "pf";

		private bool m_isHeader;

		private ReportSize m_height;

		private ReportItemCollection m_reportItems;

		private PageSectionInstance m_instance;

		public override string ID
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return base.m_renderReportItem.ID;
				}
				return base.m_reportItemDef.RenderingModelID;
			}
		}

		public override string DefinitionPath
		{
			get
			{
				return base.m_parentDefinitionPath.DefinitionPath + (this.m_isHeader ? "xH" : "xF");
			}
		}

		internal Page PageDefinition
		{
			get
			{
				return (Page)base.m_parentDefinitionPath;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.PageSection RifPageSection
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.PageSection)base.m_reportItemDef;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.PageSection RenderPageSection
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportRendering.PageSection)base.m_renderReportItem;
			}
		}

		internal bool IsHeader
		{
			get
			{
				return this.m_isHeader;
			}
		}

		public ReportSize Height
		{
			get
			{
				if (this.m_height == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_height = new ReportSize(this.RenderPageSection.Height);
					}
					else
					{
						this.m_height = new ReportSize(this.RifPageSection.Height);
					}
				}
				return this.m_height;
			}
		}

		public bool PrintOnFirstPage
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.RenderPageSection.PrintOnFirstPage;
				}
				return this.RifPageSection.PrintOnFirstPage;
			}
		}

		public bool PrintOnLastPage
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.RenderPageSection.PrintOnLastPage;
				}
				return this.RifPageSection.PrintOnLastPage;
			}
		}

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				if (this.m_reportItems == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_reportItems = new ReportItemCollection(this, false, this.RenderPageSection.ReportItemCollection, base.m_renderingContext);
					}
					else
					{
						this.m_reportItems = new ReportItemCollection(this.ReportScope, this, this.RifPageSection.ReportItems, base.m_renderingContext);
					}
				}
				return this.m_reportItems;
			}
		}

		public bool PrintBetweenSections
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return false;
				}
				return this.RifPageSection.PrintBetweenSections;
			}
		}

		internal override string InstanceUniqueName
		{
			get
			{
				if (this.Instance != null)
				{
					return this.Instance.UniqueName;
				}
				return null;
			}
		}

		internal override ReportElementInstance ReportElementInstance
		{
			get
			{
				return this.Instance;
			}
		}

		public new PageSectionInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new PageSectionInstance(this);
				}
				return this.m_instance;
			}
		}

		internal PageSection(IReportScope reportScope, IDefinitionPath parentDefinitionPath, bool isHeader, AspNetCore.ReportingServices.ReportIntermediateFormat.PageSection pageSectionDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, pageSectionDef, renderingContext)
		{
			this.m_isHeader = isHeader;
		}

		internal PageSection(IDefinitionPath parentDefinitionPath, bool isHeader, AspNetCore.ReportingServices.ReportRendering.PageSection renderPageSection, RenderingContext renderingContext)
			: base(parentDefinitionPath, renderPageSection, renderingContext)
		{
			this.m_isHeader = isHeader;
		}

		internal void UpdatePageSection(AspNetCore.ReportingServices.ReportRendering.PageSection renderPageSection)
		{
			base.m_renderReportItem = renderPageSection;
			if (this.m_reportItems != null)
			{
				this.m_reportItems.UpdateRenderReportItem(renderPageSection.ReportItemCollection);
			}
		}

		internal override void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			base.SetNewContext();
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_reportItems != null)
			{
				this.m_reportItems.SetNewContext();
			}
		}
	}
}
