using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.RichText
{
	internal class RTSelectionHighlight
	{
		private TextBoxContext m_selectionStart;

		private TextBoxContext m_selectionEnd;

		private bool m_allowColorInversion = true;

		private Color m_color = SystemColors.Highlight;

		internal TextBoxContext SelectionStart
		{
			get
			{
				return this.m_selectionStart;
			}
			set
			{
				this.m_selectionStart = value;
			}
		}

		internal TextBoxContext SelectionEnd
		{
			get
			{
				return this.m_selectionEnd;
			}
			set
			{
				this.m_selectionEnd = value;
			}
		}

		internal Color Color
		{
			get
			{
				return this.m_color;
			}
			set
			{
				this.m_color = value;
			}
		}

		internal bool AllowColorInversion
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

		internal RTSelectionHighlight(TextBoxContext Start, TextBoxContext End, Color Color)
		{
			this.m_selectionStart = Start;
			this.m_selectionEnd = End;
			this.m_color = Color;
		}

		internal RTSelectionHighlight(TextBoxContext Start, TextBoxContext End)
		{
			this.m_selectionStart = Start;
			this.m_selectionEnd = End;
		}

		public override string ToString()
		{
			return "Start: " + this.m_selectionStart.ToString() + "\r\nEnd:  " + this.m_selectionEnd.ToString();
		}
	}
}
