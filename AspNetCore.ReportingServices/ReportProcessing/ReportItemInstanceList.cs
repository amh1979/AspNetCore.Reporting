using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportItemInstanceList : ArrayList
	{
		internal new ReportItemInstance this[int index]
		{
			get
			{
				return (ReportItemInstance)base[index];
			}
		}

		internal ReportItemInstanceList()
		{
		}

		internal ReportItemInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
