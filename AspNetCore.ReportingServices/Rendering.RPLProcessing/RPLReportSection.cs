namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLReportSection
	{
		private string m_id;

		private float m_columnSpacing;

		private int m_columnCount;

		private RPLSizes m_bodyArea;

		private RPLItemMeasurement m_header;

		private RPLItemMeasurement m_footer;

		private RPLItemMeasurement[] m_columns;

		private long m_endOffset = -1L;

		public string ID
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		public float ColumnSpacing
		{
			get
			{
				return this.m_columnSpacing;
			}
			set
			{
				this.m_columnSpacing = value;
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.m_columnCount;
			}
			set
			{
				this.m_columnCount = value;
			}
		}

		public RPLSizes BodyArea
		{
			get
			{
				return this.m_bodyArea;
			}
			set
			{
				this.m_bodyArea = value;
			}
		}

		public RPLItemMeasurement Header
		{
			get
			{
				return this.m_header;
			}
			set
			{
				this.m_header = value;
			}
		}

		public RPLItemMeasurement Footer
		{
			get
			{
				return this.m_footer;
			}
			set
			{
				this.m_footer = value;
			}
		}

		public RPLItemMeasurement[] Columns
		{
			get
			{
				return this.m_columns;
			}
			set
			{
				this.m_columns = value;
			}
		}

		internal RPLReportSection(int columns)
		{
			this.m_columns = new RPLItemMeasurement[columns];
		}

		internal RPLReportSection(long endOffset)
		{
			this.m_endOffset = endOffset;
		}
	}
}
