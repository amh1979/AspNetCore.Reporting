using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionItemInstanceList : ArrayList
	{
		internal new ActionItemInstance this[int index]
		{
			get
			{
				return (ActionItemInstance)base[index];
			}
		}

		internal ActionItemInstanceList()
		{
		}

		internal ActionItemInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
