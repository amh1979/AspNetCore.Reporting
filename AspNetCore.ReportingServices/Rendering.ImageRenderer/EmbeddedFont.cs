using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class EmbeddedFont
	{
		private int m_objectId;

		private int m_toUnicodeId;

		private List<PDFFont> m_fonts = new List<PDFFont>();

		internal int ObjectId
		{
			get
			{
				return this.m_objectId;
			}
		}

		internal int ToUnicodeId
		{
			get
			{
				return this.m_toUnicodeId;
			}
		}

		internal List<PDFFont> PDFFonts
		{
			get
			{
				return this.m_fonts;
			}
		}

		internal EmbeddedFont(int objectId, int toUnicodeId)
		{
			this.m_objectId = objectId;
			this.m_toUnicodeId = toUnicodeId;
		}

		internal ushort[] GetGlyphIdArray()
		{
			int num = 0;
			foreach (PDFFont font in this.m_fonts)
			{
				num += font.UniqueGlyphs.Count;
			}
			ushort[] array = new ushort[num];
			int num2 = 0;
			foreach (PDFFont font2 in this.m_fonts)
			{
				foreach (PDFFont.GlyphData uniqueGlyph in font2.UniqueGlyphs)
				{
					array[num2] = uniqueGlyph.Glyph;
					num2++;
				}
			}
			return array;
		}

		internal IEnumerable<CMapMapping> GetGlyphIdToUnicodeMapping()
		{
			int num = 0;
			foreach (PDFFont font in this.m_fonts)
			{
				num += font.UniqueGlyphs.Count;
			}
			List<CMapMapping> list = new List<CMapMapping>(num);
			foreach (PDFFont font2 in this.m_fonts)
			{
				foreach (PDFFont.GlyphData uniqueGlyph in font2.UniqueGlyphs)
				{
					if (uniqueGlyph.Character.HasValue)
					{
						list.Add(new CMapMapping(uniqueGlyph.Glyph, uniqueGlyph.Character.Value));
					}
				}
			}
			return list;
		}
	}
}
