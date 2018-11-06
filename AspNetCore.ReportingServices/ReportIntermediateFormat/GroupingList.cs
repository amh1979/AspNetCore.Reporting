using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class GroupingList : ArrayList
	{
		internal new Grouping this[int index]
		{
			get
			{
				return (Grouping)base[index];
			}
		}

		internal Grouping LastEntry
		{
			get
			{
				if (this.Count == 0)
				{
					return null;
				}
				return this[this.Count - 1];
			}
		}

		public GroupingList()
		{
		}

		internal GroupingList(int capacity)
			: base(capacity)
		{
		}

		internal object PublishClone(AutomaticSubtotalContext context, ReportHierarchyNode owner)
		{
			int count = this.Count;
			GroupingList groupingList = new GroupingList(count);
			for (int i = 0; i < count; i++)
			{
				groupingList.Add(this[i].PublishClone(context, owner));
			}
			return groupingList;
		}

		internal new GroupingList Clone()
		{
			int count = this.Count;
			GroupingList groupingList = new GroupingList(count);
			for (int i = 0; i < count; i++)
			{
				groupingList.Add(this[i]);
			}
			return groupingList;
		}
	}
}
