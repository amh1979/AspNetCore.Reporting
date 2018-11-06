using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class SecureStringWrapper : IDisposable
	{
		private SecureString m_secStr;

		public SecureStringWrapper(string regularString)
		{
			this.m_secStr = new SecureString();
			if (regularString != null)
			{
				for (int i = 0; i < regularString.Length; i++)
				{
					this.m_secStr.AppendChar(regularString[i]);
				}
				this.m_secStr.MakeReadOnly();
			}
		}

		public SecureStringWrapper(SecureString secureString)
		{
			this.m_secStr = ((secureString == null) ? new SecureString() : secureString.Copy());
		}

		public SecureStringWrapper(SecureStringWrapper secureStringWrapper)
		{
			if (secureStringWrapper != null && secureStringWrapper.m_secStr != null)
			{
				this.m_secStr = secureStringWrapper.m_secStr.Copy();
			}
			else
			{
				this.m_secStr = new SecureString();
			}
		}

		public static string GetDecryptedString(SecureString secureString)
		{
			string empty = string.Empty;
			IntPtr intPtr = IntPtr.Zero;
			if (secureString.Length != 0)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					RuntimeHelpers.PrepareConstrainedRegions();
					try
					{
					}
					finally
					{
						intPtr = Marshal.SecureStringToBSTR(secureString);
					}
					return Marshal.PtrToStringBSTR(intPtr);
				}
				finally
				{
					if (IntPtr.Zero != intPtr)
					{
						Marshal.ZeroFreeBSTR(intPtr);
					}
				}
			}
			return empty;
		}

		public override string ToString()
		{
			return SecureStringWrapper.GetDecryptedString(this.m_secStr);
		}

		internal SecureString GetUnderlyingSecureString()
		{
			return this.m_secStr;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && this.m_secStr != null)
			{
				this.m_secStr.Dispose();
				this.m_secStr = null;
			}
		}
	}
}
