using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class FiltersList : ArrayList
	{
		internal new Filters this[int index]
		{
			get
			{
				return (Filters)base[index];
			}
		}
	}
}
