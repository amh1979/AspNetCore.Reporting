using AspNetCore.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class CharacterFormat : Format
	{
		private const int MaxChpSprmSize = 503;

		private int m_chpOffsetOffset;

		private int m_chpGrpprlOffset;

		private Stream m_chpTable;

		private List<int> m_chpOffsets;

		private byte[] m_chpFkp;

		private Stack<SprmBuffer> m_formatStack;

		private int m_chpFcOffset;

		private int m_fcStart;

		internal Stream Stream
		{
			get
			{
				return this.m_chpTable;
			}
		}

		internal List<int> Offsets
		{
			get
			{
				return this.m_chpOffsets;
			}
		}

		internal CharacterFormat(Stream chpTable, int fcStart)
			: base(503, 0)
		{
			this.m_chpTable = chpTable;
			this.m_fcStart = fcStart;
			this.m_chpFkp = new byte[512];
			this.m_chpGrpprlOffset = 510;
			this.m_chpFcOffset = 4;
			this.m_chpOffsetOffset = this.m_chpFcOffset;
			this.m_chpOffsets = new List<int>();
			this.m_chpOffsets.Add(this.m_fcStart);
			this.m_formatStack = new Stack<SprmBuffer>(1);
			LittleEndian.PutInt(this.m_chpFkp, this.m_fcStart);
		}

		internal void CommitLastCharacterRun(int cpStart, int cpEnd)
		{
			byte[] buf = base.m_grpprl.Buf;
			int offset = base.m_grpprl.Offset;
			if (!this.AddPropToChpFkp(this.m_chpFkp, cpEnd, this.m_chpFcOffset, new byte[1], this.m_chpOffsetOffset, buf, offset, this.m_chpGrpprlOffset))
			{
				this.m_chpTable.Write(this.m_chpFkp, 0, this.m_chpFkp.Length);
				this.m_chpOffsets.Add(cpStart * 2 + this.m_fcStart);
				Array.Clear(this.m_chpFkp, 0, this.m_chpFkp.Length);
				LittleEndian.PutInt(this.m_chpFkp, cpStart * 2 + this.m_fcStart);
				this.m_chpFcOffset = 4;
				this.m_chpOffsetOffset = this.m_chpFcOffset;
				this.m_chpGrpprlOffset = 510;
				this.AddPropToChpFkp(this.m_chpFkp, cpEnd, this.m_chpFcOffset, new byte[1], this.m_chpOffsetOffset, buf, offset, this.m_chpGrpprlOffset);
			}
			this.m_chpFkp[511]++;
			this.m_chpFcOffset += 4;
			this.m_chpOffsetOffset += 5;
			int num = base.m_grpprl.Offset + 1;
			this.m_chpGrpprlOffset = this.m_chpGrpprlOffset - num - num % 2;
			base.m_grpprl.Reset();
		}

		private bool AddPropToChpFkp(byte[] fkp, int cpEnd, int fcOffset, byte[] midEntry, int offsetOffset, byte[] grpprl, int grpprlEnd, int grpprlOffset)
		{
			int num = (grpprlEnd > 0) ? (grpprlEnd + 1) : 0;
			int num2 = grpprlOffset - offsetOffset;
			int num3 = 4 + midEntry.Length + num + num % 2;
			if (num3 > num2)
			{
				return false;
			}
			int num4 = (grpprlEnd > 0) ? (grpprlOffset - num - num % 2) : 0;
			Array.Copy(fkp, fcOffset, fkp, fcOffset + 4, offsetOffset - fcOffset);
			LittleEndian.PutInt(fkp, fcOffset, cpEnd * 2 + this.m_fcStart);
			midEntry[0] = (byte)(num4 / 2);
			Array.Copy(midEntry, 0, fkp, offsetOffset + 4, midEntry.Length);
			if (grpprlEnd > 0)
			{
				Array.Copy(grpprl, 0, fkp, num4 + 1, grpprlEnd);
				fkp[num4] = (byte)grpprlEnd;
			}
			return true;
		}

		internal void Finish(int lastCp)
		{
			this.m_chpOffsets.Add(lastCp * 2 + this.m_fcStart);
			this.m_chpTable.Write(this.m_chpFkp, 0, this.m_chpFkp.Length);
		}

		internal void Push(int bufSize)
		{
			this.m_formatStack.Push(base.m_grpprl);
			base.m_grpprl = new SprmBuffer(bufSize, 0);
		}

		internal void CopyAndPush()
		{
			this.m_formatStack.Push(base.m_grpprl);
			base.m_grpprl = (SprmBuffer)base.m_grpprl.Clone();
		}

		internal void Pop()
		{
			if (this.m_formatStack.Peek() != null)
			{
				base.m_grpprl = this.m_formatStack.Pop();
			}
		}

		internal void SetIsInlineImage(int position)
		{
			base.m_grpprl.AddSprm(2133, 1, null);
			base.m_grpprl.AddSprm(27139, position, null);
		}

		internal void WriteBinTableTo(BinaryWriter tableWriter, ref int pageStart)
		{
			foreach (int chpOffset in this.m_chpOffsets)
			{
				tableWriter.Write(chpOffset);
			}
			for (int i = 0; i < this.m_chpOffsets.Count - 1; i++)
			{
				tableWriter.Write(pageStart++);
			}
		}
	}
}
