using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class GaugePanel : DynamicImage
	{
		protected override PaginationInfoItems PaginationInfoEnum
		{
			get
			{
				return PaginationInfoItems.GaugePanel;
			}
		}

		protected override bool SpecialBorderHandling
		{
			get
			{
				return false;
			}
		}

		protected override PageBreak PageBreak
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel)base.m_source).PageBreak;
			}
		}

		protected override string PageName
		{
			get
			{
				return ((GaugePanelInstance)base.m_source.Instance).PageName;
			}
		}

		internal GaugePanel(AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel source, PageContext pageContext, bool createForRepeat)
			: base(source, pageContext, createForRepeat)
		{
		}

		protected override byte GetElementToken(PageContext pageContext)
		{
			return 14;
		}

		protected override string GenerateStreamName(PageContext pageContext)
		{
			return pageContext.GenerateStreamName((GaugePanelInstance)base.m_source.Instance);
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLGaugePanel();
		}
	}
}
