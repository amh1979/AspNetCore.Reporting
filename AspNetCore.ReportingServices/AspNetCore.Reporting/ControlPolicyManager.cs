using AspNetCore.ReportingServices;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace AspNetCore.Reporting
{
	internal sealed class ControlPolicyManager : PolicyManager
	{
		private static StrongName[] m_baseFullTrustAssemblies;

		public override AppDomain CreateAppDomainWithPolicy(string appDomainName, Evidence evidence, SandboxCasPolicySettings casSettings)
		{
			PermissionSet permissionSet = casSettings.BasePermissions;
			if (permissionSet == null)
			{
				permissionSet = new PermissionSet(PermissionState.None);
				permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			}
			StrongName[] baseFullTrustAssemblies = ControlPolicyManager.GetBaseFullTrustAssemblies();
			int num = baseFullTrustAssemblies.Length;
			if (casSettings.FullTrustAssemblies != null)
			{
				num += casSettings.FullTrustAssemblies.Count;
			}
			StrongName[] array = new StrongName[num];
			Array.Copy(baseFullTrustAssemblies, array, baseFullTrustAssemblies.Length);
			if (casSettings.FullTrustAssemblies != null)
			{
				casSettings.FullTrustAssemblies.CopyTo(array, baseFullTrustAssemblies.Length);
			}
			return AppDomain.CreateDomain(appDomainName);
		}

		private static StrongName[] GetBaseFullTrustAssemblies()
		{
			if (ControlPolicyManager.m_baseFullTrustAssemblies == null)
			{
				StrongName[] array = ControlPolicyManager.m_baseFullTrustAssemblies = new StrongName[2]
				{
					ControlPolicyManager.CreateStrongName(typeof(ReportRuntime).Assembly),
					ControlPolicyManager.CreateStrongName(typeof(ObjectModel).Assembly)
				};
			}
			return ControlPolicyManager.m_baseFullTrustAssemblies;
		}

		private static StrongName CreateStrongName(Assembly assembly)
		{
			AssemblyName name = assembly.GetName();
			if (name == null)
			{
				throw new InvalidOperationException("Could not get assembly name");
			}
			byte[] publicKey = name.GetPublicKey();
			if (publicKey != null && publicKey.Length != 0)
			{
				StrongNamePublicKeyBlob blob = new StrongNamePublicKeyBlob(publicKey);
				return new StrongName(blob, name.Name, name.Version);
			}
			throw new InvalidOperationException("Assembly is not strongly named");
		}
	}
}
