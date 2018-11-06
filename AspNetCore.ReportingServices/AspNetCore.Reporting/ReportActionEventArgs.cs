using System;

namespace AspNetCore.Reporting
{
	internal class ReportActionEventArgs : EventArgs
	{
		private string m_actionType;

		private string m_actionParam;

		public string ActionType
		{
			get
			{
				return this.m_actionType;
			}
		}

		public string ActionParam
		{
			get
			{
				return this.m_actionParam;
			}
		}

		internal ReportActionEventArgs(string actionType, string actionParam)
		{
			this.m_actionType = actionType;
			this.m_actionParam = actionParam;
		}
	}
}
