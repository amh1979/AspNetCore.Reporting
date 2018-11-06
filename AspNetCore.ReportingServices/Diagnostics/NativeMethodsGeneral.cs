using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal static class NativeMethodsGeneral
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct MEMORYSTATUSEX
		{
			internal int dwLength;

			internal int dwMemoryLoad;

			internal long ullTotalPhys;

			internal long ullAvailPhys;

			internal long ullTotalPageFile;

			internal long ullAvailPageFile;

			internal long ullTotalVirtual;

			internal long ullAvailVirtual;

			internal long ullAvailExtendedVirtual;

			internal void Init()
			{
				this.dwLength = Marshal.SizeOf(typeof(MEMORYSTATUSEX));
			}
		}

		public static bool GlobalMemoryStatusEx(out long ullAvailPhys, out long ullAvailVirtual)
		{
			MEMORYSTATUSEX mEMORYSTATUSEX = default(MEMORYSTATUSEX);
			mEMORYSTATUSEX.Init();
			if (NativeMethodsGeneral.GlobalMemoryStatusEx(ref mEMORYSTATUSEX) != 0)
			{
				ullAvailPhys = mEMORYSTATUSEX.ullAvailPhys;
				ullAvailVirtual = mEMORYSTATUSEX.ullAvailVirtual;
				return true;
			}
			ullAvailPhys = 0L;
			ullAvailVirtual = 0L;
			return false;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int GlobalMemoryStatusEx(ref MEMORYSTATUSEX memoryStatusEx);
	}
}
