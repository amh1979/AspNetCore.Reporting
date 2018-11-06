using AspNetCore.ReportingServices.Rendering.RichText;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFFont
	{
		internal sealed class GlyphData : IComparable
		{
			internal ushort Glyph;

			internal float Width;

			internal char? Character;

			internal GlyphData(ushort glyph, float width)
			{
				this.Glyph = glyph;
				this.Width = width;
				this.Character = null;
			}

			int IComparable.CompareTo(object o1)
			{
				GlyphData glyphData = (GlyphData)o1;
				if (this.Glyph < glyphData.Glyph)
				{
					return -1;
				}
				if (this.Glyph > glyphData.Glyph)
				{
					return 1;
				}
				return 0;
			}
		}

		internal CachedFont CachedFont;

		internal readonly string FontFamily;

		internal string FontPDFFamily;

		internal int FontId = -1;

		internal List<GlyphData> UniqueGlyphs = new List<GlyphData>();

		internal string FontCMap;

		internal string Registry;

		internal string Ordering;

		internal string Supplement;

		internal readonly FontStyle GDIFontStyle;

		internal readonly int EMHeight;

		internal readonly float GridHeight;

		internal readonly float EMGridConversion;

		internal readonly bool InternalFont;

		internal readonly bool SimulateItalic;

		internal readonly bool SimulateBold;

		internal EmbeddedFont EmbeddedFont;

		internal bool IsComposite
		{
			get
			{
				return !string.IsNullOrEmpty(this.FontCMap);
			}
		}

		internal PDFFont(CachedFont cachedFont, string fontFamily, string pdfFontFamily, string fontCMap, string registry, string ordering, string supplement, FontStyle gdiFontStyle, int emHeight, float gridHeight, bool internalFont, bool simulateItalic, bool simulateBold)
		{
			this.CachedFont = cachedFont;
			this.FontFamily = fontFamily;
			this.FontPDFFamily = pdfFontFamily;
			this.FontCMap = fontCMap;
			this.Registry = registry;
			this.Ordering = ordering;
			this.Supplement = supplement;
			this.GDIFontStyle = gdiFontStyle;
			this.EMHeight = emHeight;
			this.GridHeight = gridHeight;
			this.InternalFont = internalFont;
			this.EMGridConversion = (float)(1000.0 / (float)emHeight);
			this.SimulateItalic = simulateItalic;
			this.SimulateBold = simulateBold;
		}

		internal GlyphData AddUniqueGlyph(ushort glyph, float width)
		{
			GlyphData glyphData = new GlyphData(glyph, width);
			if (this.UniqueGlyphs.BinarySearch(glyphData) >= 0)
			{
				return null;
			}
			int num = 0;
			while (num < this.UniqueGlyphs.Count)
			{
				if (glyphData.Glyph >= this.UniqueGlyphs[num].Glyph)
				{
					num++;
					continue;
				}
				this.UniqueGlyphs.Insert(num, glyphData);
				break;
			}
			if (num == this.UniqueGlyphs.Count)
			{
				this.UniqueGlyphs.Add(glyphData);
			}
			return glyphData;
		}
	}
}
