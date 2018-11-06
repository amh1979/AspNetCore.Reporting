using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class SortedReportItemIndexList : ArrayList
	{
		internal new int this[int index]
		{
			get
			{
				return (int)base[index];
			}
		}

		internal SortedReportItemIndexList()
		{
		}

		internal SortedReportItemIndexList(int capacity)
			: base(capacity)
		{
		}

		public void Add(ReportItemList collection, int collectionIndex, bool sortVertically)
		{
			Global.Tracer.Assert(null != collection);
			ReportItem reportItem = collection[collectionIndex];
			int num = 0;
			while (num < base.Count)
			{
				if (sortVertically && reportItem.AbsoluteTopValue > collection[this[num]].AbsoluteTopValue)
				{
					num++;
				}
				else
				{
					if (sortVertically)
					{
						break;
					}
					if (!(reportItem.AbsoluteLeftValue > collection[this[num]].AbsoluteLeftValue))
					{
						break;
					}
					num++;
				}
			}
			base.Insert(num, collectionIndex);
		}
	}
}
