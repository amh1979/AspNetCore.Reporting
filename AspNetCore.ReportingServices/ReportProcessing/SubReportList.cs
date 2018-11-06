using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	[ArrayOfReferences]
	internal sealed class SubReportList : ArrayList
	{
		internal new SubReport this[int index]
		{
			get
			{
				return (SubReport)base[index];
			}
		}

		internal SubReportList()
		{
		}

		internal SubReportList(int capacity)
			: base(capacity)
		{
		}

		internal new SubReportList Clone()
		{
			int count = this.Count;
			SubReportList subReportList = new SubReportList(count);
			for (int i = 0; i < count; i++)
			{
				subReportList.Add(this[i]);
			}
			return subReportList;
		}
	}
}
