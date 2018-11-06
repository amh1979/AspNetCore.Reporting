using System.Data.Common;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ConnectionKey
	{
		private readonly string m_dataSourceType;

		private readonly string m_connectionString;

		private readonly ConnectionSecurity m_connectionSecurity;

		private readonly string m_domainName;

		private readonly string m_userName;

		private readonly bool m_impersonateUser;

		private readonly string m_impersonateUserName;

		private int m_hashCode = -1;

		private string m_hashCodeString;

		public string DataSourceType
		{
			get
			{
				return this.m_dataSourceType;
			}
		}

		public bool IsOnPremiseConnection
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_connectionString))
				{
					return false;
				}
				DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder();
				dbConnectionStringBuilder.ConnectionString = this.m_connectionString;
				return dbConnectionStringBuilder.ContainsKey("External Tenant Id");
			}
		}

		public ConnectionKey(string dataSourceType, string connectionString, ConnectionSecurity connectionSecurity, string domainName, string userName, bool impersonateUser, string impersonateUserName)
		{
			this.m_dataSourceType = dataSourceType;
			this.m_connectionString = connectionString;
			this.m_connectionSecurity = connectionSecurity;
			this.m_domainName = domainName;
			this.m_userName = userName;
			this.m_impersonateUser = impersonateUser;
			this.m_impersonateUserName = impersonateUserName;
		}

		public string GetKeyString()
		{
			if (this.m_hashCodeString == null)
			{
				this.m_hashCodeString = this.GetHashCode().ToString(CultureInfo.InvariantCulture);
			}
			return this.m_hashCodeString;
		}

		public override int GetHashCode()
		{
			if (this.m_hashCode == -1)
			{
				this.m_hashCode = this.m_connectionSecurity.GetHashCode();
				ConnectionKey.HashCombine(ref this.m_hashCode, this.m_impersonateUser.GetHashCode());
				if (this.m_dataSourceType != null)
				{
					ConnectionKey.HashCombine(ref this.m_hashCode, this.m_dataSourceType.GetHashCode());
				}
				if (this.m_connectionString != null)
				{
					ConnectionKey.HashCombine(ref this.m_hashCode, this.m_connectionString.GetHashCode());
				}
				if (this.m_domainName != null)
				{
					ConnectionKey.HashCombine(ref this.m_hashCode, this.m_domainName.GetHashCode());
				}
				if (this.m_userName != null)
				{
					ConnectionKey.HashCombine(ref this.m_hashCode, this.m_userName.GetHashCode());
				}
				if (this.m_impersonateUserName != null)
				{
					ConnectionKey.HashCombine(ref this.m_hashCode, this.m_impersonateUserName.GetHashCode());
				}
			}
			return this.m_hashCode;
		}

		public override bool Equals(object obj)
		{
			ConnectionKey connectionKey = obj as ConnectionKey;
			if (connectionKey != null && this.m_dataSourceType == connectionKey.m_dataSourceType && this.m_connectionString == connectionKey.m_connectionString && this.m_connectionSecurity == connectionKey.m_connectionSecurity && this.m_domainName == connectionKey.m_domainName && this.m_userName == connectionKey.m_userName && this.m_impersonateUser == connectionKey.m_impersonateUser && this.m_impersonateUserName == connectionKey.m_impersonateUserName)
			{
				return true;
			}
			return false;
		}

		public bool ShouldCheckIsAlive()
		{
			if (this.DataSourceType == null)
			{
				return true;
			}
			if (!this.DataSourceType.EndsWith("-Native"))
			{
				return !this.DataSourceType.EndsWith("-Managed");
			}
			return false;
		}

		private static void HashCombine(ref int seed, int other)
		{
			uint num = (uint)seed;
			num = (uint)(seed = ((int)num ^ other + -1640531527 + (int)(num << 6) + (int)(num >> 2)));
		}
	}
}
