namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class ScriptAnalysis
	{
		internal int eScript;

		internal int fRTL;

		internal int fLayoutRTL;

		internal int fLinkBefore;

		internal int fLinkAfter;

		internal int fLogicalOrder;

		internal int fNoGlyphIndex;

		internal ScriptState s;

		internal ScriptAnalysis(ushort word1)
		{
			this.eScript = (word1 & 0x3FF);
			this.fRTL = (word1 >> 10 & 1);
			this.fLayoutRTL = (word1 >> 11 & 1);
			this.fLinkBefore = (word1 >> 12 & 1);
			this.fLinkAfter = (word1 >> 13 & 1);
			this.fLogicalOrder = (word1 >> 14 & 1);
			this.fNoGlyphIndex = (word1 >> 15 & 1);
		}

		internal SCRIPT_ANALYSIS GetAs_SCRIPT_ANALYSIS()
		{
			SCRIPT_ANALYSIS result = default(SCRIPT_ANALYSIS);
			result.word1 = (ushort)((this.eScript & 0x3FF) | (this.fRTL & 1) << 10 | (this.fLayoutRTL & 1) << 11 | (this.fLinkBefore & 1) << 12 | (this.fLinkAfter & 1) << 13 | (this.fLogicalOrder & 1) << 14 | (this.fNoGlyphIndex & 1) << 15);
			result.state = this.s.GetAs_SCRIPT_STATE();
			return result;
		}
	}
}
