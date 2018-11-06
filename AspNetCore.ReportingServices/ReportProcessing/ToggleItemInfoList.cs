using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ToggleItemInfoList : ArrayList
	{
		internal new ToggleItemInfo this[int index]
		{
			get
			{
				return (ToggleItemInfo)base[index];
			}
		}
	}
}
