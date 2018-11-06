using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class DataAggregateObjResultsList : ArrayList
	{
		internal new DataAggregateObjResult[] this[int index]
		{
			get
			{
				return (DataAggregateObjResult[])base[index];
			}
		}
	}
}
