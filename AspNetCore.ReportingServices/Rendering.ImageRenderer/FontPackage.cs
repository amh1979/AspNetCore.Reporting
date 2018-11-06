using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class FontPackage
	{
		private class CompareDirectoryEntryOffsets : IComparer
		{
			int IComparer.Compare(object object1, object object2)
			{
				DirectoryEntry directoryEntry = (DirectoryEntry)object1;
				DirectoryEntry directoryEntry2 = (DirectoryEntry)object2;
				if (directoryEntry.TableOffset < directoryEntry2.TableOffset)
				{
					return -1;
				}
				if (directoryEntry.TableOffset == directoryEntry2.TableOffset)
				{
					return 0;
				}
				return 1;
			}
		}

		private class DirectoryEntry
		{
			private uint m_tableLength;

			private uint m_tableOffset;

			private uint m_tableTag;

			private int m_directoryIndex;

			public uint TableLength
			{
				get
				{
					return this.m_tableLength;
				}
			}

			public uint TableOffset
			{
				get
				{
					return this.m_tableOffset;
				}
			}

			public uint TableTag
			{
				get
				{
					return this.m_tableTag;
				}
			}

			public int DirectoryIndex
			{
				get
				{
					return this.m_directoryIndex;
				}
			}

			private uint ToUInt32BigEndian(byte[] buffer, ushort offset)
			{
				return (uint)((int)(buffer[offset] << 24 & 4278190080u) | (buffer[offset + 1] << 16 & 0xFF0000) | (buffer[offset + 2] << 8 & 0xFF00) | (buffer[offset + 3] & 0xFF));
			}

			public DirectoryEntry(byte[] rawData, int directoryIndex)
			{
				this.m_directoryIndex = directoryIndex;
				this.m_tableLength = this.ToUInt32BigEndian(rawData, 12);
				this.m_tableOffset = this.ToUInt32BigEndian(rawData, 8);
				this.m_tableTag = BitConverter.ToUInt32(rawData, 0);
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr AllocateMemory(IntPtr size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr ReAllocateMemory(IntPtr memPointer, IntPtr size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void FreeMemory(IntPtr memPointer);

		private const ushort OS2V0_WEIGHTCLASS_OFFSET = 4;

		private const ushort OS2V0_SELECTION_OFFSET = 62;

		private const ushort SIZEOF_OS2V0_TABLE = 64;

		private const uint TAG_TABLE_NAME_OS2V0 = 841962319u;

		private const byte TMPF_TRUETYPE = 4;

		private const int FW_SEMIBOLD = 600;

		private const ushort SFNT_DIRECTORYENTRY_TAG = 0;

		private const ushort SFNT_DIRECTORYENTRY_CHECKSUM = 4;

		private const ushort SFNT_DIRECTORYENTRY_TABLEOFFSET = 8;

		private const ushort SFNT_DIRECTORYENTRY_TABLELENGTH = 12;

		private const ushort SIZEOF_SFNT_DIRECTORYENTRY = 16;

		private const ushort SFNT_OFFSETTABLE_NUMOFFSETS = 4;

		private const ushort SIZEOF_SFNT_OFFSETTABLE = 12;

		private const uint tag_TTCF = 1717793908u;

		private const uint GDI_ERROR = 4294967295u;

		private const ushort TTFCFP_FLAGS_SUBSET = 1;

		private const ushort TTFCFP_FLAGS_GLYPHLIST = 8;

		private const int EMBED_PREVIEWPRINT = 1;

		private const int EMBED_EDITABLE = 2;

		private const int EMBED_INSTALLABLE = 3;

		private const int EMBED_NOEMBEDDING = 4;

		private const int E_NONE = 0;

		private static void ThrowNativeException(string source, int error, bool dump)
		{
			string message = string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, source, error);
			if (dump)
			{
				throw new Exception(message);
			}
			throw new ReportRenderingException(message);
		}

		private static void CheckGetFontDataResult(uint result)
		{
			if (result != 4294967295u)
			{
				return;
			}
			string message = string.Format(CultureInfo.InvariantCulture, ImageRendererRes.Win32ErrorInfo, "GetFontData", 4294967295u);
			throw new Exception(message);
		}

		private static ushort ToUInt16BigEndian(byte[] buffer, ushort offset)
		{
			return (ushort)((buffer[offset] << 8 & 0xFF00) | (buffer[offset + 1] & 0xFF));
		}

		private static bool GetWeightClassAndSelection(Win32DCSafeHandle hdc, out ushort weightClass, out ushort selection)
		{
			weightClass = 0;
			selection = 0;
			byte[] array = new byte[64];
			uint fontData = FontPackage.GetFontData(hdc, 841962319u, 0u, array, (uint)array.Length);
			if (fontData == array.Length)
			{
				weightClass = FontPackage.ToUInt16BigEndian(array, 4);
				selection = FontPackage.ToUInt16BigEndian(array, 62);
				return true;
			}
			return false;
		}

		internal static void CheckSimulatedFontStyles(Win32DCSafeHandle hdc, AspNetCore.ReportingServices.Rendering.RichText.Win32.TEXTMETRIC textMetric, ref bool simulateItalic, ref bool simulateBold)
		{
			simulateItalic = false;
			simulateBold = false;
			if ((textMetric.tmPitchAndFamily & 4) == 4)
			{
				if (textMetric.tmWeight < 600 && textMetric.tmItalic <= 0)
				{
					return;
				}
				ushort num = default(ushort);
				ushort num2 = default(ushort);
				if (FontPackage.GetWeightClassAndSelection(hdc, out num, out num2))
				{
					if (textMetric.tmItalic > 0 && (num2 & 1) == 0)
					{
						simulateItalic = true;
					}
					if (textMetric.tmWeight >= 600 && num < textMetric.tmWeight)
					{
						simulateBold = true;
					}
				}
			}
		}

		internal static bool CheckEmbeddingRights(Win32DCSafeHandle hdc)
		{
			uint num = 0u;
			if (FontPackage.TTGetEmbeddingType(hdc, ref num) == 0)
			{
				return num != 4;
			}
			return false;
		}

		internal static byte[] Generate(Win32DCSafeHandle hdc, string fontFamily, ushort[] glyphIdArray)
		{
			IntPtr intPtr = IntPtr.Zero;
			uint num = 0u;
			IntPtr zero = IntPtr.Zero;
			uint num2 = 0u;
			byte[] array = null;
			try
			{
				if (FontPackage.GetFontData(hdc, 1717793908u, 0u, IntPtr.Zero, 0u) == 4294967295u)
				{
					num = FontPackage.GetFontData(hdc, 0u, 0u, IntPtr.Zero, 0u);
					FontPackage.CheckGetFontDataResult(num);
					intPtr = Marshal.AllocHGlobal((int)num);
					uint fontData = FontPackage.GetFontData(hdc, 0u, 0u, intPtr, num);
					FontPackage.CheckGetFontDataResult(fontData);
				}
				else
				{
					byte[] tTCFontData = FontPackage.GetTTCFontData(hdc, ref num);
					intPtr = Marshal.AllocHGlobal((int)num);
					Marshal.Copy(tTCFontData, 0, intPtr, (int)num);
				}
				ushort usFlags = 9;
				uint num3 = 0u;
				short num4 = FontPackage.CreateFontPackage(intPtr, num, out zero, ref num2, ref num3, usFlags, (ushort)0, (ushort)0, (ushort)0, (ushort)3, (ushort)65535, glyphIdArray, (ushort)glyphIdArray.Length, (Delegate)new AllocateMemory(FontPackage.AllocateFontBufferMemory), (Delegate)new ReAllocateMemory(FontPackage.ReAllocateFontBufferMemory), (Delegate)new FreeMemory(FontPackage.FreeFontBufferMemory), IntPtr.Zero);
				if (num4 != 0)
				{
					string source = string.Format(CultureInfo.InvariantCulture, "CreateFontPackage(fontFamily={0})", fontFamily);
					FontPackage.ThrowNativeException(source, num4, false);
				}
				array = new byte[num3];
				Marshal.Copy(zero, array, 0, (int)num3);
				return array;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (zero != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(zero);
				}
			}
		}

		private static byte[] Swap(byte[] source)
		{
			byte[] array = new byte[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				array[i] = source[source.Length - i - 1];
			}
			return array;
		}

		private static byte[] GetTTCFontData(Win32DCSafeHandle hdc, ref uint fontDataLength)
		{
			byte[] array = null;
			fontDataLength = 0u;
			byte[] array2 = null;
			byte[] array3 = new byte[2];
			uint fontData = FontPackage.GetFontData(hdc, 0u, 4u, array3, 2u);
			FontPackage.CheckGetFontDataResult(fontData);
			ushort num = BitConverter.ToUInt16(FontPackage.Swap(array3), 0);
			ushort num2 = (ushort)(12 + num * 16);
			array2 = new byte[num2];
			fontData = FontPackage.GetFontData(hdc, 0u, 0u, array2, num2);
			FontPackage.CheckGetFontDataResult(fontData);
			ArrayList arrayList = new ArrayList();
			uint num3 = num2;
			for (int i = 0; i < num; i++)
			{
				byte[] array4 = new byte[16];
				Array.Copy(array2, 12 + i * 16, array4, 0, 16);
				DirectoryEntry directoryEntry = new DirectoryEntry(array4, i);
				num3 += directoryEntry.TableLength;
				arrayList.Add(directoryEntry);
			}
			array = new byte[num3];
			Array.Copy(array2, array, num2);
			fontDataLength += num2;
			arrayList.Sort(new CompareDirectoryEntryOffsets());
			foreach (DirectoryEntry item in arrayList)
			{
				byte[] array5 = new byte[item.TableLength];
				fontData = FontPackage.GetFontData(hdc, item.TableTag, 0u, array5, (uint)array5.Length);
				FontPackage.CheckGetFontDataResult(fontData);
				int destinationIndex = 12 + item.DirectoryIndex * 16 + 8;
				byte[] array6 = FontPackage.Swap(BitConverter.GetBytes(fontDataLength));
				Array.Copy(array6, 0, array, destinationIndex, array6.Length);
				Array.Copy(array5, 0L, array, fontDataLength, array5.Length);
				fontDataLength += (uint)array5.Length;
			}
			return array;
		}

		private static IntPtr AllocateFontBufferMemory(IntPtr size)
		{
			return Marshal.AllocHGlobal(size);
		}

		private static IntPtr ReAllocateFontBufferMemory(IntPtr memPointer, IntPtr size)
		{
			if (memPointer == IntPtr.Zero)
			{
				return Marshal.AllocHGlobal(size);
			}
			return Marshal.ReAllocHGlobal(memPointer, size);
		}

		private static void FreeFontBufferMemory(IntPtr memPointer)
		{
			Marshal.FreeHGlobal(memPointer);
		}

		[DllImport("gdi32.dll")]
		private static extern uint GetFontData(Win32DCSafeHandle hdc, uint dwTable, uint dwOffset, IntPtr lpvBuffer, uint cbData);

		[DllImport("gdi32.dll")]
		private static extern uint GetFontData(Win32DCSafeHandle hdc, uint dwTable, uint dwOffset, byte[] lpvBuffer, uint cbData);

		[DllImport("FontSub")]
		private static extern short CreateFontPackage(IntPtr puchSrcBuffer, uint ulSrcBufferSize, out IntPtr puchFontPackageBuffer, ref uint pulFontPackageBufferSize, ref uint pulBytesWritten, ushort usFlags, ushort usTTCIndex, ushort usSubsetFormat, ushort usSubsetLanguage, ushort usSubsetPlatform, ushort usSubsetEncoding, ushort[] pusSubsetKeepList, ushort usSubsetKeepListCount, Delegate lpfnAllocate, Delegate lpfnReAllocate, Delegate lpfnFree, IntPtr lpvReserved);

		[DllImport("T2Embed")]
		private static extern int TTGetEmbeddingType(Win32DCSafeHandle hdc, ref uint status);
	}
}
