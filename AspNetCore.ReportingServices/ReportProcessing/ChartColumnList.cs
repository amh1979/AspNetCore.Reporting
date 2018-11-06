using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartColumnList : ArrayList
	{
		internal new ChartColumn this[int index]
		{
			get
			{
				return (ChartColumn)base[index];
			}
		}

		internal ChartColumnList()
		{
		}

		internal ChartColumnList(int capacity)
			: base(capacity)
		{
		}
	}
}
