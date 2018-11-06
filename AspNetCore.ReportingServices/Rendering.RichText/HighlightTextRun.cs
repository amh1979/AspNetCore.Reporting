using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class HighlightTextRun : TextRun
	{
		private int m_highlightStart = -1;

		private int m_highlightEnd = -1;

		private Color m_highlightColor = Color.Empty;

		private int m_charIndexInOriginal;

		internal override int HighlightStart
		{
			get
			{
				return this.m_highlightStart;
			}
			set
			{
				this.m_highlightStart = value;
			}
		}

		internal override int HighlightEnd
		{
			get
			{
				return this.m_highlightEnd;
			}
			set
			{
				this.m_highlightEnd = value;
			}
		}

		internal override Color HighlightColor
		{
			get
			{
				return this.m_highlightColor;
			}
			set
			{
				this.m_highlightColor = value;
			}
		}

		internal override bool IsHighlightTextRun
		{
			get
			{
				return true;
			}
		}

		internal override int CharacterIndexInOriginal
		{
			get
			{
				return this.m_charIndexInOriginal;
			}
			set
			{
				this.m_charIndexInOriginal = value;
			}
		}

		internal HighlightTextRun(string text, ITextRunProps props)
			: base(text, props)
		{
		}

		internal HighlightTextRun(string text, TextRun textRun)
			: base(text, textRun.TextRunProperties)
		{
			this.m_charIndexInOriginal = textRun.CharacterIndexInOriginal;
		}

		internal HighlightTextRun(string text, HighlightTextRun textRun)
			: base(text, textRun)
		{
			this.m_highlightStart = textRun.m_highlightStart;
			this.m_highlightEnd = textRun.m_highlightEnd;
			this.m_highlightColor = textRun.m_highlightColor;
			this.m_charIndexInOriginal = textRun.CharacterIndexInOriginal;
		}

		internal HighlightTextRun(string text, HighlightTextRun textRun, SCRIPT_LOGATTR[] scriptLogAttr)
			: base(text, textRun, scriptLogAttr)
		{
			this.m_highlightStart = textRun.m_highlightStart;
			this.m_highlightEnd = textRun.m_highlightEnd;
			this.m_highlightColor = textRun.m_highlightColor;
			this.m_charIndexInOriginal = textRun.CharacterIndexInOriginal;
		}

		internal override TextRun Split(string text, SCRIPT_LOGATTR[] scriptLogAttr)
		{
			return new HighlightTextRun(text, this, scriptLogAttr);
		}

		internal override TextRun GetSubRun(int startIndex, int length)
		{
			if (length == base.m_text.Length)
			{
				return this;
			}
			if (startIndex > 0)
			{
				base.m_textRunProps.AddSplitIndex(startIndex);
			}
			HighlightTextRun highlightTextRun = new HighlightTextRun(base.m_text.Substring(startIndex, length), this);
			highlightTextRun.CharacterIndexInOriginal = startIndex;
			return highlightTextRun;
		}
	}
}
