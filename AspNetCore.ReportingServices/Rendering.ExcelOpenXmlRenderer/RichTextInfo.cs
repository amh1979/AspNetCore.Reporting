using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class RichTextInfo : IRichTextInfo
	{
		public delegate Style CreateStyle();

		private class CharacterRunFont
		{
			private Font _font;

			public int StartIndex
			{
				get;
				private set;
			}

			public int Length
			{
				get;
				private set;
			}

			public Font Font
			{
				get
				{
					return this._font;
				}
			}

			public CharacterRunFont(Font font, int startIndex, int length)
			{
				this.StartIndex = startIndex;
				this.Length = length;
				this._font = font;
			}

			public void Expand(int additionalLength)
			{
				this.Length += additionalLength;
			}
		}

		private Cell _cell;

		private bool _checkForRotatedFarEastChars;

		private StringBuilder _text;

		private CreateStyle _createStyle;

		private readonly int _row;

		private readonly int _column;

		private List<CharacterRunFont> _fonts;

		private bool _foundRotatedEastAsianChar;

		public bool CheckForRotatedFarEastChars
		{
			set
			{
				this._checkForRotatedFarEastChars = value;
			}
		}

		public bool FoundRotatedEastAsianChar
		{
			get
			{
				return this._foundRotatedEastAsianChar;
			}
		}

		public RichTextInfo(Cell cell, CreateStyle createStyle, int row, int column)
		{
			this._cell = cell;
			this._text = new StringBuilder();
			this._createStyle = createStyle;
			this._row = row;
			this._column = column;
			this._fonts = new List<CharacterRunFont>();
		}

		public void Commit(Style cellStyle)
		{
			if (this._text.Length > 32767)
			{
				throw new ReportRenderingException(ExcelRenderRes.MaxStringLengthExceeded(this._row.ToString(CultureInfo.InvariantCulture), this._column.ToString(CultureInfo.InvariantCulture)));
			}
			this._cell.Value = this._text.ToString();
			bool flag = true;
			foreach (CharacterRunFont font in this._fonts)
			{
				if (font.Length > 0)
				{
					if (flag)
					{
						cellStyle.Font.SetFont(font.Font);
						flag = false;
					}
					CharacterRun characters = this._cell.GetCharacters(font.StartIndex, font.Length);
					characters.Font = font.Font;
				}
			}
		}

		private void AppendWithChecking(string value)
		{
			bool flag = false;
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(value, this._text, this._checkForRotatedFarEastChars, out flag);
			if (flag)
			{
				this._checkForRotatedFarEastChars = false;
				this._foundRotatedEastAsianChar = true;
			}
		}

		private void ExpandLastRun(int length)
		{
			if (this._fonts != null && this._fonts.Count > 0)
			{
				CharacterRunFont characterRunFont = this._fonts[this._fonts.Count - 1];
				characterRunFont.Expand(length);
			}
		}

		public IFont AppendTextRun(string value)
		{
			return this.AppendTextRun(value, true);
		}

		public IFont AppendTextRun(string value, bool replaceInvalidWhitespace)
		{
			int length = this._text.Length;
			if (replaceInvalidWhitespace)
			{
				this.AppendWithChecking(value);
			}
			else
			{
				this._text.Append(value);
			}
			CharacterRunFont characterRunFont = new CharacterRunFont(this._createStyle().Font, length, this._text.Length - length);
			this._fonts.Add(characterRunFont);
			return characterRunFont.Font;
		}

		public void AppendText(string value)
		{
			this.AppendText(value, true);
		}

		public void AppendText(string value, bool replaceInvalidWhiteSpace)
		{
			if (replaceInvalidWhiteSpace)
			{
				this.AppendWithChecking(value);
			}
			else
			{
				this._text.Append(value);
			}
			this.ExpandLastRun(value.Length);
		}

		public void AppendText(char value)
		{
			this._text.Append(value.ToString());
			this.ExpandLastRun(1);
		}
	}
}
