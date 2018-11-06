using AspNetCore.ReportingServices.Rendering.Utilities;
using System;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class Ffn
	{
		private int m_cbFfnM1;

		private byte m_info;

		private ushort m_wWeight;

		private byte m_chs;

		private byte m_ixchSzAlt;

		private byte[] m_panose = new byte[10];

		private byte[] m_fontSig = new byte[24];

		private char[] m_xszFfn;

		private int m_xszFfnLength;

		internal Ffn(int size, byte info, short wWeight, byte chs, byte ixchSzAlt, byte[] panose, byte[] fontSig, char[] xszFfn)
		{
			this.m_cbFfnM1 = size;
			this.m_info = info;
			this.m_wWeight = (ushort)wWeight;
			this.m_chs = chs;
			this.m_ixchSzAlt = ixchSzAlt;
			this.m_panose = panose;
			this.m_fontSig = fontSig;
			this.m_xszFfn = xszFfn;
			this.m_xszFfnLength = this.m_xszFfn.Length;
		}

		internal virtual byte[] toByteArray()
		{
			int num = 0;
			byte[] array = new byte[this.m_cbFfnM1 + 1];
			array[num] = (byte)this.m_cbFfnM1;
			num++;
			array[num] = this.m_info;
			num++;
			LittleEndian.PutUShort(array, num, this.m_wWeight);
			num += 2;
			array[num] = this.m_chs;
			num++;
			array[num] = this.m_ixchSzAlt;
			num++;
			Array.Copy(this.m_panose, 0, array, num, this.m_panose.Length);
			num += this.m_panose.Length;
			Array.Copy(this.m_fontSig, 0, array, num, this.m_fontSig.Length);
			num += this.m_fontSig.Length;
			for (int i = 0; i < this.m_xszFfn.Length; i++)
			{
				LittleEndian.PutUShort(array, num, this.m_xszFfn[i]);
				num += 2;
			}
			return array;
		}
	}
}
