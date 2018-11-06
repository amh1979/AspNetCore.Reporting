using System;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class TexRunShapeData
	{
		private GlyphShapeData m_glyphData;

		private SCRIPT_ANALYSIS m_analysis;

		private SCRIPT_LOGATTR[] m_scriptLogAttr;

		private CachedFont m_cachedFont;

		private int? m_itemizedScriptId = null;

		private TextRunState m_runState;

		internal GlyphShapeData GlyphData
		{
			get
			{
				return this.m_glyphData;
			}
		}

		internal SCRIPT_ANALYSIS Analysis
		{
			get
			{
				return this.m_analysis;
			}
		}

		internal SCRIPT_LOGATTR[] ScriptLogAttr
		{
			get
			{
				return this.m_scriptLogAttr;
			}
		}

		internal CachedFont Font
		{
			get
			{
				return this.m_cachedFont;
			}
		}

		internal int? ItemizedScriptId
		{
			get
			{
				return this.m_itemizedScriptId;
			}
		}

		internal TextRunState State
		{
			get
			{
				return this.m_runState;
			}
		}

		internal TexRunShapeData(TextRun run, bool storeGlyph)
		{
			if (storeGlyph && run.GlyphData != null)
			{
				this.m_glyphData = run.GlyphData.GlyphScriptShapeData;
			}
			this.m_analysis = run.SCRIPT_ANALYSIS;
			this.m_scriptLogAttr = run.ScriptLogAttr;
			this.m_cachedFont = run.CachedFont;
			this.m_runState = run.State;
			this.m_itemizedScriptId = run.ItemizedScriptId;
		}

		internal TexRunShapeData(TextRun run, bool storeGlyph, int startIndex)
			: this(run, storeGlyph)
		{
			SCRIPT_LOGATTR[] scriptLogAttr = run.ScriptLogAttr;
			if (startIndex > 0 && scriptLogAttr != null)
			{
				int num = scriptLogAttr.Length;
				int num2 = num - startIndex;
				SCRIPT_LOGATTR[] array = new SCRIPT_LOGATTR[num2];
				Array.Copy(scriptLogAttr, startIndex, array, 0, num2);
				this.m_scriptLogAttr = array;
			}
		}
	}
}
