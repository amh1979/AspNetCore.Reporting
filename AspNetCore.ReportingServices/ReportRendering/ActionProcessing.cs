namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActionProcessing : MemberBase
	{
		internal string m_label;

		internal string m_action;

		internal ActionProcessing()
			: base(true)
		{
		}

		internal ActionProcessing DeepClone()
		{
			ActionProcessing actionProcessing = new ActionProcessing();
			if (this.m_label != null)
			{
				actionProcessing.m_label = string.Copy(this.m_label);
			}
			if (this.m_action != null)
			{
				actionProcessing.m_action = string.Copy(this.m_action);
			}
			return actionProcessing;
		}
	}
}
