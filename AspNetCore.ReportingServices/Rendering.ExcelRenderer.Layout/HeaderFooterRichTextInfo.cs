using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class HeaderFooterRichTextInfo : IRichTextInfo
	{
		private HeaderFooterRichTextFont m_font;

		private StringBuilder m_stringBuilder;

		private StringBuilder m_valueBuilder;

		internal string LastFontName
		{
			get
			{
				return this.m_font.LastFontName;
			}
		}

		internal double LastFontSize
		{
			get
			{
				return this.m_font.LastFontSize;
			}
		}

		public bool CheckForRotatedFarEastChars
		{
			set
			{
			}
		}

		internal HeaderFooterRichTextInfo(StringBuilder builder)
		{
			this.m_stringBuilder = builder;
			this.m_font = new HeaderFooterRichTextFont(this.m_stringBuilder);
			this.m_valueBuilder = new StringBuilder();
		}

		public IFont AppendTextRun(string value)
		{
			return this.AppendTextRun(value, true);
		}

		public IFont AppendTextRun(string value, bool replaceInvalidWhiteSpace)
		{
			if (this.m_valueBuilder.Length > 0)
			{
				this.m_stringBuilder.Append(this.m_valueBuilder.ToString());
				this.m_valueBuilder.Remove(0, this.m_valueBuilder.Length);
			}
			if (replaceInvalidWhiteSpace)
			{
				FormulaHandler.EncodeHeaderFooterString(this.m_valueBuilder, value);
			}
			else
			{
				this.m_valueBuilder.Append(value);
			}
			return this.m_font;
		}

		public void AppendText(string value)
		{
			this.AppendText(value, true);
		}

		public void AppendText(string value, bool replaceInvalidWhiteSpace)
		{
			if (replaceInvalidWhiteSpace)
			{
				FormulaHandler.EncodeHeaderFooterString(this.m_valueBuilder, value);
			}
			else
			{
				this.m_valueBuilder.Append(value);
			}
		}

		public void AppendText(char value)
		{
			this.m_valueBuilder.Append(value);
		}

		internal void CompleteRun()
		{
			if (this.m_valueBuilder.Length > 0)
			{
				this.m_stringBuilder.Append(this.m_valueBuilder.ToString());
				this.m_valueBuilder.Remove(0, this.m_valueBuilder.Length);
			}
			this.m_stringBuilder.Append(' ');
		}

		internal void CompleteCurrentFormatting()
		{
			this.m_font.Bold = 0;
			this.m_font.Italic = false;
			this.m_font.ScriptStyle = ScriptStyle.None;
			this.m_font.Strikethrough = false;
			this.m_font.Underline = Underline.None;
		}
	}
}
