using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataCell : DataCell
	{
		private AspNetCore.ReportingServices.ReportRendering.DataCell m_renderDataCell;

		private ShimDataMember m_rowParentMember;

		private ShimDataMember m_columnParentMember;

		public override DataValueCollection DataValues
		{
			get
			{
				if (base.m_dataValues == null)
				{
					base.m_dataValues = new DataValueCollection(base.m_owner.RenderingContext, this.CachedRenderDataCell);
				}
				else if (this.m_renderDataCell == null)
				{
					base.m_dataValues.UpdateDataCellValues(this.CachedRenderDataCell);
				}
				return base.m_dataValues;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.DataCell DataCellDef
		{
			get
			{
				return null;
			}
		}

		internal override AspNetCore.ReportingServices.ReportRendering.DataCell RenderItem
		{
			get
			{
				return this.CachedRenderDataCell;
			}
		}

		private AspNetCore.ReportingServices.ReportRendering.DataCell CachedRenderDataCell
		{
			get
			{
				if (this.m_renderDataCell == null)
				{
					int memberCellIndex = this.m_rowParentMember.CurrentRenderDataMember.MemberCellIndex;
					int memberCellIndex2 = this.m_columnParentMember.CurrentRenderDataMember.MemberCellIndex;
					this.m_renderDataCell = base.m_owner.RenderCri.CustomData.DataCells[memberCellIndex, memberCellIndex2];
				}
				return this.m_renderDataCell;
			}
		}

		internal ShimDataCell(CustomReportItem owner, int rowIndex, int colIndex, ShimDataMember rowParentMember, ShimDataMember columnParentMember)
			: base(owner, rowIndex, colIndex)
		{
			this.m_rowParentMember = rowParentMember;
			this.m_columnParentMember = columnParentMember;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			this.m_renderDataCell = null;
		}
	}
}
