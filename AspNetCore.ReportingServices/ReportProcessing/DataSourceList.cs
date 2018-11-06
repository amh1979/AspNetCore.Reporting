using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSourceList : ArrayList
	{
		internal new DataSource this[int index]
		{
			get
			{
				return (DataSource)base[index];
			}
		}

		internal DataSourceList()
		{
		}

		internal DataSourceList(int capacity)
			: base(capacity)
		{
		}
	}
}
