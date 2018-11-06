using AspNetCore.ReportingServices.Diagnostics;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ConnectionContext
	{
		internal string DataSourceType
		{
			get;
			set;
		}

		internal ConnectionSecurity ConnectionSecurity
		{
			get;
			set;
		}

		internal string ConnectionString
		{
			get;
			set;
		}

		internal string DomainName
		{
			get;
			set;
		}

		internal string UserName
		{
			get;
			set;
		}

		internal bool ImpersonateUser
		{
			get;
			set;
		}

		internal string ImpersonateUserName
		{
			get;
			set;
		}

		internal SecureStringWrapper Password
		{
			get;
			set;
		}

		internal string DecryptedPassword
		{
			get
			{
				if (this.Password != null)
				{
					return this.Password.ToString();
				}
				return string.Empty;
			}
		}

		internal ConnectionContext()
		{
			this.ConnectionSecurity = ConnectionSecurity.None;
		}

		internal ConnectionKey CreateConnectionKey()
		{
			return new ConnectionKey(this.DataSourceType, this.ConnectionString, this.ConnectionSecurity, this.DomainName, this.UserName, this.ImpersonateUser, this.ImpersonateUserName);
		}
	}
}
