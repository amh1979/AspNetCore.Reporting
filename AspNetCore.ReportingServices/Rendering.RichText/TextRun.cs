using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class TextRun
	{
		protected string m_text;

		protected bool m_clone;

		protected ITextRunProps m_textRunProps;

		protected GlyphData m_cachedGlyphData;

		protected CachedFont m_cachedFont;

		protected ScriptAnalysis m_scriptAnalysis;

		internal SCRIPT_ANALYSIS SCRIPT_ANALYSIS;

		internal SCRIPT_LOGATTR[] ScriptLogAttr;

		internal int m_underlineHeight;

		private int? m_itemizedScriptId = null;

		private TextRunState m_runState;

		public bool IsComplex
		{
			get
			{
				ScriptProperties properties = ScriptProperties.GetProperties(this.ScriptAnalysis.eScript);
				if (properties.IsComplex)
				{
					return true;
				}
				return false;
			}
		}

		internal ITextRunProps TextRunProperties
		{
			get
			{
				return this.m_textRunProps;
			}
		}

		internal CachedFont CachedFont
		{
			get
			{
				return this.m_cachedFont;
			}
		}

		internal TextRunState State
		{
			get
			{
				return this.m_runState;
			}
		}

		internal int? ItemizedScriptId
		{
			get
			{
				return this.m_itemizedScriptId;
			}
		}

		internal bool FallbackFont
		{
			get
			{
				return (int)(this.m_runState & TextRunState.FallbackFont) > 0;
			}
			set
			{
				if (value)
				{
					this.m_runState |= TextRunState.FallbackFont;
				}
				else
				{
					this.m_runState &= ~TextRunState.FallbackFont;
				}
			}
		}

		internal bool HasEastAsianChars
		{
			get
			{
				return (int)(this.m_runState & TextRunState.HasEastAsianChars) > 0;
			}
			set
			{
				if (value)
				{
					this.m_runState |= TextRunState.HasEastAsianChars;
				}
				else
				{
					this.m_runState &= ~TextRunState.HasEastAsianChars;
				}
			}
		}

		internal int CharacterCount
		{
			get
			{
				return this.m_text.Length;
			}
		}

		internal uint ColorInt
		{
			get
			{
				Color color = this.m_textRunProps.Color;
				return (uint)(color.B << 16 | color.G << 8 | color.R);
			}
		}

		internal ScriptAnalysis ScriptAnalysis
		{
			get
			{
				if (this.m_scriptAnalysis == null)
				{
					this.m_scriptAnalysis = new ScriptAnalysis(this.SCRIPT_ANALYSIS.word1);
					this.m_scriptAnalysis.s = new ScriptState(this.SCRIPT_ANALYSIS.state.word1);
				}
				return this.m_scriptAnalysis;
			}
		}

		internal string Text
		{
			get
			{
				return this.m_text;
			}
		}

		internal GlyphData GlyphData
		{
			get
			{
				return this.m_cachedGlyphData;
			}
		}

		internal bool Clone
		{
			get
			{
				return this.m_clone;
			}
			set
			{
				this.m_clone = value;
			}
		}

		internal virtual int HighlightStart
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		internal virtual int HighlightEnd
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		internal virtual Color HighlightColor
		{
			get
			{
				return Color.Empty;
			}
			set
			{
			}
		}

		internal virtual int CharacterIndexInOriginal
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		internal virtual bool IsHighlightTextRun
		{
			get
			{
				return false;
			}
		}

		internal virtual bool IsPlaceholderTextRun
		{
			get
			{
				return false;
			}
		}

		internal int UnderlineHeight
		{
			get
			{
				return this.m_underlineHeight;
			}
			set
			{
				this.m_underlineHeight = value;
			}
		}

		internal virtual bool AllowColorInversion
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		internal TextRun(string text, ITextRunProps props)
		{
			bool hasEastAsianChars = false;
			this.m_text = Utilities.ConvertTabAndCheckEastAsianChars(text, out hasEastAsianChars);
			this.HasEastAsianChars = hasEastAsianChars;
			this.m_textRunProps = props;
		}

		internal TextRun(string text, TextRun textRun)
			: this(text, textRun.TextRunProperties)
		{
		}

		internal TextRun(string text, ITextRunProps props, TexRunShapeData shapeData)
			: this(text, props)
		{
			if (shapeData != null)
			{
				this.SCRIPT_ANALYSIS = shapeData.Analysis;
				this.ScriptLogAttr = shapeData.ScriptLogAttr;
				this.m_cachedFont = shapeData.Font;
				this.m_itemizedScriptId = shapeData.ItemizedScriptId;
				this.m_runState = shapeData.State;
				if (shapeData.GlyphData != null)
				{
					this.m_cachedGlyphData = new GlyphData(shapeData.GlyphData);
				}
			}
		}

		internal TextRun(string text, TextRun textRun, SCRIPT_LOGATTR[] scriptLogAttr)
			: this(text, textRun)
		{
			this.SCRIPT_ANALYSIS = textRun.SCRIPT_ANALYSIS;
			this.ScriptLogAttr = scriptLogAttr;
			this.m_cachedFont = textRun.CachedFont;
			this.m_itemizedScriptId = textRun.ItemizedScriptId;
			bool hasEastAsianChars = this.HasEastAsianChars;
			this.m_runState = textRun.State;
			this.HasEastAsianChars = hasEastAsianChars;
		}

		internal virtual TextRun Split(string text, SCRIPT_LOGATTR[] scriptLogAttr)
		{
			return new TextRun(text, this, scriptLogAttr);
		}

		internal virtual TextRun GetSubRun(int startIndex, int length)
		{
			if (length == this.m_text.Length)
			{
				return this;
			}
			if (startIndex > 0)
			{
				this.m_textRunProps.AddSplitIndex(startIndex);
			}
			return new TextRun(this.m_text.Substring(startIndex, length), this);
		}

		internal TextRun GetSubRun(int startIndex)
		{
			if (startIndex == 0)
			{
				return this;
			}
			return this.GetSubRun(startIndex, this.m_text.Length - startIndex);
		}

		internal int[] GetLogicalWidths(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			GlyphData glyphData = this.GetGlyphData(hdc, fontCache);
			int[] array = new int[this.m_text.Length];
			int num = Win32.ScriptGetLogicalWidths(ref this.SCRIPT_ANALYSIS, this.m_text.Length, glyphData.GlyphScriptShapeData.GlyphCount, glyphData.ScaledAdvances, glyphData.GlyphScriptShapeData.Clusters, glyphData.GlyphScriptShapeData.VisAttrs, array);
			if (Win32.Failed(num))
			{
				Marshal.ThrowExceptionForHR(num);
			}
			if (glyphData.Scaled)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = glyphData.Scale(array[i]);
				}
			}
			return array;
		}

		internal void TerminateAt(int index)
		{
			this.m_text = this.m_text.Remove(index, this.m_text.Length - index);
			SCRIPT_LOGATTR[] array = new SCRIPT_LOGATTR[index];
			Array.Copy(this.ScriptLogAttr, 0, array, 0, array.Length);
			this.ScriptLogAttr = array;
			this.m_cachedGlyphData = null;
		}

		internal CachedFont GetCachedFont(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (this.m_cachedGlyphData == null)
			{
				this.ShapeAndPlace(hdc, fontCache);
			}
			else
			{
				this.LoadGlyphData(hdc, fontCache);
			}
			return this.m_cachedFont;
		}

		internal GlyphData GetGlyphData(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (this.m_cachedGlyphData == null)
			{
				this.ShapeAndPlace(hdc, fontCache);
			}
			else
			{
				this.LoadGlyphData(hdc, fontCache);
			}
			return this.m_cachedGlyphData;
		}

		internal int GetWidth(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			return this.GetWidth(hdc, fontCache, false);
		}

		internal int GetWidth(Win32DCSafeHandle hdc, FontCache fontCache, bool isAtLineEnd)
		{
			if (this.m_cachedGlyphData == null)
			{
				this.ShapeAndPlace(hdc, fontCache);
			}
			else
			{
				this.LoadGlyphData(hdc, fontCache);
			}
			return this.m_cachedGlyphData.GetTotalWidth(isAtLineEnd);
		}

		internal int GetHeight(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			CachedFont cachedFont = this.GetCachedFont(hdc, fontCache);
			return cachedFont.GetHeight(hdc, fontCache);
		}

		internal int GetAscent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			CachedFont cachedFont = this.GetCachedFont(hdc, fontCache);
			return cachedFont.GetAscent(hdc, fontCache);
		}

		internal int GetDescent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			CachedFont cachedFont = this.GetCachedFont(hdc, fontCache);
			return cachedFont.GetDescent(hdc, fontCache);
		}

		internal void ShapeAndPlace(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			bool verticalFont = false;
			if (fontCache.AllowVerticalFont)
			{
				verticalFont = this.HasEastAsianChars;
			}
			if (this.m_cachedFont == null)
			{
				this.m_cachedFont = fontCache.GetFont(this.m_textRunProps, this.GetCharset(), verticalFont);
				this.FallbackFont = false;
			}
			CachedFont cachedFont = this.m_cachedFont;
			bool flag = false;
			bool flag2 = false;
			string text = this.m_text;
			int num = Convert.ToInt32((double)text.Length * 1.5 + 16.0);
			this.m_cachedGlyphData = new GlyphData(num, text.Length);
			GlyphShapeData glyphScriptShapeData = this.m_cachedGlyphData.GlyphScriptShapeData;
			int num2 = Win32.ScriptShape(IntPtr.Zero, ref this.m_cachedFont.ScriptCache, text, text.Length, num, ref this.SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
			if (num2 == -2147483638)
			{
				flag = true;
				fontCache.SelectFontObject(hdc, this.m_cachedFont.Hfont);
				num2 = Win32.ScriptShape(hdc, ref this.m_cachedFont.ScriptCache, text, text.Length, num, ref this.SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
			}
			if (num2 == -2147024882)
			{
				num = text.Length * 3;
				this.m_cachedGlyphData = new GlyphData(num, text.Length);
				glyphScriptShapeData = this.m_cachedGlyphData.GlyphScriptShapeData;
				num2 = Win32.ScriptShape(hdc, ref this.m_cachedFont.ScriptCache, text, text.Length, num, ref this.SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
			}
			if (!this.FallbackFont)
			{
				if (num2 == -2147220992)
				{
					int num3 = 0;
					num3 = ((!this.m_itemizedScriptId.HasValue) ? this.ScriptAnalysis.eScript : this.m_itemizedScriptId.Value);
					this.m_cachedFont = fontCache.GetFallbackFont(this.m_textRunProps, this.GetCharset(), num3, verticalFont);
					fontCache.SelectFontObject(hdc, this.m_cachedFont.Hfont);
					flag = true;
					flag2 = true;
					num2 = Win32.ScriptShape(hdc, ref this.m_cachedFont.ScriptCache, text, text.Length, num, ref this.SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
				}
				else if (this.HasEastAsianChars)
				{
					if (!flag)
					{
						fontCache.SelectFontObject(hdc, this.m_cachedFont.Hfont);
						flag = true;
					}
					Win32.SCRIPT_FONTPROPERTIES sCRIPT_FONTPROPERTIES = default(Win32.SCRIPT_FONTPROPERTIES);
					sCRIPT_FONTPROPERTIES.cBytes = 16;
					num2 = Win32.ScriptGetFontProperties(hdc, ref this.m_cachedFont.ScriptCache, ref sCRIPT_FONTPROPERTIES);
					short wgDefault = sCRIPT_FONTPROPERTIES.wgDefault;
					int num4 = 0;
					num4 = ((!this.m_itemizedScriptId.HasValue) ? this.ScriptAnalysis.eScript : this.m_itemizedScriptId.Value);
					int num5 = 0;
					while (num5 < glyphScriptShapeData.GlyphCount)
					{
						if (glyphScriptShapeData.Glyphs[num5] != wgDefault)
						{
							num5++;
							continue;
						}
						this.m_cachedFont = fontCache.GetFallbackFont(this.m_textRunProps, this.GetCharset(), num4, verticalFont);
						this.m_cachedFont.DefaultGlyph = wgDefault;
						fontCache.SelectFontObject(hdc, this.m_cachedFont.Hfont);
						flag = true;
						flag2 = true;
						num2 = Win32.ScriptShape(hdc, ref this.m_cachedFont.ScriptCache, text, text.Length, num, ref this.SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
						break;
					}
				}
			}
			if (num2 == -2147220992)
			{
				this.m_cachedFont = cachedFont;
				if (!flag || flag2)
				{
					Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.SelectObject(hdc, this.m_cachedFont.Hfont);
					win32ObjectSafeHandle.SetHandleAsInvalid();
					flag = true;
				}
				flag2 = false;
				this.SetUndefinedScript();
				num2 = Win32.ScriptShape(hdc, ref this.m_cachedFont.ScriptCache, text, text.Length, num, ref this.SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
			}
			if (Win32.Failed(num2))
			{
				Marshal.ThrowExceptionForHR(num2);
			}
			if (flag2)
			{
				this.FallbackFont = true;
			}
			this.m_cachedGlyphData.TrimToGlyphCount();
			this.m_cachedGlyphData.ScaleFactor = this.m_cachedFont.ScaleFactor;
			this.TextScriptPlace(hdc, flag, fontCache);
		}

		private void TextScriptPlace(Win32DCSafeHandle hdc, bool fontSelected, FontCache fontCache)
		{
			int num = 0;
			GlyphShapeData glyphScriptShapeData = this.m_cachedGlyphData.GlyphScriptShapeData;
			if (fontSelected)
			{
				num = Win32.ScriptPlace(hdc, ref this.m_cachedFont.ScriptCache, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.VisAttrs, ref this.SCRIPT_ANALYSIS, this.m_cachedGlyphData.RawAdvances, this.m_cachedGlyphData.RawGOffsets, ref this.m_cachedGlyphData.ABC);
			}
			else
			{
				num = Win32.ScriptPlace(IntPtr.Zero, ref this.m_cachedFont.ScriptCache, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.VisAttrs, ref this.SCRIPT_ANALYSIS, this.m_cachedGlyphData.RawAdvances, this.m_cachedGlyphData.RawGOffsets, ref this.m_cachedGlyphData.ABC);
				if (num == -2147483638)
				{
					fontCache.SelectFontObject(hdc, this.m_cachedFont.Hfont);
					num = Win32.ScriptPlace(hdc, ref this.m_cachedFont.ScriptCache, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.VisAttrs, ref this.SCRIPT_ANALYSIS, this.m_cachedGlyphData.RawAdvances, this.m_cachedGlyphData.RawGOffsets, ref this.m_cachedGlyphData.ABC);
				}
			}
			if (Win32.Failed(num))
			{
				Marshal.ThrowExceptionForHR(num);
			}
			if (this.m_cachedGlyphData.ABC.Width > 0 && this.m_text.Length == 1 && TextBox.IsWhitespaceControlChar(this.m_text[0]))
			{
				this.m_cachedGlyphData.ABC.SetToZeroWidth();
			}
		}

		private void LoadGlyphData(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (!this.m_clone && this.m_cachedGlyphData.NeedGlyphPlaceData)
			{
				this.m_cachedGlyphData.NeedGlyphPlaceData = false;
				this.m_cachedGlyphData.ScaleFactor = this.m_cachedFont.ScaleFactor;
				this.TextScriptPlace(hdc, false, fontCache);
			}
		}

		private void SetUndefinedScript()
		{
			ScriptAnalysis scriptAnalysis = this.ScriptAnalysis;
			if (scriptAnalysis.eScript != 0)
			{
				this.m_itemizedScriptId = scriptAnalysis.eScript;
				scriptAnalysis.eScript = 0;
				this.SCRIPT_ANALYSIS = scriptAnalysis.GetAs_SCRIPT_ANALYSIS();
				this.m_scriptAnalysis = null;
			}
		}

		private byte GetCharset()
		{
			byte result = 1;
			ScriptProperties properties = ScriptProperties.GetProperties(this.ScriptAnalysis.eScript);
			if (!properties.IsComplex)
			{
				this.SetUndefinedScript();
			}
			else if (!properties.IsAmbiguousCharSet)
			{
				result = properties.CharSet;
			}
			return result;
		}
	}
}
