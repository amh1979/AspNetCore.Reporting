using System.Collections;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class SortPageItemIndexesOnTop : IComparer
	{
		private PageItem[] m_children;

		internal SortPageItemIndexesOnTop(PageItem[] children)
		{
			this.m_children = children;
		}

		int IComparer.Compare(object o1, object o2)
		{
			int num = (int)o1;
			int num2 = (int)o2;
			if (this.m_children[num] != null && this.m_children[num2] != null)
			{
				if (this.m_children[num].ItemPageSizes.Top == this.m_children[num2].ItemPageSizes.Top)
				{
					if (this.m_children[num].ItemPageSizes.Left == this.m_children[num2].ItemPageSizes.Left)
					{
						return 0;
					}
					if (this.m_children[num].ItemPageSizes.Left < this.m_children[num2].ItemPageSizes.Left)
					{
						return -1;
					}
					return 1;
				}
				if (this.m_children[num].ItemPageSizes.Top < this.m_children[num2].ItemPageSizes.Top)
				{
					return -1;
				}
				return 1;
			}
			return 0;
		}
	}
}
