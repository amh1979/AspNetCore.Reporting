using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class NativeGDIWrapper
	{
		public struct ABCFloat
		{
			public float abcfA;

			public float abcfB;

			public float abcfC;

			public ABCFloat(float a, float b, float c)
			{
				this.abcfA = a;
				this.abcfB = b;
				this.abcfC = c;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct OutlineTextMetric
		{
			public uint otmSize;

			public int tmHeight;

			public int tmAscent;

			public int tmDescent;

			public int tmInternalLeading;

			public int tmExternalLeading;

			public int tmAveCharWidth;

			public int tmMaxCharWidth;

			public int tmWeight;

			public int tmOverhang;

			public int tmDigitizedAspectX;

			public int tmDigitizedAspectY;

			public char tmFirstChar;

			public char tmLastChar;

			public char tmDefaultChar;

			public char tmBreakChar;

			public byte tmItalic;

			public byte tmUnderlined;

			public byte tmStruckOut;

			public byte tmPitchAndFamily;

			public byte tmCharSet;

			public byte otmFiller;

			public byte bFamilyType;

			public byte bSerifStyle;

			public byte bWeight;

			public byte bProportion;

			public byte bContrast;

			public byte bStrokeVariation;

			public byte bArmStyle;

			public byte bLetterform;

			public byte bMidline;

			public byte bXHeight;

			public uint otmfsSelection;

			public uint otmfsType;

			public int otmsCharSlopeRise;

			public int otmsCharSlopeRun;

			public int otmItalicAngle;

			public uint otmEMSquare;

			public int otmAscent;

			public int otmDescent;

			public uint otmLineGap;

			public uint otmsCapEmHeight;

			public uint otmsXHeight;

			public int left;

			public int top;

			public int right;

			public int bottom;

			public int otmMacAscent;

			public int otmMacDescent;

			public uint otmMacLineGap;

			public uint otmusMinimumPPEM;

			public int otmptSubscriptSizeX;

			public int otmptSubscriptSizeY;

			public int otmptSubscriptOffsetX;

			public int otmptSubscriptOffsetY;

			public int otmptSuperscriptSizeX;

			public int otmptSuperscriptSizeY;

			public int otmptSuperscriptOffsetX;

			public int otmptSuperscriptOffsetY;

			public uint otmsStrikeoutSize;

			public int otmsStrikeoutPosition;

			public int otmsUnderscoreSize;

			public int otmsUnderscorePosition;

			public string otmpFamilyName;

			public string otmpFaceName;

			public string otmpStyleName;

			public string otmpFullName;

			public OutlineTextMetric(string intialValue)
			{
				this.otmSize = 0u;
				this.tmHeight = 0;
				this.tmAscent = 0;
				this.tmDescent = 0;
				this.tmInternalLeading = 0;
				this.tmExternalLeading = 0;
				this.tmAveCharWidth = 0;
				this.tmMaxCharWidth = 0;
				this.tmWeight = 0;
				this.tmOverhang = 0;
				this.tmDigitizedAspectX = 0;
				this.tmDigitizedAspectY = 0;
				this.tmFirstChar = ' ';
				this.tmLastChar = ' ';
				this.tmDefaultChar = ' ';
				this.tmBreakChar = ' ';
				this.tmItalic = 0;
				this.tmUnderlined = 0;
				this.tmStruckOut = 0;
				this.tmPitchAndFamily = 0;
				this.tmCharSet = 0;
				this.otmFiller = 0;
				this.bFamilyType = 0;
				this.bSerifStyle = 0;
				this.bWeight = 0;
				this.bProportion = 0;
				this.bContrast = 0;
				this.bStrokeVariation = 0;
				this.bArmStyle = 0;
				this.bLetterform = 0;
				this.bMidline = 0;
				this.bXHeight = 0;
				this.otmfsSelection = 0u;
				this.otmfsType = 0u;
				this.otmsCharSlopeRise = 0;
				this.otmsCharSlopeRun = 0;
				this.otmItalicAngle = 0;
				this.otmEMSquare = 0u;
				this.otmAscent = 0;
				this.otmDescent = 0;
				this.otmLineGap = 0u;
				this.otmsCapEmHeight = 0u;
				this.otmsXHeight = 0u;
				this.left = 0;
				this.top = 0;
				this.right = 0;
				this.bottom = 0;
				this.otmMacAscent = 0;
				this.otmMacDescent = 0;
				this.otmMacLineGap = 0u;
				this.otmusMinimumPPEM = 0u;
				this.otmptSubscriptSizeX = 0;
				this.otmptSubscriptSizeY = 0;
				this.otmptSubscriptOffsetX = 0;
				this.otmptSubscriptOffsetY = 0;
				this.otmptSuperscriptSizeX = 0;
				this.otmptSuperscriptSizeY = 0;
				this.otmptSuperscriptOffsetX = 0;
				this.otmptSuperscriptOffsetY = 0;
				this.otmsStrikeoutSize = 0u;
				this.otmsStrikeoutPosition = 0;
				this.otmsUnderscoreSize = 0;
				this.otmsUnderscorePosition = 0;
				this.otmpFamilyName = intialValue;
				this.otmpFaceName = "";
				this.otmpStyleName = "";
				this.otmpFullName = "";
			}
		}

		private NativeGDIWrapper()
		{
		}

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, [In] [Out] IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject([In] [Out] IntPtr hgdiobj);

		[DllImport("gdi32.dll")]
		public static extern int GetCharABCWidthsFloat(IntPtr hdc, uint iFirstChar, uint iLastChar, [In] [Out] ABCFloat[] lpABCF);

		[DllImport("gdi32.dll")]
		public static extern uint GetGlyphIndicesW(IntPtr hdc, ushort[] lpstr, int c, ushort[] g, uint fl);

		[DllImport("gdi32.dll")]
		public static extern bool GetTextExtentExPointI(IntPtr hdc, ushort[] pgiIn, int cgi, int nMaxExtent, ref int lpnFit, [In] [Out] int[] alpDx, ref Size lpSize);

		[DllImport("gdi32.dll")]
		public static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, ref OutlineTextMetric lpOTM);
	}
}
