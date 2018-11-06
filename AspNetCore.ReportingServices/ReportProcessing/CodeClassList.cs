using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CodeClassList : ArrayList
	{
		internal new CodeClass this[int index]
		{
			get
			{
				return (CodeClass)base[index];
			}
		}

		internal CodeClassList()
		{
		}

		internal CodeClassList(int capacity)
			: base(capacity)
		{
		}
	}
}
