using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class RowList : ArrayList
	{
		internal new Row this[int index]
		{
			get
			{
				return (Row)base[index];
			}
		}

		internal RowList()
		{
		}

		internal RowList(int capacity)
			: base(capacity)
		{
		}
	}
}
