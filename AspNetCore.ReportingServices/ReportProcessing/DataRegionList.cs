using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	[ArrayOfReferences]
	internal sealed class DataRegionList : ArrayList
	{
		internal new DataRegion this[int index]
		{
			get
			{
				return (DataRegion)base[index];
			}
		}

		internal DataRegionList()
		{
		}

		internal DataRegionList(int capacity)
			: base(capacity)
		{
		}
	}
}
