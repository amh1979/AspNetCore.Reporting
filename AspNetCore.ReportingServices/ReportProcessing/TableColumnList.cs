using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableColumnList : ArrayList
	{
		internal new TableColumn this[int index]
		{
			get
			{
				return (TableColumn)base[index];
			}
		}

		internal TableColumnList()
		{
		}

		internal TableColumnList(int capacity)
			: base(capacity)
		{
		}
	}
}
