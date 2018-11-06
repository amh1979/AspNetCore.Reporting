using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IAggregateHolder
	{
		DataScopeInfo DataScopeInfo
		{
			get;
		}

		List<DataAggregateInfo> GetAggregateList();

		List<DataAggregateInfo> GetPostSortAggregateList();

		void ClearIfEmpty();
	}
}
