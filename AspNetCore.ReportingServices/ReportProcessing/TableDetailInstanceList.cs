using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableDetailInstanceList : ArrayList
	{
		internal new TableDetailInstance this[int index]
		{
			get
			{
				return (TableDetailInstance)base[index];
			}
		}

		internal TableDetailInstanceList()
		{
		}

		internal TableDetailInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
