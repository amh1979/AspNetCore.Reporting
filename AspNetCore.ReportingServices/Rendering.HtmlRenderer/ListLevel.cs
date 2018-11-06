using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class ListLevel
	{
		private static byte[] m_closeUL;

		private static byte[] m_closeOL;

		private static byte[] m_olArabic;

		private static byte[] m_olRoman;

		private static byte[] m_olAlpha;

		private static byte[] m_ulCircle;

		private static byte[] m_ulDisc;

		private static byte[] m_ulSquare;

		private static byte[] m_classNoVerticalMargin;

		private static byte[] m_noVerticalMarginClassName;

		private static byte[] m_closeBracket;

		private int m_listLevel;

		private RPLFormat.ListStyles m_style = RPLFormat.ListStyles.Bulleted;

		private IHtmlReportWriter m_renderer;

		public int Level
		{
			get
			{
				return this.m_listLevel;
			}
			set
			{
				this.m_listLevel = value;
			}
		}

		public RPLFormat.ListStyles Style
		{
			get
			{
				return this.m_style;
			}
			set
			{
				this.m_style = value;
			}
		}

		static ListLevel()
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			ListLevel.m_closeUL = uTF8Encoding.GetBytes("</ul>");
			ListLevel.m_closeOL = uTF8Encoding.GetBytes("</ol>");
			ListLevel.m_olArabic = uTF8Encoding.GetBytes("<ol");
			ListLevel.m_olRoman = uTF8Encoding.GetBytes("<ol type=\"i\"");
			ListLevel.m_olAlpha = uTF8Encoding.GetBytes("<ol type=\"a\"");
			ListLevel.m_ulDisc = uTF8Encoding.GetBytes("<ul type=\"disc\"");
			ListLevel.m_ulSquare = uTF8Encoding.GetBytes("<ul type=\"square\"");
			ListLevel.m_ulCircle = uTF8Encoding.GetBytes("<ul type=\"circle\"");
			ListLevel.m_classNoVerticalMargin = uTF8Encoding.GetBytes(" class=\"r16\"");
			ListLevel.m_noVerticalMarginClassName = uTF8Encoding.GetBytes("r16");
			ListLevel.m_closeBracket = uTF8Encoding.GetBytes(">");
		}

		internal ListLevel(IHtmlReportWriter renderer, int listLevel, RPLFormat.ListStyles style)
		{
			this.m_renderer = renderer;
			this.m_listLevel = listLevel;
			this.m_style = style;
		}

		internal void Open(bool writeNoVerticalMarginClass)
		{
			byte[] bytes = ListLevel.m_olArabic;
			switch (this.m_style)
			{
			case RPLFormat.ListStyles.Numbered:
				switch (this.m_listLevel % 3)
				{
				case 2:
					bytes = ListLevel.m_olRoman;
					break;
				case 0:
					bytes = ListLevel.m_olAlpha;
					break;
				}
				break;
			case RPLFormat.ListStyles.Bulleted:
				switch (this.m_listLevel % 3)
				{
				case 1:
					bytes = ListLevel.m_ulDisc;
					break;
				case 2:
					bytes = ListLevel.m_ulCircle;
					break;
				case 0:
					bytes = ListLevel.m_ulSquare;
					break;
				}
				break;
			}
			this.m_renderer.WriteStream(bytes);
			if (this.m_listLevel == 1 && writeNoVerticalMarginClass)
			{
				this.m_renderer.WriteClassName(ListLevel.m_noVerticalMarginClassName, ListLevel.m_classNoVerticalMargin);
			}
			this.m_renderer.WriteStream(ListLevel.m_closeBracket);
		}

		internal void Close()
		{
			byte[] bytes = ListLevel.m_closeOL;
			if (this.m_style == RPLFormat.ListStyles.Bulleted)
			{
				bytes = ListLevel.m_closeUL;
			}
			this.m_renderer.WriteStream(bytes);
		}
	}
}
