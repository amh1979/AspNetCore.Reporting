using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AspNetCore.Reporting
{
	[ComVisible(false)]
	internal sealed class SortEventArgs : CancelEventArgs
	{
		private string m_sortId;

		private SortOrder m_sortDirection;

		private bool m_clearSort;

		public string SortId
		{
			get
			{
				return this.m_sortId;
			}
		}

		public SortOrder SortDirection
		{
			get
			{
				return this.m_sortDirection;
			}
		}

		public bool ClearSort
		{
			get
			{
				return this.m_clearSort;
			}
		}

		public SortEventArgs(string sortId, SortOrder sortDirection, bool clearSort)
		{
			this.m_sortId = sortId;
			this.m_sortDirection = sortDirection;
			this.m_clearSort = clearSort;
		}
	}
}
