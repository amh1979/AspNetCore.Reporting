using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal sealed class PlaceholderTextRun : HighlightTextRun
	{
		private Color m_placeholderBorderColor = Color.Empty;

		private bool m_allowColorInversion = true;

		internal override bool IsPlaceholderTextRun
		{
			get
			{
				return true;
			}
		}

		internal Color PlaceholderBorderColor
		{
			get
			{
				return this.m_placeholderBorderColor;
			}
			set
			{
				this.m_placeholderBorderColor = value;
			}
		}

		internal override bool AllowColorInversion
		{
			get
			{
				return this.m_allowColorInversion;
			}
			set
			{
				this.m_allowColorInversion = value;
			}
		}

		internal PlaceholderTextRun(string text, ITextRunProps props)
			: base(text, props)
		{
		}

		internal PlaceholderTextRun(string text, TextRun textRun)
			: base(text, textRun.TextRunProperties)
		{
		}

		internal PlaceholderTextRun(string text, PlaceholderTextRun textRun)
			: base(text, textRun)
		{
			this.m_placeholderBorderColor = textRun.PlaceholderBorderColor;
		}

		internal PlaceholderTextRun(string text, PlaceholderTextRun textRun, SCRIPT_LOGATTR[] scriptLogAttr)
			: base(text, textRun, scriptLogAttr)
		{
			this.m_placeholderBorderColor = textRun.PlaceholderBorderColor;
		}

		internal override TextRun Split(string text, SCRIPT_LOGATTR[] scriptLogAttr)
		{
			return new PlaceholderTextRun(text, this, scriptLogAttr);
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
			PlaceholderTextRun placeholderTextRun = new PlaceholderTextRun(base.m_text.Substring(startIndex, length), this);
			placeholderTextRun.CharacterIndexInOriginal = startIndex;
			return placeholderTextRun;
		}
	}
}
