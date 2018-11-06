using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportItemCollection : ReportElementCollectionBase<ReportItem>
	{
		private IDefinitionPath m_parentDefinitionPath;

		private bool m_isOldSnapshot;

		private bool m_inSubtotal;

		private ReportItem[] m_reportItems;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection m_reportItemColDef;

		private AspNetCore.ReportingServices.ReportRendering.ReportItemCollection m_renderReportItemCollection;

		private RenderingContext m_renderingContext;

		private IReportScope m_reportScope;

		public override ReportItem this[int index]
		{
			get
			{
				return this.GetItem(index).ExposeAs(this.m_renderingContext);
			}
		}

		public override int Count
		{
			get
			{
				if (this.m_isOldSnapshot)
				{
					if (this.m_renderReportItemCollection == null)
					{
						return 0;
					}
					return this.m_renderReportItemCollection.Count;
				}
				if (this.m_reportItemColDef.ROMIndexMap != null)
				{
					return this.m_reportItemColDef.ROMIndexMap.Count;
				}
				return this.m_reportItemColDef.Count;
			}
		}

		internal bool IsOldSnapshot
		{
			get
			{
				return this.m_isOldSnapshot;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.ReportItemCollection RenderReportItemCollection
		{
			get
			{
				if (!this.m_isOldSnapshot)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.m_renderReportItemCollection;
			}
		}

		internal ReportItemCollection(IReportScope reportScope, IDefinitionPath parentDefinitionPath, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection reportItemColDef, RenderingContext renderingContext)
		{
			this.m_reportScope = reportScope;
			this.m_parentDefinitionPath = parentDefinitionPath;
			this.m_isOldSnapshot = false;
			this.m_reportItemColDef = reportItemColDef;
			this.m_renderingContext = renderingContext;
		}

		internal ReportItemCollection(IDefinitionPath parentDefinitionPath, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.ReportItemCollection renderReportItemCollection, RenderingContext renderingContext)
		{
			this.m_parentDefinitionPath = parentDefinitionPath;
			this.m_isOldSnapshot = true;
			this.m_inSubtotal = inSubtotal;
			this.m_renderReportItemCollection = renderReportItemCollection;
			this.m_renderingContext = renderingContext;
		}

		internal void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItemCollection renderReportItemCollection)
		{
			if (!this.m_isOldSnapshot)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (renderReportItemCollection != null)
			{
				this.m_renderReportItemCollection = renderReportItemCollection;
			}
			if (this.m_reportItems != null)
			{
				for (int i = 0; i < this.m_reportItems.Length; i++)
				{
					if (this.m_reportItems[i] != null)
					{
						this.m_reportItems[i].UpdateRenderReportItem(renderReportItemCollection[i]);
					}
				}
			}
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this.GetItem(i).SetNewContext();
			}
		}

		private ReportItem GetItem(int index)
		{
			if (0 <= index && index < this.Count)
			{
				ReportItem reportItem = null;
				if (this.m_reportItems == null)
				{
					this.m_reportItems = new ReportItem[this.Count];
				}
				reportItem = this.m_reportItems[index];
				if (reportItem == null)
				{
					if (this.m_isOldSnapshot)
					{
						reportItem = (this.m_reportItems[index] = ReportItem.CreateShim(this.m_parentDefinitionPath, index, this.m_inSubtotal, this.m_renderReportItemCollection[index], this.m_renderingContext));
					}
					else
					{
						int num = (this.m_reportItemColDef.ROMIndexMap == null) ? index : this.m_reportItemColDef.ROMIndexMap[index];
						reportItem = (this.m_reportItems[index] = ReportItem.CreateItem(this.m_reportScope, this.m_parentDefinitionPath, num, this.m_reportItemColDef[num], this.m_renderingContext));
					}
				}
				return reportItem;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
		}
	}
}
