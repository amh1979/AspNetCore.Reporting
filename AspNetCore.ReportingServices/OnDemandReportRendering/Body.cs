using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Body : ReportElement
	{
		private AspNetCore.ReportingServices.ReportRendering.Report m_renderReport;

		private ReportItemCollection m_reportItems;

		private BodyInstance m_instance;

		private bool m_subreportInSubtotal;

		internal override bool UseRenderStyle
		{
			get
			{
				return this.m_renderReport.BodyHasBorderStyles;
			}
		}

		public override string ID
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return this.m_renderReport.Body.ID;
				}
				if (base.m_renderingContext.IsSubReportContext)
				{
					return this.SectionDef.RenderingModelID + "xS";
				}
				return this.SectionDef.RenderingModelID + "xB";
			}
		}

		public override string DefinitionPath
		{
			get
			{
				string str = (!base.m_renderingContext.IsSubReportContext) ? "xB" : "xS";
				return base.m_parentDefinitionPath.DefinitionPath + str;
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
						this.m_reportItems = new ReportItemCollection(base.m_parentDefinitionPath, this.m_subreportInSubtotal, this.m_renderReport.Body.ReportItemCollection, base.m_renderingContext);
					}
					else
					{
						this.m_reportItems = new ReportItemCollection(this.ReportScope, base.m_parentDefinitionPath, this.SectionDef.ReportItems, base.m_renderingContext);
					}
				}
				return this.m_reportItems;
			}
		}

		public ReportSize Height
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return new ReportSize(this.m_renderReport.Body.Height);
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef = this.SectionDef;
				if (sectionDef.HeightForRendering == null)
				{
					sectionDef.HeightForRendering = new ReportSize(sectionDef.Height, sectionDef.HeightValue);
				}
				return sectionDef.HeightForRendering;
			}
		}

		internal override AspNetCore.ReportingServices.ReportRendering.ReportItem RenderReportItem
		{
			get
			{
				return this.m_renderReport.Body;
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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection SectionDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection)base.m_reportItemDef;
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

		public new BodyInstance Instance
		{
			get
			{
				if (base.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new BodyInstance(this);
				}
				return this.m_instance;
			}
		}

		internal Body(IReportScope reportScope, IDefinitionPath parentDefinitionPath, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, sectionDef, renderingContext)
		{
		}

		internal Body(IDefinitionPath parentDefinitionPath, bool subreportInSubtotal, AspNetCore.ReportingServices.ReportRendering.Report renderReport, RenderingContext renderingContext)
			: base(parentDefinitionPath, renderingContext)
		{
			base.m_isOldSnapshot = true;
			this.m_subreportInSubtotal = subreportInSubtotal;
			this.m_renderReport = renderReport;
			base.m_renderingContext = renderingContext;
		}

		internal void UpdateSubReportContents(AspNetCore.ReportingServices.ReportRendering.Report newRenderSubreport)
		{
			this.m_renderReport = newRenderSubreport;
			if (this.m_reportItems != null)
			{
				this.m_reportItems.UpdateRenderReportItem(this.m_renderReport.Body.ReportItemCollection);
			}
		}

		internal override void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.SectionDef != null)
			{
				this.SectionDef.ResetTextBoxImpls(base.m_renderingContext.OdpContext);
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
