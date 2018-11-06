using AspNetCore.ReportingServices.Rendering.Utilities;
using System;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class CellShading
	{
		internal const int ShdSize = 10;

		private byte[] m_cellShading;

		private byte[] m_cellShading2;

		private byte[] m_cellShading3;

		private byte[] m_tableShd;

		internal int SprmSize
		{
			get
			{
				return 3 + this.m_cellShading.Length + ((this.m_cellShading2 != null) ? (3 + this.m_cellShading2.Length) : 0) + ((this.m_cellShading3 != null) ? (3 + this.m_cellShading3.Length) : 0);
			}
		}

		internal CellShading(int numColumns, byte[] tableShd)
		{
			this.m_cellShading = new byte[10 * Math.Min(numColumns, 22)];
			if (numColumns > 22)
			{
				this.m_cellShading2 = new byte[10 * Math.Min(numColumns - 22, 22)];
				if (numColumns > 44)
				{
					this.m_cellShading3 = new byte[10 * Math.Min(numColumns - 44, 22)];
				}
			}
			this.m_tableShd = tableShd;
			this.InitShading();
		}

		internal byte[] ToByteArray()
		{
			byte[] array = new byte[this.SprmSize];
			int num = 0;
			num += Word97Writer.AddSprm(array, num, 54802, 0, this.m_cellShading);
			if (this.m_cellShading2 != null)
			{
				num += Word97Writer.AddSprm(array, num, 54806, 0, this.m_cellShading2);
				if (this.m_cellShading3 != null)
				{
					num += Word97Writer.AddSprm(array, num, 54796, 0, this.m_cellShading3);
				}
			}
			return array;
		}

		internal void SetCellShading(int index, int ico24)
		{
			int num = index * 10;
			byte[] data = this.m_cellShading;
			if (index >= 22)
			{
				if (index >= 44)
				{
					num = (index - 44) * 10;
					data = this.m_cellShading3;
				}
				else
				{
					num = (index - 22) * 10;
					data = this.m_cellShading2;
				}
			}
			LittleEndian.PutInt(data, num + 4, ico24);
		}

		internal void Reset()
		{
			Array.Clear(this.m_cellShading, 0, this.m_cellShading.Length);
			if (this.m_cellShading2 != null)
			{
				Array.Clear(this.m_cellShading2, 0, this.m_cellShading2.Length);
				if (this.m_cellShading3 != null)
				{
					Array.Clear(this.m_cellShading3, 0, this.m_cellShading3.Length);
				}
			}
			this.InitShading();
		}

		private void InitShading()
		{
			if (this.m_tableShd == null)
			{
				for (int i = 3; i < this.m_cellShading.Length; i += 10)
				{
					this.m_cellShading[i] = 255;
					this.m_cellShading[i + 4] = 255;
				}
				if (this.m_cellShading2 != null)
				{
					for (int j = 3; j < this.m_cellShading2.Length; j += 10)
					{
						this.m_cellShading2[j] = 255;
						this.m_cellShading2[j + 4] = 255;
					}
				}
				if (this.m_cellShading3 != null)
				{
					for (int k = 3; k < this.m_cellShading3.Length; k += 10)
					{
						this.m_cellShading3[k] = 255;
						this.m_cellShading3[k + 4] = 255;
					}
				}
			}
			else
			{
				for (int l = 0; l < this.m_cellShading.Length; l += 10)
				{
					Array.Copy(this.m_tableShd, 0, this.m_cellShading, l, 10);
				}
				if (this.m_cellShading2 != null)
				{
					for (int m = 0; m < this.m_cellShading2.Length; m += 10)
					{
						Array.Copy(this.m_tableShd, 0, this.m_cellShading2, m, 10);
					}
				}
				if (this.m_cellShading3 != null)
				{
					for (int n = 0; n < this.m_cellShading3.Length; n += 10)
					{
						Array.Copy(this.m_tableShd, 0, this.m_cellShading3, n, 10);
					}
				}
			}
		}
	}
}
