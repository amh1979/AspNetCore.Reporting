

namespace AspNetCore.Reporting
{
	internal sealed class DataSourceCredentials
	{
		private string m_name = "";

		private string m_userID = "";

		private string m_password = "";

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public string UserId
		{
			get
			{
				return this.m_userID;
			}
			set
			{
				this.m_userID = value;
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


	}
}
