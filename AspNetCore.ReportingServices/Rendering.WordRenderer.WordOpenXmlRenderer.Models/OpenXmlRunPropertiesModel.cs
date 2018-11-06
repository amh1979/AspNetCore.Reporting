using System;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlRunPropertiesModel : OpenXmlParagraphModel.IParagraphContent
	{
		private bool _rightToLeft;

		private bool _rightToLeftStyle;

		private bool _bold;

		private bool _italic;

		private bool _strikethrough;

		private bool _underline;

		private Color? _color;

		private string _font;

		private string _language;

		private double _size;

		public Color Color
		{
			set
			{
				this._color = value;
			}
		}

		public bool RightToLeft
		{
			set
			{
				this._rightToLeft = value;
			}
		}

		public string Language
		{
			set
			{
				this._language = value;
			}
		}

		public bool Strikethrough
		{
			set
			{
				this._strikethrough = value;
			}
		}

		public bool Underline
		{
			set
			{
				this._underline = value;
			}
		}

		public bool NoProof
		{
			private get;
			set;
		}

		public void SetBold(bool bold, bool rightToLeft)
		{
			this._bold = bold;
			this._rightToLeftStyle |= rightToLeft;
		}

		public void SetFont(string name, bool rightToLeft)
		{
			this._font = WordOpenXmlUtils.Escape(name);
			this._rightToLeftStyle |= rightToLeft;
		}

		public void SetItalic(bool italic, bool rightToLeft)
		{
			this._italic = italic;
			this._rightToLeftStyle |= rightToLeft;
		}

		public void SetSize(double size, bool rightToLeft)
		{
			this._size = ((size < 1.0) ? 1.0 : size);
			this._rightToLeftStyle |= rightToLeft;
		}

		public void Write(TextWriter q)
		{
			q.Write("<w:rPr>");
			if (this._font != null)
			{
				q.Write("<w:rFonts w:ascii=\"");
				q.Write(this._font);
				q.Write("\" w:hAnsi=\"");
				q.Write(this._font);
				q.Write("\" w:eastAsia=\"");
				q.Write(this._font);
				q.Write("\"");
				if (this._rightToLeftStyle)
				{
					q.Write(" w:cs=\"");
					q.Write(this._font);
					q.Write("\"");
				}
				q.Write("/>");
			}
			if (this._bold)
			{
				q.Write("<w:b/>");
				if (this._rightToLeftStyle)
				{
					q.Write("<w:bCs/>");
				}
			}
			if (this._italic)
			{
				q.Write("<w:i/>");
				if (this._rightToLeftStyle)
				{
					q.Write("<w:iCs/>");
				}
			}
			if (this._strikethrough)
			{
				q.Write("<w:strike/>");
			}
			if (this.NoProof)
			{
				q.Write("<w:noProof/>");
			}
			if (this._color.HasValue)
			{
				q.Write("<w:color w:val=\"");
				q.Write(WordOpenXmlUtils.RgbColor(this._color.Value));
				q.Write("\"/>");
			}
			if (this._size > 0.0)
			{
				q.Write("<w:sz w:val=\"");
				string value = Math.Min(3276uL, (ulong)Math.Round(this._size * 2.0)).ToString(CultureInfo.InvariantCulture);
				q.Write(value);
				q.Write("\"/>");
				if (this._rightToLeftStyle)
				{
					q.Write("<w:szCs w:val=\"");
					q.Write(value);
					q.Write("\"/>");
				}
			}
			if (this._underline)
			{
				q.Write("<w:u w:val=\"single\"/>");
			}
			if (this._rightToLeft)
			{
				if (this._language != null)
				{
					q.Write("<w:lang");
					q.Write(" w:bidi=\"");
					q.Write(this._language);
					q.Write("\"");
					q.Write("/>");
				}
				q.Write("<w:rtl/>");
			}
			q.Write("</w:rPr>");
		}
	}
}
