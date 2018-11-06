using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablix : RPLItem
	{
		private int m_columnHeaderRows;

		private int m_rowHeaderColumns;

		private int m_colsBeforeRowHeaders;

		private RPLFormat.Directions m_layoutDirection;

		private float[] m_columnsWidths;

		private float[] m_rowHeights;

		private float m_contentLeft;

		private float m_contentTop;

		private bool[] m_fixedColumns;

		private byte[] m_rowsState;

		private Queue<RPLTablixRow> m_rows;

		private long m_nextRowStart = -1L;

		private RPLTablixMemberDef[] m_tablixRowMembersDef;

		private RPLTablixMemberDef[] m_tablixColMembersDef;

		public RPLFormat.Directions LayoutDirection
		{
			get
			{
				return this.m_layoutDirection;
			}
			set
			{
				this.m_layoutDirection = value;
			}
		}

		public int ColumnHeaderRows
		{
			get
			{
				return this.m_columnHeaderRows;
			}
			set
			{
				this.m_columnHeaderRows = value;
			}
		}

		public int RowHeaderColumns
		{
			get
			{
				return this.m_rowHeaderColumns;
			}
			set
			{
				this.m_rowHeaderColumns = value;
			}
		}

		public int ColsBeforeRowHeaders
		{
			get
			{
				return this.m_colsBeforeRowHeaders;
			}
			set
			{
				this.m_colsBeforeRowHeaders = value;
			}
		}

		public float ContentTop
		{
			get
			{
				return this.m_contentTop;
			}
			set
			{
				this.m_contentTop = value;
			}
		}

		public float ContentLeft
		{
			get
			{
				return this.m_contentLeft;
			}
			set
			{
				this.m_contentLeft = value;
			}
		}

		public float[] ColumnWidths
		{
			get
			{
				return this.m_columnsWidths;
			}
			set
			{
				this.m_columnsWidths = value;
			}
		}

		public float[] RowHeights
		{
			get
			{
				return this.m_rowHeights;
			}
			set
			{
				this.m_rowHeights = value;
			}
		}

		public bool[] FixedColumns
		{
			get
			{
				return this.m_fixedColumns;
			}
			set
			{
				this.m_fixedColumns = value;
			}
		}

		public byte[] RowsState
		{
			get
			{
				return this.m_rowsState;
			}
			set
			{
				this.m_rowsState = value;
			}
		}

		internal long NextRowStart
		{
			set
			{
				this.m_nextRowStart = value;
			}
		}

		internal Queue<RPLTablixRow> Rows
		{
			set
			{
				this.m_rows = value;
			}
		}

		internal RPLTablixMemberDef[] TablixRowMembersDef
		{
			get
			{
				return this.m_tablixRowMembersDef;
			}
			set
			{
				this.m_tablixRowMembersDef = value;
			}
		}

		internal RPLTablixMemberDef[] TablixColMembersDef
		{
			get
			{
				return this.m_tablixColMembersDef;
			}
			set
			{
				this.m_tablixColMembersDef = value;
			}
		}

		internal RPLTablix()
		{
			base.m_rplElementProps = new RPLItemProps();
			base.m_rplElementProps.Definition = new RPLItemPropsDef();
		}

		internal RPLTablix(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}

		internal RPLTablix(RPLItemProps rplElementProps)
			: base(rplElementProps)
		{
		}

		public bool FixedRow(int index)
		{
			if (index >= 0 && this.m_rowsState != null && index < this.m_rowsState.Length)
			{
				return (this.m_rowsState[index] & 1) > 0;
			}
			return false;
		}

		public bool SharedLayoutRow(int index)
		{
			if (index >= 0 && this.m_rowsState != null && index < this.m_rowsState.Length)
			{
				return (this.m_rowsState[index] & 2) > 0;
			}
			return false;
		}

		public bool UseSharedLayoutRow(int index)
		{
			if (index >= 0 && this.m_rowsState != null && index < this.m_rowsState.Length)
			{
				return (this.m_rowsState[index] & 4) > 0;
			}
			return false;
		}

		public float GetRowHeight(int index, int span)
		{
			float num = 0f;
			for (int i = index; i < index + span; i++)
			{
				num += this.m_rowHeights[i];
			}
			return num;
		}

		public float GetColumnWidth(int index, int span)
		{
			float num = 0f;
			for (int i = index; i < index + span; i++)
			{
				num += this.m_columnsWidths[i];
			}
			return num;
		}

		public RPLTablixRow GetNextRow()
		{
			if (this.m_rows != null)
			{
				if (this.m_rows.Count == 0)
				{
					this.m_rows = null;
					return null;
				}
				return this.m_rows.Dequeue();
			}
			if (this.m_nextRowStart >= 0)
			{
				return RPLReader.ReadTablixRow(this.m_nextRowStart, base.m_context, this.m_tablixRowMembersDef, this.m_tablixColMembersDef, ref this.m_nextRowStart);
			}
			return null;
		}

		internal void AddRow(RPLTablixRow row)
		{
			if (this.m_rows == null)
			{
				this.m_rows = new Queue<RPLTablixRow>();
			}
			this.m_rows.Enqueue(row);
		}
	}
}
