using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal class RPLTablixRow
	{
		protected List<RPLTablixCell> m_cells;

		public RPLTablixCell this[int index]
		{
			get
			{
				return this.m_cells[index];
			}
			set
			{
				this.m_cells[index] = value;
			}
		}

		public virtual int HeaderStart
		{
			get
			{
				return -1;
			}
		}

		public virtual int BodyStart
		{
			get
			{
				return 0;
			}
		}

		public virtual List<RPLTablixMemberCell> OmittedHeaders
		{
			get
			{
				return null;
			}
		}

		public int NumCells
		{
			get
			{
				if (this.m_cells == null)
				{
					return 0;
				}
				return this.m_cells.Count;
			}
		}

		internal List<RPLTablixCell> RowCells
		{
			get
			{
				return this.m_cells;
			}
		}

		internal RPLTablixRow()
		{
			this.m_cells = new List<RPLTablixCell>();
		}

		internal RPLTablixRow(List<RPLTablixCell> cells)
		{
			this.m_cells = cells;
		}

		internal virtual void SetHeaderStart()
		{
		}

		internal virtual void SetBodyStart()
		{
		}

		internal virtual void AddOmittedHeader(RPLTablixMemberCell cell)
		{
		}

		internal void AddCells(List<RPLTablixCell> cells)
		{
			if (cells != null)
			{
				if (this.m_cells == null || this.m_cells.Count == 0)
				{
					this.m_cells = cells;
				}
				else if (this.m_cells.Count < cells.Count)
				{
					cells.InsertRange(0, this.m_cells);
					this.m_cells = cells;
				}
				else
				{
					for (int i = 0; i < cells.Count; i++)
					{
						this.m_cells.Add(cells[i]);
					}
				}
			}
		}
	}
}
