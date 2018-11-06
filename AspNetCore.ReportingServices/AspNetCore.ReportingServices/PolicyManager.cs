using System;
using System.Security.Policy;

namespace AspNetCore.ReportingServices
{
	internal abstract class PolicyManager
	{
		public event EventHandler PolicyChanged;

		public abstract AppDomain CreateAppDomainWithPolicy(string appDomainName, Evidence evidence, SandboxCasPolicySettings casSettings);

		protected void OnPolicyChanged()
		{
			if (this.PolicyChanged != null)
			{
				this.PolicyChanged(this, EventArgs.Empty);
			}
		}
	}
}
