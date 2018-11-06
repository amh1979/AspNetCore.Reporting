using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartMemberList : HierarchyNodeList
	{
		internal new ChartMember this[int index]
		{
			get
			{
				return (ChartMember)base[index];
			}
		}

		public ChartMemberList()
		{
		}

		internal ChartMemberList(int capacity)
			: base(capacity)
		{
		}
	}
}
