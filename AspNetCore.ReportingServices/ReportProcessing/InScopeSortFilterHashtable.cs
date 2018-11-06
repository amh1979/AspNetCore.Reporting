using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class InScopeSortFilterHashtable : Hashtable
	{
		internal IntList this[int index]
		{
			get
			{
				return (IntList)base[index];
			}
		}

		internal InScopeSortFilterHashtable()
		{
		}

		internal InScopeSortFilterHashtable(int capacity)
			: base(capacity)
		{
		}
	}
}
