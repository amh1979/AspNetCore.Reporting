namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class PageItemContainerHelper : PageItemHelper
	{
		private bool m_itemsCreated;

		private int[] m_indexesLeftToRight;

		private int[] m_indexesTopToBottom;

		private PageItemHelper[] m_repeatWithItems;

		private PageItemHelper m_rightEdgeItem;

		private PageItemHelper[] m_children;

		internal bool ItemsCreated
		{
			get
			{
				return this.m_itemsCreated;
			}
			set
			{
				this.m_itemsCreated = value;
			}
		}

		internal int[] IndexesLeftToRight
		{
			get
			{
				return this.m_indexesLeftToRight;
			}
			set
			{
				this.m_indexesLeftToRight = value;
			}
		}

		internal int[] IndexesTopToBottom
		{
			get
			{
				return this.m_indexesTopToBottom;
			}
			set
			{
				this.m_indexesTopToBottom = value;
			}
		}

		internal PageItemHelper[] RepeatWithItems
		{
			get
			{
				return this.m_repeatWithItems;
			}
			set
			{
				this.m_repeatWithItems = value;
			}
		}

		internal PageItemHelper RightEdgeItem
		{
			get
			{
				return this.m_rightEdgeItem;
			}
			set
			{
				this.m_rightEdgeItem = value;
			}
		}

		internal PageItemHelper[] Children
		{
			get
			{
				return this.m_children;
			}
			set
			{
				this.m_children = value;
			}
		}

		internal PageItemContainerHelper(byte type)
			: base(type)
		{
		}
	}
}
