using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class ScriptCacheSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public ScriptCacheSafeHandle()
			: base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			int hr = Win32.ScriptFreeCache(ref base.handle);
			base.handle = IntPtr.Zero;
			return Win32.Succeeded(hr);
		}
	}
}
