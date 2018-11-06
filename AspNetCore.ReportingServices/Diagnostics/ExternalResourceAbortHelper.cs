namespace AspNetCore.ReportingServices.Diagnostics
{
	internal class ExternalResourceAbortHelper : IAbortHelper
	{
		private bool m_isAborted;

		public bool IsAborted
		{
			get
			{
				return this.m_isAborted;
			}
		}

		public bool Abort(ProcessingStatus status)
		{
			this.m_isAborted = true;
			return true;
		}
	}
}
