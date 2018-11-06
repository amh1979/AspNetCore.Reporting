using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Map : DynamicImage
	{
		protected override PaginationInfoItems PaginationInfoEnum
		{
			get
			{
				return PaginationInfoItems.Map;
			}
		}

		protected override bool SpecialBorderHandling
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.Map)base.m_source).SpecialBorderHandling;
			}
		}

		protected override PageBreak PageBreak
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.Map)base.m_source).PageBreak;
			}
		}

		protected override string PageName
		{
			get
			{
				return ((MapInstance)base.m_source.Instance).PageName;
			}
		}

		internal Map(AspNetCore.ReportingServices.OnDemandReportRendering.Map source, PageContext pageContext, bool createForRepeat)
			: base(source, pageContext, createForRepeat)
		{
		}

		protected override byte GetElementToken(PageContext pageContext)
		{
			if (pageContext.VersionPicker != 0 && pageContext.VersionPicker != RPLVersionEnum.RPLAccess && pageContext.VersionPicker != RPLVersionEnum.RPL2008WithImageConsolidation)
			{
				return 21;
			}
			return 11;
		}

		protected override string GenerateStreamName(PageContext pageContext)
		{
			return pageContext.GenerateStreamName((MapInstance)base.m_source.Instance);
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLMap();
		}
	}
}
