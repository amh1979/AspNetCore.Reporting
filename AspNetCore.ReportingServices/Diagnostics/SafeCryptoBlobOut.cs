using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class SafeCryptoBlobOut : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeLocalFree m_pBlob;

		private bool m_managedBlobInitialized;

		private NativeMethods.DATA_BLOB m_blob;

		internal NativeMethods.DATA_BLOB Blob
		{
			get
			{
				if (!this.m_managedBlobInitialized)
				{
					this.m_blob = (NativeMethods.DATA_BLOB)Marshal.PtrToStructure(base.handle, typeof(NativeMethods.DATA_BLOB));
					this.m_managedBlobInitialized = true;
				}
				return this.m_blob;
			}
		}

		internal SafeCryptoBlobOut()
			: base(true)
		{
			this.m_pBlob = SafeLocalFree.LocalAlloc(Marshal.SizeOf(typeof(NativeMethods.DATA_BLOB)));
			base.handle = this.m_pBlob.DangerousGetHandle();
		}

		internal void ZeroBuffer()
		{
			NativeMethods.DATA_BLOB blob = this.Blob;
			byte[] destination = new byte[blob.cbData];
			Marshal.Copy(blob.pbData, destination, 0, blob.cbData);
		}

		protected override bool ReleaseHandle()
		{
			if (this.m_pBlob != null)
			{
				NativeMemoryMethods.LocalFree(this.Blob.pbData);
				this.m_pBlob.Close();
			}
			return true;
		}
	}
}
