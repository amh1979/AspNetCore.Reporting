using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportItemVisibilityInstance : VisibilityInstance
	{
		private ReportItem m_reportItem;

		public override bool CurrentlyHidden
		{
			get
			{
				if (!base.m_cachedCurrentlyHidden)
				{
					base.m_cachedCurrentlyHidden = true;
					if (this.m_reportItem.IsOldSnapshot)
					{
						base.m_currentlyHiddenValue = this.m_reportItem.RenderReportItem.Hidden;
					}
					else
					{
						base.m_currentlyHiddenValue = this.m_reportItem.ReportItemDef.ComputeHidden(this.m_reportItem.RenderingContext, ToggleCascadeDirection.None);
					}
				}
				return base.m_currentlyHiddenValue;
			}
		}

		public override bool StartHidden
		{
			get
			{
				if (!base.m_cachedStartHidden)
				{
					base.m_cachedStartHidden = true;
					if (this.m_reportItem.IsOldSnapshot)
					{
						base.m_startHiddenValue = this.m_reportItem.RenderReportItem.Hidden;
					}
					else
					{
						base.m_startHiddenValue = this.m_reportItem.ReportItemDef.ComputeStartHidden(this.m_reportItem.RenderingContext);
					}
				}
				return base.m_startHiddenValue;
			}
		}

		internal ReportItemVisibilityInstance(ReportItem reportitem)
			: base(reportitem.ReportScope)
		{
			this.m_reportItem = reportitem;
		}
	}
}
