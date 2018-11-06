using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class Win32DCSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private static Win32DCSafeHandle m_zero = new Win32DCSafeHandle(IntPtr.Zero, false);

		public IntPtr Handle
		{
			get
			{
				return base.handle;
			}
		}

		public static Win32DCSafeHandle Zero
		{
			get
			{
				return Win32DCSafeHandle.m_zero;
			}
		}

		public Win32DCSafeHandle()
			: base(true)
		{
		}

		public Win32DCSafeHandle(IntPtr hdc, bool ownsHandle)
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
				result = Win32.DeleteDC(base.handle);
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
