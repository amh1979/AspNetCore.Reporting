using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Line : ReportItem
	{
		public bool Slant
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return ((AspNetCore.ReportingServices.ReportRendering.Line)base.m_renderReportItem).Slant;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.Line)base.m_reportItemDef).LineSlant;
			}
		}

		internal Line(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Line reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Line(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.Line renderLine, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderLine, renderingContext)
		{
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new LineInstance(this);
			}
			return base.m_instance;
		}
	}
}
