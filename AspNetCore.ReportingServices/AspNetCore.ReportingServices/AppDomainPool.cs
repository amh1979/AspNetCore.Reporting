using System;
using System.Globalization;
using System.Security.Policy;

namespace AspNetCore.ReportingServices
{
	internal sealed class AppDomainPool
	{
		private Evidence m_evidence;

		//private AppDomainSetup m_setupInfo;

		private bool m_policyChanged;

		private PolicyManager m_policyManager;

		private RefCountedAppDomain m_lastDispensedAppDomain;

		private SandboxCasPolicySettings m_settingsForLastDispensedAppDomain;

		private DateTime m_lastAppDomainCreationTime = DateTime.MinValue;

		private bool m_areAppDomainsReusable;

		public PolicyManager PolicyManager
		{
			get
			{
				return this.m_policyManager;
			}
		}

		public AppDomainPool(bool allowAppDomainReuse, Evidence evidence,  PolicyManager policyManager)
		{
			this.m_areAppDomainsReusable = allowAppDomainReuse;
			this.m_evidence = evidence;
			//this.m_setupInfo = setupInfo;
			this.m_policyManager = policyManager;
			this.m_policyManager.PolicyChanged += this.OnPolicyChanged;
			this.m_policyChanged = true;
		}

		public RefCountedAppDomain GetAppDomain(SandboxCasPolicySettings casSettings)
		{
			lock (this)
			{
				if (this.m_lastDispensedAppDomain != null && !this.IsLastAppDomainReusable(casSettings))
				{
					try
					{
						this.m_lastDispensedAppDomain.Dispose();
					}
					finally
					{
						this.m_lastDispensedAppDomain = null;
					}
				}
				if (this.m_lastDispensedAppDomain == null)
				{
					DateTime now = DateTime.Now;
					AppDomain appDomain = this.CreateAppDomain(now, casSettings);
					RefCountedAppDomain refCountedAppDomain = new RefCountedAppDomain(appDomain);
					if (this.m_areAppDomainsReusable)
					{
						this.m_lastDispensedAppDomain = refCountedAppDomain.CreateNewReference();
						this.m_lastAppDomainCreationTime = now;
						this.m_settingsForLastDispensedAppDomain = casSettings.Copy();
					}
					return refCountedAppDomain;
				}
				return this.m_lastDispensedAppDomain.CreateNewReference();
			}
		}

		private AppDomain CreateAppDomain(DateTime timeStamp, SandboxCasPolicySettings casSettings)
		{
			if (this.m_policyChanged)
			{
				lock (this)
				{
					this.m_policyChanged = false;
				}
			}
			string appDomainName = "Local Processing " + timeStamp.ToString(CultureInfo.InvariantCulture);
			return this.m_policyManager.CreateAppDomainWithPolicy(appDomainName, this.m_evidence, casSettings);
		}

		private bool IsLastAppDomainReusable(SandboxCasPolicySettings casSettings)
		{
			if (this.m_policyChanged)
			{
				return false;
			}
			if (!object.Equals(this.m_settingsForLastDispensedAppDomain, casSettings))
			{
				return false;
			}
			TimeSpan t = DateTime.Now - this.m_lastAppDomainCreationTime;
			if (t < TimeSpan.FromMinutes(1.0))
			{
				return t >= TimeSpan.FromMinutes(0.0);
			}
			return false;
		}

		private void OnPolicyChanged(object sender, EventArgs e)
		{
			lock (this)
			{
				this.m_policyChanged = true;
			}
		}
	}
}
