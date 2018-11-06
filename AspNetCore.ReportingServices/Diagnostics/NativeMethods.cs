using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DATA_BLOB
		{
			public int cbData;

			public IntPtr pbData;
		}

		public const int CurrentUser = 0;

		public const int UIForbidden = 1;

		public const int LocalMachine = 4;

		[DllImport("crypt32", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool CryptProtectData(SafeCryptoBlobIn dataIn, string szDataDescr, IntPtr optionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, int dwFlags, SafeCryptoBlobOut pDataOut);

		[DllImport("crypt32", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool CryptUnprotectData(SafeCryptoBlobIn dataIn, StringBuilder ppszDataDescr, IntPtr optionalEntropy, IntPtr pvReserved, IntPtr pPromptStruct, int dwFlags, SafeCryptoBlobOut pDataOut);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr LocalFree(IntPtr hMem);
	}
}
