using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class PrefixRun : ITextRunProps
	{
		private const float PrefixFontSize = 10f;

		private string m_fontKey;

		public string FontFamily
		{
			get
			{
				return this.FontName;
			}
		}

		public float FontSize
		{
			get
			{
				return 10f;
			}
		}

		public Color Color
		{
			get
			{
				return Color.Black;
			}
		}

		public bool Bold
		{
			get
			{
				return false;
			}
		}

		public bool Italic
		{
			get
			{
				return false;
			}
		}

		public RPLFormat.TextDecorations TextDecoration
		{
			get
			{
				return RPLFormat.TextDecorations.None;
			}
		}

		public int IndexInParagraph
		{
			get
			{
				return -1;
			}
		}

		public string FontKey
		{
			get
			{
				return this.m_fontKey;
			}
			set
			{
				this.m_fontKey = value;
			}
		}

		internal virtual string FontName
		{
			get
			{
				return null;
			}
		}

		public void AddSplitIndex(int index)
		{
		}
	}
}
