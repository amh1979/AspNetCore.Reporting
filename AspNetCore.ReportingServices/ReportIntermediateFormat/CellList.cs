using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class CellList : ArrayList
	{
		internal new Cell this[int index]
		{
			get
			{
				return (Cell)base[index];
			}
		}

		internal CellList()
		{
		}

		internal CellList(int capacity)
			: base(capacity)
		{
		}
	}
}
