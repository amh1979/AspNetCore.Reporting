using AspNetCore.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class ParagraphFormat : Format
	{
		private const int MaxPapSprmSize = 493;

		private List<int> m_papOffsets;

		private byte[] m_papFkp;

		private Stream m_papTable;

		private int m_papOffsetOffset;

		private int m_papGrpprlOffset;

		private int m_papFcOffset;

		private int m_parStart;

		private int m_fcStart;

		internal short StyleIndex
		{
			get
			{
				return base.m_grpprl.StyleIndex;
			}
			set
			{
				base.m_grpprl.StyleIndex = value;
			}
		}

		internal Stream Stream
		{
			get
			{
				return this.m_papTable;
			}
		}

		internal List<int> Offsets
		{
			get
			{
				return this.m_papOffsets;
			}
		}

		internal ParagraphFormat(Stream papTable, int fcStart)
			: base(493, 2)
		{
			this.m_papTable = papTable;
			this.m_fcStart = fcStart;
			this.m_papFkp = new byte[512];
			this.m_papGrpprlOffset = 510;
			this.m_papFcOffset = 4;
			this.m_papOffsetOffset = this.m_papFcOffset;
			this.m_papOffsets = new List<int>();
			this.m_papOffsets.Add(this.m_fcStart);
			LittleEndian.PutInt(this.m_papFkp, this.m_fcStart);
		}

		internal void CommitParagraph(int cpEnd, TableData currentRow, Stream dataStream)
		{
			byte[] array = base.m_grpprl.Buf;
			int num = base.m_grpprl.Offset;
			if (currentRow != null)
			{
				array = currentRow.Tapx;
				num = array.Length;
				if (num >= 488)
				{
					int param = (int)dataStream.Length;
					BinaryWriter binaryWriter = new BinaryWriter(dataStream);
					binaryWriter.Write((ushort)(array.Length - 2));
					binaryWriter.Write(array, 2, array.Length - 2);
					binaryWriter.Flush();
					SprmBuffer sprmBuffer = new SprmBuffer(8, 2);
					sprmBuffer.AddSprm(26182, param, null);
					array = sprmBuffer.Buf;
					num = sprmBuffer.Offset;
				}
			}
			if (!this.AddPropToPapFkp(this.m_papFkp, cpEnd, this.m_papFcOffset, new byte[13], this.m_papOffsetOffset, array, num, this.m_papGrpprlOffset))
			{
				this.m_papTable.Write(this.m_papFkp, 0, this.m_papFkp.Length);
				this.m_papOffsets.Add(this.m_parStart * 2 + this.m_fcStart);
				Array.Clear(this.m_papFkp, 0, this.m_papFkp.Length);
				LittleEndian.PutInt(this.m_papFkp, this.m_parStart * 2 + this.m_fcStart);
				this.m_papFcOffset = 4;
				this.m_papOffsetOffset = this.m_papFcOffset;
				this.m_papGrpprlOffset = 510;
				this.AddPropToPapFkp(this.m_papFkp, cpEnd, this.m_papFcOffset, new byte[13], this.m_papOffsetOffset, array, num, this.m_papGrpprlOffset);
			}
			this.m_papFkp[511]++;
			this.m_papFcOffset += 4;
			this.m_papOffsetOffset += 17;
			int num2 = num + 1;
			this.m_papGrpprlOffset = this.m_papGrpprlOffset - num2 - num2 % 2;
			this.m_parStart = cpEnd;
			base.m_grpprl.ClearStyle();
			base.m_grpprl.Reset();
		}

		private bool AddPropToPapFkp(byte[] fkp, int cpEnd, int fcOffset, byte[] midEntry, int offsetOffset, byte[] grpprl, int grpprlEnd, int grpprlOffset)
		{
			int num = 0;
			byte[] array = null;
			int num2 = grpprlEnd;
			if (grpprlEnd > 0)
			{
				if (grpprlEnd % 2 == 1)
				{
					num = grpprlEnd + 1;
					array = new byte[1];
					num2 = grpprlEnd + 1;
				}
				else
				{
					num = grpprlEnd + 2;
					array = new byte[2];
				}
			}
			int num3 = grpprlOffset - offsetOffset;
			int num4 = 4 + midEntry.Length + num;
			if (num4 > num3)
			{
				return false;
			}
			int num5 = (grpprlEnd > 0) ? (grpprlOffset - num) : 0;
			Array.Copy(fkp, fcOffset, fkp, fcOffset + 4, offsetOffset - fcOffset);
			LittleEndian.PutInt(fkp, fcOffset, cpEnd * 2 + this.m_fcStart);
			midEntry[0] = (byte)(num5 / 2);
			Array.Copy(midEntry, 0, fkp, offsetOffset + 4, midEntry.Length);
			if (grpprlEnd > 0)
			{
				Array.Copy(grpprl, 0, fkp, num5 + array.Length, grpprlEnd);
				array[array.Length - 1] = (byte)(num2 / 2);
				Array.Copy(array, 0, fkp, num5, array.Length);
			}
			return true;
		}

		internal void Finish(int lastCp)
		{
			this.m_papOffsets.Add(lastCp * 2 + this.m_fcStart);
			this.m_papTable.Write(this.m_papFkp, 0, this.m_papFkp.Length);
		}

		internal void SetIsInTable(int m_nestingLevel)
		{
			base.m_grpprl.AddSprm(9238, 1, null);
			base.m_grpprl.AddSprm(26185, m_nestingLevel, null);
		}

		internal void WriteBinTableTo(BinaryWriter tableWriter, ref int pageStart)
		{
			foreach (int papOffset in this.m_papOffsets)
			{
				tableWriter.Write(papOffset);
			}
			for (int i = 0; i < this.m_papOffsets.Count - 1; i++)
			{
				tableWriter.Write(pageStart++);
			}
		}
	}
}
