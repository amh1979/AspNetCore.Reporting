using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageInstance : ReportElementInstance, IReportScopeInstance
	{
		private bool m_isNewContext;

		public string UniqueName
		{
			get
			{
				if (base.m_reportElementDef.IsOldSnapshot)
				{
					return this.PageDefinition.RenderReport.UniqueName + "xP";
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection reportSection = (AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection)this.PageDefinition.ReportItemDef;
				return InstancePathItem.GenerateUniqueNameString(reportSection.ID, reportSection.InstancePath) + "xP";
			}
		}

		public override StyleInstance Style
		{
			get
			{
				Page pageDefinition = this.PageDefinition;
				if (pageDefinition.ShouldUseFirstSection)
				{
					return pageDefinition.FirstSectionPage.Instance.Style;
				}
				return base.Style;
			}
		}

		internal Page PageDefinition
		{
			get
			{
				return (Page)base.m_reportElementDef;
			}
		}

		IReportScope IReportScopeInstance.ReportScope
		{
			get
			{
				return this.PageDefinition;
			}
		}

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return this.m_isNewContext;
			}
			set
			{
				this.m_isNewContext = value;
			}
		}

		internal PageInstance(Page pageDef)
			: base(pageDef)
		{
		}
	}
}
