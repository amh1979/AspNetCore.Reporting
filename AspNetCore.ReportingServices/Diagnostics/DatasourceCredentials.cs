using System;

namespace AspNetCore.ReportingServices.Diagnostics
{
	[Serializable]
	internal sealed class DatasourceCredentials
	{
		private string m_userName;

		private string m_password;

		private string m_promptID;

		public string UserName
		{
			get
			{
				return this.m_userName;
			}
		}

		public string Password
		{
			get
			{
				return this.m_password;
			}
			set
			{
				this.m_password = value;
			}
		}

		public string PromptID
		{
			get
			{
				return this.m_promptID;
			}
		}

		public DatasourceCredentials(string promptID, string userName, string password)
		{
			this.m_promptID = promptID;
			this.m_userName = userName;
			this.m_password = password;
		}
	}
}
