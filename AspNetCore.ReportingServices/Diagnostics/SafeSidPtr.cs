using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class SafeSidPtr : SafeHandleZeroOrMinusOneIsInvalid
	{
		private struct SID_IDENTIFIER_AUTHORITY
		{
			internal byte m_Value0;

			internal byte m_Value1;

			internal byte m_Value2;

			internal byte m_Value3;

			internal byte m_Value4;

			internal byte m_Value5;
		}

		internal const uint SECURITY_BUILTIN_DOMAIN_RID = 32u;

		internal const uint DOMAIN_ALIAS_RID_ADMINS = 544u;

		internal const uint SECURITY_LOCAL_SYSTEM_RID = 18u;

		internal static readonly SafeSidPtr Zero = new SafeSidPtr(false);

		private static byte[] SECURITY_NT_AUTHORITY = new byte[6]
		{
			0,
			0,
			0,
			0,
			0,
			5
		};

		private SafeSidPtr()
			: base(true)
		{
		}

		private SafeSidPtr(bool ownsHandle)
			: base(ownsHandle)
		{
		}

		internal static bool AllocateAndInitializeSid(byte nSubAuthorityCount, uint nSubAuthority0, uint nSubAuthority1, uint nSubAuthority2, uint nSubAuthority3, uint nSubAuthority4, uint nSubAuthority5, uint nSubAuthority6, uint nSubAuthority7, out SafeSidPtr pSid)
		{
			SafeLocalFree safeLocalFree = null;
			try
			{
				SID_IDENTIFIER_AUTHORITY sID_IDENTIFIER_AUTHORITY = default(SID_IDENTIFIER_AUTHORITY);
				sID_IDENTIFIER_AUTHORITY.m_Value0 = SafeSidPtr.SECURITY_NT_AUTHORITY[0];
				sID_IDENTIFIER_AUTHORITY.m_Value1 = SafeSidPtr.SECURITY_NT_AUTHORITY[1];
				sID_IDENTIFIER_AUTHORITY.m_Value2 = SafeSidPtr.SECURITY_NT_AUTHORITY[2];
				sID_IDENTIFIER_AUTHORITY.m_Value3 = SafeSidPtr.SECURITY_NT_AUTHORITY[3];
				sID_IDENTIFIER_AUTHORITY.m_Value4 = SafeSidPtr.SECURITY_NT_AUTHORITY[4];
				sID_IDENTIFIER_AUTHORITY.m_Value5 = SafeSidPtr.SECURITY_NT_AUTHORITY[5];
				int cb = Marshal.SizeOf(sID_IDENTIFIER_AUTHORITY);
				safeLocalFree = SafeLocalFree.LocalAlloc(cb);
				Marshal.StructureToPtr(sID_IDENTIFIER_AUTHORITY, safeLocalFree.DangerousGetHandle(), true);
				return NativeMemoryMethods.AllocateAndInitializeSid(safeLocalFree, nSubAuthorityCount, nSubAuthority0, nSubAuthority1, nSubAuthority2, nSubAuthority3, nSubAuthority4, nSubAuthority5, nSubAuthority6, nSubAuthority7, out pSid);
			}
			finally
			{
				if (safeLocalFree != null)
				{
					safeLocalFree.Close();
				}
			}
		}

		protected override bool ReleaseHandle()
		{
			return NativeMemoryMethods.FreeSid(base.handle) == IntPtr.Zero;
		}
	}
}
