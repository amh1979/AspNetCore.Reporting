using AspNetCore.ReportingServices.Diagnostics;
using System;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal sealed class DataSourceHelper
	{
		private readonly byte[] m_encryptedDomainAndUserName;

		private readonly byte[] m_encryptedPassword;

		private readonly IDataProtection m_dp;

		public DataSourceHelper(byte[] encryptedDomainAndUserName, byte[] encryptedPassword, IDataProtection dataProtection)
		{
			this.m_encryptedDomainAndUserName = encryptedDomainAndUserName;
			this.m_encryptedPassword = encryptedPassword;
			if (dataProtection == null)
			{
				throw new ArgumentNullException("dataProtection");
			}
			this.m_dp = dataProtection;
		}

		public string GetPassword()
		{
			return this.m_dp.UnprotectDataToString(this.m_encryptedPassword, "Password");
		}

		public string GetUserName()
		{
			string domainAndUserName = this.m_dp.UnprotectDataToString(this.m_encryptedDomainAndUserName, "UserName");
			return DataSourceInfo.GetUserNameOnly(domainAndUserName);
		}

		public string GetDomainName()
		{
			string domainAndUserName = this.m_dp.UnprotectDataToString(this.m_encryptedDomainAndUserName, "UserName");
			return DataSourceInfo.GetDomainOnly(domainAndUserName);
		}
	}
}
