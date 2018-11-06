using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal class ReverseStringBuilder
	{
		private char[] m_buffer;

		private int m_pos;

		public ReverseStringBuilder()
			: this(16)
		{
		}

		public ReverseStringBuilder(int capacity)
		{
			this.m_buffer = new char[capacity];
			this.m_pos = capacity - 1;
		}

		public void Append(string str)
		{
			int length = str.Length;
			this.EnsureCapacity(length);
			for (int i = 0; i < length; i++)
			{
				this.m_buffer[this.m_pos--] = str[i];
			}
		}

		public void Append(char c)
		{
			this.EnsureCapacity(1);
			this.m_buffer[this.m_pos--] = c;
		}

		private void EnsureCapacity(int lengthNeeded)
		{
			int num = this.m_buffer.Length;
			if (this.m_pos >= 0 && num - this.m_pos >= lengthNeeded)
			{
				return;
			}
			int num2 = num - this.m_pos - 1;
			int num3 = Math.Max(lengthNeeded, num) * 2;
			int num4 = num3 - num2 - 1;
			char[] array = new char[num3];
			Array.Copy(this.m_buffer, this.m_pos + 1, array, num4 + 1, num2);
			this.m_buffer = array;
			this.m_pos = num4;
		}

		public override string ToString()
		{
			return new string(this.m_buffer, this.m_pos + 1, this.m_buffer.Length - this.m_pos - 1);
		}
	}
}
