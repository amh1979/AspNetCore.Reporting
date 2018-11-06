using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class SafeLocalFree : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal const int LMEM_FIXED = 0;

		internal const int LMEM_ZEROINIT = 64;

		internal const int LPTR = 64;

		private const int NULL = 0;

		internal static readonly SafeLocalFree Zero = new SafeLocalFree(false);

		private SafeLocalFree()
			: base(true)
		{
		}

		private SafeLocalFree(bool ownsHandle)
			: base(ownsHandle)
		{
		}

		internal static SafeLocalFree LocalAlloc(int cb)
		{
			return SafeLocalFree.LocalAlloc(0, cb);
		}

		internal static SafeLocalFree LocalAlloc(int flags, int cb)
		{
			SafeLocalFree safeLocalFree = NativeMemoryMethods.LocalAlloc(flags, (UIntPtr)(ulong)cb);
			if (safeLocalFree.IsInvalid)
			{
				safeLocalFree.SetHandleAsInvalid();
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32Exception(lastWin32Error);
			}
			return safeLocalFree;
		}

		protected override bool ReleaseHandle()
		{
			return NativeMemoryMethods.LocalFree(base.handle) == IntPtr.Zero;
		}
	}
}
