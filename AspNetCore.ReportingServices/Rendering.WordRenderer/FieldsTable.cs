using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class FieldsTable
	{
		internal const byte StartCode = 19;

		internal const byte MiddleCode = 20;

		internal const byte EndCode = 21;

		private List<FieldInfo> m_offsets;

		internal int Size
		{
			get
			{
				return (this.m_offsets.Count + 1) * 4 + this.m_offsets.Count * 2;
			}
		}

		internal FieldsTable()
		{
			this.m_offsets = new List<FieldInfo>();
		}

		internal void Add(FieldInfo info)
		{
			this.m_offsets.Add(info);
		}

		internal void WriteTo(BinaryWriter dataWriter, int startCP, int endCP)
		{
			for (int i = 0; i < this.m_offsets.Count; i++)
			{
				dataWriter.Write(this.m_offsets[i].Offset - startCP);
			}
			dataWriter.Write(endCP);
			dataWriter.Flush();
			for (int j = 0; j < this.m_offsets.Count; j++)
			{
				this.m_offsets[j].WriteData(dataWriter);
			}
		}
	}
}
