using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal class NativeLogonMethods
	{
		internal sealed class SafeUserToken : SafeHandle
		{
			public override bool IsInvalid
			{
				get
				{
					return base.handle == IntPtr.Zero;
				}
			}

			public SafeUserToken()
				: base(IntPtr.Zero, true)
			{
			}

			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			protected override bool ReleaseHandle()
			{
				return SafeUserToken.CloseHandle(base.handle);
			}

			[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			private static extern bool CloseHandle(IntPtr hToken);
		}

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out SafeUserToken hToken);
	}
}
