using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimRenderGroups
	{
		private DataRegion.Type m_type;

		private bool m_beforeSubtotal;

		private bool m_afterSubtotal;

		private ListContentCollection m_renderListContents;

		private TableGroupCollection m_renderTableGroups;

		private MatrixMemberCollection m_renderMatrixMembers;

		private AspNetCore.ReportingServices.ReportRendering.ChartMemberCollection m_renderChartMembers;

		private AspNetCore.ReportingServices.ReportRendering.DataMemberCollection m_renderDataMembers;

		internal int Count
		{
			get
			{
				switch (this.m_type)
				{
				case DataRegion.Type.List:
					return this.m_renderListContents.Count;
				case DataRegion.Type.Table:
					return this.m_renderTableGroups.Count;
				case DataRegion.Type.Matrix:
					if (!this.m_afterSubtotal && !this.m_beforeSubtotal)
					{
						return this.m_renderMatrixMembers.Count;
					}
					return this.m_renderMatrixMembers.Count - 1;
				case DataRegion.Type.Chart:
					return this.m_renderChartMembers.Count;
				case DataRegion.Type.CustomReportItem:
					return this.m_renderDataMembers.Count;
				default:
					return 0;
				}
			}
		}

		internal int MatrixMemberCollectionCount
		{
			get
			{
				DataRegion.Type type = this.m_type;
				if (type == DataRegion.Type.Matrix)
				{
					return this.m_renderMatrixMembers.Count;
				}
				return -1;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.Group this[int index]
		{
			get
			{
				switch (this.m_type)
				{
				case DataRegion.Type.List:
					return this.m_renderListContents[index];
				case DataRegion.Type.Table:
					return this.m_renderTableGroups[index];
				case DataRegion.Type.Matrix:
					if (this.m_beforeSubtotal)
					{
						return this.m_renderMatrixMembers[index + 1];
					}
					return this.m_renderMatrixMembers[index];
				case DataRegion.Type.Chart:
					return this.m_renderChartMembers[index];
				case DataRegion.Type.CustomReportItem:
					return this.m_renderDataMembers[index];
				default:
					return null;
				}
			}
		}

		internal ShimRenderGroups(ListContentCollection renderGroups)
		{
			this.m_type = DataRegion.Type.List;
			this.m_renderListContents = renderGroups;
		}

		internal ShimRenderGroups(TableGroupCollection renderGroups)
		{
			this.m_type = DataRegion.Type.Table;
			this.m_renderTableGroups = renderGroups;
		}

		internal ShimRenderGroups(MatrixMemberCollection renderGroups, bool beforeSubtotal, bool afterSubtotal)
		{
			this.m_type = DataRegion.Type.Matrix;
			this.m_renderMatrixMembers = renderGroups;
			this.m_beforeSubtotal = beforeSubtotal;
			this.m_afterSubtotal = afterSubtotal;
		}

		internal ShimRenderGroups(AspNetCore.ReportingServices.ReportRendering.ChartMemberCollection renderGroups)
		{
			this.m_type = DataRegion.Type.Chart;
			this.m_renderChartMembers = renderGroups;
		}

		internal ShimRenderGroups(AspNetCore.ReportingServices.ReportRendering.DataMemberCollection renderGroups)
		{
			this.m_type = DataRegion.Type.CustomReportItem;
			this.m_renderDataMembers = renderGroups;
		}
	}
}
