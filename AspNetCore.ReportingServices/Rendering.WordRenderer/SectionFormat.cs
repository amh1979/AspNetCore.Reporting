using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class SectionFormat : Format
	{
		private const int OneSectionSize = 38;

		private List<int> m_cpOffsets = new List<int>();

		private bool m_useTitlePage;

		internal bool UseTitlePage
		{
			get
			{
				return this.m_useTitlePage;
			}
			set
			{
				this.m_useTitlePage = value;
			}
		}

		internal int SectionCount
		{
			get
			{
				return this.m_cpOffsets.Count;
			}
		}

		internal SectionFormat()
			: base(38, 0)
		{
		}

		internal void EndSection(int cpOffset)
		{
			this.m_cpOffsets.Add(cpOffset);
		}

		internal void WriteTo(BinaryWriter tableWriter, BinaryWriter mainWriter, int lastCp)
		{
			tableWriter.Write(0);
			int count = this.m_cpOffsets.Count;
			for (int i = 0; i < count; i++)
			{
				tableWriter.Write(this.m_cpOffsets[i]);
			}
			tableWriter.Write(lastCp);
			SprmBuffer sprmBuffer = base.m_grpprl;
			if (this.UseTitlePage)
			{
				SprmBuffer sprmBuffer2 = (SprmBuffer)base.m_grpprl.Clone();
				sprmBuffer2.AddSprm(12298, 1, null);
				sprmBuffer = sprmBuffer2;
			}
			for (int j = 0; j < count + 1; j++)
			{
				tableWriter.Write(new byte[2]);
				tableWriter.Write((int)mainWriter.BaseStream.Position);
				tableWriter.Write(new byte[6]);
				if (j == 1)
				{
					sprmBuffer = base.m_grpprl;
					base.AddSprm(12297, 0, null);
				}
				mainWriter.Write((short)sprmBuffer.Offset);
				mainWriter.Write(sprmBuffer.Buf);
			}
		}
	}
}
