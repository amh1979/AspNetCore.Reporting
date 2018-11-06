using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Security.Principal;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal class UserContext
	{
		protected string m_userName;

		protected object m_userToken;

		protected AuthenticationType m_authType;

		protected bool m_initialized;

		protected byte[] m_additionalUserToken;

		public string UserName
		{
			get
			{
				return this.m_userName;
			}
		}

		public object UserToken
		{
			get
			{
				return this.m_userToken;
			}
		}

		public AuthenticationType AuthenticationType
		{
			get
			{
				return this.m_authType;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return this.m_initialized;
			}
		}

		public byte[] AdditionalUserToken
		{
			get
			{
				return this.m_additionalUserToken;
			}
			set
			{
				this.m_additionalUserToken = value;
			}
		}

		internal virtual bool UseAdditionalToken
		{
			get
			{
				return true;
			}
		}
        /*
		internal virtual WindowsIdentity GetWindowsIdentity()
		{
            
			if (this.m_authType != AuthenticationType.Windows)
			{
				throw new WindowsIntegratedSecurityDisabledException();
			}
			RSTrace.SecurityTracer.Assert(this.m_userToken != null && this.m_userToken is IntPtr);
			IntPtr intPtr = (IntPtr)this.m_userToken;
			if (intPtr != IntPtr.Zero)
			{
				return new WindowsIdentity(intPtr);
			}
			return null;
		}
        */
		public UserContext(string userName, object token, AuthenticationType authType)
		{
			this.m_userName = userName;
			this.m_userToken = token;
			this.m_authType = authType;
			this.m_initialized = true;
			this.m_additionalUserToken = null;
		}

		public UserContext()
		{
			this.m_userName = string.Empty;
			this.m_userToken = null;
			this.m_authType = AuthenticationType.None;
			this.m_additionalUserToken = null;
		}

		public UserContext(AuthenticationType authType)
		{
			this.m_userName = string.Empty;
			this.m_userToken = null;
			this.m_authType = authType;
			this.m_initialized = false;
			this.m_additionalUserToken = null;
		}
	}
}
