using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableGroupInstanceList : ArrayList
	{
		internal new TableGroupInstance this[int index]
		{
			get
			{
				return (TableGroupInstance)base[index];
			}
		}

		internal TableGroupInstanceList()
		{
		}

		internal TableGroupInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
