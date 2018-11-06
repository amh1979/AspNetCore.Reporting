using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing.Persistence
{
	internal sealed class MemberInfoList : ArrayList
	{
		internal new MemberInfo this[int index]
		{
			get
			{
				return (MemberInfo)base[index];
			}
		}

		internal MemberInfoList()
		{
		}

		internal MemberInfoList(int capacity)
			: base(capacity)
		{
		}
	}
}
