using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IRunningValueHolder
	{
		DataScopeInfo DataScopeInfo
		{
			get;
		}

		List<RunningValueInfo> GetRunningValueList();

		void ClearIfEmpty();
	}
}
