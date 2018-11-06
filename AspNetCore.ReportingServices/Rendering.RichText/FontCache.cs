using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class FontCache : IDisposable
	{
		private const int DEFAULT_EM_HEIGHT = 2048;

		internal static List<string> FontFallback = FontCache.PopulateFontFallBack();

		internal static Dictionary<int, int> ScriptFontMapping = FontCache.PopulateScriptFontMapping();

		private Dictionary<string, CachedFont> m_fontDict;

		private readonly float m_dpi;

		private Win32ObjectSafeHandle m_selectedFont = Win32ObjectSafeHandle.Zero;

		private Win32DCSafeHandle m_selectedHdc = Win32DCSafeHandle.Zero;

		private RPLFormat.WritingModes m_writingMode;

		private bool m_useEmSquare;

		private uint m_fontQuality;

		internal float Dpi
		{
			get
			{
				return this.m_dpi;
			}
		}

		internal RPLFormat.WritingModes WritingMode
		{
			set
			{
				this.m_writingMode = value;
			}
		}

		internal bool VerticalMode
		{
			get
			{
				if (this.m_writingMode != RPLFormat.WritingModes.Vertical)
				{
					return this.m_writingMode == RPLFormat.WritingModes.Rotate270;
				}
				return true;
			}
		}

		internal bool UseEmSquare
		{
			get
			{
				return this.m_useEmSquare;
			}
		}

		internal bool AllowVerticalFont
		{
			get
			{
				if (this.m_writingMode == RPLFormat.WritingModes.Vertical)
				{
					return true;
				}
				return false;
			}
		}

		public uint FontQuality
		{
			get
			{
				return this.m_fontQuality;
			}
			set
			{
				this.m_fontQuality = value;
			}
		}

		internal FontCache(float dpi)
		{
			this.m_fontDict = new Dictionary<string, CachedFont>();
			this.m_dpi = dpi;
		}

		internal FontCache(float dpi, bool useEmSquare)
			: this(dpi)
		{
			this.m_useEmSquare = useEmSquare;
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_fontDict != null)
				{
					foreach (CachedFont value in this.m_fontDict.Values)
					{
						value.Dispose();
					}
					this.m_fontDict = null;
				}
				this.ResetGraphics();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~FontCache()
		{
			this.Dispose(false);
		}

		internal void ResetGraphics()
		{
			this.m_selectedHdc = Win32DCSafeHandle.Zero;
			this.m_selectedFont = Win32ObjectSafeHandle.Zero;
		}

		internal CachedFont GetFont(ITextRunProps textRunProps, byte charset, bool verticalFont)
		{
			string fontFamily = null;
			float fontSize = 0f;
			CachedFont result = null;
			string runKey = this.GetRunKey(textRunProps, out fontFamily, out fontSize);
			string key = this.GetKey(runKey, charset, verticalFont, null, null);
			if (!this.m_fontDict.TryGetValue(key, out result))
			{
				return this.CreateFont(textRunProps, key, charset, verticalFont, fontFamily, fontSize);
			}
			return result;
		}

		internal CachedFont GetFallbackFont(ITextRunProps textRunProps, byte charset, int script, bool verticalFont)
		{
			string fontFamily = null;
			float fontSize = 0f;
			string runKey = this.GetRunKey(textRunProps, out fontFamily, out fontSize);
			int index = 0;
			if (!FontCache.ScriptFontMapping.TryGetValue(script, out index))
			{
				index = 0;
			}
			fontFamily = FontCache.FontFallback[index];
			string key = this.GetKey(runKey, charset, verticalFont, fontFamily, null);
			CachedFont result = null;
			if (!this.m_fontDict.TryGetValue(key, out result))
			{
				return this.CreateFont(textRunProps, key, charset, verticalFont, fontFamily, fontSize);
			}
			return result;
		}

		internal CachedFont GetFont(ITextRunProps textRunProps, byte charset, float fontSize, bool verticalFont)
		{
			string fontFamily = null;
			float num = 0f;
			string runKey = this.GetRunKey(textRunProps, out fontFamily, out num);
			string key = this.GetKey(runKey, charset, verticalFont, null, fontSize);
			CachedFont result = null;
			if (!this.m_fontDict.TryGetValue(key, out result))
			{
				return this.CreateFont(textRunProps, key, charset, verticalFont, fontFamily, fontSize);
			}
			return result;
		}

		private CachedFont CreateFont(ITextRunProps textRun, string key, byte charset, bool verticalFont, string fontFamily, float fontSize)
		{
			CachedFont cachedFont = new CachedFont();
			this.m_fontDict.Add(key, cachedFont);
			bool bold = textRun.Bold;
			bool italic = textRun.Italic;
			RPLFormat.TextDecorations textDecoration = textRun.TextDecoration;
			bool lineThrough = textDecoration == RPLFormat.TextDecorations.LineThrough;
			cachedFont.Font = FontCache.CreateGdiPlusFont(fontFamily, textRun.FontSize, ref bold, ref italic, lineThrough, false);
			int num = 0;
			if (this.UseEmSquare)
			{
				int emHeight = cachedFont.Font.FontFamily.GetEmHeight(cachedFont.Font.Style);
				cachedFont.ScaleFactor = (float)emHeight / fontSize;
				num = emHeight;
			}
			else
			{
				num = (int)(fontSize + 0.5);
			}
			cachedFont.Hfont = this.CreateGdiFont(this.m_writingMode, num, bold, italic, lineThrough, charset, verticalFont, cachedFont.Font.FontFamily.Name);
			return cachedFont;
		}

		private Win32ObjectSafeHandle CreateGdiFont(RPLFormat.WritingModes writingMode, int fontSize, bool bold, bool italic, bool lineThrough, byte charset, bool verticalFont, string fontFamily)
		{
			int nEscapement = 0;
			int nOrientation = 0;
			if (this.VerticalMode)
			{
				if (!this.UseEmSquare)
				{
					if (writingMode == RPLFormat.WritingModes.Vertical)
					{
						nEscapement = 2700;
						nOrientation = 2700;
					}
					else
					{
						nEscapement = 900;
						nOrientation = 900;
					}
				}
				if (verticalFont)
				{
					fontFamily = "@" + fontFamily;
				}
			}
			return Win32.CreateFont(-fontSize, 0, nEscapement, nOrientation, bold ? 700 : 400, (uint)(italic ? 1 : 0), 0u, (uint)(lineThrough ? 1 : 0), charset, 4u, 0u, this.m_fontQuality, 0u, fontFamily);
		}

		private string GetRunKey(ITextRunProps textRunProps, out string fontFamily, out float fontSize)
		{
			string text = textRunProps.FontKey;
			if (text == null)
			{
				text = (textRunProps.FontKey = this.GetKey(textRunProps, out fontFamily, out fontSize));
			}
			else
			{
				fontFamily = textRunProps.FontFamily;
				fontSize = (float)(textRunProps.FontSize * this.m_dpi / 72.0);
			}
			return text;
		}

		private string GetKey(string runKey, int charset, bool verticalFont, string fontFamily, float? fontSize)
		{
			StringBuilder stringBuilder = new StringBuilder(runKey);
			stringBuilder.Append(fontFamily);
			if (fontSize.HasValue)
			{
				stringBuilder.Append(fontSize.Value.ToString(CultureInfo.InvariantCulture));
			}
			switch (this.m_writingMode)
			{
			case RPLFormat.WritingModes.Horizontal:
				stringBuilder.Append('h');
				break;
			case RPLFormat.WritingModes.Vertical:
				stringBuilder.Append('v');
				break;
			case RPLFormat.WritingModes.Rotate270:
				stringBuilder.Append('r');
				break;
			}
			stringBuilder.Append(charset);
			stringBuilder.Append((char)(verticalFont ? 116 : 102));
			return stringBuilder.ToString();
		}

		private string GetKey(ITextRunProps textRunProps, out string fontFamily, out float fontSize)
		{
			fontSize = (float)(textRunProps.FontSize * this.m_dpi / 72.0);
			fontFamily = textRunProps.FontFamily;
			StringBuilder stringBuilder = new StringBuilder(fontFamily);
			stringBuilder.Append(fontSize.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append((char)(textRunProps.Bold ? 98 : 110));
			stringBuilder.Append((char)(textRunProps.Italic ? 105 : 110));
			RPLFormat.TextDecorations textDecoration = textRunProps.TextDecoration;
			if (this.UseEmSquare && textDecoration == RPLFormat.TextDecorations.Underline)
			{
				stringBuilder.Append('u');
			}
			else
			{
				switch (textDecoration)
				{
				case RPLFormat.TextDecorations.Overline:
					stringBuilder.Append('o');
					break;
				case RPLFormat.TextDecorations.LineThrough:
					stringBuilder.Append('s');
					break;
				default:
					stringBuilder.Append('n');
					break;
				}
			}
			return stringBuilder.ToString();
		}

		internal void SelectFontObject(Win32DCSafeHandle hdc, Win32ObjectSafeHandle hFont)
		{
			if (hdc != this.m_selectedHdc)
			{
				this.m_selectedHdc = hdc;
				this.m_selectedFont = Win32ObjectSafeHandle.Zero;
			}
			if (hFont != this.m_selectedFont)
			{
				Win32ObjectSafeHandle win32ObjectSafeHandle = Win32.SelectObject(hdc, hFont);
				win32ObjectSafeHandle.SetHandleAsInvalid();
				this.m_selectedFont = hFont;
			}
		}

		internal static Font CreateGdiPlusFont(string fontFamilyName, float fontSize, ref bool bold, ref bool italic, bool lineThrough, bool underLine)
		{
			FontStyle fontStyle = FontStyle.Regular;
			if (bold)
			{
				fontStyle |= FontStyle.Bold;
			}
			if (italic)
			{
				fontStyle |= FontStyle.Italic;
			}
			if (underLine)
			{
				fontStyle |= FontStyle.Underline;
			}
			else if (lineThrough)
			{
				fontStyle |= FontStyle.Strikeout;
			}
			Font font = null;
			try
			{
				try
				{
					font = new Font(fontFamilyName, fontSize, fontStyle);
				}
				catch (ArgumentException)
				{
					FontStyle fontStyle2 = FontStyle.Bold;
					if (italic)
					{
						fontStyle2 |= FontStyle.Italic;
					}
					if (underLine)
					{
						fontStyle2 |= FontStyle.Underline;
					}
					else if (lineThrough)
					{
						fontStyle2 |= FontStyle.Strikeout;
					}
					try
					{
						font = new Font(fontFamilyName, fontSize, fontStyle2);
						bold = true;
					}
					catch (ArgumentException)
					{
						fontStyle2 = FontStyle.Italic;
						if (bold)
						{
							fontStyle2 |= FontStyle.Bold;
						}
						if (underLine)
						{
							fontStyle2 |= FontStyle.Underline;
						}
						else if (lineThrough)
						{
							fontStyle2 |= FontStyle.Strikeout;
						}
						try
						{
							font = new Font(fontFamilyName, fontSize, fontStyle2);
						}
						catch (ArgumentException)
						{
							fontStyle2 = (FontStyle.Bold | FontStyle.Italic);
							if (underLine)
							{
								fontStyle2 |= FontStyle.Underline;
							}
							else if (lineThrough)
							{
								fontStyle2 |= FontStyle.Strikeout;
							}
							try
							{
								font = new Font(fontFamilyName, fontSize, fontStyle2);
							}
							catch (ArgumentException)
							{
								font = new Font(FontFamily.GenericSansSerif, fontSize, fontStyle);
							}
						}
					}
				}
				bold = font.Bold;
				italic = font.Italic;
				return font;
			}
			catch
			{
				if (font != null)
				{
					font.Dispose();
					font = null;
				}
				throw;
			}
		}

		private static List<string> PopulateFontFallBack()
		{
			List<string> list = new List<string>(28);
			list.Add("Microsoft Sans Serif");
			list.Add("Mangal");
			list.Add("Latha");
			list.Add("Estrangelo Edessa");
			list.Add("Sylfaen");
			list.Add("Raavi");
			list.Add("Shruti");
			list.Add("Tunga");
			list.Add("Gautami");
			list.Add("Mv Boli");
			list.Add("Vrinda");
			list.Add("Kartika");
			list.Add("Kalinga");
			list.Add("Microsoft Himalaya");
			list.Add("DokChampa");
			list.Add("DaunPenh");
			list.Add("Mongolian Baiti");
			list.Add("Nyala");
			list.Add("Plantagenet Cherokee");
			list.Add("Euphemia");
			list.Add("Iskoola Pota");
			list.Add("Microsoft Yi Baiti");
			list.Add("MS PGothic");
			list.Add("PMingLiu");
			list.Add("Gulim");
			list.Add("Microsoft Tai Le");
			list.Add("Microsoft New Tai Lue");
			list.Add("MingLiU-ExtB");
			return list;
		}

		private static Dictionary<int, int> PopulateScriptFontMapping()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>(49);
			dictionary.Add(34, 1);
			dictionary.Add(35, 1);
			dictionary.Add(36, 2);
			dictionary.Add(37, 2);
			dictionary.Add(30, 3);
			dictionary.Add(9, 4);
			dictionary.Add(10, 4);
			dictionary.Add(41, 5);
			dictionary.Add(42, 5);
			dictionary.Add(43, 6);
			dictionary.Add(44, 6);
			dictionary.Add(45, 6);
			dictionary.Add(50, 7);
			dictionary.Add(51, 7);
			dictionary.Add(48, 8);
			dictionary.Add(49, 8);
			dictionary.Add(66, 9);
			dictionary.Add(38, 10);
			dictionary.Add(39, 10);
			dictionary.Add(40, 10);
			dictionary.Add(52, 11);
			dictionary.Add(53, 11);
			dictionary.Add(46, 12);
			dictionary.Add(47, 12);
			dictionary.Add(54, 13);
			dictionary.Add(55, 13);
			dictionary.Add(56, 14);
			dictionary.Add(57, 14);
			dictionary.Add(58, 15);
			dictionary.Add(59, 15);
			dictionary.Add(62, 16);
			dictionary.Add(63, 16);
			dictionary.Add(64, 17);
			dictionary.Add(65, 17);
			dictionary.Add(68, 18);
			dictionary.Add(67, 19);
			dictionary.Add(72, 20);
			dictionary.Add(21, 21);
			dictionary.Add(14, 22);
			dictionary.Add(15, 22);
			dictionary.Add(16, 22);
			dictionary.Add(17, 23);
			dictionary.Add(18, 24);
			dictionary.Add(19, 24);
			dictionary.Add(20, 23);
			dictionary.Add(22, 23);
			dictionary.Add(75, 25);
			dictionary.Add(76, 26);
			dictionary.Add(12, 27);
			return dictionary;
		}
	}
}
