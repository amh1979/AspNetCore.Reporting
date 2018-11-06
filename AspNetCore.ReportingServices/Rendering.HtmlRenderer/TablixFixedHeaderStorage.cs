using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class TablixFixedHeaderStorage
	{
		private string m_bodyId;

		private string m_htmlId;

		private string m_lastRowGroupCol = "";

		private int m_firstRowGroupColIndex = 1;

		private List<string> m_rowHeaders;

		private List<string> m_columnHeaders;

		private List<string> m_cornerHeaders;

		internal string BodyID
		{
			get
			{
				return this.m_bodyId;
			}
			set
			{
				this.m_bodyId = value;
			}
		}

		internal string HtmlId
		{
			get
			{
				return this.m_htmlId;
			}
			set
			{
				this.m_htmlId = value;
			}
		}

		public List<string> RowHeaders
		{
			get
			{
				return this.m_rowHeaders;
			}
			set
			{
				this.m_rowHeaders = value;
			}
		}

		public bool HasEmptyCol
		{
			get
			{
				return this.m_firstRowGroupColIndex == 2;
			}
			set
			{
				if (value)
				{
					this.m_firstRowGroupColIndex = 2;
				}
				else
				{
					this.m_firstRowGroupColIndex = 1;
				}
			}
		}

		public string FirstRowGroupCol
		{
			get
			{
				if (this.m_rowHeaders == null)
				{
					return "";
				}
				return this.m_rowHeaders[this.m_firstRowGroupColIndex];
			}
		}

		public string LastRowGroupCol
		{
			get
			{
				return this.m_lastRowGroupCol;
			}
			set
			{
				this.m_lastRowGroupCol = value;
			}
		}

		public string LastColGroupRow
		{
			get
			{
				if (this.m_columnHeaders == null)
				{
					return "";
				}
				return this.m_columnHeaders[this.m_columnHeaders.Count - 1];
			}
		}

		public List<string> ColumnHeaders
		{
			get
			{
				return this.m_columnHeaders;
			}
			set
			{
				this.m_columnHeaders = value;
			}
		}

		public List<string> CornerHeaders
		{
			get
			{
				return this.m_cornerHeaders;
			}
			set
			{
				this.m_cornerHeaders = value;
			}
		}
	}
}
