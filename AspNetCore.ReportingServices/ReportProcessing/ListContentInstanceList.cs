using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ListContentInstanceList : ArrayList
	{
		internal new ListContentInstance this[int index]
		{
			get
			{
				return (ListContentInstance)base[index];
			}
		}

		internal ListContentInstanceList()
		{
		}

		internal ListContentInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
