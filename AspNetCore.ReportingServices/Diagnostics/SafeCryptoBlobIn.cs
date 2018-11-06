using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class SafeCryptoBlobIn : SafeHandleZeroOrMinusOneIsInvalid
	{
		private int m_bufferSize;

		private SafeLocalFree m_blob;

		private SafeLocalFree m_pbData;

		internal SafeCryptoBlobIn(byte[] data)
			: base(true)
		{
			this.m_bufferSize = data.Length;
			this.m_blob = SafeLocalFree.LocalAlloc(Marshal.SizeOf(typeof(NativeMethods.DATA_BLOB)));
			base.handle = this.m_blob.DangerousGetHandle();
			this.m_pbData = SafeLocalFree.LocalAlloc(data.Length);
			Marshal.Copy(data, 0, this.m_pbData.DangerousGetHandle(), data.Length);
			Marshal.StructureToPtr(new NativeMethods.DATA_BLOB
			{
				cbData = data.Length,
				pbData = this.m_pbData.DangerousGetHandle()
			}, base.handle, true);
		}

		internal void ZeroBuffer()
		{
			if (!this.IsInvalid && this.m_bufferSize > 0 && this.m_pbData != null)
			{
				byte[] source = new byte[this.m_bufferSize];
				Marshal.Copy(source, 0, this.m_pbData.DangerousGetHandle(), this.m_bufferSize);
			}
		}

		protected override bool ReleaseHandle()
		{
			if (this.m_pbData != null)
			{
				this.m_pbData.Close();
			}
			if (this.m_blob != null)
			{
				this.m_blob.Close();
			}
			return true;
		}
	}
}
