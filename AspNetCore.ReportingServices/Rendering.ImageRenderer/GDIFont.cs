using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class GDIFont : IDisposable
	{
		private Font m_font;

		private float m_height;

		private string m_family;

		private float m_size;

		private string m_key;

		internal Font Font
		{
			get
			{
				return this.m_font;
			}
		}

		internal float Height
		{
			get
			{
				return this.m_height;
			}
		}

		internal string Family
		{
			get
			{
				if (this.m_family == null)
				{
					this.m_family = this.m_font.FontFamily.GetName(1033);
				}
				return this.m_family;
			}
		}

		internal float Size
		{
			get
			{
				if (this.m_size == 0.0)
				{
					this.m_size = this.m_font.Size;
				}
				return this.m_size;
			}
		}

		internal string Key
		{
			get
			{
				return this.m_key;
			}
		}

		internal GDIFont(string key, Font font, float height)
		{
			this.m_key = key;
			this.m_font = font;
			this.m_height = height;
		}

		private void Dispose(bool disposing)
		{
			if (disposing && this.m_font != null)
			{
				this.m_font.Dispose();
				this.m_font = null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GDIFont()
		{
			this.Dispose(false);
		}

		internal static GDIFont GetOrCreateFont(Dictionary<string, GDIFont> gdiFonts, string fontFamily, float fontSize, RPLFormat.FontWeights fontWeight, RPLFormat.FontStyles fontStyle, RPLFormat.TextDecorations textDecoration)
		{
			string key = GDIFont.GetKey(fontFamily, fontSize, fontWeight, fontStyle, textDecoration);
			GDIFont gDIFont = default(GDIFont);
			if (gdiFonts.TryGetValue(key, out gDIFont))
			{
				return gDIFont;
			}
			bool flag = SharedRenderer.IsWeightBold(fontWeight);
			bool flag2 = fontStyle == RPLFormat.FontStyles.Italic;
			bool underLine = false;
			bool lineThrough = false;
			switch (textDecoration)
			{
			case RPLFormat.TextDecorations.Underline:
				underLine = true;
				break;
			case RPLFormat.TextDecorations.LineThrough:
				lineThrough = true;
				break;
			}
			Font font = null;
			try
			{
				font = FontCache.CreateGdiPlusFont(fontFamily, fontSize, ref flag, ref flag2, lineThrough, underLine);
				gDIFont = new GDIFont(key, font, fontSize);
				gdiFonts.Add(key, gDIFont);
				return gDIFont;
			}
			catch
			{
				if (font != null && !gdiFonts.ContainsKey(key))
				{
					font.Dispose();
					font = null;
				}
				throw;
			}
		}

		private static string GetKey(string fontFamily, float fontSize, RPLFormat.FontWeights fontWeight, RPLFormat.FontStyles fontStyle, RPLFormat.TextDecorations textDecoration)
		{
			StringBuilder stringBuilder = new StringBuilder("FO");
			stringBuilder.Append(fontFamily);
			stringBuilder.Append(fontSize.ToString(CultureInfo.InvariantCulture));
			if (SharedRenderer.IsWeightBold(fontWeight))
			{
				stringBuilder.Append('b');
			}
			else
			{
				stringBuilder.Append('n');
			}
			if (fontStyle == RPLFormat.FontStyles.Italic)
			{
				stringBuilder.Append('i');
			}
			else
			{
				stringBuilder.Append('n');
			}
			switch (textDecoration)
			{
			case RPLFormat.TextDecorations.Underline:
				stringBuilder.Append('u');
				break;
			case RPLFormat.TextDecorations.LineThrough:
				stringBuilder.Append('s');
				break;
			default:
				stringBuilder.Append('n');
				break;
			}
			return stringBuilder.ToString();
		}
	}
}
