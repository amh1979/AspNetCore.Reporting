using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class RichTextInfo : IRichTextInfo
	{
		private List<Pair<int, int>> m_fontList;

		private BIFF8Font m_font;

		private StyleContainer m_styleContainer;

		private StringBuilder m_stringBuilder;

		private int m_startIndex = -1;

		private int m_maxFontIndex;

		private bool m_firstRun;

		private bool m_checkForRotatedFarEastChars;

		private bool m_foundRotatedFarEastChar;

		internal int MaxFontIndex
		{
			get
			{
				return this.m_maxFontIndex;
			}
		}

		internal bool FoundRotatedFarEastChar
		{
			get
			{
				return this.m_foundRotatedFarEastChar;
			}
		}

		public bool CheckForRotatedFarEastChars
		{
			set
			{
				this.m_checkForRotatedFarEastChars = value;
			}
		}

		internal RichTextInfo(StyleContainer styleContainer)
		{
			this.m_styleContainer = styleContainer;
			this.m_startIndex = 0;
			this.m_font = null;
			this.m_fontList = new List<Pair<int, int>>();
			this.m_stringBuilder = new StringBuilder();
			this.m_maxFontIndex = 0;
			this.m_firstRun = true;
		}

		public IFont AppendTextRun(string value)
		{
			return this.AppendTextRun(value, true);
		}

		public IFont AppendTextRun(string value, bool replaceInvalidWhiteSpace)
		{
			if (this.m_font != null)
			{
				int num = this.m_styleContainer.AddFont(this.m_font);
				this.m_maxFontIndex = Math.Max(this.m_maxFontIndex, num);
				this.m_fontList.Add(new Pair<int, int>(this.m_startIndex, num));
			}
			this.m_startIndex = this.m_stringBuilder.Length;
			if (replaceInvalidWhiteSpace)
			{
				this.AppendWithChecking(value);
			}
			else
			{
				this.m_stringBuilder.Append(value);
			}
			if (this.m_firstRun)
			{
				this.m_firstRun = false;
				return this.m_styleContainer;
			}
			this.m_font = new BIFF8Font();
			return this.m_font;
		}

		public void AppendText(string value)
		{
			this.AppendText(value, true);
		}

		public void AppendText(string value, bool replaceInvalidWhiteSpace)
		{
			if (this.m_startIndex >= 0)
			{
				if (replaceInvalidWhiteSpace)
				{
					this.AppendWithChecking(value);
				}
				else
				{
					this.m_stringBuilder.Append(value);
				}
			}
		}

		public void AppendText(char value)
		{
			if (this.m_startIndex >= 0)
			{
				this.m_stringBuilder.Append(value);
			}
		}

		private void AppendWithChecking(string value)
		{
			bool flag = default(bool);
			ExcelGeneratorStringUtil.ConvertWhitespaceAppendString(value, this.m_stringBuilder, this.m_checkForRotatedFarEastChars, out flag);
			if (this.m_checkForRotatedFarEastChars && flag)
			{
				this.m_checkForRotatedFarEastChars = false;
				this.m_foundRotatedFarEastChar = true;
			}
		}

		internal StringWrapperBIFF8 CompleteRun()
		{
			if (this.m_font != null)
			{
				int num = this.m_styleContainer.AddFont(this.m_font);
				this.m_maxFontIndex = Math.Max(this.m_maxFontIndex, num);
				this.m_fontList.Add(new Pair<int, int>(this.m_startIndex, num));
			}
			StringWrapperBIFF8 stringWrapperBIFF = new StringWrapperBIFF8(this.m_stringBuilder.ToString());
			stringWrapperBIFF.SetRunsList(this.m_fontList);
			this.m_fontList = null;
			this.m_font = null;
			this.m_stringBuilder = null;
			return stringWrapperBIFF;
		}
	}
}
