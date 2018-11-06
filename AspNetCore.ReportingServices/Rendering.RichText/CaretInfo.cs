using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class CaretInfo
	{
		private Point m_position = Point.Empty;

		private bool m_isFirstLine;

		private bool m_isLastLine;

		private int m_ascent;

		private int m_descent;

		private int m_height;

		private int m_lineHeight;

		private int m_lineYOffset;

		internal Point Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				this.m_position = value;
			}
		}

		internal bool IsFirstLine
		{
			get
			{
				return this.m_isFirstLine;
			}
			set
			{
				this.m_isFirstLine = value;
			}
		}

		internal bool IsLastLine
		{
			get
			{
				return this.m_isLastLine;
			}
			set
			{
				this.m_isLastLine = value;
			}
		}

		internal int Ascent
		{
			get
			{
				return this.m_ascent;
			}
			set
			{
				this.m_ascent = value;
			}
		}

		internal int Descent
		{
			get
			{
				return this.m_descent;
			}
			set
			{
				this.m_descent = value;
			}
		}

		internal int LineHeight
		{
			get
			{
				return this.m_lineHeight;
			}
			set
			{
				this.m_lineHeight = value;
			}
		}

		internal int LineYOffset
		{
			get
			{
				return this.m_lineYOffset;
			}
			set
			{
				this.m_lineYOffset = value;
			}
		}

		internal int Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				this.m_height = value;
			}
		}

		internal CaretInfo()
		{
		}
	}
}
