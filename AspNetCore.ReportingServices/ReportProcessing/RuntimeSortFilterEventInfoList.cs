using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class RuntimeSortFilterEventInfoList : ArrayList
	{
		internal new RuntimeSortFilterEventInfo this[int index]
		{
			get
			{
				return (RuntimeSortFilterEventInfo)base[index];
			}
		}

		internal RuntimeSortFilterEventInfoList()
		{
		}
	}
}
