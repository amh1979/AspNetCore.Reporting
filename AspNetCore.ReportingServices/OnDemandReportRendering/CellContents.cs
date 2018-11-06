using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class CellContents
	{
		private IDefinitionPath m_ownerPath;

		private bool m_isOldSnapshot;

		private bool m_inSubtotal;

		private RenderingContext m_renderingContext;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem m_cellReportItem;

		private AspNetCore.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		private ReportItem m_reportItem;

		private int m_colSpan = 1;

		private int m_rowSpan = 1;

		private IReportScope m_reportScope;

		private double m_sizeDelta;

		private bool m_isColumn;

		public ReportItem ReportItem
		{
			get
			{
				if (this.m_reportItem == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_reportItem = ReportItem.CreateShim(this.m_ownerPath, 0, this.m_inSubtotal, this.m_renderReportItem, this.m_renderingContext);
						if (this.m_sizeDelta > 0.0)
						{
							if (this.m_isColumn)
							{
								this.m_reportItem.SetCachedWidth(this.m_sizeDelta);
							}
							else
							{
								this.m_reportItem.SetCachedHeight(this.m_sizeDelta);
							}
						}
					}
					else if (this.m_cellReportItem != null)
					{
						this.m_reportItem = ReportItem.CreateItem(this.m_reportScope, this.m_ownerPath, 0, this.m_cellReportItem, this.m_renderingContext);
					}
				}
				if (this.m_reportItem != null)
				{
					return this.m_reportItem.ExposeAs(this.m_renderingContext);
				}
				return null;
			}
		}

		public int ColSpan
		{
			get
			{
				return this.m_colSpan;
			}
		}

		public int RowSpan
		{
			get
			{
				return this.m_rowSpan;
			}
		}

		internal CellContents(IReportScope reportScope, IDefinitionPath ownerPath, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem cellReportItem, int rowSpan, int colSpan, RenderingContext renderingContext)
		{
			this.m_reportScope = reportScope;
			this.m_rowSpan = rowSpan;
			this.m_colSpan = colSpan;
			this.m_ownerPath = ownerPath;
			this.m_isOldSnapshot = false;
			this.m_cellReportItem = cellReportItem;
			this.m_renderingContext = renderingContext;
		}

		internal CellContents(IDefinitionPath ownerPath, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem, int rowSpan, int colSpan, RenderingContext renderingContext)
		{
			this.m_rowSpan = rowSpan;
			this.m_colSpan = colSpan;
			this.m_ownerPath = ownerPath;
			this.m_isOldSnapshot = true;
			this.m_inSubtotal = inSubtotal;
			this.m_renderReportItem = renderReportItem;
			this.m_renderingContext = renderingContext;
		}

		internal CellContents(IDefinitionPath ownerPath, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem, int rowSpan, int colSpan, RenderingContext renderingContext, double sizeDelta, bool isColumn)
		{
			this.m_rowSpan = rowSpan;
			this.m_colSpan = colSpan;
			this.m_ownerPath = ownerPath;
			this.m_isOldSnapshot = true;
			this.m_inSubtotal = inSubtotal;
			this.m_renderReportItem = renderReportItem;
			this.m_renderingContext = renderingContext;
			this.m_sizeDelta = sizeDelta;
			this.m_isColumn = isColumn;
		}

		internal CellContents(Rectangle rectangle, int rowSpan, int colSpan, RenderingContext renderingContext)
		{
			this.m_rowSpan = rowSpan;
			this.m_colSpan = colSpan;
			this.m_ownerPath = rectangle;
			this.m_reportItem = rectangle;
			this.m_renderingContext = renderingContext;
			this.m_isOldSnapshot = true;
		}

		internal void SetNewContext()
		{
			if (this.m_reportItem != null)
			{
				this.m_reportItem.SetNewContext();
			}
		}

		internal void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (renderReportItem != null)
			{
				this.m_renderReportItem = renderReportItem;
			}
			if (this.m_reportItem != null)
			{
				this.m_reportItem.UpdateRenderReportItem(this.m_renderReportItem);
			}
		}
	}
}
