using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RunningValueInfoList : ArrayList
	{
		internal new RunningValueInfo this[int index]
		{
			get
			{
				return (RunningValueInfo)base[index];
			}
		}

		internal RunningValueInfoList()
		{
		}

		internal RunningValueInfoList(int capacity)
			: base(capacity)
		{
		}
	}
}
