namespace AspNetCore.ReportingServices.DataExtensions
{
	internal sealed class ServerDataSourceSettings
	{
		private bool m_allowIntegratedSecurity = true;

		private bool m_isSurrogatePresent;

		public bool IsSurrogatePresent
		{
			get
			{
				return this.m_isSurrogatePresent;
			}
		}

		public bool AllowIntegratedSecurity
		{
			get
			{
				return this.m_allowIntegratedSecurity;
			}
		}

		public ServerDataSourceSettings(bool isSurrogatePresent, bool allowIntegratedSecurity)
		{
			this.m_isSurrogatePresent = isSurrogatePresent;
			this.m_allowIntegratedSecurity = allowIntegratedSecurity;
		}
	}
}
