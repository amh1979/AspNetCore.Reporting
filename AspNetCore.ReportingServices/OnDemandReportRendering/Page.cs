using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Page : ReportElement, IReportScope
	{
		private PageInstance m_instance;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Page m_pageDef;

		private AspNetCore.ReportingServices.ReportRendering.Report m_renderReport;

		private ReportSection m_reportSection;

		private PageSection m_pageHeader;

		private PageSection m_pageFooter;

		public override string ID
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderReport.ReportDef.ID.ToString(CultureInfo.InvariantCulture) + "xP";
				}
				return this.m_pageDef.RenderingModelID;
			}
		}

		public override string DefinitionPath
		{
			get
			{
				return ((base.ParentDefinitionPath.DefinitionPath != null) ? base.ParentDefinitionPath.DefinitionPath : "") + "xP";
			}
		}

		public PageSection PageHeader
		{
			get
			{
				if (this.m_pageHeader == null && !base.m_renderingContext.IsSubReportContext)
				{
					if (base.m_isOldSnapshot && this.m_renderReport.PageHeader != null)
					{
						this.m_pageHeader = new PageSection(this, true, this.m_renderReport.PageHeader, this.m_reportSection.Report.HeaderFooterRenderingContext);
					}
					else if (!base.m_isOldSnapshot && this.m_pageDef.PageHeader != null)
					{
						this.m_pageHeader = new PageSection(this, this, true, this.m_pageDef.PageHeader, this.m_reportSection.Report.HeaderFooterRenderingContext);
					}
				}
				return this.m_pageHeader;
			}
		}

		public PageSection PageFooter
		{
			get
			{
				if (this.m_pageFooter == null && !base.m_renderingContext.IsSubReportContext)
				{
					if (base.m_isOldSnapshot && this.m_renderReport.PageFooter != null)
					{
						this.m_pageFooter = new PageSection(this, false, this.m_renderReport.PageFooter, this.m_reportSection.Report.HeaderFooterRenderingContext);
					}
					else if (!base.m_isOldSnapshot && this.m_pageDef.PageFooter != null)
					{
						this.m_pageFooter = new PageSection(this, this, false, this.m_pageDef.PageFooter, this.m_reportSection.Report.HeaderFooterRenderingContext);
					}
				}
				return this.m_pageFooter;
			}
		}

		public ReportSize PageHeight
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.PageHeight);
				}
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.PageHeight;
				}
				if (this.m_pageDef.PageHeightForRendering == null)
				{
					this.m_pageDef.PageHeightForRendering = new ReportSize(this.m_pageDef.PageHeight, this.m_pageDef.PageHeightValue);
				}
				return this.m_pageDef.PageHeightForRendering;
			}
		}

		public ReportSize PageWidth
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.PageWidth);
				}
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.PageWidth;
				}
				if (this.m_pageDef.PageWidthForRendering == null)
				{
					this.m_pageDef.PageWidthForRendering = new ReportSize(this.m_pageDef.PageWidth, this.m_pageDef.PageWidthValue);
				}
				return this.m_pageDef.PageWidthForRendering;
			}
		}

		public ReportSize InteractiveHeight
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.ReportDef.InteractiveHeight, this.m_renderReport.ReportDef.InteractiveHeightValue);
				}
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.InteractiveHeight;
				}
				if (this.m_pageDef.InteractiveHeightForRendering == null)
				{
					this.m_pageDef.InteractiveHeightForRendering = new ReportSize(this.m_pageDef.InteractiveHeight, this.m_pageDef.InteractiveHeightValue);
				}
				return this.m_pageDef.InteractiveHeightForRendering;
			}
		}

		public ReportSize InteractiveWidth
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.ReportDef.InteractiveWidth, this.m_renderReport.ReportDef.InteractiveWidthValue);
				}
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.InteractiveWidth;
				}
				if (this.m_pageDef.InteractiveWidthForRendering == null)
				{
					this.m_pageDef.InteractiveWidthForRendering = new ReportSize(this.m_pageDef.InteractiveWidth, this.m_pageDef.InteractiveWidthValue);
				}
				return this.m_pageDef.InteractiveWidthForRendering;
			}
		}

		public ReportSize LeftMargin
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.LeftMargin);
				}
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.LeftMargin;
				}
				if (this.m_pageDef.LeftMarginForRendering == null)
				{
					this.m_pageDef.LeftMarginForRendering = new ReportSize(this.m_pageDef.LeftMargin, this.m_pageDef.LeftMarginValue);
				}
				return this.m_pageDef.LeftMarginForRendering;
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.RightMargin);
				}
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.RightMargin;
				}
				if (this.m_pageDef.RightMarginForRendering == null)
				{
					this.m_pageDef.RightMarginForRendering = new ReportSize(this.m_pageDef.RightMargin, this.m_pageDef.RightMarginValue);
				}
				return this.m_pageDef.RightMarginForRendering;
			}
		}

		public ReportSize TopMargin
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.TopMargin);
				}
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.TopMargin;
				}
				if (this.m_pageDef.TopMarginForRendering == null)
				{
					this.m_pageDef.TopMarginForRendering = new ReportSize(this.m_pageDef.TopMargin, this.m_pageDef.TopMarginValue);
				}
				return this.m_pageDef.TopMarginForRendering;
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.BottomMargin);
				}
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.BottomMargin;
				}
				if (this.m_pageDef.BottomMarginForRendering == null)
				{
					this.m_pageDef.BottomMarginForRendering = new ReportSize(this.m_pageDef.BottomMargin, this.m_pageDef.BottomMarginValue);
				}
				return this.m_pageDef.BottomMarginForRendering;
			}
		}

		internal override bool UseRenderStyle
		{
			get
			{
				return !this.m_renderReport.BodyHasBorderStyles;
			}
		}

		internal override IStyleContainer StyleContainer
		{
			get
			{
				return this.m_pageDef;
			}
		}

		public int Columns
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderReport.Columns;
				}
				return this.m_pageDef.Columns;
			}
		}

		public ReportSize ColumnSpacing
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.ColumnSpacing);
				}
				if (this.m_pageDef.ColumnSpacingForRendering == null)
				{
					this.m_pageDef.ColumnSpacingForRendering = new ReportSize(this.m_pageDef.ColumnSpacing, this.m_pageDef.ColumnSpacingValue);
				}
				return this.m_pageDef.ColumnSpacingForRendering;
			}
		}

		public override Style Style
		{
			get
			{
				if (this.ShouldUseFirstSection)
				{
					return this.FirstSectionPage.Style;
				}
				return base.Style;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.Report RenderReport
		{
			get
			{
				if (!base.m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.m_renderReport;
			}
		}

		internal override AspNetCore.ReportingServices.ReportRendering.ReportItem RenderReportItem
		{
			get
			{
				if (!base.m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.m_renderReport.Body;
			}
		}

		internal Page FirstSectionPage
		{
			get
			{
				return this.m_reportSection.Report.FirstSection.Page;
			}
		}

		internal bool ShouldUseFirstSection
		{
			get
			{
				return this.m_reportSection.SectionIndex > 0;
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

		public new PageInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new PageInstance(this);
				}
				return this.m_instance;
			}
		}

		internal override IReportScope ReportScope
		{
			get
			{
				return this;
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
				return this.m_pageDef;
			}
		}

		internal Page(IDefinitionPath parentDefinitionPath, RenderingContext renderingContext, ReportSection reportSection)
			: base(null, parentDefinitionPath, reportSection.SectionDef, renderingContext)
		{
			base.m_isOldSnapshot = false;
			this.m_pageDef = reportSection.SectionDef.Page;
			this.m_reportSection = reportSection;
		}

		internal Page(IDefinitionPath parentDefinitionPath, AspNetCore.ReportingServices.ReportRendering.Report renderReport, RenderingContext renderingContext, ReportSection reportSection)
			: base(parentDefinitionPath, renderingContext)
		{
			base.m_isOldSnapshot = true;
			this.m_renderReport = renderReport;
			this.m_reportSection = reportSection;
		}

		internal void UpdateWithCurrentPageSections(AspNetCore.ReportingServices.ReportRendering.PageSection header, AspNetCore.ReportingServices.ReportRendering.PageSection footer)
		{
			if (header != null)
			{
				this.PageHeader.UpdatePageSection(header);
			}
			if (footer != null)
			{
				this.PageFooter.UpdatePageSection(footer);
			}
		}

		internal void UpdateSubReportContents(AspNetCore.ReportingServices.ReportRendering.Report newRenderSubreport)
		{
			this.m_renderReport = newRenderSubreport;
			this.UpdateWithCurrentPageSections(this.m_renderReport.PageHeader, this.m_renderReport.PageFooter);
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_pageHeader != null)
			{
				this.m_pageHeader.SetNewContext();
			}
			if (this.m_pageFooter != null)
			{
				this.m_pageFooter.SetNewContext();
			}
		}
	}
}
