using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSetList : ArrayList
	{
		internal new DataSet this[int index]
		{
			get
			{
				return (DataSet)base[index];
			}
		}

		internal DataSetList()
		{
		}

		internal DataSetList(int capacity)
			: base(capacity)
		{
		}
	}
}
