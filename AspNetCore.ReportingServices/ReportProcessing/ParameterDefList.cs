using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParameterDefList : ArrayList
	{
		internal new ParameterDef this[int index]
		{
			get
			{
				return (ParameterDef)base[index];
			}
		}

		public ParameterDefList()
		{
		}

		internal ParameterDefList(int capacity)
			: base(capacity)
		{
		}
	}
}
