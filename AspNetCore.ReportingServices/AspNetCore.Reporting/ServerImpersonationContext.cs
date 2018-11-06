using System;
using System.Security.Principal;

namespace AspNetCore.Reporting
{
	internal sealed class ServerImpersonationContext : IDisposable
	{
		private WindowsIdentity m_oldUser;

		public ServerImpersonationContext(WindowsIdentity userToImpersonate)
		{
			try
			{
				if (userToImpersonate != null)
				{
					this.m_oldUser = WindowsIdentity.GetCurrent();
					//userToImpersonate.Impersonate();
				}
			}
			catch
			{
				this.Dispose();
				throw;
			}
		}

		public void Dispose()
		{
			if (this.m_oldUser != null)
			{
				this.m_oldUser.Dispose();
			}
		}
	}
}
