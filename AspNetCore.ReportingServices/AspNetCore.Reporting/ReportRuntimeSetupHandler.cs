using AspNetCore.ReportingServices;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class ReportRuntimeSetupHandler : IDisposable
	{
		private enum TriState
		{
			Unknown,
			True,
			False
		}

		private ReportRuntimeSetup m_reportRuntimeSetup;

		private TriState m_executeInSandbox;

		private TriState m_isAppDomainCasPolicyEnabled;

		private SandboxCasPolicySettings m_sandboxCasSettings;

		[NonSerialized]
		private RefCountedAppDomain m_exprHostSandboxAppDomain;

		[NonSerialized]
		private bool m_isExprHostSandboxAppDomainDirty;

		private static AppDomainPool m_appDomainPool;

		private static readonly object m_appDomainPoolLoaderLock = new object();

		internal ReportRuntimeSetup ReportRuntimeSetup
		{
			[SecurityCritical]
			
			get
			{
				if (this.m_isExprHostSandboxAppDomainDirty)
				{
					this.ReleaseSandboxAppDomain();
					this.m_isExprHostSandboxAppDomainDirty = false;
				}
				this.EnsureSandboxAppDomainIfNeeded();
				return this.GetReportRuntimeSetup();
			}
		}

		internal bool RequireExpressionHostWithRefusedPermissions
		{
			
			[SecurityCritical]
			[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
			get
			{
				return this.GetReportRuntimeSetup().RequireExpressionHostWithRefusedPermissions;
			}
		}

		private bool IsAppDomainCasPolicyEnabled
		{
			
			[SecurityCritical]
			[SecurityPermission(SecurityAction.Assert, ControlDomainPolicy = true)]
			get
			{
				if (this.m_isAppDomainCasPolicyEnabled == TriState.Unknown)
				{
					if (AppDomain.CurrentDomain.IsFullyTrusted)
					{
						this.m_isAppDomainCasPolicyEnabled = TriState.True;
					}
					else
					{
						this.m_isAppDomainCasPolicyEnabled = TriState.False;
					}
				}
				return this.m_isAppDomainCasPolicyEnabled == TriState.True;
			}
		}

		private bool ExecuteInSandbox
		{
			[SecurityCritical]
			get
			{
				if (this.m_executeInSandbox == TriState.Unknown)
				{
					if (this.IsAppDomainCasPolicyEnabled)
					{
						this.m_executeInSandbox = TriState.False;
					}
					else
					{
						this.m_executeInSandbox = TriState.True;
					}
				}
				return this.m_executeInSandbox == TriState.True;
			}
		}

		internal ReportRuntimeSetupHandler()
		{
			this.m_executeInSandbox = TriState.Unknown;
			this.m_isAppDomainCasPolicyEnabled = TriState.Unknown;
			this.m_sandboxCasSettings = new SandboxCasPolicySettings();
		}

		[SecurityCritical]
		private ReportRuntimeSetup GetReportRuntimeSetup()
		{
			if (this.m_reportRuntimeSetup == null)
			{
				if (this.ExecuteInSandbox)
				{
					this.ExecuteReportInSandboxAppDomain();
				}
				else
				{
					this.ExecuteReportInCurrentAppDomain();
				}
			}
			return this.m_reportRuntimeSetup;
		}

		public void Dispose()
		{
			this.ReleaseSandboxAppDomain();
		}

		public void ReleaseSandboxAppDomain()
		{
			if (this.m_exprHostSandboxAppDomain != null)
			{
				this.m_exprHostSandboxAppDomain.Dispose();
				this.m_exprHostSandboxAppDomain = null;
				ReportRuntimeSetupHandler.m_appDomainPool.PolicyManager.PolicyChanged -= this.OnPolicyChanged;
			}
		}

		
		[SecurityCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		internal void ExecuteReportInCurrentAppDomain()
		{
			if (!this.IsAppDomainCasPolicyEnabled)
			{
				throw new InvalidOperationException(ProcessingStrings.CasPolicyUnavailableForCurrentAppDomain);
			}
			this.SetAppDomain(false);
			this.m_reportRuntimeSetup = ReportRuntimeSetup.CreateForCurrentAppDomainExecution();
		}

		[SecurityCritical]
		
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		internal void ExecuteReportInCurrentAppDomain(Evidence reportEvidence)
		{
			if (!this.IsAppDomainCasPolicyEnabled)
			{
				throw new InvalidOperationException(ProcessingStrings.CasPolicyUnavailableForCurrentAppDomain);
			}
			this.SetAppDomain(false);
			this.m_reportRuntimeSetup = ReportRuntimeSetup.CreateForCurrentAppDomainExecution(reportEvidence);
		}

		
		[SecurityCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		internal void ExecuteReportInSandboxAppDomain()
		{
			this.SetAppDomain(true);
			AppDomain exprHostAppDomain = null;
			if (this.m_exprHostSandboxAppDomain != null)
			{
				exprHostAppDomain = this.m_exprHostSandboxAppDomain.AppDomain;
			}
			this.m_reportRuntimeSetup = ReportRuntimeSetup.CreateForSeparateAppDomainExecution(exprHostAppDomain);
		}

		[SecurityCritical]
		
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		internal void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
		{
			if (!this.IsAppDomainCasPolicyEnabled)
			{
				throw new InvalidOperationException(ProcessingStrings.CasPolicyUnavailableForCurrentAppDomain);
			}
			this.GetReportRuntimeSetup().AddTrustedCodeModuleInCurrentAppDomain(assemblyName);
		}

		[SecurityCritical]
		
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		internal void AddFullTrustModuleInSandboxAppDomain(StrongName assemblyName)
		{
			this.m_isExprHostSandboxAppDomainDirty = true;
			this.m_sandboxCasSettings.AddFullTrustAssembly(assemblyName);
		}

		
		[SecurityCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		internal void SetBasePermissionsForSandboxAppDomain(PermissionSet permissions)
		{
			this.m_isExprHostSandboxAppDomainDirty = true;
			this.m_sandboxCasSettings.BasePermissions = permissions;
		}

		private void SetAppDomain(bool useSandBoxAppDomain)
		{
			if (useSandBoxAppDomain)
			{
				this.m_executeInSandbox = TriState.True;
			}
			else
			{
				this.m_executeInSandbox = TriState.False;
				this.ReleaseSandboxAppDomain();
			}
		}

		[SecurityCritical]
		private void EnsureSandboxAppDomainIfNeeded()
		{
			if (this.ExecuteInSandbox && this.m_exprHostSandboxAppDomain == null)
			{
				this.m_exprHostSandboxAppDomain = ReportRuntimeSetupHandler.m_appDomainPool.GetAppDomain(this.m_sandboxCasSettings);
				ReportRuntimeSetupHandler.m_appDomainPool.PolicyManager.PolicyChanged += this.OnPolicyChanged;
				this.m_reportRuntimeSetup = new ReportRuntimeSetup(this.GetReportRuntimeSetup(), this.m_exprHostSandboxAppDomain.AppDomain);
			}
		}

		public static void InitAppDomainPool(Evidence sandboxEvidence, PolicyManager policyManager)
		{
            if (ReportRuntimeSetupHandler.m_appDomainPool == null)
            {
                lock (ReportRuntimeSetupHandler.m_appDomainPoolLoaderLock)
                {
                    if (ReportRuntimeSetupHandler.m_appDomainPool == null)
                    {
                        ReportRuntimeSetupHandler.m_appDomainPool = new AppDomainPool(false, sandboxEvidence, policyManager);
                    }
                }
            }
        }

		private void OnPolicyChanged(object sender, EventArgs e)
		{
			this.m_isExprHostSandboxAppDomainDirty = true;
		}
	}
}
