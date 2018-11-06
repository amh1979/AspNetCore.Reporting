using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablixFullRow : RPLTablixRow
	{
		private int m_headerStart = -1;

		private int m_bodyStart = -1;

		private List<RPLTablixMemberCell> m_omittedHeaders;

		public override int HeaderStart
		{
			get
			{
				return this.m_headerStart;
			}
		}

		public override int BodyStart
		{
			get
			{
				return this.m_bodyStart;
			}
		}

		public override List<RPLTablixMemberCell> OmittedHeaders
		{
			get
			{
				return this.m_omittedHeaders;
			}
		}

		internal RPLTablixFullRow(int headerStart, int bodyStart)
		{
			this.m_headerStart = headerStart;
			this.m_bodyStart = bodyStart;
		}

		internal RPLTablixFullRow(List<RPLTablixCell> cells, List<RPLTablixMemberCell> omittedHeaders, int headerStart, int bodyStart)
			: base(cells)
		{
			this.m_headerStart = headerStart;
			this.m_bodyStart = bodyStart;
			this.m_omittedHeaders = omittedHeaders;
		}

		internal override void SetHeaderStart()
		{
			if (this.m_headerStart < 0)
			{
				if (base.m_cells == null)
				{
					this.m_headerStart = 0;
				}
				else
				{
					this.m_headerStart = base.m_cells.Count;
				}
			}
		}

		internal override void SetBodyStart()
		{
			if (this.m_bodyStart < 0)
			{
				if (base.m_cells == null)
				{
					this.m_bodyStart = 0;
				}
				else
				{
					this.m_bodyStart = base.m_cells.Count;
				}
			}
		}

		internal override void AddOmittedHeader(RPLTablixMemberCell cell)
		{
			if (this.m_omittedHeaders == null)
			{
				this.m_omittedHeaders = new List<RPLTablixMemberCell>();
			}
			this.m_omittedHeaders.Add(cell);
		}
	}
}
