using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF.ExcelCallbacks.Convert;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class HeaderFooterRichTextFont : IFont
	{
		private StringBuilder m_builder;

		private bool m_boldSet;

		private bool m_italicSet;

		private bool m_strikethroughSet;

		private bool m_underlineSet;

		private string m_fontName;

		private double m_fontSize;

		public int Bold
		{
			set
			{
				bool flag = value >= 600;
				if (this.m_boldSet != flag)
				{
					this.m_builder.Append("&B");
				}
				this.m_boldSet = flag;
			}
		}

		public bool Italic
		{
			set
			{
				if (this.m_italicSet != value)
				{
					this.m_builder.Append("&I");
				}
				this.m_italicSet = value;
			}
		}

		public bool Strikethrough
		{
			set
			{
				if (this.m_strikethroughSet != value)
				{
					this.m_builder.Append("&s");
				}
				this.m_strikethroughSet = value;
			}
		}

		public ScriptStyle ScriptStyle
		{
			set
			{
			}
		}

		public IColor Color
		{
			set
			{
			}
		}

		public Underline Underline
		{
			set
			{
				bool flag = value != Underline.None;
				if (this.m_underlineSet != flag)
				{
					this.m_builder.Append("&u");
				}
				this.m_underlineSet = flag;
			}
		}

		public string Name
		{
			set
			{
				if (!string.IsNullOrEmpty(value) && value != this.m_fontName)
				{
					this.m_builder.Append("&").Append('"');
					FormulaHandler.EncodeHeaderFooterString(this.m_builder, value);
					this.m_builder.Append('"');
					this.m_fontName = value;
				}
			}
		}

		public double Size
		{
			set
			{
				if (value != 0.0 && this.m_fontSize != value)
				{
					this.m_builder.Append("&").Append((int)value);
				}
				this.m_fontSize = value;
			}
		}

		internal string LastFontName
		{
			get
			{
				return this.m_fontName;
			}
		}

		internal double LastFontSize
		{
			get
			{
				return this.m_fontSize;
			}
		}

		internal HeaderFooterRichTextFont(StringBuilder builder)
		{
			this.m_builder = builder;
		}
	}
}
