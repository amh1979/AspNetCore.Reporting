using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal struct CellRenderingDetails
	{
		private BinaryWriter m_Writer;

		private int m_row;

		private short m_col;

		private ushort m_ixfe;

		internal BinaryWriter Output
		{
			get
			{
				return this.m_Writer;
			}
		}

		internal int Row
		{
			get
			{
				return this.m_row;
			}
		}

		internal short Column
		{
			get
			{
				return this.m_col;
			}
		}

		internal ushort Ixfe
		{
			get
			{
				return this.m_ixfe;
			}
		}

		internal void Initialize(BinaryWriter writer, int row, short col, ushort ixfe)
		{
			this.m_Writer = writer;
			this.m_row = row;
			this.m_col = col;
			this.m_ixfe = ixfe;
		}
	}
}
