using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal class SafeNativeLoggingPointer : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeNativeLoggingPointer()
			: base(true)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		protected override bool ReleaseHandle()
		{
			//NativeLoggingMethods.ReleaseNativeLoggingObject(base.handle);
			return true;
		}
	}
}
