using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TablixMemberList : HierarchyNodeList
	{
		internal new TablixMember this[int index]
		{
			get
			{
				return (TablixMember)base[index];
			}
		}

		public TablixMemberList()
		{
		}

		internal TablixMemberList(int capacity)
			: base(capacity)
		{
		}
	}
}
