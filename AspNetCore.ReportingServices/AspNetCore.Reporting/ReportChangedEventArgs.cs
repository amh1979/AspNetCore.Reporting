using System;

namespace AspNetCore.Reporting
{
	internal class ReportChangedEventArgs : EventArgs
	{
		private bool m_isRefreshOnly;

		public bool IsRefreshOnly
		{
			get
			{
				return this.m_isRefreshOnly;
			}
		}

		public ReportChangedEventArgs()
			: this(false)
		{
		}

		public ReportChangedEventArgs(bool isRefreshOnly)
		{
			this.m_isRefreshOnly = isRefreshOnly;
		}
	}
}
