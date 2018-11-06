using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterValueList : ArrayList
	{
		internal new ParameterValue this[int index]
		{
			get
			{
				return (ParameterValue)base[index];
			}
		}

		internal ParameterValueList()
		{
		}

		internal ParameterValueList(int capacity)
			: base(capacity)
		{
		}
	}
}
