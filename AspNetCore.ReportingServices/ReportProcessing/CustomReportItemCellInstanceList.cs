using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemCellInstanceList : ArrayList
	{
		internal new CustomReportItemCellInstance this[int index]
		{
			get
			{
				return (CustomReportItemCellInstance)base[index];
			}
		}

		internal CustomReportItemCellInstanceList()
		{
		}

		internal CustomReportItemCellInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
