using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActionCollectionProcessing : MemberBase
	{
		internal ArrayList m_actions;

		internal ActionCollectionProcessing()
			: base(true)
		{
		}

		internal ActionCollectionProcessing DeepClone()
		{
			if (this.m_actions != null && this.m_actions.Count != 0)
			{
				ActionCollectionProcessing actionCollectionProcessing = new ActionCollectionProcessing();
				int count = this.m_actions.Count;
				actionCollectionProcessing.m_actions = new ArrayList();
				for (int i = 0; i < count; i++)
				{
					actionCollectionProcessing.m_actions.Add(((Action)this.m_actions[i]).DeepClone());
				}
				return actionCollectionProcessing;
			}
			return null;
		}
	}
}
