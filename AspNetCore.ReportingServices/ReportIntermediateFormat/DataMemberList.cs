using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataMemberList : HierarchyNodeList
	{
		internal new DataMember this[int index]
		{
			get
			{
				return (DataMember)base[index];
			}
		}

		public DataMemberList()
		{
		}

		internal DataMemberList(int capacity)
			: base(capacity)
		{
		}
	}
}
