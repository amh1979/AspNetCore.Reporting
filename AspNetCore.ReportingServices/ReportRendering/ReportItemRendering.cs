using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportItemRendering : MemberBase
	{
		internal RenderingContext m_renderingContext;

		internal AspNetCore.ReportingServices.ReportProcessing.ReportItem m_reportItemDef;

		internal ReportItemInstance m_reportItemInstance;

		internal ReportItemInstanceInfo m_reportItemInstanceInfo;

		internal MatrixHeadingInstance m_headingInstance;

		internal ReportItemRendering()
			: base(false)
		{
		}
	}
}
