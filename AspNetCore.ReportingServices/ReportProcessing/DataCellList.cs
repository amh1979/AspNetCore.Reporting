using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataCellList : ArrayList
	{
		internal new DataValueCRIList this[int index]
		{
			get
			{
				return (DataValueCRIList)base[index];
			}
		}

		internal DataCellList()
		{
		}

		internal DataCellList(int capacity)
			: base(capacity)
		{
		}
	}
}
