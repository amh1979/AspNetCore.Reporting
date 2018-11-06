using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataAggregateInfoList : ArrayList
	{
		internal new DataAggregateInfo this[int index]
		{
			get
			{
				return (DataAggregateInfo)base[index];
			}
		}

		internal DataAggregateInfoList()
		{
		}

		internal DataAggregateInfoList(int capacity)
			: base(capacity)
		{
		}
	}
}
