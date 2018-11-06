namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class BitField
	{
		private int m_mask;

		private int m_shiftCount;

		internal BitField(int mask)
		{
			this.m_mask = mask;
			int num = 0;
			int num2 = mask;
			if (num2 != 0)
			{
				while ((num2 & 1) == 0)
				{
					num++;
					num2 >>= 1;
				}
			}
			this.m_shiftCount = num;
		}

		internal virtual int GetValue(int holder)
		{
			return this.GetRawValue(holder) >> this.m_shiftCount;
		}

		internal virtual short GetShortValue(short holder)
		{
			return (short)this.GetValue(holder);
		}

		internal virtual int GetRawValue(int holder)
		{
			return holder & this.m_mask;
		}

		internal virtual short GetShortRawValue(short holder)
		{
			return (short)this.GetRawValue(holder);
		}

		internal virtual bool IsSet(int holder)
		{
			return (holder & this.m_mask) != 0;
		}

		internal virtual bool IsAllSet(int holder)
		{
			return (holder & this.m_mask) == this.m_mask;
		}

		internal virtual int SetValue(int holder, int value_Renamed)
		{
			return (holder & ~this.m_mask) | (value_Renamed << this.m_shiftCount & this.m_mask);
		}

		internal virtual short SetShortValue(short holder, short value_Renamed)
		{
			return (short)this.SetValue(holder, value_Renamed);
		}

		internal virtual int Clear(int holder)
		{
			return holder & ~this.m_mask;
		}

		internal virtual short ClearShort(short holder)
		{
			return (short)this.Clear(holder);
		}

		internal virtual byte ClearByte(byte holder)
		{
			return (byte)this.Clear(holder);
		}

		internal virtual int Mark(int holder)
		{
			return holder | this.m_mask;
		}

		internal virtual short SetShort(short holder)
		{
			return (short)this.Mark(holder);
		}

		internal virtual byte SetByte(byte holder)
		{
			return (byte)this.Mark(holder);
		}

		internal virtual int SetBoolean(int holder, bool flag)
		{
			if (!flag)
			{
				return this.Clear(holder);
			}
			return this.Mark(holder);
		}

		internal virtual short SetShortBoolean(short holder, bool flag)
		{
			if (!flag)
			{
				return this.ClearShort(holder);
			}
			return this.SetShort(holder);
		}

		internal virtual byte SetByteBoolean(byte holder, bool flag)
		{
			if (!flag)
			{
				return this.ClearByte(holder);
			}
			return this.SetByte(holder);
		}
	}
}
