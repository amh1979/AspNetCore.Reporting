using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class StringWrapperBIFF8
	{
		private const byte m_fHighByte = 1;

		private const byte m_fExtSt = 4;

		private const byte m_fRichSt = 8;

		private byte m_grbit = 1;

		private string m_rgb;

		private List<Pair<int, int>> m_runsList;

		private int m_hash;

		internal int DataSize
		{
			get
			{
				int characterSize = this.CharacterSize;
				return characterSize + this.FinalDataSize;
			}
		}

		internal int CharacterSize
		{
			get
			{
				if ((this.m_grbit & 1) == 0)
				{
					return this.m_rgb.Length;
				}
				return this.m_rgb.Length * 2;
			}
		}

		internal int FinalDataSize
		{
			get
			{
				return this.FormatRunsDataSize;
			}
		}

		internal int FormatRunsDataSize
		{
			get
			{
				int result = 0;
				if (this.m_runsList != null)
				{
					result = (((this.m_grbit & 8) != 0) ? (this.m_runsList.Count * 4) : 0);
				}
				return result;
			}
		}

		internal int Size
		{
			get
			{
				return this.HeaderSize + this.DataSize;
			}
		}

		internal int HeaderSize
		{
			get
			{
				int num = 3;
				if ((this.m_grbit & 8) != 0)
				{
					num += 2;
				}
				return num;
			}
		}

		internal byte[] FinalData
		{
			get
			{
				byte[] array = new byte[this.FinalDataSize];
				byte[] formatRunsData = this.FormatRunsData;
				Array.Copy(formatRunsData, array, formatRunsData.Length);
				return array;
			}
		}

		internal byte[] FormatRunsData
		{
			get
			{
				int formatRunsDataSize = this.FormatRunsDataSize;
				if (formatRunsDataSize > 0)
				{
					byte[] array = new byte[formatRunsDataSize];
					int num = 0;
					for (int i = 0; i < this.m_runsList.Count; i++)
					{
						Pair<int, int> pair = this.m_runsList[i];
						LittleEndianHelper.WriteShortU(pair.First, array, num);
						LittleEndianHelper.WriteShortU(pair.Second, array, num + 2);
						num += 4;
					}
					return array;
				}
				return new byte[0];
			}
		}

		internal string String
		{
			get
			{
				return this.m_rgb;
			}
		}

		internal StringChunkInfo ChunkInfo
		{
			get
			{
				StringChunkInfo stringChunkInfo = new StringChunkInfo();
				stringChunkInfo.CharsTotal = this.m_rgb.Length;
				return stringChunkInfo;
			}
		}

		internal bool FirstChunkCompressed
		{
			get
			{
				return (this.m_grbit & 1) == 0;
			}
		}

		internal int RunCount
		{
			get
			{
				if (this.m_runsList == null)
				{
					return 0;
				}
				return this.m_runsList.Count;
			}
		}

		internal int Cch
		{
			get
			{
				return this.m_rgb.Length;
			}
		}

		internal StringWrapperBIFF8(string aStr)
		{
			this.m_rgb = aStr;
		}

		public override int GetHashCode()
		{
			if (this.m_hash == 0)
			{
				this.m_hash = this.m_rgb.GetHashCode();
				this.m_hash <<= 3;
				if (this.m_runsList != null)
				{
					this.m_hash |= this.m_runsList.Count;
				}
			}
			return this.m_hash;
		}

		public override bool Equals(object o)
		{
			StringWrapperBIFF8 stringWrapperBIFF = (StringWrapperBIFF8)o;
			if (this.m_runsList == null != (stringWrapperBIFF.m_runsList == null))
			{
				return false;
			}
			if ((stringWrapperBIFF.m_grbit & 0xFE) != (this.m_grbit & 0xFE))
			{
				return false;
			}
			if (stringWrapperBIFF.m_rgb.Length != this.m_rgb.Length)
			{
				return false;
			}
			if (string.Compare(stringWrapperBIFF.m_rgb, this.m_rgb, StringComparison.Ordinal) != 0)
			{
				return false;
			}
			if (this.m_runsList != null && this.m_runsList.Count > 0)
			{
				for (int i = 0; i < this.m_runsList.Count; i++)
				{
					Pair<int, int> pair = stringWrapperBIFF.m_runsList[i];
					Pair<int, int> pair2 = this.m_runsList[i];
					if (pair.First != pair2.First || pair.Second != pair2.Second)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override string ToString()
		{
			return this.String;
		}

		internal void SetRunsList(List<Pair<int, int>> value)
		{
			this.m_hash = 0;
			this.m_runsList = value;
			if (this.m_runsList != null && this.m_runsList.Count > 0)
			{
				this.m_grbit |= 8;
			}
			else
			{
				this.m_grbit &= 247;
			}
		}

		internal void WriteHeaderData(Stream aOut, bool aCompressed)
		{
			if (aCompressed)
			{
				this.m_grbit &= 254;
			}
			else
			{
				this.m_grbit |= 1;
			}
			LittleEndianHelper.WriteShortU(aOut, this.m_rgb.Length);
			aOut.WriteByte(this.m_grbit);
			if (this.m_runsList != null)
			{
				if ((this.m_grbit & 8) != 0)
				{
					LittleEndianHelper.WriteShortU(aOut, this.m_runsList.Count);
				}
			}
			else if ((this.m_grbit & 8) != 0)
			{
				LittleEndianHelper.WriteShortU(aOut, 0);
			}
		}

		internal byte[] GetCharacterData(StringChunkInfo aChunkInfo, int aBytesAvailable)
		{
			if (aBytesAvailable <= 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return aChunkInfo.Data;
			}
			int num = Math.Min(this.Cch - aChunkInfo.CharPos, aBytesAvailable);
			if (num == 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return aChunkInfo.Data;
			}
			string text = this.String.Substring(aChunkInfo.CharPos, aChunkInfo.CharPos + num - aChunkInfo.CharPos);
			if (StringUtil.CanCompress(text))
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = StringUtil.DecodeTo1Byte(text);
				aChunkInfo.CharPos += num;
				return aChunkInfo.Data;
			}
			int num2 = Math.Min(this.Cch - aChunkInfo.CharPos, aBytesAvailable / 2);
			byte[] data;
			if (num2 != 0)
			{
				text = text.Substring(0, num2);
				data = StringUtil.DecodeTo2ByteLE(text);
			}
			else
			{
				data = new byte[0];
			}
			aChunkInfo.Compressed = false;
			aChunkInfo.Data = data;
			aChunkInfo.CharPos += num2;
			return aChunkInfo.Data;
		}

		internal void PrepareWriteCharacterData(StringChunkInfo aChunkInfo, int aBytesAvailable)
		{
			if (aBytesAvailable <= 0)
			{
				aChunkInfo.Compressed = true;
			}
			else
			{
				int num = Math.Min(this.Cch - aChunkInfo.CharPos, aBytesAvailable);
				if (num == 0)
				{
					aChunkInfo.Compressed = true;
				}
				else
				{
					aChunkInfo.Compressed = StringUtil.CanCompress(this.m_rgb, aChunkInfo.CharPos, num);
				}
			}
		}

		internal int WriteCharacterData(Stream aOut, StringChunkInfo aChunkInfo, int aBytesAvailable)
		{
			if (aBytesAvailable <= 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return 0;
			}
			int num = Math.Min(this.Cch - aChunkInfo.CharPos, aBytesAvailable);
			if (num == 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return 0;
			}
			if (aChunkInfo.Compressed)
			{
				StringUtil.DecodeTo1Byte(aOut, this.m_rgb, aChunkInfo.CharPos, num);
				aChunkInfo.CharPos += num;
				return num;
			}
			int num2 = Math.Min(this.Cch - aChunkInfo.CharPos, aBytesAvailable / 2);
			if (num2 != 0)
			{
				StringUtil.DecodeTo2ByteLE(aOut, this.m_rgb, aChunkInfo.CharPos, num2);
			}
			aChunkInfo.CharPos += num2;
			return num2 * 2;
		}

		internal int GetCharacterDataSize(StringChunkInfo aChunkInfo, int aBytesAvailable)
		{
			if (aBytesAvailable <= 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return 0;
			}
			int num = Math.Min(this.Cch - aChunkInfo.CharPos, aBytesAvailable);
			if (num == 0)
			{
				aChunkInfo.Compressed = true;
				aChunkInfo.Data = null;
				return 0;
			}
			if (aChunkInfo.Compressed)
			{
				aChunkInfo.CharPos += num;
				return num;
			}
			int num2 = Math.Min(this.Cch - aChunkInfo.CharPos, aBytesAvailable / 2);
			aChunkInfo.CharPos += num2;
			return num2 * 2;
		}
	}
}
