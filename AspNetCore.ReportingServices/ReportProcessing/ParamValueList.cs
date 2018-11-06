using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ParamValueList : ArrayList
	{
		internal new ParamValue this[int index]
		{
			get
			{
				return (ParamValue)base[index];
			}
		}

		internal ParamValueList()
		{
		}

		internal ParamValueList(int capacity)
			: base(capacity)
		{
		}
	}
}
