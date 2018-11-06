using System;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class GlyphShapeData
	{
		internal int GlyphCount;

		internal short[] Glyphs;

		internal short[] Clusters;

		internal SCRIPT_VISATTR[] VisAttrs;

		internal GlyphShapeData(int maxglyphs, int numChars)
		{
			this.Glyphs = new short[maxglyphs];
			this.Clusters = new short[numChars];
			this.VisAttrs = new SCRIPT_VISATTR[maxglyphs];
		}

		internal void TrimToGlyphCount()
		{
			if (this.GlyphCount < this.Glyphs.Length)
			{
				Array.Resize<short>(ref this.Glyphs, this.GlyphCount);
				Array.Resize<SCRIPT_VISATTR>(ref this.VisAttrs, this.GlyphCount);
			}
		}
	}
}
