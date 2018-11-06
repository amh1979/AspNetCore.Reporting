using System;
using System.Collections.Specialized;
using System.Security.Policy;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportRuntimeSetup
	{
		private static ReportRuntimeSetup DefaultRuntimeSetup;

		[NonSerialized]
		private readonly AppDomain m_exprHostAppDomain;

		private readonly Evidence m_exprHostEvidence;

		private readonly bool m_restrictCodeModulesInCurrentAppDomain;

		private StringCollection m_currentAppDomainTrustedCodeModules;

		private readonly bool m_requireExpressionHostWithRefusedPermissions;

		public bool ExecutesInSeparateAppDomain
		{
			get
			{
				return this.ExprHostAppDomain != null;
			}
		}

		internal AppDomain ExprHostAppDomain
		{
			get
			{
				return this.m_exprHostAppDomain;
			}
		}

		internal Evidence ExprHostEvidence
		{
			get
			{
				return this.m_exprHostEvidence;
			}
		}

		public bool RequireExpressionHostWithRefusedPermissions
		{
			get
			{
				return this.m_requireExpressionHostWithRefusedPermissions;
			}
		}

		public ReportRuntimeSetup(ReportRuntimeSetup originalSetup, AppDomain newAppDomain)
			: this(newAppDomain, originalSetup.m_exprHostEvidence, originalSetup.m_restrictCodeModulesInCurrentAppDomain, originalSetup.m_requireExpressionHostWithRefusedPermissions)
		{
			if (originalSetup.m_currentAppDomainTrustedCodeModules != null)
			{
				StringEnumerator enumerator = originalSetup.m_currentAppDomainTrustedCodeModules.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						this.AddTrustedCodeModuleInCurrentAppDomain(current);
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		public static ReportRuntimeSetup GetDefault()
		{
			if (ReportRuntimeSetup.DefaultRuntimeSetup == null)
			{
				ReportRuntimeSetup.DefaultRuntimeSetup = new ReportRuntimeSetup(null, null, false, false);
			}
			return ReportRuntimeSetup.DefaultRuntimeSetup;
		}

		public static ReportRuntimeSetup CreateForSeparateAppDomainExecution(AppDomain exprHostAppDomain)
		{
			return new ReportRuntimeSetup(exprHostAppDomain, null, false, false);
		}

		public static ReportRuntimeSetup CreateForCurrentAppDomainExecution()
		{
			return new ReportRuntimeSetup(null, null, true, true);
		}

		public static ReportRuntimeSetup CreateForCurrentAppDomainExecution(Evidence exprHostEvidence)
		{
			return new ReportRuntimeSetup(null, exprHostEvidence, true, false);
		}

		public void AddTrustedCodeModuleInCurrentAppDomain(string assemblyName)
		{
			if (this.m_currentAppDomainTrustedCodeModules == null)
			{
				this.m_currentAppDomainTrustedCodeModules = new StringCollection();
			}
			if (!this.m_currentAppDomainTrustedCodeModules.Contains(assemblyName))
			{
				this.m_currentAppDomainTrustedCodeModules.Add(assemblyName);
			}
		}

		internal bool CheckCodeModuleIsTrustedInCurrentAppDomain(string assemblyName)
		{
			if (this.m_restrictCodeModulesInCurrentAppDomain)
			{
				if (this.m_currentAppDomainTrustedCodeModules != null)
				{
					return this.m_currentAppDomainTrustedCodeModules.Contains(assemblyName);
				}
				return false;
			}
			return true;
		}

		private ReportRuntimeSetup(AppDomain exprHostAppDomain, Evidence exprHostEvidence, bool restrictCodeModulesInCurrentAppDomain, bool requireExpressionHostWithRefusedPermissions)
		{
			this.m_exprHostAppDomain = exprHostAppDomain;
			this.m_exprHostEvidence = exprHostEvidence;
			this.m_restrictCodeModulesInCurrentAppDomain = restrictCodeModulesInCurrentAppDomain;
			this.m_requireExpressionHostWithRefusedPermissions = requireExpressionHostWithRefusedPermissions;
		}
	}
}
