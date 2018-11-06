using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class InScopeSortFilterHashtable : Hashtable
	{
		internal List<int> this[int index]
		{
			get
			{
				return (List<int>)base[index];
			}
		}

		public InScopeSortFilterHashtable()
		{
		}

		internal InScopeSortFilterHashtable(int capacity)
			: base(capacity)
		{
		}
	}
}
