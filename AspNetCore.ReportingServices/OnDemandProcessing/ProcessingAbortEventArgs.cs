using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class ProcessingAbortEventArgs : EventArgs
	{
		private string m_uniqueName;

		internal string UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
		}

		internal ProcessingAbortEventArgs(string uniqueName)
		{
			this.m_uniqueName = uniqueName;
		}
	}
}
