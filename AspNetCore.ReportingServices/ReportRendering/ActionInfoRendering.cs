using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActionInfoRendering : MemberBase
	{
		internal AspNetCore.ReportingServices.ReportProcessing.Action m_actionInfoDef;

		internal ActionInstance m_actionInfoInstance;

		internal RenderingContext m_renderingContext;

		internal ActionStyle m_style;

		internal ActionCollection m_actionCollection;

		internal string m_ownerUniqueName;

		internal ActionInfoRendering()
			: base(false)
		{
		}
	}
}
