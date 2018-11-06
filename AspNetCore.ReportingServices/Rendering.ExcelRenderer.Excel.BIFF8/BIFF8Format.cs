namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal struct BIFF8Format
	{
		private int m_ifmt;

		private string m_string;

		internal int Index
		{
			get
			{
				return this.m_ifmt;
			}
			set
			{
				this.m_ifmt = value;
			}
		}

		internal string String
		{
			get
			{
				return this.m_string;
			}
			set
			{
				this.m_string = value;
			}
		}

		internal BIFF8Format(string builtInFormat, int ifmt)
		{
			this.m_ifmt = ifmt;
			this.m_string = builtInFormat;
		}
	}
}
