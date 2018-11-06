using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionItemList : ArrayList
	{
		internal new ActionItem this[int index]
		{
			get
			{
				return (ActionItem)base[index];
			}
		}

		internal ActionItemList()
		{
		}

		internal ActionItemList(int capacity)
			: base(capacity)
		{
		}
	}
}
