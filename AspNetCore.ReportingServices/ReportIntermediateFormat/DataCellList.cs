using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataCellList : CellList
	{
		internal new DataCell this[int index]
		{
			get
			{
				return (DataCell)base[index];
			}
		}

		public DataCellList()
		{
		}

		internal DataCellList(int capacity)
			: base(capacity)
		{
		}
	}
}
