using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemCellInstancesList : ArrayList
	{
		internal new CustomReportItemCellInstanceList this[int index]
		{
			get
			{
				return (CustomReportItemCellInstanceList)base[index];
			}
		}

		internal CustomReportItemCellInstancesList()
		{
		}

		internal CustomReportItemCellInstancesList(int capacity)
			: base(capacity)
		{
		}
	}
}
