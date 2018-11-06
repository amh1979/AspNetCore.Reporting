using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class TextBoxState
	{
		[Flags]
		private enum TextBoxInternalState : byte
		{
			Direction = 1,
			DefaultTextAlign = 2,
			WritingMode = 4,
			VerticalAlign = 0x18,
			VerticalAlignBottom = 8,
			VerticalAlignMiddle = 0x10,
			SpanPages = 0x20,
			ResetHorizontalState = 0x40,
			VerticalMode = 0x80
		}

		private TextBoxInternalState m_state;

		public byte State
		{
			get
			{
				return (byte)this.m_state;
			}
			set
			{
				this.m_state = (TextBoxInternalState)value;
			}
		}

		public RPLFormat.Directions Direction
		{
			get
			{
				if ((int)(this.m_state & TextBoxInternalState.Direction) > 0)
				{
					return RPLFormat.Directions.RTL;
				}
				return RPLFormat.Directions.LTR;
			}
			set
			{
				if (value == RPLFormat.Directions.RTL)
				{
					this.m_state |= TextBoxInternalState.Direction;
				}
				else
				{
					this.m_state &= ~TextBoxInternalState.Direction;
				}
			}
		}

		public RPLFormat.TextAlignments DefaultTextAlign
		{
			get
			{
				if ((int)(this.m_state & TextBoxInternalState.DefaultTextAlign) > 0)
				{
					return RPLFormat.TextAlignments.Right;
				}
				return RPLFormat.TextAlignments.Left;
			}
			set
			{
				if (value == RPLFormat.TextAlignments.Right)
				{
					this.m_state |= TextBoxInternalState.DefaultTextAlign;
				}
				else
				{
					this.m_state &= ~TextBoxInternalState.DefaultTextAlign;
				}
			}
		}

		public RPLFormat.WritingModes WritingMode
		{
			get
			{
				if ((int)(this.m_state & TextBoxInternalState.WritingMode) > 0)
				{
					if ((int)(this.m_state & TextBoxInternalState.VerticalMode) > 0)
					{
						return RPLFormat.WritingModes.Rotate270;
					}
					return RPLFormat.WritingModes.Vertical;
				}
				return RPLFormat.WritingModes.Horizontal;
			}
			set
			{
				if (value == RPLFormat.WritingModes.Vertical || value == RPLFormat.WritingModes.Rotate270)
				{
					this.m_state |= TextBoxInternalState.WritingMode;
					if (value == RPLFormat.WritingModes.Rotate270)
					{
						this.m_state |= TextBoxInternalState.VerticalMode;
					}
					else
					{
						this.m_state &= ~TextBoxInternalState.VerticalMode;
					}
				}
				else
				{
					this.m_state &= ~TextBoxInternalState.WritingMode;
				}
			}
		}

		public bool VerticalText
		{
			get
			{
				if (this.WritingMode != RPLFormat.WritingModes.Vertical)
				{
					return this.WritingMode == RPLFormat.WritingModes.Rotate270;
				}
				return true;
			}
		}

		public bool HorizontalText
		{
			get
			{
				return this.WritingMode == RPLFormat.WritingModes.Horizontal;
			}
		}

		public RPLFormat.VerticalAlignments VerticalAlignment
		{
			get
			{
				if ((int)(this.m_state & TextBoxInternalState.VerticalAlign) > 0)
				{
					if ((int)(this.m_state & TextBoxInternalState.VerticalAlignBottom) > 0)
					{
						return RPLFormat.VerticalAlignments.Bottom;
					}
					return RPLFormat.VerticalAlignments.Middle;
				}
				return RPLFormat.VerticalAlignments.Top;
			}
			set
			{
				this.m_state &= ~TextBoxInternalState.VerticalAlign;
				switch (value)
				{
				case RPLFormat.VerticalAlignments.Bottom:
					this.m_state |= TextBoxInternalState.VerticalAlignBottom;
					break;
				case RPLFormat.VerticalAlignments.Middle:
					this.m_state |= TextBoxInternalState.VerticalAlignMiddle;
					break;
				}
			}
		}

		public bool SpanPages
		{
			get
			{
				return (int)(this.m_state & TextBoxInternalState.SpanPages) > 0;
			}
			set
			{
				if (value)
				{
					this.m_state |= TextBoxInternalState.SpanPages;
				}
				else
				{
					this.m_state &= ~TextBoxInternalState.SpanPages;
				}
			}
		}

		public bool ResetHorizontalState
		{
			get
			{
				return (int)(this.m_state & TextBoxInternalState.ResetHorizontalState) > 0;
			}
			set
			{
				if (value)
				{
					this.m_state |= TextBoxInternalState.ResetHorizontalState;
				}
				else
				{
					this.m_state &= ~TextBoxInternalState.ResetHorizontalState;
				}
			}
		}
	}
}
