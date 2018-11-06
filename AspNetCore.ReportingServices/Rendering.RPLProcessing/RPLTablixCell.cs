namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLTablixCell
	{
		private IRPLItemFactory m_cellElement;

		private byte m_cellElementState;

		private RPLSizes m_contentSizes;

		protected int m_colSpan = 1;

		private int m_columnIndex = -1;

		private int m_rowIndex = -1;

		public int ColSpan
		{
			get
			{
				return this.m_colSpan;
			}
			set
			{
				this.m_colSpan = value;
			}
		}

		public virtual int RowSpan
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		public int ColIndex
		{
			get
			{
				return this.m_columnIndex;
			}
			set
			{
				this.m_columnIndex = value;
			}
		}

		public int RowIndex
		{
			get
			{
				return this.m_rowIndex;
			}
			set
			{
				this.m_rowIndex = value;
			}
		}

		public RPLItem Element
		{
			get
			{
				if (this.m_cellElement != null)
				{
					return this.m_cellElement.GetRPLItem();
				}
				return null;
			}
			set
			{
				this.m_cellElement = value;
			}
		}

		public RPLSizes ContentSizes
		{
			get
			{
				return this.m_contentSizes;
			}
			set
			{
				this.m_contentSizes = value;
			}
		}

		public byte ElementState
		{
			get
			{
				return this.m_cellElementState;
			}
			set
			{
				this.m_cellElementState = value;
			}
		}

		internal RPLTablixCell()
		{
		}

		internal RPLTablixCell(RPLItem element, byte elementState)
		{
			this.m_cellElement = element;
			this.m_cellElementState = elementState;
		}

		internal void SetOffset(long offset, RPLContext context)
		{
			if (offset >= 0)
			{
				this.m_cellElement = new OffsetItemInfo(offset, context);
			}
		}
	}
}
