using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Security.Policy;

namespace AspNetCore.ReportingServices
{
	[Serializable]
	internal class SandboxCasPolicySettings
	{
		private PermissionSet m_basePermissions;

		private List<StrongName> m_fullTrustAssemblies;

		private ReadOnlyCollection<StrongName> m_fullTrustAssembliesReadOnly;

		public PermissionSet BasePermissions
		{
			get
			{
				return this.m_basePermissions;
			}
			set
			{
				this.m_basePermissions = value;
			}
		}

		public ReadOnlyCollection<StrongName> FullTrustAssemblies
		{
			get
			{
				return this.m_fullTrustAssembliesReadOnly;
			}
		}

		public void AddFullTrustAssembly(StrongName assemblyName)
		{
			if (this.m_fullTrustAssemblies == null)
			{
				this.m_fullTrustAssemblies = new List<StrongName>();
				this.m_fullTrustAssembliesReadOnly = new ReadOnlyCollection<StrongName>(this.m_fullTrustAssemblies);
			}
			this.m_fullTrustAssemblies.Add(assemblyName);
		}

		public SandboxCasPolicySettings Copy()
		{
			SandboxCasPolicySettings sandboxCasPolicySettings = new SandboxCasPolicySettings();
			if (this.m_basePermissions != null)
			{
				sandboxCasPolicySettings.m_basePermissions = this.m_basePermissions.Copy();
			}
			if (this.m_fullTrustAssemblies != null)
			{
				{
					foreach (StrongName fullTrustAssembly in this.m_fullTrustAssemblies)
					{
						sandboxCasPolicySettings.AddFullTrustAssembly(fullTrustAssembly);
					}
					return sandboxCasPolicySettings;
				}
			}
			return sandboxCasPolicySettings;
		}

		public override bool Equals(object obj)
		{
			SandboxCasPolicySettings sandboxCasPolicySettings = obj as SandboxCasPolicySettings;
			if (sandboxCasPolicySettings == null)
			{
				return false;
			}
			if (!object.Equals(this.m_basePermissions, sandboxCasPolicySettings.m_basePermissions))
			{
				return false;
			}
			if (this.m_fullTrustAssemblies == null)
			{
				if (sandboxCasPolicySettings.m_fullTrustAssemblies != null)
				{
					return false;
				}
			}
			else
			{
				if (sandboxCasPolicySettings.m_fullTrustAssemblies == null)
				{
					return false;
				}
				int count = this.m_fullTrustAssemblies.Count;
				if (count != sandboxCasPolicySettings.m_fullTrustAssemblies.Count)
				{
					return false;
				}
				for (int i = 0; i < count; i++)
				{
					if (!object.Equals(this.m_fullTrustAssemblies[i], sandboxCasPolicySettings.m_fullTrustAssemblies[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (this.m_basePermissions != null)
			{
				num ^= this.m_basePermissions.GetHashCode();
			}
			if (this.m_fullTrustAssemblies != null)
			{
				num ^= this.m_fullTrustAssemblies.GetHashCode();
			}
			return num;
		}
	}
}
