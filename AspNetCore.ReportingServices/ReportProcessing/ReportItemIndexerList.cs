using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportItemIndexerList : ArrayList
	{
		internal new ReportItemIndexer this[int index]
		{
			get
			{
				return (ReportItemIndexer)base[index];
			}
		}

		internal ReportItemIndexerList()
		{
		}

		internal ReportItemIndexerList(int capacity)
			: base(capacity)
		{
		}
	}
}
