using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class ListData
	{
		private const short NilStyle = 4095;

		private int m_lsid;

		internal ListLevelOnFile[] m_levels;

		internal virtual int Lsid
		{
			get
			{
				return this.m_lsid;
			}
		}

		internal virtual ListLevelOnFile[] Levels
		{
			get
			{
				return this.m_levels;
			}
		}

		internal ListData(int listID)
		{
			this.m_lsid = listID;
			this.m_levels = new ListLevelOnFile[9];
		}

		internal virtual void SetLevel(int index, ListLevelOnFile level)
		{
			this.m_levels[index] = level;
		}

		internal void Write(BinaryWriter dataWriter, BinaryWriter levelWriter, Word97Writer writer)
		{
			dataWriter.Write(this.m_lsid);
			dataWriter.Write(this.m_lsid);
			for (int i = 0; i < 9; i++)
			{
				dataWriter.Write((short)4095);
			}
			dataWriter.Write((short)0);
			for (int j = 0; j < this.m_levels.Length; j++)
			{
				if (this.m_levels[j] == null)
				{
					this.m_levels[j] = new ListLevelOnFile(j, RPLFormat.ListStyles.Numbered, writer);
				}
				this.m_levels[j].Write(levelWriter);
			}
		}
	}
}
