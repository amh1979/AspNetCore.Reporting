using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActionInfoProcessing : MemberBase
	{
		internal ActionStyle m_style;

		internal ActionCollection m_actionCollection;

		internal DataValueInstanceList m_sharedStyles;

		internal DataValueInstanceList m_nonSharedStyles;

		internal ActionInfoProcessing()
			: base(true)
		{
		}

		internal ActionInfoProcessing DeepClone()
		{
			Global.Tracer.Assert(this.m_sharedStyles == null && null == this.m_nonSharedStyles);
			ActionInfoProcessing actionInfoProcessing = new ActionInfoProcessing();
			if (this.m_style != null)
			{
				((StyleBase)this.m_style).ExtractRenderStyles(out actionInfoProcessing.m_sharedStyles, out actionInfoProcessing.m_nonSharedStyles);
			}
			if (this.m_actionCollection != null)
			{
				actionInfoProcessing.m_actionCollection = this.m_actionCollection.DeepClone();
			}
			return actionInfoProcessing;
		}
	}
}
