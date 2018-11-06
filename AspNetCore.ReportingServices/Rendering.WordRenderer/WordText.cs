using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class WordText
	{
		private int m_currentCp;

		private Stream m_textPiece;

		internal int CurrentCp
		{
			get
			{
				return this.m_currentCp;
			}
		}

		internal Stream Stream
		{
			get
			{
				return this.m_textPiece;
			}
		}

		internal WordText(Stream textPiece)
		{
			this.m_textPiece = textPiece;
		}

		internal static char[] EscapeText(string text, out int length)
		{
			length = 0;
			char[] array = new char[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '\n':
					array[length++] = '\v';
					break;
				case '\u0001':
				case '\u0002':
				case '\u0003':
				case '\u0004':
				case '\u0005':
				case '\u0006':
				case '\a':
				case '\b':
				case '\t':
				case '\v':
				case '\f':
				case '\u000e':
				case '\u000f':
				case '\u0010':
				case '\u0011':
				case '\u0012':
				case '\u0013':
				case '\u0014':
				case '\u0015':
				case '\u0016':
				case '\u0017':
				case '\u0018':
				case '\u0019':
				case '\u001a':
				case '\u001b':
				case '\u001c':
				case '\u001d':
				case '\u001e':
				case '\u001f':
					array[length++] = ' ';
					break;
				default:
					array[length++] = text[i];
					break;
				case '\r':
					break;
				}
			}
			return array;
		}

		internal void WriteText(string text)
		{
			if (text != null)
			{
				int num = 0;
				char[] chars = WordText.EscapeText(text, out num);
				byte[] bytes = Encoding.Unicode.GetBytes(chars, 0, num);
				this.m_textPiece.Write(bytes, 0, bytes.Length);
				this.m_currentCp += num;
			}
		}

		internal void WriteSpecialText(string text)
		{
			if (text != null)
			{
				byte[] bytes = Encoding.Unicode.GetBytes(text);
				this.m_textPiece.Write(bytes, 0, bytes.Length);
				this.m_currentCp += text.Length;
			}
		}

		internal void WriteClxTo(BinaryWriter tableWriter, int fcStart)
		{
			tableWriter.Write((byte)2);
			tableWriter.Write(16);
			tableWriter.Write(0);
			tableWriter.Write((int)(this.m_textPiece.Length / 2));
			tableWriter.Write((ushort)64);
			tableWriter.Write(fcStart);
			tableWriter.Write((short)0);
		}
	}
}
