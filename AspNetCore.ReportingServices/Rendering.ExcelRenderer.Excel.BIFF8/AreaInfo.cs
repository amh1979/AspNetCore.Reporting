using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class AreaInfo
	{
		private ushort m_FirstRow;

		private ushort m_LastRow;

		private ushort m_FirstColumn;

		private ushort m_LastColumn;

		internal AreaInfo(ushort firstRow, ushort lastRow, ushort firstCol, ushort lastCol)
		{
			this.m_FirstRow = firstRow;
			this.m_LastRow = lastRow;
			this.m_FirstColumn = firstCol;
			this.m_LastColumn = lastCol;
		}

		internal AreaInfo(int firstRow, int lastRow, int firstCol, int lastCol)
			: this((ushort)firstRow, (ushort)lastRow, (ushort)firstCol, (ushort)lastCol)
		{
		}

		internal void WriteToStream(BinaryWriter output)
		{
			output.Write(this.m_FirstRow);
			output.Write(this.m_LastRow);
			output.Write(this.m_FirstColumn);
			output.Write(this.m_LastColumn);
		}
	}
}
