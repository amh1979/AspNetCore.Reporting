using System;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.Utilities
{
	internal sealed class OfficeImageHasher
	{
		private int m_a = 1732584193;

		private int m_b = -271733879;

		private int m_c = -1732584194;

		private int m_d = 271733878;

		private int[] m_dd;

		private int m_numwords;

		internal byte[] Hash
		{
			get
			{
				byte[] array = new byte[16];
				Array.Copy(BitConverter.GetBytes(this.m_a), 0, array, 0, 4);
				Array.Copy(BitConverter.GetBytes(this.m_b), 0, array, 4, 4);
				Array.Copy(BitConverter.GetBytes(this.m_c), 0, array, 8, 4);
				Array.Copy(BitConverter.GetBytes(this.m_d), 0, array, 12, 4);
				return array;
			}
		}

		internal OfficeImageHasher(byte[] inputBuffer)
		{
			this.Mdinit(inputBuffer);
			this.Calc();
		}

		internal OfficeImageHasher(Stream inputStream)
		{
			this.Mdinit(inputStream);
			this.Calc();
		}

		internal void Mdinit(byte[] inputBuffer)
		{
			long num = inputBuffer.Length * 8;
			int num2 = inputBuffer.Length % 64;
			int num3 = (num2 >= 56) ? (64 - num2 + 64) : (64 - num2);
			int num4 = inputBuffer.Length + num3;
			byte[] array = new byte[num4];
			Array.Copy(inputBuffer, array, inputBuffer.Length);
			array[inputBuffer.Length] = 128;
			for (int i = 0; i < 8; i++)
			{
				array[num4 - 8 + i] = (byte)(num & 0xFF);
				num >>= 8;
			}
			this.m_numwords = num4 / 4;
			this.m_dd = new int[this.m_numwords];
			for (int i = 0; i < num4; i += 4)
			{
				this.m_dd[i / 4] = (array[i] & 0xFF) + ((array[i + 1] & 0xFF) << 8) + ((array[i + 2] & 0xFF) << 16) + ((array[i + 3] & 0xFF) << 24);
			}
		}

		internal void Mdinit(Stream inputStream)
		{
			long num = inputStream.Length * 8;
			int num2 = (int)(inputStream.Length % 64);
			int num3 = (num2 >= 56) ? (64 - num2 + 64) : (64 - num2);
			int num4 = (int)inputStream.Length + num3;
			byte[] array = new byte[num4];
			inputStream.Read(array, 0, (int)inputStream.Length);
			array[(int)inputStream.Length] = 128;
			for (int i = 0; i < 8; i++)
			{
				array[num4 - 8 + i] = (byte)(num & 0xFF);
				num >>= 8;
			}
			this.m_numwords = num4 / 4;
			this.m_dd = new int[this.m_numwords];
			for (int i = 0; i < num4; i += 4)
			{
				this.m_dd[i / 4] = (array[i] & 0xFF) + ((array[i + 1] & 0xFF) << 8) + ((array[i + 2] & 0xFF) << 16) + ((array[i + 3] & 0xFF) << 24);
			}
		}

		internal void Calc()
		{
			for (int i = 0; i < this.m_numwords / 16; i++)
			{
				int a = this.m_a;
				int b = this.m_b;
				int c = this.m_c;
				int d = this.m_d;
				this.Round1(i);
				this.Round2(i);
				this.Round3(i);
				this.m_a += a;
				this.m_b += b;
				this.m_c += c;
				this.m_d += d;
			}
		}

		internal static int F(int x, int y, int z)
		{
			return (x & y) | (~x & z);
		}

		internal static int G(int x, int y, int z)
		{
			return (x & y) | (x & z) | (y & z);
		}

		internal static int H(int x, int y, int z)
		{
			return x ^ y ^ z;
		}

		internal void Round1(int blk)
		{
			int num = 16 * blk;
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.F(this.m_b, this.m_c, this.m_d) + this.m_dd[num], 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.F(this.m_a, this.m_b, this.m_c) + this.m_dd[1 + num], 7);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.F(this.m_d, this.m_a, this.m_b) + this.m_dd[2 + num], 11);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.F(this.m_c, this.m_d, this.m_a) + this.m_dd[3 + num], 19);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.F(this.m_b, this.m_c, this.m_d) + this.m_dd[4 + num], 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.F(this.m_a, this.m_b, this.m_c) + this.m_dd[5 + num], 7);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.F(this.m_d, this.m_a, this.m_b) + this.m_dd[6 + num], 11);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.F(this.m_c, this.m_d, this.m_a) + this.m_dd[7 + num], 19);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.F(this.m_b, this.m_c, this.m_d) + this.m_dd[8 + num], 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.F(this.m_a, this.m_b, this.m_c) + this.m_dd[9 + num], 7);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.F(this.m_d, this.m_a, this.m_b) + this.m_dd[10 + num], 11);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.F(this.m_c, this.m_d, this.m_a) + this.m_dd[11 + num], 19);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.F(this.m_b, this.m_c, this.m_d) + this.m_dd[12 + num], 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.F(this.m_a, this.m_b, this.m_c) + this.m_dd[13 + num], 7);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.F(this.m_d, this.m_a, this.m_b) + this.m_dd[14 + num], 11);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.F(this.m_c, this.m_d, this.m_a) + this.m_dd[15 + num], 19);
		}

		internal void Round2(int blk)
		{
			int num = 16 * blk;
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.G(this.m_b, this.m_c, this.m_d) + this.m_dd[num] + 1518500249, 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.G(this.m_a, this.m_b, this.m_c) + this.m_dd[4 + num] + 1518500249, 5);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.G(this.m_d, this.m_a, this.m_b) + this.m_dd[8 + num] + 1518500249, 9);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.G(this.m_c, this.m_d, this.m_a) + this.m_dd[12 + num] + 1518500249, 13);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.G(this.m_b, this.m_c, this.m_d) + this.m_dd[1 + num] + 1518500249, 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.G(this.m_a, this.m_b, this.m_c) + this.m_dd[5 + num] + 1518500249, 5);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.G(this.m_d, this.m_a, this.m_b) + this.m_dd[9 + num] + 1518500249, 9);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.G(this.m_c, this.m_d, this.m_a) + this.m_dd[13 + num] + 1518500249, 13);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.G(this.m_b, this.m_c, this.m_d) + this.m_dd[2 + num] + 1518500249, 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.G(this.m_a, this.m_b, this.m_c) + this.m_dd[6 + num] + 1518500249, 5);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.G(this.m_d, this.m_a, this.m_b) + this.m_dd[10 + num] + 1518500249, 9);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.G(this.m_c, this.m_d, this.m_a) + this.m_dd[14 + num] + 1518500249, 13);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.G(this.m_b, this.m_c, this.m_d) + this.m_dd[3 + num] + 1518500249, 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.G(this.m_a, this.m_b, this.m_c) + this.m_dd[7 + num] + 1518500249, 5);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.G(this.m_d, this.m_a, this.m_b) + this.m_dd[11 + num] + 1518500249, 9);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.G(this.m_c, this.m_d, this.m_a) + this.m_dd[15 + num] + 1518500249, 13);
		}

		internal void Round3(int blk)
		{
			int num = 16 * blk;
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.H(this.m_b, this.m_c, this.m_d) + this.m_dd[num] + 1859775393, 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.H(this.m_a, this.m_b, this.m_c) + this.m_dd[8 + num] + 1859775393, 9);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.H(this.m_d, this.m_a, this.m_b) + this.m_dd[4 + num] + 1859775393, 11);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.H(this.m_c, this.m_d, this.m_a) + this.m_dd[12 + num] + 1859775393, 15);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.H(this.m_b, this.m_c, this.m_d) + this.m_dd[2 + num] + 1859775393, 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.H(this.m_a, this.m_b, this.m_c) + this.m_dd[10 + num] + 1859775393, 9);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.H(this.m_d, this.m_a, this.m_b) + this.m_dd[6 + num] + 1859775393, 11);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.H(this.m_c, this.m_d, this.m_a) + this.m_dd[14 + num] + 1859775393, 15);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.H(this.m_b, this.m_c, this.m_d) + this.m_dd[1 + num] + 1859775393, 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.H(this.m_a, this.m_b, this.m_c) + this.m_dd[9 + num] + 1859775393, 9);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.H(this.m_d, this.m_a, this.m_b) + this.m_dd[5 + num] + 1859775393, 11);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.H(this.m_c, this.m_d, this.m_a) + this.m_dd[13 + num] + 1859775393, 15);
			this.m_a = OfficeImageHasher.Rotintlft(this.m_a + OfficeImageHasher.H(this.m_b, this.m_c, this.m_d) + this.m_dd[3 + num] + 1859775393, 3);
			this.m_d = OfficeImageHasher.Rotintlft(this.m_d + OfficeImageHasher.H(this.m_a, this.m_b, this.m_c) + this.m_dd[11 + num] + 1859775393, 9);
			this.m_c = OfficeImageHasher.Rotintlft(this.m_c + OfficeImageHasher.H(this.m_d, this.m_a, this.m_b) + this.m_dd[7 + num] + 1859775393, 11);
			this.m_b = OfficeImageHasher.Rotintlft(this.m_b + OfficeImageHasher.H(this.m_c, this.m_d, this.m_a) + this.m_dd[15 + num] + 1859775393, 15);
		}

		internal int[] Getregs()
		{
			return new int[4]
			{
				this.m_a,
				this.m_b,
				this.m_c,
				this.m_d
			};
		}

		internal static int Rotintlft(int val, int numbits)
		{
			return val << numbits | (int)((uint)val >> 32 - numbits);
		}

		public override string ToString()
		{
			return OfficeImageHasher.Tohex(this.m_a) + OfficeImageHasher.Tohex(this.m_b) + OfficeImageHasher.Tohex(this.m_c) + OfficeImageHasher.Tohex(this.m_d);
		}

		internal static string Tohex(int i)
		{
			string text = "";
			for (int j = 0; j < 4; j++)
			{
				text = text + Convert.ToString(i >> 4 & 0xF, 16) + Convert.ToString(i & 0xF, 16);
				i >>= 8;
			}
			return text;
		}
	}
}
