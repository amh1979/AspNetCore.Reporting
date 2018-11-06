using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Chart : DynamicImage
	{
		protected override PaginationInfoItems PaginationInfoEnum
		{
			get
			{
				return PaginationInfoItems.Chart;
			}
		}

		protected override bool SpecialBorderHandling
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.Chart)base.m_source).SpecialBorderHandling;
			}
		}

		protected override PageBreak PageBreak
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.Chart)base.m_source).PageBreak;
			}
		}

		protected override string PageName
		{
			get
			{
				return ((ChartInstance)base.m_source.Instance).PageName;
			}
		}

		internal override double SourceWidthInMM
		{
			get
			{
				return ((ChartInstance)base.m_source.Instance).DynamicWidth.ToMillimeters();
			}
		}

		internal Chart(AspNetCore.ReportingServices.OnDemandReportRendering.Chart source, PageContext pageContext, bool createForRepeat)
			: base(source, pageContext, createForRepeat)
		{
			ChartInstance chartInstance = (ChartInstance)base.m_source.Instance;
			base.m_itemPageSizes.AdjustHeightTo(chartInstance.DynamicHeight.ToMillimeters());
			base.m_itemPageSizes.AdjustWidthTo(chartInstance.DynamicWidth.ToMillimeters());
		}

		protected override byte GetElementToken(PageContext pageContext)
		{
			return 11;
		}

		protected override string GenerateStreamName(PageContext pageContext)
		{
			return pageContext.GenerateStreamName((ChartInstance)base.m_source.Instance);
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLChart();
		}
	}
}
