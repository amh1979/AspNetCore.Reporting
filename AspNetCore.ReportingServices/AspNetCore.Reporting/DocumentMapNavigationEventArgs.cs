using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AspNetCore.Reporting
{
	[ComVisible(false)]
	internal sealed class DocumentMapNavigationEventArgs : CancelEventArgs
	{
		private string m_docMapID;

		public string DocumentMapId
		{
			get
			{
				return this.m_docMapID;
			}
		}

		public DocumentMapNavigationEventArgs(string docMapID)
		{
			this.m_docMapID = docMapID;
		}
	}
}
