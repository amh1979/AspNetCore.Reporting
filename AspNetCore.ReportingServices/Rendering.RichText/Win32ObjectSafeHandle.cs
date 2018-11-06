using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class Win32ObjectSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private static Win32ObjectSafeHandle m_zero = new Win32ObjectSafeHandle(IntPtr.Zero, false);

		public IntPtr Handle
		{
			get
			{
				return base.handle;
			}
		}

		public static Win32ObjectSafeHandle Zero
		{
			get
			{
				return Win32ObjectSafeHandle.m_zero;
			}
		}

		public Win32ObjectSafeHandle()
			: base(true)
		{
		}

		public Win32ObjectSafeHandle(IntPtr hdc, bool ownsHandle)
			: base(ownsHandle)
		{
			base.handle = hdc;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			bool result;
			try
			{
				result = (Win32.DeleteObject(base.handle) != 0);
			}
			catch
			{
				result = false;
			}
			base.handle = IntPtr.Zero;
			return result;
		}
	}
}
