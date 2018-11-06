using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataFieldList : ArrayList
	{
		internal new Field this[int index]
		{
			get
			{
				return (Field)base[index];
			}
		}

		internal DataFieldList()
		{
		}

		internal DataFieldList(int capacity)
			: base(capacity)
		{
		}
	}
}
