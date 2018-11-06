using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class DataAggregateObjList : ArrayList
	{
		internal new DataAggregateObj this[int index]
		{
			get
			{
				return (DataAggregateObj)base[index];
			}
		}
	}
}
