using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Rectangle : ReportItem, IPageBreakItem
	{
		private AspNetCore.ReportingServices.ReportRendering.Rectangle m_renderRectangle;

		private ListContent m_listContents;

		private ReportItemCollection m_reportItems;

		private PageBreak m_pageBreak;

		private ReportStringProperty m_pageName;

		public override string Name
		{
			get
			{
				if (base.m_isListContentsRectangle)
				{
					return this.m_listContents.OwnerDataRegion.Name + "_Contents";
				}
				return base.Name;
			}
		}

		public override int LinkToChild
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (base.m_isListContentsRectangle)
					{
						return -1;
					}
					return this.m_renderRectangle.LinkToChild;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)base.ReportItemDef).LinkToChild;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (base.m_isOldSnapshot && base.m_isListContentsRectangle)
				{
					return DataElementOutputTypes.ContentsOnly;
				}
				return base.DataElementOutput;
			}
		}

		public override ReportSize Top
		{
			get
			{
				if (base.m_isListContentsRectangle)
				{
					return new ReportSize("0 mm", 0.0);
				}
				return base.Top;
			}
		}

		public override ReportSize Left
		{
			get
			{
				if (base.m_isListContentsRectangle)
				{
					return new ReportSize("0 mm", 0.0);
				}
				return base.Left;
			}
		}

		public bool IsSimple
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return base.m_isListContentsRectangle;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)base.m_reportItemDef).IsSimple;
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
						if (base.m_isListContentsRectangle)
						{
							this.m_reportItems = new ReportItemCollection(this, base.m_inSubtotal, this.m_listContents.ReportItemCollection, base.m_renderingContext);
						}
						else
						{
							this.m_reportItems = new ReportItemCollection(this, base.m_inSubtotal, this.m_renderRectangle.ReportItemCollection, base.m_renderingContext);
						}
					}
					else
					{
						this.m_reportItems = new ReportItemCollection(this.ReportScope, this, ((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)base.m_reportItemDef).ReportItems, base.m_renderingContext);
					}
				}
				return this.m_reportItems;
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				if (this.m_pageBreak == null)
				{
					if (base.m_isOldSnapshot)
					{
						PageBreakLocation pageBreaklocation = (!base.m_isListContentsRectangle) ? PageBreakHelper.GetPageBreakLocation(this.m_renderRectangle.PageBreakAtStart, this.m_renderRectangle.PageBreakAtEnd) : PageBreakHelper.GetPageBreakLocation(false, false);
						this.m_pageBreak = new PageBreak(base.RenderingContext, this.ReportScope, pageBreaklocation);
					}
					else
					{
						IPageBreakOwner pageBreakOwner = (AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)base.m_reportItemDef;
						this.m_pageBreak = new PageBreak(base.RenderingContext, this.ReportScope, pageBreakOwner);
					}
				}
				return this.m_pageBreak;
			}
		}

		public ReportStringProperty PageName
		{
			get
			{
				if (this.m_pageName == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_pageName = new ReportStringProperty();
					}
					else
					{
						this.m_pageName = new ReportStringProperty(((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)base.m_reportItemDef).PageName);
					}
				}
				return this.m_pageName;
			}
		}

		public bool KeepTogether
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return base.m_isListContentsRectangle;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)base.ReportItemDef).KeepTogether;
			}
		}

		public bool OmitBorderOnPageBreak
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return false;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)base.ReportItemDef).OmitBorderOnPageBreak;
			}
		}

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					if (base.m_isListContentsRectangle)
					{
						return PageBreakHelper.GetPageBreakLocation(false, false);
					}
					return PageBreakHelper.GetPageBreakLocation(this.m_renderRectangle.PageBreakAtStart, this.m_renderRectangle.PageBreakAtEnd);
				}
				IPageBreakOwner pageBreakOwner = (IPageBreakOwner)base.ReportItemDef;
				if (pageBreakOwner.PageBreak != null)
				{
					PageBreak pageBreak = this.PageBreak;
					if (pageBreak.HasEnabledInstance)
					{
						return pageBreak.BreakLocation;
					}
				}
				return PageBreakLocation.None;
			}
		}

		internal override AspNetCore.ReportingServices.ReportRendering.ReportItem RenderReportItem
		{
			get
			{
				if (base.m_isListContentsRectangle)
				{
					return this.m_listContents.OwnerDataRegion;
				}
				return base.m_renderReportItem;
			}
		}

		public override Visibility Visibility
		{
			get
			{
				if (base.m_isListContentsRectangle)
				{
					return null;
				}
				return base.Visibility;
			}
		}

		internal bool IsListContentsRectangle
		{
			get
			{
				return base.m_isListContentsRectangle;
			}
		}

		internal Rectangle(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Rectangle(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.Rectangle renderRectangle, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderRectangle, renderingContext)
		{
			this.m_renderRectangle = renderRectangle;
		}

		internal Rectangle(IDefinitionPath parentDefinitionPath, bool inSubtotal, ListContent renderContents, RenderingContext renderingContext)
			: base(parentDefinitionPath, inSubtotal, renderingContext)
		{
			this.m_listContents = renderContents;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new RectangleInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_reportItems != null)
			{
				this.m_reportItems.SetNewContext();
			}
			if (this.m_pageBreak != null)
			{
				this.m_pageBreak.SetNewContext();
			}
		}

		internal void UpdateListContents(ListContent listContents)
		{
			if (!base.m_isOldSnapshot)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			this.SetNewContext();
			if (listContents != null)
			{
				this.m_listContents = listContents;
			}
			if (this.m_reportItems != null)
			{
				this.m_reportItems.UpdateRenderReportItem(listContents.ReportItemCollection);
			}
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			this.m_renderRectangle = (AspNetCore.ReportingServices.ReportRendering.Rectangle)renderReportItem;
			if (this.m_reportItems != null && renderReportItem is AspNetCore.ReportingServices.ReportRendering.Rectangle)
			{
				this.m_reportItems.UpdateRenderReportItem(this.m_renderRectangle.ReportItemCollection);
			}
		}
	}
}
