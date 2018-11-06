using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActionCollectionRendering : MemberBase
	{
		internal ActionItemList m_actionList;

		internal ActionItemInstanceList m_actionInstanceList;

		internal RenderingContext m_renderingContext;

		internal Action[] m_actions;

		internal string m_ownerUniqueName;

		internal ActionCollectionRendering()
			: base(false)
		{
		}
	}
}
