namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal class PrintTitleInfo
	{
		private ushort m_externSheetIndex;

		private ushort m_currentSheetIndex;

		private ushort m_firstRow;

		private ushort m_lastRow;

		internal ushort ExternSheetIndex
		{
			get
			{
				return this.m_externSheetIndex;
			}
		}

		internal ushort CurrentSheetIndex
		{
			get
			{
				return this.m_currentSheetIndex;
			}
		}

		internal ushort FirstRow
		{
			get
			{
				return this.m_firstRow;
			}
		}

		internal ushort LastRow
		{
			get
			{
				return this.m_lastRow;
			}
		}

		internal PrintTitleInfo(ushort externSheetIndex, ushort currentSheetIndex, ushort firstRow, ushort lastRow)
		{
			this.m_externSheetIndex = externSheetIndex;
			this.m_currentSheetIndex = currentSheetIndex;
			this.m_firstRow = firstRow;
			this.m_lastRow = lastRow;
		}
	}
}
